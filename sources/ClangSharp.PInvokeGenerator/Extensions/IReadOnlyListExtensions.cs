// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;

namespace ClangSharp
{
    internal static class IReadOnlyListExtensions
    {
        public static int IndexOf<T>(this IReadOnlyList<T> self, T value)
        {
            var comparer = EqualityComparer<T>.Default;

            for (var i = 0; i < self.Count; i++)
            {
                if (comparer.Equals(self[i], value))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
