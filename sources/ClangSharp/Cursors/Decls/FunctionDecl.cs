// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public class FunctionDecl : DeclaratorDecl, IDeclContext, IRedeclarable<FunctionDecl>
{
    private readonly Lazy<Type> _callResultType;
    private readonly Lazy<Type> _declaredReturnType;
    private readonly Lazy<FunctionDecl> _definition;
    private readonly Lazy<FunctionTemplateDecl> _describedFunctionDecl;
    private readonly Lazy<FunctionDecl> _instantiatedFromMemberFunction;
    private readonly LazyList<ParmVarDecl> _parameters;
    private readonly Lazy<FunctionTemplateDecl> _primaryTemplate;
    private readonly Lazy<Type> _returnType;
    private readonly Lazy<FunctionDecl> _templateInstantiationPattern;
    private readonly LazyList<TemplateArgument> _templateSpecializationArgs;

    internal FunctionDecl(CXCursor handle) : this(handle, CXCursor_FunctionDecl, CX_DeclKind_Function)
    {
    }

    private protected FunctionDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastFunction or < CX_DeclKind_FirstFunction)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _callResultType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.CallResultType));
        _declaredReturnType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.DeclaredReturnType));
        _definition = new Lazy<FunctionDecl>(() => TranslationUnit.GetOrCreate<FunctionDecl>(Handle.Definition));
        _describedFunctionDecl = new Lazy<FunctionTemplateDecl>(() => TranslationUnit.GetOrCreate<FunctionTemplateDecl>(Handle.DescribedCursorTemplate));
        _instantiatedFromMemberFunction = new Lazy<FunctionDecl>(() => TranslationUnit.GetOrCreate<FunctionDecl>(Handle.InstantiatedFromMember));
        _parameters = LazyList.Create<ParmVarDecl>(Handle.NumArguments, (i) => TranslationUnit.GetOrCreate<ParmVarDecl>(Handle.GetArgument(unchecked((uint)i))));
        _primaryTemplate = new Lazy<FunctionTemplateDecl>(() => TranslationUnit.GetOrCreate<FunctionTemplateDecl>(Handle.PrimaryTemplate));
        _returnType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ReturnType));
        _templateInstantiationPattern = new Lazy<FunctionDecl>(() => TranslationUnit.GetOrCreate<FunctionDecl>(Handle.TemplateInstantiationPattern));
        _templateSpecializationArgs = LazyList.Create<TemplateArgument>(Handle.NumTemplateArguments, (i) => TranslationUnit.GetOrCreate(Handle.GetTemplateArgument(unchecked((uint)i))));
    }

    public Type CallResultType => _callResultType.Value;

    public new FunctionDecl CanonicalDecl => (FunctionDecl)base.CanonicalDecl;

    public Type DeclaredReturnType => _declaredReturnType.Value;

    public FunctionDecl Definition => _definition.Value;

    public FunctionTemplateDecl DescribedFunctionDecl => _describedFunctionDecl.Value;

    public CXCursor_ExceptionSpecificationKind ExceptionSpecType => (CXCursor_ExceptionSpecificationKind)Handle.ExceptionSpecificationType;

    public bool HasBody => Handle.HasBody;

    public bool HasImplicitReturnZero => Handle.HasImplicitReturnZero;

    public FunctionDecl InstantiatedFromMemberFunction => _instantiatedFromMemberFunction.Value;

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

    public FunctionTemplateDecl PrimaryTemplate => _primaryTemplate.Value;

    public Type ReturnType => _returnType.Value;

    public CX_StorageClass StorageClass => Handle.StorageClass;

    public FunctionDecl TemplateInstantiationPattern => _templateInstantiationPattern.Value;

    public IReadOnlyList<TemplateArgument> TemplateSpecializationArgs => _templateSpecializationArgs;

    public CX_TemplateSpecializationKind TemplateSpecializationKind => Handle.TemplateSpecializationKind;
}
