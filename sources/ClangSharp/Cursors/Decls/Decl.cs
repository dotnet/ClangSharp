// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public class Decl : Cursor
{
    private ValueLazy<Decl, FunctionDecl> _asFunction;
    private readonly LazyList<Attr> _attrs;
    private ValueLazy<Decl, Stmt?> _body;
    private ValueLazy<Decl, Decl> _canonicalDecl;
    private readonly LazyList<Decl> _decls;
    private ValueLazy<Decl, TemplateDecl?> _describedTemplate;
    private ValueLazy<Decl, Decl> _mostRecentDecl;
    private ValueLazy<Decl, Decl> _nextDeclInContext;
    private ValueLazy<Decl, Decl> _nonClosureContext;
    private ValueLazy<Decl, IDeclContext?> _parentFunctionOrMethod;
    private ValueLazy<Decl, Decl> _previousDecl;
    private ValueLazy<Decl, IDeclContext?> _redeclContext;
    private ValueLazy<Decl, TranslationUnitDecl> _translationUnitDecl;

    private protected unsafe Decl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind)
    {
        // When the native libClangSharp doesn't have a mapping for a declaration kind,
        // it returns CX_DeclKind_Invalid. When the default case in Decl.Create() constructs
        // a generic Decl with expectedDeclKind == CX_DeclKind_Invalid, we should allow it
        // rather than throwing, so that unknown declaration kinds degrade gracefully to a
        // base Decl wrapper instead of crashing the entire traversal.
        if (handle.DeclKind != expectedDeclKind)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        _asFunction = new ValueLazy<Decl, FunctionDecl>(&AsFunctionFactory);
        _attrs = LazyList.Create<Attr>(this, Handle.NumAttrs, &AttrsFactory);
        _body = new ValueLazy<Decl, Stmt?>(&BodyFactory);
        _canonicalDecl = new ValueLazy<Decl, Decl>(&CanonicalDeclFactory);
        _decls = LazyList.Create<Decl>(this, Handle.NumDecls, &DeclsFactory);
        _describedTemplate = new ValueLazy<Decl, TemplateDecl?>(&DescribedTemplateFactory);
        _mostRecentDecl = new ValueLazy<Decl, Decl>(&MostRecentDeclFactory);
        _nextDeclInContext = new ValueLazy<Decl, Decl>(&NextDeclInContextFactory);
        _nonClosureContext = new ValueLazy<Decl, Decl>(&NonClosureContextFactory);
        _parentFunctionOrMethod = new ValueLazy<Decl, IDeclContext?>(&ParentFunctionOrMethodFactory);
        _previousDecl = new ValueLazy<Decl, Decl>(&PreviousDeclFactory);
        _redeclContext = new ValueLazy<Decl, IDeclContext?>(&RedeclContextFactory);
        _translationUnitDecl = new ValueLazy<Decl, TranslationUnitDecl>(&TranslationUnitDeclFactory);
    }

    public CX_CXXAccessSpecifier Access => Handle.CXXAccessSpecifier;

    public FunctionDecl AsFunction => _asFunction.GetValue(this);

    public IReadOnlyList<Attr> Attrs => _attrs;

    public CXAvailabilityKind Availability => Handle.Availability;

    public Stmt? Body => _body.GetValue(this);

    public Decl CanonicalDecl => _canonicalDecl.GetValue(this);

    public IDeclContext? DeclContext => SemanticParentCursor as IDeclContext;

    public string DeclKindName => Handle.DeclKindSpelling;

    public IReadOnlyList<Decl> Decls => _decls;

    /// <summary>
    /// Per clang documentation: This returns null for partial specializations, because they are not modeled as TemplateDecls.
    /// Use DescribedTemplateParams to handle those cases.
    /// </summary>
    public TemplateDecl? DescribedTemplate => _describedTemplate.GetValue(this);

    public bool HasAttrs => Handle.HasAttrs;

    public bool IsCanonicalDecl => Handle.IsCanonical;

    public bool IsDeprecated => Handle.IsDeprecated;

    public bool IsInStdNamespace => (DeclContext?.IsStdNamespace).GetValueOrDefault();

    public bool IsInvalidDecl => Handle.IsInvalidDeclaration;

    public bool IsNamespace => Kind == CX_DeclKind_Namespace;

    public bool IsStdNamespace
    {
        get
        {
            if (this is NamespaceDecl nd)
            {
                var parent = nd.Parent;
                Debug.Assert(parent is not null);

                if (nd.IsInline)
                {
                    return parent!.IsStdNamespace;
                }
                else
                {
                    var redeclContext = parent!.RedeclContext;
                    Debug.Assert(redeclContext is not null);
                    return redeclContext!.IsTranslationUnit && nd.Name.Equals("std", StringComparison.Ordinal);
                }
            }

            return false;
        }
    }

    public bool IsTemplated => Handle.IsTemplated;

    public bool IsTranslationUnit => Kind == CX_DeclKind_TranslationUnit;

    public bool IsUnavailable => Handle.IsUnavailable;

    public bool IsUnconditionallyVisible => Handle.IsUnconditionallyVisible;

    public CX_DeclKind Kind => Handle.DeclKind;

    public IDeclContext? LexicalDeclContext => LexicalParentCursor as IDeclContext;

    public IDeclContext? LexicalParent => (this is IDeclContext) ? LexicalDeclContext : null;

    public uint MaxAlignment => Handle.MaxAlignment;

    public Decl MostRecentDecl => _mostRecentDecl.GetValue(this);

    public Decl NextDeclInContext => _nextDeclInContext.GetValue(this);

    public Decl NonClosureContext => _nonClosureContext.GetValue(this);

    public IDeclContext? Parent => (this is IDeclContext) ? DeclContext : null;

    public IDeclContext? ParentFunctionOrMethod => _parentFunctionOrMethod.GetValue(this);

    public Decl PreviousDecl => _previousDecl.GetValue(this);

    public IDeclContext? RedeclContext => _redeclContext.GetValue(this);

    public CXSourceRange SourceRange => clangsharp.Cursor_getSourceRange(Handle);

    public CXSourceRange SourceRangeRaw => clangsharp.Cursor_getSourceRangeRaw(Handle);

    public TranslationUnitDecl TranslationUnitDecl => _translationUnitDecl.GetValue(this);

    internal static new Decl Create(CXCursor handle) => handle.DeclKind switch {
        CX_DeclKind_Invalid => new Decl(handle, handle.kind, handle.DeclKind),
        CX_DeclKind_TranslationUnit => new TranslationUnitDecl(handle),
        CX_DeclKind_TopLevelStmt => new TopLevelStmtDecl(handle),
        CX_DeclKind_RequiresExprBody => new RequiresExprBodyDecl(handle),
        CX_DeclKind_OutlinedFunction => new OutlinedFunctionDecl(handle),
        CX_DeclKind_LinkageSpec => new LinkageSpecDecl(handle),
        CX_DeclKind_ExternCContext => new ExternCContextDecl(handle),
        CX_DeclKind_Export => new ExportDecl(handle),
        CX_DeclKind_Captured => new CapturedDecl(handle),
        CX_DeclKind_Block => new BlockDecl(handle),
        CX_DeclKind_StaticAssert => new StaticAssertDecl(handle),
        CX_DeclKind_PragmaDetectMismatch => new PragmaDetectMismatchDecl(handle),
        CX_DeclKind_PragmaComment => new PragmaCommentDecl(handle),
        CX_DeclKind_OpenACCRoutine => new OpenACCRoutineDecl(handle),
        CX_DeclKind_OpenACCDeclare => new OpenACCDeclareDecl(handle),
        CX_DeclKind_ObjCPropertyImpl => new ObjCPropertyImplDecl(handle),
        CX_DeclKind_OMPThreadPrivate => new OMPThreadPrivateDecl(handle),
        CX_DeclKind_OMPRequires => new OMPRequiresDecl(handle),
        CX_DeclKind_OMPAllocate => new OMPAllocateDecl(handle),
        CX_DeclKind_ObjCMethod => new ObjCMethodDecl(handle),
        CX_DeclKind_ObjCProtocol => new ObjCProtocolDecl(handle),
        CX_DeclKind_ObjCInterface => new ObjCInterfaceDecl(handle),
        CX_DeclKind_ObjCImplementation => new ObjCImplementationDecl(handle),
        CX_DeclKind_ObjCCategoryImpl => new ObjCCategoryImplDecl(handle),
        CX_DeclKind_ObjCCategory => new ObjCCategoryDecl(handle),
        CX_DeclKind_Namespace => new NamespaceDecl(handle),
        CX_DeclKind_HLSLBuffer => new HLSLBufferDecl(handle),
        CX_DeclKind_OMPDeclareReduction => new OMPDeclareReductionDecl(handle),
        CX_DeclKind_OMPDeclareMapper => new OMPDeclareMapperDecl(handle),
        CX_DeclKind_UnresolvedUsingValue => new UnresolvedUsingValueDecl(handle),
        CX_DeclKind_UnnamedGlobalConstant => new UnnamedGlobalConstantDecl(handle),
        CX_DeclKind_TemplateParamObject => new TemplateParamObjectDecl(handle),
        CX_DeclKind_MSGuid => new MSGuidDecl(handle),
        CX_DeclKind_IndirectField => new IndirectFieldDecl(handle),
        CX_DeclKind_EnumConstant => new EnumConstantDecl(handle),
        CX_DeclKind_Function => new FunctionDecl(handle),
        CX_DeclKind_CXXMethod => new CXXMethodDecl(handle),
        CX_DeclKind_CXXDestructor => new CXXDestructorDecl(handle),
        CX_DeclKind_CXXConversion => new CXXConversionDecl(handle),
        CX_DeclKind_CXXConstructor => new CXXConstructorDecl(handle),
        CX_DeclKind_CXXDeductionGuide => new CXXDeductionGuideDecl(handle),
        CX_DeclKind_Var => new VarDecl(handle),
        CX_DeclKind_VarTemplateSpecialization => new VarTemplateSpecializationDecl(handle),
        CX_DeclKind_VarTemplatePartialSpecialization => new VarTemplatePartialSpecializationDecl(handle),
        CX_DeclKind_ParmVar => new ParmVarDecl(handle),
        CX_DeclKind_OMPCapturedExpr => new OMPCapturedExprDecl(handle),
        CX_DeclKind_ImplicitParam => new ImplicitParamDecl(handle),
        CX_DeclKind_Decomposition => new DecompositionDecl(handle),
        CX_DeclKind_NonTypeTemplateParm => new NonTypeTemplateParmDecl(handle),
        CX_DeclKind_MSProperty => new MSPropertyDecl(handle),
        CX_DeclKind_Field => new FieldDecl(handle),
        CX_DeclKind_ObjCIvar => new ObjCIvarDecl(handle),
        CX_DeclKind_ObjCAtDefsField => new ObjCAtDefsFieldDecl(handle),
        CX_DeclKind_Binding => new BindingDecl(handle),
        CX_DeclKind_UsingShadow => new UsingShadowDecl(handle),
        CX_DeclKind_ConstructorUsingShadow => new ConstructorUsingShadowDecl(handle),
        CX_DeclKind_UsingPack => new UsingPackDecl(handle),
        CX_DeclKind_UsingDirective => new UsingDirectiveDecl(handle),
        CX_DeclKind_UnresolvedUsingIfExists => new UnresolvedUsingIfExistsDecl(handle),
        CX_DeclKind_Record => new RecordDecl(handle),
        CX_DeclKind_CXXRecord => new CXXRecordDecl(handle),
        CX_DeclKind_ClassTemplateSpecialization => new ClassTemplateSpecializationDecl(handle),
        CX_DeclKind_ClassTemplatePartialSpecialization => new ClassTemplatePartialSpecializationDecl(handle),
        CX_DeclKind_Enum => new EnumDecl(handle),
        CX_DeclKind_UnresolvedUsingTypename => new UnresolvedUsingTypenameDecl(handle),
        CX_DeclKind_Typedef => new TypedefDecl(handle),
        CX_DeclKind_TypeAlias => new TypeAliasDecl(handle),
        CX_DeclKind_ObjCTypeParam => new ObjCTypeParamDecl(handle),
        CX_DeclKind_TemplateTypeParm => new TemplateTypeParmDecl(handle),
        CX_DeclKind_TemplateTemplateParm => new TemplateTemplateParmDecl(handle),
        CX_DeclKind_VarTemplate => new VarTemplateDecl(handle),
        CX_DeclKind_TypeAliasTemplate => new TypeAliasTemplateDecl(handle),
        CX_DeclKind_FunctionTemplate => new FunctionTemplateDecl(handle),
        CX_DeclKind_ClassTemplate => new ClassTemplateDecl(handle),
        CX_DeclKind_Concept => new ConceptDecl(handle),
        CX_DeclKind_BuiltinTemplate => new BuiltinTemplateDecl(handle),
        CX_DeclKind_ObjCProperty => new ObjCPropertyDecl(handle),
        CX_DeclKind_ObjCCompatibleAlias => new ObjCCompatibleAliasDecl(handle),
        CX_DeclKind_NamespaceAlias => new NamespaceAliasDecl(handle),
        CX_DeclKind_Label => new LabelDecl(handle),
        CX_DeclKind_HLSLRootSignature => new HLSLRootSignatureDecl(handle),
        CX_DeclKind_UsingEnum => new UsingEnumDecl(handle),
        CX_DeclKind_Using => new UsingDecl(handle),
        CX_DeclKind_LifetimeExtendedTemporary => new LifetimeExtendedTemporaryDecl(handle),
        CX_DeclKind_Import => new ImportDecl(handle),
        CX_DeclKind_ImplicitConceptSpecialization => new ImplicitConceptSpecializationDecl(handle),
        CX_DeclKind_FriendTemplate => new FriendTemplateDecl(handle),
        CX_DeclKind_Friend => new FriendDecl(handle),
        CX_DeclKind_FileScopeAsm => new FileScopeAsmDecl(handle),
        CX_DeclKind_Empty => new EmptyDecl(handle),
        CX_DeclKind_AccessSpec => new AccessSpecDecl(handle),
        _ => new Decl(handle, handle.kind, handle.DeclKind),
    };

    private static unsafe TranslationUnitDecl TranslationUnitDeclFactory(Decl self) => self.TranslationUnit.GetOrCreate<TranslationUnitDecl>(self.Handle.TranslationUnit.Cursor);

    private static unsafe IDeclContext? RedeclContextFactory(Decl self) => self.TranslationUnit.GetOrCreate<Decl>(self.Handle.RedeclContext) as IDeclContext;

    private static unsafe Decl PreviousDeclFactory(Decl self) => self.TranslationUnit.GetOrCreate<Decl>(self.Handle.PreviousDecl);

    private static unsafe IDeclContext? ParentFunctionOrMethodFactory(Decl self) => self.TranslationUnit.GetOrCreate<Decl>(self.Handle.ParentFunctionOrMethod) as IDeclContext;

    private static unsafe Decl NonClosureContextFactory(Decl self) => self.TranslationUnit.GetOrCreate<Decl>(self.Handle.NonClosureContext);

    private static unsafe Decl NextDeclInContextFactory(Decl self) => self.TranslationUnit.GetOrCreate<Decl>(self.Handle.NextDeclInContext);

    private static unsafe Decl MostRecentDeclFactory(Decl self) => self.TranslationUnit.GetOrCreate<Decl>(self.Handle.MostRecentDecl);

    private static unsafe TemplateDecl? DescribedTemplateFactory(Decl self) {
            var describedTemplate = self.Handle.DescribedTemplate;
            return describedTemplate.IsNull ? null : self.TranslationUnit.GetOrCreate<TemplateDecl>(describedTemplate);
        }

    private static unsafe Decl CanonicalDeclFactory(Decl self) => self.TranslationUnit.GetOrCreate<Decl>(self.Handle.CanonicalCursor);

    private static unsafe Stmt? BodyFactory(Decl self) => !self.Handle.Body.IsNull ? self.TranslationUnit.GetOrCreate<Stmt>(self.Handle.Body) : null;

    private static unsafe FunctionDecl AsFunctionFactory(Decl self) => self.TranslationUnit.GetOrCreate<FunctionDecl>(self.Handle.AsFunction);

    private static unsafe Attr AttrsFactory(object self, int i)
    {
        var @this = (Decl)self;
        return @this.TranslationUnit.GetOrCreate<Attr>(@this.Handle.GetAttr(unchecked((uint)i)));
    }

    private static unsafe Decl DeclsFactory(object self, int i, Decl? previousDecl)
    {
        var @this = (Decl)self;

        if (previousDecl is null)
        {
            return @this.TranslationUnit.GetOrCreate<Decl>(@this.Handle.GetDecl(unchecked((uint)i)));
        }
        else
        {
            return previousDecl.NextDeclInContext;
        }
    }

    private protected static unsafe LazyList<LazyList<NamedDecl>> CreateTemplateParameterLists(Decl self)
    {
        return LazyList.Create<LazyList<NamedDecl>>(self, self.Handle.NumTemplateParameterLists, &TemplateParameterListFactory);
    }

    private static unsafe LazyList<NamedDecl> TemplateParameterListFactory(object self, int listIndex)
    {
        var @this = (Decl)self;
        var numTemplateParameters = @this.Handle.GetNumTemplateParameters(unchecked((uint)listIndex));
        return LazyList.Create<NamedDecl>(new TemplateParameterListContext(@this, unchecked((uint)listIndex)), numTemplateParameters, &TemplateParameterFactory);
    }

    private static unsafe NamedDecl TemplateParameterFactory(object self, int parameterIndex)
    {
        var context = (TemplateParameterListContext)self;
        return context.Owner.TranslationUnit.GetOrCreate<NamedDecl>(context.Owner.Handle.GetTemplateParameter(context.ListIndex, unchecked((uint)parameterIndex)));
    }

    private sealed class TemplateParameterListContext(Decl owner, uint listIndex)
    {
        public readonly Decl Owner = owner;
        public readonly uint ListIndex = listIndex;
    }
}
