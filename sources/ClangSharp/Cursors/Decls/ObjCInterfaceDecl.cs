// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;
using System.Linq;

namespace ClangSharp;

public sealed class ObjCInterfaceDecl : ObjCContainerDecl, IRedeclarable<ObjCInterfaceDecl>
{
    private readonly Lazy<IReadOnlyList<ObjCCategoryDecl>> _categoryList;
    private readonly Lazy<ObjCInterfaceDecl> _definition;
    private readonly Lazy<ObjCImplementationDecl> _implementation;
    private readonly Lazy<IReadOnlyList<ObjCIvarDecl>> _ivars;
    private readonly Lazy<IReadOnlyList<ObjCCategoryDecl>> _knownExtensions;
    private readonly Lazy<IReadOnlyList<ObjCProtocolDecl>> _protocols;
    private readonly Lazy<ObjCInterfaceDecl> _superClass;
    private readonly Lazy<ObjCObjectType> _superClassType;
    private readonly Lazy<Type> _typeForDecl;
    private readonly Lazy<IReadOnlyList<ObjCTypeParamDecl>> _typeParamList;
    private readonly Lazy<IReadOnlyList<ObjCCategoryDecl>> _visibleCategories;
    private readonly Lazy<IReadOnlyList<ObjCCategoryDecl>> _visibleExtensions;

    internal ObjCInterfaceDecl(CXCursor handle) : base(handle, CXCursor_ObjCInterfaceDecl, CX_DeclKind_ObjCInterface)
    {
        _categoryList = new Lazy<IReadOnlyList<ObjCCategoryDecl>>(() => {
            var categories = new List<ObjCCategoryDecl>();

            var category = TranslationUnit.GetOrCreate<ObjCCategoryDecl>(handle.GetSubDecl(0));

            while (category is not null)
            {
                categories.Add(category);
                category = category.NextClassCategoryRaw;
            }

            return categories;
        });

        _definition = new Lazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.Definition));
        _implementation = new Lazy<ObjCImplementationDecl>(() => TranslationUnit.GetOrCreate<ObjCImplementationDecl>(Handle.GetSubDecl(1)));
        _ivars = new Lazy<IReadOnlyList<ObjCIvarDecl>>(() => Decls.OfType<ObjCIvarDecl>().ToList());
        _knownExtensions = new Lazy<IReadOnlyList<ObjCCategoryDecl>>(() => CategoryList.Where((category) => category.IsClassExtension).ToList());

        _protocols = new Lazy<IReadOnlyList<ObjCProtocolDecl>>(() => {
            var numProtocols = Handle.NumProtocols;
            var protocols = new List<ObjCProtocolDecl>(numProtocols);

            for (var i = 0; i < numProtocols; i++)
            {
                var protocol = TranslationUnit.GetOrCreate<ObjCProtocolDecl>(Handle.GetProtocol(unchecked((uint)i)));
                protocols.Add(protocol);
            }

            return protocols;
        });

        _superClass = new Lazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.GetSubDecl(2)));
        _superClassType = new Lazy<ObjCObjectType>(() => TranslationUnit.GetOrCreate<ObjCObjectType>(Handle.TypeOperand));
        _typeForDecl = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ThisType));

        _typeParamList = new Lazy<IReadOnlyList<ObjCTypeParamDecl>>(() => {
            var numTypeParams = Handle.NumArguments;
            var typeParams = new List<ObjCTypeParamDecl>(numTypeParams);

            for (var i = 0; i < numTypeParams; i++)
            {
                var typeParam = TranslationUnit.GetOrCreate<ObjCTypeParamDecl>(Handle.GetArgument(unchecked((uint)i)));
                typeParams.Add(typeParam);
            }

            return typeParams;
        });

        _visibleCategories = new Lazy<IReadOnlyList<ObjCCategoryDecl>>(() => CategoryList.Where((category) => category.IsUnconditionallyVisible).ToList());
        _visibleExtensions = new Lazy<IReadOnlyList<ObjCCategoryDecl>>(() => CategoryList.Where((category) => category.IsClassExtension && category.IsUnconditionallyVisible).ToList());
    }

    public new ObjCInterfaceDecl CanonicalDecl => (ObjCInterfaceDecl)base.CanonicalDecl;

    public IReadOnlyList<ObjCCategoryDecl> CategoryList => _categoryList.Value;

    public ObjCInterfaceDecl Definition => _definition.Value;

    public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

    public ObjCImplementationDecl Implementation => _implementation.Value;

    public IReadOnlyList<ObjCIvarDecl> Ivars => _ivars.Value;

    public IReadOnlyList<ObjCCategoryDecl> KnownCategories => CategoryList;

    public IReadOnlyList<ObjCCategoryDecl> KnownExtensions => _knownExtensions.Value;

    public ObjCInterfaceDecl SuperClass => _superClass.Value;

    public ObjCObjectType SuperClassType => _superClassType.Value;

    public IReadOnlyList<ObjCProtocolDecl> Protocols => _protocols.Value;

    public IReadOnlyList<ObjCProtocolDecl> ReferencedProtocols => Protocols;

    public IReadOnlyList<ObjCTypeParamDecl> TypeParamList => _typeParamList.Value;

    public Type TypeForDecl => _typeForDecl.Value;

    public IReadOnlyList<ObjCCategoryDecl> VisibleCategories => _visibleCategories.Value;

    public IReadOnlyList<ObjCCategoryDecl> VisibleExtensions => _visibleExtensions.Value;
}
