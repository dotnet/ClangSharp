// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class ObjCPropertyImplDecl : Decl
{
    private ValueLazy<ObjCPropertyImplDecl, Expr> _getterCXXConstructor;
    private ValueLazy<ObjCPropertyImplDecl, ObjCMethodDecl> _getterMethodDecl;
    private ValueLazy<ObjCPropertyImplDecl, ObjCPropertyDecl> _propertyDecl;
    private ValueLazy<ObjCPropertyImplDecl, ObjCIvarDecl> _propertyIvarDecl;
    private ValueLazy<ObjCPropertyImplDecl, ObjCMethodDecl> _setterMethodDecl;
    private ValueLazy<ObjCPropertyImplDecl, Expr> _setterCXXAssignment;

    internal unsafe ObjCPropertyImplDecl(CXCursor handle) : base(handle, handle.Kind, CX_DeclKind_ObjCPropertyImpl)
    {
        if (handle.Kind is not CXCursor_ObjCSynthesizeDecl and not CXCursor_ObjCDynamicDecl)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _getterCXXConstructor = new ValueLazy<ObjCPropertyImplDecl, Expr>(&GetterCXXConstructorFactory);
        _getterMethodDecl = new ValueLazy<ObjCPropertyImplDecl, ObjCMethodDecl>(&GetterMethodDeclFactory);
        _propertyDecl = new ValueLazy<ObjCPropertyImplDecl, ObjCPropertyDecl>(&PropertyDeclFactory);
        _propertyIvarDecl = new ValueLazy<ObjCPropertyImplDecl, ObjCIvarDecl>(&PropertyIvarDeclFactory);
        _setterMethodDecl = new ValueLazy<ObjCPropertyImplDecl, ObjCMethodDecl>(&SetterMethodDeclFactory);
        _setterCXXAssignment = new ValueLazy<ObjCPropertyImplDecl, Expr>(&SetterCXXAssignmentFactory);
    }

    public Expr GetterCXXConstructor => _getterCXXConstructor.GetValue(this);

    public ObjCMethodDecl GetterMethodDecl => _getterMethodDecl.GetValue(this);

    public ObjCPropertyDecl PropertyDecl => _propertyDecl.GetValue(this);

    public ObjCIvarDecl PropertyIvarDecl => _propertyIvarDecl.GetValue(this);

    public Expr SetterCXXAssignment => _setterCXXAssignment.GetValue(this);

    public ObjCMethodDecl SetterMethodDecl => _setterMethodDecl.GetValue(this);

    private static unsafe Expr SetterCXXAssignmentFactory(ObjCPropertyImplDecl self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.GetExpr(1));

    private static unsafe ObjCMethodDecl SetterMethodDeclFactory(ObjCPropertyImplDecl self) => self.TranslationUnit.GetOrCreate<ObjCMethodDecl>(self.Handle.GetSubDecl(3));

    private static unsafe ObjCIvarDecl PropertyIvarDeclFactory(ObjCPropertyImplDecl self) => self.TranslationUnit.GetOrCreate<ObjCIvarDecl>(self.Handle.GetSubDecl(2));

    private static unsafe ObjCPropertyDecl PropertyDeclFactory(ObjCPropertyImplDecl self) => self.TranslationUnit.GetOrCreate<ObjCPropertyDecl>(self.Handle.GetSubDecl(1));

    private static unsafe ObjCMethodDecl GetterMethodDeclFactory(ObjCPropertyImplDecl self) => self.TranslationUnit.GetOrCreate<ObjCMethodDecl>(self.Handle.GetSubDecl(0));

    private static unsafe Expr GetterCXXConstructorFactory(ObjCPropertyImplDecl self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.GetExpr(0));
}
