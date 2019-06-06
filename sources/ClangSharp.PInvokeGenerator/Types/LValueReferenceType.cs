using System.Diagnostics;

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
