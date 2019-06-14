using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class OverloadedDeclRef : Ref
    {
        private readonly Decl[] _overloadedDecls;

        public OverloadedDeclRef(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_OverloadedDeclRef);

            _overloadedDecls = new Decl[Handle.NumOverloadedDecls];

            for (uint index = 0; index < Handle.NumArguments; index++)
            {
                var declHandle = Handle.GetOverloadedDecl(index);
                var decl = GetOrAddDecl(declHandle);
                decl.Visit(clientData: default);
            }
        }

        public IReadOnlyList<Decl> OverloadedDecls => _overloadedDecls;
    }
}
