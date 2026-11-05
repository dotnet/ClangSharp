// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Interop;

public static unsafe class CXModuleCache
{
    public static void Prune(string path, long pruneIntervalSeconds, long pruneAfterSeconds)
    {
        using var marshaledPath = new MarshaledString(path);
        clang.ModuleCache_prune(marshaledPath, pruneIntervalSeconds, pruneAfterSeconds);
    }

    public static void PruneWithCallback(string path, long pruneIntervalSeconds, long pruneAfterSeconds, CXModuleCachePruneCallback? callback, CXClientData clientData = default)
    {
        delegate* unmanaged[Cdecl]<sbyte*, void*, void> pCallback = default;

        if (callback is not null)
        {
            pCallback = (delegate* unmanaged[Cdecl]<sbyte*, void*, void>)Marshal.GetFunctionPointerForDelegate(callback);
        }

        PruneWithCallback(path, pruneIntervalSeconds, pruneAfterSeconds, pCallback, clientData);

        GC.KeepAlive(callback);
    }

    public static void PruneWithCallback(string path, long pruneIntervalSeconds, long pruneAfterSeconds, delegate* unmanaged[Cdecl]<sbyte*, void*, void> callback, CXClientData clientData = default)
    {
        using var marshaledPath = new MarshaledString(path);
        clang.ModuleCache_pruneWithCallback(marshaledPath, pruneIntervalSeconds, pruneAfterSeconds, callback, clientData);
    }
}
