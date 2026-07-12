// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class CapturedDecl : Decl, IDeclContext
{
    private ValueLazy<CapturedDecl, ImplicitParamDecl> _contextParam;
    private readonly LazyList<ImplicitParamDecl> _parameters;

    internal unsafe CapturedDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_Captured)
    {
        _contextParam = new ValueLazy<CapturedDecl, ImplicitParamDecl>(&ContextParamFactory);
        _parameters = LazyList.Create<ImplicitParamDecl>(Handle.NumArguments, (i) => TranslationUnit.GetOrCreate<ImplicitParamDecl>(Handle.GetArgument(unchecked((uint)i))));
    }

    public ImplicitParamDecl ContextParam => _contextParam.GetValue(this);

    public uint ContextParamPosition => unchecked((uint)Handle.ContextParamPosition);

    public bool IsNothrow => Handle.IsNothrow;

    public uint NumParams => unchecked((uint)Handle.NumArguments);

    public IReadOnlyList<ImplicitParamDecl> Parameters => _parameters;

    private static unsafe ImplicitParamDecl ContextParamFactory(CapturedDecl self) => self.TranslationUnit.GetOrCreate<ImplicitParamDecl>(self.Handle.ContextParam);
}
