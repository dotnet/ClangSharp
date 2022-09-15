// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class ObjCCompatibleAliasDecl : NamedDecl
{
    private readonly Lazy<ObjCInterfaceDecl> _classInterface;

    internal ObjCCompatibleAliasDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_ObjCCompatibleAlias)
    {

        _classInterface = new Lazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.GetSubDecl(0)));
    }

    public ObjCInterfaceDecl ClassInterface => _classInterface.Value;
}
