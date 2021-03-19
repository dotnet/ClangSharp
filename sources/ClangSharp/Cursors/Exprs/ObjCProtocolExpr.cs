// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCProtocolExpr : Expr
    {
        private readonly Lazy<ObjCProtocolDecl> _protocol;

        internal ObjCProtocolExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCProtocolExpr, CX_StmtClass.CX_StmtClass_ObjCProtocolExpr)
        {
            Debug.Assert(NumChildren is 0);
            _protocol = new Lazy<ObjCProtocolDecl>(() => TranslationUnit.GetOrCreate<ObjCProtocolDecl>(Handle.Referenced));
        }

        public ObjCProtocolDecl Protocol => _protocol.Value;
    }
}
