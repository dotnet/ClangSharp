// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class ConstructorUsingShadowDecl : UsingShadowDecl
{
    private ValueLazy<ConstructorUsingShadowDecl, CXXRecordDecl> _constructedBaseClass;
    private ValueLazy<ConstructorUsingShadowDecl, ConstructorUsingShadowDecl> _constructedBaseClassShadowDecl;
    private ValueLazy<ConstructorUsingShadowDecl, CXXRecordDecl> _nominatedBaseClass;
    private ValueLazy<ConstructorUsingShadowDecl, ConstructorUsingShadowDecl> _nominatedBaseClassShadowDecl;

    internal unsafe ConstructorUsingShadowDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_ConstructorUsingShadow)
    {
        _constructedBaseClass = new ValueLazy<ConstructorUsingShadowDecl, CXXRecordDecl>(&ConstructedBaseClassFactory);
        _constructedBaseClassShadowDecl = new ValueLazy<ConstructorUsingShadowDecl, ConstructorUsingShadowDecl>(&ConstructedBaseClassShadowDeclFactory);
        _nominatedBaseClass = new ValueLazy<ConstructorUsingShadowDecl, CXXRecordDecl>(&NominatedBaseClassFactory);
        _nominatedBaseClassShadowDecl = new ValueLazy<ConstructorUsingShadowDecl, ConstructorUsingShadowDecl>(&NominatedBaseClassShadowDeclFactory);
    }

    public CXXRecordDecl ConstructedBaseClass => _constructedBaseClass.GetValue(this);

    public ConstructorUsingShadowDecl ConstructedBaseClassShadowDecl => _constructedBaseClassShadowDecl.GetValue(this);

    public bool ConstructsVirtualBase => Handle.ConstructsVirtualBase;

    public CXXRecordDecl NominatedBaseClass => _nominatedBaseClass.GetValue(this);

    public ConstructorUsingShadowDecl NominatedBaseClassShadowDecl => _nominatedBaseClassShadowDecl.GetValue(this);

    public new CXXRecordDecl? Parent => (CXXRecordDecl?)DeclContext;

    private static unsafe ConstructorUsingShadowDecl NominatedBaseClassShadowDeclFactory(ConstructorUsingShadowDecl self) => self.TranslationUnit.GetOrCreate<ConstructorUsingShadowDecl>(self.Handle.NominatedBaseClassShadowDecl);

    private static unsafe CXXRecordDecl NominatedBaseClassFactory(ConstructorUsingShadowDecl self) => self.TranslationUnit.GetOrCreate<CXXRecordDecl>(self.Handle.NominatedBaseClass);

    private static unsafe ConstructorUsingShadowDecl ConstructedBaseClassShadowDeclFactory(ConstructorUsingShadowDecl self) => self.TranslationUnit.GetOrCreate<ConstructorUsingShadowDecl>(self.Handle.ConstructedBaseClassShadowDecl);

    private static unsafe CXXRecordDecl ConstructedBaseClassFactory(ConstructorUsingShadowDecl self) => self.TranslationUnit.GetOrCreate<CXXRecordDecl>(self.Handle.ConstructedBaseClass);
}
