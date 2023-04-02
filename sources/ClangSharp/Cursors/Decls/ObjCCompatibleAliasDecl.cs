// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class ObjCCompatibleAliasDecl : NamedDecl
{
    private readonly Lazy<ObjCInterfaceDecl> _classInterface;

    internal ObjCCompatibleAliasDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_ObjCCompatibleAlias)
    {

        _classInterface = new Lazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.GetSubDecl(0)));
    }

    public ObjCInterfaceDecl ClassInterface => _classInterface.Value;
}
