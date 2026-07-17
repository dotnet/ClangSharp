// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public class AsmStmt : Stmt
{
    private readonly LazyList<string> _clobbers;
    private readonly LazyList<string> _inputConstraints;
    private readonly LazyList<string> _outputConstraints;

    private protected unsafe AsmStmt(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
    {
        if (handle.StmtClass is > CX_StmtClass_LastAsmStmt or < CX_StmtClass_FirstAsmStmt)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _clobbers = LazyList.Create<string>(this, (int)Handle.NumClobbers, &ClobbersFactory);
        _inputConstraints = LazyList.Create<string>(this, (int)Handle.NumInputs, &InputConstraintsFactory);
        _outputConstraints = LazyList.Create<string>(this, (int)Handle.NumOutputs, &OutputConstraintsFactory);
    }

    public IReadOnlyList<string> Clobbers => _clobbers;

    public IReadOnlyList<string> InputConstraints => _inputConstraints;

    public bool IsSimple => Handle.IsSimple;

    public bool IsVolatile => Handle.IsVolatile;

    public uint NumClobbers => Handle.NumClobbers;

    public uint NumInputs => Handle.NumInputs;

    public uint NumOutputs => Handle.NumOutputs;

    public IReadOnlyList<string> OutputConstraints => _outputConstraints;

    private static string ClobbersFactory(object self, int i) => ((AsmStmt)self).Handle.GetClobber(unchecked((uint)i)).CString;

    private static string InputConstraintsFactory(object self, int i) => ((AsmStmt)self).Handle.GetInputConstraint(unchecked((uint)i)).CString;

    private static string OutputConstraintsFactory(object self, int i) => ((AsmStmt)self).Handle.GetOutputConstraint(unchecked((uint)i)).CString;
}
