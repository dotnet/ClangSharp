// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class ObjCAtCatchStmt : Stmt
{
    private readonly Lazy<VarDecl?> _catchParamDecl;

    internal ObjCAtCatchStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCAtCatchStmt, CX_StmtClass.CX_StmtClass_ObjCAtCatchStmt)
    {
        Debug.Assert(NumChildren is 1);
        _catchParamDecl = new Lazy<VarDecl?>(() => !Handle.Referenced.IsNull ? TranslationUnit.GetOrCreate<VarDecl>(Handle.Referenced) : null);
    }

    public Stmt CatchBody => Children[0];

    public VarDecl? CatchParamDecl => _catchParamDecl.Value;

    public bool HasEllipsis => CatchParamDecl is null;
}
