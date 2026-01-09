// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-21.1.8/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

using System;

namespace ClangSharp.Interop;

[Flags]
public enum ObjCPropertyAttributeKind
{
    NoAttr = 0x00,
    ReadOnly = 0x01,
    Getter = 0x02,
    Assign = 0x04,
    ReadWrite = 0x08,
    Retain = 0x10,
    Copy = 0x20,
    NonAtomic = 0x40,
    Setter = 0x80,
    Atomic = 0x100,
    Weak = 0x200,
    Strong = 0x400,
    UnsafeUnretained = 0x800,
    Nullability = 0x1000,
    NullResettable = 0x2000,
    Class = 0x4000,
    Direct = 0x8000,
}
