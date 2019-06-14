using ClangSharp.Interop;

namespace ClangSharp
{
    public class TypeWithKeyword : Type
    {
        protected TypeWithKeyword(CXType handle, TranslationUnit translationUnit) : base(handle, translationUnit)
        {
        }
    }
}
