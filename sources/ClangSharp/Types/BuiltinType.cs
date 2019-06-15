using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class BuiltinType : Type
    {
        public BuiltinType(CXType handle, TranslationUnitDecl translationUnit) : base(handle, translationUnit)
        {
        }
    }
}
