// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-21.1.8/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

using System.Runtime.InteropServices;

namespace ClangSharp.Interop;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void CXInclusionVisitor([NativeTypeName("CXFile")] void* included_file, CXSourceLocation* inclusion_stack, [NativeTypeName("unsigned int")] uint include_len, [NativeTypeName("CXClientData")] void* client_data);
