// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-12.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public enum CXTypeNullabilityKind
    {
        CXTypeNullability_NonNull = 0,
        CXTypeNullability_Nullable = 1,
        CXTypeNullability_Unspecified = 2,
        CXTypeNullability_Invalid = 3,
        CXTypeNullability_NullableResult = 4,
    }
}
