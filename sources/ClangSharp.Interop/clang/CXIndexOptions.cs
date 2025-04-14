// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-20.1.2/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

using System.Runtime.CompilerServices;

namespace ClangSharp.Interop;

public unsafe partial struct CXIndexOptions
{
    [NativeTypeName("unsigned int")]
    public uint Size;

    [NativeTypeName("unsigned char")]
    public byte ThreadBackgroundPriorityForIndexing;

    [NativeTypeName("unsigned char")]
    public byte ThreadBackgroundPriorityForEditing;

    public uint _bitfield;

    [NativeTypeName("unsigned int : 1")]
    public uint ExcludeDeclarationsFromPCH
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get
        {
            return _bitfield & 0x1u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            _bitfield = (_bitfield & ~0x1u) | (value & 0x1u);
        }
    }

    [NativeTypeName("unsigned int : 1")]
    public uint DisplayDiagnostics
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get
        {
            return (_bitfield >> 1) & 0x1u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            _bitfield = (_bitfield & ~(0x1u << 1)) | ((value & 0x1u) << 1);
        }
    }

    [NativeTypeName("unsigned int : 1")]
    public uint StorePreamblesInMemory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get
        {
            return (_bitfield >> 2) & 0x1u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            _bitfield = (_bitfield & ~(0x1u << 2)) | ((value & 0x1u) << 2);
        }
    }

    [NativeTypeName("unsigned int : 13")]
    public uint Anonymous
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get
        {
            return (_bitfield >> 3) & 0x1FFFu;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            _bitfield = (_bitfield & ~(0x1FFFu << 3)) | ((value & 0x1FFFu) << 3);
        }
    }

    [NativeTypeName("const char *")]
    public sbyte* PreambleStoragePath;

    [NativeTypeName("const char *")]
    public sbyte* InvocationEmissionPath;
}
