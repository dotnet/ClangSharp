// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ClangSharp;

internal class QualifiedNameComparer : IEqualityComparer<string>, IAlternateEqualityComparer<ReadOnlySpan<char>, string>
{
    public static readonly QualifiedNameComparer Default = new QualifiedNameComparer();

    public string Create(ReadOnlySpan<char> alternate) => alternate.ToString();

    public bool Equals(ReadOnlySpan<char> alternate, string other) => Equals(alternate, other.AsSpan());

    public static bool Equals(ReadOnlySpan<char> alternate, ReadOnlySpan<char> other)
    {
        while ((alternate.Length != 0) && (other.Length != 0))
        {
            if (alternate.StartsWith("::", StringComparison.Ordinal))
            {
                if (other.StartsWith('.'))
                {
                    alternate = alternate[2..];
                    other = other[1..];
                }
                else
                {
                    return false;
                }
            }
            else if (alternate.StartsWith('.'))
            {
                if (other.StartsWith("::", StringComparison.Ordinal))
                {
                    alternate = alternate[1..];
                    other = other[2..];
                }
                else
                {
                    return false;
                }
            }

            var prefixLength = alternate.CommonPrefixLength(other);

            if (prefixLength == 0)
            {
                break;
            }

            alternate = alternate[prefixLength..];
            other = other[prefixLength..];
        }

        return (alternate.Length == 0) && (other.Length == 0);
    }

    public bool Equals(string? x, string? y) => Equals(x.AsSpan(), y.AsSpan());

    public int GetHashCode(ReadOnlySpan<char> alternate)
    {
        var hashCode = new HashCode();

        while (alternate.Length != 0)
        {
            var part = alternate;
            var separatorLength = 0;

            var colonSeparatorIndex = part.IndexOf("::", StringComparison.Ordinal);

            if (colonSeparatorIndex != -1)
            {
                part = part[..colonSeparatorIndex];
                separatorLength = 2;
            }

            var dotSeparatorIndex = part.IndexOf('.');

            if (dotSeparatorIndex != -1)
            {
                part = part[..dotSeparatorIndex];
                separatorLength = 1;
            }

            hashCode.Add(string.GetHashCode(part, StringComparison.Ordinal));

            if (separatorLength != 0)
            {
                hashCode.Add('.');
                alternate = alternate[(part.Length + separatorLength)..];
            }
            else
            {
                alternate = alternate[part.Length..];
            }
        }

        return hashCode.ToHashCode();
    }

    public int GetHashCode([DisallowNull] string obj) => GetHashCode(obj.AsSpan());
}
