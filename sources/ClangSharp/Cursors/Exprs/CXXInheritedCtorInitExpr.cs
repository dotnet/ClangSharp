// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXInheritedCtorInitExpr : Expr
    {
        private readonly Lazy<CXXConstructorDecl> _constructor;

        internal CXXInheritedCtorInitExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CallExpr, CX_StmtClass.CX_StmtClass_CXXInheritedCtorInitExpr)
        {
            Debug.Assert(NumChildren is 0);
            _constructor = new Lazy<CXXConstructorDecl>(() => TranslationUnit.GetOrCreate<CXXConstructorDecl>(Handle.Referenced));
        }

        public CX_ConstructionKind ConstructionKind => ConstructsVBase ? CX_ConstructionKind.CX_CK_VirtualBase : CX_ConstructionKind.CX_CK_NonVirtualBase;

        public CXXConstructorDecl Constructor => _constructor.Value;

        public bool ConstructsVBase => Handle.ConstructsVirtualBase;

        public bool InheritedFromVBase => Handle.InheritedFromVBase;
    }
}
