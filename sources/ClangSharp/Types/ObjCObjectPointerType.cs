// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class ObjCObjectPointerType : Type
{
    private ValueLazy<ObjCObjectPointerType, ObjCInterfaceType> _interfaceType;
    private ValueLazy<ObjCObjectPointerType, Type> _superClassType;

    internal unsafe ObjCObjectPointerType(CXType handle) : base(handle, CXType_ObjCObjectPointer, CX_TypeClass_ObjCObjectPointer)
    {
        _interfaceType = new ValueLazy<ObjCObjectPointerType, ObjCInterfaceType>(&InterfaceTypeFactory);
        _superClassType = new ValueLazy<ObjCObjectPointerType, Type>(&SuperClassTypeFactory);
    }

    public ObjCInterfaceDecl InterfaceDecl => ObjectType.Interface;

    public ObjCInterfaceType InterfaceType => _interfaceType.GetValue(this);

    public ObjCObjectType ObjectType => PointeeType.CastAs<ObjCObjectType>();

    public IReadOnlyList<ObjCProtocolDecl> Protocols => ObjectType.Protocols;

    public Type SuperClassType => _superClassType.GetValue(this);

    public IReadOnlyList<Type> TypeArgs => ObjectType.TypeArgs;

    private static unsafe Type SuperClassTypeFactory(ObjCObjectPointerType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.UnderlyingType);

    private static unsafe ObjCInterfaceType InterfaceTypeFactory(ObjCObjectPointerType self) => self.TranslationUnit.GetOrCreate<ObjCInterfaceType>(self.Handle.OriginalType);
}
