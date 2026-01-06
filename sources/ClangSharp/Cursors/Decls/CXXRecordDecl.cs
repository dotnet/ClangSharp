// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class CXXRecordDecl : RecordDecl
{
    private readonly LazyList<CXXBaseSpecifier> _bases;
    private readonly LazyList<CXXConstructorDecl> _ctors;
    private readonly ValueLazy<FunctionTemplateDecl> _dependentLambdaCallOperator;
    private readonly ValueLazy<ClassTemplateDecl> _describedClassTemplate;
    private readonly ValueLazy<CXXDestructorDecl?> _destructor;
    private readonly LazyList<FriendDecl> _friends;
    private readonly ValueLazy<CXXRecordDecl> _instantiatedFromMemberClass;
    private readonly ValueLazy<CXXMethodDecl> _lambdaCallOperator;
    private readonly ValueLazy<Decl> _lambdaContextDecl;
    private readonly ValueLazy<CXXMethodDecl> _lambdaStaticInvoker;
    private readonly LazyList<CXXMethodDecl> _methods;
    private readonly ValueLazy<CXXRecordDecl> _templateInstantiationPattern;
    private readonly LazyList<CXXBaseSpecifier> _vbases;

    internal CXXRecordDecl(CXCursor handle) : this(handle, handle.Kind, CX_DeclKind_CXXRecord)
    {
    }

    private protected CXXRecordDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastCXXRecord or < CX_DeclKind_FirstCXXRecord)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _bases = LazyList.Create<CXXBaseSpecifier>(Handle.NumBases, (i) => TranslationUnit.GetOrCreate<CXXBaseSpecifier>(Handle.GetBase(unchecked((uint)i))));
        _ctors = LazyList.Create<CXXConstructorDecl>(Handle.NumCtors, (i) => TranslationUnit.GetOrCreate<CXXConstructorDecl>(Handle.GetCtor(unchecked((uint)i))));
        _dependentLambdaCallOperator = new ValueLazy<FunctionTemplateDecl>(() => TranslationUnit.GetOrCreate<FunctionTemplateDecl>(Handle.DependentLambdaCallOperator));
        _describedClassTemplate = new ValueLazy<ClassTemplateDecl>(() => TranslationUnit.GetOrCreate<ClassTemplateDecl>(Handle.DescribedCursorTemplate));
        _destructor = new ValueLazy<CXXDestructorDecl?>(() => {
            var destructor = Handle.Destructor;
            return destructor.IsNull ? null : TranslationUnit.GetOrCreate<CXXDestructorDecl>(Handle.Destructor);
        });
        _friends = LazyList.Create<FriendDecl>(Handle.NumFriends, (i) => TranslationUnit.GetOrCreate<FriendDecl>(Handle.GetFriend(unchecked((uint)i))));
        _instantiatedFromMemberClass = new ValueLazy<CXXRecordDecl>(() => TranslationUnit.GetOrCreate<CXXRecordDecl>(Handle.InstantiatedFromMember));
        _lambdaCallOperator = new ValueLazy<CXXMethodDecl>(() => TranslationUnit.GetOrCreate<CXXMethodDecl>(Handle.LambdaCallOperator));
        _lambdaContextDecl = new ValueLazy<Decl>(() => TranslationUnit.GetOrCreate<Decl>(Handle.LambdaContextDecl));
        _lambdaStaticInvoker = new ValueLazy<CXXMethodDecl>(() => TranslationUnit.GetOrCreate<CXXMethodDecl>(Handle.LambdaStaticInvoker));
        _methods = LazyList.Create<CXXMethodDecl>(Handle.NumMethods, (i) => TranslationUnit.GetOrCreate<CXXMethodDecl>(Handle.GetMethod(unchecked((uint)i))));
        _templateInstantiationPattern = new ValueLazy<CXXRecordDecl>(() => TranslationUnit.GetOrCreate<CXXRecordDecl>(Handle.TemplateInstantiationPattern));
        _vbases = LazyList.Create<CXXBaseSpecifier>(Handle.NumVBases, (i) => TranslationUnit.GetOrCreate<CXXBaseSpecifier>(Handle.GetVBase(unchecked((uint)i))));
    }

    public bool IsAbstract => Handle.CXXRecord_IsAbstract;

    public bool IsPOD => Handle.CXXRecord_IsPOD;

    public IReadOnlyList<CXXBaseSpecifier> Bases => _bases;

    public new CXXRecordDecl CanonicalDecl => (CXXRecordDecl)base.CanonicalDecl;

    public IReadOnlyList<CXXConstructorDecl> Ctors => _ctors;

    public new CXXRecordDecl? Definition => (CXXRecordDecl?)base.Definition;

    public FunctionTemplateDecl DependentLambdaCallOperator => _dependentLambdaCallOperator.Value;

    public ClassTemplateDecl DescribedClassTemplate => _describedClassTemplate.Value;

    public CXXDestructorDecl? Destructor => _destructor.Value;

    public IReadOnlyList<FriendDecl> Friends => _friends;

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

    public IReadOnlyList<CXXMethodDecl> Methods => _methods;

    public new CXXRecordDecl MostRecentDecl => (CXXRecordDecl)base.MostRecentDecl;

    public CXXRecordDecl MostRecentNonInjectedDecl
    {
        get
        {
            var recent = MostRecentDecl;

            while ((recent is not null) && recent.IsInjectedClassName)
            {
                recent = recent.PreviousDecl;
            }

            Debug.Assert(recent is not null);
            return recent!;
        }
    }

    public uint NumBases => unchecked((uint)Handle.NumBases);

    public uint NumVBases => unchecked((uint)Handle.NumVBases);

    public new CXXRecordDecl PreviousDecl => (CXXRecordDecl)base.PreviousDecl;

    public CX_TemplateSpecializationKind TemplateSpecializationKind => Handle.TemplateSpecializationKind;

    public CXXRecordDecl TemplateInstantiationPattern => _templateInstantiationPattern.Value;

    public IReadOnlyList<CXXBaseSpecifier> VBases => _vbases;
}
