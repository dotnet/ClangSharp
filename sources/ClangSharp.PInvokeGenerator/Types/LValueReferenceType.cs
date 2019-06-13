using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class LValueReferenceType : ReferenceType
    {
        public LValueReferenceType(CXType handle, TranslationUnit translationUnit) : base(handle, translationUnit)
        {
            Debug.Assert(handle.kind == CXTypeKind.CXType_LValueReference);
        }
    }
}
