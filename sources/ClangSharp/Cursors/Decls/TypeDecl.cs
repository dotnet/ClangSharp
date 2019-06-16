using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class TypeDecl : NamedDecl
    {
        private readonly Lazy<Type> _typeForDecl;

        private protected TypeDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
            _typeForDecl = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Type));
        }

        public Type TypeForDecl => _typeForDecl.Value;
    }
}
