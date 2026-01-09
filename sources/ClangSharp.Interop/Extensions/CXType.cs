// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp.Interop;

[DebuggerDisplay("{DebuggerDisplayString,nq}")]
public unsafe partial struct CXType : IEquatable<CXType>
{
    public readonly uint AddressSpace => (kind != CXType_Invalid) ? clang.getAddressSpace(this) : default;

    public readonly CXCursor AddrSpaceExpr => clangsharp.Type_getAddrSpaceExpr(this);

    public readonly CXType AdjustedType => (kind != CXType_Invalid) ? clangsharp.Type_getAdjustedType(this) : default;

    public readonly long AlignOf => clang.Type_getAlignOf(this);

    public readonly CXType ArrayElementType => clang.getArrayElementType(this);

    public readonly long ArraySize => clang.getArraySize(this);

    public readonly CX_AttrKind AttrKind => clangsharp.Type_getAttrKind(this);

    public readonly CXType BaseType => clangsharp.Type_getBaseType(this);

    public readonly CXType CanonicalType => clang.getCanonicalType(this);

    public readonly CXType ClassType => clang.Type_getClassType(this);

    public readonly CXCursor ColumnExpr => clangsharp.Type_getColumnExpr(this);

    public readonly CXRefQualifierKind CXXRefQualifier => clang.Type_getCXXRefQualifier(this);

    public readonly CXType DecayedType => clangsharp.Type_getDecayedType(this);

    public readonly CXCursor Declaration => (kind != CXType_Invalid) ? clangsharp.Type_getDeclaration(this) : default;

    public readonly CXType DeducedType => (kind != CXType_Invalid) ? clangsharp.Type_getDeducedType(this) : default;

    public readonly int Depth => clangsharp.Type_getDepth(this);

    public readonly CXType Desugar => (kind != CXType_Invalid) ? clangsharp.Type_desugar(this) : default;

    public readonly CXType ElementType => (kind != CXType_Invalid) ? clangsharp.Type_getElementType(this) :default;

    public readonly CXType EquivalentType => (kind != CXType_Invalid) ? clangsharp.Type_getEquivalentType(this) : default;

    public readonly CXCursor_ExceptionSpecificationKind ExceptionSpecificationType => (CXCursor_ExceptionSpecificationKind)clang.getExceptionSpecificationType(this);

    public readonly CXCallingConv FunctionTypeCallingConv => clang.getFunctionTypeCallingConv(this);

    public readonly int Index => clangsharp.Type_getIndex(this);

    public readonly CXType InjectedSpecializationType => (kind != CXType_Invalid) ? clangsharp.Type_getInjectedSpecializationType(this) : default;

    public readonly CXType InjectedTST => (kind != CXType_Invalid) ? clangsharp.Type_getInjectedTST(this) : default;

    public readonly bool IsCanonical => Equals(CanonicalType);

    public readonly bool IsConstQualified => clang.isConstQualifiedType(this) != 0;

    public readonly bool IsFunctionTypeVariadic => clang.isFunctionTypeVariadic(this) != 0;

    public readonly bool IsObjCInstanceType => (kind != CXType_Invalid) && clangsharp.Type_getIsObjCInstanceType(this) != 0;

    public readonly bool IsPODType => clang.isPODType(this) != 0;

    public readonly bool IsRestrictQualified => clang.isRestrictQualifiedType(this) != 0;

    public readonly bool IsSigned => clangsharp.Type_getIsSigned(this) != 0;

    public readonly bool IsSugared => (kind != CXType_Invalid) && clangsharp.Type_getIsSugared(this) != 0;

    public readonly bool IsTransparentTagTypedef => clang.Type_isTransparentTagTypedef(this) != 0;

    public readonly bool IsTypeAlias => clangsharp.Type_getIsTypeAlias(this) != 0;

    public readonly bool IsUnsigned => clangsharp.Type_getIsUnsigned(this) != 0;

    public readonly bool IsVolatileQualified => clang.isVolatileQualifiedType(this) != 0;

    public readonly CXString KindSpelling => clang.getTypeKindSpelling(kind);

    public readonly CXType ModifiedType => clangsharp.Type_getModifiedType(this);

    public readonly CXType NamedType => clang.Type_getNamedType(this);

    public readonly CXType NonReferenceType => (kind != CXType_Invalid) ? clang.getNonReferenceType(this) : default;

    public readonly CXTypeNullabilityKind Nullability => clang.Type_getNullability(this);

    public readonly int NumArgTypes => clang.getNumArgTypes(this);

    public readonly int NumBits => clangsharp.Type_getNumBits(this);

