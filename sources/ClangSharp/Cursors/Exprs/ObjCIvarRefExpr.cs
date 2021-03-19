// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCIvarRefExpr : Expr
    {
        private readonly Lazy<ObjCIvarDecl> _decl;

        internal ObjCIvarRefExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_MemberRefExpr, CX_StmtClass.CX_StmtClass_ObjCIvarRefExpr)
        {
            Debug.Assert(NumChildren is 1);
            _decl = new Lazy<ObjCIvarDecl>(() => TranslationUnit.GetOrCreate<ObjCIvarDecl>(Handle.Referenced));
        }

        public Expr Base => (Expr)Children[0];

        public ObjCIvarDecl Decl => _decl.Value;

        public bool IsArrow => Handle.IsArrow;
    }
}
