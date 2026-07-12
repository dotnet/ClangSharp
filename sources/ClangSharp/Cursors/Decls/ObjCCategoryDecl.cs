// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class ObjCCategoryDecl : ObjCContainerDecl
{
    private ValueLazy<ObjCCategoryDecl, ObjCInterfaceDecl> _classInterface;
    private ValueLazy<ObjCCategoryDecl, ObjCCategoryImplDecl> _implementation;
    private ValueLazy<ObjCCategoryDecl, List<ObjCIvarDecl>> _ivars;
    private ValueLazy<ObjCCategoryDecl, ObjCCategoryDecl> _nextClassCategory;
    private ValueLazy<ObjCCategoryDecl, ObjCCategoryDecl> _nextClassCategoryRaw;
    private readonly LazyList<ObjCProtocolDecl> _protocols;
    private readonly LazyList<ObjCTypeParamDecl> _typeParamList;

    internal unsafe ObjCCategoryDecl(CXCursor handle) : base(handle, CXCursor_ObjCCategoryDecl, CX_DeclKind_ObjCCategory)
    {
        _classInterface = new ValueLazy<ObjCCategoryDecl, ObjCInterfaceDecl>(&ClassInterfaceFactory);
        _implementation = new ValueLazy<ObjCCategoryDecl, ObjCCategoryImplDecl>(&ImplementationFactory);
        _ivars = new ValueLazy<ObjCCategoryDecl, List<ObjCIvarDecl>>(&IvarsFactory);
        _nextClassCategory = new ValueLazy<ObjCCategoryDecl, ObjCCategoryDecl>(&NextClassCategoryFactory);
        _nextClassCategoryRaw = new ValueLazy<ObjCCategoryDecl, ObjCCategoryDecl>(&NextClassCategoryRawFactory);
        _protocols = LazyList.Create<ObjCProtocolDecl>(Handle.NumProtocols, (i) => TranslationUnit.GetOrCreate<ObjCProtocolDecl>(Handle.GetProtocol(unchecked((uint)i))));
        _typeParamList = LazyList.Create<ObjCTypeParamDecl>(Handle.NumTypeParams, (i) => TranslationUnit.GetOrCreate<ObjCTypeParamDecl>(Handle.GetTypeParam(unchecked((uint)i))));
    }

    public ObjCInterfaceDecl ClassInterface => _classInterface.GetValue(this);

    public ObjCCategoryImplDecl Implementation => _implementation.GetValue(this);

    public bool IsClassExtension => Handle.IsClassExtension;

    public IReadOnlyList<ObjCIvarDecl> Ivars => _ivars.GetValue(this);

    public ObjCCategoryDecl NextClassCategory => _nextClassCategory.GetValue(this);

    public ObjCCategoryDecl NextClassCategoryRaw => _nextClassCategoryRaw.GetValue(this);

    public IReadOnlyList<ObjCProtocolDecl> Protocols => _protocols;

    public IReadOnlyList<ObjCProtocolDecl> ReferencedProtocols => Protocols;

    public IReadOnlyList<ObjCTypeParamDecl> TypeParamList => _typeParamList;

    private static unsafe ObjCCategoryDecl NextClassCategoryRawFactory(ObjCCategoryDecl self) => self.TranslationUnit.GetOrCreate<ObjCCategoryDecl>(self.Handle.GetSubDecl(3));

    private static unsafe ObjCCategoryDecl NextClassCategoryFactory(ObjCCategoryDecl self) => self.TranslationUnit.GetOrCreate<ObjCCategoryDecl>(self.Handle.GetSubDecl(2));

    private static unsafe List<ObjCIvarDecl> IvarsFactory(ObjCCategoryDecl self) => [.. self.Decls.OfType<ObjCIvarDecl>()];

    private static unsafe ObjCCategoryImplDecl ImplementationFactory(ObjCCategoryDecl self) => self.TranslationUnit.GetOrCreate<ObjCCategoryImplDecl>(self.Handle.GetSubDecl(1));

    private static unsafe ObjCInterfaceDecl ClassInterfaceFactory(ObjCCategoryDecl self) => self.TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(self.Handle.GetSubDecl(0));
}
