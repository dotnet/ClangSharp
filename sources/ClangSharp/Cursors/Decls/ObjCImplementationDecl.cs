// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class ObjCImplementationDecl : ObjCImplDecl
{
    private readonly LazyList<Expr> _initExprs;
    private readonly ValueLazy<List<ObjCIvarDecl>> _ivars;
    private readonly ValueLazy<ObjCInterfaceDecl> _superClass;

    internal ObjCImplementationDecl(CXCursor handle) : base(handle, CXCursor_ObjCImplementationDecl, CX_DeclKind_ObjCImplementation)
    {
        _initExprs = LazyList.Create<Expr>(Handle.NumExprs, (i) => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(unchecked((uint)i))));
        _ivars = new ValueLazy<List<ObjCIvarDecl>>(() => [.. Decls.OfType<ObjCIvarDecl>()]);
        _superClass = new ValueLazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.GetSubDecl(1)));
    }

    public IReadOnlyList<Expr> InitExprs => _initExprs;

    public IReadOnlyList<ObjCIvarDecl> Ivars => _ivars.Value;

    public uint NumIvarInitializers => unchecked((uint)_ivars.Value.Count);

    public ObjCInterfaceDecl SuperClass => _superClass.Value;
}
