// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class ObjCInterfaceDecl : ObjCContainerDecl, IRedeclarable<ObjCInterfaceDecl>
{
    private ValueLazy<ObjCInterfaceDecl, List<ObjCCategoryDecl>> _categoryList;
    private ValueLazy<ObjCInterfaceDecl, ObjCInterfaceDecl> _definition;
    private ValueLazy<ObjCInterfaceDecl, ObjCImplementationDecl> _implementation;
    private ValueLazy<ObjCInterfaceDecl, List<ObjCIvarDecl>> _ivars;
    private ValueLazy<ObjCInterfaceDecl, List<ObjCCategoryDecl>> _knownExtensions;
    private readonly LazyList<ObjCProtocolDecl> _protocols;
    private ValueLazy<ObjCInterfaceDecl, ObjCInterfaceDecl> _superClass;
    private ValueLazy<ObjCInterfaceDecl, ObjCObjectType> _superClassType;
    private ValueLazy<ObjCInterfaceDecl, Type> _typeForDecl;
    private readonly LazyList<ObjCTypeParamDecl> _typeParamList;
    private ValueLazy<ObjCInterfaceDecl, List<ObjCCategoryDecl>> _visibleCategories;
    private ValueLazy<ObjCInterfaceDecl, List<ObjCCategoryDecl>> _visibleExtensions;

    internal unsafe ObjCInterfaceDecl(CXCursor handle) : base(handle, CXCursor_ObjCInterfaceDecl, CX_DeclKind_ObjCInterface)
    {
        _categoryList = new ValueLazy<ObjCInterfaceDecl, List<ObjCCategoryDecl>>(&CategoryListFactory);

        _definition = new ValueLazy<ObjCInterfaceDecl, ObjCInterfaceDecl>(&DefinitionFactory);
        _implementation = new ValueLazy<ObjCInterfaceDecl, ObjCImplementationDecl>(&ImplementationFactory);
        _ivars = new ValueLazy<ObjCInterfaceDecl, List<ObjCIvarDecl>>(&IvarsFactory);
        _knownExtensions = new ValueLazy<ObjCInterfaceDecl, List<ObjCCategoryDecl>>(&KnownExtensionsFactory);
        _protocols = LazyList.Create<ObjCProtocolDecl>(Handle.NumProtocols, (i) => TranslationUnit.GetOrCreate<ObjCProtocolDecl>(Handle.GetProtocol(unchecked((uint)i))));
        _superClass = new ValueLazy<ObjCInterfaceDecl, ObjCInterfaceDecl>(&SuperClassFactory);
        _superClassType = new ValueLazy<ObjCInterfaceDecl, ObjCObjectType>(&SuperClassTypeFactory);
        _typeForDecl = new ValueLazy<ObjCInterfaceDecl, Type>(&TypeForDeclFactory);
        _typeParamList = LazyList.Create<ObjCTypeParamDecl>(Handle.NumTypeParams, (i) => TranslationUnit.GetOrCreate<ObjCTypeParamDecl>(Handle.GetTypeParam(unchecked((uint)i))));
        _visibleCategories = new ValueLazy<ObjCInterfaceDecl, List<ObjCCategoryDecl>>(&VisibleCategoriesFactory);
        _visibleExtensions = new ValueLazy<ObjCInterfaceDecl, List<ObjCCategoryDecl>>(&VisibleExtensionsFactory);
    }

    public new ObjCInterfaceDecl CanonicalDecl => (ObjCInterfaceDecl)base.CanonicalDecl;

    public IReadOnlyList<ObjCCategoryDecl> CategoryList => _categoryList.GetValue(this);

    public ObjCInterfaceDecl Definition => _definition.GetValue(this);

    public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

    public ObjCImplementationDecl Implementation => _implementation.GetValue(this);

    public IReadOnlyList<ObjCIvarDecl> Ivars => _ivars.GetValue(this);

    public IReadOnlyList<ObjCCategoryDecl> KnownCategories => CategoryList;

    public IReadOnlyList<ObjCCategoryDecl> KnownExtensions => _knownExtensions.GetValue(this);

    public ObjCInterfaceDecl SuperClass => _superClass.GetValue(this);

    public ObjCObjectType SuperClassType => _superClassType.GetValue(this);

    public IReadOnlyList<ObjCProtocolDecl> Protocols => _protocols;

    public IReadOnlyList<ObjCProtocolDecl> ReferencedProtocols => Protocols;

    public IReadOnlyList<ObjCTypeParamDecl> TypeParamList => _typeParamList;

    public Type TypeForDecl => _typeForDecl.GetValue(this);

    public IReadOnlyList<ObjCCategoryDecl> VisibleCategories => _visibleCategories.GetValue(this);

    public IReadOnlyList<ObjCCategoryDecl> VisibleExtensions => _visibleExtensions.GetValue(this);

    private static unsafe List<ObjCCategoryDecl> VisibleExtensionsFactory(ObjCInterfaceDecl self) => [.. self.CategoryList.Where((category) => category.IsClassExtension && category.IsUnconditionallyVisible)];

    private static unsafe List<ObjCCategoryDecl> VisibleCategoriesFactory(ObjCInterfaceDecl self) => [.. self.CategoryList.Where((category) => category.IsUnconditionallyVisible)];

    private static unsafe Type TypeForDeclFactory(ObjCInterfaceDecl self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ThisType);

    private static unsafe ObjCObjectType SuperClassTypeFactory(ObjCInterfaceDecl self) => self.TranslationUnit.GetOrCreate<ObjCObjectType>(self.Handle.TypeOperand);

    private static unsafe ObjCInterfaceDecl SuperClassFactory(ObjCInterfaceDecl self) => self.TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(self.Handle.GetSubDecl(2));

    private static unsafe List<ObjCCategoryDecl> KnownExtensionsFactory(ObjCInterfaceDecl self) => [.. self.CategoryList.Where((category) => category.IsClassExtension)];

    private static unsafe List<ObjCIvarDecl> IvarsFactory(ObjCInterfaceDecl self) => [.. self.Decls.OfType<ObjCIvarDecl>()];

    private static unsafe ObjCImplementationDecl ImplementationFactory(ObjCInterfaceDecl self) => self.TranslationUnit.GetOrCreate<ObjCImplementationDecl>(self.Handle.GetSubDecl(1));

    private static unsafe ObjCInterfaceDecl DefinitionFactory(ObjCInterfaceDecl self) => self.TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(self.Handle.Definition);

    private static unsafe List<ObjCCategoryDecl> CategoryListFactory(ObjCInterfaceDecl self) {
            var categories = new List<ObjCCategoryDecl>();

            var category = self.TranslationUnit.GetOrCreate<ObjCCategoryDecl>(self.Handle.GetSubDecl(0));

            while (category is not null)
            {
                categories.Add(category);
                category = category.NextClassCategoryRaw;
            }

            return categories;
        }
}
