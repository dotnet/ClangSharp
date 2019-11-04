using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DeclStmt : Stmt
    {
        private readonly Lazy<IReadOnlyList<Decl>> _children;
        internal DeclStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_DeclStmt)
        {
            _children = new Lazy<IReadOnlyList<Decl>>(() => CursorChildren.OfType<Decl>().ToList());
        }

        public IReadOnlyList<Decl> Decls => _children.Value;
    }
}
