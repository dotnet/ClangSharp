// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public class CXXMethodDecl : FunctionDecl
{
    private readonly LazyList<CXXMethodDecl> _overriddenMethods;
    private ValueLazy<CXXMethodDecl, Type> _thisType;
    private ValueLazy<CXXMethodDecl, Type> _thisObjectType;

    internal CXXMethodDecl(CXCursor handle) : this(handle, CXCursor_CXXMethod, CX_DeclKind_CXXMethod)
    {
    }

    private protected unsafe CXXMethodDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastCXXMethod or < CX_DeclKind_FirstCXXMethod)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _overriddenMethods = LazyList.Create<CXXMethodDecl>(Handle.NumMethods, (i) => TranslationUnit.GetOrCreate<CXXMethodDecl>(Handle.GetMethod(unchecked((uint)i))));
        _thisType = new ValueLazy<CXXMethodDecl, Type>(&ThisTypeFactory);
        _thisObjectType = new ValueLazy<CXXMethodDecl, Type>(&ThisObjectTypeFactory);
    }

    public new CXXMethodDecl CanonicalDecl => (CXXMethodDecl)base.CanonicalDecl;

    public bool IsConst => Handle.CXXMethod_IsConst;

    public bool IsCopyAssignmentOperator => Handle.CXXMethod_IsCopyAssignmentOperator;

    public bool IsMoveAssignmentOperator => Handle.CXXMethod_IsMoveAssignmentOperator;

    public bool IsVirtual => Handle.CXXMethod_IsVirtual;

    public new CXXMethodDecl MostRecentDecl => (CXXMethodDecl)base.MostRecentDecl;

    public IReadOnlyList<CXXMethodDecl> OverriddenMethods => _overriddenMethods;

    public new CXXRecordDecl? Parent => (CXXRecordDecl?)(base.Parent ?? ThisObjectType.AsCXXRecordDecl);

    public uint SizeOverriddenMethods => unchecked((uint)Handle.NumMethods);

    public Type ThisType => _thisType.GetValue(this);

    public Type ThisObjectType => _thisObjectType.GetValue(this);

    public long VtblIndex => IsVirtual ? Handle.VtblIdx : -1;

    private static unsafe Type ThisObjectTypeFactory(CXXMethodDecl self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ThisObjectType);

    private static unsafe Type ThisTypeFactory(CXXMethodDecl self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ThisType);
}
