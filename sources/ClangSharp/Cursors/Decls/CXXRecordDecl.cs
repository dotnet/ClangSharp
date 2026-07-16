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
    private ValueLazy<CXXRecordDecl, FunctionTemplateDecl> _dependentLambdaCallOperator;
    private ValueLazy<CXXRecordDecl, ClassTemplateDecl> _describedClassTemplate;
    private ValueLazy<CXXRecordDecl, CXXDestructorDecl?> _destructor;
    private readonly LazyList<FriendDecl> _friends;
    private ValueLazy<CXXRecordDecl, CXXRecordDecl> _instantiatedFromMemberClass;
    private ValueLazy<CXXRecordDecl, CXXMethodDecl> _lambdaCallOperator;
    private ValueLazy<CXXRecordDecl, Decl> _lambdaContextDecl;
    private ValueLazy<CXXRecordDecl, CXXMethodDecl> _lambdaStaticInvoker;
    private readonly LazyList<CXXMethodDecl> _methods;
    private ValueLazy<CXXRecordDecl, CXXRecordDecl> _templateInstantiationPattern;
    private readonly LazyList<CXXBaseSpecifier> _vbases;

    internal CXXRecordDecl(CXCursor handle) : this(handle, handle.Kind, CX_DeclKind_CXXRecord)
    {
    }

    private protected unsafe CXXRecordDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
    {
        if (handle.DeclKind is > CX_DeclKind_LastCXXRecord or < CX_DeclKind_FirstCXXRecord)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _bases = LazyList.Create<CXXBaseSpecifier>(this, Handle.NumBases, &BasesFactory);
        _ctors = LazyList.Create<CXXConstructorDecl>(this, Handle.NumCtors, &CtorsFactory);
        _dependentLambdaCallOperator = new ValueLazy<CXXRecordDecl, FunctionTemplateDecl>(&DependentLambdaCallOperatorFactory);
        _describedClassTemplate = new ValueLazy<CXXRecordDecl, ClassTemplateDecl>(&DescribedClassTemplateFactory);
        _destructor = new ValueLazy<CXXRecordDecl, CXXDestructorDecl?>(&DestructorFactory);
        _friends = LazyList.Create<FriendDecl>(this, Handle.NumFriends, &FriendsFactory);
        _instantiatedFromMemberClass = new ValueLazy<CXXRecordDecl, CXXRecordDecl>(&InstantiatedFromMemberClassFactory);
        _lambdaCallOperator = new ValueLazy<CXXRecordDecl, CXXMethodDecl>(&LambdaCallOperatorFactory);
        _lambdaContextDecl = new ValueLazy<CXXRecordDecl, Decl>(&LambdaContextDeclFactory);
        _lambdaStaticInvoker = new ValueLazy<CXXRecordDecl, CXXMethodDecl>(&LambdaStaticInvokerFactory);
        _methods = LazyList.Create<CXXMethodDecl>(this, Handle.NumMethods, &MethodsFactory);
        _templateInstantiationPattern = new ValueLazy<CXXRecordDecl, CXXRecordDecl>(&TemplateInstantiationPatternFactory);
        _vbases = LazyList.Create<CXXBaseSpecifier>(this, Handle.NumVBases, &VbasesFactory);
    }

    public bool IsAbstract => Handle.CXXRecord_IsAbstract;

    public bool IsAggregate => Handle.IsAggregate;

    public bool IsCXX11StandardLayout => Handle.IsCXX11StandardLayout;

    public bool IsDynamicClass => Handle.IsDynamicClass;

    public bool IsEffectivelyFinal => Handle.IsEffectivelyFinal;

    public bool IsEmpty => Handle.IsEmpty;

    public bool IsLiteral => Handle.IsLiteral;

    public bool IsPOD => Handle.CXXRecord_IsPOD;

    public bool IsPolymorphic => Handle.IsPolymorphic;

    public bool IsStandardLayout => Handle.IsStandardLayout;

    public bool IsTrivial => Handle.IsTrivial;

    public bool IsTriviallyCopyable => Handle.IsTriviallyCopyable;

    public IReadOnlyList<CXXBaseSpecifier> Bases => _bases;

    public new CXXRecordDecl CanonicalDecl => (CXXRecordDecl)base.CanonicalDecl;

    public IReadOnlyList<CXXConstructorDecl> Ctors => _ctors;

    public new CXXRecordDecl? Definition => (CXXRecordDecl?)base.Definition;

    public FunctionTemplateDecl DependentLambdaCallOperator => _dependentLambdaCallOperator.GetValue(this);

    public ClassTemplateDecl DescribedClassTemplate => _describedClassTemplate.GetValue(this);

    public CXXDestructorDecl? Destructor => _destructor.GetValue(this);

    public IReadOnlyList<FriendDecl> Friends => _friends;

    public bool HasDefinition => Definition is not null;

    public bool HasDeletedDestructor => Handle.HasDeletedDestructor;

    public bool HasFriends => Handle.NumFriends != 0;

    public bool HasInClassInitializer => Handle.HasInClassInitializer;

    public bool HasMutableFields => Handle.HasMutableFields;

    public bool HasNonTrivialDefaultConstructor => Handle.HasNonTrivialDefaultConstructor;

    public bool HasNonTrivialDestructor => Handle.HasNonTrivialDestructor;

    public bool HasPrivateFields => Handle.HasPrivateFields;

    public bool HasProtectedFields => Handle.HasProtectedFields;

    public bool HasTrivialCopyConstructor => Handle.HasTrivialCopyConstructor;

    public bool HasTrivialDefaultConstructor => Handle.HasTrivialDefaultConstructor;

    public bool HasUserDeclaredConstructor => Handle.HasUserDeclaredConstructor;

    public bool HasUserDeclaredCopyAssignment => Handle.HasUserDeclaredCopyAssignment;

    public bool HasUserDeclaredCopyConstructor => Handle.HasUserDeclaredCopyConstructor;

    public bool HasUserDeclaredDestructor => Handle.HasUserDeclaredDestructor;

    public bool HasUserDeclaredMoveAssignment => Handle.HasUserDeclaredMoveAssignment;

    public bool HasUserDeclaredMoveConstructor => Handle.HasUserDeclaredMoveConstructor;

    public bool HasUserDeclaredMoveOperation => Handle.HasUserDeclaredMoveOperation;

    public CXXRecordDecl InstantiatedFromMemberClass => _instantiatedFromMemberClass.GetValue(this);

    public CXXMethodDecl LambdaCallOperator => _lambdaCallOperator.GetValue(this);

    public Decl LambdaContextDecl => _lambdaContextDecl.GetValue(this);

    public CXXMethodDecl LambdaStaticInvoker => _lambdaStaticInvoker.GetValue(this);

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

    public CXXRecordDecl TemplateInstantiationPattern => _templateInstantiationPattern.GetValue(this);

    public IReadOnlyList<CXXBaseSpecifier> VBases => _vbases;

    private static unsafe CXXRecordDecl TemplateInstantiationPatternFactory(CXXRecordDecl self) => self.TranslationUnit.GetOrCreate<CXXRecordDecl>(self.Handle.TemplateInstantiationPattern);

    private static unsafe CXXMethodDecl LambdaStaticInvokerFactory(CXXRecordDecl self) => self.TranslationUnit.GetOrCreate<CXXMethodDecl>(self.Handle.LambdaStaticInvoker);

    private static unsafe Decl LambdaContextDeclFactory(CXXRecordDecl self) => self.TranslationUnit.GetOrCreate<Decl>(self.Handle.LambdaContextDecl);

    private static unsafe CXXMethodDecl LambdaCallOperatorFactory(CXXRecordDecl self) => self.TranslationUnit.GetOrCreate<CXXMethodDecl>(self.Handle.LambdaCallOperator);

    private static unsafe CXXRecordDecl InstantiatedFromMemberClassFactory(CXXRecordDecl self) => self.TranslationUnit.GetOrCreate<CXXRecordDecl>(self.Handle.InstantiatedFromMember);

    private static unsafe CXXDestructorDecl? DestructorFactory(CXXRecordDecl self) {
            var destructor = self.Handle.Destructor;
            return destructor.IsNull ? null : self.TranslationUnit.GetOrCreate<CXXDestructorDecl>(self.Handle.Destructor);
        }

    private static unsafe ClassTemplateDecl DescribedClassTemplateFactory(CXXRecordDecl self) => self.TranslationUnit.GetOrCreate<ClassTemplateDecl>(self.Handle.DescribedCursorTemplate);

    private static unsafe FunctionTemplateDecl DependentLambdaCallOperatorFactory(CXXRecordDecl self) => self.TranslationUnit.GetOrCreate<FunctionTemplateDecl>(self.Handle.DependentLambdaCallOperator);

    private static unsafe CXXBaseSpecifier BasesFactory(object self, int i)
    {
        var @this = (CXXRecordDecl)self;
        return @this.TranslationUnit.GetOrCreate<CXXBaseSpecifier>(@this.Handle.GetBase(unchecked((uint)i)));
    }

    private static unsafe CXXConstructorDecl CtorsFactory(object self, int i)
    {
        var @this = (CXXRecordDecl)self;
        return @this.TranslationUnit.GetOrCreate<CXXConstructorDecl>(@this.Handle.GetCtor(unchecked((uint)i)));
    }

    private static unsafe FriendDecl FriendsFactory(object self, int i)
    {
        var @this = (CXXRecordDecl)self;
        return @this.TranslationUnit.GetOrCreate<FriendDecl>(@this.Handle.GetFriend(unchecked((uint)i)));
    }

    private static unsafe CXXMethodDecl MethodsFactory(object self, int i)
    {
        var @this = (CXXRecordDecl)self;
        return @this.TranslationUnit.GetOrCreate<CXXMethodDecl>(@this.Handle.GetMethod(unchecked((uint)i)));
    }

    private static unsafe CXXBaseSpecifier VbasesFactory(object self, int i)
    {
        var @this = (CXXRecordDecl)self;
        return @this.TranslationUnit.GetOrCreate<CXXBaseSpecifier>(@this.Handle.GetVBase(unchecked((uint)i)));
    }
}
