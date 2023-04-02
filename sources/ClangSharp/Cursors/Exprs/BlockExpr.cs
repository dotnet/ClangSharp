// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class BlockExpr : Expr
{
    private readonly Lazy<BlockDecl> _blockDecl;

    internal BlockExpr(CXCursor handle) : base(handle, CXCursor_BlockExpr, CX_StmtClass_BlockExpr)
    {
        Debug.Assert(NumChildren is 0);
        _blockDecl = new Lazy<BlockDecl>(() => TranslationUnit.GetOrCreate<BlockDecl>(Handle.Referenced));
    }

    public BlockDecl BlockDecl => _blockDecl.Value;

    public Stmt? Body => BlockDecl.Body;

    public FunctionProtoType FunctionType => (FunctionProtoType)((BlockPointerType)Type).PointeeType;
}
