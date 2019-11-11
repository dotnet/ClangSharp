// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ClangSharp.Interop
{
    internal unsafe struct MarshaledString : IDisposable
    {
        public MarshaledString(string input)
        {
            int length;
            IntPtr value;

            if (input is null)
            {
                length = 0;
                value = IntPtr.Zero;
            }
            else
            {
                var valueBytes = (input.Length != 0) ? Encoding.UTF8.GetBytes(input) : Array.Empty<byte>();
                length = valueBytes.Length;
                value = Marshal.AllocHGlobal(length + 1);
                Marshal.Copy(valueBytes, 0, value, length);
                Marshal.WriteByte(value, length, 0);
            }

            Length = length;
            Value = (sbyte*)value;
        }

        public ReadOnlySpan<byte> AsSpan() => new ReadOnlySpan<byte>(Value, Length);

        public int Length { get; private set; }

        public sbyte* Value { get; private set; }

        public void Dispose()
        {
            if (Value != null)
            {
                Marshal.FreeHGlobal((IntPtr)Value);
                Value = null;
                Length = 0;
            }
        }

        public static implicit operator sbyte*(in MarshaledString value) => value.Value;

        public override string ToString()
        {
            var span = new ReadOnlySpan<byte>(Value, Length);
            return span.AsString();
        }
    }
}
