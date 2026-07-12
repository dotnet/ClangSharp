// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public class FunctionDecl : DeclaratorDecl, IDeclContext, IRedeclarable<FunctionDecl>
{
    private ValueLazy<FunctionDecl, Type> _callResultType;
    private ValueLazy<FunctionDecl, Type> _declaredReturnType;
    private ValueLazy<FunctionDecl, FunctionDecl> _definition;
    private ValueLazy<FunctionDecl, FunctionTemplateDecl> _describedFunctionDecl;
    private ValueLazy<FunctionDecl, FunctionDecl> _instantiatedFromMemberFunction;
    private readonly LazyList<ParmVarDecl> _parameters;
    private ValueLazy<FunctionDecl, FunctionTemplateDecl> _primaryTemplate;
    private ValueLazy<FunctionDecl, Type> _returnType;
    private ValueLazy<FunctionDecl, FunctionDecl> _templateInstantiationPattern;
    private readonly LazyList<TemplateArgument> _templateSpecializationArgs;

    internal FunctionDecl(CXCursor handle) : this(handle, CXCursor_FunctionDecl, CX_DeclKind_Function)
    {
    }

    private protected unsafe FunctionDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastFunction or < CX_DeclKind_FirstFunction)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _callResultType = new ValueLazy<FunctionDecl, Type>(&CallResultTypeFactory);
        _declaredReturnType = new ValueLazy<FunctionDecl, Type>(&DeclaredReturnTypeFactory);
        _definition = new ValueLazy<FunctionDecl, FunctionDecl>(&DefinitionFactory);
        _describedFunctionDecl = new ValueLazy<FunctionDecl, FunctionTemplateDecl>(&DescribedFunctionDeclFactory);
        _instantiatedFromMemberFunction = new ValueLazy<FunctionDecl, FunctionDecl>(&InstantiatedFromMemberFunctionFactory);
        _parameters = LazyList.Create<ParmVarDecl>(this, Handle.NumArguments, &ParametersFactory);
        _primaryTemplate = new ValueLazy<FunctionDecl, FunctionTemplateDecl>(&PrimaryTemplateFactory);
        _returnType = new ValueLazy<FunctionDecl, Type>(&ReturnTypeFactory);
        _templateInstantiationPattern = new ValueLazy<FunctionDecl, FunctionDecl>(&TemplateInstantiationPatternFactory);
        _templateSpecializationArgs = LazyList.Create<TemplateArgument>(this, Handle.NumTemplateArguments, &TemplateSpecializationArgsFactory);
    }

    public Type CallResultType => _callResultType.GetValue(this);

    public new FunctionDecl CanonicalDecl => (FunctionDecl)base.CanonicalDecl;

    public Type DeclaredReturnType => _declaredReturnType.GetValue(this);

    public FunctionDecl Definition => _definition.GetValue(this);

    public FunctionTemplateDecl DescribedFunctionDecl => _describedFunctionDecl.GetValue(this);

    public CXCursor_ExceptionSpecificationKind ExceptionSpecType => (CXCursor_ExceptionSpecificationKind)Handle.ExceptionSpecificationType;

    public bool HasBody => Handle.HasBody;

    public bool HasImplicitReturnZero => Handle.HasImplicitReturnZero;

    public FunctionDecl InstantiatedFromMemberFunction => _instantiatedFromMemberFunction.GetValue(this);

    public bool IsDefaulted => Handle.CXXMethod_IsDefaulted;

    public bool IsDefined => Handle.IsDefined;

    public bool IsDeleted => Handle.IsDeleted;

    public bool IsExplicitlyDefaulted => Handle.IsExplicitlyDefaulted;

    public bool IsExternC => Handle.IsExternC;

    public bool IsGlobal => Handle.IsGlobal;

    public bool IsInlined => Handle.IsFunctionInlined;

    public bool IsInstance => !IsStatic;

    public bool IsNoReturn => Handle.IsNoReturn;

    public bool IsOverloadedOperator => Handle.IsOverloadedOperator;

    public bool IsPure => Handle.IsPureVirtual;

    public bool IsStatic => Handle.IsStatic;

    public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

    public bool IsUserProvided => Handle.IsUserProvided;

    public bool IsVariadic => Handle.IsVariadic;

    public string NameInfoName => Handle.Name.CString;

    public uint NumParams => unchecked((uint)Handle.NumArguments);

    public CX_OverloadedOperatorKind OverloadedOperator => Handle.OverloadedOperatorKind;

    public IReadOnlyList<ParmVarDecl> Parameters => _parameters;

    public FunctionTemplateDecl PrimaryTemplate => _primaryTemplate.GetValue(this);

    public Type ReturnType => _returnType.GetValue(this);

    public CX_StorageClass StorageClass => Handle.StorageClass;

    public FunctionDecl TemplateInstantiationPattern => _templateInstantiationPattern.GetValue(this);

    public IReadOnlyList<TemplateArgument> TemplateSpecializationArgs => _templateSpecializationArgs;

    public CX_TemplateSpecializationKind TemplateSpecializationKind => Handle.TemplateSpecializationKind;

    private static unsafe FunctionDecl TemplateInstantiationPatternFactory(FunctionDecl self) => self.TranslationUnit.GetOrCreate<FunctionDecl>(self.Handle.TemplateInstantiationPattern);

    private static unsafe Type ReturnTypeFactory(FunctionDecl self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ReturnType);

    private static unsafe FunctionTemplateDecl PrimaryTemplateFactory(FunctionDecl self) => self.TranslationUnit.GetOrCreate<FunctionTemplateDecl>(self.Handle.PrimaryTemplate);

    private static unsafe FunctionDecl InstantiatedFromMemberFunctionFactory(FunctionDecl self) => self.TranslationUnit.GetOrCreate<FunctionDecl>(self.Handle.InstantiatedFromMember);

    private static unsafe FunctionTemplateDecl DescribedFunctionDeclFactory(FunctionDecl self) => self.TranslationUnit.GetOrCreate<FunctionTemplateDecl>(self.Handle.DescribedCursorTemplate);

    private static unsafe FunctionDecl DefinitionFactory(FunctionDecl self) => self.TranslationUnit.GetOrCreate<FunctionDecl>(self.Handle.Definition);

    private static unsafe Type DeclaredReturnTypeFactory(FunctionDecl self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.DeclaredReturnType);

    private static unsafe Type CallResultTypeFactory(FunctionDecl self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.CallResultType);

    private static unsafe ParmVarDecl ParametersFactory(object self, int i)
    {
        var @this = (FunctionDecl)self;
        return @this.TranslationUnit.GetOrCreate<ParmVarDecl>(@this.Handle.GetArgument(unchecked((uint)i)));
    }

    private static unsafe TemplateArgument TemplateSpecializationArgsFactory(object self, int i)
    {
        var @this = (FunctionDecl)self;
        return @this.TranslationUnit.GetOrCreate(@this.Handle.GetTemplateArgument(unchecked((uint)i)));
    }
}
