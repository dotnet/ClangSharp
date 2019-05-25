using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    internal class Cursor
    {
        public static Cursor Create(CXCursor handle, Cursor parent)
        {
            Debug.Assert(!handle.IsNull);

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

                case CXCursorKind.CXCursor_TypeRef:
                {
                    return new TypeRef(handle, parent);
                }

                case CXCursorKind.CXCursor_CXXBaseSpecifier:
                {
                    return new CXXBaseSpecifier(handle, parent);
                }

                case CXCursorKind.CXCursor_TemplateRef:
                {
                    return new TemplateRef(handle, parent);
                }

                case CXCursorKind.CXCursor_NamespaceRef:
                {
                    return new NamespaceRef(handle, parent);
                }

                case CXCursorKind.CXCursor_MemberRef:
                {
                    return new MemberRef(handle, parent);
                }

                case CXCursorKind.CXCursor_OverloadedDeclRef:
                {
                    return new OverloadedDeclRef(handle, parent);
                }

                case CXCursorKind.CXCursor_UnexposedExpr:
                {
                    return new UnexposedExpr(handle, parent);
                }

                case CXCursorKind.CXCursor_DeclRefExpr:
                {
                    return new DeclRefExpr(handle, parent);
                }

                case CXCursorKind.CXCursor_MemberRefExpr:
                {
                    return new MemberRefExpr(handle, parent);
                }

                case CXCursorKind.CXCursor_CallExpr:
                {
                    return new CallExpr(handle, parent);
                }

                case CXCursorKind.CXCursor_IntegerLiteral:
                {
                    return new IntegerLiteral(handle, parent);
                }

                case CXCursorKind.CXCursor_FloatingLiteral:
                {
                    return new FloatingLiteral(handle, parent);
                }

                case CXCursorKind.CXCursor_StringLiteral:
                {
                    return new StringLiteral(handle, parent);
                }

                case CXCursorKind.CXCursor_CharacterLiteral:
                {
                    return new CharacterLiteral(handle, parent);
                }

                case CXCursorKind.CXCursor_ParenExpr:
                {
                    return new ParenExpr(handle, parent);
                }

                case CXCursorKind.CXCursor_UnaryOperator:
                {
                    return new UnaryOperator(handle, parent);
                }

                case CXCursorKind.CXCursor_ArraySubscriptExpr:
                {
                    return new ArraySubscriptExpr(handle, parent);
                }

                case CXCursorKind.CXCursor_BinaryOperator:
                {
                    return new BinaryOperator(handle, parent);
                }

                case CXCursorKind.CXCursor_ConditionalOperator:
                {
                    return new ConditionalOperator(handle, parent);
                }

                case CXCursorKind.CXCursor_CStyleCastExpr:
                {
                    return new CStyleCastExpr(handle, parent);
                }

                case CXCursorKind.CXCursor_InitListExpr:
                {
                    return new InitListExpr(handle, parent);
                }

                case CXCursorKind.CXCursor_CXXStaticCastExpr:
                {
                    return new CXXStaticCastExpr(handle, parent);
                }

                case CXCursorKind.CXCursor_CXXConstCastExpr:
                {
                    return new CXXConstCastExpr(handle, parent);
                }

                case CXCursorKind.CXCursor_CXXFunctionalCastExpr:
                {
                    return new CXXFunctionalCastExpr(handle, parent);
                }

                case CXCursorKind.CXCursor_CXXBoolLiteralExpr:
                {
                    return new CXXBoolLiteralExpr(handle, parent);
                }

                case CXCursorKind.CXCursor_CXXNullPtrLiteralExpr:
                {
                    return new CXXNullPtrLiteralExpr(handle, parent);
                }

                case CXCursorKind.CXCursor_CXXThisExpr:
                {
                    return new CXXThisExpr(handle, parent);
                }

                case CXCursorKind.CXCursor_UnaryExpr:
                {
                    return new UnaryExpr(handle, parent);
                }

                case CXCursorKind.CXCursor_PackExpansionExpr:
                {
                    return new PackExpansionExpr(handle, parent);
                }

                case CXCursorKind.CXCursor_SizeOfPackExpr:
                {
                    return new SizeOfPackExpr(handle, parent);
                }

                case CXCursorKind.CXCursor_CompoundStmt:
                {
                    return new CompoundStmt(handle, parent);
                }

                case CXCursorKind.CXCursor_IfStmt:
                {
                    return new IfStmt(handle, parent);
                }

                case CXCursorKind.CXCursor_DoStmt:
                {
                    return new DoStmt(handle, parent);
                }

                case CXCursorKind.CXCursor_ForStmt:
                {
                    return new ForStmt(handle, parent);
                }

                case CXCursorKind.CXCursor_BreakStmt:
                {
                    return new BreakStmt(handle, parent);
                }

                case CXCursorKind.CXCursor_ReturnStmt:
                {
                    return new ReturnStmt(handle, parent);
                }

                case CXCursorKind.CXCursor_NullStmt:
                {
                    return new NullStmt(handle, parent);
                }

                case CXCursorKind.CXCursor_DeclStmt:
                {
                    return new DeclStmt(handle, parent);
                }

                case CXCursorKind.CXCursor_UnexposedAttr:
                {
                    return new UnexposedAttr(handle, parent);
                }

                case CXCursorKind.CXCursor_CXXFinalAttr:
                {
                    return new CXXFinalAttr(handle, parent);
                }

                case CXCursorKind.CXCursor_PureAttr:
                {
                    return new PureAttr(handle, parent);
                }

                case CXCursorKind.CXCursor_ConstAttr:
                {
                    return new ConstAttr(handle, parent);
                }

                case CXCursorKind.CXCursor_VisibilityAttr:
                {
                    return new VisibilityAttr(handle, parent);
                }

                case CXCursorKind.CXCursor_DLLExport:
                {
                    return new DLLExport(handle, parent);
                }

                case CXCursorKind.CXCursor_DLLImport:
                {
                    return new DLLImport(handle, parent);
                }

                case CXCursorKind.CXCursor_StaticAssert:
                {
                    return new DLLImport(handle, parent);
                }

                case CXCursorKind.CXCursor_TypeAliasTemplateDecl:
                {
                    return new TypeAliasTemplateDecl(handle, parent);
                }

                case CXCursorKind.CXCursor_FriendDecl:
                {
                    return new FriendDecl(handle, parent);
                }

                default:
                {
                    Debug.WriteLine($"Unhandled cursor kind: {handle.KindSpelling}.");
                    Debugger.Break();
                    return null;
                }
            }
        }

        private readonly List<Cursor> _children;
        private readonly Lazy<Cursor> _canonicalCursor;
        private bool _visited;

        protected Cursor(CXCursor handle, Cursor parent)
        {
            _children = new List<Cursor>();

            Handle = handle;
            Parent = parent;

            if (parent is null)
            {
                Debug.Assert(this is TranslationUnit);
                TranslationUnit = this as TranslationUnit;
            }
            else
            {
                Debug.Assert(parent.TranslationUnit != null);
                TranslationUnit = parent.TranslationUnit;
            }
            TranslationUnit.AddVisitedCursor(this);

            _canonicalCursor = new Lazy<Cursor>(() => {
                var cursor = TranslationUnit.GetOrCreateCursor(handle.CanonicalCursor, () => Create(handle.CanonicalCursor, this));
                cursor.Visit(clientData: default);
                return cursor;
            });
        }

        public Cursor CanonicalCursor => _canonicalCursor.Value;

        public IReadOnlyList<Cursor> Children => _children;

        public CXSourceRange Extent => Handle.Extent;

        public CXCursor Handle { get; }

        public CXCursorKind Kind => Handle.Kind;

        public string KindSpelling => Handle.KindSpelling.ToString();

        public CXSourceLocation Location => Handle.Location;

        public Cursor Parent { get; }

        public string Spelling => Handle.Spelling.ToString();

        public TranslationUnit TranslationUnit { get; }

        public CXToken[] Tokenize(Cursor cursor)
        {
            Handle.TranslationUnit.Tokenize(cursor.Extent, out CXToken[] tokens);
            return tokens;
        }

        public CXChildVisitResult Visit(CXClientData clientData)
        {
            if (!_visited)
            {
                _visited = true;
                Handle.VisitChildren(VisitChildren, clientData);
            }

            // We always return CXChildVisit_Continue since some clang will return
            // CXChildVisit_Break for some calls, such as if they have no children.

            return CXChildVisitResult.CXChildVisit_Continue;
        }

        protected TCursor GetOrAddChild<TCursor>(CXCursor childHandle)
            where TCursor : Cursor
        {
            var childCursor = TranslationUnit.GetOrCreateCursor(childHandle, () => Create(childHandle, this));
            _children.Add(childCursor);
            return (TCursor)childCursor;
        }

        protected virtual CXChildVisitResult VisitChildren(CXCursor childHandle, CXCursor handle, CXClientData clientData)
        {
            ValidateVisit(ref handle);

            switch (childHandle.Kind)
            {
                default:
                {
                    Debug.WriteLine($"Unhandled cursor kind: {childHandle.KindSpelling} in {Handle.KindSpelling}.");
                    Debugger.Break();
                    return CXChildVisitResult.CXChildVisit_Break;
                }
            }
        }

        protected virtual void ValidateVisit(ref CXCursor handle)
        {
            Debug.Assert(handle.Equals(Handle));
        }
    }
}
