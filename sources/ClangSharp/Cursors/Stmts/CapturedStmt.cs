// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed partial class CapturedStmt : Stmt
{
    private ValueLazy<CapturedStmt, CapturedDecl> _capturedDecl;
    private ValueLazy<CapturedStmt, RecordDecl> _capturedRecordDecl;
    private ValueLazy<CapturedStmt, Stmt> _captureStmt;
    private readonly LazyList<Capture> _captures;
    private readonly LazyList<Expr, Stmt> _captureInits;

    internal unsafe CapturedStmt(CXCursor handle) : base(handle, CXCursor_UnexposedStmt, CX_StmtClass_CapturedStmt)
    {
        _capturedDecl = new ValueLazy<CapturedStmt, CapturedDecl>(&CapturedDeclFactory);
        _capturedRecordDecl = new ValueLazy<CapturedStmt, RecordDecl>(&CapturedRecordDeclFactory);
        _captureStmt = new ValueLazy<CapturedStmt, Stmt>(&CaptureStmtFactory);
        _captures = LazyList.Create<Capture>(Handle.NumCaptures, (i) => new Capture(this, unchecked((uint)i)));
        _captureInits = LazyList.Create<Expr, Stmt>(_children);
    }

    public CapturedDecl CapturedDecl => _capturedDecl.GetValue(this);

    public RecordDecl CapturedRecordDecl => _capturedRecordDecl.GetValue(this);

    public CX_CapturedRegionKind CapturedRegionKind => Handle.CapturedRegionKind;

    public Stmt CaptureStmt => _captureStmt.GetValue(this);

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

    private static unsafe Stmt CaptureStmtFactory(CapturedStmt self) => self.TranslationUnit.GetOrCreate<Stmt>(self.Handle.CapturedStmt);

    private static unsafe RecordDecl CapturedRecordDeclFactory(CapturedStmt self) => self.TranslationUnit.GetOrCreate<RecordDecl>(self.Handle.CapturedRecordDecl);

    private static unsafe CapturedDecl CapturedDeclFactory(CapturedStmt self) => self.TranslationUnit.GetOrCreate<CapturedDecl>(self.Handle.CapturedDecl);
}
