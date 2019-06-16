using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class ValueDecl : NamedDecl
    {
        private readonly Lazy<Type> _type;

        private protected ValueDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
            _type = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Type));
        }

        public Type Type => _type.Value;
    }
}
