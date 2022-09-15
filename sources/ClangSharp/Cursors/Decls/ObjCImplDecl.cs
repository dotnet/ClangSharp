// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp;

public class ObjCImplDecl : ObjCContainerDecl
{
    private readonly Lazy<ObjCInterfaceDecl> _classInterface;
    private readonly Lazy<IReadOnlyList<ObjCPropertyImplDecl>> _propertyImpls;

    private protected ObjCImplDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind.CX_DeclKind_LastObjCImpl or < CX_DeclKind.CX_DeclKind_FirstObjCImpl)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _classInterface = new Lazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.GetSubDecl(0)));
        _propertyImpls = new Lazy<IReadOnlyList<ObjCPropertyImplDecl>>(() => Decls.OfType<ObjCPropertyImplDecl>().ToList());
    }

    public ObjCInterfaceDecl ClassInterface => _classInterface.Value;

    public IReadOnlyList<ObjCPropertyImplDecl> PropertyImpls => _propertyImpls.Value;
}
