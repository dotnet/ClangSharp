// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class Decl : Cursor
    {
        private readonly Lazy<IReadOnlyList<Attr>> _attrs;
        private readonly Lazy<Decl> _canonicalDecl;
        private readonly Lazy<IDeclContext> _declContext;
        private readonly Lazy<IDeclContext> _lexicalDeclContext;
        private readonly Lazy<TranslationUnitDecl> _translationUnitDecl;

        private protected Decl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind)
        {
            if ((handle.DeclKind == CX_DeclKind.CX_DeclKind_Invalid) || (handle.DeclKind != expectedDeclKind))
            {
                throw new ArgumentException(nameof(handle));
            }

            _attrs = new Lazy<IReadOnlyList<Attr>>(() => CursorChildren.OfType<Attr>().ToList());
            _canonicalDecl = new Lazy<Decl>(() => TranslationUnit.GetOrCreate<Decl>(Handle.CanonicalCursor));
            _declContext = new Lazy<IDeclContext>(() => TranslationUnit.GetOrCreate<Decl>(Handle.SemanticParent) as IDeclContext);
            _lexicalDeclContext = new Lazy<IDeclContext>(() => TranslationUnit.GetOrCreate<Decl>(Handle.LexicalParent) as IDeclContext);
            _translationUnitDecl = new Lazy<TranslationUnitDecl>(() => TranslationUnit.GetOrCreate<TranslationUnitDecl>(Handle.TranslationUnit.Cursor));
        }

        public CX_CXXAccessSpecifier Access => Handle.CXXAccessSpecifier;

        public IReadOnlyList<Attr> Attrs => _attrs.Value;

        public CXAvailabilityKind Availability => Handle.Availability;

        public Decl CanonicalDecl => _canonicalDecl.Value;

        public IDeclContext DeclContext => _declContext.Value;

        public string DeclKindName => Handle.DeclKindSpelling;

        public bool HasAttrs => Handle.HasAttrs;

        public bool IsCanonicalDecl => Handle.IsCanonical;

        public bool IsInvalidDecl => Handle.IsInvalidDeclaration;

        public CX_DeclKind Kind => Handle.DeclKind;

        public IDeclContext LexicalDeclContext => _lexicalDeclContext.Value;

        public TranslationUnitDecl TranslationUnitDecl => _translationUnitDecl.Value;

        internal static new Decl Create(CXCursor handle) => handle.DeclKind switch
        {
            CX_DeclKind.CX_DeclKind_AccessSpec => new AccessSpecDecl(handle),
            CX_DeclKind.CX_DeclKind_Block => new BlockDecl(handle),
            CX_DeclKind.CX_DeclKind_Captured => new CapturedDecl(handle),
            CX_DeclKind.CX_DeclKind_ClassScopeFunctionSpecialization => new ClassScopeFunctionSpecializationDecl(handle),
            CX_DeclKind.CX_DeclKind_Empty => new EmptyDecl(handle),
            CX_DeclKind.CX_DeclKind_Export => new ExportDecl(handle),
            CX_DeclKind.CX_DeclKind_ExternCContext => new ExternCContextDecl(handle),
            CX_DeclKind.CX_DeclKind_FileScopeAsm => new FileScopeAsmDecl(handle),
            CX_DeclKind.CX_DeclKind_Friend => new FriendDecl(handle),
            CX_DeclKind.CX_DeclKind_FriendTemplate => new FriendTemplateDecl(handle),
            CX_DeclKind.CX_DeclKind_Import => new ImportDecl(handle),
            CX_DeclKind.CX_DeclKind_LinkageSpec => new LinkageSpecDecl(handle),
            CX_DeclKind.CX_DeclKind_Label => new LabelDecl(handle),
            CX_DeclKind.CX_DeclKind_Namespace => new NamespaceDecl(handle),
            CX_DeclKind.CX_DeclKind_NamespaceAlias => new NamespaceAliasDecl(handle),
            CX_DeclKind.CX_DeclKind_ObjCCompatibleAlias => new ObjCCompatibleAliasDecl(handle),
            CX_DeclKind.CX_DeclKind_ObjCCategory => new ObjCCategoryDecl(handle),
            CX_DeclKind.CX_DeclKind_ObjCCategoryImpl => new ObjCCategoryImplDecl(handle),
            CX_DeclKind.CX_DeclKind_ObjCImplementation => new ObjCImplementationDecl(handle),
            CX_DeclKind.CX_DeclKind_ObjCInterface => new ObjCInterfaceDecl(handle),
            CX_DeclKind.CX_DeclKind_ObjCProtocol => new ObjCPropertyDecl(handle),
            CX_DeclKind.CX_DeclKind_ObjCMethod => new ObjCMethodDecl(handle),
            CX_DeclKind.CX_DeclKind_ObjCProperty => new ObjCPropertyDecl(handle),
            CX_DeclKind.CX_DeclKind_BuiltinTemplate => new BuiltinTemplateDecl(handle),
            CX_DeclKind.CX_DeclKind_Concept => new ConceptDecl(handle),
            CX_DeclKind.CX_DeclKind_ClassTemplate => new ClassTemplateDecl(handle),
            CX_DeclKind.CX_DeclKind_FunctionTemplate => new FunctionTemplateDecl(handle),
            CX_DeclKind.CX_DeclKind_TypeAliasTemplate => new TypeAliasTemplateDecl(handle),
            CX_DeclKind.CX_DeclKind_VarTemplate => new VarTemplateDecl(handle),
            CX_DeclKind.CX_DeclKind_TemplateTemplateParm => new TemplateTemplateParmDecl(handle),
            CX_DeclKind.CX_DeclKind_Enum => new EnumDecl(handle),
            CX_DeclKind.CX_DeclKind_Record => new RecordDecl(handle),
            CX_DeclKind.CX_DeclKind_CXXRecord => new CXXRecordDecl(handle),
            CX_DeclKind.CX_DeclKind_ClassTemplateSpecialization => new ClassTemplateSpecializationDecl(handle),
            CX_DeclKind.CX_DeclKind_ClassTemplatePartialSpecialization => new ClassTemplatePartialSpecializationDecl(handle),
            CX_DeclKind.CX_DeclKind_TemplateTypeParm => new TemplateTypeParmDecl(handle),
            CX_DeclKind.CX_DeclKind_ObjCTypeParam => new ObjCTypeParamDecl(handle),
            CX_DeclKind.CX_DeclKind_TypeAlias => new TypeAliasDecl(handle),
            CX_DeclKind.CX_DeclKind_Typedef => new TypedefDecl(handle),
            CX_DeclKind.CX_DeclKind_UnresolvedUsingTypename => new UnresolvedUsingTypenameDecl(handle),
            CX_DeclKind.CX_DeclKind_Using => new UsingDecl(handle),
            CX_DeclKind.CX_DeclKind_UsingDirective => new UsingDirectiveDecl(handle),
            CX_DeclKind.CX_DeclKind_UsingPack => new UsingPackDecl(handle),
            CX_DeclKind.CX_DeclKind_UsingShadow => new UsingShadowDecl(handle),
            CX_DeclKind.CX_DeclKind_ConstructorUsingShadow => new ConstructorUsingShadowDecl(handle),
            CX_DeclKind.CX_DeclKind_Binding => new BindingDecl(handle),
            CX_DeclKind.CX_DeclKind_Field => new FieldDecl(handle),
            CX_DeclKind.CX_DeclKind_ObjCAtDefsField => new ObjCAtDefsFieldDecl(handle),
            CX_DeclKind.CX_DeclKind_ObjCIvar => new ObjCIvarDecl(handle),
            CX_DeclKind.CX_DeclKind_Function => new FunctionDecl(handle),
            CX_DeclKind.CX_DeclKind_CXXDeductionGuide => new CXXDeductionGuideDecl(handle),
            CX_DeclKind.CX_DeclKind_CXXMethod => new CXXMethodDecl(handle),
            CX_DeclKind.CX_DeclKind_CXXConstructor => new CXXConstructorDecl(handle),
            CX_DeclKind.CX_DeclKind_CXXConversion => new CXXConversionDecl(handle),
            CX_DeclKind.CX_DeclKind_CXXDestructor => new CXXDestructorDecl(handle),
            CX_DeclKind.CX_DeclKind_MSProperty => new MSPropertyDecl(handle),
            CX_DeclKind.CX_DeclKind_NonTypeTemplateParm => new NonTypeTemplateParmDecl(handle),
            CX_DeclKind.CX_DeclKind_Var => new VarDecl(handle),
            CX_DeclKind.CX_DeclKind_Decomposition => new DecompositionDecl(handle),
            CX_DeclKind.CX_DeclKind_ImplicitParam => new ImplicitParamDecl(handle),
            CX_DeclKind.CX_DeclKind_OMPCapturedExpr => new OMPCapturedExprDecl(handle),
            CX_DeclKind.CX_DeclKind_ParmVar => new ParmVarDecl(handle),
            CX_DeclKind.CX_DeclKind_VarTemplateSpecialization => new VarTemplateSpecializationDecl(handle),
            CX_DeclKind.CX_DeclKind_VarTemplatePartialSpecialization => new VarTemplatePartialSpecializationDecl(handle),
            CX_DeclKind.CX_DeclKind_EnumConstant => new EnumConstantDecl(handle),
            CX_DeclKind.CX_DeclKind_IndirectField => new IndirectFieldDecl(handle),
            CX_DeclKind.CX_DeclKind_OMPDeclareMapper => new OMPDeclareMapperDecl(handle),
            CX_DeclKind.CX_DeclKind_OMPDeclareReduction => new OMPDeclareReductionDecl(handle),
            CX_DeclKind.CX_DeclKind_UnresolvedUsingValue => new UnresolvedUsingValueDecl(handle),
            CX_DeclKind.CX_DeclKind_OMPAllocate => new OMPAllocateDecl(handle),
            CX_DeclKind.CX_DeclKind_OMPRequires => new OMPRequiresDecl(handle),
            CX_DeclKind.CX_DeclKind_OMPThreadPrivate => new OMPThreadPrivateDecl(handle),
            CX_DeclKind.CX_DeclKind_ObjCPropertyImpl => new ObjCPropertyImplDecl(handle),
            CX_DeclKind.CX_DeclKind_PragmaComment => new PragmaCommentDecl(handle),
            CX_DeclKind.CX_DeclKind_PragmaDetectMismatch => new PragmaDetectMismatchDecl(handle),
            CX_DeclKind.CX_DeclKind_StaticAssert => new StaticAssertDecl(handle),
            CX_DeclKind.CX_DeclKind_TranslationUnit => new TranslationUnitDecl(handle),
            _ => new Decl(handle, handle.kind, handle.DeclKind),
        };
    }
}
