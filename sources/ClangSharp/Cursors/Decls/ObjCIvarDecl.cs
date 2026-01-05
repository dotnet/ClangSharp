// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class ObjCIvarDecl : FieldDecl
{
    private readonly ValueLazy<ObjCInterfaceDecl> _containingInterface;
    private readonly ValueLazy<ObjCIvarDecl> _nextIvar;

    internal ObjCIvarDecl(CXCursor handle) : base(handle, CXCursor_ObjCIvarDecl, CX_DeclKind_ObjCIvar)
    {
        _containingInterface = new ValueLazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.GetSubDecl(0)));
        _nextIvar = new ValueLazy<ObjCIvarDecl>(() => TranslationUnit.GetOrCreate<ObjCIvarDecl>(Handle.GetSubDecl(1)));
    }

    public ObjCInterfaceDecl ContainingInterface => _containingInterface.Value;

    public ObjCIvarDecl NextIvar => _nextIvar.Value;
}
