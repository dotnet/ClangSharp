using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal class Type
    {
        public static Type Create(CXType handle, TranslationUnit translationUnit)
        {
            Debug.Assert(handle.kind != CXTypeKind.CXType_Invalid);

            switch (handle.kind)
            {
                case CXTypeKind.CXType_Unexposed:
                {
                    return new UnexposedType(handle, translationUnit);
                }

                case CXTypeKind.CXType_Void:
                case CXTypeKind.CXType_Bool:
                case CXTypeKind.CXType_Char_U:
                case CXTypeKind.CXType_UChar:
                case CXTypeKind.CXType_UShort:
                case CXTypeKind.CXType_UInt:
                case CXTypeKind.CXType_ULong:
                case CXTypeKind.CXType_ULongLong:
                case CXTypeKind.CXType_Char_S:
                case CXTypeKind.CXType_SChar:
                case CXTypeKind.CXType_WChar:
                case CXTypeKind.CXType_Short:
                case CXTypeKind.CXType_Int:
                case CXTypeKind.CXType_Long:
                case CXTypeKind.CXType_LongLong:
                case CXTypeKind.CXType_Float:
                case CXTypeKind.CXType_Double:
                case CXTypeKind.CXType_LongDouble:
                case CXTypeKind.CXType_NullPtr:
                case CXTypeKind.CXType_Dependent:
                {
                    return new BuiltinType(handle, translationUnit);
                }

                case CXTypeKind.CXType_Pointer:
                {
                    return new PointerType(handle, translationUnit);
                }

                case CXTypeKind.CXType_LValueReference:
                {
                    return new LValueReferenceType(handle, translationUnit);
                }

                case CXTypeKind.CXType_Record:
                {
                    return new RecordType(handle, translationUnit);
                }

                case CXTypeKind.CXType_Enum:
                {
                    return new EnumType(handle, translationUnit);
                }

                case CXTypeKind.CXType_Typedef:
                {
                    return new TypedefType(handle, translationUnit);
                }

                case CXTypeKind.CXType_FunctionProto:
                {
                    return new FunctionProtoType(handle, translationUnit);
                }

                case CXTypeKind.CXType_ConstantArray:
                {
                    return new ConstantArrayType(handle, translationUnit);
                }

                case CXTypeKind.CXType_IncompleteArray:
                {
                    return new IncompleteArrayType(handle, translationUnit);
                }

                case CXTypeKind.CXType_DependentSizedArray:
                {
                    return new DependentSizedArrayType(handle, translationUnit);
                }

                case CXTypeKind.CXType_Elaborated:
                {
                    return new ElaboratedType(handle, translationUnit);
                }

                case CXTypeKind.CXType_Attributed:
                {
                    return new AttributedType(handle, translationUnit);
                }

                default:
                {
                    Debug.WriteLine($"Unhandled type kind: {handle.KindSpelling}.");
                    Debugger.Break();
                    return new Type(handle, translationUnit);
                }
            }
        }

        protected Type(CXType handle, TranslationUnit translationUnit)
        {
            Debug.Assert(translationUnit != null);

            Handle = handle;
            TranslationUnit = translationUnit;

            translationUnit.AddVisitedType(this);
            CanonicalType = TranslationUnit.GetOrCreateType(Handle.CanonicalType, () => Create(Handle.CanonicalType, TranslationUnit));
        }

        public Type CanonicalType { get; }

        public CXType Handle { get; }

        public bool IsConstQualified => Handle.IsConstQualified;

        public CXTypeKind Kind => Handle.kind;

        public string KindSpelling => Handle.KindSpelling.ToString();

        public TranslationUnit TranslationUnit { get; }

        public string Spelling => Handle.Spelling.ToString();
    }
}
