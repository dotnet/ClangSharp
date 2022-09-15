// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using System;
using System.Collections.Generic;

namespace ClangSharp;

public sealed partial class BlockDecl : Decl, IDeclContext
{
    private readonly Lazy<Decl> _blockManglingContextDecl;
    private readonly Lazy<IReadOnlyList<Capture>> _captures;
    private readonly Lazy<IReadOnlyList<ParmVarDecl>> _parameters;

    internal BlockDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedDecl, CX_DeclKind.CX_DeclKind_Block)
    {
        _blockManglingContextDecl = new Lazy<Decl>(() => TranslationUnit.GetOrCreate<Decl>(Handle.BlockManglingContextDecl));
        _captures = new Lazy<IReadOnlyList<Capture>>(() => {
            var captureCount = Handle.NumCaptures;
            var captures = new List<Capture>(captureCount);

            for (var i = 0; i < captureCount; i++)
            {
                var capture = new Capture(this, unchecked((uint)i));
                captures.Add(capture);
            }

            return captures;
        });
        _parameters = new Lazy<IReadOnlyList<ParmVarDecl>>(() => {
            var parameterCount = Handle.NumArguments;
            var parameters = new List<ParmVarDecl>(parameterCount);

            for (var i = 0; i < parameterCount; i++)
            {
                var parameter = TranslationUnit.GetOrCreate<ParmVarDecl>(Handle.GetArgument(unchecked((uint)i)));
                parameters.Add(parameter);
            }

            return parameters;
        });
    }

    public Decl BlockManglingContextDecl => _blockManglingContextDecl.Value;

    public uint BlockManglingNumber => unchecked((uint)Handle.BlockManglingNumber);

    public bool BlockMissingReturnType => Handle.BlockMissingReturnType;

    public IReadOnlyList<Capture> Captures => _captures.Value;

    public bool CanAvoidCopyToHeap() => Handle.CanAvoidCopyToHeap;

    public bool CapturesCXXThis => Handle.CapturesCXXThis;

    public CompoundStmt CompoundBody => (CompoundStmt)Body;

    public bool DoesNotEscape => Handle.DoesNotEscape;

    public bool HasCaptures => NumCaptures != 0;

    public bool IsConversionFromLambda => Handle.IsConversionFromLambda;

    public bool IsVariadic => Handle.IsVariadic;

    public uint NumCaptures => unchecked((uint)Handle.NumCaptures);

    public uint NumParams => unchecked((uint)Handle.NumArguments);

    public IReadOnlyList<ParmVarDecl> Parameters => _parameters.Value;

    public bool ParamEmpty => ParamSize == 0;

    public nuint ParamSize => NumParams;

    public bool CapturesVariable(VarDecl var) => Handle.CapturesVariable(var.Handle);
}
