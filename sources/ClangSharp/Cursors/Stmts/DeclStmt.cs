// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DeclStmt : Stmt
    {
        private readonly Lazy<IReadOnlyList<Decl>> _decls;

        internal DeclStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_DeclStmt, CX_StmtClass.CX_StmtClass_DeclStmt)
        {
            _decls = new Lazy<IReadOnlyList<Decl>>(() => {
                var numDecls = Handle.NumDecls;
                var decls = new List<Decl>(numDecls);

                for (var i = 0; i < numDecls; i++)
                {
                    var decl = TranslationUnit.GetOrCreate<Decl>(Handle.GetDecl(unchecked((uint)i)));
                    decls.Add(decl);
                }

                return decls;
            });
        }

        public IReadOnlyList<Decl> Decls => _decls.Value;

        public bool IsSingleDecl => Decls.Count == 1;

        public Decl SingleDecl => Decls.SingleOrDefault();
    }
}
