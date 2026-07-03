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

        // When invoked as a dotnet tool (ClangSharpPInvokeGenerator), native libraries
        // are co-located with the executable in the NuGet cache but aren't found
        // by the default resolver on Unix (default dlopen only searches system paths
        // and LD_LIBRARY_PATH), so we explicitly try the application base directory.
        // NativeLibrary.TryLoad with an absolute path does not apply platform-specific
        // name mangling (lib prefix, .so/.dylib/.dll suffix), so we add it explicitly.
        var suffix = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".dll"
                    : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? ".dylib"
                    : ".so";

        if (NativeLibrary.TryLoad(Path.Combine(AppContext.BaseDirectory, libraryName + suffix), out nativeLibrary))
        {
            return nativeLibrary;
        }

        return IntPtr.Zero;
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
