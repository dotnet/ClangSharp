// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXNewExpr : Expr
    {
        private readonly Lazy<FunctionDecl> _operatorDelete;
        private readonly Lazy<FunctionDecl> _operatorNew;
        private readonly Lazy<IReadOnlyList<Expr>> _placementArgs;

        internal CXXNewExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXNewExpr, CX_StmtClass.CX_StmtClass_CXXNewExpr)
        {
            _operatorDelete = new Lazy<FunctionDecl>(() => TranslationUnit.GetOrCreate<FunctionDecl>(Handle.GetDecl(0)));
            _operatorNew = new Lazy<FunctionDecl>(() => TranslationUnit.GetOrCreate<FunctionDecl>(Handle.GetDecl(1)));
            _placementArgs = new Lazy<IReadOnlyList<Expr>>(() => Children.Skip(PlacementNewArgsOffset).Cast<Expr>().ToList());
        }

        public Type AllocatedType => ((PointerType)Type).PointeeType;

        public Expr ArraySize => IsArray ? (Expr)Children[ArraySizeOffset] : null;

        public CXXConstructExpr ConstructExpr => Initializer as CXXConstructExpr;

        public bool DoesUsualArrayDeleteWantSize => Handle.DoesUsualArrayDeleteWantSize;

        public bool HasInitializer => Handle.HasInit;

        public Expr Initializer => HasInitializer ? (Expr)Children[InitExprOffset] : null;

        public bool IsArray => Handle.IsArrayForm;

        public bool IsGlobalNew => Handle.IsGlobal;

        public uint NumPlacementArgs => unchecked((uint)(Handle.NumArguments - PlacementNewArgsOffset));

        public FunctionDecl OperatorDelete => _operatorDelete.Value;

        public FunctionDecl OperatorNew => _operatorNew.Value;

        public IReadOnlyList<Expr> PlacementArgs => _placementArgs.Value;

        private static int ArraySizeOffset => 0;

        private int InitExprOffset => ArraySizeOffset + (IsArray ? 1 : 0);

        private int PlacementNewArgsOffset => InitExprOffset + (HasInitializer ? 1 : 0);
    }
}
