// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class Attr : Cursor
    {
        private protected Attr(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
        }

        internal static new Attr Create(CXCursor handle)
        {
            Attr result;

            switch (handle.Kind)
            {
                case CXCursorKind.CXCursor_UnexposedAttr:
                {
                    result = new Attr(handle, CXCursorKind.CXCursor_UnexposedAttr);
                    break;
                }

                case CXCursorKind.CXCursor_IBActionAttr:
                case CXCursorKind.CXCursor_IBOutletAttr:
                case CXCursorKind.CXCursor_IBOutletCollectionAttr:
                case CXCursorKind.CXCursor_CXXFinalAttr:
                case CXCursorKind.CXCursor_CXXOverrideAttr:
                {
                    result = new InheritableAttr(handle, handle.Kind);
                    break;
                }


                case CXCursorKind.CXCursor_AnnotateAttr:
                {
                    result = new InheritableParamAttr(handle, handle.Kind);
                    break;
                }

                case CXCursorKind.CXCursor_AsmLabelAttr:
                case CXCursorKind.CXCursor_PackedAttr:
                case CXCursorKind.CXCursor_PureAttr:
                case CXCursorKind.CXCursor_ConstAttr:
                case CXCursorKind.CXCursor_NoDuplicateAttr:
                case CXCursorKind.CXCursor_CUDAConstantAttr:
                case CXCursorKind.CXCursor_CUDADeviceAttr:
                case CXCursorKind.CXCursor_CUDAGlobalAttr:
                case CXCursorKind.CXCursor_CUDAHostAttr:
                case CXCursorKind.CXCursor_CUDASharedAttr:
                case CXCursorKind.CXCursor_VisibilityAttr:
                case CXCursorKind.CXCursor_DLLExport:
                case CXCursorKind.CXCursor_DLLImport:
                {
                    result = new InheritableAttr(handle, handle.Kind);
                    break;
                }

                case CXCursorKind.CXCursor_NSReturnsRetained:
                {
                    result = new DeclOrTypeAttr(handle, handle.Kind);
                    break;
                }

                case CXCursorKind.CXCursor_NSReturnsNotRetained:
                case CXCursorKind.CXCursor_NSReturnsAutoreleased:
                case CXCursorKind.CXCursor_NSConsumesSelf:
                {
                    result = new InheritableAttr(handle, handle.Kind);
                    break;
                }

                case CXCursorKind.CXCursor_NSConsumed:
                {
                    result = new InheritableParamAttr(handle, handle.Kind);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCException:
                case CXCursorKind.CXCursor_ObjCNSObject:
                case CXCursorKind.CXCursor_ObjCIndependentClass:
                case CXCursorKind.CXCursor_ObjCPreciseLifetime:
                case CXCursorKind.CXCursor_ObjCReturnsInnerPointer:
                case CXCursorKind.CXCursor_ObjCRequiresSuper:
                case CXCursorKind.CXCursor_ObjCRootClass:
                case CXCursorKind.CXCursor_ObjCSubclassingRestricted:
                case CXCursorKind.CXCursor_ObjCExplicitProtocolImpl:
                {
                    result = new InheritableAttr(handle, handle.Kind);
                    break;
                }

                case CXCursorKind.CXCursor_ObjCDesignatedInitializer:
                case CXCursorKind.CXCursor_ObjCRuntimeVisible:
                case CXCursorKind.CXCursor_ObjCBoxable:
                {
                    result = new Attr(handle, handle.Kind);
                    break;
                }

                case CXCursorKind.CXCursor_FlagEnum:
                case CXCursorKind.CXCursor_ConvergentAttr:
                case CXCursorKind.CXCursor_WarnUnusedAttr:
                case CXCursorKind.CXCursor_WarnUnusedResultAttr:
                case CXCursorKind.CXCursor_AlignedAttr:
                {
                    result = new InheritableAttr(handle, handle.Kind);
                    break;
                }

                default:
                {
                    Debug.WriteLine($"Unhandled attribute kind: {handle.KindSpelling}.");
                    result = new Attr(handle, handle.kind);
                    break;
                }
            }

            return result;
        }

        public CX_AttrKind Kind => Handle.AttrKind;
    }
}
