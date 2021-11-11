// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class ObjCContainerDecl : NamedDecl, IDeclContext
    {
        private readonly Lazy<IReadOnlyList<ObjCMethodDecl>> _classMethods;
        private readonly Lazy<IReadOnlyList<ObjCPropertyDecl>> _classProperties;
        private readonly Lazy<IReadOnlyList<ObjCMethodDecl>> _instanceMethods;
        private readonly Lazy<IReadOnlyList<ObjCPropertyDecl>> _instanceProperties;
        private readonly Lazy<IReadOnlyList<ObjCMethodDecl>> _methods;
        private readonly Lazy<IReadOnlyList<ObjCPropertyDecl>> _properties;

        private protected ObjCContainerDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if (handle.DeclKind is > CX_DeclKind.CX_DeclKind_LastObjCContainer or < CX_DeclKind.CX_DeclKind_FirstObjCContainer)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }

            _classMethods = new Lazy<IReadOnlyList<ObjCMethodDecl>>(() => Methods.Where((method) => method.IsClassMethod).ToList());
            _classProperties = new Lazy<IReadOnlyList<ObjCPropertyDecl>>(() => Properties.Where((property) => property.IsClassProperty).ToList());
            _instanceMethods = new Lazy<IReadOnlyList<ObjCMethodDecl>>(() => Methods.Where((method) => method.IsInstanceMethod).ToList());
            _instanceProperties = new Lazy<IReadOnlyList<ObjCPropertyDecl>>(() => Properties.Where((property) => property.IsInstanceProperty).ToList());
            _methods = new Lazy<IReadOnlyList<ObjCMethodDecl>>(() => Decls.OfType<ObjCMethodDecl>().ToList());
            _properties = new Lazy<IReadOnlyList<ObjCPropertyDecl>>(() => Decls.OfType<ObjCPropertyDecl>().ToList());
        }

        public IReadOnlyList<ObjCMethodDecl> ClassMethods => _classMethods.Value;

        public IReadOnlyList<ObjCPropertyDecl> ClassProperties => _classProperties.Value;

        public IReadOnlyList<ObjCMethodDecl> InstanceMethods => _instanceMethods.Value;

        public IReadOnlyList<ObjCPropertyDecl> InstanceProperties => _instanceProperties.Value;

        public IReadOnlyList<ObjCMethodDecl> Methods => _methods.Value;

        public IReadOnlyList<ObjCPropertyDecl> Properties => _properties.Value;
    }
}
