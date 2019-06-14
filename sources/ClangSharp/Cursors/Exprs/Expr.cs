using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class Expr : ValueStmt
    {
        public static new Expr Create(CXCursor handle, Cursor parent)
        {
            switch (handle.Kind)
            {
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
                    return new MemberExpr(handle, parent);
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

                case CXCursorKind.CXCursor_CompoundAssignOperator:
                {
                    return new CompoundAssignOperator(handle, parent);
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
                    return new UnaryExprOrTypeTraitExpr(handle, parent);
                }

                case CXCursorKind.CXCursor_PackExpansionExpr:
                {
                    return new PackExpansionExpr(handle, parent);
                }

                case CXCursorKind.CXCursor_SizeOfPackExpr:
                {
                    return new SizeOfPackExpr(handle, parent);
                }

                default:
                {
                    Debug.WriteLine($"Unhandled expression kind: {handle.KindSpelling}.");
                    Debugger.Break();
                    return new Expr(handle, parent);
                }
            }
        }

        private readonly Lazy<Cursor> _definition;

        protected Expr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.IsExpression);

            _definition = new Lazy<Cursor>(() => {
                var cursor = TranslationUnit.GetOrCreateCursor(Handle.Definition, () => Create(Handle.Definition, this));
                cursor?.Visit(clientData: default);
                return cursor;
            });

            Type = TranslationUnit.GetOrCreateType(Handle.Type, () => Type.Create(Handle.Type, TranslationUnit));
        }

        public Cursor Definition => _definition.Value;

        public bool IsDynamicCall => Handle.IsDynamicCall;

        public Type Type { get; }

        protected unsafe override void ValidateVisit(ref CXCursor handle)
        {
            // Clang currently uses the PostChildrenVisitor which clears data0

            var modifiedHandle = Handle;
            modifiedHandle.data[0] = null;

            Debug.Assert(handle.Equals(modifiedHandle));
            handle = Handle;
        }
    }
}
