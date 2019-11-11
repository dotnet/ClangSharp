// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class Expr : ValueStmt
    {
        private readonly Lazy<Type> _type;

        private protected Expr(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
            _type = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Type));
        }

        public Type Type { get; }

        internal static new Expr Create(CXCursor handle)
        {
            Expr result;

            switch (handle.Kind)
            {
                case CXCursorKind.CXCursor_UnexposedExpr:
                {
                    result = new Expr(handle, CXCursorKind.CXCursor_UnexposedExpr);
                    break;
                }

                case CXCursorKind.CXCursor_DeclRefExpr:
                {
                    result = new DeclRefExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_MemberRefExpr:
                {
                    result = new MemberExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CallExpr:
                {
                    result = new CallExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_IntegerLiteral:
                {
                    result = new IntegerLiteral(handle);
                    break;
                }

                case CXCursorKind.CXCursor_FloatingLiteral:
                {
                    result = new FloatingLiteral(handle);
                    break;
                }

                case CXCursorKind.CXCursor_StringLiteral:
                {
                    result = new StringLiteral(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CharacterLiteral:
                {
                    result = new CharacterLiteral(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ParenExpr:
                {
                    result = new ParenExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_UnaryOperator:
                {
                    result = new UnaryOperator(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ArraySubscriptExpr:
                {
                    result = new ArraySubscriptExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_BinaryOperator:
                {
                    result = new BinaryOperator(handle, CXCursorKind.CXCursor_BinaryOperator);
                    break;
                }

                case CXCursorKind.CXCursor_CompoundAssignOperator:
                {
                    result = new CompoundAssignOperator(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ConditionalOperator:
                {
                    result = new ConditionalOperator(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CStyleCastExpr:
                {
                    result = new CStyleCastExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_InitListExpr:
                {
                    result = new InitListExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CXXStaticCastExpr:
                {
                    result = new CXXStaticCastExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CXXConstCastExpr:
                {
                    result = new CXXConstCastExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CXXFunctionalCastExpr:
                {
                    result = new CXXFunctionalCastExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CXXBoolLiteralExpr:
                {
                    result = new CXXBoolLiteralExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CXXNullPtrLiteralExpr:
                {
                    result = new CXXNullPtrLiteralExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CXXThisExpr:
                {
                    result = new CXXThisExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_UnaryExpr:
                {
                    result = new UnaryExprOrTypeTraitExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_PackExpansionExpr:
                {
                    result = new PackExpansionExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_SizeOfPackExpr:
                {
                    result = new SizeOfPackExpr(handle);
                    break;
                }

                default:
                {
                    Debug.WriteLine($"Unhandled expression kind: {handle.KindSpelling}.");
                    Debugger.Break();

                    result = new Expr(handle, handle.Kind);
                    break;
                }
            }

            return result;
        }
    }
}
