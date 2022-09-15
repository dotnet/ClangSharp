// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp;

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

            for (var i = 0; i < numProtocols; i++)
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
