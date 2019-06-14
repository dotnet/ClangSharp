using ClangSharp.Interop;

namespace ClangSharp
{
    public class TypeDecl : NamedDecl
    {
        protected TypeDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Type = TranslationUnit.GetOrCreateType(Handle.Type, () => Type.Create(Handle.Type, TranslationUnit));
        }

        public Type Type { get; }
    }
}
