// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class ObjCCategoryDecl : ObjCContainerDecl
{
    private readonly ValueLazy<ObjCInterfaceDecl> _classInterface;
    private readonly ValueLazy<ObjCCategoryImplDecl> _implementation;
    private readonly ValueLazy<List<ObjCIvarDecl>> _ivars;
    private readonly ValueLazy<ObjCCategoryDecl> _nextClassCategory;
    private readonly ValueLazy<ObjCCategoryDecl> _nextClassCategoryRaw;
    private readonly LazyList<ObjCProtocolDecl> _protocols;
    private readonly LazyList<ObjCTypeParamDecl> _typeParamList;

    internal ObjCCategoryDecl(CXCursor handle) : base(handle, CXCursor_ObjCCategoryDecl, CX_DeclKind_ObjCCategory)
    {
        _classInterface = new ValueLazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.GetSubDecl(0)));
        _implementation = new ValueLazy<ObjCCategoryImplDecl>(() => TranslationUnit.GetOrCreate<ObjCCategoryImplDecl>(Handle.GetSubDecl(1)));
        _ivars = new ValueLazy<List<ObjCIvarDecl>>(() => [.. Decls.OfType<ObjCIvarDecl>()]);
        _nextClassCategory = new ValueLazy<ObjCCategoryDecl>(() => TranslationUnit.GetOrCreate<ObjCCategoryDecl>(Handle.GetSubDecl(2)));
        _nextClassCategoryRaw = new ValueLazy<ObjCCategoryDecl>(() => TranslationUnit.GetOrCreate<ObjCCategoryDecl>(Handle.GetSubDecl(3)));
        _protocols = LazyList.Create<ObjCProtocolDecl>(Handle.NumProtocols, (i) => TranslationUnit.GetOrCreate<ObjCProtocolDecl>(Handle.GetProtocol(unchecked((uint)i))));
        _typeParamList = LazyList.Create<ObjCTypeParamDecl>(Handle.NumArguments, (i) => TranslationUnit.GetOrCreate<ObjCTypeParamDecl>(Handle.GetArgument(unchecked((uint)i))));
    }

    public ObjCInterfaceDecl ClassInterface => _classInterface.Value;

    public ObjCCategoryImplDecl Implementation => _implementation.Value;

    public bool IsClassExtension => Handle.IsClassExtension;

    public IReadOnlyList<ObjCIvarDecl> Ivars => _ivars.Value;

    public ObjCCategoryDecl NextClassCategory => _nextClassCategory.Value;

    public ObjCCategoryDecl NextClassCategoryRaw => _nextClassCategoryRaw.Value;

    public IReadOnlyList<ObjCProtocolDecl> Protocols => _protocols;

    public IReadOnlyList<ObjCProtocolDecl> ReferencedProtocols => Protocols;

    public IReadOnlyList<ObjCTypeParamDecl> TypeParamList => _typeParamList;
}
