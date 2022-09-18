// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace ClangSharp.Interop;

public unsafe partial struct CXTUResourceUsageEntry
{
    public string Name
    {
        get
        {
            var pName = clang.getTUResourceUsageName(kind);

            if (pName is null)
            {
                return string.Empty;
            }

            return SpanExtensions.AsString(pName);
        }
    }

    public override string ToString() => Name;
}
