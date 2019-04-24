using System;

namespace ClangSharp
{
    public partial struct CXType : IEquatable<CXType>
    {
        public uint AddressSpace => clang.getAddressSpace(this);

        public long AlignOf => clang.Type_getAlignOf(this);

        public CXType ArrayElementType => clang.getArrayElementType(this);

        public long ArraySize => clang.getArraySize(this);

        public CXType CanonicalType => clang.getCanonicalType(this);

        public CXType ClassType => clang.Type_getClassType(this);

        public CXRefQualifierKind CXXRefQualifier => clang.Type_getCXXRefQualifier(this);

        public CXCursor Declaration => clang.getTypeDeclaration(this);

        public CXType ElementType => clang.getElementType(this);

        public CXCursor_ExceptionSpecificationKind ExceptionSpecificationType => (CXCursor_ExceptionSpecificationKind)clang.getExceptionSpecificationType(this);

        public CXCallingConv FunctionTypeCallingConv => clang.getFunctionTypeCallingConv(this);

        public bool IsConstQualified => clang.isConstQualifiedType(this) != 0;

        public bool IsFunctionTypeVariadic => clang.isFunctionTypeVariadic(this) != 0;

        public bool IsPODType => clang.isPODType(this) != 0;

        public bool IsRestrictQualified => clang.isRestrictQualifiedType(this) != 0;

        public bool IsTransparentTagTypedef => clang.Type_isTransparentTagTypedef(this) != 0;

        public bool IsVolatileQualified => clang.isVolatileQualifiedType(this) != 0;

        public CXString KindSpelling => clang.getTypeKindSpelling(kind);

        public CXType ModifierType => clang.Type_getModifiedType(this);

        public CXType NamedType => clang.Type_getNamedType(this);

        public CXTypeNullabilityKind Nullability => clang.Type_getNullability(this);

        public int NumArgTypes => clang.getNumArgTypes(this);

        public long NumElements => clang.getNumElements(this);

        public uint NumObjCProtocolRefs => clang.Type_getNumObjCProtocolRefs(this);

        public uint NumObjCTypeArgs => clang.Type_getNumObjCTypeArgs(this);

        public int NumTemplateArguments => clang.Type_getNumTemplateArguments(this);

        public CXType ObjCObjectBaseType => clang.Type_getObjCObjectBaseType(this);

        public CXString ObjCEncoding => clang.Type_getObjCEncoding(this);

        public CXType PointeeType => clang.getPointeeType(this);

        public CXType ResultType => clang.getResultType(this);

        public long SizeOf => clang.Type_getSizeOf(this);

        public CXString TypedefName => clang.getTypedefName(this);

        public override bool Equals(object obj) => (obj is CXType other) && Equals(other);

        public bool Equals(CXType other) => clang.equalTypes(this, other) != 0;

        public CXType GetArgType(uint i) => clang.getArgType(this, i);

        public CXCursor GetObjCProtocolDecl(uint i) => clang.Type_getObjCProtocolDecl(this, i);

        public CXType GetObjCTypeArg(uint i) => clang.Type_getObjCTypeArg(this, i);

        public long GetOffsetOf(string s) => clang.Type_getOffsetOf(this, s);

        public CXType GetTemplateArgumentAsType(uint i) => clang.Type_getTemplateArgumentAsType(this, i);

        public CXString Spelling => clang.getTypeSpelling(this);

        public override string ToString() => Spelling.ToString();

        public CXVisitorResult VisitFields(CXFieldVisitor visitor, CXClientData clientData) => (CXVisitorResult)clang.Type_visitFields(this, visitor, clientData);
    }
}
