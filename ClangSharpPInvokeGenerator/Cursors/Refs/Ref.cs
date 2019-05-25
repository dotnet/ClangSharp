using System;
using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal class Ref : Cursor
    {
        private readonly Lazy<Type> _type;

        protected Ref(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.IsReference);
            _type = new Lazy<Type>(() => TranslationUnit.GetOrCreateType(Handle.Type, () => Type.Create(Handle.Type, TranslationUnit)));
        }

        public Type Type => _type.Value;
    }
}
