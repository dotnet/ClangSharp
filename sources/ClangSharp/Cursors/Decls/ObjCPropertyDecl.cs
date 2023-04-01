// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class ObjCPropertyDecl : NamedDecl
{
    private readonly Lazy<ObjCMethodDecl> _getterMethodDecl;
    private readonly Lazy<ObjCIvarDecl> _propertyIvarDecl;
    private readonly Lazy<ObjCMethodDecl> _setterMethodDecl;
    private readonly Lazy<Type> _type;

    internal ObjCPropertyDecl(CXCursor handle) : base(handle, CXCursor_ObjCPropertyDecl, CX_DeclKind_ObjCProperty)
    {
        _getterMethodDecl = new Lazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.GetSubDecl(0)));
        _propertyIvarDecl = new Lazy<ObjCIvarDecl>(() => TranslationUnit.GetOrCreate<ObjCIvarDecl>(Handle.GetSubDecl(1)));
        _setterMethodDecl = new Lazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.GetSubDecl(2)));
        _type = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ReturnType));
    }

    public ObjCMethodDecl GetterMethodDecl => _getterMethodDecl.Value;

    public bool IsClassProperty => (PropertyAttributes & CXObjCPropertyAttrKind.CXObjCPropertyAttr_class) != 0;

    public bool IsInstanceProperty => !IsClassProperty;

    public CXObjCPropertyAttrKind PropertyAttributes => Handle.GetObjCPropertyAttributes(0);

    public ObjCIvarDecl PropertyIvarDecl => _propertyIvarDecl.Value;

    public ObjCMethodDecl SetterMethodDecl => _setterMethodDecl.Value;

    public Type Type => _type.Value;
}
