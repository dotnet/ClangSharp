// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using System;
using ClangSharp.Interop;
using System.Linq;

namespace ClangSharp
{
    public sealed class ObjCImplementationDecl : ObjCImplDecl
    {
        private readonly Lazy<IReadOnlyList<Expr>> _initExprs;
        private readonly Lazy<IReadOnlyList<ObjCIvarDecl>> _ivars;
        private readonly Lazy<ObjCInterfaceDecl> _superClass;

        internal ObjCImplementationDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCImplementationDecl, CX_DeclKind.CX_DeclKind_ObjCImplementation)
        {
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

            _ivars = new Lazy<IReadOnlyList<ObjCIvarDecl>>(() => Decls.OfType<ObjCIvarDecl>().ToList());
            _superClass = new Lazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.GetSubDecl(1)));
        }

        public IReadOnlyList<Expr> InitExprs => _initExprs.Value;

        public IReadOnlyList<ObjCIvarDecl> Ivars => _ivars.Value;

        public uint NumIvarInitializers => unchecked((uint)_ivars.Value.Count);

        public ObjCInterfaceDecl SuperClass => _superClass.Value;
    }
}
