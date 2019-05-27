using System;

namespace ClangSharp
{
    internal class TypeDecl : NamedDecl
    {
        private readonly Lazy<Type> _type;

        protected TypeDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            _type = new Lazy<Type>(() => TranslationUnit.GetOrCreateType(Handle.Type, () => Type.Create(Handle.Type, TranslationUnit)));
        }

        public Type Type => _type.Value;
    }
}
