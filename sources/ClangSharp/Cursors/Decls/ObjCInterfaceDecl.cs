// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class ObjCInterfaceDecl : ObjCContainerDecl, IRedeclarable<ObjCInterfaceDecl>
{
    private readonly ValueLazy<List<ObjCCategoryDecl>> _categoryList;
    private readonly ValueLazy<ObjCInterfaceDecl> _definition;
    private readonly ValueLazy<ObjCImplementationDecl> _implementation;
    private readonly ValueLazy<List<ObjCIvarDecl>> _ivars;
    private readonly ValueLazy<List<ObjCCategoryDecl>> _knownExtensions;
    private readonly LazyList<ObjCProtocolDecl> _protocols;
    private readonly ValueLazy<ObjCInterfaceDecl> _superClass;
    private readonly ValueLazy<ObjCObjectType> _superClassType;
    private readonly ValueLazy<Type> _typeForDecl;
    private readonly LazyList<ObjCTypeParamDecl> _typeParamList;
    private readonly ValueLazy<List<ObjCCategoryDecl>> _visibleCategories;
    private readonly ValueLazy<List<ObjCCategoryDecl>> _visibleExtensions;

    internal ObjCInterfaceDecl(CXCursor handle) : base(handle, CXCursor_ObjCInterfaceDecl, CX_DeclKind_ObjCInterface)
    {
        _categoryList = new ValueLazy<List<ObjCCategoryDecl>>(() => {
            var categories = new List<ObjCCategoryDecl>();

            var category = TranslationUnit.GetOrCreate<ObjCCategoryDecl>(handle.GetSubDecl(0));

            while (category is not null)
            {
                categories.Add(category);
                category = category.NextClassCategoryRaw;
            }

            return categories;
        });

        _definition = new ValueLazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.Definition));
        _implementation = new ValueLazy<ObjCImplementationDecl>(() => TranslationUnit.GetOrCreate<ObjCImplementationDecl>(Handle.GetSubDecl(1)));
        _ivars = new ValueLazy<List<ObjCIvarDecl>>(() => [.. Decls.OfType<ObjCIvarDecl>()]);
        _knownExtensions = new ValueLazy<List<ObjCCategoryDecl>>(() => [.. CategoryList.Where((category) => category.IsClassExtension)]);
        _protocols = LazyList.Create<ObjCProtocolDecl>(Handle.NumProtocols, (i) => TranslationUnit.GetOrCreate<ObjCProtocolDecl>(Handle.GetProtocol(unchecked((uint)i))));
        _superClass = new ValueLazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.GetSubDecl(2)));
        _superClassType = new ValueLazy<ObjCObjectType>(() => TranslationUnit.GetOrCreate<ObjCObjectType>(Handle.TypeOperand));
        _typeForDecl = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ThisType));
        _typeParamList = LazyList.Create<ObjCTypeParamDecl>(Handle.NumArguments, (i) => TranslationUnit.GetOrCreate<ObjCTypeParamDecl>(Handle.GetArgument(unchecked((uint)i))));
        _visibleCategories = new ValueLazy<List<ObjCCategoryDecl>>(() => [.. CategoryList.Where((category) => category.IsUnconditionallyVisible)]);
        _visibleExtensions = new ValueLazy<List<ObjCCategoryDecl>>(() => [.. CategoryList.Where((category) => category.IsClassExtension && category.IsUnconditionallyVisible)]);
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

    public IReadOnlyList<ObjCProtocolDecl> Protocols => _protocols;

    public IReadOnlyList<ObjCProtocolDecl> ReferencedProtocols => Protocols;

    public IReadOnlyList<ObjCTypeParamDecl> TypeParamList => _typeParamList;

    public Type TypeForDecl => _typeForDecl.Value;

    public IReadOnlyList<ObjCCategoryDecl> VisibleCategories => _visibleCategories.Value;

    public IReadOnlyList<ObjCCategoryDecl> VisibleExtensions => _visibleExtensions.Value;
}
