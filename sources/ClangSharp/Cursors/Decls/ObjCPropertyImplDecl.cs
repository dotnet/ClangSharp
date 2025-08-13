// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class ObjCPropertyImplDecl : Decl
{
    private readonly ValueLazy<Expr> _getterCXXConstructor;
    private readonly ValueLazy<ObjCMethodDecl> _getterMethodDecl;
    private readonly ValueLazy<ObjCPropertyDecl> _propertyDecl;
    private readonly ValueLazy<ObjCIvarDecl> _propertyIvarDecl;
    private readonly ValueLazy<ObjCMethodDecl> _setterMethodDecl;
    private readonly ValueLazy<Expr> _setterCXXAssignment;

    internal ObjCPropertyImplDecl(CXCursor handle) : base(handle, handle.Kind, CX_DeclKind_ObjCPropertyImpl)
    {
        if (handle.Kind is not CXCursor_ObjCSynthesizeDecl and not CXCursor_ObjCDynamicDecl)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _getterCXXConstructor = new ValueLazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(0)));
        _getterMethodDecl = new ValueLazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.GetSubDecl(0)));
        _propertyDecl = new ValueLazy<ObjCPropertyDecl>(() => TranslationUnit.GetOrCreate<ObjCPropertyDecl>(Handle.GetSubDecl(1)));
        _propertyIvarDecl = new ValueLazy<ObjCIvarDecl>(() => TranslationUnit.GetOrCreate<ObjCIvarDecl>(Handle.GetSubDecl(2)));
        _setterMethodDecl = new ValueLazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.GetSubDecl(3)));
        _setterCXXAssignment = new ValueLazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(1)));
    }

    public Expr GetterCXXConstructor => _getterCXXConstructor.Value;

    public ObjCMethodDecl GetterMethodDecl => _getterMethodDecl.Value;

    public ObjCPropertyDecl PropertyDecl => _propertyDecl.Value;

    public ObjCIvarDecl PropertyIvarDecl => _propertyIvarDecl.Value;

    public Expr SetterCXXAssignment => _setterCXXAssignment.Value;

    public ObjCMethodDecl SetterMethodDecl => _setterMethodDecl.Value;
}
