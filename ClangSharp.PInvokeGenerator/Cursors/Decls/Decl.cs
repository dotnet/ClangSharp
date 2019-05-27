using System;
using System.Diagnostics;

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
                {
                    return new StructDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_UnionDecl:
                {
                    return new UnionDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_ClassDecl:
                {
                    return new ClassDecl(handle, parent);
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

        private readonly Lazy<Cursor> _definition;

        protected Decl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.IsDeclaration);

            _definition = new Lazy<Cursor>(() => {
                var cursor = TranslationUnit.GetOrCreateCursor(Handle.Definition, () => Create(Handle.Definition, this));
                cursor.Visit(clientData: default);
                return cursor;
            });
        }

        public CX_CXXAccessSpecifier AccessSpecifier => Handle.CXXAccessSpecifier;

        public CXAvailabilityKind Availability => Handle.Availability;

        public string BriefCommentText => Handle.BriefCommentText.ToString();

        public CXSourceRange CommentRange => Handle.CommentRange;

        public Cursor Definition => _definition.Value;

        public int ExceptionSpecificationType => Handle.ExceptionSpecificationType;

        public bool HasAttrs => Handle.HasAttrs;

        public bool IsDefinition => Handle.IsDefinition;
    }
}
