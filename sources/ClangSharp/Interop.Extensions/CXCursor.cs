// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ClangSharp.Interop
{
    [DebuggerDisplay("{DebuggerDisplayString,nq}")]
    public unsafe partial struct CXCursor : IEquatable<CXCursor>
    {
        public static CXCursor Null => clang.getNullCursor();

        public CX_AttrKind AttrKind => clangsharp.Cursor_getAttrKind(this);

        public string AttrKindSpelling
        {
            get
            {
                Debug.Assert(CX_AttrKind.CX_AttrKind_FirstDeclOrTypeAttr == CX_AttrKind.CX_AttrKind_AArch64VectorPcs);
                Debug.Assert(CX_AttrKind.CX_AttrKind_LastDeclOrTypeAttr == CX_AttrKind.CX_AttrKind_VectorCall);
                Debug.Assert(CX_AttrKind.CX_AttrKind_FirstInheritableParamAttr == CX_AttrKind.CX_AttrKind_SwiftContext);
                Debug.Assert(CX_AttrKind.CX_AttrKind_LastInheritableParamAttr == CX_AttrKind.CX_AttrKind_UseHandle);
                Debug.Assert(CX_AttrKind.CX_AttrKind_FirstParameterABIAttr == CX_AttrKind.CX_AttrKind_SwiftContext);
                Debug.Assert(CX_AttrKind.CX_AttrKind_LastParameterABIAttr == CX_AttrKind.CX_AttrKind_SwiftIndirectResult);
                Debug.Assert(CX_AttrKind.CX_AttrKind_LastAttr == CX_AttrKind.CX_AttrKind_Thread);
                Debug.Assert(CX_AttrKind.CX_AttrKind_FirstTypeAttr == CX_AttrKind.CX_AttrKind_AddressSpace);
                Debug.Assert(CX_AttrKind.CX_AttrKind_LastTypeAttr == CX_AttrKind.CX_AttrKind_UPtr);
                Debug.Assert(CX_AttrKind.CX_AttrKind_FirstStmtAttr == CX_AttrKind.CX_AttrKind_FallThrough);
                Debug.Assert(CX_AttrKind.CX_AttrKind_LastStmtAttr == CX_AttrKind.CX_AttrKind_Suppress);
                Debug.Assert(CX_AttrKind.CX_AttrKind_FirstInheritableAttr == CX_AttrKind.CX_AttrKind_AArch64VectorPcs);
                Debug.Assert(CX_AttrKind.CX_AttrKind_LastInheritableAttr == CX_AttrKind.CX_AttrKind_XRayLogArgs);

                return AttrKind switch
                {
                    CX_AttrKind.CX_AttrKind_Invalid => "Invalid",
                    CX_AttrKind.CX_AttrKind_AddressSpace => "AddressSpace",
                    CX_AttrKind.CX_AttrKind_NoDeref => "NoDeref",
                    CX_AttrKind.CX_AttrKind_ObjCGC => "ObjCGC",
                    CX_AttrKind.CX_AttrKind_ObjCInertUnsafeUnretained => "ObjCInertUnsafeUnretained",
                    CX_AttrKind.CX_AttrKind_ObjCKindOf => "ObjCKindOf",
                    CX_AttrKind.CX_AttrKind_OpenCLConstantAddressSpace => "OpenCLConstantAddressSpace",
                    CX_AttrKind.CX_AttrKind_OpenCLGenericAddressSpace => "OpenCLGenericAddressSpace",
                    CX_AttrKind.CX_AttrKind_OpenCLGlobalAddressSpace => "OpenCLGlobalAddressSpace",
                    CX_AttrKind.CX_AttrKind_OpenCLLocalAddressSpace => "OpenCLLocalAddressSpace",
                    CX_AttrKind.CX_AttrKind_OpenCLPrivateAddressSpace => "OpenCLPrivateAddressSpace",
                    CX_AttrKind.CX_AttrKind_Ptr32 => "Ptr32",
                    CX_AttrKind.CX_AttrKind_Ptr64 => "Ptr64",
                    CX_AttrKind.CX_AttrKind_SPtr => "SPtr",
                    CX_AttrKind.CX_AttrKind_TypeNonNull => "TypeNonNull",
                    CX_AttrKind.CX_AttrKind_TypeNullUnspecified => "TypeNullUnspecified",
                    CX_AttrKind.CX_AttrKind_TypeNullable => "TypeNullable",
                    CX_AttrKind.CX_AttrKind_UPtr => "UPtr",
                    CX_AttrKind.CX_AttrKind_FallThrough => "FallThrough",
                    CX_AttrKind.CX_AttrKind_Suppress => "Suppress",
                    CX_AttrKind.CX_AttrKind_AArch64VectorPcs => "AArch64VectorPcs",
                    CX_AttrKind.CX_AttrKind_AcquireHandle => "AcquireHandle",
                    CX_AttrKind.CX_AttrKind_AnyX86NoCfCheck => "AnyX86NoCfCheck",
                    CX_AttrKind.CX_AttrKind_CDecl => "CDecl",
                    CX_AttrKind.CX_AttrKind_FastCall => "FastCall",
                    CX_AttrKind.CX_AttrKind_IntelOclBicc => "IntelOclBicc",
                    CX_AttrKind.CX_AttrKind_LifetimeBound => "LifetimeBound",
                    CX_AttrKind.CX_AttrKind_MSABI => "MSABI",
                    CX_AttrKind.CX_AttrKind_NSReturnsRetained => "NSReturnsRetained",
                    CX_AttrKind.CX_AttrKind_ObjCOwnership => "ObjCOwnership",
                    CX_AttrKind.CX_AttrKind_Pascal => "Pascal",
                    CX_AttrKind.CX_AttrKind_Pcs => "Pcs",
                    CX_AttrKind.CX_AttrKind_PreserveAll => "PreserveAll",
                    CX_AttrKind.CX_AttrKind_PreserveMost => "PreserveMost",
                    CX_AttrKind.CX_AttrKind_RegCall => "RegCall",
                    CX_AttrKind.CX_AttrKind_StdCall => "StdCall",
                    CX_AttrKind.CX_AttrKind_SwiftCall => "SwiftCall",
                    CX_AttrKind.CX_AttrKind_SysVABI => "SysVABI",
                    CX_AttrKind.CX_AttrKind_ThisCall => "ThisCall",
                    CX_AttrKind.CX_AttrKind_VectorCall => "VectorCall",
                    CX_AttrKind.CX_AttrKind_SwiftContext => "SwiftContext",
                    CX_AttrKind.CX_AttrKind_SwiftErrorResult => "SwiftErrorResult",
                    CX_AttrKind.CX_AttrKind_SwiftIndirectResult => "SwiftIndirectResult",
                    CX_AttrKind.CX_AttrKind_Annotate => "Annotate",
                    CX_AttrKind.CX_AttrKind_CFConsumed => "CFConsumed",
                    CX_AttrKind.CX_AttrKind_CarriesDependency => "CarriesDependency",
                    CX_AttrKind.CX_AttrKind_NSConsumed => "NSConsumed",
                    CX_AttrKind.CX_AttrKind_NonNull => "NonNull",
                    CX_AttrKind.CX_AttrKind_OSConsumed => "OSConsumed",
                    CX_AttrKind.CX_AttrKind_PassObjectSize => "PassObjectSize",
                    CX_AttrKind.CX_AttrKind_ReleaseHandle => "ReleaseHandle",
                    CX_AttrKind.CX_AttrKind_UseHandle => "UseHandle",
                    CX_AttrKind.CX_AttrKind_AMDGPUFlatWorkGroupSize => "AMDGPUFlatWorkGroupSize",
                    CX_AttrKind.CX_AttrKind_AMDGPUNumSGPR => "AMDGPUNumSGPR",
                    CX_AttrKind.CX_AttrKind_AMDGPUNumVGPR => "AMDGPUNumVGPR",
                    CX_AttrKind.CX_AttrKind_AMDGPUWavesPerEU => "AMDGPUWavesPerEU",
                    CX_AttrKind.CX_AttrKind_ARMInterrupt => "ARMInterrupt",
                    CX_AttrKind.CX_AttrKind_AVRInterrupt => "AVRInterrupt",
                    CX_AttrKind.CX_AttrKind_AVRSignal => "AVRSignal",
                    CX_AttrKind.CX_AttrKind_AcquireCapability => "AcquireCapability",
                    CX_AttrKind.CX_AttrKind_AcquiredAfter => "AcquiredAfter",
                    CX_AttrKind.CX_AttrKind_AcquiredBefore => "AcquiredBefore",
                    CX_AttrKind.CX_AttrKind_AlignMac68k => "AlignMac68k",
                    CX_AttrKind.CX_AttrKind_Aligned => "Aligned",
                    CX_AttrKind.CX_AttrKind_AllocAlign => "AllocAlign",
                    CX_AttrKind.CX_AttrKind_AllocSize => "AllocSize",
                    CX_AttrKind.CX_AttrKind_AlwaysDestroy => "AlwaysDestroy",
                    CX_AttrKind.CX_AttrKind_AlwaysInline => "AlwaysInline",
                    CX_AttrKind.CX_AttrKind_AnalyzerNoReturn => "AnalyzerNoReturn",
                    CX_AttrKind.CX_AttrKind_AnyX86Interrupt => "AnyX86Interrupt",
                    CX_AttrKind.CX_AttrKind_AnyX86NoCallerSavedRegisters => "AnyX86NoCallerSavedRegisters",
                    CX_AttrKind.CX_AttrKind_ArcWeakrefUnavailable => "ArcWeakrefUnavailable",
                    CX_AttrKind.CX_AttrKind_ArgumentWithTypeTag => "ArgumentWithTypeTag",
                    CX_AttrKind.CX_AttrKind_ArmMveAlias => "ArmMveAlias",
                    CX_AttrKind.CX_AttrKind_Artificial => "Artificial",
                    CX_AttrKind.CX_AttrKind_AsmLabel => "AsmLabel",
                    CX_AttrKind.CX_AttrKind_AssertCapability => "AssertCapability",
                    CX_AttrKind.CX_AttrKind_AssertExclusiveLock => "AssertExclusiveLock",
                    CX_AttrKind.CX_AttrKind_AssertSharedLock => "AssertSharedLock",
                    CX_AttrKind.CX_AttrKind_AssumeAligned => "AssumeAligned",
                    CX_AttrKind.CX_AttrKind_Availability => "Availability",
                    CX_AttrKind.CX_AttrKind_BPFPreserveAccessIndex => "BPFPreserveAccessIndex",
                    CX_AttrKind.CX_AttrKind_Blocks => "Blocks",
                    CX_AttrKind.CX_AttrKind_C11NoReturn => "C11NoReturn",
                    CX_AttrKind.CX_AttrKind_CFAuditedTransfer => "CFAuditedTransfer",
                    CX_AttrKind.CX_AttrKind_CFGuard => "CFGuard",
                    CX_AttrKind.CX_AttrKind_CFICanonicalJumpTable => "CFICanonicalJumpTable",
                    CX_AttrKind.CX_AttrKind_CFReturnsNotRetained => "CFReturnsNotRetained",
                    CX_AttrKind.CX_AttrKind_CFReturnsRetained => "CFReturnsRetained",
                    CX_AttrKind.CX_AttrKind_CFUnknownTransfer => "CFUnknownTransfer",
                    CX_AttrKind.CX_AttrKind_CPUDispatch => "CPUDispatch",
                    CX_AttrKind.CX_AttrKind_CPUSpecific => "CPUSpecific",
                    CX_AttrKind.CX_AttrKind_CUDAConstant => "CUDAConstant",
                    CX_AttrKind.CX_AttrKind_CUDADevice => "CUDADevice",
                    CX_AttrKind.CX_AttrKind_CUDAGlobal => "CUDAGlobal",
                    CX_AttrKind.CX_AttrKind_CUDAHost => "CUDAHost",
                    CX_AttrKind.CX_AttrKind_CUDAInvalidTarget => "CUDAInvalidTarget",
                    CX_AttrKind.CX_AttrKind_CUDALaunchBounds => "CUDALaunchBounds",
                    CX_AttrKind.CX_AttrKind_CUDAShared => "CUDAShared",
                    CX_AttrKind.CX_AttrKind_CXX11NoReturn => "CXX11NoReturn",
                    CX_AttrKind.CX_AttrKind_CallableWhen => "CallableWhen",
                    CX_AttrKind.CX_AttrKind_Callback => "Callback",
                    CX_AttrKind.CX_AttrKind_Capability => "Capability",
                    CX_AttrKind.CX_AttrKind_CapturedRecord => "CapturedRecord",
                    CX_AttrKind.CX_AttrKind_Cleanup => "Cleanup",
                    CX_AttrKind.CX_AttrKind_CodeSeg => "CodeSeg",
                    CX_AttrKind.CX_AttrKind_Cold => "Cold",
                    CX_AttrKind.CX_AttrKind_Common => "Common",
                    CX_AttrKind.CX_AttrKind_Const => "Const",
                    CX_AttrKind.CX_AttrKind_ConstInit => "ConstInit",
                    CX_AttrKind.CX_AttrKind_Constructor => "Constructor",
                    CX_AttrKind.CX_AttrKind_Consumable => "Consumable",
                    CX_AttrKind.CX_AttrKind_ConsumableAutoCast => "ConsumableAutoCast",
                    CX_AttrKind.CX_AttrKind_ConsumableSetOnRead => "ConsumableSetOnRead",
                    CX_AttrKind.CX_AttrKind_Convergent => "Convergent",
                    CX_AttrKind.CX_AttrKind_DLLExport => "DLLExport",
                    CX_AttrKind.CX_AttrKind_DLLExportStaticLocal => "DLLExportStaticLocal",
                    CX_AttrKind.CX_AttrKind_DLLImport => "DLLImport",
                    CX_AttrKind.CX_AttrKind_DLLImportStaticLocal => "DLLImportStaticLocal",
                    CX_AttrKind.CX_AttrKind_Deprecated => "Deprecated",
                    CX_AttrKind.CX_AttrKind_Destructor => "Destructor",
                    CX_AttrKind.CX_AttrKind_DiagnoseIf => "DiagnoseIf",
                    CX_AttrKind.CX_AttrKind_DisableTailCalls => "DisableTailCalls",
                    CX_AttrKind.CX_AttrKind_EmptyBases => "EmptyBases",
                    CX_AttrKind.CX_AttrKind_EnableIf => "EnableIf",
                    CX_AttrKind.CX_AttrKind_EnumExtensibility => "EnumExtensibility",
                    CX_AttrKind.CX_AttrKind_ExcludeFromExplicitInstantiation => "ExcludeFromExplicitInstantiation",
                    CX_AttrKind.CX_AttrKind_ExclusiveTrylockFunction => "ExclusiveTrylockFunction",
                    CX_AttrKind.CX_AttrKind_ExternalSourceSymbol => "ExternalSourceSymbol",
                    CX_AttrKind.CX_AttrKind_Final => "Final",
                    CX_AttrKind.CX_AttrKind_FlagEnum => "FlagEnum",
                    CX_AttrKind.CX_AttrKind_Flatten => "Flatten",
                    CX_AttrKind.CX_AttrKind_Format => "Format",
                    CX_AttrKind.CX_AttrKind_FormatArg => "FormatArg",
                    CX_AttrKind.CX_AttrKind_GNUInline => "GNUInline",
                    CX_AttrKind.CX_AttrKind_GuardedBy => "GuardedBy",
                    CX_AttrKind.CX_AttrKind_GuardedVar => "GuardedVar",
                    CX_AttrKind.CX_AttrKind_HIPPinnedShadow => "HIPPinnedShadow",
                    CX_AttrKind.CX_AttrKind_Hot => "Hot",
                    CX_AttrKind.CX_AttrKind_IBAction => "IBAction",
                    CX_AttrKind.CX_AttrKind_IBOutlet => "IBOutlet",
                    CX_AttrKind.CX_AttrKind_IBOutletCollection => "IBOutletCollection",
                    CX_AttrKind.CX_AttrKind_InitPriority => "InitPriority",
                    CX_AttrKind.CX_AttrKind_InternalLinkage => "InternalLinkage",
                    CX_AttrKind.CX_AttrKind_LTOVisibilityPublic => "LTOVisibilityPublic",
                    CX_AttrKind.CX_AttrKind_LayoutVersion => "LayoutVersion",
                    CX_AttrKind.CX_AttrKind_LockReturned => "LockReturned",
                    CX_AttrKind.CX_AttrKind_LocksExcluded => "LocksExcluded",
                    CX_AttrKind.CX_AttrKind_MIGServerRoutine => "MIGServerRoutine",
                    CX_AttrKind.CX_AttrKind_MSAllocator => "MSAllocator",
                    CX_AttrKind.CX_AttrKind_MSInheritance => "MSInheritance",
                    CX_AttrKind.CX_AttrKind_MSNoVTable => "MSNoVTable",
                    CX_AttrKind.CX_AttrKind_MSP430Interrupt => "MSP430Interrupt",
                    CX_AttrKind.CX_AttrKind_MSStruct => "MSStruct",
                    CX_AttrKind.CX_AttrKind_MSVtorDisp => "MSVtorDisp",
                    CX_AttrKind.CX_AttrKind_MaxFieldAlignment => "MaxFieldAlignment",
                    CX_AttrKind.CX_AttrKind_MayAlias => "MayAlias",
                    CX_AttrKind.CX_AttrKind_MicroMips => "MicroMips",
                    CX_AttrKind.CX_AttrKind_MinSize => "MinSize",
                    CX_AttrKind.CX_AttrKind_MinVectorWidth => "MinVectorWidth",
                    CX_AttrKind.CX_AttrKind_Mips16 => "Mips16",
                    CX_AttrKind.CX_AttrKind_MipsInterrupt => "MipsInterrupt",
                    CX_AttrKind.CX_AttrKind_MipsLongCall => "MipsLongCall",
                    CX_AttrKind.CX_AttrKind_MipsShortCall => "MipsShortCall",
                    CX_AttrKind.CX_AttrKind_NSConsumesSelf => "NSConsumesSelf",
                    CX_AttrKind.CX_AttrKind_NSReturnsAutoreleased => "NSReturnsAutoreleased",
                    CX_AttrKind.CX_AttrKind_NSReturnsNotRetained => "NSReturnsNotRetained",
                    CX_AttrKind.CX_AttrKind_Naked => "Naked",
                    CX_AttrKind.CX_AttrKind_NoAlias => "NoAlias",
                    CX_AttrKind.CX_AttrKind_NoCommon => "NoCommon",
                    CX_AttrKind.CX_AttrKind_NoDebug => "NoDebug",
                    CX_AttrKind.CX_AttrKind_NoDestroy => "NoDestroy",
                    CX_AttrKind.CX_AttrKind_NoDuplicate => "NoDuplicate",
                    CX_AttrKind.CX_AttrKind_NoInline => "NoInline",
                    CX_AttrKind.CX_AttrKind_NoInstrumentFunction => "NoInstrumentFunction",
                    CX_AttrKind.CX_AttrKind_NoMicroMips => "NoMicroMips",
                    CX_AttrKind.CX_AttrKind_NoMips16 => "NoMips16",
                    CX_AttrKind.CX_AttrKind_NoReturn => "NoReturn",
                    CX_AttrKind.CX_AttrKind_NoSanitize => "NoSanitize",
                    CX_AttrKind.CX_AttrKind_NoSpeculativeLoadHardening => "NoSpeculativeLoadHardening",
                    CX_AttrKind.CX_AttrKind_NoSplitStack => "NoSplitStack",
                    CX_AttrKind.CX_AttrKind_NoStackProtector => "NoStackProtector",
                    CX_AttrKind.CX_AttrKind_NoThreadSafetyAnalysis => "NoThreadSafetyAnalysis",
                    CX_AttrKind.CX_AttrKind_NoThrow => "NoThrow",
                    CX_AttrKind.CX_AttrKind_NoUniqueAddress => "NoUniqueAddress",
                    CX_AttrKind.CX_AttrKind_NotTailCalled => "NotTailCalled",
                    CX_AttrKind.CX_AttrKind_OMPAllocateDecl => "OMPAllocateDecl",
                    CX_AttrKind.CX_AttrKind_OMPCaptureNoInit => "OMPCaptureNoInit",
                    CX_AttrKind.CX_AttrKind_OMPDeclareTargetDecl => "OMPDeclareTargetDecl",
                    CX_AttrKind.CX_AttrKind_OMPDeclareVariant => "OMPDeclareVariant",
                    CX_AttrKind.CX_AttrKind_OMPThreadPrivateDecl => "OMPThreadPrivateDecl",
                    CX_AttrKind.CX_AttrKind_OSConsumesThis => "OSConsumesThis",
                    CX_AttrKind.CX_AttrKind_OSReturnsNotRetained => "OSReturnsNotRetained",
                    CX_AttrKind.CX_AttrKind_OSReturnsRetained => "OSReturnsRetained",
                    CX_AttrKind.CX_AttrKind_OSReturnsRetainedOnNonZero => "OSReturnsRetainedOnNonZero",
                    CX_AttrKind.CX_AttrKind_OSReturnsRetainedOnZero => "OSReturnsRetainedOnZero",
                    CX_AttrKind.CX_AttrKind_ObjCBridge => "ObjCBridge",
                    CX_AttrKind.CX_AttrKind_ObjCBridgeMutable => "ObjCBridgeMutable",
                    CX_AttrKind.CX_AttrKind_ObjCBridgeRelated => "ObjCBridgeRelated",
                    CX_AttrKind.CX_AttrKind_ObjCException => "ObjCException",
                    CX_AttrKind.CX_AttrKind_ObjCExplicitProtocolImpl => "ObjCExplicitProtocolImpl",
                    CX_AttrKind.CX_AttrKind_ObjCExternallyRetained => "ObjCExternallyRetained",
                    CX_AttrKind.CX_AttrKind_ObjCIndependentClass => "ObjCIndependentClass",
                    CX_AttrKind.CX_AttrKind_ObjCMethodFamily => "ObjCMethodFamily",
                    CX_AttrKind.CX_AttrKind_ObjCNSObject => "ObjCNSObject",
                    CX_AttrKind.CX_AttrKind_ObjCPreciseLifetime => "ObjCPreciseLifetime",
                    CX_AttrKind.CX_AttrKind_ObjCRequiresPropertyDefs => "ObjCRequiresPropertyDefs",
                    CX_AttrKind.CX_AttrKind_ObjCRequiresSuper => "ObjCRequiresSuper",
                    CX_AttrKind.CX_AttrKind_ObjCReturnsInnerPointer => "ObjCReturnsInnerPointer",
                    CX_AttrKind.CX_AttrKind_ObjCRootClass => "ObjCRootClass",
                    CX_AttrKind.CX_AttrKind_ObjCSubclassingRestricted => "ObjCSubclassingRestricted",
                    CX_AttrKind.CX_AttrKind_OpenCLIntelReqdSubGroupSize => "OpenCLIntelReqdSubGroupSize",
                    CX_AttrKind.CX_AttrKind_OpenCLKernel => "OpenCLKernel",
                    CX_AttrKind.CX_AttrKind_OpenCLUnrollHint => "OpenCLUnrollHint",
                    CX_AttrKind.CX_AttrKind_OptimizeNone => "OptimizeNone",
                    CX_AttrKind.CX_AttrKind_Override => "Override",
                    CX_AttrKind.CX_AttrKind_Owner => "Owner",
                    CX_AttrKind.CX_AttrKind_Ownership => "Ownership",
                    CX_AttrKind.CX_AttrKind_Packed => "Packed",
                    CX_AttrKind.CX_AttrKind_ParamTypestate => "ParamTypestate",
                    CX_AttrKind.CX_AttrKind_PatchableFunctionEntry => "PatchableFunctionEntry",
                    CX_AttrKind.CX_AttrKind_Pointer => "Pointer",
                    CX_AttrKind.CX_AttrKind_PragmaClangBSSSection => "PragmaClangBSSSection",
                    CX_AttrKind.CX_AttrKind_PragmaClangDataSection => "PragmaClangDataSection",
                    CX_AttrKind.CX_AttrKind_PragmaClangRelroSection => "PragmaClangRelroSection",
                    CX_AttrKind.CX_AttrKind_PragmaClangRodataSection => "PragmaClangRodataSection",
                    CX_AttrKind.CX_AttrKind_PragmaClangTextSection => "PragmaClangTextSection",
                    CX_AttrKind.CX_AttrKind_PtGuardedBy => "PtGuardedBy",
                    CX_AttrKind.CX_AttrKind_PtGuardedVar => "PtGuardedVar",
                    CX_AttrKind.CX_AttrKind_Pure => "Pure",
                    CX_AttrKind.CX_AttrKind_RISCVInterrupt => "RISCVInterrupt",
                    CX_AttrKind.CX_AttrKind_Reinitializes => "Reinitializes",
                    CX_AttrKind.CX_AttrKind_ReleaseCapability => "ReleaseCapability",
                    CX_AttrKind.CX_AttrKind_ReqdWorkGroupSize => "ReqdWorkGroupSize",
                    CX_AttrKind.CX_AttrKind_RequiresCapability => "RequiresCapability",
                    CX_AttrKind.CX_AttrKind_Restrict => "Restrict",
                    CX_AttrKind.CX_AttrKind_ReturnTypestate => "ReturnTypestate",
                    CX_AttrKind.CX_AttrKind_ReturnsNonNull => "ReturnsNonNull",
                    CX_AttrKind.CX_AttrKind_ReturnsTwice => "ReturnsTwice",
                    CX_AttrKind.CX_AttrKind_SYCLKernel => "SYCLKernel",
                    CX_AttrKind.CX_AttrKind_ScopedLockable => "ScopedLockable",
                    CX_AttrKind.CX_AttrKind_Section => "Section",
                    CX_AttrKind.CX_AttrKind_SelectAny => "SelectAny",
                    CX_AttrKind.CX_AttrKind_Sentinel => "Sentinel",
                    CX_AttrKind.CX_AttrKind_SetTypestate => "SetTypestate",
                    CX_AttrKind.CX_AttrKind_SharedTrylockFunction => "SharedTrylockFunction",
                    CX_AttrKind.CX_AttrKind_SpeculativeLoadHardening => "SpeculativeLoadHardening",
                    CX_AttrKind.CX_AttrKind_TLSModel => "TLSModel",
                    CX_AttrKind.CX_AttrKind_Target => "Target",
                    CX_AttrKind.CX_AttrKind_TestTypestate => "TestTypestate",
                    CX_AttrKind.CX_AttrKind_TransparentUnion => "TransparentUnion",
                    CX_AttrKind.CX_AttrKind_TrivialABI => "TrivialABI",
                    CX_AttrKind.CX_AttrKind_TryAcquireCapability => "TryAcquireCapability",
                    CX_AttrKind.CX_AttrKind_TypeTagForDatatype => "TypeTagForDatatype",
                    CX_AttrKind.CX_AttrKind_TypeVisibility => "TypeVisibility",
                    CX_AttrKind.CX_AttrKind_Unavailable => "Unavailable",
                    CX_AttrKind.CX_AttrKind_Uninitialized => "Uninitialized",
                    CX_AttrKind.CX_AttrKind_Unused => "Unused",
                    CX_AttrKind.CX_AttrKind_Used => "Used",
                    CX_AttrKind.CX_AttrKind_Uuid => "Uuid",
                    CX_AttrKind.CX_AttrKind_VecReturn => "VecReturn",
                    CX_AttrKind.CX_AttrKind_VecTypeHint => "VecTypeHint",
                    CX_AttrKind.CX_AttrKind_Visibility => "Visibility",
                    CX_AttrKind.CX_AttrKind_WarnUnused => "WarnUnused",
                    CX_AttrKind.CX_AttrKind_WarnUnusedResult => "WarnUnusedResult",
                    CX_AttrKind.CX_AttrKind_Weak => "Weak",
                    CX_AttrKind.CX_AttrKind_WeakImport => "WeakImport",
                    CX_AttrKind.CX_AttrKind_WeakRef => "WeakRef",
                    CX_AttrKind.CX_AttrKind_WebAssemblyExportName => "WebAssemblyExportName",
                    CX_AttrKind.CX_AttrKind_WebAssemblyImportModule => "WebAssemblyImportModule",
                    CX_AttrKind.CX_AttrKind_WebAssemblyImportName => "WebAssemblyImportName",
                    CX_AttrKind.CX_AttrKind_WorkGroupSizeHint => "WorkGroupSizeHint",
                    CX_AttrKind.CX_AttrKind_X86ForceAlignArgPointer => "X86ForceAlignArgPointer",
                    CX_AttrKind.CX_AttrKind_XRayInstrument => "XRayInstrument",
                    CX_AttrKind.CX_AttrKind_XRayLogArgs => "XRayLogArgs",
                    CX_AttrKind.CX_AttrKind_AbiTag => "AbiTag",
                    CX_AttrKind.CX_AttrKind_Alias => "Alias",
                    CX_AttrKind.CX_AttrKind_AlignValue => "AlignValue",
                    CX_AttrKind.CX_AttrKind_IFunc => "IFunc",
                    CX_AttrKind.CX_AttrKind_InitSeg => "InitSeg",
                    CX_AttrKind.CX_AttrKind_LoopHint => "LoopHint",
                    CX_AttrKind.CX_AttrKind_Mode => "Mode",
                    CX_AttrKind.CX_AttrKind_NoBuiltin => "NoBuiltin",
                    CX_AttrKind.CX_AttrKind_NoEscape => "NoEscape",
                    CX_AttrKind.CX_AttrKind_OMPCaptureKind => "OMPCaptureKind",
                    CX_AttrKind.CX_AttrKind_OMPDeclareSimdDecl => "OMPDeclareSimdDecl",
                    CX_AttrKind.CX_AttrKind_OMPReferencedVar => "OMPReferencedVar",
                    CX_AttrKind.CX_AttrKind_ObjCBoxable => "ObjCBoxable",
                    CX_AttrKind.CX_AttrKind_ObjCClassStub => "ObjCClassStub",
                    CX_AttrKind.CX_AttrKind_ObjCDesignatedInitializer => "ObjCDesignatedInitializer",
                    CX_AttrKind.CX_AttrKind_ObjCDirect => "ObjCDirect",
                    CX_AttrKind.CX_AttrKind_ObjCDirectMembers => "ObjCDirectMembers",
                    CX_AttrKind.CX_AttrKind_ObjCNonLazyClass => "ObjCNonLazyClass",
                    CX_AttrKind.CX_AttrKind_ObjCRuntimeName => "ObjCRuntimeName",
                    CX_AttrKind.CX_AttrKind_ObjCRuntimeVisible => "ObjCRuntimeVisible",
                    CX_AttrKind.CX_AttrKind_OpenCLAccess => "OpenCLAccess",
                    CX_AttrKind.CX_AttrKind_Overloadable => "Overloadable",
                    CX_AttrKind.CX_AttrKind_RenderScriptKernel => "RenderScriptKernel",
                    CX_AttrKind.CX_AttrKind_Thread => "Thread",
                    _ => Kind.ToString().Substring(12),
                };
            }
        }

        public CXAvailabilityKind Availability => clang.getCursorAvailability(this);

        public CX_BinaryOperatorKind BinaryOperatorKind => clangsharp.Cursor_getBinaryOpcode(this);

        public CXString BinaryOperatorKindSpelling => clangsharp.Cursor_getBinaryOpcodeSpelling(BinaryOperatorKind);

        public CXString BriefCommentText => clang.Cursor_getBriefCommentText(this);

        public CXCursor CanonicalCursor => clang.getCanonicalCursor(this);

        public CX_CastKind CastKind => clangsharp.Cursor_getCastKind(this);

        public string CastKindSpelling
        {
            get
            {
                return CastKind switch
                {
                    CX_CastKind.CX_CK_Invalid => "Invalid",
                    CX_CastKind.CX_CK_Dependent => "Dependent",
                    CX_CastKind.CX_CK_BitCast => "BitCast",
                    CX_CastKind.CX_CK_LValueBitCast => "LValueBitCast",
                    CX_CastKind.CX_CK_LValueToRValueBitCast => "LValueToRValueBitCast",
                    CX_CastKind.CX_CK_LValueToRValue => "LValueToRValue",
                    CX_CastKind.CX_CK_NoOp => "NoOp",
                    CX_CastKind.CX_CK_BaseToDerived => "BaseToDerived",
                    CX_CastKind.CX_CK_DerivedToBase => "DerivedToBase",
                    CX_CastKind.CX_CK_UncheckedDerivedToBase => "UncheckedDerivedToBase",
                    CX_CastKind.CX_CK_Dynamic => "Dynamic",
                    CX_CastKind.CX_CK_ToUnion => "ToUnion",
                    CX_CastKind.CX_CK_ArrayToPointerDecay => "ArrayToPointerDecay",
                    CX_CastKind.CX_CK_FunctionToPointerDecay => "FunctionToPointerDecay",
                    CX_CastKind.CX_CK_NullToPointer => "NullToPointer",
                    CX_CastKind.CX_CK_NullToMemberPointer => "NullToMemberPointer",
                    CX_CastKind.CX_CK_BaseToDerivedMemberPointer => "BaseToDerivedMemberPointer",
                    CX_CastKind.CX_CK_DerivedToBaseMemberPointer => "DerivedToBaseMemberPointer",
                    CX_CastKind.CX_CK_MemberPointerToBoolean => "MemberPointerToBoolean",
                    CX_CastKind.CX_CK_ReinterpretMemberPointer => "ReinterpretMemberPointer",
                    CX_CastKind.CX_CK_UserDefinedConversion => "UserDefinedConversion",
                    CX_CastKind.CX_CK_ConstructorConversion => "ConstructorConversion",
                    CX_CastKind.CX_CK_IntegralToPointer => "IntegralToPointer",
                    CX_CastKind.CX_CK_PointerToIntegral => "PointerToIntegral",
                    CX_CastKind.CX_CK_PointerToBoolean => "PointerToBoolean",
                    CX_CastKind.CX_CK_ToVoid => "ToVoid",
                    CX_CastKind.CX_CK_VectorSplat => "VectorSplat",
                    CX_CastKind.CX_CK_IntegralCast => "IntegralCast",
                    CX_CastKind.CX_CK_IntegralToBoolean => "IntegralToBoolean",
                    CX_CastKind.CX_CK_IntegralToFloating => "IntegralToFloating",
                    CX_CastKind.CX_CK_FixedPointCast => "FixedPointCast",
                    CX_CastKind.CX_CK_FixedPointToIntegral => "FixedPointToIntegral",
                    CX_CastKind.CX_CK_IntegralToFixedPoint => "IntegralToFixedPoint",
                    CX_CastKind.CX_CK_FixedPointToBoolean => "FixedPointToBoolean",
                    CX_CastKind.CX_CK_FloatingToIntegral => "FloatingToIntegral",
                    CX_CastKind.CX_CK_FloatingToBoolean => "FloatingToBoolean",
                    CX_CastKind.CX_CK_BooleanToSignedIntegral => "BooleanToSignedIntegral",
                    CX_CastKind.CX_CK_FloatingCast => "FloatingCast",
                    CX_CastKind.CX_CK_CPointerToObjCPointerCast => "CPointerToObjCPointerCast",
                    CX_CastKind.CX_CK_BlockPointerToObjCPointerCast => "BlockPointerToObjCPointerCast",
                    CX_CastKind.CX_CK_AnyPointerToBlockPointerCast => "AnyPointerToBlockPointerCast",
                    CX_CastKind.CX_CK_ObjCObjectLValueCast => "ObjCObjectLValueCast",
                    CX_CastKind.CX_CK_FloatingRealToComplex => "FloatingRealToComplex",
                    CX_CastKind.CX_CK_FloatingComplexToReal => "FloatingComplexToReal",
                    CX_CastKind.CX_CK_FloatingComplexToBoolean => "FloatingComplexToBoolean",
                    CX_CastKind.CX_CK_FloatingComplexCast => "FloatingComplexCast",
                    CX_CastKind.CX_CK_FloatingComplexToIntegralComplex => "FloatingComplexToIntegralComplex",
                    CX_CastKind.CX_CK_IntegralRealToComplex => "IntegralRealToComplex",
                    CX_CastKind.CX_CK_IntegralComplexToReal => "IntegralComplexToReal",
                    CX_CastKind.CX_CK_IntegralComplexToBoolean => "IntegralComplexToBoolean",
                    CX_CastKind.CX_CK_IntegralComplexCast => "IntegralComplexCast",
                    CX_CastKind.CX_CK_IntegralComplexToFloatingComplex => "IntegralComplexToFloatingComplex",
                    CX_CastKind.CX_CK_ARCProduceObject => "ARCProduceObject",
                    CX_CastKind.CX_CK_ARCConsumeObject => "ARCConsumeObject",
                    CX_CastKind.CX_CK_ARCReclaimReturnedObject => "ARCReclaimReturnedObject",
                    CX_CastKind.CX_CK_ARCExtendBlockObject => "ARCExtendBlockObject",
                    CX_CastKind.CX_CK_AtomicToNonAtomic => "AtomicToNonAtomic",
                    CX_CastKind.CX_CK_NonAtomicToAtomic => "NonAtomicToAtomic",
                    CX_CastKind.CX_CK_CopyAndAutoreleaseBlockObject => "CopyAndAutoreleaseBlockObject",
                    CX_CastKind.CX_CK_BuiltinFnToFnPtr => "BuiltinFnToFnPtr",
                    CX_CastKind.CX_CK_ZeroToOCLOpaqueType => "ZeroToOCLOpaqueType",
                    CX_CastKind.CX_CK_AddressSpaceConversion => "AddressSpaceConversion",
                    CX_CastKind.CX_CK_IntToOCLSampler => "IntToOCLSampler",
                    _ => CastKind.ToString().Substring(6),
                };
            }
        }

        public CXSourceRange CommentRange => clang.Cursor_getCommentRange(this);

        public CXCompletionString CompletionString => (CXCompletionString)clang.getCursorCompletionString(this);

        public CX_CXXAccessSpecifier CXXAccessSpecifier => clang.getCXXAccessSpecifier(this);

        public bool CXXConstructor_IsConvertingConstructor => clang.CXXConstructor_isConvertingConstructor(this) != 0;

        public bool CXXConstructor_IsCopyConstructor => clang.CXXConstructor_isCopyConstructor(this) != 0;

        public bool CXXConstructor_IsDefaultConstructor => clang.CXXConstructor_isDefaultConstructor(this) != 0;

        public bool CXXConstructor_IsMoveConstructor => clang.CXXConstructor_isMoveConstructor(this) != 0;

        public bool CXXField_IsMutable => clang.CXXField_isMutable(this) != 0;

        public CXStringSet* CXXManglings => clang.Cursor_getCXXManglings(this);

        public bool CXXMethod_IsConst => clang.CXXMethod_isConst(this) != 0;

        public bool CXXMethod_IsDefaulted => clang.CXXMethod_isDefaulted(this) != 0;

        public bool CXXMethod_IsPureVirtual => clang.CXXMethod_isPureVirtual(this) != 0;

        public bool CXXMethod_IsStatic => clang.CXXMethod_isStatic(this) != 0;

        public bool CXXMethod_IsVirtual => clang.CXXMethod_isVirtual(this) != 0;

        public bool CXXRecord_IsAbstract => clang.CXXRecord_isAbstract(this) != 0;

        public CX_DeclKind DeclKind => clangsharp.Cursor_getDeclKind(this);

        public string DeclKindSpelling
        {
            get
            {
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstObjCImpl == CX_DeclKind.CX_DeclKind_ObjCCategoryImpl);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastObjCImpl == CX_DeclKind.CX_DeclKind_ObjCImplementation);
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstObjCContainer == CX_DeclKind.CX_DeclKind_ObjCCategory);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastObjCContainer == CX_DeclKind.CX_DeclKind_ObjCProtocol);
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstRedeclarableTemplate == CX_DeclKind.CX_DeclKind_ClassTemplate);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastRedeclarableTemplate == CX_DeclKind.CX_DeclKind_VarTemplate);
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstTemplate == CX_DeclKind.CX_DeclKind_BuiltinTemplate);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastTemplate == CX_DeclKind.CX_DeclKind_TemplateTemplateParm);
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstClassTemplateSpecialization == CX_DeclKind.CX_DeclKind_ClassTemplateSpecialization);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastClassTemplateSpecialization == CX_DeclKind.CX_DeclKind_ClassTemplatePartialSpecialization);
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstCXXRecord == CX_DeclKind.CX_DeclKind_CXXRecord);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastCXXRecord == CX_DeclKind.CX_DeclKind_ClassTemplatePartialSpecialization);
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstRecord == CX_DeclKind.CX_DeclKind_Record);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastRecord == CX_DeclKind.CX_DeclKind_ClassTemplatePartialSpecialization);
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstTag == CX_DeclKind.CX_DeclKind_Enum);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastTag == CX_DeclKind.CX_DeclKind_ClassTemplatePartialSpecialization);
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstTypedefName == CX_DeclKind.CX_DeclKind_ObjCTypeParam);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastTypedefName == CX_DeclKind.CX_DeclKind_Typedef);
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstType == CX_DeclKind.CX_DeclKind_Enum);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastType == CX_DeclKind.CX_DeclKind_UnresolvedUsingTypename);
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstUsingShadow == CX_DeclKind.CX_DeclKind_UsingShadow);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastUsingShadow == CX_DeclKind.CX_DeclKind_ConstructorUsingShadow);
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstField == CX_DeclKind.CX_DeclKind_Field);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastField == CX_DeclKind.CX_DeclKind_ObjCIvar);
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstCXXMethod == CX_DeclKind.CX_DeclKind_CXXMethod);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastCXXMethod == CX_DeclKind.CX_DeclKind_CXXDestructor);
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstFunction == CX_DeclKind.CX_DeclKind_Function);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastFunction == CX_DeclKind.CX_DeclKind_CXXDestructor);
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstVarTemplateSpecialization == CX_DeclKind.CX_DeclKind_VarTemplateSpecialization);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastVarTemplateSpecialization == CX_DeclKind.CX_DeclKind_VarTemplatePartialSpecialization);
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstVar == CX_DeclKind.CX_DeclKind_Var);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastVar == CX_DeclKind.CX_DeclKind_VarTemplatePartialSpecialization);
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstDeclarator == CX_DeclKind.CX_DeclKind_Field);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastDeclarator == CX_DeclKind.CX_DeclKind_VarTemplatePartialSpecialization);
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstValue == CX_DeclKind.CX_DeclKind_Binding);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastValue == CX_DeclKind.CX_DeclKind_UnresolvedUsingValue);
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstNamed == CX_DeclKind.CX_DeclKind_Label);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastNamed == CX_DeclKind.CX_DeclKind_UnresolvedUsingValue);
                Debug.Assert(CX_DeclKind.CX_DeclKind_FirstDecl == CX_DeclKind.CX_DeclKind_AccessSpec);
                Debug.Assert(CX_DeclKind.CX_DeclKind_LastDecl == CX_DeclKind.CX_DeclKind_TranslationUnit);

                return DeclKind switch
                {
                    CX_DeclKind.CX_DeclKind_Invalid => "Invalid",
                    CX_DeclKind.CX_DeclKind_AccessSpec => "AccessSpec",
                    CX_DeclKind.CX_DeclKind_Block => "Block",
                    CX_DeclKind.CX_DeclKind_Captured => "Captured",
                    CX_DeclKind.CX_DeclKind_ClassScopeFunctionSpecialization => "ClassScopeFunctionSpecialization",
                    CX_DeclKind.CX_DeclKind_Empty => "Empty",
                    CX_DeclKind.CX_DeclKind_Export => "Export",
                    CX_DeclKind.CX_DeclKind_ExternCContext => "ExternCContext",
                    CX_DeclKind.CX_DeclKind_FileScopeAsm => "FileScopeAsm",
                    CX_DeclKind.CX_DeclKind_Friend => "Friend",
                    CX_DeclKind.CX_DeclKind_FriendTemplate => "FriendTemplate",
                    CX_DeclKind.CX_DeclKind_Import => "Import",
                    CX_DeclKind.CX_DeclKind_LifetimeExtendedTemporary => "LifetimeExtendedTemporary",
                    CX_DeclKind.CX_DeclKind_LinkageSpec => "LinkageSpec",
                    CX_DeclKind.CX_DeclKind_Label => "Label",
                    CX_DeclKind.CX_DeclKind_Namespace => "Namespace",
                    CX_DeclKind.CX_DeclKind_NamespaceAlias => "NamespaceAlias",
                    CX_DeclKind.CX_DeclKind_ObjCCompatibleAlias => "ObjCCompatibleAlias",
                    CX_DeclKind.CX_DeclKind_ObjCCategory => "ObjCCategory",
                    CX_DeclKind.CX_DeclKind_ObjCCategoryImpl => "ObjCCategoryImpl",
                    CX_DeclKind.CX_DeclKind_ObjCImplementation => "ObjCImplementation",
                    CX_DeclKind.CX_DeclKind_ObjCInterface => "ObjCInterface",
                    CX_DeclKind.CX_DeclKind_ObjCProtocol => "ObjCProtocol",
                    CX_DeclKind.CX_DeclKind_ObjCMethod => "ObjCMethod",
                    CX_DeclKind.CX_DeclKind_ObjCProperty => "ObjCProperty",
                    CX_DeclKind.CX_DeclKind_BuiltinTemplate => "BuiltinTemplate",
                    CX_DeclKind.CX_DeclKind_Concept => "Concept",
                    CX_DeclKind.CX_DeclKind_ClassTemplate => "ClassTemplate",
                    CX_DeclKind.CX_DeclKind_FunctionTemplate => "FunctionTemplate",
                    CX_DeclKind.CX_DeclKind_TypeAliasTemplate => "TypeAliasTemplate",
                    CX_DeclKind.CX_DeclKind_VarTemplate => "VarTemplate",
                    CX_DeclKind.CX_DeclKind_TemplateTemplateParm => "TemplateTemplateParm",
                    CX_DeclKind.CX_DeclKind_Enum => "Enum",
                    CX_DeclKind.CX_DeclKind_Record => "Record",
                    CX_DeclKind.CX_DeclKind_CXXRecord => "CXXRecord",
                    CX_DeclKind.CX_DeclKind_ClassTemplateSpecialization => "ClassTemplateSpecialization",
                    CX_DeclKind.CX_DeclKind_ClassTemplatePartialSpecialization => "ClassTemplatePartialSpecialization",
                    CX_DeclKind.CX_DeclKind_TemplateTypeParm => "TemplateTypeParm",
                    CX_DeclKind.CX_DeclKind_ObjCTypeParam => "ObjCTypeParam",
                    CX_DeclKind.CX_DeclKind_TypeAlias => "TypeAlias",
                    CX_DeclKind.CX_DeclKind_Typedef => "Typedef",
                    CX_DeclKind.CX_DeclKind_UnresolvedUsingTypename => "UnresolvedUsingTypename",
                    CX_DeclKind.CX_DeclKind_Using => "Using",
                    CX_DeclKind.CX_DeclKind_UsingDirective => "UsingDirective",
                    CX_DeclKind.CX_DeclKind_UsingPack => "UsingPack",
                    CX_DeclKind.CX_DeclKind_UsingShadow => "UsingShadow",
                    CX_DeclKind.CX_DeclKind_ConstructorUsingShadow => "ConstructorUsingShadow",
                    CX_DeclKind.CX_DeclKind_Binding => "Binding",
                    CX_DeclKind.CX_DeclKind_Field => "Field",
                    CX_DeclKind.CX_DeclKind_ObjCAtDefsField => "ObjCAtDefsField",
                    CX_DeclKind.CX_DeclKind_ObjCIvar => "ObjCIvar",
                    CX_DeclKind.CX_DeclKind_Function => "Function",
                    CX_DeclKind.CX_DeclKind_CXXDeductionGuide => "CXXDeductionGuide",
                    CX_DeclKind.CX_DeclKind_CXXMethod => "CXXMethod",
                    CX_DeclKind.CX_DeclKind_CXXConstructor => "CXXConstructor",
                    CX_DeclKind.CX_DeclKind_CXXConversion => "CXXConversion",
                    CX_DeclKind.CX_DeclKind_CXXDestructor => "CXXDestructor",
                    CX_DeclKind.CX_DeclKind_MSProperty => "MSProperty",
                    CX_DeclKind.CX_DeclKind_NonTypeTemplateParm => "NonTypeTemplateParm",
                    CX_DeclKind.CX_DeclKind_Var => "Var",
                    CX_DeclKind.CX_DeclKind_Decomposition => "Decomposition",
                    CX_DeclKind.CX_DeclKind_ImplicitParam => "ImplicitParam",
                    CX_DeclKind.CX_DeclKind_OMPCapturedExpr => "OMPCapturedExpr",
                    CX_DeclKind.CX_DeclKind_ParmVar => "ParmVar",
                    CX_DeclKind.CX_DeclKind_VarTemplateSpecialization => "VarTemplateSpecialization",
                    CX_DeclKind.CX_DeclKind_VarTemplatePartialSpecialization => "VarTemplatePartialSpecialization",
                    CX_DeclKind.CX_DeclKind_EnumConstant => "EnumConstant",
                    CX_DeclKind.CX_DeclKind_IndirectField => "IndirectField",
                    CX_DeclKind.CX_DeclKind_OMPDeclareMapper => "OMPDeclareMapper",
                    CX_DeclKind.CX_DeclKind_OMPDeclareReduction => "OMPDeclareReduction",
                    CX_DeclKind.CX_DeclKind_UnresolvedUsingValue => "UnresolvedUsingValue",
                    CX_DeclKind.CX_DeclKind_OMPAllocate => "OMPAllocate",
                    CX_DeclKind.CX_DeclKind_OMPRequires => "OMPRequires",
                    CX_DeclKind.CX_DeclKind_OMPThreadPrivate => "OMPThreadPrivate",
                    CX_DeclKind.CX_DeclKind_ObjCPropertyImpl => "ObjCPropertyImpl",
                    CX_DeclKind.CX_DeclKind_PragmaComment => "PragmaComment",
                    CX_DeclKind.CX_DeclKind_PragmaDetectMismatch => "PragmaDetectMismatch",
                    CX_DeclKind.CX_DeclKind_RequiresExprBody => "RequiresExprBody",
                    CX_DeclKind.CX_DeclKind_StaticAssert => "StaticAssert",
                    CX_DeclKind.CX_DeclKind_TranslationUnit => "TranslationUnit",
                    _ => Kind.ToString().Substring(12),
                };
            }
        }

        public CXString DeclObjCTypeEncoding => clang.getDeclObjCTypeEncoding(this);

        public CXCursor Definition => clang.getCursorDefinition(this);

        public CXString DisplayName => clang.getCursorDisplayName(this);

        public ulong EnumConstantDeclUnsignedValue => clang.getEnumConstantDeclUnsignedValue(this);

        public long EnumConstantDeclValue => clang.getEnumConstantDeclValue(this);

        public CXType EnumDecl_IntegerType => clang.getEnumDeclIntegerType(this);

        public bool EnumDecl_IsScoped => clang.EnumDecl_isScoped(this) != 0;

        public CXEvalResult Evaluate => (CXEvalResult)clang.Cursor_Evaluate(this);

        public int ExceptionSpecificationType => clang.getCursorExceptionSpecificationType(this);

        public CXSourceRange Extent => clang.getCursorExtent(this);

        public int FieldDeclBitWidth => clang.getFieldDeclBitWidth(this);

        public bool HasAttrs => clang.Cursor_hasAttrs(this) != 0;

        public uint Hash => clang.hashCursor(this);

        public CXType IBOutletCollectionType => clang.getIBOutletCollectionType(this);

        public CXFile IncludedFile => (CXFile)clang.getIncludedFile(this);

        public bool IsAnonymous => clang.Cursor_isAnonymous(this) != 0;

        public bool IsAnonymousRecordDecl => clang.Cursor_isAnonymousRecordDecl(this) != 0;

        public bool IsAttribute => clang.isAttribute(Kind) != 0;

        public bool IsBitField => clang.Cursor_isBitField(this) != 0;

        public bool IsCanonical => Equals(CanonicalCursor);

        public bool IsDeclaration => clang.isDeclaration(Kind) != 0;

        public bool IsDefinition => clang.isCursorDefinition(this) != 0;

        public bool IsDynamicCall => clang.Cursor_isDynamicCall(this) != 0;

        public bool IsExpression => clang.isExpression(Kind) != 0;

        public bool IsFunctionInlined => clang.Cursor_isFunctionInlined(this) != 0;

        public bool IsInlineNamespace => clang.Cursor_isInlineNamespace(this) != 0;

        public bool IsInvalid => clang.isInvalid(Kind) != 0;

        public bool IsInvalidDeclaration => clang.isInvalidDeclaration(this) != 0;

        public bool IsNull => clang.Cursor_isNull(this) != 0;

        public bool IsMacroBuiltIn => clang.Cursor_isMacroBuiltin(this) != 0;

        public bool IsMacroFunctionLike => clang.Cursor_isMacroFunctionLike(this) != 0;

        public bool IsObjCOptional => clang.Cursor_isObjCOptional(this) != 0;

        public bool IsPreprocessing => clang.isPreprocessing(Kind) != 0;

        public bool IsReference => clang.isReference(Kind) != 0;

        public bool IsStatement => clang.isStatement(Kind) != 0;

        public bool IsTranslationUnit => clang.isTranslationUnit(Kind) != 0;

        public bool IsUnexposed => clang.isUnexposed(Kind) != 0;

        public bool IsVariadic => clang.Cursor_isVariadic(this) != 0;

        public bool IsVirtualBase => clang.isVirtualBase(this) != 0;

        public CXCursorKind Kind => clang.getCursorKind(this);

        public CXString KindSpelling => clang.getCursorKindSpelling(Kind);

        public CXLanguageKind Language => clang.getCursorLanguage(this);

        public CXCursor LexicalParent => clang.getCursorLexicalParent(this);

        public CXLinkageKind Linkage => clang.getCursorLinkage(this);

        public CXSourceLocation Location => clang.getCursorLocation(this);

        public CXString Mangling => clang.Cursor_getMangling(this);

        public CXModule Module => (CXModule)clang.Cursor_getModule(this);

        public int NumArguments => clang.Cursor_getNumArguments(this);

        public uint NumOverloadedDecls => clang.getNumOverloadedDecls(this);

        public int NumTemplateArguments => clang.Cursor_getNumTemplateArguments(this);

        public CXObjCDeclQualifierKind ObjCDeclQualifiers => (CXObjCDeclQualifierKind)clang.Cursor_getObjCDeclQualifiers(this);

        public CXStringSet* ObjCManglings => clang.Cursor_getObjCManglings(this);

        public CXString ObjCPropertyGetterName => clang.Cursor_getObjCPropertyGetterName(this);

        public CXString ObjCPropertySetterName => clang.Cursor_getObjCPropertySetterName(this);

        public int ObjCSelectorIndex => clang.Cursor_getObjCSelectorIndex(this);

        public long OffsetOfField => clang.Cursor_getOffsetOfField(this);

        public ReadOnlySpan<CXCursor> OverriddenCursors
        {
            get
            {
                CXCursor* overridden;
                uint numOverridden;

                clang.getOverriddenCursors(this, &overridden, &numOverridden);
                return new ReadOnlySpan<CXCursor>(overridden, (int)numOverridden);
            }
        }

        public CXComment ParsedComment => clang.Cursor_getParsedComment(this);

        public CXPrintingPolicy PrintingPolicy => (CXPrintingPolicy)clang.getCursorPrintingPolicy(this);

        public CXString RawCommentText => clang.Cursor_getRawCommentText(this);

        public CXSourceRange RawExtent => clangsharp.getCursorExtent(this);

        public CXType ReceiverType => !IsExpression ? default : clang.Cursor_getReceiverType(this);

        public CXCursor Referenced => clang.getCursorReferenced(this);

        public CXType ResultType => clang.getCursorResultType(this);

        public CXCursor SemanticParent => clang.getCursorSemanticParent(this);

        public CXCursor SpecializedCursorTemplate => clang.getSpecializedCursorTemplate(this);

        public CXString Spelling => clang.getCursorSpelling(this);

        public CX_StmtClass StmtClass => clangsharp.Cursor_getStmtClass(this);

        public string StmtClassSpelling
        {
            get
            {
                Debug.Assert(CX_StmtClass.CX_StmtClass_FirstAsmStmt == CX_StmtClass.CX_StmtClass_GCCAsmStmt);
                Debug.Assert(CX_StmtClass.CX_StmtClass_LastAsmStmt == CX_StmtClass.CX_StmtClass_MSAsmStmt);
                Debug.Assert(CX_StmtClass.CX_StmtClass_FirstOMPLoopDirective == CX_StmtClass.CX_StmtClass_OMPDistributeDirective);
                Debug.Assert(CX_StmtClass.CX_StmtClass_LastOMPLoopDirective == CX_StmtClass.CX_StmtClass_OMPTeamsDistributeSimdDirective);
                Debug.Assert(CX_StmtClass.CX_StmtClass_FirstOMPExecutableDirective == CX_StmtClass.CX_StmtClass_OMPAtomicDirective);
                Debug.Assert(CX_StmtClass.CX_StmtClass_LastOMPExecutableDirective == CX_StmtClass.CX_StmtClass_OMPTeamsDirective);
                Debug.Assert(CX_StmtClass.CX_StmtClass_FirstSwitchCase == CX_StmtClass.CX_StmtClass_CaseStmt);
                Debug.Assert(CX_StmtClass.CX_StmtClass_LastSwitchCase == CX_StmtClass.CX_StmtClass_DefaultStmt);
                Debug.Assert(CX_StmtClass.CX_StmtClass_FirstAbstractConditionalOperator == CX_StmtClass.CX_StmtClass_BinaryConditionalOperator);
                Debug.Assert(CX_StmtClass.CX_StmtClass_LastAbstractConditionalOperator == CX_StmtClass.CX_StmtClass_ConditionalOperator);
                Debug.Assert(CX_StmtClass.CX_StmtClass_FirstBinaryOperator == CX_StmtClass.CX_StmtClass_BinaryOperator);
                Debug.Assert(CX_StmtClass.CX_StmtClass_LastBinaryOperator == CX_StmtClass.CX_StmtClass_CompoundAssignOperator);
                Debug.Assert(CX_StmtClass.CX_StmtClass_FirstCXXConstructExpr == CX_StmtClass.CX_StmtClass_CXXConstructExpr);
                Debug.Assert(CX_StmtClass.CX_StmtClass_LastCXXConstructExpr == CX_StmtClass.CX_StmtClass_CXXTemporaryObjectExpr);
                Debug.Assert(CX_StmtClass.CX_StmtClass_FirstCallExpr == CX_StmtClass.CX_StmtClass_CallExpr);
                Debug.Assert(CX_StmtClass.CX_StmtClass_LastCallExpr == CX_StmtClass.CX_StmtClass_UserDefinedLiteral);
                Debug.Assert(CX_StmtClass.CX_StmtClass_FirstCXXNamedCastExpr == CX_StmtClass.CX_StmtClass_CXXConstCastExpr);
                Debug.Assert(CX_StmtClass.CX_StmtClass_LastCXXNamedCastExpr == CX_StmtClass.CX_StmtClass_CXXStaticCastExpr);
                Debug.Assert(CX_StmtClass.CX_StmtClass_FirstExplicitCastExpr == CX_StmtClass.CX_StmtClass_BuiltinBitCastExpr);
                Debug.Assert(CX_StmtClass.CX_StmtClass_LastExplicitCastExpr == CX_StmtClass.CX_StmtClass_ObjCBridgedCastExpr);
                Debug.Assert(CX_StmtClass.CX_StmtClass_FirstCastExpr == CX_StmtClass.CX_StmtClass_BuiltinBitCastExpr);
                Debug.Assert(CX_StmtClass.CX_StmtClass_LastCastExpr == CX_StmtClass.CX_StmtClass_ImplicitCastExpr);
                Debug.Assert(CX_StmtClass.CX_StmtClass_FirstCoroutineSuspendExpr == CX_StmtClass.CX_StmtClass_CoawaitExpr);
                Debug.Assert(CX_StmtClass.CX_StmtClass_LastCoroutineSuspendExpr == CX_StmtClass.CX_StmtClass_CoyieldExpr);
                Debug.Assert(CX_StmtClass.CX_StmtClass_FirstFullExpr == CX_StmtClass.CX_StmtClass_ConstantExpr);
                Debug.Assert(CX_StmtClass.CX_StmtClass_LastFullExpr == CX_StmtClass.CX_StmtClass_ExprWithCleanups);
                Debug.Assert(CX_StmtClass.CX_StmtClass_FirstOverloadExpr == CX_StmtClass.CX_StmtClass_UnresolvedLookupExpr);
                Debug.Assert(CX_StmtClass.CX_StmtClass_LastOverloadExpr == CX_StmtClass.CX_StmtClass_UnresolvedMemberExpr);
                Debug.Assert(CX_StmtClass.CX_StmtClass_FirstExpr == CX_StmtClass.CX_StmtClass_BinaryConditionalOperator);
                Debug.Assert(CX_StmtClass.CX_StmtClass_LastExpr == CX_StmtClass.CX_StmtClass_VAArgExpr);
                Debug.Assert(CX_StmtClass.CX_StmtClass_FirstValueStmt == CX_StmtClass.CX_StmtClass_AttributedStmt);
                Debug.Assert(CX_StmtClass.CX_StmtClass_LastValueStmt == CX_StmtClass.CX_StmtClass_LabelStmt);
                Debug.Assert(CX_StmtClass.CX_StmtClass_FirstStmt == CX_StmtClass.CX_StmtClass_GCCAsmStmt);
                Debug.Assert(CX_StmtClass.CX_StmtClass_LastStmt == CX_StmtClass.CX_StmtClass_WhileStmt);

                return StmtClass switch
                {
                    CX_StmtClass.CX_StmtClass_Invalid => "Invalid",
                    CX_StmtClass.CX_StmtClass_GCCAsmStmt => "GCCAsmStmt",
                    CX_StmtClass.CX_StmtClass_MSAsmStmt => "MSAsmStmt",
                    CX_StmtClass.CX_StmtClass_BreakStmt => "BreakStmt",
                    CX_StmtClass.CX_StmtClass_CXXCatchStmt => "CXXCatchStmt",
                    CX_StmtClass.CX_StmtClass_CXXForRangeStmt => "CXXForRangeStmt",
                    CX_StmtClass.CX_StmtClass_CXXTryStmt => "CXXTryStmt",
                    CX_StmtClass.CX_StmtClass_CapturedStmt => "CapturedStmt",
                    CX_StmtClass.CX_StmtClass_CompoundStmt => "CompoundStmt",
                    CX_StmtClass.CX_StmtClass_ContinueStmt => "ContinueStmt",
                    CX_StmtClass.CX_StmtClass_CoreturnStmt => "CoreturnStmt",
                    CX_StmtClass.CX_StmtClass_CoroutineBodyStmt => "CoroutineBodyStmt",
                    CX_StmtClass.CX_StmtClass_DeclStmt => "DeclStmt",
                    CX_StmtClass.CX_StmtClass_DoStmt => "DoStmt",
                    CX_StmtClass.CX_StmtClass_ForStmt => "ForStmt",
                    CX_StmtClass.CX_StmtClass_GotoStmt => "GotoStmt",
                    CX_StmtClass.CX_StmtClass_IfStmt => "IfStmt",
                    CX_StmtClass.CX_StmtClass_IndirectGotoStmt => "IndirectGotoStmt",
                    CX_StmtClass.CX_StmtClass_MSDependentExistsStmt => "MSDependentExistsStmt",
                    CX_StmtClass.CX_StmtClass_NullStmt => "NullStmt",
                    CX_StmtClass.CX_StmtClass_OMPAtomicDirective => "OMPAtomicDirective",
                    CX_StmtClass.CX_StmtClass_OMPBarrierDirective => "OMPBarrierDirective",
                    CX_StmtClass.CX_StmtClass_OMPCancelDirective => "OMPCancelDirective",
                    CX_StmtClass.CX_StmtClass_OMPCancellationPointDirective => "OMPCancellationPointDirective",
                    CX_StmtClass.CX_StmtClass_OMPCriticalDirective => "OMPCriticalDirective",
                    CX_StmtClass.CX_StmtClass_OMPFlushDirective => "OMPFlushDirective",
                    CX_StmtClass.CX_StmtClass_OMPDistributeDirective => "OMPDistributeDirective",
                    CX_StmtClass.CX_StmtClass_OMPDistributeParallelForDirective => "OMPDistributeParallelForDirective",
                    CX_StmtClass.CX_StmtClass_OMPDistributeParallelForSimdDirective => "OMPDistributeParallelForSimdDirective",
                    CX_StmtClass.CX_StmtClass_OMPDistributeSimdDirective => "OMPDistributeSimdDirective",
                    CX_StmtClass.CX_StmtClass_OMPForDirective => "OMPForDirective",
                    CX_StmtClass.CX_StmtClass_OMPForSimdDirective => "OMPForSimdDirective",
                    CX_StmtClass.CX_StmtClass_OMPMasterTaskLoopDirective => "OMPMasterTaskLoopDirective",
                    CX_StmtClass.CX_StmtClass_OMPMasterTaskLoopSimdDirective => "OMPMasterTaskLoopSimdDirective",
                    CX_StmtClass.CX_StmtClass_OMPParallelForDirective => "OMPParallelForDirective",
                    CX_StmtClass.CX_StmtClass_OMPParallelForSimdDirective => "OMPParallelForSimdDirective",
                    CX_StmtClass.CX_StmtClass_OMPParallelMasterTaskLoopDirective => "OMPParallelMasterTaskLoopDirective",
                    CX_StmtClass.CX_StmtClass_OMPParallelMasterTaskLoopSimdDirective => "OMPParallelMasterTaskLoopSimdDirective",
                    CX_StmtClass.CX_StmtClass_OMPSimdDirective => "OMPSimdDirective",
                    CX_StmtClass.CX_StmtClass_OMPTargetParallelForSimdDirective => "OMPTargetParallelForSimdDirective",
                    CX_StmtClass.CX_StmtClass_OMPTargetSimdDirective => "OMPTargetSimdDirective",
                    CX_StmtClass.CX_StmtClass_OMPTargetTeamsDistributeDirective => "OMPTargetTeamsDistributeDirective",
                    CX_StmtClass.CX_StmtClass_OMPTargetTeamsDistributeParallelForDirective => "OMPTargetTeamsDistributeParallelForDirective",
                    CX_StmtClass.CX_StmtClass_OMPTargetTeamsDistributeParallelForSimdDirective => "OMPTargetTeamsDistributeParallelForSimdDirective",
                    CX_StmtClass.CX_StmtClass_OMPTargetTeamsDistributeSimdDirective => "OMPTargetTeamsDistributeSimdDirective",
                    CX_StmtClass.CX_StmtClass_OMPTaskLoopDirective => "OMPTaskLoopDirective",
                    CX_StmtClass.CX_StmtClass_OMPTaskLoopSimdDirective => "OMPTaskLoopSimdDirective",
                    CX_StmtClass.CX_StmtClass_OMPTeamsDistributeDirective => "OMPTeamsDistributeDirective",
                    CX_StmtClass.CX_StmtClass_OMPTeamsDistributeParallelForDirective => "OMPTeamsDistributeParallelForDirective",
                    CX_StmtClass.CX_StmtClass_OMPTeamsDistributeParallelForSimdDirective => "OMPTeamsDistributeParallelForSimdDirective",
                    CX_StmtClass.CX_StmtClass_OMPTeamsDistributeSimdDirective => "OMPTeamsDistributeSimdDirective",
                    CX_StmtClass.CX_StmtClass_OMPMasterDirective => "OMPMasterDirective",
                    CX_StmtClass.CX_StmtClass_OMPOrderedDirective => "OMPOrderedDirective",
                    CX_StmtClass.CX_StmtClass_OMPParallelDirective => "OMPParallelDirective",
                    CX_StmtClass.CX_StmtClass_OMPParallelMasterDirective => "OMPParallelMasterDirective",
                    CX_StmtClass.CX_StmtClass_OMPParallelSectionsDirective => "OMPParallelSectionsDirective",
                    CX_StmtClass.CX_StmtClass_OMPSectionDirective => "OMPSectionDirective",
                    CX_StmtClass.CX_StmtClass_OMPSectionsDirective => "OMPSectionsDirective",
                    CX_StmtClass.CX_StmtClass_OMPSingleDirective => "OMPSingleDirective",
                    CX_StmtClass.CX_StmtClass_OMPTargetDataDirective => "OMPTargetDataDirective",
                    CX_StmtClass.CX_StmtClass_OMPTargetDirective => "OMPTargetDirective",
                    CX_StmtClass.CX_StmtClass_OMPTargetEnterDataDirective => "OMPTargetEnterDataDirective",
                    CX_StmtClass.CX_StmtClass_OMPTargetExitDataDirective => "OMPTargetExitDataDirective",
                    CX_StmtClass.CX_StmtClass_OMPTargetParallelDirective => "OMPTargetParallelDirective",
                    CX_StmtClass.CX_StmtClass_OMPTargetParallelForDirective => "OMPTargetParallelForDirective",
                    CX_StmtClass.CX_StmtClass_OMPTargetTeamsDirective => "OMPTargetTeamsDirective",
                    CX_StmtClass.CX_StmtClass_OMPTargetUpdateDirective => "OMPTargetUpdateDirective",
                    CX_StmtClass.CX_StmtClass_OMPTaskDirective => "OMPTaskDirective",
                    CX_StmtClass.CX_StmtClass_OMPTaskgroupDirective => "OMPTaskgroupDirective",
                    CX_StmtClass.CX_StmtClass_OMPTaskwaitDirective => "OMPTaskwaitDirective",
                    CX_StmtClass.CX_StmtClass_OMPTaskyieldDirective => "OMPTaskyieldDirective",
                    CX_StmtClass.CX_StmtClass_OMPTeamsDirective => "OMPTeamsDirective",
                    CX_StmtClass.CX_StmtClass_ObjCAtCatchStmt => "ObjCAtCatchStmt",
                    CX_StmtClass.CX_StmtClass_ObjCAtFinallyStmt => "ObjCAtFinallyStmt",
                    CX_StmtClass.CX_StmtClass_ObjCAtSynchronizedStmt => "ObjCAtSynchronizedStmt",
                    CX_StmtClass.CX_StmtClass_ObjCAtThrowStmt => "ObjCAtThrowStmt",
                    CX_StmtClass.CX_StmtClass_ObjCAtTryStmt => "ObjCAtTryStmt",
                    CX_StmtClass.CX_StmtClass_ObjCAutoreleasePoolStmt => "ObjCAutoreleasePoolStmt",
                    CX_StmtClass.CX_StmtClass_ObjCForCollectionStmt => "ObjCForCollectionStmt",
                    CX_StmtClass.CX_StmtClass_ReturnStmt => "ReturnStmt",
                    CX_StmtClass.CX_StmtClass_SEHExceptStmt => "SEHExceptStmt",
                    CX_StmtClass.CX_StmtClass_SEHFinallyStmt => "SEHFinallyStmt",
                    CX_StmtClass.CX_StmtClass_SEHLeaveStmt => "SEHLeaveStmt",
                    CX_StmtClass.CX_StmtClass_SEHTryStmt => "SEHTryStmt",
                    CX_StmtClass.CX_StmtClass_CaseStmt => "CaseStmt",
                    CX_StmtClass.CX_StmtClass_DefaultStmt => "DefaultStmt",
                    CX_StmtClass.CX_StmtClass_SwitchStmt => "SwitchStmt",
                    CX_StmtClass.CX_StmtClass_AttributedStmt => "AttributedStmt",
                    CX_StmtClass.CX_StmtClass_BinaryConditionalOperator => "BinaryConditionalOperator",
                    CX_StmtClass.CX_StmtClass_ConditionalOperator => "ConditionalOperator",
                    CX_StmtClass.CX_StmtClass_AddrLabelExpr => "AddrLabelExpr",
                    CX_StmtClass.CX_StmtClass_ArrayInitIndexExpr => "ArrayInitIndexExpr",
                    CX_StmtClass.CX_StmtClass_ArrayInitLoopExpr => "ArrayInitLoopExpr",
                    CX_StmtClass.CX_StmtClass_ArraySubscriptExpr => "ArraySubscriptExpr",
                    CX_StmtClass.CX_StmtClass_ArrayTypeTraitExpr => "ArrayTypeTraitExpr",
                    CX_StmtClass.CX_StmtClass_AsTypeExpr => "AsTypeExpr",
                    CX_StmtClass.CX_StmtClass_AtomicExpr => "AtomicExpr",
                    CX_StmtClass.CX_StmtClass_BinaryOperator => "BinaryOperator",
                    CX_StmtClass.CX_StmtClass_CompoundAssignOperator => "CompoundAssignOperator",
                    CX_StmtClass.CX_StmtClass_BlockExpr => "BlockExpr",
                    CX_StmtClass.CX_StmtClass_CXXBindTemporaryExpr => "CXXBindTemporaryExpr",
                    CX_StmtClass.CX_StmtClass_CXXBoolLiteralExpr => "CXXBoolLiteralExpr",
                    CX_StmtClass.CX_StmtClass_CXXConstructExpr => "CXXConstructExpr",
                    CX_StmtClass.CX_StmtClass_CXXTemporaryObjectExpr => "CXXTemporaryObjectExpr",
                    CX_StmtClass.CX_StmtClass_CXXDefaultArgExpr => "CXXDefaultArgExpr",
                    CX_StmtClass.CX_StmtClass_CXXDefaultInitExpr => "CXXDefaultInitExpr",
                    CX_StmtClass.CX_StmtClass_CXXDeleteExpr => "CXXDeleteExpr",
                    CX_StmtClass.CX_StmtClass_CXXDependentScopeMemberExpr => "CXXDependentScopeMemberExpr",
                    CX_StmtClass.CX_StmtClass_CXXFoldExpr => "CXXFoldExpr",
                    CX_StmtClass.CX_StmtClass_CXXInheritedCtorInitExpr => "CXXInheritedCtorInitExpr",
                    CX_StmtClass.CX_StmtClass_CXXNewExpr => "CXXNewExpr",
                    CX_StmtClass.CX_StmtClass_CXXNoexceptExpr => "CXXNoexceptExpr",
                    CX_StmtClass.CX_StmtClass_CXXNullPtrLiteralExpr => "CXXNullPtrLiteralExpr",
                    CX_StmtClass.CX_StmtClass_CXXPseudoDestructorExpr => "CXXPseudoDestructorExpr",
                    CX_StmtClass.CX_StmtClass_CXXRewrittenBinaryOperator => "CXXRewrittenBinaryOperator",
                    CX_StmtClass.CX_StmtClass_CXXScalarValueInitExpr => "CXXScalarValueInitExpr",
                    CX_StmtClass.CX_StmtClass_CXXStdInitializerListExpr => "CXXStdInitializerListExpr",
                    CX_StmtClass.CX_StmtClass_CXXThisExpr => "CXXThisExpr",
                    CX_StmtClass.CX_StmtClass_CXXThrowExpr => "CXXThrowExpr",
                    CX_StmtClass.CX_StmtClass_CXXTypeidExpr => "CXXTypeidExpr",
                    CX_StmtClass.CX_StmtClass_CXXUnresolvedConstructExpr => "CXXUnresolvedConstructExpr",
                    CX_StmtClass.CX_StmtClass_CXXUuidofExpr => "CXXUuidofExpr",
                    CX_StmtClass.CX_StmtClass_CallExpr => "CallExpr",
                    CX_StmtClass.CX_StmtClass_CUDAKernelCallExpr => "CUDAKernelCallExpr",
                    CX_StmtClass.CX_StmtClass_CXXMemberCallExpr => "CXXMemberCallExpr",
                    CX_StmtClass.CX_StmtClass_CXXOperatorCallExpr => "CXXOperatorCallExpr",
                    CX_StmtClass.CX_StmtClass_UserDefinedLiteral => "UserDefinedLiteral",
                    CX_StmtClass.CX_StmtClass_BuiltinBitCastExpr => "BuiltinBitCastExpr",
                    CX_StmtClass.CX_StmtClass_CStyleCastExpr => "CStyleCastExpr",
                    CX_StmtClass.CX_StmtClass_CXXFunctionalCastExpr => "CXXFunctionalCastExpr",
                    CX_StmtClass.CX_StmtClass_CXXConstCastExpr => "CXXConstCastExpr",
                    CX_StmtClass.CX_StmtClass_CXXDynamicCastExpr => "CXXDynamicCastExpr",
                    CX_StmtClass.CX_StmtClass_CXXReinterpretCastExpr => "CXXReinterpretCastExpr",
                    CX_StmtClass.CX_StmtClass_CXXStaticCastExpr => "CXXStaticCastExpr",
                    CX_StmtClass.CX_StmtClass_ObjCBridgedCastExpr => "ObjCBridgedCastExpr",
                    CX_StmtClass.CX_StmtClass_ImplicitCastExpr => "ImplicitCastExpr",
                    CX_StmtClass.CX_StmtClass_CharacterLiteral => "CharacterLiteral",
                    CX_StmtClass.CX_StmtClass_ChooseExpr => "ChooseExpr",
                    CX_StmtClass.CX_StmtClass_CompoundLiteralExpr => "CompoundLiteralExpr",
                    CX_StmtClass.CX_StmtClass_ConceptSpecializationExpr => "ConceptSpecializationExpr",
                    CX_StmtClass.CX_StmtClass_ConvertVectorExpr => "ConvertVectorExpr",
                    CX_StmtClass.CX_StmtClass_CoawaitExpr => "CoawaitExpr",
                    CX_StmtClass.CX_StmtClass_CoyieldExpr => "CoyieldExpr",
                    CX_StmtClass.CX_StmtClass_DeclRefExpr => "DeclRefExpr",
                    CX_StmtClass.CX_StmtClass_DependentCoawaitExpr => "DependentCoawaitExpr",
                    CX_StmtClass.CX_StmtClass_DependentScopeDeclRefExpr => "DependentScopeDeclRefExpr",
                    CX_StmtClass.CX_StmtClass_DesignatedInitExpr => "DesignatedInitExpr",
                    CX_StmtClass.CX_StmtClass_DesignatedInitUpdateExpr => "DesignatedInitUpdateExpr",
                    CX_StmtClass.CX_StmtClass_ExpressionTraitExpr => "ExpressionTraitExpr",
                    CX_StmtClass.CX_StmtClass_ExtVectorElementExpr => "ExtVectorElementExpr",
                    CX_StmtClass.CX_StmtClass_FixedPointLiteral => "FixedPointLiteral",
                    CX_StmtClass.CX_StmtClass_FloatingLiteral => "FloatingLiteral",
                    CX_StmtClass.CX_StmtClass_ConstantExpr => "ConstantExpr",
                    CX_StmtClass.CX_StmtClass_ExprWithCleanups => "ExprWithCleanups",
                    CX_StmtClass.CX_StmtClass_FunctionParmPackExpr => "FunctionParmPackExpr",
                    CX_StmtClass.CX_StmtClass_GNUNullExpr => "GNUNullExpr",
                    CX_StmtClass.CX_StmtClass_GenericSelectionExpr => "GenericSelectionExpr",
                    CX_StmtClass.CX_StmtClass_ImaginaryLiteral => "ImaginaryLiteral",
                    CX_StmtClass.CX_StmtClass_ImplicitValueInitExpr => "ImplicitValueInitExpr",
                    CX_StmtClass.CX_StmtClass_InitListExpr => "InitListExpr",
                    CX_StmtClass.CX_StmtClass_IntegerLiteral => "IntegerLiteral",
                    CX_StmtClass.CX_StmtClass_LambdaExpr => "LambdaExpr",
                    CX_StmtClass.CX_StmtClass_MSPropertyRefExpr => "MSPropertyRefExpr",
                    CX_StmtClass.CX_StmtClass_MSPropertySubscriptExpr => "MSPropertySubscriptExpr",
                    CX_StmtClass.CX_StmtClass_MaterializeTemporaryExpr => "MaterializeTemporaryExpr",
                    CX_StmtClass.CX_StmtClass_MemberExpr => "MemberExpr",
                    CX_StmtClass.CX_StmtClass_NoInitExpr => "NoInitExpr",
                    CX_StmtClass.CX_StmtClass_OMPArraySectionExpr => "OMPArraySectionExpr",
                    CX_StmtClass.CX_StmtClass_ObjCArrayLiteral => "ObjCArrayLiteral",
                    CX_StmtClass.CX_StmtClass_ObjCAvailabilityCheckExpr => "ObjCAvailabilityCheckExpr",
                    CX_StmtClass.CX_StmtClass_ObjCBoolLiteralExpr => "ObjCBoolLiteralExpr",
                    CX_StmtClass.CX_StmtClass_ObjCBoxedExpr => "ObjCBoxedExpr",
                    CX_StmtClass.CX_StmtClass_ObjCDictionaryLiteral => "ObjCDictionaryLiteral",
                    CX_StmtClass.CX_StmtClass_ObjCEncodeExpr => "ObjCEncodeExpr",
                    CX_StmtClass.CX_StmtClass_ObjCIndirectCopyRestoreExpr => "ObjCIndirectCopyRestoreExpr",
                    CX_StmtClass.CX_StmtClass_ObjCIsaExpr => "ObjCIsaExpr",
                    CX_StmtClass.CX_StmtClass_ObjCIvarRefExpr => "ObjCIvarRefExpr",
                    CX_StmtClass.CX_StmtClass_ObjCMessageExpr => "ObjCMessageExpr",
                    CX_StmtClass.CX_StmtClass_ObjCPropertyRefExpr => "ObjCPropertyRefExpr",
                    CX_StmtClass.CX_StmtClass_ObjCProtocolExpr => "ObjCProtocolExpr",
                    CX_StmtClass.CX_StmtClass_ObjCSelectorExpr => "ObjCSelectorExpr",
                    CX_StmtClass.CX_StmtClass_ObjCStringLiteral => "ObjCStringLiteral",
                    CX_StmtClass.CX_StmtClass_ObjCSubscriptRefExpr => "ObjCSubscriptRefExpr",
                    CX_StmtClass.CX_StmtClass_OffsetOfExpr => "OffsetOfExpr",
                    CX_StmtClass.CX_StmtClass_OpaqueValueExpr => "OpaqueValueExpr",
                    CX_StmtClass.CX_StmtClass_UnresolvedLookupExpr => "UnresolvedLookupExpr",
                    CX_StmtClass.CX_StmtClass_UnresolvedMemberExpr => "UnresolvedMemberExpr",
                    CX_StmtClass.CX_StmtClass_PackExpansionExpr => "PackExpansionExpr",
                    CX_StmtClass.CX_StmtClass_ParenExpr => "ParenExpr",
                    CX_StmtClass.CX_StmtClass_ParenListExpr => "ParenListExpr",
                    CX_StmtClass.CX_StmtClass_PredefinedExpr => "PredefinedExpr",
                    CX_StmtClass.CX_StmtClass_PseudoObjectExpr => "PseudoObjectExpr",
                    CX_StmtClass.CX_StmtClass_RequiresExpr => "RequiresExpr",
                    CX_StmtClass.CX_StmtClass_ShuffleVectorExpr => "ShuffleVectorExpr",
                    CX_StmtClass.CX_StmtClass_SizeOfPackExpr => "SizeOfPackExpr",
                    CX_StmtClass.CX_StmtClass_SourceLocExpr => "SourceLocExpr",
                    CX_StmtClass.CX_StmtClass_StmtExpr => "StmtExpr",
                    CX_StmtClass.CX_StmtClass_StringLiteral => "StringLiteral",
                    CX_StmtClass.CX_StmtClass_SubstNonTypeTemplateParmExpr => "SubstNonTypeTemplateParmExpr",
                    CX_StmtClass.CX_StmtClass_SubstNonTypeTemplateParmPackExpr => "SubstNonTypeTemplateParmPackExpr",
                    CX_StmtClass.CX_StmtClass_TypeTraitExpr => "TypeTraitExpr",
                    CX_StmtClass.CX_StmtClass_TypoExpr => "TypoExpr",
                    CX_StmtClass.CX_StmtClass_UnaryExprOrTypeTraitExpr => "UnaryExprOrTypeTraitExpr",
                    CX_StmtClass.CX_StmtClass_UnaryOperator => "UnaryOperator",
                    CX_StmtClass.CX_StmtClass_VAArgExpr => "VAArgExpr",
                    CX_StmtClass.CX_StmtClass_LabelStmt => "LabelStmt",
                    CX_StmtClass.CX_StmtClass_WhileStmt => "WhileStmt",
                    _ => StmtClass.ToString().Substring(13),
                };
            }
        }

        public CX_StorageClass StorageClass => clang.Cursor_getStorageClass(this);

        public CXCursorKind TemplateCursorKind => clang.getTemplateCursorKind(this);

        public CXTLSKind TlsKind => clang.getCursorTLSKind(this);

        public CXTranslationUnit TranslationUnit => clang.Cursor_getTranslationUnit(this);

        public CXType Type => clang.getCursorType(this);

        public CXType TypedefDeclUnderlyingType => clang.getTypedefDeclUnderlyingType(this);

        public CX_UnaryOperatorKind UnaryOperatorKind => clangsharp.Cursor_getUnaryOpcode(this);

        public CXString UnaryOperatorKindSpelling => clangsharp.Cursor_getUnaryOpcodeSpelling(UnaryOperatorKind);

        public CXString Usr => clang.getCursorUSR(this);

        public CXVisibilityKind Visibility => clang.getCursorVisibility(this);

        internal string DebuggerDisplayString
        {
            get
            {
                if (AttrKind != CX_AttrKind.CX_AttrKind_Invalid)
                {
                    return $"{AttrKindSpelling}: {this}";
                }
                else if (DeclKind != CX_DeclKind.CX_DeclKind_Invalid)
                {
                    return $"{DeclKindSpelling}: {this}";
                }
                else if (StmtClass != CX_StmtClass.CX_StmtClass_Invalid)
                {
                    var additionalInfo = string.Empty;

                    if (BinaryOperatorKind != CX_BinaryOperatorKind.CX_BO_Invalid)
                    {
                        additionalInfo = $"({BinaryOperatorKindSpelling})";
                    }
                    else if (CastKind != CX_CastKind.CX_CK_Invalid)
                    {
                        additionalInfo = $"({CastKindSpelling})";
                    }
                    else if (UnaryOperatorKind != CX_UnaryOperatorKind.CX_UO_Invalid)
                    {
                        additionalInfo = $"({UnaryOperatorKindSpelling})";
                    }

                    return $"{StmtClassSpelling}: {this}";
                }
                else
                {
                    return $"{KindSpelling}: {this}";
                }
            }
        }

        public static bool operator ==(CXCursor left, CXCursor right) => clang.equalCursors(left, right) != 0;

        public static bool operator !=(CXCursor left, CXCursor right) => clang.equalCursors(left, right) == 0;

        public void DisposeOverriddenCursors(ReadOnlySpan<CXCursor> overridden)
        {
            fixed (CXCursor* pOverridden = overridden)
            {
                clang.disposeOverriddenCursors(pOverridden);
            }
        }

        public override bool Equals(object obj) => (obj is CXCursor other) && Equals(other);

        public bool Equals(CXCursor other) => this == other;

        public CXResult FindReferencesInFile(CXFile file, CXCursorAndRangeVisitor visitor) => clang.findReferencesInFile(this, file, visitor);

        public CXCursor GetArgument(uint index) => clang.Cursor_getArgument(this, index);

        public void GetDefinitionSpellingAndExtent(out string spelling, out uint startLine, out uint startColumn, out uint endLine, out uint endColumn)
        {
            fixed (uint* pStartLine = &startLine)
            fixed (uint* pStartColumn = &startColumn)
            fixed (uint* pEndLine = &endLine)
            fixed (uint* pEndColumn = &endColumn)
            {
                sbyte* startBuf;
                sbyte* endBuf;
                clang.getDefinitionSpellingAndExtent(this, &startBuf, &endBuf, pStartLine, pStartColumn, pEndLine, pEndColumn);
                spelling = new ReadOnlySpan<byte>(startBuf, (int)(endBuf - startBuf)).AsString();
            }
        }

        public override int GetHashCode() => (int)Hash;

        public bool GetIsExternalSymbol(out CXString language, out CXString definedIn, out bool isGenerated)
        {
            fixed (CXString* pLanguage = &language)
            fixed (CXString* pDefinedIn = &definedIn)
            {
                uint isGeneratedOut;
                var result = clang.Cursor_isExternalSymbol(this, pLanguage, pDefinedIn, &isGeneratedOut);
                isGenerated = isGeneratedOut != 0;
                return result != 0;
            }
        }

        public CXObjCPropertyAttrKind GetObjCPropertyAttributes(uint reserved) => (CXObjCPropertyAttrKind)clang.Cursor_getObjCPropertyAttributes(this, reserved);

        public CXCursor GetOverloadedDecl(uint index) => clang.getOverloadedDecl(this, index);

        public int GetPlatformAvailability(out bool alwaysDeprecated, out CXString deprecatedMessage, out bool alwaysUnavailable, out CXString unavailableMessage, Span<CXPlatformAvailability> availability)
        {
            fixed (CXString* pDeprecatedMessage = &deprecatedMessage)
            fixed (CXString* pUnavailableMessage = &unavailableMessage)
            fixed (CXPlatformAvailability* pAvailability = availability)
            {
                int alwaysDeprecatedOut;
                int alwaysUnavailableOut;
                var result = clang.getCursorPlatformAvailability(this, &alwaysDeprecatedOut, pDeprecatedMessage, &alwaysUnavailableOut, pUnavailableMessage, pAvailability, availability.Length);
                alwaysDeprecated = alwaysDeprecatedOut != 0;
                alwaysUnavailable = alwaysUnavailableOut != 0;
                return result;
            }
        }

        public CXString GetPrettyPrinted(CXPrintingPolicy policy) => clang.getCursorPrettyPrinted(this, policy);

        public CXSourceRange GetReferenceNameRange(CXNameRefFlags nameFlags, uint pieceIndex) => clang.getCursorReferenceNameRange(this, (uint)nameFlags, pieceIndex);

        public CXSourceRange GetSpellingNameRange(uint pieceIndex, uint options) => clang.Cursor_getSpellingNameRange(this, pieceIndex, options);

        public CXTemplateArgumentKind GetTemplateArgumentKind(uint i) => clang.Cursor_getTemplateArgumentKind(this, i);

        public CXType GetTemplateArgumentType(uint i) => clang.Cursor_getTemplateArgumentType(this, i);

        public ulong GetTemplateArgumentUnsignedValue(uint i) => clang.Cursor_getTemplateArgumentUnsignedValue(this, i);

        public long GetTemplateArgumentValue(uint i) => clang.Cursor_getTemplateArgumentValue(this, i);

        public override string ToString() => Spelling.ToString();

        public CXChildVisitResult VisitChildren(CXCursorVisitor visitor, CXClientData clientData)
        {
            var pVisitor = Marshal.GetFunctionPointerForDelegate(visitor);
            var result = (CXChildVisitResult)clang.visitChildren(this, pVisitor, clientData);

            GC.KeepAlive(visitor);
            return result;
        }
    }
}
