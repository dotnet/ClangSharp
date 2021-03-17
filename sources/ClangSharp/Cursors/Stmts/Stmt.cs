// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class Stmt : Cursor
    {
        private readonly Lazy<IReadOnlyList<Stmt>> _children;
        private readonly Lazy<IDeclContext> _declContext;

        private protected Stmt(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind)
        {
            if ((handle.StmtClass == CX_StmtClass.CX_StmtClass_Invalid) || (handle.StmtClass != expectedStmtClass))
            {
                throw new ArgumentException(nameof(handle));
            }

            _children = new Lazy<IReadOnlyList<Stmt>>(() => {
                var numChildren = Handle.NumChildren;
                var children = new List<Stmt>(numChildren);

                for (int i = 0; i < numChildren; i++)
                {
                    var child = TranslationUnit.GetOrCreate<Stmt>(Handle.GetChild(unchecked((uint)i)));
                    children.Add(child);
                }

                return children;
            });

            _declContext = new Lazy<IDeclContext>(() => {
                var semanticParent = TranslationUnit.GetOrCreate<Cursor>(Handle.SemanticParent);

                while (!(semanticParent is IDeclContext) && (semanticParent != null))
                {
                    semanticParent = TranslationUnit.GetOrCreate<Cursor>(semanticParent.Handle.SemanticParent);
                }

                return (IDeclContext)semanticParent;
            });
        }

        public IReadOnlyList<Stmt> Children => _children.Value;

        public IDeclContext DeclContext => _declContext.Value;

        public CX_StmtClass StmtClass => Handle.StmtClass;

        public string StmtClassName => Handle.StmtClassSpelling;

        internal static new Stmt Create(CXCursor handle) => handle.StmtClass switch
        {
            CX_StmtClass.CX_StmtClass_GCCAsmStmt => new GCCAsmStmt(handle),
            CX_StmtClass.CX_StmtClass_MSAsmStmt => new MSAsmStmt(handle),
            CX_StmtClass.CX_StmtClass_BreakStmt => new BreakStmt(handle),
            CX_StmtClass.CX_StmtClass_CXXCatchStmt => new CXXCatchStmt(handle),
            CX_StmtClass.CX_StmtClass_CXXForRangeStmt => new CXXForRangeStmt(handle),
            CX_StmtClass.CX_StmtClass_CXXTryStmt => new CXXTryStmt(handle),
            CX_StmtClass.CX_StmtClass_CapturedStmt => new CapturedStmt(handle),
            CX_StmtClass.CX_StmtClass_CompoundStmt => new CompoundStmt(handle),
            CX_StmtClass.CX_StmtClass_ContinueStmt => new ContinueStmt(handle),
            CX_StmtClass.CX_StmtClass_CoreturnStmt => new CoreturnStmt(handle),
            CX_StmtClass.CX_StmtClass_CoroutineBodyStmt => new CoroutineBodyStmt(handle),
            CX_StmtClass.CX_StmtClass_DeclStmt => new DeclStmt(handle),
            CX_StmtClass.CX_StmtClass_DoStmt => new DoStmt(handle),
            CX_StmtClass.CX_StmtClass_ForStmt => new ForStmt(handle),
            CX_StmtClass.CX_StmtClass_GotoStmt => new GotoStmt(handle),
            CX_StmtClass.CX_StmtClass_IfStmt => new IfStmt(handle),
            CX_StmtClass.CX_StmtClass_IndirectGotoStmt => new IndirectGotoStmt(handle),
            CX_StmtClass.CX_StmtClass_MSDependentExistsStmt => new MSDependentExistsStmt(handle),
            CX_StmtClass.CX_StmtClass_NullStmt => new NullStmt(handle),
            CX_StmtClass.CX_StmtClass_OMPAtomicDirective => new OMPAtomicDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPBarrierDirective => new OMPBarrierDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPCancelDirective => new OMPCancelDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPCancellationPointDirective => new OMPCancellationPointDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPCriticalDirective => new OMPCriticalDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPFlushDirective => new OMPFlushDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPDistributeDirective => new OMPDistributeDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPDistributeParallelForDirective => new OMPDistributeParallelForDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPDistributeParallelForSimdDirective => new OMPDistributeParallelForSimdDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPDistributeSimdDirective => new OMPDistributeSimdDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPForDirective => new OMPForDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPForSimdDirective => new OMPForSimdDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPParallelForDirective => new OMPParallelForDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPParallelForSimdDirective => new OMPParallelForSimdDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPSimdDirective => new OMPSimdDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTargetParallelForSimdDirective => new OMPTargetParallelForSimdDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTargetSimdDirective => new OMPTargetSimdDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTargetTeamsDistributeDirective => new OMPTargetTeamsDistributeDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTargetTeamsDistributeParallelForDirective => new OMPTargetTeamsDistributeParallelForDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTargetTeamsDistributeParallelForSimdDirective => new OMPTargetTeamsDistributeParallelForSimdDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTargetTeamsDistributeSimdDirective => new OMPTargetTeamsDistributeSimdDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTaskLoopDirective => new OMPTaskLoopDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTaskLoopSimdDirective => new OMPTaskLoopSimdDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTeamsDistributeDirective => new OMPTeamsDistributeDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTeamsDistributeParallelForDirective => new OMPTeamsDistributeParallelForDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTeamsDistributeParallelForSimdDirective => new OMPTeamsDistributeParallelForSimdDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTeamsDistributeSimdDirective => new OMPTeamsDistributeSimdDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPMasterDirective => new OMPMasterDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPOrderedDirective => new OMPOrderedDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPParallelDirective => new OMPParallelDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPParallelSectionsDirective => new OMPParallelSectionsDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPSectionDirective => new OMPSectionDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPSectionsDirective => new OMPSectionsDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPSingleDirective => new OMPSingleDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTargetDataDirective => new OMPTargetDataDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTargetDirective => new OMPTargetDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTargetEnterDataDirective => new OMPTargetEnterDataDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTargetExitDataDirective => new OMPTargetExitDataDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTargetParallelDirective => new OMPTargetParallelDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTargetParallelForDirective => new OMPTargetParallelForDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTargetTeamsDirective => new OMPTargetTeamsDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTargetUpdateDirective => new OMPTargetUpdateDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTaskDirective => new OMPTaskDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTaskgroupDirective => new OMPTaskgroupDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTaskwaitDirective => new OMPTaskwaitDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTaskyieldDirective => new OMPTaskyieldDirective(handle),
            CX_StmtClass.CX_StmtClass_OMPTeamsDirective => new OMPTeamsDirective(handle),
            CX_StmtClass.CX_StmtClass_ObjCAtCatchStmt => new ObjCAtCatchStmt(handle),
            CX_StmtClass.CX_StmtClass_ObjCAtFinallyStmt => new ObjCAtFinallyStmt(handle),
            CX_StmtClass.CX_StmtClass_ObjCAtSynchronizedStmt => new ObjCAtSynchronizedStmt(handle),
            CX_StmtClass.CX_StmtClass_ObjCAtThrowStmt => new ObjCAtThrowStmt(handle),
            CX_StmtClass.CX_StmtClass_ObjCAtTryStmt => new ObjCAtTryStmt(handle),
            CX_StmtClass.CX_StmtClass_ObjCAutoreleasePoolStmt => new ObjCAutoreleasePoolStmt(handle),
            CX_StmtClass.CX_StmtClass_ObjCForCollectionStmt => new ObjCForCollectionStmt(handle),
            CX_StmtClass.CX_StmtClass_ReturnStmt => new ReturnStmt(handle),
            CX_StmtClass.CX_StmtClass_SEHExceptStmt => new SEHExceptStmt(handle),
            CX_StmtClass.CX_StmtClass_SEHFinallyStmt => new SEHFinallyStmt(handle),
            CX_StmtClass.CX_StmtClass_SEHLeaveStmt => new SEHLeaveStmt(handle),
            CX_StmtClass.CX_StmtClass_SEHTryStmt => new SEHTryStmt(handle),
            CX_StmtClass.CX_StmtClass_CaseStmt => new CaseStmt(handle),
            CX_StmtClass.CX_StmtClass_DefaultStmt => new DefaultStmt(handle),
            CX_StmtClass.CX_StmtClass_SwitchStmt => new SwitchStmt(handle),
            CX_StmtClass.CX_StmtClass_AttributedStmt => new AttributedStmt(handle),
            CX_StmtClass.CX_StmtClass_BinaryConditionalOperator => new BinaryConditionalOperator(handle),
            CX_StmtClass.CX_StmtClass_ConditionalOperator => new ConditionalOperator(handle),
            CX_StmtClass.CX_StmtClass_AddrLabelExpr => new AddrLabelExpr(handle),
            CX_StmtClass.CX_StmtClass_ArrayInitIndexExpr => new ArrayInitIndexExpr(handle),
            CX_StmtClass.CX_StmtClass_ArrayInitLoopExpr => new ArrayInitLoopExpr(handle),
            CX_StmtClass.CX_StmtClass_ArraySubscriptExpr => new ArraySubscriptExpr(handle),
            CX_StmtClass.CX_StmtClass_ArrayTypeTraitExpr => new ArrayTypeTraitExpr(handle),
            CX_StmtClass.CX_StmtClass_AsTypeExpr => new AsTypeExpr(handle),
            CX_StmtClass.CX_StmtClass_AtomicExpr => new AtomicExpr(handle),
            CX_StmtClass.CX_StmtClass_BinaryOperator => new BinaryOperator(handle),
            CX_StmtClass.CX_StmtClass_CompoundAssignOperator => new CompoundAssignOperator(handle),
            CX_StmtClass.CX_StmtClass_BlockExpr => new BlockExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXBindTemporaryExpr => new CXXBindTemporaryExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXBoolLiteralExpr => new CXXBoolLiteralExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXConstructExpr => new CXXConstructExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXTemporaryObjectExpr => new CXXTemporaryObjectExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXDefaultArgExpr => new CXXDefaultArgExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXDefaultInitExpr => new CXXDefaultInitExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXDeleteExpr => new CXXDeleteExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXDependentScopeMemberExpr => new CXXDependentScopeMemberExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXFoldExpr => new CXXFoldExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXInheritedCtorInitExpr => new CXXInheritedCtorInitExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXNewExpr => new CXXNewExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXNoexceptExpr => new CXXNoexceptExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXNullPtrLiteralExpr => new CXXNullPtrLiteralExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXPseudoDestructorExpr => new CXXPseudoDestructorExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXScalarValueInitExpr => new CXXScalarValueInitExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXStdInitializerListExpr => new CXXStdInitializerListExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXThisExpr => new CXXThisExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXThrowExpr => new CXXThrowExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXTypeidExpr => new CXXTypeidExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXUnresolvedConstructExpr => new CXXUnresolvedConstructExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXUuidofExpr => new CXXUuidofExpr(handle),
            CX_StmtClass.CX_StmtClass_CallExpr => new CallExpr(handle),
            CX_StmtClass.CX_StmtClass_CUDAKernelCallExpr => new CUDAKernelCallExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXMemberCallExpr => new CXXMemberCallExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXOperatorCallExpr => new CXXOperatorCallExpr(handle),
            CX_StmtClass.CX_StmtClass_UserDefinedLiteral => new UserDefinedLiteral(handle),
            CX_StmtClass.CX_StmtClass_BuiltinBitCastExpr => new BuiltinBitCastExpr(handle),
            CX_StmtClass.CX_StmtClass_CStyleCastExpr => new CStyleCastExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXFunctionalCastExpr => new CXXFunctionalCastExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXConstCastExpr => new CXXConstCastExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXDynamicCastExpr => new CXXDynamicCastExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXReinterpretCastExpr => new CXXReinterpretCastExpr(handle),
            CX_StmtClass.CX_StmtClass_CXXStaticCastExpr => new CXXStaticCastExpr(handle),
            CX_StmtClass.CX_StmtClass_ObjCBridgedCastExpr => new ObjCBridgedCastExpr(handle),
            CX_StmtClass.CX_StmtClass_ImplicitCastExpr => new ImplicitCastExpr(handle),
            CX_StmtClass.CX_StmtClass_CharacterLiteral => new CharacterLiteral(handle),
            CX_StmtClass.CX_StmtClass_ChooseExpr => new ChooseExpr(handle),
            CX_StmtClass.CX_StmtClass_CompoundLiteralExpr => new CompoundLiteralExpr(handle),
            CX_StmtClass.CX_StmtClass_ConvertVectorExpr => new ConvertVectorExpr(handle),
            CX_StmtClass.CX_StmtClass_CoawaitExpr => new CoawaitExpr(handle),
            CX_StmtClass.CX_StmtClass_CoyieldExpr => new CoyieldExpr(handle),
            CX_StmtClass.CX_StmtClass_DeclRefExpr => new DeclRefExpr(handle),
            CX_StmtClass.CX_StmtClass_DependentCoawaitExpr => new DependentCoawaitExpr(handle),
            CX_StmtClass.CX_StmtClass_DependentScopeDeclRefExpr => new DependentScopeDeclRefExpr(handle),
            CX_StmtClass.CX_StmtClass_DesignatedInitExpr => new DesignatedInitExpr(handle),
            CX_StmtClass.CX_StmtClass_DesignatedInitUpdateExpr => new DesignatedInitUpdateExpr(handle),
            CX_StmtClass.CX_StmtClass_ExpressionTraitExpr => new ExpressionTraitExpr(handle),
            CX_StmtClass.CX_StmtClass_ExtVectorElementExpr => new ExtVectorElementExpr(handle),
            CX_StmtClass.CX_StmtClass_FixedPointLiteral => new FixedPointLiteral(handle),
            CX_StmtClass.CX_StmtClass_FloatingLiteral => new FloatingLiteral(handle),
            CX_StmtClass.CX_StmtClass_ConstantExpr => new ConstantExpr(handle),
            CX_StmtClass.CX_StmtClass_ExprWithCleanups => new ExprWithCleanups(handle),
            CX_StmtClass.CX_StmtClass_FunctionParmPackExpr => new FunctionParmPackExpr(handle),
            CX_StmtClass.CX_StmtClass_GNUNullExpr => new GNUNullExpr(handle),
            CX_StmtClass.CX_StmtClass_GenericSelectionExpr => new GenericSelectionExpr(handle),
            CX_StmtClass.CX_StmtClass_ImaginaryLiteral => new ImaginaryLiteral(handle),
            CX_StmtClass.CX_StmtClass_ImplicitValueInitExpr => new ImplicitValueInitExpr(handle),
            CX_StmtClass.CX_StmtClass_InitListExpr => new InitListExpr(handle),
            CX_StmtClass.CX_StmtClass_IntegerLiteral => new IntegerLiteral(handle),
            CX_StmtClass.CX_StmtClass_LambdaExpr => new LambdaExpr(handle),
            CX_StmtClass.CX_StmtClass_MSPropertyRefExpr => new MSPropertyRefExpr(handle),
            CX_StmtClass.CX_StmtClass_MSPropertySubscriptExpr => new MSPropertySubscriptExpr(handle),
            CX_StmtClass.CX_StmtClass_MaterializeTemporaryExpr => new MaterializeTemporaryExpr(handle),
            CX_StmtClass.CX_StmtClass_MemberExpr => new MemberExpr(handle),
            CX_StmtClass.CX_StmtClass_NoInitExpr => new NoInitExpr(handle),
            CX_StmtClass.CX_StmtClass_OMPArraySectionExpr => new OMPArraySectionExpr(handle),
            CX_StmtClass.CX_StmtClass_ObjCArrayLiteral => new ObjCArrayLiteral(handle),
            CX_StmtClass.CX_StmtClass_ObjCAvailabilityCheckExpr => new ObjCAvailabilityCheckExpr(handle),
            CX_StmtClass.CX_StmtClass_ObjCBoolLiteralExpr => new ObjCBoolLiteralExpr(handle),
            CX_StmtClass.CX_StmtClass_ObjCBoxedExpr => new ObjCBoxedExpr(handle),
            CX_StmtClass.CX_StmtClass_ObjCDictionaryLiteral => new ObjCDictionaryLiteral(handle),
            CX_StmtClass.CX_StmtClass_ObjCEncodeExpr => new ObjCEncodeExpr(handle),
            CX_StmtClass.CX_StmtClass_ObjCIndirectCopyRestoreExpr => new ObjCIndirectCopyRestoreExpr(handle),
            CX_StmtClass.CX_StmtClass_ObjCIsaExpr => new ObjCIsaExpr(handle),
            CX_StmtClass.CX_StmtClass_ObjCIvarRefExpr => new ObjCIvarRefExpr(handle),
            CX_StmtClass.CX_StmtClass_ObjCMessageExpr => new ObjCMessageExpr(handle),
            CX_StmtClass.CX_StmtClass_ObjCPropertyRefExpr => new ObjCPropertyRefExpr(handle),
            CX_StmtClass.CX_StmtClass_ObjCProtocolExpr => new ObjCProtocolExpr(handle),
            CX_StmtClass.CX_StmtClass_ObjCSelectorExpr => new ObjCSelectorExpr(handle),
            CX_StmtClass.CX_StmtClass_ObjCStringLiteral => new ObjCStringLiteral(handle),
            CX_StmtClass.CX_StmtClass_ObjCSubscriptRefExpr => new ObjCSubscriptRefExpr(handle),
            CX_StmtClass.CX_StmtClass_OffsetOfExpr => new OffsetOfExpr(handle),
            CX_StmtClass.CX_StmtClass_OpaqueValueExpr => new OpaqueValueExpr(handle),
            CX_StmtClass.CX_StmtClass_UnresolvedLookupExpr => new UnresolvedLookupExpr(handle),
            CX_StmtClass.CX_StmtClass_UnresolvedMemberExpr => new UnresolvedMemberExpr(handle),
            CX_StmtClass.CX_StmtClass_PackExpansionExpr => new PackExpansionExpr(handle),
            CX_StmtClass.CX_StmtClass_ParenExpr => new ParenExpr(handle),
            CX_StmtClass.CX_StmtClass_ParenListExpr => new ParenListExpr(handle),
            CX_StmtClass.CX_StmtClass_PredefinedExpr => new PredefinedExpr(handle),
            CX_StmtClass.CX_StmtClass_PseudoObjectExpr => new PseudoObjectExpr(handle),
            CX_StmtClass.CX_StmtClass_ShuffleVectorExpr => new ShuffleVectorExpr(handle),
            CX_StmtClass.CX_StmtClass_SizeOfPackExpr => new SizeOfPackExpr(handle),
            CX_StmtClass.CX_StmtClass_SourceLocExpr => new SourceLocExpr(handle),
            CX_StmtClass.CX_StmtClass_StmtExpr => new StmtExpr(handle),
            CX_StmtClass.CX_StmtClass_StringLiteral => new StringLiteral(handle),
            CX_StmtClass.CX_StmtClass_SubstNonTypeTemplateParmExpr => new SubstNonTypeTemplateParmExpr(handle),
            CX_StmtClass.CX_StmtClass_SubstNonTypeTemplateParmPackExpr => new SubstNonTypeTemplateParmPackExpr(handle),
            CX_StmtClass.CX_StmtClass_TypeTraitExpr => new TypeTraitExpr(handle),
            CX_StmtClass.CX_StmtClass_TypoExpr => new TypoExpr(handle),
            CX_StmtClass.CX_StmtClass_UnaryExprOrTypeTraitExpr => new UnaryExprOrTypeTraitExpr(handle),
            CX_StmtClass.CX_StmtClass_UnaryOperator => new UnaryOperator(handle),
            CX_StmtClass.CX_StmtClass_VAArgExpr => new VAArgExpr(handle),
            CX_StmtClass.CX_StmtClass_LabelStmt => new LabelStmt(handle),
            CX_StmtClass.CX_StmtClass_WhileStmt => new WhileStmt(handle),
            _ => new Stmt(handle, handle.Kind, handle.StmtClass),
        };
    }
}
