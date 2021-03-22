// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class UnresolvedMemberExpr : OverloadExpr
    {
        private readonly Lazy<Type> _baseType;

        internal UnresolvedMemberExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_MemberRefExpr, CX_StmtClass.CX_StmtClass_UnresolvedMemberExpr)
        {
            Debug.Assert(NumChildren is 0 or 1);
            _baseType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
        }

        public Expr Base => (Expr)Children.SingleOrDefault();

        public Type BaseType => _baseType.Value;

        public bool IsImplicitAccess => NumChildren == 0;

        public bool IsArrow => Handle.IsArrow;

        public string MemberName => Handle.Name.CString;
    }
}