    public readonly CXCursor NumBitsExpr => clangsharp.Type_getNumBitsExpr(this);

    public readonly int NumColumns => clangsharp.Type_getNumColumns(this);

    public readonly int NumElementsFlattened => clangsharp.Type_getNumElementsFlattened(this);

    public readonly int NumRows => clangsharp.Type_getNumRows(this);

    public readonly long NumElements => clang.getNumElements(this);

    public readonly uint NumObjCProtocolRefs => clang.Type_getNumObjCProtocolRefs(this);

    public readonly uint NumObjCTypeArgs => clang.Type_getNumObjCTypeArgs(this);

    public readonly int NumTemplateArguments => clang.Type_getNumTemplateArguments(this);

    public readonly CXType ObjCObjectBaseType => clang.Type_getObjCObjectBaseType(this);

    public readonly CXType OriginalType => (kind != CXType_Invalid) ? clangsharp.Type_getOriginalType(this) : default;

    public readonly CXCursor OwnedTagDecl => (kind != CXType_Invalid) ? clangsharp.Type_getOwnedTagDecl(this) : default;

    public readonly CXType PointeeType => (kind != CXType_Invalid) ? clangsharp.Type_getPointeeType(this) : default;

    public readonly CXType ResultType => clang.getResultType(this);

    public readonly CXCursor RowExpr => (kind != CXType_Invalid) ? clangsharp.Type_getRowExpr(this) : default;

    public readonly CXCursor SizeExpr => (kind != CXType_Invalid) ? clangsharp.Type_getSizeExpr(this) : default;

    public readonly long SizeOf => clang.Type_getSizeOf(this);

    public readonly CXString Spelling => (kind != CXType_Invalid) ? clang.getTypeSpelling(this) : default;

    public CX_TemplateName TemplateName
    {
        get
        {
            var TN = clangsharp.Type_getTemplateName(this);
            TN.tu = (CXTranslationUnitImpl*)data[1];
            return TN;
        }
    }

    public readonly CX_TypeClass TypeClass => clangsharp.Type_getTypeClass(this);

    public readonly string TypeClassSpelling
    {
        get
        {
            Debug.Assert(CX_TypeClass_TypeLast == CX_TypeClass_ExtVector);
            Debug.Assert(CX_TypeClass_TagFirst == CX_TypeClass_Record);
            Debug.Assert(CX_TypeClass_TagLast == CX_TypeClass_Enum);

            return TypeClass switch {
                CX_TypeClass_Invalid => "Invalid",
                CX_TypeClass_Adjusted => "Adjusted",
                CX_TypeClass_Decayed => "Decayed",
                CX_TypeClass_ConstantArray => "ConstantArray",
                CX_TypeClass_ArrayParameter => "ArrayParameter",
                CX_TypeClass_DependentSizedArray => "DependentSizedArray",
                CX_TypeClass_IncompleteArray => "IncompleteArray",
                CX_TypeClass_VariableArray => "VariableArray",
                CX_TypeClass_Atomic => "Atomic",
                CX_TypeClass_Attributed => "Attributed",
                CX_TypeClass_BTFTagAttributed => "BTFTagAttributed",
                CX_TypeClass_BitInt => "BitInt",
                CX_TypeClass_BlockPointer => "BlockPointer",
                CX_TypeClass_CountAttributed => "CountAttributed",
                CX_TypeClass_Builtin => "Builtin",
                CX_TypeClass_Complex => "Complex",
                CX_TypeClass_Decltype => "Decltype",
                CX_TypeClass_Auto => "Auto",
                CX_TypeClass_DeducedTemplateSpecialization => "DeducedTemplateSpecialization",
                CX_TypeClass_DependentAddressSpace => "DependentAddressSpace",
                CX_TypeClass_DependentBitInt => "DependentBitInt",
                CX_TypeClass_DependentName => "DependentName",
                CX_TypeClass_DependentSizedExtVector => "DependentSizedExtVector",
                CX_TypeClass_DependentTemplateSpecialization => "DependentTemplateSpecialization",
                CX_TypeClass_DependentVector => "DependentVector",
                CX_TypeClass_Elaborated => "Elaborated",
                CX_TypeClass_FunctionNoProto => "FunctionNoProto",
                CX_TypeClass_FunctionProto => "FunctionProto",
                CX_TypeClass_HLSLAttributedResource => "HLSLAttributedResource",
                CX_TypeClass_HLSLInlineSpirv => "HLSLInlineSpirv",
                CX_TypeClass_InjectedClassName => "InjectedClassName",
                CX_TypeClass_MacroQualified => "MacroQualified",
                CX_TypeClass_ConstantMatrix => "ConstantMatrix",
                CX_TypeClass_DependentSizedMatrix => "DependentSizedMatrix",
                CX_TypeClass_MemberPointer => "MemberPointer",
                CX_TypeClass_ObjCObjectPointer => "ObjCObjectPointer",
                CX_TypeClass_ObjCObject => "ObjCObject",
                CX_TypeClass_ObjCInterface => "ObjCInterface",
                CX_TypeClass_ObjCTypeParam => "ObjCTypeParam",
                CX_TypeClass_PackExpansion => "PackExpansion",
                CX_TypeClass_PackIndexing => "PackIndexing",
                CX_TypeClass_Paren => "Paren",
                CX_TypeClass_Pipe => "Pipe",
                CX_TypeClass_Pointer => "Pointer",
                CX_TypeClass_LValueReference => "LValueReference",
                CX_TypeClass_RValueReference => "RValueReference",
                CX_TypeClass_SubstTemplateTypeParmPack => "SubstTemplateTypeParmPack",
                CX_TypeClass_SubstTemplateTypeParm => "SubstTemplateTypeParm",
                CX_TypeClass_Enum => "Enum",
                CX_TypeClass_Record => "Record",
                CX_TypeClass_TemplateSpecialization => "TemplateSpecialization",
                CX_TypeClass_TemplateTypeParm => "TemplateTypeParm",
                CX_TypeClass_TypeOfExpr => "TypeOfExpr",
                CX_TypeClass_TypeOf => "TypeOf",
                CX_TypeClass_Typedef => "Typedef",
                CX_TypeClass_UnaryTransform => "UnaryTransform",
                CX_TypeClass_UnresolvedUsing => "UnresolvedUsing",
                CX_TypeClass_Using => "Using",
                CX_TypeClass_Vector => "Vector",
                CX_TypeClass_ExtVector => "ExtVector",
                _ => TypeClass.ToString()[13..],
            };
        }
    }

