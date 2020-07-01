// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class BlockExpr : Expr
    {
        private readonly Lazy<BlockDecl> _blockDecl;
        private readonly Lazy<Stmt> _body;
        private readonly Lazy<FunctionProtoType> _functionType;

        internal BlockExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_BlockExpr, CX_StmtClass.CX_StmtClass_BlockExpr)
        {
            _blockDecl = new Lazy<BlockDecl>(() => TranslationUnit.GetOrCreate<BlockDecl>(Handle.Referenced));
            _body = new Lazy<Stmt>(() => TranslationUnit.GetOrCreate<Stmt>(Handle.Body));
            _functionType = new Lazy<FunctionProtoType>(() => TranslationUnit.GetOrCreate<FunctionProtoType>(Handle.FunctionType));
        }
    }
}
