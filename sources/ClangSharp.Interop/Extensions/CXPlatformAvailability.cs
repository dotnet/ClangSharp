// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe partial struct CXPlatformAvailability : IDisposable
{
    public void Dispose()
    {
        fixed (CXPlatformAvailability* pThis = &this)
        {
            clang.disposeCXPlatformAvailability(pThis);
        }
    }
}
