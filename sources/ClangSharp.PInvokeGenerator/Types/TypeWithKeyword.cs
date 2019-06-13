using ClangSharp.Interop;

namespace ClangSharp
{
    internal class TypeWithKeyword : Type
    {
        protected TypeWithKeyword(CXType handle, TranslationUnit translationUnit) : base(handle, translationUnit)
        {
        }
    }
}
