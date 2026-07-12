// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public class ObjCObjectType : Type
{
    private ValueLazy<ObjCObjectType, Type> _baseType;
    private ValueLazy<ObjCObjectType, ObjCInterfaceDecl> _interface;
    private readonly LazyList<ObjCProtocolDecl> _protocols;
    private ValueLazy<ObjCObjectType, Type> _superClassType;
    private readonly LazyList<Type> _typeArgs;

    internal ObjCObjectType(CXType handle) : this(handle, CXType_ObjCObject, CX_TypeClass_ObjCObject)
    {
    }

    private protected unsafe ObjCObjectType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
    {
        _baseType = new ValueLazy<ObjCObjectType, Type>(&BaseTypeFactory);
        _interface = new ValueLazy<ObjCObjectType, ObjCInterfaceDecl>(&InterfaceFactory);
        _protocols = LazyList.Create<ObjCProtocolDecl>(unchecked((int)Handle.NumObjCProtocolRefs), (i) => TranslationUnit.GetOrCreate<ObjCProtocolDecl>(Handle.GetObjCProtocolDecl(unchecked((uint)i))));
        _superClassType = new ValueLazy<ObjCObjectType, Type>(&SuperClassTypeFactory);
        _typeArgs = LazyList.Create<Type>(unchecked((int)Handle.NumObjCTypeArgs), (i) => TranslationUnit.GetOrCreate<Type>(Handle.GetObjCTypeArg(unchecked((uint)i))));
    }

    public Type BaseType => _baseType.GetValue(this);

    public ObjCInterfaceDecl Interface => _interface.GetValue(this);

    public IReadOnlyList<ObjCProtocolDecl> Protocols => _protocols;

    public Type SuperClassType => _superClassType.GetValue(this);

    public IReadOnlyList<Type> TypeArgs => _typeArgs;

    private static unsafe Type SuperClassTypeFactory(ObjCObjectType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.UnderlyingType);

    private static unsafe ObjCInterfaceDecl InterfaceFactory(ObjCObjectType self) => self.TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(self.Handle.Declaration);

    private static unsafe Type BaseTypeFactory(ObjCObjectType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ObjCObjectBaseType);
}
