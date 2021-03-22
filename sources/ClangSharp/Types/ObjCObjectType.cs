// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class ObjCObjectType : Type
    {
        private readonly Lazy<Type> _baseType;
        private readonly Lazy<ObjCInterfaceDecl> _interface;
        private readonly Lazy<IReadOnlyList<ObjCProtocolDecl>> _protocols;
        private readonly Lazy<Type> _superClassType;
        private readonly Lazy<IReadOnlyList<Type>> _typeArgs;

        internal ObjCObjectType(CXType handle) : this(handle, CXTypeKind.CXType_ObjCObject, CX_TypeClass.CX_TypeClass_ObjCObject)
        {
        }

        private protected ObjCObjectType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
        {
            _baseType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ObjCObjectBaseType));
            _interface = new Lazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.Declaration));

            _protocols = new Lazy<IReadOnlyList<ObjCProtocolDecl>>(() => {
                var numProtocols = unchecked((int)Handle.NumObjCProtocolRefs);
                var protocols = new List<ObjCProtocolDecl>(numProtocols);

                for (var i = 0; i < numProtocols; i++)
                {
                    var protocol = TranslationUnit.GetOrCreate<ObjCProtocolDecl>(Handle.GetObjCProtocolDecl(unchecked((uint)i)));
                    protocols.Add(protocol);
                }

                return protocols;
            });

            _superClassType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.UnderlyingType));
            _typeArgs = new Lazy<IReadOnlyList<Type>>(() => {
                var numTypeArgs = unchecked((int)Handle.NumObjCTypeArgs);
                var typeArgs = new List<Type>(numTypeArgs);

                for (var i = 0; i < numTypeArgs; i++)
                {
                    var typeArg = TranslationUnit.GetOrCreate<Type>(Handle.GetObjCTypeArg(unchecked((uint)i)));
                    typeArgs.Add(typeArg);
                }

                return typeArgs;
            });
        }

        public Type BaseType => _baseType.Value;

        public ObjCInterfaceDecl Interface => _interface.Value;

        public IReadOnlyList<ObjCProtocolDecl> Protocols => _protocols.Value;

        public Type SuperClassType => _superClassType.Value;

        public IReadOnlyList<Type> TypeArgs => _typeArgs.Value;
    }
}
