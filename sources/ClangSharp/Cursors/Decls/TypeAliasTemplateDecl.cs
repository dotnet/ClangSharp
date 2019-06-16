using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TypeAliasTemplateDecl : RedeclarableTemplateDecl
    {
        internal TypeAliasTemplateDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_TypeAliasTemplateDecl)
        {
        }
    }
}
