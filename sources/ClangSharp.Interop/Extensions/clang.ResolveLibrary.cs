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
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OnDllImport);
    }

    private static IntPtr OnDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        var result = TryResolveLibrary(libraryName, assembly, searchPath, out var nativeLibrary) ? nativeLibrary
                   : libraryName.Equals("libclang") && TryResolveClang(assembly, searchPath, out nativeLibrary) ? nativeLibrary
                   : libraryName.Equals("libClangSharp") && TryResolveClangSharp(assembly, searchPath, out nativeLibrary) ? nativeLibrary
                   : IntPtr.Zero;

        if (result == IntPtr.Zero)
        {
            Console.WriteLine();
            Console.WriteLine("*****IMPORTANT*****");
            Console.WriteLine($"Failed to resolve {libraryName}.");
            Console.WriteLine("If you are running as a dotnet tool, you may need to manually copy the appropriate DLLs from NuGet due to limitations in the dotnet tool support. Please see https://github.com/dotnet/clangsharp for more details.");
            Console.WriteLine("*****IMPORTANT*****");
            Console.WriteLine();
        }
        return result;
    }

    private static bool TryResolveClang(Assembly assembly, DllImportSearchPath? searchPath, out IntPtr nativeLibrary)
    {
        return (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && NativeLibrary.TryLoad("libclang.so.16", assembly, searchPath, out nativeLibrary))
            || (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && NativeLibrary.TryLoad("libclang-16", assembly, searchPath, out nativeLibrary))
            || (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && NativeLibrary.TryLoad("libclang.so.1", assembly, searchPath, out nativeLibrary))
            || NativeLibrary.TryLoad("libclang", assembly, searchPath, out nativeLibrary);
    }

    private static bool TryResolveClangSharp(Assembly assembly, DllImportSearchPath? searchPath, out IntPtr nativeLibrary) => NativeLibrary.TryLoad("libClangSharp", assembly, searchPath, out nativeLibrary);

    private static bool TryResolveLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath, out IntPtr nativeLibrary)
    {
        var resolveLibrary = ResolveLibrary;

        if (resolveLibrary != null)
        {
            var resolvers = resolveLibrary.GetInvocationList();

            foreach (DllImportResolver resolver in resolvers.Cast<DllImportResolver>())
            {
                try
                {
                    nativeLibrary = resolver(libraryName, assembly, searchPath);
                }
                catch
                {
                    nativeLibrary = IntPtr.Zero;
                }

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
