// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: DisableRuntimeMarshalling]

namespace ClangSharp.Interop;

public static unsafe partial class @clang
{
    public static event DllImportResolver? ResolveLibrary;

    static @clang()
    {
        if (!Configuration.DisableResolveLibraryHook)
        {
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OnDllImport);
        }
    }

    private static IntPtr OnDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (TryResolveLibrary(libraryName, assembly, searchPath, out var nativeLibrary))
        {
            return nativeLibrary;
        }

        // On Unix, LoadLibraryHelper ignores the DllImportSearchPath flags, so we
        // manually handle SafeDirectories by trying the application directory first
        // (which SafeDirectories normally includes on Windows but is ignored on Unix),
        // then fall back to the standard system search with the various names the native
        // library can bind to.
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (searchPath.GetValueOrDefault().HasFlag(DllImportSearchPath.SafeDirectories))
            {
                // Try the names using AppContext.BaseDirectory
                if (TryResolveClang(libraryName, AppContext.BaseDirectory, out nativeLibrary))
                {
                    return nativeLibrary;
                }
            }

            // Try the standard variations via system search
            if (TryResolveClang(libraryName, null, out nativeLibrary))
            {
                return nativeLibrary;
            }
        }

        return IntPtr.Zero;
    }

    private static bool TryResolveClang(string libraryName, string? baseDirectory, out IntPtr nativeLibrary)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var path = (baseDirectory is not null ? Path.Combine(baseDirectory, libraryName) : libraryName) + ".dylib";
            return NativeLibrary.TryLoad(path, out nativeLibrary);
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            if (baseDirectory is not null)
            {
                return NativeLibrary.TryLoad(Path.Combine(baseDirectory, libraryName + ".so.20"), out nativeLibrary)
                    || NativeLibrary.TryLoad(Path.Combine(baseDirectory, libraryName + "-20"), out nativeLibrary)
                    || NativeLibrary.TryLoad(Path.Combine(baseDirectory, libraryName + ".so.1"), out nativeLibrary);
            }
            else
            {
                return NativeLibrary.TryLoad(libraryName + ".so.20", out nativeLibrary)
                    || NativeLibrary.TryLoad(libraryName + "-20", out nativeLibrary)
                    || NativeLibrary.TryLoad(libraryName + ".so.1", out nativeLibrary);
            }
        }

        nativeLibrary = IntPtr.Zero;
        return false;
    }

    private static bool TryResolveLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath, out IntPtr nativeLibrary)
    {
        var resolveLibrary = ResolveLibrary;

        if (resolveLibrary is not null)
        {
            var resolvers = resolveLibrary.GetInvocationList().Cast<DllImportResolver>();

            foreach (DllImportResolver resolver in resolvers)
            {
                nativeLibrary = resolver(libraryName, assembly, searchPath);

                if (nativeLibrary != IntPtr.Zero)
                {
                    return true;
                }
            }
        }

        nativeLibrary = IntPtr.Zero;
        return false;
    }
}
