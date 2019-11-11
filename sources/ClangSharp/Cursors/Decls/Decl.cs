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

        private protected Decl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
            _attrs = new Lazy<IReadOnlyList<Attr>>(() => CursorChildren.OfType<Attr>().ToList());
            _canonicalDecl = new Lazy<Decl>(() => TranslationUnit.GetOrCreate<Decl>(Handle.CanonicalCursor));
            _declContext = new Lazy<IDeclContext>(() => Create(Handle.SemanticParent) as IDeclContext);
            _lexicalDeclContext = new Lazy<IDeclContext>(() => Create(Handle.LexicalParent) as IDeclContext);
            _translationUnitDecl = new Lazy<TranslationUnitDecl>(() => TranslationUnit.GetOrCreate<TranslationUnitDecl>(Handle.TranslationUnit.Cursor));
        }

        public CX_CXXAccessSpecifier Access => Handle.CXXAccessSpecifier;

        public IReadOnlyList<Attr> Attrs => _attrs.Value;

        public CXAvailabilityKind Availability => Handle.Availability;

        public Decl CanonicalDecl => _canonicalDecl.Value;

        public IDeclContext DeclContext => _declContext.Value;

        public bool HasAttrs => Handle.HasAttrs;

        public bool IsCanonicalDecl => Handle.IsCanonical;

        public bool IsInvalidDecl => Handle.IsInvalidDeclaration;

        public IDeclContext LexicalDeclContext => _lexicalDeclContext.Value;

        public TranslationUnitDecl TranslationUnitDecl => _translationUnitDecl.Value;

        internal static new Decl Create(CXCursor handle)
        {
            Decl result;

            switch (handle.Kind)
            {
                case CXCursorKind.CXCursor_UnexposedDecl:
                {
                    result = new Decl(handle, handle.Kind);
                    break;
                }

                case CXCursorKind.CXCursor_StructDecl:
                case CXCursorKind.CXCursor_UnionDecl:
                case CXCursorKind.CXCursor_ClassDecl:
                {
                    if (handle.Language == CXLanguageKind.CXLanguage_CPlusPlus)
                    {
                        result = new CXXRecordDecl(handle, handle.Kind);
                    }
                    else
                    {
                        result = new RecordDecl(handle, handle.Kind);
                    }
                    break;
                }

                case CXCursorKind.CXCursor_EnumDecl:
                {
                    result = new EnumDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_FieldDecl:
                {
                    result = new FieldDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_EnumConstantDecl:
                {
                    result = new EnumConstantDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_FunctionDecl:
                {
                    result = new FunctionDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_VarDecl:
                {
                    result = new VarDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ParmDecl:
                {
                    result = new ParmVarDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCInterfaceDecl:
                {
                    result = new ObjCInterfaceDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCCategoryDecl:
                {
                    result = new ObjCCategoryDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCProtocolDecl:
                {
                    result = new ObjCProtocolDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCPropertyDecl:
                {
                    result = new ObjCPropertyDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCIvarDecl:
                {
                    result = new ObjCIvarDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCInstanceMethodDecl:
                case CXCursorKind.CXCursor_ObjCClassMethodDecl:
                {
                    result = new ObjCMethodDecl(handle, handle.Kind);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCImplementationDecl:
                {
                    result = new ObjCImplementationDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCCategoryImplDecl:
                {
                    result = new ObjCCategoryImplDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_TypedefDecl:
                {
                    result = new TypedefDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CXXMethod:
                {
                    result = new CXXMethodDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_Namespace:
                {
                    result = new NamespaceDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_LinkageSpec:
                {
                    result = new LinkageSpecDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_Constructor:
                {
                    result = new CXXConstructorDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_Destructor:
                {
                    result = new CXXDestructorDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ConversionFunction:
                {
                    result = new CXXConversionDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_TemplateTypeParameter:
                {
                    result = new TemplateTypeParmDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_NonTypeTemplateParameter:
                {
                    result = new NonTypeTemplateParmDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_TemplateTemplateParameter:
                {
                    result = new TemplateTemplateParmDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_FunctionTemplate:
                {
                    result = new FunctionTemplateDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ClassTemplate:
                {
                    result = new ClassTemplateDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ClassTemplatePartialSpecialization:
                {
                    result = new ClassTemplatePartialSpecializationDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_NamespaceAlias:
                {
                    result = new NamespaceAliasDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_UsingDirective:
                {
                    result = new UsingDirectiveDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_UsingDeclaration:
                {
                    result = new UsingDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_TypeAliasDecl:
                {
                    result = new TypeAliasDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CXXAccessSpecifier:
                {
                    result = new AccessSpecDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCSynthesizeDecl:
                case CXCursorKind.CXCursor_ObjCDynamicDecl:
                {
                    result = new ObjCPropertyImplDecl(handle, handle.Kind);
                    break;
                }

                case CXCursorKind.CXCursor_ModuleImportDecl:
                {
                    result = new ImportDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_TypeAliasTemplateDecl:
                {
                    result = new TypeAliasTemplateDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_StaticAssert:
                {
                    result = new StaticAssertDecl(handle);
                    break;
                }

                case CXCursorKind.CXCursor_FriendDecl:
                {
                    result = new FriendDecl(handle);
                    break;
                }

                default:
                {
                    Debug.WriteLine($"Unhandled declaration kind: {handle.KindSpelling}.");
                    result = new Decl(handle, handle.Kind);
                    break;
                }
            }

            return result;
        }
    }
}
