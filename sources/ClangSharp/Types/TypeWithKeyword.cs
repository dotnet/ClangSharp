using ClangSharp.Interop;

namespace ClangSharp
{
    public class TypeWithKeyword : Type
    {
        protected TypeWithKeyword(CXType handle, TranslationUnitDecl translationUnit) : base(handle, translationUnit)
        {
        }
    }
}
