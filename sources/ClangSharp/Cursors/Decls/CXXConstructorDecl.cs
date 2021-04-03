// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXConstructorDecl : CXXMethodDecl
    {
        private readonly Lazy<CXXConstructorDecl> _inheritedConstructor;
        private readonly Lazy<IReadOnlyList<Expr>> _initExprs;

        internal CXXConstructorDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_Constructor, CX_DeclKind.CX_DeclKind_CXXConstructor)
        {
            _inheritedConstructor = new Lazy<CXXConstructorDecl>(() => TranslationUnit.GetOrCreate<CXXConstructorDecl>(Handle.InheritedConstructor));
            _initExprs = new Lazy<IReadOnlyList<Expr>>(() => {
                var numInitExprs = Handle.NumExprs;
                var initExprs = new List<Expr>(numInitExprs);

                for (var i = 0; i < numInitExprs; i++)
                {
                    var initExpr = TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(unchecked((uint)i)));
                    initExprs.Add(initExpr);
                }

                return initExprs;
            });
        }

        public new CXXConstructorDecl CanonicalDecl => (CXXConstructorDecl)base.CanonicalDecl;

        public CXXConstructorDecl InheritedConstructor => _inheritedConstructor.Value;

        public IReadOnlyList<Expr> InitExprs => _initExprs.Value;

        public bool IsConvertingConstructor => Handle.CXXConstructor_IsConvertingConstructor;

        public bool IsCopyConstructor => Handle.CXXConstructor_IsCopyConstructor;

        public bool IsCopyOrMoveConstructor => Handle.IsCopyOrMoveConstructor;

        public bool IsDefaultConstructor => Handle.CXXConstructor_IsDefaultConstructor;

        public bool IsDelegatingConstructor => Handle.IsDelegatingConstructor;

        public bool IsExplicit => !Handle.IsImplicit;

        public bool IsInheritingConstructor => Handle.IsInheritingConstructor;

        public bool IsMoveConstructor => Handle.CXXConstructor_IsMoveConstructor;

        public uint NumCtorInitializers => unchecked((uint)Handle.NumExprs);

        public CXXConstructorDecl TargetConstructor
        {
            get
            {
                if (!IsDelegatingConstructor)
                {
                    return null;
                }

                var e = InitExprs.FirstOrDefault()?.IgnoreImplicit;

                return e is CXXConstructExpr construct ? construct.Constructor : null;
            }
        }
    }
}
