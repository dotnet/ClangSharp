namespace ClangSharp
{
    internal class ValueDecl : NamedDecl
    {
        protected ValueDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Type = TranslationUnit.GetOrCreateType(Handle.Type, () => Type.Create(Handle.Type, TranslationUnit));
        }

        public Type Type { get; }
    }
}
