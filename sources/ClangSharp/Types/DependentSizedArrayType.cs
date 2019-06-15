using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DependentSizedArrayType : ArrayType
    {
        public DependentSizedArrayType(CXType handle, TranslationUnitDecl translationUnit) : base(handle, translationUnit)
        {
            Debug.Assert(handle.kind == CXTypeKind.CXType_DependentSizedArray);
        }
    }
}
