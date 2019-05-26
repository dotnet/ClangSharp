using System;
using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
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
                    return new ParmDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_TypedefDecl:
                {
                    return new TypedefDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_CXXMethod:
                {
                    return new CXXMethod(handle, parent);
                }

                case CXCursorKind.CXCursor_Namespace:
                {
                    return new Namespace(handle, parent);
                }

                case CXCursorKind.CXCursor_Constructor:
                {
                    return new Constructor(handle, parent);
                }

                case CXCursorKind.CXCursor_Destructor:
                {
                    return new Destructor(handle, parent);
                }

                case CXCursorKind.CXCursor_ConversionFunction:
                {
                    return new ConversionFunction(handle, parent);
                }

                case CXCursorKind.CXCursor_TemplateTypeParameter:
                {
                    return new TemplateTypeParameter(handle, parent);
                }

                case CXCursorKind.CXCursor_NonTypeTemplateParameter:
                {
                    return new NonTypeTemplateParameter(handle, parent);
                }

                case CXCursorKind.CXCursor_TemplateTemplateParameter:
                {
                    return new TemplateTemplateParameter(handle, parent);
                }

                case CXCursorKind.CXCursor_FunctionTemplate:
                {
                    return new FunctionTemplate(handle, parent);
                }

                case CXCursorKind.CXCursor_ClassTemplate:
                {
                    return new ClassTemplate(handle, parent);
                }

                case CXCursorKind.CXCursor_ClassTemplatePartialSpecialization:
                {
                    return new ClassTemplatePartialSpecialization(handle, parent);
                }

                case CXCursorKind.CXCursor_UsingDeclaration:
                {
                    return new UsingDeclaration(handle, parent);
                }

                case CXCursorKind.CXCursor_TypeAliasDecl:
                {
                    return new TypeAliasDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_CXXAccessSpecifier:
                {
                    return new CXXAccessSpecifier(handle, parent);
                }

                case CXCursorKind.CXCursor_TypeAliasTemplateDecl:
                {
                    return new TypeAliasTemplateDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_StaticAssert:
                {
                    return new StaticAssert(handle, parent);
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

        private readonly Lazy<Type> _type;

        protected Decl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.IsDeclaration);
            _type = new Lazy<Type>(() => TranslationUnit.GetOrCreateType(Handle.Type, () => Type.Create(Handle.Type, TranslationUnit)));
        }

        public bool IsAnonymous => Handle.IsAnonymous;

        public Type Type => _type.Value;
    }
}
