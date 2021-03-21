// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class ObjCImplDecl : ObjCContainerDecl
    {
        private readonly Lazy<ObjCInterfaceDecl> _classInterface;
        private readonly Lazy<IReadOnlyList<ObjCPropertyImplDecl>> _propertyImpls;

        private protected ObjCImplDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if ((CX_DeclKind.CX_DeclKind_LastObjCImpl < handle.DeclKind) || (handle.DeclKind < CX_DeclKind.CX_DeclKind_FirstObjCImpl))
            {
                throw new ArgumentException(nameof(handle));
            }

            _classInterface = new Lazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.GetSubDecl(0)));
            _propertyImpls = new Lazy<IReadOnlyList<ObjCPropertyImplDecl>>(() => Decls.OfType<ObjCPropertyImplDecl>().ToList());
        }

        public ObjCInterfaceDecl ClassInterface => _classInterface.Value;

        public IReadOnlyList<ObjCPropertyImplDecl> PropertyImpls => _propertyImpls.Value;
    }
}
