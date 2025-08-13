// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed partial class CapturedStmt : Stmt
{
    private readonly ValueLazy<CapturedDecl> _capturedDecl;
    private readonly ValueLazy<RecordDecl> _capturedRecordDecl;
    private readonly ValueLazy<Stmt> _captureStmt;
    private readonly LazyList<Capture> _captures;
    private readonly LazyList<Expr, Stmt> _captureInits;

    internal CapturedStmt(CXCursor handle) : base(handle, CXCursor_UnexposedStmt, CX_StmtClass_CapturedStmt)
    {
        _capturedDecl = new ValueLazy<CapturedDecl>(() => TranslationUnit.GetOrCreate<CapturedDecl>(Handle.CapturedDecl));
        _capturedRecordDecl = new ValueLazy<RecordDecl>(() => TranslationUnit.GetOrCreate<RecordDecl>(Handle.CapturedRecordDecl));
        _captureStmt = new ValueLazy<Stmt>(() => TranslationUnit.GetOrCreate<Stmt>(Handle.CapturedStmt));
        _captures = LazyList.Create<Capture>(Handle.NumCaptures, (i) => new Capture(this, unchecked((uint)i)));
        _captureInits = LazyList.Create<Expr, Stmt>(_children);
    }

    public CapturedDecl CapturedDecl => _capturedDecl.Value;

    public RecordDecl CapturedRecordDecl => _capturedRecordDecl.Value;

    public CX_CapturedRegionKind CapturedRegionKind => Handle.CapturedRegionKind;

    public Stmt CaptureStmt => _captureStmt.Value;

    public IReadOnlyList<Capture> Captures => _captures;

    public uint CaptureSize => unchecked((uint)Handle.NumCaptures);

    public IReadOnlyList<Expr> CaptureInits => _captureInits;

    public bool CapturesVariable(VarDecl var)
    {
        var canonicalDecl = var?.CanonicalDecl;

        foreach (var i in Captures)
        {
            if (!i.CapturesVariable && !i.CapturesVariableByCopy)
            {
                continue;
            }

            if (i.CapturedVar.CanonicalDecl == canonicalDecl)
            {
                return true;
            }
        }
        return false;
    }
}
