// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class BlockExpr : Expr
    {
        private readonly Lazy<BlockDecl> _blockDecl;

        internal BlockExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_BlockExpr, CX_StmtClass.CX_StmtClass_BlockExpr)
        {
            Debug.Assert(NumChildren is 0);
            _blockDecl = new Lazy<BlockDecl>(() => TranslationUnit.GetOrCreate<BlockDecl>(Handle.Referenced));
        }

        public BlockDecl BlockDecl => _blockDecl.Value;

        public Stmt Body => BlockDecl.Body;

        public FunctionProtoType FunctionType => (FunctionProtoType)((BlockPointerType)Type).PointeeType;
    }
}
