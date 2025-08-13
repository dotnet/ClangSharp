// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class CXXConstructorDecl : CXXMethodDecl
{
    private readonly ValueLazy<CXXConstructorDecl> _inheritedConstructor;
    private readonly LazyList<Expr> _initExprs;

    internal CXXConstructorDecl(CXCursor handle) : base(handle, CXCursor_Constructor, CX_DeclKind_CXXConstructor)
    {
        _inheritedConstructor = new ValueLazy<CXXConstructorDecl>(() => TranslationUnit.GetOrCreate<CXXConstructorDecl>(Handle.InheritedConstructor));
        _initExprs = LazyList.Create<Expr>(Handle.NumExprs , (i) => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(unchecked((uint)i))));
    }

    public new CXXConstructorDecl CanonicalDecl => (CXXConstructorDecl)base.CanonicalDecl;

    public CXXConstructorDecl InheritedConstructor => _inheritedConstructor.Value;

    public IReadOnlyList<Expr> InitExprs => _initExprs;

    public bool IsConvertingConstructor => Handle.CXXConstructor_IsConvertingConstructor;

    public bool IsCopyConstructor => Handle.CXXConstructor_IsCopyConstructor;

    public bool IsCopyOrMoveConstructor => Handle.IsCopyOrMoveConstructor;

    public bool IsDefaultConstructor => Handle.CXXConstructor_IsDefaultConstructor;

    public bool IsDelegatingConstructor => Handle.IsDelegatingConstructor;

    public bool IsExplicit => !Handle.IsImplicit;

    public bool IsInheritingConstructor => Handle.IsInheritingConstructor;

    public bool IsMoveConstructor => Handle.CXXConstructor_IsMoveConstructor;

    public uint NumCtorInitializers => unchecked((uint)Handle.NumExprs);

    public CXXConstructorDecl? TargetConstructor
    {
        get
        {
            if (!IsDelegatingConstructor)
            {
                return null;
            }

            var initExprs = InitExprs;
            var e = (initExprs.Count != 0) ? initExprs[0].IgnoreImplicit : null;
            return (e is CXXConstructExpr construct) ? construct.Constructor : null;
        }
    }
}
