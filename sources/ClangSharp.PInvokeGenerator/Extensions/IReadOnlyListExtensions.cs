// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

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
