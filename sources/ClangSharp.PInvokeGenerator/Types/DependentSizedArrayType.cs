using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class DependentSizedArrayType : ArrayType
    {
        public DependentSizedArrayType(CXType handle, TranslationUnit translationUnit) : base(handle, translationUnit)
        {
            Debug.Assert(handle.kind == CXTypeKind.CXType_DependentSizedArray);
        }
    }
}
