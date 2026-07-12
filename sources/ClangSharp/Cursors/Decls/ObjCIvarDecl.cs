// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class ObjCIvarDecl : FieldDecl
{
    private ValueLazy<ObjCIvarDecl, ObjCInterfaceDecl> _containingInterface;
    private ValueLazy<ObjCIvarDecl, ObjCIvarDecl> _nextIvar;

    internal unsafe ObjCIvarDecl(CXCursor handle) : base(handle, CXCursor_ObjCIvarDecl, CX_DeclKind_ObjCIvar)
    {
        _containingInterface = new ValueLazy<ObjCIvarDecl, ObjCInterfaceDecl>(&ContainingInterfaceFactory);
        _nextIvar = new ValueLazy<ObjCIvarDecl, ObjCIvarDecl>(&NextIvarFactory);
    }

    public ObjCInterfaceDecl ContainingInterface => _containingInterface.GetValue(this);

    public ObjCIvarDecl NextIvar => _nextIvar.GetValue(this);

    private static unsafe ObjCIvarDecl NextIvarFactory(ObjCIvarDecl self) => self.TranslationUnit.GetOrCreate<ObjCIvarDecl>(self.Handle.GetSubDecl(1));

    private static unsafe ObjCInterfaceDecl ContainingInterfaceFactory(ObjCIvarDecl self) => self.TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(self.Handle.GetSubDecl(0));
}
