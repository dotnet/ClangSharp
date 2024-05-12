// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ClangSharp.Interop;

public unsafe struct MarshaledString : IDisposable
{
    public MarshaledString(string? input)
    {
        int valueLength;
        sbyte* pValue;

        if (input is null)
        {
            valueLength = 0;
            pValue = null;
        }
        else
        {
            var maxValueLength = Encoding.UTF8.GetMaxByteCount(input.Length);
            pValue = (sbyte*)NativeMemory.Alloc((uint)maxValueLength + 1);
            valueLength = Encoding.UTF8.GetBytes(input, new Span<byte>(pValue, maxValueLength));
            pValue[valueLength] = 0;
        }

        Length = valueLength;
        Value = pValue;
    }

    public MarshaledString(ReadOnlySpan<char> input)
    {
        int valueLength;
        sbyte* pValue;

        var maxValueLength = Encoding.UTF8.GetMaxByteCount(input.Length);
        pValue = (sbyte*)NativeMemory.Alloc((uint)maxValueLength + 1);
        valueLength = Encoding.UTF8.GetBytes(input, new Span<byte>(pValue, maxValueLength));
        pValue[valueLength] = 0;

        Length = valueLength;
        Value = pValue;
    }

    public readonly ReadOnlySpan<byte> AsSpan() => new ReadOnlySpan<byte>(Value, Length);

    public int Length { get; private set; }

    public sbyte* Value { get; private set; }

    public void Dispose()
    {
        if (Value != null)
        {
            NativeMemory.Free(Value);
            Value = null;
            Length = 0;
        }
    }

    public static implicit operator sbyte*(in MarshaledString value) => value.Value;

    public override readonly string ToString()
    {
        var span = new ReadOnlySpan<byte>(Value, Length);
        return span.AsString();
    }
}
