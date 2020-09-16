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
            IntPtr nativeLibrary;

            if (TryResolveLibrary(libraryName, assembly, searchPath, out nativeLibrary))
            {
                return nativeLibrary;
            }

            if (libraryName.Equals("libclang") && TryResolveClang(assembly, searchPath, out nativeLibrary))
            {
                return nativeLibrary;
            }

            if (libraryName.Equals("libClangSharp") && TryResolveClangSharp(assembly, searchPath, out nativeLibrary))
            {
                return nativeLibrary;
            }

            return IntPtr.Zero;
        }

        private static bool TryResolveClang(Assembly assembly, DllImportSearchPath? searchPath, out IntPtr nativeLibrary)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && NativeLibrary.TryLoad("libclang.so.10", assembly, searchPath, out nativeLibrary))
            {
                return true;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && NativeLibrary.TryLoad("libclang-10", assembly, searchPath, out nativeLibrary))
            {
                return true;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && NativeLibrary.TryLoad("libclang.so.1", assembly, searchPath, out nativeLibrary))
            {
                return true;
            }

            if (NativeLibrary.TryLoad("libclang", assembly, searchPath, out nativeLibrary))
            {
                return true;
            }

            return false;
        }

        private static bool TryResolveClangSharp(Assembly assembly, DllImportSearchPath? searchPath, out IntPtr nativeLibrary)
        {
            return NativeLibrary.TryLoad("libClangSharp", assembly, searchPath, out nativeLibrary);
        }

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
