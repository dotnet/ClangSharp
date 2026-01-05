// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class ObjCProtocolDecl : ObjCContainerDecl, IRedeclarable<ObjCProtocolDecl>
{
    private readonly ValueLazy<ObjCProtocolDecl> _definition;
    private readonly LazyList<ObjCProtocolDecl> _protocols;

    internal ObjCProtocolDecl(CXCursor handle) : base(handle, CXCursor_ObjCProtocolDecl, CX_DeclKind_ObjCProtocol)
    {
        _definition = new ValueLazy<ObjCProtocolDecl>(() => TranslationUnit.GetOrCreate<ObjCProtocolDecl>(Handle.Definition));
        _protocols = LazyList.Create<ObjCProtocolDecl>(Handle.NumProtocols, (i) => TranslationUnit.GetOrCreate<ObjCProtocolDecl>(Handle.GetProtocol(unchecked((uint)i))));
    }

    public new ObjCProtocolDecl CanonicalDecl => (ObjCProtocolDecl)base.CanonicalDecl;

    public ObjCProtocolDecl Definition => _definition.Value;

    public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

    public string ObjCRuntimeNameAsString => Handle.Name.CString;

    public IReadOnlyList<ObjCProtocolDecl> Protocols => _protocols;

    public uint ProtocolSize => unchecked((uint)Handle.NumProtocols);

    public IReadOnlyList<ObjCProtocolDecl> ReferencedProtocols => Protocols;
}
