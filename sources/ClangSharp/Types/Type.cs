// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

[DebuggerDisplay("{Handle.DebuggerDisplayString,nq}")]
public unsafe class Type : IEquatable<Type>
{
    private ValueLazy<Type, string> _asString;
    private ValueLazy<Type, Type> _canonicalType;
    private ValueLazy<Type, Type> _desugar;
    private ValueLazy<Type, string> _kindSpelling;
    private ValueLazy<Type, Type> _nonReferenceType;
    private ValueLazy<Type, Type> _pointeeType;
    private ValueLazy<Type, TranslationUnit> _translationUnit;
    private ValueLazy<Type, Type> _unqualifiedType;

    protected Type(CXType handle, CXTypeKind expectedKind, CX_TypeClass expectedTypeClass, params ReadOnlySpan<CXTypeKind> additionalExpectedKinds)
    {
        if ((handle.TypeClass == CX_TypeClass_Invalid) || (handle.TypeClass != expectedTypeClass))
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        // CXTypeKind is validated after TypeClass because libclang's CXTypeKind uses a
        // coarser classification than libClangSharp's CX_TypeClass. For example, libclang
        // may return CXType_ObjCId or CXType_Unexposed for a type that libClangSharp
        // correctly classifies as CX_TypeClass_Attributed via the Clang AST. Since
        // TypeClass is the authoritative classifier and already validated above, a
        // CXTypeKind mismatch is not fatal.
        var kindMatches = handle.kind == expectedKind
#if NET10_0_OR_GREATER
            || additionalExpectedKinds.Contains(handle.kind);
#else
            || Contains(additionalExpectedKinds, handle.kind);
#endif

        if (!kindMatches)
        {
            Debug.WriteLine($"Unexpected CXTypeKind for {handle.TypeClass}: {handle.kind}.");
        }

        Handle = handle;

        _asString = new ValueLazy<Type, string>(&AsStringFactory);
        _canonicalType = new ValueLazy<Type, Type>(&CanonicalTypeFactory);
        _desugar = new ValueLazy<Type, Type>(&DesugarFactory);
        _kindSpelling = new ValueLazy<Type, string>(&KindSpellingFactory);
        _nonReferenceType = new ValueLazy<Type, Type>(&NonReferenceTypeFactory);
        _pointeeType = new ValueLazy<Type, Type>(&PointeeTypeFactory);
        _translationUnit = new ValueLazy<Type, TranslationUnit>(&TranslationUnitFactory);
        _unqualifiedType = new ValueLazy<Type, Type>(&UnqualifiedTypeFactory);
    }

#if !NET10_0_OR_GREATER
    private static bool Contains(ReadOnlySpan<CXTypeKind> kinds, CXTypeKind find)
    {
        for (var i = 0; i < kinds.Length; i++)
        {
            if (kinds[i] == find)
            {
                return true;
            }
        }
        return false;
    }
#endif

    public uint AddressSpace => Handle.AddressSpace;

    public long AlignOf => Handle.AlignOf;

    public CXXRecordDecl? AsCXXRecordDecl => AsTagDecl as CXXRecordDecl;

    public string AsString => _asString.GetValue(this);

    public TagDecl? AsTagDecl
    {
        get
        {
            return (GetAs<TagType>() is TagType tt)
                ? tt.Decl
                : (ClangSharp.TagDecl?)((GetAs<InjectedClassNameType>() is InjectedClassNameType injected) ? injected.Decl : null);
        }
    }

    public Type CanonicalType => _canonicalType.GetValue(this);

    public Type Desugar => _desugar.GetValue(this);

    public CXType Handle { get; }

    public bool IsAnyPointerType => IsPointerType || IsObjCObjectPointerType;

    public bool IsArrayType => CanonicalType is ArrayType;

    public bool IsAtomicType => CanonicalType is AtomicType;

    public bool IsBitIntType => CanonicalType is BitIntType;

    public bool IsBlockPointerType => CanonicalType is BlockPointerType;

    public bool IsBooleanType => (CanonicalType is BuiltinType builtinType) && builtinType.Kind == CXType_Bool;

    public bool IsCharType => (CanonicalType is BuiltinType builtinType) && builtinType.Kind is CXType_Char_U or CXType_UChar or CXType_Char_S or CXType_SChar;

    public bool IsEnumeralType => CanonicalType is EnumType;

    public bool IsFunctionType => CanonicalType is FunctionType;

    public bool IsIntegerType => (CanonicalType is BuiltinType builtinType) && builtinType.Kind is >= CXType_Bool and <= CXType_Int128;