    public readonly CXString TypedefName => (kind != CXType_Invalid) ? clang.getTypedefName(this) : default;

    public readonly CXCursor UnderlyingExpr => (kind != CXType_Invalid) ? clangsharp.Type_getUnderlyingExpr(this) : default;

    public readonly CXType UnderlyingType => (kind != CXType_Invalid) ? clangsharp.Type_getUnderlyingType(this) : default;

    public readonly CXType UnqualifiedType => (kind != CXType_Invalid) ? clang.getUnqualifiedType(this) : default;

    public readonly CXType ValueType => clang.Type_getValueType(this);

    internal readonly string DebuggerDisplayString => $"{TypeClassSpelling}: {this}";

    public static bool operator ==(CXType left, CXType right) => clang.equalTypes(left, right) != 0;

    public static bool operator !=(CXType left, CXType right) => clang.equalTypes(left, right) == 0;

    public override readonly bool Equals(object? obj) => (obj is CXType other) && Equals(other);

    public readonly bool Equals(CXType other) => this == other;

    public readonly CXType GetArgType(uint i) => clang.getArgType(this, i);

    public override int GetHashCode() => HashCode.Combine(kind, (IntPtr)data[0], (IntPtr)data[1]);

    public readonly CXString GetObjCEncoding() => clang.Type_getObjCEncoding(this);

    public readonly CXCursor GetObjCProtocolDecl(uint i) => clang.Type_getObjCProtocolDecl(this, i);

    public readonly CXType GetObjCTypeArg(uint i) => clang.Type_getObjCTypeArg(this, i);

    public readonly long GetOffsetOf(string s)
    {
        using var marshaledS = new MarshaledString(s);
        return clang.Type_getOffsetOf(this, marshaledS);
    }

    public readonly CX_TemplateArgument GetTemplateArgument(uint i) => clangsharp.Type_getTemplateArgument(this, i);

    public override readonly string ToString() => Spelling.ToString();

    public readonly CXVisitorResult VisitFields(CXFieldVisitor visitor, CXClientData clientData)
    {
        var pVisitor = (delegate* unmanaged[Cdecl]<CXCursor, void*, CXVisitorResult>)Marshal.GetFunctionPointerForDelegate(visitor);
        var result = VisitFields(pVisitor, clientData);

        GC.KeepAlive(visitor);
        return result;
    }

    public readonly CXVisitorResult VisitFields(delegate* unmanaged[Cdecl]<CXCursor, void*, CXVisitorResult> visitor, CXClientData clientData)
    {
        return (CXVisitorResult)clang.Type_visitFields(this, visitor, clientData);
    }
}
