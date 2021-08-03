// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ClangSharp.Interop
{
    public static unsafe partial class clang
    {
        public static event DllImportResolver ResolveLibrary;

        static clang()
        {
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OnDllImport);
        }

        private static IntPtr OnDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {

            return TryResolveLibrary(libraryName, assembly, searchPath, out var nativeLibrary)
                ? nativeLibrary
                : libraryName.Equals("libclang") && TryResolveClang(assembly, searchPath, out nativeLibrary)
                ? nativeLibrary
                : libraryName.Equals("libClangSharp") && TryResolveClangSharp(assembly, searchPath, out nativeLibrary)
                ? nativeLibrary
                : IntPtr.Zero;
        }

        private static bool TryResolveClang(Assembly assembly, DllImportSearchPath? searchPath, out IntPtr nativeLibrary)
        {
            return (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && NativeLibrary.TryLoad("libclang.so.12", assembly, searchPath, out nativeLibrary))
                || (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && NativeLibrary.TryLoad("libclang-12", assembly, searchPath, out nativeLibrary))
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
}