    public bool IsIntegralOrEnumerationType
    {
        get
        {
            return CanonicalType is BuiltinType bt
                ? bt.Kind is >= CXType_Bool and <= CXType_Int128
                : CanonicalType is EnumType et ? et.Decl.IsComplete : IsBitIntType;
        }
    }

    public bool IsLValueReferenceType => CanonicalType is LValueReferenceType;

    public bool IsLocalConstQualified => Handle.IsConstQualified;

    public bool IsLocalRestrictQualified => Handle.IsRestrictQualified;

    public bool IsLocalVolatileQualified => Handle.IsVolatileQualified;

    public bool IsMatrixType => CanonicalType is MatrixType;

    public bool IsMemberPointerType => CanonicalType is MemberPointerType;

    public bool IsNullPtrType => (CanonicalType is BuiltinType builtinType) && builtinType.Kind == CXType_NullPtr;

    public bool IsObjCInstanceType => Handle.IsObjCInstanceType;

    public bool IsObjCObjectPointerType => CanonicalType is ObjCObjectPointerType;

    public bool IsPODType => Handle.IsPODType;

    public bool IsPointerType => CanonicalType is PointerType;

    public bool IsRValueReferenceType => CanonicalType is RValueReferenceType;

    public bool IsRecordType => CanonicalType is RecordType;

    public bool IsReferenceType => CanonicalType is ReferenceType;

    public bool IsSugared => Handle.IsSugared;

    public bool IsVectorType => CanonicalType is VectorType or ExtVectorType;

    public bool IsVoidType => (CanonicalType is BuiltinType builtinType) && builtinType.Kind == CXType_Void;

    public CXTypeKind Kind => Handle.kind;

    public string KindSpelling => _kindSpelling.GetValue(this);

    public Type NonReferenceType => _nonReferenceType.GetValue(this);

    public CXTypeNullabilityKind Nullability => Handle.Nullability;

    public Type PointeeType => _pointeeType.GetValue(this);

    public long SizeOf => Handle.SizeOf;

    public TranslationUnit TranslationUnit => _translationUnit.GetValue(this);

    public CX_TypeClass TypeClass => Handle.TypeClass;

    public string TypeClassSpelling => Handle.TypeClassSpelling;

    public Type UnqualifiedDesugaredType
    {
        get
        {
            var cur = this;

            while (true)
            {
                if (!cur.IsSugared)
                {
                    return cur;
                }
                cur = cur.Desugar;
            }
        }
    }

    public Type UnqualifiedType => _unqualifiedType.GetValue(this);

    public static bool operator ==(Type? left, Type? right) => (left is not null) ? ((right is not null) && (left.Handle == right.Handle)) : (right is null);

    public static bool operator !=(Type? left, Type? right) => (left is not null) ? ((right is null) || (left.Handle != right.Handle)) : (right is not null);

