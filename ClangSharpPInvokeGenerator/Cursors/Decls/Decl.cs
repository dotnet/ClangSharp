using System;
using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal class Decl : Cursor
    {
        private readonly Lazy<Type> _type;

        protected Decl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.IsDeclaration);
            _type = new Lazy<Type>(() => TranslationUnit.GetOrCreateType(Handle.Type, () => Type.Create(Handle.Type, TranslationUnit)));
        }

        public bool IsAnonymous => Handle.IsAnonymous;

        public Type Type => _type.Value;
    }
}
