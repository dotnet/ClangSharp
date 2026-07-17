// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class GCCAsmStmt : AsmStmt
{
    private readonly LazyList<Expr> _inputExprs;
    private readonly LazyList<Expr> _labelExprs;
    private readonly LazyList<string> _labelNames;
    private readonly LazyList<Expr> _outputExprs;

    internal unsafe GCCAsmStmt(CXCursor handle) : base(handle, CXCursor_GCCAsmStmt, CX_StmtClass_GCCAsmStmt)
    {
        _inputExprs = LazyList.Create<Expr>(this, (int)Handle.NumInputs, &InputExprsFactory);
        _labelExprs = LazyList.Create<Expr>(this, (int)Handle.NumLabels, &LabelExprsFactory);
        _labelNames = LazyList.Create<string>(this, (int)Handle.NumLabels, &LabelNamesFactory);
        _outputExprs = LazyList.Create<Expr>(this, (int)Handle.NumOutputs, &OutputExprsFactory);
    }

    public string AsmString => Handle.AsmString.CString;

    public Expr AsmStringExpr => TranslationUnit.GetOrCreate<Expr>(Handle.AsmStringExpr);

    public IReadOnlyList<Expr> InputExprs => _inputExprs;

    public bool IsAsmGoto => Handle.IsAsmGoto;

    public IReadOnlyList<Expr> LabelExprs => _labelExprs;

    public IReadOnlyList<string> LabelNames => _labelNames;

    public uint NumLabels => Handle.NumLabels;

    public IReadOnlyList<Expr> OutputExprs => _outputExprs;

    private static Expr InputExprsFactory(object self, int i) => ((GCCAsmStmt)self).TranslationUnit.GetOrCreate<Expr>(((GCCAsmStmt)self).Handle.GetInputExpr(unchecked((uint)i)));

    private static Expr LabelExprsFactory(object self, int i) => ((GCCAsmStmt)self).TranslationUnit.GetOrCreate<Expr>(((GCCAsmStmt)self).Handle.GetLabelExpr(unchecked((uint)i)));

    private static string LabelNamesFactory(object self, int i) => ((GCCAsmStmt)self).Handle.GetLabelName(unchecked((uint)i)).CString;

    private static Expr OutputExprsFactory(object self, int i) => ((GCCAsmStmt)self).TranslationUnit.GetOrCreate<Expr>(((GCCAsmStmt)self).Handle.GetOutputExpr(unchecked((uint)i)));
}
