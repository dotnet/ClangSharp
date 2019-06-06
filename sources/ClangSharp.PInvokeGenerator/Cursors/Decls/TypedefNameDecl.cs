namespace ClangSharp
{
    internal class TypedefNameDecl : TypeDecl
    {
        protected TypedefNameDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            UnderlyingType = TranslationUnit.GetOrCreateType(Handle.TypedefDeclUnderlyingType, () => Type.Create(Handle.TypedefDeclUnderlyingType, TranslationUnit));
        }

        public Type UnderlyingType { get; }
    }
}
