// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class DeclStmt : Stmt
{
    private readonly LazyList<Decl> _decls;

    internal DeclStmt(CXCursor handle) : base(handle, CXCursor_DeclStmt, CX_StmtClass_DeclStmt)
    {
        _decls = LazyList.Create<Decl>(Handle.NumDecls, (i) => TranslationUnit.GetOrCreate<Decl>(Handle.GetDecl(unchecked((uint)i))));
    }

    public IReadOnlyList<Decl> Decls => _decls;

    public bool IsSingleDecl => Decls.Count == 1;

    public Decl? SingleDecl => Decls.SingleOrDefault();
}
