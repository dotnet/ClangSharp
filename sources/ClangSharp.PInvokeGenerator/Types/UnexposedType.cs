using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class UnexposedType : Type
    {
        public UnexposedType(CXType handle, TranslationUnit translationUnit) : base(handle, translationUnit)
        {
            Debug.Assert(handle.kind == CXTypeKind.CXType_Unexposed);
        }
    }
}
