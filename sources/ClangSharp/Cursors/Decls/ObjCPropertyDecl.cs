// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCPropertyDecl : NamedDecl
    {
        private readonly Lazy<ObjCMethodDecl> _getterMethodDecl;
        private readonly Lazy<ObjCIvarDecl> _propertyIvarDecl;
        private readonly Lazy<ObjCMethodDecl> _setterMethodDecl;
        private readonly Lazy<Type> _type;

        internal ObjCPropertyDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCPropertyDecl, CX_DeclKind.CX_DeclKind_ObjCProperty)
        {
            _getterMethodDecl = new Lazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.GetSubDecl(0)));
            _propertyIvarDecl = new Lazy<ObjCIvarDecl>(() => TranslationUnit.GetOrCreate<ObjCIvarDecl>(Handle.GetSubDecl(1)));
            _setterMethodDecl = new Lazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.GetSubDecl(2)));
            _type = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ReturnType));
        }

        public ObjCMethodDecl GetterMethodDecl => _getterMethodDecl.Value;

        public ObjCIvarDecl PropertyIvarDecl => _propertyIvarDecl.Value;

        public ObjCMethodDecl SetterMethodDecl => _setterMethodDecl.Value;

        public Type Type => _type.Value;
    }
}
