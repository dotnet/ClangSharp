using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class BuiltinType : Type
    {
        public BuiltinType(CXType handle, TranslationUnit translationUnit) : base(handle, translationUnit)
        {
        }
    }
}
