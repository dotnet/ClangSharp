// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ObjCAtCatchStmt : Stmt
{
    private readonly ValueLazy<VarDecl?> _catchParamDecl;

    internal ObjCAtCatchStmt(CXCursor handle) : base(handle, CXCursor_ObjCAtCatchStmt, CX_StmtClass_ObjCAtCatchStmt)
    {
        Debug.Assert(NumChildren is 1);
        _catchParamDecl = new ValueLazy<VarDecl?>(() => !Handle.Referenced.IsNull ? TranslationUnit.GetOrCreate<VarDecl>(Handle.Referenced) : null);
    }

    public Stmt CatchBody => Children[0];

    public VarDecl? CatchParamDecl => _catchParamDecl.Value;

    public bool HasEllipsis => CatchParamDecl is null;
}