    internal static Type Create(CXType handle) => handle.TypeClass switch {
        CX_TypeClass_Invalid => new Type(handle, handle.kind, handle.TypeClass),
        CX_TypeClass_Adjusted => new AdjustedType(handle),
        CX_TypeClass_Decayed => new DecayedType(handle),
        CX_TypeClass_ConstantArray => new ConstantArrayType(handle),
        CX_TypeClass_ArrayParameter => new ArrayParameterType(handle),
        CX_TypeClass_DependentSizedArray => new DependentSizedArrayType(handle),
        CX_TypeClass_IncompleteArray => new IncompleteArrayType(handle),
        CX_TypeClass_VariableArray => new VariableArrayType(handle),
        CX_TypeClass_Atomic => new AtomicType(handle),
        CX_TypeClass_Attributed => new AttributedType(handle),
        CX_TypeClass_BTFTagAttributed => new BTFTagAttributedType(handle),
        CX_TypeClass_BitInt => new BitIntType(handle),
        CX_TypeClass_BlockPointer => new BlockPointerType(handle),
        CX_TypeClass_CountAttributed => new CountAttributedType(handle),
        CX_TypeClass_Builtin => new BuiltinType(handle),
        CX_TypeClass_Complex => new ComplexType(handle),
        CX_TypeClass_Decltype => new DecltypeType(handle),
        CX_TypeClass_Auto => new AutoType(handle),
        CX_TypeClass_DeducedTemplateSpecialization => new DeducedTemplateSpecializationType(handle),
        CX_TypeClass_DependentAddressSpace => new DependentAddressSpaceType(handle),
        CX_TypeClass_DependentBitInt => new DependentBitIntType(handle),
        CX_TypeClass_DependentName => new DependentNameType(handle),
        CX_TypeClass_DependentSizedExtVector => new DependentSizedExtVectorType(handle),
        CX_TypeClass_DependentVector => new DependentVectorType(handle),
        CX_TypeClass_FunctionNoProto => new FunctionNoProtoType(handle),
        CX_TypeClass_FunctionProto => new FunctionProtoType(handle),
        CX_TypeClass_HLSLAttributedResource => new HLSLAttributedResourceType(handle),
        CX_TypeClass_HLSLInlineSpirv => new HLSLInlineSpirvType(handle),
        CX_TypeClass_InjectedClassName => new InjectedClassNameType(handle),
        CX_TypeClass_MacroQualified => new MacroQualifiedType(handle),
        CX_TypeClass_ConstantMatrix => new ConstantMatrixType(handle),
        CX_TypeClass_DependentSizedMatrix => new DependentSizedMatrixType(handle),
        CX_TypeClass_MemberPointer => new MemberPointerType(handle),
        CX_TypeClass_ObjCObjectPointer => new ObjCObjectPointerType(handle),
        CX_TypeClass_ObjCObject => new ObjCObjectType(handle),
        CX_TypeClass_ObjCInterface => new ObjCInterfaceType(handle),
        CX_TypeClass_ObjCTypeParam => new ObjCTypeParamType(handle),
        CX_TypeClass_PackExpansion => new PackExpansionType(handle),
        CX_TypeClass_PackIndexing => new PackIndexingType(handle),
        CX_TypeClass_Paren => new ParenType(handle),
        CX_TypeClass_Pipe => new PipeType(handle),
        CX_TypeClass_Pointer => new PointerType(handle),
        CX_TypeClass_LValueReference => new LValueReferenceType(handle),
        CX_TypeClass_RValueReference => new RValueReferenceType(handle),
        CX_TypeClass_SubstTemplateTypeParmPack => new SubstTemplateTypeParmPackType(handle),
        CX_TypeClass_SubstTemplateTypeParm => new SubstTemplateTypeParmType(handle),
        CX_TypeClass_Enum => new EnumType(handle),
        CX_TypeClass_Record => new RecordType(handle),
        CX_TypeClass_TemplateSpecialization => new TemplateSpecializationType(handle),
        CX_TypeClass_TemplateTypeParm => new TemplateTypeParmType(handle),
        CX_TypeClass_TypeOfExpr => new TypeOfExprType(handle),
        CX_TypeClass_TypeOf => new TypeOfType(handle),
        CX_TypeClass_Typedef => new TypedefType(handle),
        CX_TypeClass_UnaryTransform => new UnaryTransformType(handle),
        CX_TypeClass_UnresolvedUsing => new UnresolvedUsingType(handle),
        CX_TypeClass_Using => new UsingType(handle),
        CX_TypeClass_Vector => new VectorType(handle),
        CX_TypeClass_ExtVector => new ExtVectorType(handle),
        _ => new Type(handle, handle.kind, handle.TypeClass),
    };

    public override bool Equals(object? obj) => (obj is Type other) && Equals(other);

    public bool Equals(Type? other) => this == other;

    public T CastAs<T>()
        where T : Type
    {
        Debug.Assert(!typeof(ArrayType).IsAssignableFrom(typeof(T)), "ArrayType cannot be used with castAs!");

        if (this is T ty)
        {
            return ty;
        }

        Debug.Assert(CanonicalType is T);
        return (T)UnqualifiedDesugaredType;
    }

    public T? GetAs<T>()
        where T : Type
    {
        Debug.Assert(!typeof(ArrayType).IsAssignableFrom(typeof(T)), "ArrayType cannot be used with getAs!");

        return (this is T ty) ? ty : (CanonicalType is not T) ? null : (T)UnqualifiedDesugaredType;
    }

    public override int GetHashCode() => Handle.GetHashCode();

    public override string ToString() => AsString;

    private static unsafe TranslationUnit TranslationUnitFactory(Type self) => TranslationUnit.GetOrCreate((CXTranslationUnit)self.Handle.data[1]);

    private static unsafe Type PointeeTypeFactory(Type self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.PointeeType);

    private static unsafe Type NonReferenceTypeFactory(Type self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.NonReferenceType);

    private static unsafe Type UnqualifiedTypeFactory(Type self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.UnqualifiedType);

    private static unsafe string KindSpellingFactory(Type self) => self.Handle.KindSpelling.ToString();

    private static unsafe Type DesugarFactory(Type self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.Desugar);

    private static unsafe Type CanonicalTypeFactory(Type self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.CanonicalType);

    private static unsafe string AsStringFactory(Type self) => self.Handle.Spelling.ToString();
}
