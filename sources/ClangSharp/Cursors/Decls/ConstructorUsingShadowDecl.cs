// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class ConstructorUsingShadowDecl : UsingShadowDecl
{
    private readonly ValueLazy<CXXRecordDecl> _constructedBaseClass;
    private readonly ValueLazy<ConstructorUsingShadowDecl> _constructedBaseClassShadowDecl;
    private readonly ValueLazy<CXXRecordDecl> _nominatedBaseClass;
    private readonly ValueLazy<ConstructorUsingShadowDecl> _nominatedBaseClassShadowDecl;

    internal ConstructorUsingShadowDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_ConstructorUsingShadow)
    {
        _constructedBaseClass = new ValueLazy<CXXRecordDecl>(() => TranslationUnit.GetOrCreate<CXXRecordDecl>(Handle.ConstructedBaseClass));
        _constructedBaseClassShadowDecl = new ValueLazy<ConstructorUsingShadowDecl>(() => TranslationUnit.GetOrCreate<ConstructorUsingShadowDecl>(Handle.ConstructedBaseClassShadowDecl));
        _nominatedBaseClass = new ValueLazy<CXXRecordDecl>(() => TranslationUnit.GetOrCreate<CXXRecordDecl>(Handle.NominatedBaseClass));
        _nominatedBaseClassShadowDecl = new ValueLazy<ConstructorUsingShadowDecl>(() => TranslationUnit.GetOrCreate<ConstructorUsingShadowDecl>(Handle.NominatedBaseClassShadowDecl));
    }

    public CXXRecordDecl ConstructedBaseClass => _constructedBaseClass.Value;

    public ConstructorUsingShadowDecl ConstructedBaseClassShadowDecl => _constructedBaseClassShadowDecl.Value;

    public bool ConstructsVirtualBase => Handle.ConstructsVirtualBase;

    public CXXRecordDecl NominatedBaseClass => _nominatedBaseClass.Value;

    public ConstructorUsingShadowDecl NominatedBaseClassShadowDecl => _nominatedBaseClassShadowDecl.Value;

    public new CXXRecordDecl? Parent => (CXXRecordDecl?)DeclContext;
}
