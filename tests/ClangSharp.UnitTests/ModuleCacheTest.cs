// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using ClangSharp.Interop;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed unsafe class ModuleCacheTest
{
    [Test]
    public void PruneTest()
    {
        var path = Path.Combine(Path.GetTempPath(), $"ClangSharp-{Guid.NewGuid():N}");
        _ = Directory.CreateDirectory(path);

        try
        {
            CXModuleCache.Prune(path, pruneIntervalSeconds: 1, pruneAfterSeconds: 1);

            var modulePath = Path.Combine(path, "module.pcm");
            File.WriteAllText(modulePath, "");
            File.SetLastAccessTimeUtc(modulePath, DateTime.UnixEpoch);

            var timestampPath = Path.Combine(path, "modules.timestamp");
            File.WriteAllText(timestampPath, "");
            File.SetLastWriteTimeUtc(timestampPath, DateTime.UnixEpoch);

            var prunedPaths = new List<string>();
            var callbackData = GCHandle.Alloc(prunedPaths);

            try
            {
                CXModuleCache.PruneWithCallback(path, pruneIntervalSeconds: 1, pruneAfterSeconds: 1, OnPrunedPath, (CXClientData)(void*)GCHandle.ToIntPtr(callbackData));
            }
            finally
            {
                callbackData.Free();
            }

            Assert.That(File.Exists(modulePath), Is.False);
            Assert.That(prunedPaths, Does.Contain(modulePath));
        }
        finally
        {
            Directory.Delete(path, recursive: true);
        }
    }

    private static void OnPrunedPath(sbyte* path, void* userData)
    {
        var length = 0;

        while (path[length] != 0)
        {
            length++;
        }

        var paths = (List<string>?)GCHandle.FromIntPtr((IntPtr)userData).Target;
        paths!.Add(new ReadOnlySpan<byte>((byte*)path, length).AsString());
    }
}
