// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCProtocolDecl : ObjCContainerDecl, IRedeclarable<ObjCProtocolDecl>
    {
        private readonly Lazy<ObjCProtocolDecl> _definition;
        private readonly Lazy<IReadOnlyList<ObjCProtocolDecl>> _protocols;

        internal ObjCProtocolDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCProtocolDecl, CX_DeclKind.CX_DeclKind_ObjCProtocol)
        {
            _definition = new Lazy<ObjCProtocolDecl>(() => TranslationUnit.GetOrCreate<ObjCProtocolDecl>(Handle.Definition));

            _protocols = new Lazy<IReadOnlyList<ObjCProtocolDecl>>(() => {
                var numProtocols = Handle.NumProtocols;
                var protocols = new List<ObjCProtocolDecl>(numProtocols);

                for (int i = 0; i < numProtocols; i++)
                {
                    var protocol = TranslationUnit.GetOrCreate<ObjCProtocolDecl>(Handle.GetProtocol(unchecked((uint)i)));
                    protocols.Add(protocol);
                }

                return protocols;
            });
        }

        public new ObjCProtocolDecl CanonicalDecl => (ObjCProtocolDecl)base.CanonicalDecl;

        public ObjCProtocolDecl Definition => _definition.Value;

        public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

        public string ObjCRuntimeNameAsString => Handle.Name.CString;

        public IReadOnlyList<ObjCProtocolDecl> Protocols => _protocols.Value;

        public uint ProtocolSize => unchecked((uint)Handle.NumProtocols);

        public IReadOnlyList<ObjCProtocolDecl> ReferencedProtocols => Protocols;
    }
}
