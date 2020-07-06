// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class CXXRecordDecl : RecordDecl
    {
        private readonly Lazy<IReadOnlyList<CXXBaseSpecifier>> _bases;
        private readonly Lazy<IReadOnlyList<CXXConstructorDecl>> _ctors;
        private readonly Lazy<FunctionTemplateDecl> _dependentLambdaCallOperator;
        private readonly Lazy<ClassTemplateDecl> _describedClassTemplate;
        private readonly Lazy<CXXDestructorDecl> _destructor;
        private readonly Lazy<IReadOnlyList<FriendDecl>> _friends;
        private readonly Lazy<CXXRecordDecl> _instantiatedFromMemberClass;
        private readonly Lazy<CXXMethodDecl> _lambdaCallOperator;
        private readonly Lazy<Decl> _lambdaContextDecl;
        private readonly Lazy<CXXMethodDecl> _lambdaStaticInvoker;
        private readonly Lazy<IReadOnlyList<CXXMethodDecl>> _methods;
        private readonly Lazy<CXXRecordDecl> _mostRecentNonInjectedDecl;
        private readonly Lazy<CXXRecordDecl> _templateInstantiationPattern;
        private readonly Lazy<IReadOnlyList<CXXBaseSpecifier>> _vbases;

        internal CXXRecordDecl(CXCursor handle) : this(handle, handle.Kind, CX_DeclKind.CX_DeclKind_CXXRecord)
        {
        }

        private protected CXXRecordDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if ((CX_DeclKind.CX_DeclKind_LastCXXRecord < handle.DeclKind) || (handle.DeclKind < CX_DeclKind.CX_DeclKind_FirstCXXRecord))
            {
                throw new ArgumentException(nameof(handle));
            }

            _bases = new Lazy<IReadOnlyList<CXXBaseSpecifier>>(() => CursorChildren.OfType<CXXBaseSpecifier>().ToList());
            _ctors = new Lazy<IReadOnlyList<CXXConstructorDecl>>(() => Methods.OfType<CXXConstructorDecl>().ToList());
            _dependentLambdaCallOperator = new Lazy<FunctionTemplateDecl>(() => TranslationUnit.GetOrCreate<FunctionTemplateDecl>(Handle.DependentLambdaCallOperator));
            _describedClassTemplate = new Lazy<ClassTemplateDecl>(() => TranslationUnit.GetOrCreate<ClassTemplateDecl>(Handle.DescribedClassTemplate));
            _destructor = new Lazy<CXXDestructorDecl>(() => TranslationUnit.GetOrCreate<CXXDestructorDecl>(Handle.Destructor));
            _friends = new Lazy<IReadOnlyList<FriendDecl>>(() => Decls.OfType<FriendDecl>().ToList());
            _instantiatedFromMemberClass = new Lazy<CXXRecordDecl>(() => TranslationUnit.GetOrCreate<CXXRecordDecl>(Handle.InstantiatedFromMember));
            _lambdaCallOperator = new Lazy<CXXMethodDecl>(() => TranslationUnit.GetOrCreate<CXXMethodDecl>(Handle.LambdaCallOperator));
            _lambdaContextDecl = new Lazy<Decl>(() => TranslationUnit.GetOrCreate<Decl>(Handle.LambdaContextDecl));
            _lambdaStaticInvoker = new Lazy<CXXMethodDecl>(() => TranslationUnit.GetOrCreate<CXXMethodDecl>(Handle.LambdaStaticInvoker));
            _methods = new Lazy<IReadOnlyList<CXXMethodDecl>>(() => Decls.OfType<CXXMethodDecl>().ToList());
            _mostRecentNonInjectedDecl = new Lazy<CXXRecordDecl>(() => TranslationUnit.GetOrCreate<CXXRecordDecl>(Handle.MostRecentNonInjectedDecl));
            _templateInstantiationPattern = new Lazy<CXXRecordDecl>(() => TranslationUnit.GetOrCreate<CXXRecordDecl>(Handle.TemplateInstantiationPattern));
            _vbases = new Lazy<IReadOnlyList<CXXBaseSpecifier>>(() => Bases.Where((@base) => @base.IsVirtual).ToList());
        }

        public bool IsAbstract => Handle.CXXRecord_IsAbstract;

        public IReadOnlyList<CXXBaseSpecifier> Bases => _bases.Value;

        public new CXXRecordDecl CanonicalDecl => (CXXRecordDecl)base.CanonicalDecl;

        public IReadOnlyList<CXXConstructorDecl> Ctors => _ctors.Value;

        public new CXXRecordDecl Definition => (CXXRecordDecl)base.Definition;

        public FunctionTemplateDecl DependentLambdaCallOperator => _dependentLambdaCallOperator.Value;

        public ClassTemplateDecl DescribedClassTemplate => _describedClassTemplate.Value;

        public CXXDestructorDecl Destructor => _destructor.Value;

        public IReadOnlyList<FriendDecl> Friends => _friends.Value;

        public bool HasUserDeclaredDestructor => Decls.OfType<CXXDestructorDecl>().Any();

        public CXXRecordDecl InstantiatedFromMemberClass => _instantiatedFromMemberClass.Value;

        public CXXMethodDecl LambdaCallOperator => _lambdaCallOperator.Value;

        public Decl LambdaContextDecl => _lambdaContextDecl.Value;

        public CXXMethodDecl LambdaStaticInvoker => _lambdaStaticInvoker.Value;

        public IReadOnlyList<CXXMethodDecl> Methods => _methods.Value;

        public new CXXRecordDecl MostRecentDecl => (CXXRecordDecl)base.MostRecentDecl;

        public CXXRecordDecl MostRecentNonInjectedDecl => _mostRecentNonInjectedDecl.Value;

        public new CXXRecordDecl PreviousDecl => (CXXRecordDecl)base.PreviousDecl;

        public CX_TemplateSpecializationKind TemplateSpecializationKind => Handle.TemplateSpecializationKind;

        public CXXRecordDecl TemplateInstantiationPattern => _templateInstantiationPattern.Value;

        public IReadOnlyList<CXXBaseSpecifier> VBases => _vbases.Value;
    }
}
