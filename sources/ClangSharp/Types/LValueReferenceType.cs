using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class LValueReferenceType : ReferenceType
    {
        public LValueReferenceType(CXType handle, TranslationUnitDecl translationUnit) : base(handle, translationUnit)
        {
            Debug.Assert(handle.kind == CXTypeKind.CXType_LValueReference);
        }
    }
}
