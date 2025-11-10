// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class ObjCPropertyDecl : NamedDecl
{
    private readonly ValueLazy<ObjCMethodDecl> _getterMethodDecl;
    private readonly ValueLazy<ObjCIvarDecl> _propertyIvarDecl;
    private readonly ValueLazy<ObjCMethodDecl> _setterMethodDecl;
    private readonly ValueLazy<Type> _type;

    internal ObjCPropertyDecl(CXCursor handle) : base(handle, CXCursor_ObjCPropertyDecl, CX_DeclKind_ObjCProperty)
    {
        _getterMethodDecl = new ValueLazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.GetSubDecl(0)));
        _propertyIvarDecl = new ValueLazy<ObjCIvarDecl>(() => TranslationUnit.GetOrCreate<ObjCIvarDecl>(Handle.GetSubDecl(1)));
        _setterMethodDecl = new ValueLazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.GetSubDecl(2)));
        _type = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ReturnType));
    }

    public ObjCMethodDecl GetterMethodDecl => _getterMethodDecl.Value;

    public bool IsClassProperty => (PropertyAttributes & CXObjCPropertyAttrKind.CXObjCPropertyAttr_class) != 0;

    public bool IsInstanceProperty => !IsClassProperty;

#pragma warning disable CA1721
    // CA1721: The property name 'PropertyAttributes' is confusing given the existence of method 'GetPropertyAttributes'.
    // Handle.GetObjCPropertyAttributes() calls Cursor_getObjCPropertyAttributes, which confusingly is implemented using
    // 'getPropertyAttributeAsWritten()' (and not 'getPropertyAttributes()'): https://github.com/llvm/llvm-project/blob/1cea4a0841dacefa49241538a55fbf4f34462633/clang/tools/libclang/CIndex.cpp#L9159-L9165
    // We have to keep our 'PropertyAttributes' property for backwards compatibility, so introduce
    // a new method, GetPropertyAttributes(), that gets the final property attributes, and not just the as written variety.

    /// <summary>This calls ObjCPropertyDecl->getPropertyAttributesAsWritten()</summary>
    public CXObjCPropertyAttrKind PropertyAttributes => Handle.GetObjCPropertyAttributes(0);

    /// <summary>This calls ObjCPropertyDecl->getPropertyAttributes()</summary>
    public ObjCPropertyAttributeKind GetPropertyAttributes () => Handle.GetPropertyAttributes();
#pragma warning restore CA1721

    public ObjCIvarDecl PropertyIvarDecl => _propertyIvarDecl.Value;

    public ObjCMethodDecl SetterMethodDecl => _setterMethodDecl.Value;

    public Type Type => _type.Value;
}
