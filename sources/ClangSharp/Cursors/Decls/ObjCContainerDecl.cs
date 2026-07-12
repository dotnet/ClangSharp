// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class ObjCContainerDecl : NamedDecl, IDeclContext
{
    private ValueLazy<ObjCContainerDecl, List<ObjCMethodDecl>> _classMethods;
    private ValueLazy<ObjCContainerDecl, List<ObjCPropertyDecl>> _classProperties;
    private ValueLazy<ObjCContainerDecl, List<ObjCMethodDecl>> _instanceMethods;
    private ValueLazy<ObjCContainerDecl, List<ObjCPropertyDecl>> _instanceProperties;
    private ValueLazy<ObjCContainerDecl, List<ObjCMethodDecl>> _methods;
    private ValueLazy<ObjCContainerDecl, List<ObjCPropertyDecl>> _properties;

    private protected unsafe ObjCContainerDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastObjCContainer or < CX_DeclKind_FirstObjCContainer)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _classMethods = new ValueLazy<ObjCContainerDecl, List<ObjCMethodDecl>>(&ClassMethodsFactory);
        _classProperties = new ValueLazy<ObjCContainerDecl, List<ObjCPropertyDecl>>(&ClassPropertiesFactory);
        _instanceMethods = new ValueLazy<ObjCContainerDecl, List<ObjCMethodDecl>>(&InstanceMethodsFactory);
        _instanceProperties = new ValueLazy<ObjCContainerDecl, List<ObjCPropertyDecl>>(&InstancePropertiesFactory);
        _methods = new ValueLazy<ObjCContainerDecl, List<ObjCMethodDecl>>(&MethodsFactory);
        _properties = new ValueLazy<ObjCContainerDecl, List<ObjCPropertyDecl>>(&PropertiesFactory);
    }

    public IReadOnlyList<ObjCMethodDecl> ClassMethods => _classMethods.GetValue(this);

    public IReadOnlyList<ObjCPropertyDecl> ClassProperties => _classProperties.GetValue(this);

    public IReadOnlyList<ObjCMethodDecl> InstanceMethods => _instanceMethods.GetValue(this);

    public IReadOnlyList<ObjCPropertyDecl> InstanceProperties => _instanceProperties.GetValue(this);

    public IReadOnlyList<ObjCMethodDecl> Methods => _methods.GetValue(this);

    public IReadOnlyList<ObjCPropertyDecl> Properties => _properties.GetValue(this);

    private static unsafe List<ObjCPropertyDecl> PropertiesFactory(ObjCContainerDecl self) => [.. self.Decls.OfType<ObjCPropertyDecl>()];

    private static unsafe List<ObjCMethodDecl> MethodsFactory(ObjCContainerDecl self) => [.. self.Decls.OfType<ObjCMethodDecl>()];

    private static unsafe List<ObjCPropertyDecl> InstancePropertiesFactory(ObjCContainerDecl self) => [.. self.Properties.Where((property) => property.IsInstanceProperty)];

    private static unsafe List<ObjCMethodDecl> InstanceMethodsFactory(ObjCContainerDecl self) => [.. self.Methods.Where((method) => method.IsInstanceMethod)];

    private static unsafe List<ObjCPropertyDecl> ClassPropertiesFactory(ObjCContainerDecl self) => [.. self.Properties.Where((property) => property.IsClassProperty)];

    private static unsafe List<ObjCMethodDecl> ClassMethodsFactory(ObjCContainerDecl self) => [.. self.Methods.Where((method) => method.IsClassMethod)];
}
