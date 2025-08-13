// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class ObjCContainerDecl : NamedDecl, IDeclContext
{
    private readonly ValueLazy<List<ObjCMethodDecl>> _classMethods;
    private readonly ValueLazy<List<ObjCPropertyDecl>> _classProperties;
    private readonly ValueLazy<List<ObjCMethodDecl>> _instanceMethods;
    private readonly ValueLazy<List<ObjCPropertyDecl>> _instanceProperties;
    private readonly ValueLazy<List<ObjCMethodDecl>> _methods;
    private readonly ValueLazy<List<ObjCPropertyDecl>> _properties;

    private protected ObjCContainerDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastObjCContainer or < CX_DeclKind_FirstObjCContainer)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _classMethods = new ValueLazy<List<ObjCMethodDecl>>(() => [.. Methods.Where((method) => method.IsClassMethod)]);
        _classProperties = new ValueLazy<List<ObjCPropertyDecl>>(() => [.. Properties.Where((property) => property.IsClassProperty)]);
        _instanceMethods = new ValueLazy<List<ObjCMethodDecl>>(() => [.. Methods.Where((method) => method.IsInstanceMethod)]);
        _instanceProperties = new ValueLazy<List<ObjCPropertyDecl>>(() => [.. Properties.Where((property) => property.IsInstanceProperty)]);
        _methods = new ValueLazy<List<ObjCMethodDecl>>(() => [.. Decls.OfType<ObjCMethodDecl>()]);
        _properties = new ValueLazy<List<ObjCPropertyDecl>>(() => [.. Decls.OfType<ObjCPropertyDecl>()]);
    }

    public IReadOnlyList<ObjCMethodDecl> ClassMethods => _classMethods.Value;

    public IReadOnlyList<ObjCPropertyDecl> ClassProperties => _classProperties.Value;

    public IReadOnlyList<ObjCMethodDecl> InstanceMethods => _instanceMethods.Value;

    public IReadOnlyList<ObjCPropertyDecl> InstanceProperties => _instanceProperties.Value;

    public IReadOnlyList<ObjCMethodDecl> Methods => _methods.Value;

    public IReadOnlyList<ObjCPropertyDecl> Properties => _properties.Value;
}
