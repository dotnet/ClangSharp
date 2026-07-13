// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: DisableRuntimeMarshalling]

namespace ClangSharp.Interop;

public static unsafe partial class @clang
{
    public static event DllImportResolver? ResolveLibrary;

    public const int MajorVersion = 21;
    public const int MinorVersion = 1;

    static @clang()
    {
        if (!Configuration.DisableResolveLibraryHook)
        {
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OnDllImport);
        }
    }

    private static IntPtr OnDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        // Let user hooks attempt to resolve first, so they can override the behavior for their app
        if (TryResolveLibrary(libraryName, assembly, searchPath, out var nativeLibrary))
        {
            return nativeLibrary;
        }

        // Then try the normal resolution of the library
        if (TryResolve(libraryName, assembly, searchPath.GetValueOrDefault(), out nativeLibrary))
        {
            return nativeLibrary;
        }

        // Finally, do library specific resolution as applicable
        if (libraryName.Equals("libclang", StringComparison.Ordinal))
        {
            if (TryResolveClang(assembly, searchPath, out nativeLibrary))
            {
                return nativeLibrary;
            }
        }

        return default;
    }

    private static bool TryResolve(string libraryName, Assembly assembly, DllImportSearchPath searchPath, out IntPtr nativeLibrary)
    {
        if (TryResolveFromAppDirectory(libraryName, assembly, searchPath, out nativeLibrary))
        {
            return true;
        }

        return NativeLibrary.TryLoad(libraryName, assembly, searchPath, out nativeLibrary);
    }

    private static bool TryResolveClang(Assembly assembly, DllImportSearchPath? searchPath, out IntPtr nativeLibrary)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return TryResolve($"libclang.so.{MajorVersion}", assembly, searchPath.GetValueOrDefault(), out nativeLibrary)
                || TryResolve($"libclang-{MajorVersion}", assembly, searchPath.GetValueOrDefault(), out nativeLibrary)
                || TryResolve("libclang.so.1", assembly, searchPath.GetValueOrDefault(), out nativeLibrary);
        }

        nativeLibrary = default;
        return false;
    }

    private static bool TryResolveFromAppDirectory(string libraryName, Assembly assembly, DllImportSearchPath searchPath, out IntPtr nativeLibrary)
    {
        nativeLibrary = default;

        // DllImportSearchPath is ignored on non-Windows and the default resolution
        // algorithm does not look in the application directory.
        // We need to look here for any dependencies that are shipped SxS with the
        // application, using the standard variations allowed.
        if (OperatingSystem.IsWindows())
        {
            return false;
        }

        if (((searchPath & (DllImportSearchPath.SafeDirectories | DllImportSearchPath.ApplicationDirectory)) == 0))
        {
            return false;
        }

        if (Path.IsPathFullyQualified(libraryName))
        {
            return false;
        }

        // We use Environment.ProcessPath because that is the location of the executable
        // that launched the process. AppContext.BaseDirectory can be overridden and is
        // rather more the "AssemblyDirectory". This is a subtle distinction, but an important one.
        var applicationDirectory = Path.GetDirectoryName(Environment.ProcessPath);

        if (applicationDirectory is null)
        {
            return false;
        }

        var libraryPath = Path.Combine(applicationDirectory, libraryName);

        if (NativeLibrary.TryLoad(libraryPath, assembly, searchPath, out nativeLibrary))
        {
            return true;
        }

        // We try the given name first, as that is always preferred. Then we try, in order:
        //   * libname
        //   * name.ext
        //   * libname.ext
        //
        // This can result in some redundant lookups, but it's how the built-in logic works
        // for non-relative paths.
        var prefixedLibraryName = "";

        if (!libraryName.Contains(Path.DirectorySeparatorChar, StringComparison.Ordinal))
        {
            prefixedLibraryName = $"lib{libraryName}";
            libraryPath = Path.Combine(applicationDirectory, prefixedLibraryName);

            if (NativeLibrary.TryLoad(libraryPath, assembly, searchPath, out nativeLibrary))
            {
                return true;
            }
        }

        var extension = ".so";

        if (OperatingSystem.IsMacOS() || OperatingSystem.IsIOS() || OperatingSystem.IsTvOS())
        {
            extension = ".dylib";
        }

        libraryPath = Path.Combine(applicationDirectory, $"{libraryName}{extension}");

        if (NativeLibrary.TryLoad(libraryPath, assembly, searchPath, out nativeLibrary))
        {
            return true;
        }

        if (prefixedLibraryName.Length != 0)
        {
            libraryPath = Path.Combine(applicationDirectory, $"{prefixedLibraryName}{extension}");

            if (NativeLibrary.TryLoad(libraryPath, assembly, searchPath, out nativeLibrary))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryResolveLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath, out IntPtr nativeLibrary)
    {
        var resolveLibrary = ResolveLibrary;

        if (resolveLibrary is not null)
        {
            foreach (DllImportResolver resolver in Delegate.EnumerateInvocationList(resolveLibrary))
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
