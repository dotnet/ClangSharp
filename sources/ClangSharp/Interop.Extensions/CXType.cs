// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ClangSharp.Interop
{
    [DebuggerDisplay("{DebuggerDisplayString,nq}")]
    public unsafe partial struct CXType : IEquatable<CXType>
    {
        public uint AddressSpace => (kind != CXTypeKind.CXType_Invalid) ? clang.getAddressSpace(this) : default;

        public CXCursor AddrSpaceExpr => clangsharp.Type_getAddrSpaceExpr(this);

        public CXType AdjustedType => clangsharp.Type_getAdjustedType(this);

        public long AlignOf => clang.Type_getAlignOf(this);

        public CXType ArrayElementType => clang.getArrayElementType(this);

        public long ArraySize => clang.getArraySize(this);

        public CX_AttrKind AttrKind => clangsharp.Type_getAttrKind(this);

        public CXType BaseType => clangsharp.Type_getBaseType(this);

        public CXType CanonicalType => clang.getCanonicalType(this);

        public CXType ClassType => clang.Type_getClassType(this);

        public CXCursor ColumnExpr => clangsharp.Type_getColumnExpr(this);

        public CXRefQualifierKind CXXRefQualifier => clang.Type_getCXXRefQualifier(this);

        public CXType DecayedType => clangsharp.Type_getDecayedType(this);

        public CXCursor Declaration => clangsharp.Type_getDeclaration(this);

        public CXType DeducedType => clangsharp.Type_getDeducedType(this);

        public int Depth => clangsharp.Type_getDepth(this);

        public CXType ElementType => clangsharp.Type_getElementType(this);

        public CXType EquivalentType => clangsharp.Type_getEquivalentType(this);

        public CXCursor_ExceptionSpecificationKind ExceptionSpecificationType => (CXCursor_ExceptionSpecificationKind)clang.getExceptionSpecificationType(this);

        public CXCallingConv FunctionTypeCallingConv => clang.getFunctionTypeCallingConv(this);

        public int Index => clangsharp.Type_getIndex(this);

        public CXType InjectedSpecializationType => clangsharp.Type_getInjectedSpecializationType(this);

        public CXType InjectedTST => clangsharp.Type_getInjectedTST(this);

        public bool IsCanonical => Equals(CanonicalType);

        public bool IsConstQualified => clang.isConstQualifiedType(this) != 0;

        public bool IsFunctionTypeVariadic => clang.isFunctionTypeVariadic(this) != 0;

        public bool IsPODType => clang.isPODType(this) != 0;

        public bool IsRestrictQualified => clang.isRestrictQualifiedType(this) != 0;

        public bool IsSigned => clangsharp.Type_getIsSigned(this) != 0;

        public bool IsSugared => clangsharp.Type_getIsSugared(this) != 0;

        public bool IsTransparentTagTypedef => clang.Type_isTransparentTagTypedef(this) != 0;

        public bool IsTypeAlias => clangsharp.Type_getIsTypeAlias(this) != 0;

        public bool IsUnsigned => clangsharp.Type_getIsUnsigned(this) != 0;

        public bool IsVolatileQualified => clang.isVolatileQualifiedType(this) != 0;

        public CXString KindSpelling => clang.getTypeKindSpelling(kind);

        public CXType ModifiedType => clangsharp.Type_getModifiedType(this);

        public CXType NamedType => clang.Type_getNamedType(this);

        public CXTypeNullabilityKind Nullability => clang.Type_getNullability(this);

        public int NumArgTypes => clang.getNumArgTypes(this);

        public int NumBits => clangsharp.Type_getNumBits(this);

        public CXCursor NumBitsExpr => clangsharp.Type_getNumBitsExpr(this);

        public int NumColumns => clangsharp.Type_getNumColumns(this);

        public int NumElementsFlattened => clangsharp.Type_getNumElementsFlattened(this);

        public int NumRows => clangsharp.Type_getNumRows(this);

        public long NumElements => clang.getNumElements(this);

        public uint NumObjCProtocolRefs => clang.Type_getNumObjCProtocolRefs(this);

        public uint NumObjCTypeArgs => clang.Type_getNumObjCTypeArgs(this);

        public int NumTemplateArguments => clang.Type_getNumTemplateArguments(this);

        public CXType ObjCObjectBaseType => clang.Type_getObjCObjectBaseType(this);

        public CXType OriginalType => clangsharp.Type_getOriginalType(this);

        public CXCursor OwnedTagDecl => clangsharp.Type_getOwnedTagDecl(this);

        public CXType PointeeType => clangsharp.Type_getPointeeType(this);

        public CXType ResultType => clang.getResultType(this);

        public CXCursor RowExpr => clangsharp.Type_getRowExpr(this);

        public CXCursor SizeExpr => clangsharp.Type_getSizeExpr(this);

        public long SizeOf => clang.Type_getSizeOf(this);

        public CXString Spelling => clang.getTypeSpelling(this);

        public CX_TypeClass TypeClass => clangsharp.Type_getTypeClass(this);

        public string TypeClassSpelling
        {
            get
            {
                Debug.Assert(CX_TypeClass.CX_TypeClass_TypeLast == CX_TypeClass.CX_TypeClass_ExtVector);
                Debug.Assert(CX_TypeClass.CX_TypeClass_TagFirst == CX_TypeClass.CX_TypeClass_Record);
                Debug.Assert(CX_TypeClass.CX_TypeClass_TagLast == CX_TypeClass.CX_TypeClass_Enum);

                return TypeClass switch
                {
                    CX_TypeClass.CX_TypeClass_Invalid => "Invalid",
                    CX_TypeClass.CX_TypeClass_Adjusted => "Adjusted",
                    CX_TypeClass.CX_TypeClass_Decayed => "Decayed",
                    CX_TypeClass.CX_TypeClass_ConstantArray => "ConstantArray",
                    CX_TypeClass.CX_TypeClass_DependentSizedArray => "DependentSizedArray",
                    CX_TypeClass.CX_TypeClass_IncompleteArray => "IncompleteArray",
                    CX_TypeClass.CX_TypeClass_VariableArray => "VariableArray",
                    CX_TypeClass.CX_TypeClass_Atomic => "Atomic",
                    CX_TypeClass.CX_TypeClass_Attributed => "Attributed",
                    CX_TypeClass.CX_TypeClass_BlockPointer => "BlockPointer",
                    CX_TypeClass.CX_TypeClass_Builtin => "Builtin",
                    CX_TypeClass.CX_TypeClass_Complex => "Complex",
                    CX_TypeClass.CX_TypeClass_Decltype => "Decltype",
                    CX_TypeClass.CX_TypeClass_Auto => "Auto",
                    CX_TypeClass.CX_TypeClass_DeducedTemplateSpecialization => "DeducedTemplateSpecialization",
                    CX_TypeClass.CX_TypeClass_DependentAddressSpace => "DependentAddressSpace",
                    CX_TypeClass.CX_TypeClass_DependentExtInt => "DependentExtInt",
                    CX_TypeClass.CX_TypeClass_DependentName => "DependentName",
                    CX_TypeClass.CX_TypeClass_DependentSizedExtVector => "DependentSizedExtVector",
                    CX_TypeClass.CX_TypeClass_DependentTemplateSpecialization => "DependentTemplateSpecialization",
                    CX_TypeClass.CX_TypeClass_DependentVector => "DependentVector",
                    CX_TypeClass.CX_TypeClass_Elaborated => "Elaborated",
                    CX_TypeClass.CX_TypeClass_ExtInt => "ExtInt",
                    CX_TypeClass.CX_TypeClass_FunctionNoProto => "FunctionNoProto",
                    CX_TypeClass.CX_TypeClass_FunctionProto => "FunctionProto",
                    CX_TypeClass.CX_TypeClass_InjectedClassName => "InjectedClassName",
                    CX_TypeClass.CX_TypeClass_MacroQualified => "MacroQualified",
                    CX_TypeClass.CX_TypeClass_ConstantMatrix => "ConstantMatrix",
                    CX_TypeClass.CX_TypeClass_DependentSizedMatrix => "DependentSizedMatrix",
                    CX_TypeClass.CX_TypeClass_MemberPointer => "MemberPointer",
                    CX_TypeClass.CX_TypeClass_ObjCObjectPointer => "ObjCObjectPointer",
                    CX_TypeClass.CX_TypeClass_ObjCObject => "ObjCObject",
                    CX_TypeClass.CX_TypeClass_ObjCInterface => "ObjCInterface",
                    CX_TypeClass.CX_TypeClass_ObjCTypeParam => "ObjCTypeParam",
                    CX_TypeClass.CX_TypeClass_PackExpansion => "PackExpansion",
                    CX_TypeClass.CX_TypeClass_Paren => "Paren",
                    CX_TypeClass.CX_TypeClass_Pipe => "Pipe",
                    CX_TypeClass.CX_TypeClass_Pointer => "Pointer",
                    CX_TypeClass.CX_TypeClass_LValueReference => "LValueReference",
                    CX_TypeClass.CX_TypeClass_RValueReference => "RValueReference",
                    CX_TypeClass.CX_TypeClass_SubstTemplateTypeParmPack => "SubstTemplateTypeParmPack",
                    CX_TypeClass.CX_TypeClass_SubstTemplateTypeParm => "SubstTemplateTypeParm",
                    CX_TypeClass.CX_TypeClass_Enum => "Enum",
                    CX_TypeClass.CX_TypeClass_Record => "Record",
                    CX_TypeClass.CX_TypeClass_TemplateSpecialization => "TemplateSpecialization",
                    CX_TypeClass.CX_TypeClass_TemplateTypeParm => "TemplateTypeParm",
                    CX_TypeClass.CX_TypeClass_TypeOfExpr => "TypeOfExpr",
                    CX_TypeClass.CX_TypeClass_TypeOf => "TypeOf",
                    CX_TypeClass.CX_TypeClass_Typedef => "Typedef",
                    CX_TypeClass.CX_TypeClass_UnaryTransform => "UnaryTransform",
                    CX_TypeClass.CX_TypeClass_UnresolvedUsing => "UnresolvedUsing",
                    CX_TypeClass.CX_TypeClass_Vector => "Vector",
                    CX_TypeClass.CX_TypeClass_ExtVector => "ExtVector",
                    _ => TypeClass.ToString().Substring(13),
                };
            }
        }

        public CXString TypedefName => clang.getTypedefName(this);

        public CXCursor UnderlyingExpr => clangsharp.Type_getUnderlyingExpr(this);

        public CXType UnderlyingType => clangsharp.Type_getUnderlyingType(this);

        public CXType ValueType => clang.Type_getValueType(this);

        internal string DebuggerDisplayString
        {
            get
            {
                return $"{TypeClassSpelling}: {this}";
            }
        }

        public static bool operator ==(CXType left, CXType right) => clang.equalTypes(left, right) != 0;

        public static bool operator !=(CXType left, CXType right) => clang.equalTypes(left, right) == 0;

        public CXType Desugar() => clangsharp.Type_desugar(this);

        public override bool Equals(object obj) => (obj is CXType other) && Equals(other);

        public bool Equals(CXType other) => this == other;

        public CXType GetArgType(uint i) => clang.getArgType(this, i);

        public override int GetHashCode() => HashCode.Combine(kind, (IntPtr)data[0], (IntPtr)data[1]);

        public CXString GetObjCEncoding() => clang.Type_getObjCEncoding(this);

        public CXCursor GetObjCProtocolDecl(uint i) => clang.Type_getObjCProtocolDecl(this, i);

        public CXType GetObjCTypeArg(uint i) => clang.Type_getObjCTypeArg(this, i);

        public long GetOffsetOf(string s)
        {
            using var marshaledS = new MarshaledString(s);
            return clang.Type_getOffsetOf(this, marshaledS);
        }

        public CXCursor GetTemplateArgumentAsDecl(uint i) => clangsharp.Type_getTemplateArgumentAsDecl(this, i);

        public CXCursor GetTemplateArgumentAsExpr(uint i) => clangsharp.Type_getTemplateArgumentAsExpr(this, i);

        public long GetTemplateArgumentAsIntegral(uint i) => clangsharp.Type_getTemplateArgumentAsIntegral(this, i);

        public CXType GetTemplateArgumentAsType(uint i) => clangsharp.Type_getTemplateArgumentAsType(this, i);

        public CXType GetTemplateArgumentIntegralType(uint i) => clangsharp.Type_getTemplateArgumentIntegralType(this, i);

        public CXTemplateArgumentKind GetTemplateArgumentKind(uint i) => clangsharp.Type_getTemplateArgumentKind(this, i);

        public CXType GetTemplateArgumentNullPtrType(uint i) => clangsharp.Type_getTemplateArgumentNullPtrType(this, i);

        public override string ToString() => Spelling.ToString();

        public CXVisitorResult VisitFields(CXFieldVisitor visitor, CXClientData clientData)
        {
            var pVisitor = Marshal.GetFunctionPointerForDelegate(visitor);
            var result = (CXVisitorResult)clang.Type_visitFields(this, pVisitor, clientData);

            GC.KeepAlive(visitor);
            return result;
        }
    }
}
