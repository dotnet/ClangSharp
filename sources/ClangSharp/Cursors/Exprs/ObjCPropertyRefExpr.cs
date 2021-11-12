// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCPropertyRefExpr : Expr
    {
        private readonly Lazy<Expr> _base;
        private readonly Lazy<ObjCInterfaceDecl> _classReceiver;
        private readonly Lazy<ObjCPropertyDecl> _explicitProperty;
        private readonly Lazy<ObjCMethodDecl> _implicitPropertyGetter;
        private readonly Lazy<ObjCMethodDecl> _implicitPropertySetter;
        private readonly Lazy<Type> _superReceiverType;

        internal ObjCPropertyRefExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_MemberRefExpr, CX_StmtClass.CX_StmtClass_ObjCPropertyRefExpr)
        {
            _base = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(0)));
            _classReceiver = new Lazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.GetDecl(0)));
            _explicitProperty = new Lazy<ObjCPropertyDecl>(() => TranslationUnit.GetOrCreate<ObjCPropertyDecl>(Handle.GetDecl(1)));
            _implicitPropertyGetter = new Lazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.GetDecl(2)));
            _implicitPropertySetter = new Lazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.GetDecl(3)));
            _superReceiverType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
        }


        public Expr Base => _base.Value;

        public ObjCInterfaceDecl ClassReceiver => _classReceiver.Value;

        public ObjCPropertyDecl ExplicitProperty => _explicitProperty.Value;

        public bool IsExplicitProperty => !IsImplicitProperty;

        public bool IsImplicitProperty => Handle.IsImplicit;

        public ObjCMethodDecl ImplicitPropertyGetter => _implicitPropertyGetter.Value;

        public ObjCMethodDecl ImplicitPropertySetter => _implicitPropertySetter.Value;

        public Type SuperReceiverType => _superReceiverType.Value;
    }
}
