// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-21.1.8/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public enum CXCallingConv
{
    CXCallingConv_Default = 0,
    CXCallingConv_C = 1,
    CXCallingConv_X86StdCall = 2,
    CXCallingConv_X86FastCall = 3,
    CXCallingConv_X86ThisCall = 4,
    CXCallingConv_X86Pascal = 5,
    CXCallingConv_AAPCS = 6,
    CXCallingConv_AAPCS_VFP = 7,
    CXCallingConv_X86RegCall = 8,
    CXCallingConv_IntelOclBicc = 9,
    CXCallingConv_Win64 = 10,
    CXCallingConv_X86_64Win64 = CXCallingConv_Win64,
    CXCallingConv_X86_64SysV = 11,
    CXCallingConv_X86VectorCall = 12,
    CXCallingConv_Swift = 13,
    CXCallingConv_PreserveMost = 14,
    CXCallingConv_PreserveAll = 15,
    CXCallingConv_AArch64VectorCall = 16,
    CXCallingConv_SwiftAsync = 17,
    CXCallingConv_AArch64SVEPCS = 18,
    CXCallingConv_M68kRTD = 19,
    CXCallingConv_PreserveNone = 20,
    CXCallingConv_RISCVVectorCall = 21,
    CXCallingConv_RISCVVLSCall_32 = 22,
    CXCallingConv_RISCVVLSCall_64 = 23,
    CXCallingConv_RISCVVLSCall_128 = 24,
    CXCallingConv_RISCVVLSCall_256 = 25,
    CXCallingConv_RISCVVLSCall_512 = 26,
    CXCallingConv_RISCVVLSCall_1024 = 27,
    CXCallingConv_RISCVVLSCall_2048 = 28,
    CXCallingConv_RISCVVLSCall_4096 = 29,
    CXCallingConv_RISCVVLSCall_8192 = 30,
    CXCallingConv_RISCVVLSCall_16384 = 31,
    CXCallingConv_RISCVVLSCall_32768 = 32,
    CXCallingConv_RISCVVLSCall_65536 = 33,
    CXCallingConv_Invalid = 100,
    CXCallingConv_Unexposed = 200,
}
