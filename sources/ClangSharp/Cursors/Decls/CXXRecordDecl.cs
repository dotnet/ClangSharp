// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
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
        private readonly Lazy<CXXRecordDecl> _templateInstantiationPattern;
        private readonly Lazy<IReadOnlyList<CXXBaseSpecifier>> _vbases;

        internal CXXRecordDecl(CXCursor handle) : this(handle, handle.Kind, CX_DeclKind.CX_DeclKind_CXXRecord)
        {
        }

        private protected CXXRecordDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if (handle.DeclKind is > CX_DeclKind.CX_DeclKind_LastCXXRecord or < CX_DeclKind.CX_DeclKind_FirstCXXRecord)
            {
                throw new ArgumentOutOfRangeException(nameof(handle));
            }

            _bases = new Lazy<IReadOnlyList<CXXBaseSpecifier>>(() => {
                var numBases = Handle.NumBases;
                var bases = new List<CXXBaseSpecifier>(numBases);

                for (var i = 0; i < numBases; i++)
                {
                    var @base = TranslationUnit.GetOrCreate<CXXBaseSpecifier>(Handle.GetBase(unchecked((uint)i)));
                    bases.Add(@base);
                }

                return bases;
            });

            _ctors = new Lazy<IReadOnlyList<CXXConstructorDecl>>(() => {
                var numCtors = Handle.NumCtors;
                var ctors = new List<CXXConstructorDecl>(numCtors);

                for (var i = 0; i < numCtors; i++)
                {
                    var ctor = TranslationUnit.GetOrCreate<CXXConstructorDecl>(Handle.GetCtor(unchecked((uint)i)));
                    ctors.Add(ctor);
                }

                return ctors;
            });

            _dependentLambdaCallOperator = new Lazy<FunctionTemplateDecl>(() => TranslationUnit.GetOrCreate<FunctionTemplateDecl>(Handle.DependentLambdaCallOperator));
            _describedClassTemplate = new Lazy<ClassTemplateDecl>(() => TranslationUnit.GetOrCreate<ClassTemplateDecl>(Handle.DescribedCursorTemplate));
            _destructor = new Lazy<CXXDestructorDecl>(() => TranslationUnit.GetOrCreate<CXXDestructorDecl>(Handle.Destructor));

            _friends = new Lazy<IReadOnlyList<FriendDecl>>(() => {
                var numFriends = Handle.NumFriends;
                var friends = new List<FriendDecl>(numFriends);

                for (var i = 0; i < numFriends; i++)
                {
                    var friend = TranslationUnit.GetOrCreate<FriendDecl>(Handle.GetFriend(unchecked((uint)i)));
                    friends.Add(friend);
                }

                return friends;
            });

            _instantiatedFromMemberClass = new Lazy<CXXRecordDecl>(() => TranslationUnit.GetOrCreate<CXXRecordDecl>(Handle.InstantiatedFromMember));
            _lambdaCallOperator = new Lazy<CXXMethodDecl>(() => TranslationUnit.GetOrCreate<CXXMethodDecl>(Handle.LambdaCallOperator));
            _lambdaContextDecl = new Lazy<Decl>(() => TranslationUnit.GetOrCreate<Decl>(Handle.LambdaContextDecl));
            _lambdaStaticInvoker = new Lazy<CXXMethodDecl>(() => TranslationUnit.GetOrCreate<CXXMethodDecl>(Handle.LambdaStaticInvoker));

            _methods = new Lazy<IReadOnlyList<CXXMethodDecl>>(() => {
                var numMethods = Handle.NumMethods;
                var methods = new List<CXXMethodDecl>(numMethods);

                for (var i = 0; i < numMethods; i++)
                {
                    var method = TranslationUnit.GetOrCreate<CXXMethodDecl>(Handle.GetMethod(unchecked((uint)i)));
                    methods.Add(method);
                }

                return methods;
            });

            _templateInstantiationPattern = new Lazy<CXXRecordDecl>(() => TranslationUnit.GetOrCreate<CXXRecordDecl>(Handle.TemplateInstantiationPattern));

            _vbases = new Lazy<IReadOnlyList<CXXBaseSpecifier>>(() => {
                var numVBases = Handle.NumVBases;
                var vbases = new List<CXXBaseSpecifier>(numVBases);

                for (var i = 0; i < numVBases; i++)
                {
                    var vbase = TranslationUnit.GetOrCreate<CXXBaseSpecifier>(Handle.GetVBase(unchecked((uint)i)));
                    vbases.Add(vbase);
                }

                return vbases;
            });
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

        public bool HasDefinition => Definition is not null;

        public bool HasFriends => Handle.NumFriends != 0;

        public bool HasUserDeclaredConstructor => Handle.HasUserDeclaredConstructor;

        public bool HasUserDeclaredCopyAssignment => Handle.HasUserDeclaredCopyAssignment;

        public bool HasUserDeclaredCopyConstructor => Handle.HasUserDeclaredCopyConstructor;

        public bool HasUserDeclaredDestructor => Handle.HasUserDeclaredDestructor;

        public bool HasUserDeclaredMoveAssignment => Handle.HasUserDeclaredMoveAssignment;

        public bool HasUserDeclaredMoveConstructor => Handle.HasUserDeclaredMoveConstructor;

        public bool HasUserDeclaredMoveOperation => Handle.HasUserDeclaredMoveOperation;

        public CXXRecordDecl InstantiatedFromMemberClass => _instantiatedFromMemberClass.Value;

        public CXXMethodDecl LambdaCallOperator => _lambdaCallOperator.Value;

        public Decl LambdaContextDecl => _lambdaContextDecl.Value;

        public CXXMethodDecl LambdaStaticInvoker => _lambdaStaticInvoker.Value;

        public IReadOnlyList<CXXMethodDecl> Methods => _methods.Value;

        public new CXXRecordDecl MostRecentDecl => (CXXRecordDecl)base.MostRecentDecl;

        public CXXRecordDecl MostRecentNonInjectedDecl
        {
            get
            {
                var recent = MostRecentDecl;

                while ((recent != null) && recent.IsInjectedClassName)
                {
                    recent = recent.PreviousDecl;
                }
                return recent;
            }
        }

        public uint NumBases => unchecked((uint)Handle.NumBases);

        public uint NumVBases => unchecked((uint)Handle.NumVBases);

        public new CXXRecordDecl PreviousDecl => (CXXRecordDecl)base.PreviousDecl;

        public CX_TemplateSpecializationKind TemplateSpecializationKind => Handle.TemplateSpecializationKind;

        public CXXRecordDecl TemplateInstantiationPattern => _templateInstantiationPattern.Value;

        public IReadOnlyList<CXXBaseSpecifier> VBases => _vbases.Value;
    }
}
