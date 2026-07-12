// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class ObjCImplementationDecl : ObjCImplDecl
{
    private readonly LazyList<Expr> _initExprs;
    private ValueLazy<ObjCImplementationDecl, List<ObjCIvarDecl>> _ivars;
    private ValueLazy<ObjCImplementationDecl, ObjCInterfaceDecl> _superClass;

    internal unsafe ObjCImplementationDecl(CXCursor handle) : base(handle, CXCursor_ObjCImplementationDecl, CX_DeclKind_ObjCImplementation)
    {
        _initExprs = LazyList.Create<Expr>(Handle.NumExprs, (i) => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(unchecked((uint)i))));
        _ivars = new ValueLazy<ObjCImplementationDecl, List<ObjCIvarDecl>>(&IvarsFactory);
        _superClass = new ValueLazy<ObjCImplementationDecl, ObjCInterfaceDecl>(&SuperClassFactory);
    }

    public IReadOnlyList<Expr> InitExprs => _initExprs;

    public IReadOnlyList<ObjCIvarDecl> Ivars => _ivars.GetValue(this);

    public uint NumIvarInitializers => unchecked((uint)_ivars.GetValue(this).Count);

    public ObjCInterfaceDecl SuperClass => _superClass.GetValue(this);

    private static unsafe ObjCInterfaceDecl SuperClassFactory(ObjCImplementationDecl self) => self.TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(self.Handle.GetSubDecl(1));

    private static unsafe List<ObjCIvarDecl> IvarsFactory(ObjCImplementationDecl self) => [.. self.Decls.OfType<ObjCIvarDecl>()];
}
