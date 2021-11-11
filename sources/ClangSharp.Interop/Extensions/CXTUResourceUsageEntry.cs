// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop
{
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

                var span = new ReadOnlySpan<byte>(pName, int.MaxValue);
                return span.Slice(0, span.IndexOf((byte)'\0')).AsString();
            }
        }

        public override string ToString() => Name;
    }
}
