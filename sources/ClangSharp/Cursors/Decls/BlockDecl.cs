// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed partial class BlockDecl : Decl, IDeclContext
{
    private readonly ValueLazy<Decl> _blockManglingContextDecl;
    private readonly LazyList<Capture> _captures;
    private readonly LazyList<ParmVarDecl> _parameters;

    internal BlockDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_Block)
    {
        _blockManglingContextDecl = new ValueLazy<Decl>(() => TranslationUnit.GetOrCreate<Decl>(Handle.BlockManglingContextDecl));
        _captures = LazyList.Create<Capture>(Handle.NumCaptures, (i) => new Capture(this, unchecked((uint)i)));
        _parameters = LazyList.Create<ParmVarDecl>(Handle.NumArguments, (i) => TranslationUnit.GetOrCreate<ParmVarDecl>(Handle.GetArgument(unchecked((uint)i))));
    }

    public Decl BlockManglingContextDecl => _blockManglingContextDecl.Value;

    public uint BlockManglingNumber => unchecked((uint)Handle.BlockManglingNumber);

    public bool BlockMissingReturnType => Handle.BlockMissingReturnType;

    public IReadOnlyList<Capture> Captures => _captures;

    public bool CanAvoidCopyToHeap() => Handle.CanAvoidCopyToHeap;

    public bool CapturesCXXThis => Handle.CapturesCXXThis;

    public CompoundStmt? CompoundBody => (CompoundStmt?)Body;

    public bool DoesNotEscape => Handle.DoesNotEscape;

    public bool HasCaptures => NumCaptures != 0;

    public bool IsConversionFromLambda => Handle.IsConversionFromLambda;

    public bool IsVariadic => Handle.IsVariadic;

    public uint NumCaptures => unchecked((uint)Handle.NumCaptures);

    public uint NumParams => unchecked((uint)Handle.NumArguments);

    public IReadOnlyList<ParmVarDecl> Parameters => _parameters;

    public bool ParamEmpty => ParamSize == 0;

    public nuint ParamSize => NumParams;

    public bool CapturesVariable(VarDecl var) => Handle.CapturesVariable((var is not null) ? var.Handle : CXCursor.Null);
}
