using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class BuiltinType : Type
    {
        public BuiltinType(CXType handle, TranslationUnit translationUnit) : base(handle, translationUnit)
        {
        }
    }
}
