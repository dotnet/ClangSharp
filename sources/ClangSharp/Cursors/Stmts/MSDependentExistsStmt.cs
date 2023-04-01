// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class MSDependentExistsStmt : Stmt
{
    internal MSDependentExistsStmt(CXCursor handle) : base(handle, CXCursor_UnexposedStmt, CX_StmtClass_MSDependentExistsStmt)
    {
        Debug.Assert(NumChildren is 1);
    }

    public bool IsIfExists => Handle.IsIfExists;

    public bool IsIfNotExists => !IsIfExists;

    public string Name => Handle.Name.CString;

    public CompoundStmt SubStmt => (CompoundStmt)Children[0];
}
