using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class IncompleteArrayType : ArrayType
    {
        public IncompleteArrayType(CXType handle, TranslationUnitDecl translationUnit) : base(handle, translationUnit)
        {
            Debug.Assert(handle.kind == CXTypeKind.CXType_IncompleteArray);
        }
    }
}
