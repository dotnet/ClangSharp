// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class ObjCCompatibleAliasDecl : NamedDecl
{
    private ValueLazy<ObjCCompatibleAliasDecl, ObjCInterfaceDecl> _classInterface;

    internal unsafe ObjCCompatibleAliasDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_ObjCCompatibleAlias)
    {

        _classInterface = new ValueLazy<ObjCCompatibleAliasDecl, ObjCInterfaceDecl>(&ClassInterfaceFactory);
    }

    public ObjCInterfaceDecl ClassInterface => _classInterface.GetValue(this);

    private static unsafe ObjCInterfaceDecl ClassInterfaceFactory(ObjCCompatibleAliasDecl self) => self.TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(self.Handle.GetSubDecl(0));
}
