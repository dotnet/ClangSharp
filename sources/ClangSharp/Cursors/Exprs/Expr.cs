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
                    result = new Expr(handle, handle.Kind);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCSelfExpr:
                case CXCursorKind.CXCursor_DeclRefExpr:
                {
                    result = new DeclRefExpr(handle, handle.Kind);
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

                case CXCursorKind.CXCursor_ObjCMessageExpr:
                {
                    result = new ObjCMessageExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_BlockExpr:
                {
                    result = new BlockExpr(handle);
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

                case CXCursorKind.CXCursor_ImaginaryLiteral:
                {
                    result = new ImaginaryLiteral(handle);
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
                    result = new BinaryOperator(handle);
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

                case CXCursorKind.CXCursor_CompoundLiteralExpr:
                {
                    result = new CompoundLiteralExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_InitListExpr:
                {
                    result = new InitListExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_AddrLabelExpr:
                {
                    result = new AddrLabelExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_StmtExpr:
                {
                    result = new StmtExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_GenericSelectionExpr:
                {
                    result = new GenericSelectionExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_GNUNullExpr:
                {
                    result = new GNUNullExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CXXStaticCastExpr:
                {
                    result = new CXXStaticCastExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CXXDynamicCastExpr:
                {
                    result = new CXXDynamicCastExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CXXReinterpretCastExpr:
                {
                    result = new CXXReinterpretCastExpr(handle);
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

                case CXCursorKind.CXCursor_CXXTypeidExpr:
                {
                    result = new CXXTypeidExpr(handle);
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

                case CXCursorKind.CXCursor_CXXThrowExpr:
                {
                    result = new CXXThrowExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CXXNewExpr:
                {
                    result = new CXXNewExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CXXDeleteExpr:
                {
                    result = new CXXDeleteExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_UnaryExpr:
                {
                    result = new UnaryExprOrTypeTraitExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCStringLiteral:
                {
                    result = new ObjCStringLiteral(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCEncodeExpr:
                {
                    result = new ObjCEncodeExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCSelectorExpr:
                {
                    result = new ObjCSelectorExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCProtocolExpr:
                {
                    result = new ObjCProtocolExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCBridgedCastExpr:
                {
                    result = new ObjCBridgedCastExpr(handle);
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

                case CXCursorKind.CXCursor_LambdaExpr:
                {
                    result = new LambdaExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCBoolLiteralExpr:
                {
                    result = new ObjCBoolLiteralExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPArraySectionExpr:
                {
                    result = new OMPArraySectionExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCAvailabilityCheckExpr:
                {
                    result = new ObjCAvailabilityCheckExpr(handle);
                    break;
                }

                case CXCursorKind.CXCursor_FixedPointLiteral:
                {
                    result = new FixedPointLiteral(handle);
                    break;
                }

                default:
                {
                    Debug.WriteLine($"Unhandled expression kind: {handle.KindSpelling}.");
                    result = new Expr(handle, handle.Kind);
                    break;
                }
            }

            return result;
        }
    }
}
