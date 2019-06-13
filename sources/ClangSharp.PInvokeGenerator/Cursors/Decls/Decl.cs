using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal class Decl : Cursor
    {
        public static new Decl Create(CXCursor handle, Cursor parent)
        {
            switch (handle.Kind)
            {
                case CXCursorKind.CXCursor_UnexposedDecl:
                {
                    return new UnexposedDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_StructDecl:
                case CXCursorKind.CXCursor_UnionDecl:
                case CXCursorKind.CXCursor_ClassDecl:
                {
                    if (handle.Language == CXLanguageKind.CXLanguage_C)
                    {
                        return new RecordDecl(handle, parent);
                    }
                    else if (handle.Language == CXLanguageKind.CXLanguage_CPlusPlus)
                    {
                        return new CXXRecordDecl(handle, parent);
                    }
                    else
                    {
                        goto default;
                    }
                }

                case CXCursorKind.CXCursor_EnumDecl:
                {
                    return new EnumDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_FieldDecl:
                {
                    return new FieldDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_EnumConstantDecl:
                {
                    return new EnumConstantDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_FunctionDecl:
                {
                    return new FunctionDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_VarDecl:
                {
                    return new VarDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_ParmDecl:
                {
                    return new ParmVarDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_TypedefDecl:
                {
                    return new TypedefDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_CXXMethod:
                {
                    return new CXXMethodDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_Namespace:
                {
                    return new NamespaceDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_Constructor:
                {
                    return new CXXConstructorDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_Destructor:
                {
                    return new CXXDestructorDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_ConversionFunction:
                {
                    return new CXXConversionDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_TemplateTypeParameter:
                {
                    return new TemplateTypeParmDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_NonTypeTemplateParameter:
                {
                    return new NonTypeTemplateParmDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_TemplateTemplateParameter:
                {
                    return new TemplateTemplateParmDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_FunctionTemplate:
                {
                    return new FunctionTemplateDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_ClassTemplate:
                {
                    return new ClassTemplateDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_ClassTemplatePartialSpecialization:
                {
                    return new ClassTemplatePartialSpecializationDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_UsingDeclaration:
                {
                    return new UsingDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_TypeAliasDecl:
                {
                    return new TypeAliasDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_CXXAccessSpecifier:
                {
                    return new AccessSpecDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_TypeAliasTemplateDecl:
                {
                    return new TypeAliasTemplateDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_StaticAssert:
                {
                    return new StaticAssertDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_FriendDecl:
                {
                    return new FriendDecl(handle, parent);
                }

                default:
                {
                    Debug.WriteLine($"Unhandled declaration kind: {handle.KindSpelling}.");
                    Debugger.Break();
                    return new Decl(handle, parent);
                }
            }
        }

        private readonly List<Attr> _attributes = new List<Attr>();
        private readonly Lazy<Decl> _canonical;
        private readonly Lazy<Cursor> _lexicalParent;

        protected Decl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.IsDeclaration);

            _canonical = new Lazy<Decl>(() => {
                var cursor = TranslationUnit.GetOrCreateCursor(Handle.CanonicalCursor, () => Create(Handle.CanonicalCursor, this));
                cursor?.Visit(clientData: default);
                return (Decl)cursor;
            });
            _lexicalParent = new Lazy<Cursor>(() => {
                var cursor = TranslationUnit.GetOrCreateCursor(Handle.LexicalParent, () => Create(Handle.LexicalParent, this));
                cursor?.Visit(clientData: default);
                return cursor;
            });
        }

        public CX_CXXAccessSpecifier AccessSpecifier => Handle.CXXAccessSpecifier;

        public IReadOnlyList<Attr> Attributes => _attributes;

        public CXAvailabilityKind Availability => Handle.Availability;

        public string BriefCommentText => Handle.BriefCommentText.ToString();

        public Decl Canonical => _canonical.Value;

        public CXSourceRange CommentRange => Handle.CommentRange;

        public int ExceptionSpecificationType => Handle.ExceptionSpecificationType;

        public bool HasAttrs => Handle.HasAttrs;

        public bool IsCanonical => Handle.IsCanonical;

        public bool IsInvalid => Handle.IsInvalidDeclaration;

        public CXLanguageKind Language => Handle.Language;

        public Cursor LexicalParent => _lexicalParent.Value;

        public CXComment ParsedComment => Handle.ParsedComment;

        public string RawCommentText => Handle.RawCommentText.ToString();

        protected override Attr GetOrAddAttr(CXCursor childHandle)
        {
            var attr = base.GetOrAddAttr(childHandle);

            Debug.Assert(!_attributes.Contains(attr));
            _attributes.Add(attr);

            return attr;
        }
    }
}
