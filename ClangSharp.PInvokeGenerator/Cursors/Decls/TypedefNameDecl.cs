using System;

namespace ClangSharp
{
    internal class TypedefNameDecl : TypeDecl
    {
        private readonly Lazy<Type> _underlyingType;

        protected TypedefNameDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            _underlyingType = new Lazy<Type>(() => TranslationUnit.GetOrCreateType(Handle.TypedefDeclUnderlyingType, () => Type.Create(Handle.TypedefDeclUnderlyingType, TranslationUnit)));
        }

        public Type UnderlyingType => _underlyingType.Value;
    }
}
