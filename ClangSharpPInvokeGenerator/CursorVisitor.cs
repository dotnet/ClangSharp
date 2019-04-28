using System;
using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal class CursorVisitor
    {
        public CXChildVisitResult VisitBinaryOperator(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            Debug.Assert(parent.Kind == CXCursorKind.CXCursor_BinaryOperator);

            switch (cursor.Kind)
            {
                case CXCursorKind.CXCursor_DeclRefExpr:
                {
                    return Handle(VisitDeclRefExpr, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_IntegerLiteral:
                {
                    return Handle(VisitIntegerLiteral, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_ParenExpr:
                {
                    return Handle(VisitParenExpr, cursor, parent, data);
                }

                default:
                {
                    return Unhandled(cursor, parent);
                }
            }
        }

        public CXChildVisitResult VisitDeclRefExpr(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            Debug.Assert(parent.Kind == CXCursorKind.CXCursor_DeclRefExpr);

            switch (cursor.Kind)
            {
                default:
                {
                    return Unhandled(cursor, parent);
                }
            }
        }

        public CXChildVisitResult VisitDLLImport(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            Debug.Assert(parent.Kind == CXCursorKind.CXCursor_DLLImport);

            switch (cursor.Kind)
            {
                default:
                {
                    return Unhandled(cursor, parent);
                }
            }
        }

        public CXChildVisitResult VisitEnumConstantDecl(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            Debug.Assert(parent.Kind == CXCursorKind.CXCursor_EnumConstantDecl);

            switch (cursor.Kind)
            {
                case CXCursorKind.CXCursor_DeclRefExpr:
                {
                    return Handle(VisitDeclRefExpr, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_IntegerLiteral:
                {
                    return Handle(VisitIntegerLiteral, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_ParenExpr:
                {
                    return Handle(VisitParenExpr, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_UnaryOperator:
                {
                    return Handle(VisitUnaryOperator, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_BinaryOperator:
                {
                    return Handle(VisitBinaryOperator, cursor, parent, data);
                }

                default:
                {
                    return Unhandled(cursor, parent);
                }
            }
        }

        public CXChildVisitResult VisitEnumDecl(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            Debug.Assert(parent.Kind == CXCursorKind.CXCursor_EnumDecl);

            switch (cursor.Kind)
            {
                case CXCursorKind.CXCursor_EnumConstantDecl:
                {
                    return Handle(VisitEnumConstantDecl, cursor, parent, data);
                }

                default:
                {
                    return Unhandled(cursor, parent);
                }
            }
        }

        public CXChildVisitResult VisitFieldDecl(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            Debug.Assert(parent.Kind == CXCursorKind.CXCursor_FieldDecl);

            switch (cursor.Kind)
            {
                case CXCursorKind.CXCursor_ParmDecl:
                {
                    return Handle(VisitParmDecl, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_TypeRef:
                {
                    return Handle(VisitTypeRef, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_IntegerLiteral:
                {
                    return Handle(VisitIntegerLiteral, cursor, parent, data);
                }

                default:
                {
                    return Unhandled(cursor, parent);
                }
            }
        }

        public CXChildVisitResult VisitFunctionDecl(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            Debug.Assert(parent.Kind == CXCursorKind.CXCursor_FunctionDecl);

            switch (cursor.Kind)
            {
                case CXCursorKind.CXCursor_ParmDecl:
                {
                    return Handle(VisitParmDecl, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_TypeRef:
                {
                    return Handle(VisitTypeRef, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_CompoundStmt:
                {
                    return CXChildVisitResult.CXChildVisit_Continue;
                }

                case CXCursorKind.CXCursor_UnexposedAttr:
                {
                    return Handle(VisitUnexposedAttr, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_DLLImport:
                {
                    return Handle(VisitDLLImport, cursor, parent, data);
                }

                default:
                {
                    return Unhandled(cursor, parent);
                }
            }
        }

        public CXChildVisitResult VisitIntegerLiteral(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            Debug.Assert(parent.Kind == CXCursorKind.CXCursor_IntegerLiteral);

            switch (cursor.Kind)
            {
                default:
                {
                    return Unhandled(cursor, parent);
                }
            }
        }

        public CXChildVisitResult VisitParenExpr(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            Debug.Assert(parent.Kind == CXCursorKind.CXCursor_ParenExpr);

            switch (cursor.Kind)
            {
                case CXCursorKind.CXCursor_BinaryOperator:
                {
                    return Handle(VisitBinaryOperator, cursor, parent, data);
                }

                default:
                {
                    return Unhandled(cursor, parent);
                }
            }
        }

        public CXChildVisitResult VisitParmDecl(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            Debug.Assert(parent.Kind == CXCursorKind.CXCursor_ParmDecl);

            switch (cursor.Kind)
            {
                case CXCursorKind.CXCursor_ParmDecl:
                {
                    return Handle(VisitParmDecl, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_TypeRef:
                {
                    return Handle(VisitTypeRef, cursor, parent, data);
                }

                default:
                {
                    return Unhandled(cursor, parent);
                }
            }
        }

        public CXChildVisitResult VisitStructDecl(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            Debug.Assert(parent.Kind == CXCursorKind.CXCursor_StructDecl);

            switch (cursor.Kind)
            {
                case CXCursorKind.CXCursor_FieldDecl:
                {
                    return Handle(VisitFieldDecl, cursor, parent, data);
                }

                default:
                {
                    return Unhandled(cursor, parent);
                }
            }
        }

        public CXChildVisitResult VisitTranslationUnit(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            Debug.Assert(parent.Kind == CXCursorKind.CXCursor_TranslationUnit);

            if (cursor.Location.IsInSystemHeader || !cursor.Location.IsFromMainFile)
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            switch (cursor.Kind)
            {
                case CXCursorKind.CXCursor_UnexposedDecl:
                {
                    return Handle(VisitUnexposedDecl, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_StructDecl:
                {
                    return Handle(VisitStructDecl, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_EnumDecl:
                {
                    return Handle(VisitEnumDecl, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_TypedefDecl:
                {
                    return Handle(VisitTypedefDecl, cursor, parent, data);
                }

                default:
                {
                    return Unhandled(cursor, parent);
                }
            }
        }

        public CXChildVisitResult VisitTypedefDecl(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            Debug.Assert(parent.Kind == CXCursorKind.CXCursor_TypedefDecl);

            switch (cursor.Kind)
            {
                case CXCursorKind.CXCursor_StructDecl:
                {
                    return Handle(VisitStructDecl, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_EnumDecl:
                {
                    return Handle(VisitEnumDecl, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_ParmDecl:
                {
                    return Handle(VisitParmDecl, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_TypeRef:
                {
                    return Handle(VisitTypeRef, cursor, parent, data);
                }

                default:
                {
                    return Unhandled(cursor, parent);
                }
            }
        }

        public CXChildVisitResult VisitTypeRef(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            Debug.Assert(parent.Kind == CXCursorKind.CXCursor_TypeRef);

            switch (cursor.Kind)
            {
                default:
                {
                    return Unhandled(cursor, parent);
                }
            }
        }

        public CXChildVisitResult VisitUnaryOperator(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            Debug.Assert(parent.Kind == CXCursorKind.CXCursor_UnaryOperator);

            switch (cursor.Kind)
            {
                case CXCursorKind.CXCursor_IntegerLiteral:
                {
                    return Handle(VisitIntegerLiteral, cursor, parent, data);
                }

                default:
                {
                    return Unhandled(cursor, parent);
                }
            }
        }

        public CXChildVisitResult VisitUnexposedAttr(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            Debug.Assert(parent.Kind == CXCursorKind.CXCursor_UnexposedAttr);

            switch (cursor.Kind)
            {
                default:
                {
                    return Unhandled(cursor, parent);
                }
            }
        }

        public CXChildVisitResult VisitUnexposedDecl(CXCursor cursor, CXCursor parent, CXClientData data)
        {
            Debug.Assert(parent.Kind == CXCursorKind.CXCursor_UnexposedDecl);

            switch (cursor.Kind)
            {
                case CXCursorKind.CXCursor_StructDecl:
                {
                    return Handle(VisitStructDecl, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_EnumDecl:
                {
                    return Handle(VisitEnumDecl, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_FunctionDecl:
                {
                    return Handle(VisitFunctionDecl, cursor, parent, data);
                }

                case CXCursorKind.CXCursor_TypedefDecl:
                {
                    return Handle(VisitTypedefDecl, cursor, parent, data);
                }

                default:
                {
                    return Unhandled(cursor, parent);
                }
            }
        }

        protected virtual bool BeginHandle(CXCursor cursor, CXCursor parent)
        {
            return true;
        }

        protected virtual void EndHandle(CXCursor cursor, CXCursor parent)
        {
        }

        protected CXChildVisitResult Handle(CXCursorVisitor visitor, CXCursor cursor, CXCursor parent, CXClientData data)
        {
            if (BeginHandle(cursor, parent))
            {
                cursor.VisitChildren(visitor, data);
            }
            EndHandle(cursor, parent);

            return CXChildVisitResult.CXChildVisit_Continue;
        }

        protected CXChildVisitResult Unhandled(CXCursor cursor, CXCursor parent)
        {
            Debug.WriteLine($"Unhandled cursor kind: {cursor.KindSpelling} in {parent.KindSpelling}.");
            Debugger.Break();
            return CXChildVisitResult.CXChildVisit_Break;
        }
    }
}
