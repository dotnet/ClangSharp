// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class GotoStmt : Stmt
{
    private ValueLazy<GotoStmt, LabelDecl> _label;

    internal unsafe GotoStmt(CXCursor handle) : base(handle, CXCursor_GotoStmt, CX_StmtClass_GotoStmt)
    {
        Debug.Assert(NumChildren is 0);
        _label = new ValueLazy<GotoStmt, LabelDecl>(&LabelFactory);
    }

    public LabelDecl Label => _label.GetValue(this);

    private static unsafe LabelDecl LabelFactory(GotoStmt self) => self.TranslationUnit.GetOrCreate<LabelDecl>(self.Handle.Referenced);
}
