// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCPropertyImplDecl : Decl
    {
        private readonly Lazy<Expr> _getterCXXConstructor;
        private readonly Lazy<ObjCMethodDecl> _getterMethodDecl;
        private readonly Lazy<ObjCPropertyDecl> _propertyDecl;
        private readonly Lazy<ObjCIvarDecl> _propertyIvarDecl;
        private readonly Lazy<ObjCMethodDecl> _setterMethodDecl;
        private readonly Lazy<Expr> _setterCXXAssignment;

        internal ObjCPropertyImplDecl(CXCursor handle) : base(handle, handle.Kind, CX_DeclKind.CX_DeclKind_ObjCPropertyImpl)
        {
            if (handle.Kind is not CXCursorKind.CXCursor_ObjCSynthesizeDecl and not CXCursorKind.CXCursor_ObjCDynamicDecl)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }

            _getterCXXConstructor = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(0)));
            _getterMethodDecl = new Lazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.GetSubDecl(0)));
            _propertyDecl = new Lazy<ObjCPropertyDecl>(() => TranslationUnit.GetOrCreate<ObjCPropertyDecl>(Handle.GetSubDecl(1)));
            _propertyIvarDecl = new Lazy<ObjCIvarDecl>(() => TranslationUnit.GetOrCreate<ObjCIvarDecl>(Handle.GetSubDecl(2)));
            _setterMethodDecl = new Lazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.GetSubDecl(3)));
            _setterCXXAssignment = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(1)));
        }

        public Expr GetterCXXConstructor => _getterCXXConstructor.Value;

        public ObjCMethodDecl GetterMethodDecl => _getterMethodDecl.Value;

        public ObjCPropertyDecl PropertyDecl => _propertyDecl.Value;

        public ObjCIvarDecl PropertyIvarDecl => _propertyIvarDecl.Value;

        public Expr SetterCXXAssignment => _setterCXXAssignment.Value;

        public ObjCMethodDecl SetterMethodDecl => _setterMethodDecl.Value;
    }
}
