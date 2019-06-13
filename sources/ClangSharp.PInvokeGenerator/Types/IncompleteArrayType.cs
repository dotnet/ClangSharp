using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class IncompleteArrayType : ArrayType
    {
        public IncompleteArrayType(CXType handle, TranslationUnit translationUnit) : base(handle, translationUnit)
        {
            Debug.Assert(handle.kind == CXTypeKind.CXType_IncompleteArray);
        }
    }
}
