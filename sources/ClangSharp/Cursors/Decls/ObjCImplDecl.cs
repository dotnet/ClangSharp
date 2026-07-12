// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class ObjCImplDecl : ObjCContainerDecl
{
    private ValueLazy<ObjCImplDecl, ObjCInterfaceDecl> _classInterface;
    private ValueLazy<ObjCImplDecl, List<ObjCPropertyImplDecl>> _propertyImpls;

    private protected unsafe ObjCImplDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastObjCImpl or < CX_DeclKind_FirstObjCImpl)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _classInterface = new ValueLazy<ObjCImplDecl, ObjCInterfaceDecl>(&ClassInterfaceFactory);
        _propertyImpls = new ValueLazy<ObjCImplDecl, List<ObjCPropertyImplDecl>>(&PropertyImplsFactory);
    }

    public ObjCInterfaceDecl ClassInterface => _classInterface.GetValue(this);

    public IReadOnlyList<ObjCPropertyImplDecl> PropertyImpls => _propertyImpls.GetValue(this);

    private static unsafe List<ObjCPropertyImplDecl> PropertyImplsFactory(ObjCImplDecl self) => [.. self.Decls.OfType<ObjCPropertyImplDecl>()];

    private static unsafe ObjCInterfaceDecl ClassInterfaceFactory(ObjCImplDecl self) => self.TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(self.Handle.GetSubDecl(0));
}
