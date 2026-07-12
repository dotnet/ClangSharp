// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class SizeOfPackExpr : Expr
{
    private ValueLazy<SizeOfPackExpr, NamedDecl> _pack;
    private readonly LazyList<TemplateArgument> _partialArguments;

    internal unsafe SizeOfPackExpr(CXCursor handle) : base(handle, CXCursor_SizeOfPackExpr, CX_StmtClass_SizeOfPackExpr)
    {
        _pack = new ValueLazy<SizeOfPackExpr, NamedDecl>(&PackFactory);
        _partialArguments = LazyList.Create<TemplateArgument>(Handle.NumTemplateArguments, (i) => TranslationUnit.GetOrCreate(Handle.GetTemplateArgument(unchecked((uint)i))));
    }

    public NamedDecl Pack => _pack.GetValue(this);

    public uint PackLength => unchecked((uint)Handle.PackLength);

    public bool IsPartiallySubstituted => Handle.IsPartiallySubstituted;

    public IReadOnlyList<TemplateArgument> PartialArguments => _partialArguments;

    private static unsafe NamedDecl PackFactory(SizeOfPackExpr self) => self.TranslationUnit.GetOrCreate<NamedDecl>(self.Handle.Referenced);
}
