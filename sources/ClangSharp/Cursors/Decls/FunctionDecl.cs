// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class FunctionDecl : DeclaratorDecl, IDeclContext, IRedeclarable<FunctionDecl>
    {
        private readonly Lazy<Type> _callResultType;
        private readonly Lazy<Type> _declaredReturnType;
        private readonly Lazy<IReadOnlyList<Decl>> _decls;
        private readonly Lazy<FunctionDecl> _definition;
        private readonly Lazy<FunctionDecl> _instantiatedFromMemberFunction;
        private readonly Lazy<IReadOnlyList<ParmVarDecl>> _parameters;
        private readonly Lazy<FunctionTemplateDecl> _primaryTemplate;
        private readonly Lazy<Type> _returnType;
        private readonly Lazy<FunctionDecl> _templateInstantiationPattern;

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
            _decls = new Lazy<IReadOnlyList<Decl>>(() => CursorChildren.OfType<Decl>().ToList());
            _definition = new Lazy<FunctionDecl>(() => TranslationUnit.GetOrCreate<FunctionDecl>(Handle.Definition));
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
        }

        public Type CallResultType => _callResultType.Value;

        public new FunctionDecl CanonicalDecl => (FunctionDecl)base.CanonicalDecl;

        public Type DeclaredReturnType => _declaredReturnType.Value;

        public IReadOnlyList<Decl> Decls => _decls.Value;

        public FunctionDecl Definition => _definition.Value;

        public CXCursor_ExceptionSpecificationKind ExceptionSpecType => (CXCursor_ExceptionSpecificationKind)Handle.ExceptionSpecificationType;

        public bool HasBody => Handle.HasBody;

        public bool HasImplicitReturnZero => Handle.HasImplicitReturnZero;

        public FunctionDecl InstantiatedFromMemberEnum => _instantiatedFromMemberFunction.Value;

        public bool IsDefaulted => Handle.CXXMethod_IsDefaulted;

        public bool IsDefined => Handle.IsDefined;

        public bool IsExternC => Handle.IsExternC;

        public bool IsGlobal => Handle.IsGlobal;

        public bool IsInlined => Handle.IsFunctionInlined;

        public bool IsNoReturn => Handle.IsNoReturn;

        public bool IsOverloadedOperator => Handle.IsOverloadedOperator;

        public bool IsPure => Handle.IsPure;

        public bool IsStatic => Handle.IsStatic;

        public bool IsThisDeclarationADefinition => Handle.IsThisDeclarationADefinition;

        public bool IsVariadic => Handle.IsVariadic;

        public IDeclContext LexicalParent => LexicalDeclContext;

        public IReadOnlyList<ParmVarDecl> Parameters => _parameters.Value;

        public IDeclContext Parent => DeclContext;

        public FunctionTemplateDecl PrimaryTemplate => _primaryTemplate.Value;

        public Type ReturnType => _returnType.Value;

        public CX_StorageClass StorageClass => Handle.StorageClass;

        public FunctionDecl TemplateInstantiationPattern => _templateInstantiationPattern.Value;

        public CX_TemplateSpecializationKind TemplateSpecializationKind => Handle.TemplateSpecializationKind;
    }
}
