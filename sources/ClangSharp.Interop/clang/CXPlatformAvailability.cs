// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-20.1.2/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public partial struct CXPlatformAvailability
{
    public CXString Platform;

    public CXVersion Introduced;

    public CXVersion Deprecated;

    public CXVersion Obsoleted;

    public int Unavailable;

    public CXString Message;
}
