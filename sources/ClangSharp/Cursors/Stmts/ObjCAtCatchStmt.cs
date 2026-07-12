// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ObjCAtCatchStmt : Stmt
{
    private ValueLazy<ObjCAtCatchStmt, VarDecl?> _catchParamDecl;

    internal unsafe ObjCAtCatchStmt(CXCursor handle) : base(handle, CXCursor_ObjCAtCatchStmt, CX_StmtClass_ObjCAtCatchStmt)
    {
        Debug.Assert(NumChildren is 1);
        _catchParamDecl = new ValueLazy<ObjCAtCatchStmt, VarDecl?>(&CatchParamDeclFactory);
    }

    public Stmt CatchBody => Children[0];

    public VarDecl? CatchParamDecl => _catchParamDecl.GetValue(this);

    public bool HasEllipsis => CatchParamDecl is null;

    private static unsafe VarDecl? CatchParamDeclFactory(ObjCAtCatchStmt self) => !self.Handle.Referenced.IsNull ? self.TranslationUnit.GetOrCreate<VarDecl>(self.Handle.Referenced) : null;
}
