// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

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

        if (libraryName.Equals("libclang") && TryResolveClang(assembly, searchPath, out nativeLibrary))
        {
            return nativeLibrary;
        }

        return IntPtr.Zero;
    }

    private static bool TryResolveClang(Assembly assembly, DllImportSearchPath? searchPath, out IntPtr nativeLibrary)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return NativeLibrary.TryLoad("libclang.so.16", assembly, searchPath, out nativeLibrary)
                || NativeLibrary.TryLoad("libclang-16", assembly, searchPath, out nativeLibrary)
                || NativeLibrary.TryLoad("libclang.so.1", assembly, searchPath, out nativeLibrary);
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
