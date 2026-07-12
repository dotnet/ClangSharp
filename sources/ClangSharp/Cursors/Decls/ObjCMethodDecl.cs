// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class ObjCMethodDecl : NamedDecl, IDeclContext
{
    private ValueLazy<ObjCMethodDecl, ObjCInterfaceDecl> _classInterface;
    private ValueLazy<ObjCMethodDecl, ImplicitParamDecl> _cmdDecl;
    private readonly LazyList<ParmVarDecl> _parameters;
    private ValueLazy<ObjCMethodDecl, Type> _returnType;
    private ValueLazy<ObjCMethodDecl, ImplicitParamDecl> _selfDecl;
    private ValueLazy<ObjCMethodDecl, Type> _sendResultType;

    internal unsafe ObjCMethodDecl(CXCursor handle) : base(handle, handle.Kind, CX_DeclKind_ObjCMethod)
    {
        if (handle.Kind is not CXCursor_ObjCInstanceMethodDecl and not CXCursor_ObjCClassMethodDecl)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _classInterface = new ValueLazy<ObjCMethodDecl, ObjCInterfaceDecl>(&ClassInterfaceFactory);
        _cmdDecl = new ValueLazy<ObjCMethodDecl, ImplicitParamDecl>(&CmdDeclFactory);
        _parameters = LazyList.Create<ParmVarDecl>(this, Handle.NumArguments, &ParametersFactory);
        _selfDecl = new ValueLazy<ObjCMethodDecl, ImplicitParamDecl>(&SelfDeclFactory);
        _returnType = new ValueLazy<ObjCMethodDecl, Type>(&ReturnTypeFactory);
        _sendResultType = new ValueLazy<ObjCMethodDecl, Type>(&SendResultTypeFactory);
    }

    public new ObjCMethodDecl CanonicalDecl => (ObjCMethodDecl)base.CanonicalDecl;

    public ObjCInterfaceDecl ClassInterface => _classInterface.GetValue(this);

    public ImplicitParamDecl CmdDecl => _cmdDecl.GetValue(this);

    public bool IsClassMethod => CursorKind == CXCursor_ObjCClassMethodDecl;

    public bool IsInstanceMethod => CursorKind == CXCursor_ObjCInstanceMethodDecl;

    public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

    public ObjCMethodFamily MethodFamily => Handle.MethodFamily;

    public CXObjCDeclQualifierKind ObjCDeclQualifier => Handle.ObjCDeclQualifiers;

    public IReadOnlyList<ParmVarDecl> Parameters => _parameters;

    public Type ReturnType => _returnType.GetValue(this);

    public ImplicitParamDecl SelfDecl => _selfDecl.GetValue(this);

    public Type SendResultType => _sendResultType.GetValue(this);

    public bool IsPropertyAccessor => Handle.IsPropertyAccessor;

    public string Selector => Handle.Selector.ToString();

    private static unsafe Type SendResultTypeFactory(ObjCMethodDecl self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.TypeOperand);

    private static unsafe Type ReturnTypeFactory(ObjCMethodDecl self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ReturnType);

    private static unsafe ImplicitParamDecl SelfDeclFactory(ObjCMethodDecl self) => self.TranslationUnit.GetOrCreate<ImplicitParamDecl>(self.Handle.GetSubDecl(2));

    private static unsafe ImplicitParamDecl CmdDeclFactory(ObjCMethodDecl self) => self.TranslationUnit.GetOrCreate<ImplicitParamDecl>(self.Handle.GetSubDecl(1));

    private static unsafe ObjCInterfaceDecl ClassInterfaceFactory(ObjCMethodDecl self) => self.TranslationUnit.GetOrCreate<ObjCInterfaceDecl>(self.Handle.GetSubDecl(0));

    private static unsafe ParmVarDecl ParametersFactory(object self, int i)
    {
        var @this = (ObjCMethodDecl)self;
        return @this.TranslationUnit.GetOrCreate<ParmVarDecl>(@this.Handle.GetArgument(unchecked((uint)i)));
    }
}
