// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TemplateArgument
    {
        private readonly Lazy<ValueDecl> _asDecl;
        private readonly Lazy<Expr> _asExpr;
        private readonly Lazy<TemplateName> _asTemplate;
        private readonly Lazy<TemplateName> _asTemplateOrTemplatePattern;
        private readonly Lazy<Type> _asType;
        private readonly Lazy<Type> _integralType;
        private readonly Lazy<Type> _nonTypeTemplateArgumentType;
        private readonly Lazy<Type> _nullPtrType;
        private readonly Lazy<Type> _paramTypeForDecl;
        private readonly Lazy<TranslationUnit> _translationUnit;

        internal TemplateArgument(CX_TemplateArgument handle)
        {
            Handle = handle;

            _asDecl = new Lazy<ValueDecl>(() =>  _translationUnit.Value.GetOrCreate<ValueDecl>(Handle.AsDecl));
            _asExpr = new Lazy<Expr>(() => _translationUnit.Value.GetOrCreate<Expr>(Handle.AsExpr));
            _asTemplate = new Lazy<TemplateName>(() => _translationUnit.Value.GetOrCreate(Handle.AsTemplate));
            _asTemplateOrTemplatePattern = new Lazy<TemplateName>(() => _translationUnit.Value.GetOrCreate(Handle.AsTemplateOrTemplatePattern));
            _asType = new Lazy<Type>(() => _translationUnit.Value.GetOrCreate<Type>(Handle.AsType));
            _integralType = new Lazy<Type>(() => _translationUnit.Value.GetOrCreate<Type>(Handle.IntegralType));
            _nonTypeTemplateArgumentType = new Lazy<Type>(() => _translationUnit.Value.GetOrCreate<Type>(Handle.NonTypeTemplateArgumentType));
            _nullPtrType = new Lazy<Type>(() => _translationUnit.Value.GetOrCreate<Type>(Handle.NullPtrType));
            _paramTypeForDecl = new Lazy<Type>(() => _translationUnit.Value.GetOrCreate<Type>(Handle.ParamTypeForDecl));
            _translationUnit = new Lazy<TranslationUnit>(() => TranslationUnit.GetOrCreate(Handle.tu));
        }

        public ValueDecl AsDecl => _asDecl.Value;

        public Expr AsExpr => _asExpr.Value;

        public long AsIntegral => Handle.AsIntegral;

        public TemplateName AsTemplate => _asTemplate.Value;

        public TemplateName AsTemplateOrTemplatePattern => _asTemplateOrTemplatePattern.Value;

        public Type AsType => _asType.Value;

        public bool ContainsUnexpandedParameterPack => (Dependence & CX_TemplateArgumentDependence.CX_TAD_UnexpandedPack) != 0;

        public CX_TemplateArgumentDependence Dependence => Handle.Dependence;

        public Type IntegralType => _integralType.Value;

        public bool IsDependent => (Dependence & CX_TemplateArgumentDependence.CX_TAD_Dependent) != 0;

        public bool IsInstantiationDependent => (Dependence & CX_TemplateArgumentDependence.CX_TAD_Instantiation) != 0;

        public bool IsNull => Kind == CXTemplateArgumentKind.CXTemplateArgumentKind_Null;

        public bool IsPackExpansion
        {
            get
            {
                switch (Kind)
{
                    case CXTemplateArgumentKind.CXTemplateArgumentKind_Null:
                    case CXTemplateArgumentKind.CXTemplateArgumentKind_Declaration:
                    case CXTemplateArgumentKind.CXTemplateArgumentKind_Integral:
                    case CXTemplateArgumentKind.CXTemplateArgumentKind_Pack:
                    case CXTemplateArgumentKind.CXTemplateArgumentKind_Template:
                    case CXTemplateArgumentKind.CXTemplateArgumentKind_NullPtr:
                    {
                        return false;
                    }

                    case CXTemplateArgumentKind.CXTemplateArgumentKind_TemplateExpansion:
                    {
                        return true;
                    }

                    case CXTemplateArgumentKind.CXTemplateArgumentKind_Type:
                    {
                        return AsType is PackExpansionType;
                    }

                    case CXTemplateArgumentKind.CXTemplateArgumentKind_Expression:
                    {
                        return AsExpr is PackExpansionExpr;
                    }
                }

                Debug.Fail("Invalid TemplateArgument Kind!");
                return false;
            }
        }

        public CX_TemplateArgument Handle { get; }

        public CXTemplateArgumentKind Kind => Handle.kind;

        public Type NonTypeTemplateArgumentType => _nonTypeTemplateArgumentType.Value;

        public Type NullPtrType => _nullPtrType.Value;

        public Type ParamTypeForDecl => _paramTypeForDecl.Value;
    }
}
