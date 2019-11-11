// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class Stmt : Cursor
    {
        private readonly Lazy<IReadOnlyList<Stmt>> _children;

        private protected Stmt(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
            _children = new Lazy<IReadOnlyList<Stmt>>(() => CursorChildren.OfType<Stmt>().ToList());
        }

        public IReadOnlyList<Stmt> Children => _children.Value;

        internal static new Stmt Create(CXCursor handle)
        {
            Stmt result;

            switch (handle.Kind)
            {
                case CXCursorKind.CXCursor_UnexposedStmt:
                {
                    result = new Stmt(handle, handle.Kind);
                    break;
                }

                case CXCursorKind.CXCursor_LabelStmt:
                {
                    result = new LabelStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CompoundStmt:
                {
                    result = new CompoundStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CaseStmt:
                {
                    result = new CaseStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_DefaultStmt:
                {
                    result = new DefaultStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_IfStmt:
                {
                    result = new IfStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_SwitchStmt:
                {
                    result = new SwitchStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_WhileStmt:
                {
                    result = new WhileStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_DoStmt:
                {
                    result = new DoStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ForStmt:
                {
                    result = new ForStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_GotoStmt:
                {
                    result = new GotoStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_IndirectGotoStmt:
                {
                    result = new IndirectGotoStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ContinueStmt:
                {
                    result = new ContinueStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_BreakStmt:
                {
                    result = new BreakStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ReturnStmt:
                {
                    result = new ReturnStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_GCCAsmStmt:
                {
                    result = new GCCAsmStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCAtTryStmt:
                {
                    result = new ObjCAtTryStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCAtCatchStmt:
                {
                    result = new ObjCAtCatchStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCAtFinallyStmt:
                {
                    result = new ObjCAtFinallyStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCAtThrowStmt:
                {
                    result = new ObjCAtThrowStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCAtSynchronizedStmt:
                {
                    result = new ObjCAtSynchronizedStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCAutoreleasePoolStmt:
                {
                    result = new ObjCAutoreleasePoolStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCForCollectionStmt:
                {
                    result = new ObjCForCollectionStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CXXCatchStmt:
                {
                    result = new CXXCatchStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CXXTryStmt:
                {
                    result = new CXXTryStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_CXXForRangeStmt:
                {
                    result = new CXXForRangeStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_SEHTryStmt:
                {
                    result = new SEHTryStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_SEHExceptStmt:
                {
                    result = new SEHExceptStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_SEHFinallyStmt:
                {
                    result = new SEHFinallyStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_MSAsmStmt:
                {
                    result = new MSAsmStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_NullStmt:
                {
                    result = new NullStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_DeclStmt:
                {
                    result = new DeclStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPParallelDirective:
                {
                    result = new OMPParallelDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPSimdDirective:
                {
                    result = new OMPSimdDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPForDirective:
                {
                    result = new OMPForDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPSectionsDirective:
                {
                    result = new OMPSectionsDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPSectionDirective:
                {
                    result = new OMPSectionDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPSingleDirective:
                {
                    result = new OMPSingleDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPParallelForDirective:
                {
                    result = new OMPParallelForDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPParallelSectionsDirective:
                {
                    result = new OMPParallelSectionsDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTaskDirective:
                {
                    result = new OMPTaskDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPMasterDirective:
                {
                    result = new OMPMasterDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPCriticalDirective:
                {
                    result = new OMPCriticalDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTaskyieldDirective:
                {
                    result = new OMPTaskyieldDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPBarrierDirective:
                {
                    result = new OMPBarrierDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTaskwaitDirective:
                {
                    result = new OMPTaskwaitDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPFlushDirective:
                {
                    result = new OMPFlushDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_SEHLeaveStmt:
                {
                    result = new SEHLeaveStmt(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPOrderedDirective:
                {
                    result = new OMPOrderedDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPAtomicDirective:
                {
                    result = new OMPAtomicDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPForSimdDirective:
                {
                    result = new OMPForSimdDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPParallelForSimdDirective:
                {
                    result = new OMPParallelForSimdDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTargetDirective:
                {
                    result = new OMPTargetDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTeamsDirective:
                {
                    result = new OMPTeamsDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTaskgroupDirective:
                {
                    result = new OMPTaskgroupDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPCancellationPointDirective:
                {
                    result = new OMPCancellationPointDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPCancelDirective:
                {
                    result = new OMPCancelDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTargetDataDirective:
                {
                    result = new OMPTargetDataDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTaskLoopDirective:
                {
                    result = new OMPTaskLoopDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTaskLoopSimdDirective:
                {
                    result = new OMPTaskLoopSimdDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPDistributeDirective:
                {
                    result = new OMPDistributeDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTargetEnterDataDirective:
                {
                    result = new OMPTargetEnterDataDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTargetExitDataDirective:
                {
                    result = new OMPTargetExitDataDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTargetParallelDirective:
                {
                    result = new OMPTargetParallelDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTargetParallelForDirective:
                {
                    result = new OMPTargetParallelForDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTargetUpdateDirective:
                {
                    result = new OMPTargetUpdateDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPDistributeParallelForDirective:
                {
                    result = new OMPDistributeParallelForDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPDistributeParallelForSimdDirective:
                {
                    result = new OMPDistributeParallelForSimdDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPDistributeSimdDirective:
                {
                    result = new OMPDistributeSimdDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTargetParallelForSimdDirective:
                {
                    result = new OMPTargetParallelForSimdDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTargetSimdDirective:
                {
                    result = new OMPTargetSimdDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTeamsDistributeDirective:
                {
                    result = new OMPTeamsDistributeDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTeamsDistributeSimdDirective:
                {
                    result = new OMPTeamsDistributeSimdDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTeamsDistributeParallelForSimdDirective:
                {
                    result = new OMPTeamsDistributeParallelForSimdDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTeamsDistributeParallelForDirective:
                {
                    result = new OMPTeamsDistributeParallelForDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTargetTeamsDirective:
                {
                    result = new OMPTargetTeamsDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTargetTeamsDistributeDirective:
                {
                    result = new OMPTargetTeamsDistributeDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTargetTeamsDistributeParallelForDirective:
                {
                    result = new OMPTargetTeamsDistributeParallelForDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTargetTeamsDistributeParallelForSimdDirective:
                {
                    result = new OMPTargetTeamsDistributeParallelForSimdDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_OMPTargetTeamsDistributeSimdDirective:
                {
                    result = new OMPTargetTeamsDistributeSimdDirective(handle);
                    break;
                }

                case CXCursorKind.CXCursor_BuiltinBitCastExpr:
                {
                    result = new BuiltinBitCastExpr(handle);
                    break;
                }

                default:
                {
                    Debug.WriteLine($"Unhandled statement kind: {handle.KindSpelling}.");
                    result = new Stmt(handle, handle.Kind);
                    break;
                }
            }

            return result;
        }
    }
}
