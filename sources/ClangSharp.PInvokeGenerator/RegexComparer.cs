// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ClangSharp;

internal class RegexComparer : IComparer<Regex>
{
    public int Compare(Regex? x, Regex? y) => string.CompareOrdinal(x?.ToString(), y?.ToString());
}
