// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe ref struct MarshaledStringArray
{
    private MarshaledString[]? _values;

    public MarshaledStringArray(ReadOnlySpan<string> inputs)
    {
        if (inputs.Length == 0)
        {
            _values = null;
        }
        else
        {
            _values = new MarshaledString[inputs.Length];

            for (var i = 0; i < inputs.Length; i++)
            {
                _values[i] = new MarshaledString(inputs[i]);
            }
        }
    }

    public ReadOnlySpan<MarshaledString> Values => _values;

    public void Dispose()
    {
        if (_values != null)
        {
            for (var i = 0; i < _values.Length; i++)
            {
                _values[i].Dispose();
            }

            _values = null;
        }
    }

    public void Fill(sbyte** pDestination)
    {
        if (_values != null)
        {
            for (var i = 0; i < _values.Length; i++)
            {
                pDestination[i] = Values[i];
            }
        }
    }
}
