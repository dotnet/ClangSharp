// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Runtime.InteropServices;

[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]

namespace ClangSharp.Interop;

internal static class Configuration
{
    private static readonly bool s_disableResolveLibraryHook = GetAppContextData("ClangSharp.Interop.DisableResolveLibraryHook", defaultValue: false);

    public static bool DisableResolveLibraryHook => s_disableResolveLibraryHook;

    private static bool GetAppContextData(string name, bool defaultValue)
    {
        object? data = AppContext.GetData(name);

        if (data is bool value)
        {
            return value;
        }
        else if ((data is string s) && bool.TryParse(s, out bool result))
        {
            return result;
        }
        else
        {
            return defaultValue;
        }
    }
}
