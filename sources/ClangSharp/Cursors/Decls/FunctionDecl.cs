// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class FunctionDecl : DeclaratorDecl, IDeclContext, IRedeclarable<FunctionDecl>
    {
        private readonly Lazy<Type> _callResultType;
        private readonly Lazy<Type> _declaredReturnType;
        private readonly Lazy<FunctionDecl> _definition;
        private readonly Lazy<FunctionTemplateDecl> _describedFunctionDecl;
        private readonly Lazy<FunctionDecl> _instantiatedFromMemberFunction;
        private readonly Lazy<IReadOnlyList<ParmVarDecl>> _parameters;
        private readonly Lazy<FunctionTemplateDecl> _primaryTemplate;
        private readonly Lazy<Type> _returnType;
        private readonly Lazy<FunctionDecl> _templateInstantiationPattern;
        private readonly Lazy<IReadOnlyList<TemplateArgument>> _templateSpecializationArgs;

        internal FunctionDecl(CXCursor handle) : this(handle, CXCursorKind.CXCursor_FunctionDecl, CX_DeclKind.CX_DeclKind_Function)
        {
        }

        private protected FunctionDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if ((CX_DeclKind.CX_DeclKind_LastFunction < handle.DeclKind) || (handle.DeclKind < CX_DeclKind.CX_DeclKind_FirstFunction))
            {
                throw new ArgumentException(nameof(handle));
            }

            _callResultType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.CallResultType));
            _declaredReturnType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.DeclaredReturnType));
            _definition = new Lazy<FunctionDecl>(() => TranslationUnit.GetOrCreate<FunctionDecl>(Handle.Definition));
            _describedFunctionDecl = new Lazy<FunctionTemplateDecl>(() => TranslationUnit.GetOrCreate<FunctionTemplateDecl>(Handle.DescribedCursorTemplate));
            _instantiatedFromMemberFunction = new Lazy<FunctionDecl>(() => TranslationUnit.GetOrCreate<FunctionDecl>(Handle.InstantiatedFromMember));

            _parameters = new Lazy<IReadOnlyList<ParmVarDecl>>(() => {
                var parameterCount = Handle.NumArguments;
                var parameters = new List<ParmVarDecl>(parameterCount);

                for (int i = 0; i < parameterCount; i++)
                {
                    var parameter = TranslationUnit.GetOrCreate<ParmVarDecl>(Handle.GetArgument(unchecked((uint)i)));
                    parameters.Add(parameter);
                }

                return parameters;
            });

            _primaryTemplate = new Lazy<FunctionTemplateDecl>(() => TranslationUnit.GetOrCreate<FunctionTemplateDecl>(Handle.PrimaryTemplate));
            _returnType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ReturnType));
            _templateInstantiationPattern = new Lazy<FunctionDecl>(() => TranslationUnit.GetOrCreate<FunctionDecl>(Handle.TemplateInstantiationPattern));

            _templateSpecializationArgs = new Lazy<IReadOnlyList<TemplateArgument>>(() => {
                var templateArgCount = Handle.NumTemplateArguments;
                var templateArgs = new List<TemplateArgument>(templateArgCount);

                for (int i = 0; i < templateArgCount; i++)
                {
                    var templateArg = TranslationUnit.GetOrCreate(Handle.GetTemplateArgument(unchecked((uint)i)));
                    templateArgs.Add(templateArg);
                }

                return templateArgs;
            });
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

        public bool IsPure => Handle.IsPure;

        public bool IsStatic => Handle.IsStatic;

        public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

        public bool IsUserProvided => Handle.IsUserProvided;

        public bool IsVariadic => Handle.IsVariadic;

        public string NameInfoName => Handle.Name.CString;

        public uint NumParams => unchecked((uint)Handle.NumArguments);

        public CX_OverloadedOperatorKind OverloadedOperator => Handle.OverloadedOperatorKind;

        public IReadOnlyList<ParmVarDecl> Parameters => _parameters.Value;

        public FunctionTemplateDecl PrimaryTemplate => _primaryTemplate.Value;

        public Type ReturnType => _returnType.Value;

        public CX_StorageClass StorageClass => Handle.StorageClass;

        public FunctionDecl TemplateInstantiationPattern => _templateInstantiationPattern.Value;

        public IReadOnlyList<TemplateArgument> TemplateSpecializationArgs => _templateSpecializationArgs.Value;

        public CX_TemplateSpecializationKind TemplateSpecializationKind => Handle.TemplateSpecializationKind;
    }
}
