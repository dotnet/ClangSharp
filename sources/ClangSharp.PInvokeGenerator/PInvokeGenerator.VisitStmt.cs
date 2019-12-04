// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public partial class PInvokeGenerator
    {
        private void VisitArraySubscriptExpr(ArraySubscriptExpr arraySubscriptExpr)
        {
            Visit(arraySubscriptExpr.Base);
            _outputBuilder.Write('[');
            Visit(arraySubscriptExpr.Idx);
            _outputBuilder.Write(']');
        }

        private void VisitBinaryOperator(BinaryOperator binaryOperator)
        {
            Visit(binaryOperator.LHS);
            _outputBuilder.Write(' ');
            _outputBuilder.Write(binaryOperator.OpcodeStr);
            _outputBuilder.Write(' ');
            Visit(binaryOperator.RHS);
        }

        private void VisitCallExpr(CallExpr callExpr)
        {
            var calleeDecl = callExpr.CalleeDecl;

            if (calleeDecl is FunctionDecl functionDecl)
            {
                _outputBuilder.WriteIndentation();
                VisitStmt(callExpr.Callee);
                _outputBuilder.Write('(');

                var args = callExpr.Args;

                if (args.Count != 0)
                {
                    Visit(args[0]);

                    for (int i = 1; i < args.Count; i++)
                    {
                        _outputBuilder.Write(',');
                        _outputBuilder.Write(' ');
                        Visit(args[i]);
                    }
                }

                _outputBuilder.WriteLine(");");
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported callee declaration: '{calleeDecl.Kind}'. Generated bindings may be incomplete.", calleeDecl);
            }
        }

        private void VisitCharacterLiteral(CharacterLiteral characterLiteral)
        {
            _outputBuilder.Write(characterLiteral.Value);
        }

        private void VisitCompoundStmt(CompoundStmt compoundStmt)
        {
            _outputBuilder.WriteBlockStart();
            VisitStmts(compoundStmt.Body);
            _outputBuilder.WriteBlockEnd();
        }

        private void VisitConditionalOperator(ConditionalOperator conditionalOperator)
        {
            Visit(conditionalOperator.Cond);
            _outputBuilder.Write(' ');
            _outputBuilder.Write('?');
            _outputBuilder.Write(' ');
            Visit(conditionalOperator.TrueExpr);
            _outputBuilder.Write(' ');
            _outputBuilder.Write(':');
            _outputBuilder.Write(' ');
            Visit(conditionalOperator.FalseExpr);
        }

        private void VisitCXXBoolLiteralExpr(CXXBoolLiteralExpr cxxBoolLiteralExpr)
        {
            _outputBuilder.Write(cxxBoolLiteralExpr.Value);
        }

        private void VisitCXXNullPtrLiteralExpr(CXXNullPtrLiteralExpr cxxNullPtrLiteralExpr)
        {
            _outputBuilder.Write("null");
        }

        private void VisitDeclRefExpr(DeclRefExpr declRefExpr)
        {
            var name = GetRemappedCursorName(declRefExpr.Decl);
            _outputBuilder.Write(EscapeAndStripName(name));
        }

        private void VisitExplicitCastExpr(ExplicitCastExpr explicitCastExpr)
        {
            if (!(explicitCastExpr is CXXConstCastExpr))
            {
                // C# doesn't have a concept of const pointers so
                // ignore rather than adding a cast from T* to T*

                var type = explicitCastExpr.Type;
                var typeName = GetRemappedTypeName(explicitCastExpr, type, out var nativeTypeName);

                _outputBuilder.Write('(');
                _outputBuilder.Write(typeName);
                _outputBuilder.Write(')');
            }
            Visit(explicitCastExpr.SubExpr);
        }

        private void VisitFloatingLiteral(FloatingLiteral floatingLiteral)
        {
            _outputBuilder.Write(floatingLiteral.Value);
        }

        private void VisitImplicitCastExpr(ImplicitCastExpr implicitCastExpr)
        {
            if ((implicitCastExpr.Type is PointerType) && (implicitCastExpr.SubExpr is IntegerLiteral integerLiteral) && (integerLiteral.Value.Equals("0")))
            {
                // C# doesn't have implicit conversion from zero to a pointer
                // so we will manually check and handle the most common case

                _outputBuilder.Write("null");
            }
            else
            {
                Visit(implicitCastExpr.SubExpr);
            }
        }

        private void VisitIntegerLiteral(IntegerLiteral integerLiteral)
        {
            _outputBuilder.Write(integerLiteral.Value);
        }

        private void VisitParenExpr(ParenExpr parenExpr)
        {
            _outputBuilder.Write('(');
            Visit(parenExpr.SubExpr);
            _outputBuilder.Write(')');
        }

        private void VisitReturnStmt(ReturnStmt returnStmt)
        {
            Debug.Assert(returnStmt.RetValue != null);

            _outputBuilder.WriteIndented("return");
            _outputBuilder.Write(' ');

            Visit(returnStmt.RetValue);
            _outputBuilder.WriteLine(';');
        }

        private void VisitStmt(Stmt stmt)
        {
            switch (stmt.StmtClass)
            {
                // case CX_StmtClass.CX_StmtClass_GCCAsmStmt:
                // case CX_StmtClass.CX_StmtClass_MSAsmStmt:
                // case CX_StmtClass.CX_StmtClass_BreakStmt:
                // case CX_StmtClass.CX_StmtClass_CXXCatchStmt:
                // case CX_StmtClass.CX_StmtClass_CXXForRangeStmt:
                // case CX_StmtClass.CX_StmtClass_CXXTryStmt:
                // case CX_StmtClass.CX_StmtClass_CapturedStmt:

                case CX_StmtClass.CX_StmtClass_CompoundStmt:
                {
                    VisitCompoundStmt((CompoundStmt)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_ContinueStmt:
                // case CX_StmtClass.CX_StmtClass_CoreturnStmt:
                // case CX_StmtClass.CX_StmtClass_CoroutineBodyStmt:
                // case CX_StmtClass.CX_StmtClass_DeclStmt:
                // case CX_StmtClass.CX_StmtClass_DoStmt:
                // case CX_StmtClass.CX_StmtClass_ForStmt:
                // case CX_StmtClass.CX_StmtClass_GotoStmt:
                // case CX_StmtClass.CX_StmtClass_IfStmt:
                // case CX_StmtClass.CX_StmtClass_IndirectGotoStmt:
                // case CX_StmtClass.CX_StmtClass_MSDependentExistsStmt:
                // case CX_StmtClass.CX_StmtClass_NullStmt:
                // case CX_StmtClass.CX_StmtClass_OMPAtomicDirective:
                // case CX_StmtClass.CX_StmtClass_OMPBarrierDirective:
                // case CX_StmtClass.CX_StmtClass_OMPCancelDirective:
                // case CX_StmtClass.CX_StmtClass_OMPCancellationPointDirective:
                // case CX_StmtClass.CX_StmtClass_OMPCriticalDirective:
                // case CX_StmtClass.CX_StmtClass_OMPFlushDirective:
                // case CX_StmtClass.CX_StmtClass_OMPDistributeDirective:
                // case CX_StmtClass.CX_StmtClass_OMPDistributeParallelForDirective:
                // case CX_StmtClass.CX_StmtClass_OMPDistributeParallelForSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPDistributeSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPForDirective:
                // case CX_StmtClass.CX_StmtClass_OMPForSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPParallelForDirective:
                // case CX_StmtClass.CX_StmtClass_OMPParallelForSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetParallelForSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetTeamsDistributeDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetTeamsDistributeParallelForDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetTeamsDistributeParallelForSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetTeamsDistributeSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTaskLoopDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTaskLoopSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTeamsDistributeDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTeamsDistributeParallelForDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTeamsDistributeParallelForSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTeamsDistributeSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPMasterDirective:
                // case CX_StmtClass.CX_StmtClass_OMPOrderedDirective:
                // case CX_StmtClass.CX_StmtClass_OMPParallelDirective:
                // case CX_StmtClass.CX_StmtClass_OMPParallelSectionsDirective:
                // case CX_StmtClass.CX_StmtClass_OMPSectionDirective:
                // case CX_StmtClass.CX_StmtClass_OMPSectionsDirective:
                // case CX_StmtClass.CX_StmtClass_OMPSingleDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetDataDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetEnterDataDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetExitDataDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetParallelDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetParallelForDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetTeamsDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetUpdateDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTaskDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTaskgroupDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTaskwaitDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTaskyieldDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTeamsDirective:
                // case CX_StmtClass.CX_StmtClass_ObjCAtCatchStmt:
                // case CX_StmtClass.CX_StmtClass_ObjCAtFinallyStmt:
                // case CX_StmtClass.CX_StmtClass_ObjCAtSynchronizedStmt:
                // case CX_StmtClass.CX_StmtClass_ObjCAtThrowStmt:
                // case CX_StmtClass.CX_StmtClass_ObjCAtTryStmt:
                // case CX_StmtClass.CX_StmtClass_ObjCAutoreleasePoolStmt:
                // case CX_StmtClass.CX_StmtClass_ObjCForCollectionStmt:

                case CX_StmtClass.CX_StmtClass_ReturnStmt:
                {
                    VisitReturnStmt((ReturnStmt)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_SEHExceptStmt:
                // case CX_StmtClass.CX_StmtClass_SEHFinallyStmt:
                // case CX_StmtClass.CX_StmtClass_SEHLeaveStmt:
                // case CX_StmtClass.CX_StmtClass_SEHTryStmt:
                // case CX_StmtClass.CX_StmtClass_CaseStmt:
                // case CX_StmtClass.CX_StmtClass_DefaultStmt:
                // case CX_StmtClass.CX_StmtClass_SwitchStmt:
                // case CX_StmtClass.CX_StmtClass_AttributedStmt:
                // case CX_StmtClass.CX_StmtClass_BinaryConditionalOperator:

                case CX_StmtClass.CX_StmtClass_ConditionalOperator:
                {
                    VisitConditionalOperator((ConditionalOperator)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_AddrLabelExpr:
                // case CX_StmtClass.CX_StmtClass_ArrayInitIndexExpr:
                // case CX_StmtClass.CX_StmtClass_ArrayInitLoopExpr:

                case CX_StmtClass.CX_StmtClass_ArraySubscriptExpr:
                {
                    VisitArraySubscriptExpr((ArraySubscriptExpr)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_ArrayTypeTraitExpr:
                // case CX_StmtClass.CX_StmtClass_AsTypeExpr:
                // case CX_StmtClass.CX_StmtClass_AtomicExpr:

                case CX_StmtClass.CX_StmtClass_BinaryOperator:
                case CX_StmtClass.CX_StmtClass_CompoundAssignOperator:
                {
                    VisitBinaryOperator((BinaryOperator)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_BlockExpr:
                // case CX_StmtClass.CX_StmtClass_CXXBindTemporaryExpr:

                case CX_StmtClass.CX_StmtClass_CXXBoolLiteralExpr:
                {
                    VisitCXXBoolLiteralExpr((CXXBoolLiteralExpr)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_CXXConstructExpr:
                // case CX_StmtClass.CX_StmtClass_CXXTemporaryObjectExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDefaultArgExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDefaultInitExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDeleteExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDependentScopeMemberExpr:
                // case CX_StmtClass.CX_StmtClass_CXXFoldExpr:
                // case CX_StmtClass.CX_StmtClass_CXXInheritedCtorInitExpr:
                // case CX_StmtClass.CX_StmtClass_CXXNewExpr:
                // case CX_StmtClass.CX_StmtClass_CXXNoexceptExpr:

                case CX_StmtClass.CX_StmtClass_CXXNullPtrLiteralExpr:
                {
                    VisitCXXNullPtrLiteralExpr((CXXNullPtrLiteralExpr)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_CXXPseudoDestructorExpr:
                // case CX_StmtClass.CX_StmtClass_CXXScalarValueInitExpr:
                // case CX_StmtClass.CX_StmtClass_CXXStdInitializerListExpr:
                // case CX_StmtClass.CX_StmtClass_CXXThisExpr:
                // case CX_StmtClass.CX_StmtClass_CXXThrowExpr:
                // case CX_StmtClass.CX_StmtClass_CXXTypeidExpr:
                // case CX_StmtClass.CX_StmtClass_CXXUnresolvedConstructExpr:
                // case CX_StmtClass.CX_StmtClass_CXXUuidofExpr:

                case CX_StmtClass.CX_StmtClass_CallExpr:
                {
                    VisitCallExpr((CallExpr)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_CUDAKernelCallExpr:
                // case CX_StmtClass.CX_StmtClass_CXXMemberCallExpr:
                // case CX_StmtClass.CX_StmtClass_CXXOperatorCallExpr:
                // case CX_StmtClass.CX_StmtClass_UserDefinedLiteral:
                // case CX_StmtClass.CX_StmtClass_BuiltinBitCastExpr:

                case CX_StmtClass.CX_StmtClass_CStyleCastExpr:
                case CX_StmtClass.CX_StmtClass_CXXFunctionalCastExpr:
                case CX_StmtClass.CX_StmtClass_CXXConstCastExpr:
                case CX_StmtClass.CX_StmtClass_CXXDynamicCastExpr:
                case CX_StmtClass.CX_StmtClass_CXXReinterpretCastExpr:
                case CX_StmtClass.CX_StmtClass_CXXStaticCastExpr:
                {
                    VisitExplicitCastExpr((ExplicitCastExpr)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_ObjCBridgedCastExpr:

                case CX_StmtClass.CX_StmtClass_ImplicitCastExpr:
                {
                    VisitImplicitCastExpr((ImplicitCastExpr)stmt);
                    break;
                }

                case CX_StmtClass.CX_StmtClass_CharacterLiteral:
                {
                    VisitCharacterLiteral((CharacterLiteral)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_ChooseExpr:
                // case CX_StmtClass.CX_StmtClass_CompoundLiteralExpr:
                // case CX_StmtClass.CX_StmtClass_ConvertVectorExpr:
                // case CX_StmtClass.CX_StmtClass_CoawaitExpr:
                // case CX_StmtClass.CX_StmtClass_CoyieldExpr:

                case CX_StmtClass.CX_StmtClass_DeclRefExpr:
                {
                    VisitDeclRefExpr((DeclRefExpr)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_DependentCoawaitExpr:
                // case CX_StmtClass.CX_StmtClass_DependentScopeDeclRefExpr:
                // case CX_StmtClass.CX_StmtClass_DesignatedInitExpr:
                // case CX_StmtClass.CX_StmtClass_DesignatedInitUpdateExpr:
                // case CX_StmtClass.CX_StmtClass_ExpressionTraitExpr:
                // case CX_StmtClass.CX_StmtClass_ExtVectorElementExpr:
                // case CX_StmtClass.CX_StmtClass_FixedPointLiteral:

                case CX_StmtClass.CX_StmtClass_FloatingLiteral:
                {
                    VisitFloatingLiteral((FloatingLiteral)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_ConstantExpr:
                // case CX_StmtClass.CX_StmtClass_ExprWithCleanups:
                // case CX_StmtClass.CX_StmtClass_FunctionParmPackExpr:
                // case CX_StmtClass.CX_StmtClass_GNUNullExpr:
                // case CX_StmtClass.CX_StmtClass_GenericSelectionExpr:
                // case CX_StmtClass.CX_StmtClass_ImaginaryLiteral:
                // case CX_StmtClass.CX_StmtClass_ImplicitValueInitExpr:
                // case CX_StmtClass.CX_StmtClass_InitListExpr:

                case CX_StmtClass.CX_StmtClass_IntegerLiteral:
                {
                    VisitIntegerLiteral((IntegerLiteral)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_LambdaExpr:
                // case CX_StmtClass.CX_StmtClass_MSPropertyRefExpr:
                // case CX_StmtClass.CX_StmtClass_MSPropertySubscriptExpr:
                // case CX_StmtClass.CX_StmtClass_MaterializeTemporaryExpr:
                // case CX_StmtClass.CX_StmtClass_MemberExpr:
                // case CX_StmtClass.CX_StmtClass_NoInitExpr:
                // case CX_StmtClass.CX_StmtClass_OMPArraySectionExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCArrayLiteral:
                // case CX_StmtClass.CX_StmtClass_ObjCAvailabilityCheckExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCBoolLiteralExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCBoxedExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCDictionaryLiteral:
                // case CX_StmtClass.CX_StmtClass_ObjCEncodeExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCIndirectCopyRestoreExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCIsaExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCIvarRefExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCMessageExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCPropertyRefExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCProtocolExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCSelectorExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCStringLiteral:
                // case CX_StmtClass.CX_StmtClass_ObjCSubscriptRefExpr:
                // case CX_StmtClass.CX_StmtClass_OffsetOfExpr:
                // case CX_StmtClass.CX_StmtClass_OpaqueValueExpr:
                // case CX_StmtClass.CX_StmtClass_UnresolvedLookupExpr:
                // case CX_StmtClass.CX_StmtClass_UnresolvedMemberExpr:
                // case CX_StmtClass.CX_StmtClass_PackExpansionExpr:

                case CX_StmtClass.CX_StmtClass_ParenExpr:
                {
                    VisitParenExpr((ParenExpr)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_ParenListExpr:
                // case CX_StmtClass.CX_StmtClass_PredefinedExpr:
                // case CX_StmtClass.CX_StmtClass_PseudoObjectExpr:
                // case CX_StmtClass.CX_StmtClass_ShuffleVectorExpr:
                // case CX_StmtClass.CX_StmtClass_SizeOfPackExpr:
                // case CX_StmtClass.CX_StmtClass_SourceLocExpr:
                // case CX_StmtClass.CX_StmtClass_StmtExpr:
                // case CX_StmtClass.CX_StmtClass_StringLiteral:
                // case CX_StmtClass.CX_StmtClass_SubstNonTypeTemplateParmExpr:
                // case CX_StmtClass.CX_StmtClass_SubstNonTypeTemplateParmPackExpr:
                // case CX_StmtClass.CX_StmtClass_TypeTraitExpr:
                // case CX_StmtClass.CX_StmtClass_TypoExpr:
                // case CX_StmtClass.CX_StmtClass_UnaryExprOrTypeTraitExpr:

                case CX_StmtClass.CX_StmtClass_UnaryOperator:
                {
                    VisitUnaryOperator((UnaryOperator)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_VAArgExpr:
                // case CX_StmtClass.CX_StmtClass_LabelStmt:
                // case CX_StmtClass.CX_StmtClass_WhileStmt:

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported statement: '{stmt.StmtClass}'. Generated bindings may be incomplete.", stmt);
                    break;
                }
            }
        }

        private void VisitStmts(IEnumerable<Stmt> stmts)
        {
            foreach (var stmt in stmts)
            {
                Visit(stmt);
            }
        }

        private void VisitUnaryOperator(UnaryOperator unaryOperator)
        {
            switch (unaryOperator.Opcode)
            {
                case CX_UnaryOperatorKind.CX_UO_PostInc:
                case CX_UnaryOperatorKind.CX_UO_PostDec:
                {
                    Visit(unaryOperator.SubExpr);
                    _outputBuilder.Write(unaryOperator.OpcodeStr);
                    break;
                }

                case CX_UnaryOperatorKind.CX_UO_PreInc:
                case CX_UnaryOperatorKind.CX_UO_PreDec:
                case CX_UnaryOperatorKind.CX_UO_AddrOf:
                case CX_UnaryOperatorKind.CX_UO_Deref:
                case CX_UnaryOperatorKind.CX_UO_Plus:
                case CX_UnaryOperatorKind.CX_UO_Minus:
                case CX_UnaryOperatorKind.CX_UO_Not:
                case CX_UnaryOperatorKind.CX_UO_LNot:
                {
                    _outputBuilder.Write(unaryOperator.OpcodeStr);
                    Visit(unaryOperator.SubExpr);
                    break;
                }

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported unary operator opcode: '{unaryOperator.OpcodeStr}'. Generated bindings may be incomplete.", unaryOperator);
                    break;
                }
            }
        }
    }
}
