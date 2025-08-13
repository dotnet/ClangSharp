// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class ObjCImplDecl : ObjCContainerDecl
{
    private readonly ValueLazy<ObjCInterfaceDecl> _classInterface;
    private readonly ValueLazy<List<ObjCPropertyImplDecl>> _propertyImpls;

    private protected ObjCImplDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastObjCImpl or < CX_DeclKind_FirstObjCImpl)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _classInterface = new ValueLazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.GetSubDecl(0)));
        _propertyImpls = new ValueLazy<List<ObjCPropertyImplDecl>>(() => [.. Decls.OfType<ObjCPropertyImplDecl>()]);
    }

    public ObjCInterfaceDecl ClassInterface => _classInterface.Value;

    public IReadOnlyList<ObjCPropertyImplDecl> PropertyImpls => _propertyImpls.Value;
}
