using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class UsingDecl : NamedDecl
    {
        private readonly Lazy<Cursor> _referenced;

        public UsingDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_UsingDeclaration);

            _referenced = new Lazy<Cursor>(() => {
                var cursor = TranslationUnit.GetOrCreateCursor(Handle.Referenced, () => Create(Handle.Referenced, this));
                cursor?.Visit(clientData: default);
                return cursor;
            });
        }

        public Cursor Referenced => _referenced.Value;
    }
}
