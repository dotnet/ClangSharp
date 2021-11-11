// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCArrayLiteral : Expr
    {
        private readonly Lazy<ObjCMethodDecl> _arrayWithObjectsMethod;
        private readonly Lazy<IReadOnlyList<Expr>> _elements;

        internal ObjCArrayLiteral(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_ObjCArrayLiteral)
        {
            _arrayWithObjectsMethod = new Lazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.Referenced));
            _elements = new Lazy<IReadOnlyList<Expr>>(() => Children.Cast<Expr>().ToList());
        }

        public ObjCMethodDecl ArrayWithObjectsMethod => _arrayWithObjectsMethod.Value;

        public IReadOnlyList<Expr> Elements => _elements.Value;

        public uint NumElements => NumChildren;
    }
}
