// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class ObjCMethodDecl : NamedDecl, IDeclContext
{
    private readonly ValueLazy<ObjCInterfaceDecl> _classInterface;
    private readonly ValueLazy<ImplicitParamDecl> _cmdDecl;
    private readonly LazyList<ParmVarDecl> _parameters;
    private readonly ValueLazy<Type> _returnType;
    private readonly ValueLazy<ImplicitParamDecl> _selfDecl;
    private readonly ValueLazy<Type> _sendResultType;

    internal ObjCMethodDecl(CXCursor handle) : base(handle, handle.Kind, CX_DeclKind_ObjCMethod)
    {
        if (handle.Kind is not CXCursor_ObjCInstanceMethodDecl and not CXCursor_ObjCClassMethodDecl)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _classInterface = new ValueLazy<ObjCInterfaceDecl>(() => TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(Handle.GetSubDecl(0)));
        _cmdDecl = new ValueLazy<ImplicitParamDecl>(() => TranslationUnit.GetOrCreate<ImplicitParamDecl>(Handle.GetSubDecl(1)));
        _parameters = LazyList.Create<ParmVarDecl>(Handle.NumArguments, (i) => TranslationUnit.GetOrCreate<ParmVarDecl>(Handle.GetArgument(unchecked((uint)i))));
        _selfDecl = new ValueLazy<ImplicitParamDecl>(() => TranslationUnit.GetOrCreate<ImplicitParamDecl>(Handle.GetSubDecl(2)));
        _returnType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ReturnType));
        _sendResultType = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
    }

    public new ObjCMethodDecl CanonicalDecl => (ObjCMethodDecl)base.CanonicalDecl;

    public ObjCInterfaceDecl ClassInterface => _classInterface.Value;

    public ImplicitParamDecl CmdDecl => _cmdDecl.Value;

    public bool IsClassMethod => CursorKind == CXCursor_ObjCClassMethodDecl;

    public bool IsInstanceMethod => CursorKind == CXCursor_ObjCInstanceMethodDecl;

    public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

    public CXObjCDeclQualifierKind ObjCDeclQualifier => Handle.ObjCDeclQualifiers;

    public IReadOnlyList<ParmVarDecl> Parameters => _parameters;

    public Type ReturnType => _returnType.Value;

    public ImplicitParamDecl SelfDecl => _selfDecl.Value;

    public Type SendResultType => _sendResultType.Value;
}
