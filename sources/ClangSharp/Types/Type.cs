// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    [DebuggerDisplay("{Handle.DebuggerDisplayString,nq}")]
    public unsafe class Type : IEquatable<Type>
    {
        private readonly Lazy<Type> _canonicalType;
        private readonly Lazy<Type> _desugar;
        private readonly Lazy<Type> _pointeeType;
        private readonly Lazy<TranslationUnit> _translationUnit;

        protected Type(CXType handle, CXTypeKind expectedKind, CX_TypeClass expectedTypeClass)
        {
            if (handle.kind != expectedKind)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }

            if ((handle.TypeClass == CX_TypeClass.CX_TypeClass_Invalid) || (handle.TypeClass != expectedTypeClass))
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }
            Handle = handle;

            _canonicalType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.CanonicalType));
            _desugar = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar));
            _pointeeType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.PointeeType));
            _translationUnit = new Lazy<TranslationUnit>(() => TranslationUnit.GetOrCreate((CXTranslationUnit)Handle.data[1]));
        }

        public CXXRecordDecl AsCXXRecordDecl => AsTagDecl as CXXRecordDecl;

        public string AsString => Handle.Spelling.ToString();

        public TagDecl AsTagDecl
        {
            get
            {
                return GetAs<TagType>() is TagType tt
                    ? tt.Decl
                    : (global::ClangSharp.TagDecl)(GetAs<InjectedClassNameType>() is InjectedClassNameType injected ? injected.Decl : null);
            }
        }

        public Type CanonicalType => _canonicalType.Value;

        public Type Desugar => _desugar.Value;

        public CXType Handle { get; }

        public bool IsAnyPointerType => IsPointerType || IsObjCObjectPointerType;

        public bool IsExtIntType => CanonicalType is ExtIntType;

        public bool IsIntegerType => (CanonicalType is BuiltinType builtinType) && builtinType.Kind is >= CXTypeKind.CXType_Bool and <= CXTypeKind.CXType_Int128;

        public bool IsIntegralOrEnumerationType
        {
            get
            {
                return CanonicalType is BuiltinType bt
                    ? bt.Kind is >= CXTypeKind.CXType_Bool and <= CXTypeKind.CXType_Int128
                    : CanonicalType is EnumType et ? et.Decl.IsComplete : IsExtIntType;
            }
        }

        public bool IsLocalConstQualified => Handle.IsConstQualified;

        public bool IsObjCObjectPointerType => CanonicalType is ObjCObjectPointerType;

        public bool IsPointerType => CanonicalType is PointerType;

        public bool IsSugared => Handle.IsSugared;

        public CXTypeKind Kind => Handle.kind;

        public string KindSpelling => Handle.KindSpelling.ToString();

        public Type PointeeType => _pointeeType.Value;

        public TranslationUnit TranslationUnit => _translationUnit.Value;

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

        public static bool operator ==(Type left, Type right) => (left is object) ? ((right is object) && (left.Handle == right.Handle)) : (right is null);

        public static bool operator !=(Type left, Type right) => (left is object) ? ((right is null) || (left.Handle != right.Handle)) : (right is object);

        internal static Type Create(CXType handle) => handle.TypeClass switch
        {
            CX_TypeClass.CX_TypeClass_Adjusted => new AdjustedType(handle),
            CX_TypeClass.CX_TypeClass_Decayed => new DecayedType(handle),
            CX_TypeClass.CX_TypeClass_ConstantArray => new ConstantArrayType(handle),
            CX_TypeClass.CX_TypeClass_DependentSizedArray => new DependentSizedArrayType(handle),
            CX_TypeClass.CX_TypeClass_IncompleteArray => new IncompleteArrayType(handle),
            CX_TypeClass.CX_TypeClass_VariableArray => new VariableArrayType(handle),
            CX_TypeClass.CX_TypeClass_Atomic => new AtomicType(handle),
            CX_TypeClass.CX_TypeClass_Attributed => new AttributedType(handle),
            CX_TypeClass.CX_TypeClass_BlockPointer => new BlockPointerType(handle),
            CX_TypeClass.CX_TypeClass_Builtin => new BuiltinType(handle),
            CX_TypeClass.CX_TypeClass_Complex => new ComplexType(handle),
            CX_TypeClass.CX_TypeClass_Decltype => new DecltypeType(handle),
            CX_TypeClass.CX_TypeClass_Auto => new AutoType(handle),
            CX_TypeClass.CX_TypeClass_DeducedTemplateSpecialization => new DeducedTemplateSpecializationType(handle),
            CX_TypeClass.CX_TypeClass_DependentAddressSpace => new DependentAddressSpaceType(handle),
            CX_TypeClass.CX_TypeClass_DependentName => new DependentNameType(handle),
            CX_TypeClass.CX_TypeClass_DependentSizedExtVector => new DependentSizedExtVectorType(handle),
            CX_TypeClass.CX_TypeClass_DependentTemplateSpecialization => new DependentTemplateSpecializationType(handle),
            CX_TypeClass.CX_TypeClass_DependentVector => new DependentVectorType(handle),
            CX_TypeClass.CX_TypeClass_Elaborated => new ElaboratedType(handle),
            CX_TypeClass.CX_TypeClass_FunctionNoProto => new FunctionNoProtoType(handle),
            CX_TypeClass.CX_TypeClass_FunctionProto => new FunctionProtoType(handle),
            CX_TypeClass.CX_TypeClass_InjectedClassName => new InjectedClassNameType(handle),
            CX_TypeClass.CX_TypeClass_MacroQualified => new MacroQualifiedType(handle),
            CX_TypeClass.CX_TypeClass_MemberPointer => new MemberPointerType(handle),
            CX_TypeClass.CX_TypeClass_ObjCObjectPointer => new ObjCObjectPointerType(handle),
            CX_TypeClass.CX_TypeClass_ObjCObject => new ObjCObjectType(handle),
            CX_TypeClass.CX_TypeClass_ObjCInterface => new ObjCInterfaceType(handle),
            CX_TypeClass.CX_TypeClass_ObjCTypeParam => new ObjCTypeParamType(handle),
            CX_TypeClass.CX_TypeClass_PackExpansion => new PackExpansionType(handle),
            CX_TypeClass.CX_TypeClass_Paren => new ParenType(handle),
            CX_TypeClass.CX_TypeClass_Pipe => new PipeType(handle),
            CX_TypeClass.CX_TypeClass_Pointer => new PointerType(handle),
            CX_TypeClass.CX_TypeClass_LValueReference => new LValueReferenceType(handle),
            CX_TypeClass.CX_TypeClass_RValueReference => new RValueReferenceType(handle),
            CX_TypeClass.CX_TypeClass_SubstTemplateTypeParmPack => new SubstTemplateTypeParmPackType(handle),
            CX_TypeClass.CX_TypeClass_SubstTemplateTypeParm => new SubstTemplateTypeParmType(handle),
            CX_TypeClass.CX_TypeClass_Enum => new EnumType(handle),
            CX_TypeClass.CX_TypeClass_Record => new RecordType(handle),
            CX_TypeClass.CX_TypeClass_TemplateSpecialization => new TemplateSpecializationType(handle),
            CX_TypeClass.CX_TypeClass_TemplateTypeParm => new TemplateTypeParmType(handle),
            CX_TypeClass.CX_TypeClass_TypeOfExpr => new TypeOfExprType(handle),
            CX_TypeClass.CX_TypeClass_TypeOf => new TypeOfType(handle),
            CX_TypeClass.CX_TypeClass_Typedef => new TypedefType(handle),
            CX_TypeClass.CX_TypeClass_UnaryTransform => new UnaryTransformType(handle),
            CX_TypeClass.CX_TypeClass_UnresolvedUsing => new UnresolvedUsingType(handle),
            CX_TypeClass.CX_TypeClass_Vector => new VectorType(handle),
            CX_TypeClass.CX_TypeClass_ExtVector => new ExtVectorType(handle),
            _ => new Type(handle, handle.kind, handle.TypeClass),            
        };

        public override bool Equals(object obj) => (obj is Type other) && Equals(other);

        public bool Equals(Type other) => this == other;

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

        public T GetAs<T>()
            where T : Type
        {
            Debug.Assert(!typeof(ArrayType).IsAssignableFrom(typeof(T)), "ArrayType cannot be used with getAs!");

            return this is T ty ? ty : CanonicalType is not T ? null : (T)UnqualifiedDesugaredType;
        }

        public override int GetHashCode() => Handle.GetHashCode();

        public override string ToString() => Handle.ToString();
    }
}
