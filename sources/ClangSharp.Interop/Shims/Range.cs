// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System;

internal readonly struct Range : IEquatable<Range>
{
    public Index Start { get; }

    public Index End { get; }

    public Range(Index start, Index end)
    {
        Start = start;
        End = end;
    }

    public override bool Equals(object value) =>
        value is Range r &&
        r.Start.Equals(Start) &&
        r.End.Equals(End);

    public bool Equals(Range other) => other.Start.Equals(Start) && other.End.Equals(End);

    public override int GetHashCode() => HashCode.Combine(Start.GetHashCode(), End.GetHashCode());

    public override string ToString() => Start.ToString() + ".." + End.ToString();

    public static Range StartAt(Index start) => new Range(start, Index.End);

    public static Range EndAt(Index end) => new Range(Index.Start, end);

    public static Range All => new Range(Index.Start, Index.End);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (int Offset, int Length) GetOffsetAndLength(int length)
    {
        int start;
        var startIndex = Start;
        start = startIndex.IsFromEnd ? length - startIndex.Value : startIndex.Value;

        int end;
        var endIndex = End;
        end = endIndex.IsFromEnd ? length - endIndex.Value : endIndex.Value;

        return (uint)end > (uint)length || (uint)start > (uint)end
            ? throw new ArgumentOutOfRangeException(nameof(length))
            : (start, end - start);
    }
}
