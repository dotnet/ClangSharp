// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class BlockExpr : Expr
{
    private ValueLazy<BlockExpr, BlockDecl> _blockDecl;

    internal unsafe BlockExpr(CXCursor handle) : base(handle, CXCursor_BlockExpr, CX_StmtClass_BlockExpr)
    {
        Debug.Assert(NumChildren is 0);
        _blockDecl = new ValueLazy<BlockExpr, BlockDecl>(&BlockDeclFactory);
    }

    public BlockDecl BlockDecl => _blockDecl.GetValue(this);

    public Stmt? Body => BlockDecl.Body;

    public FunctionProtoType FunctionType => (FunctionProtoType)((BlockPointerType)Type).PointeeType;

    private static unsafe BlockDecl BlockDeclFactory(BlockExpr self) => self.TranslationUnit.GetOrCreate<BlockDecl>(self.Handle.Referenced);
}
