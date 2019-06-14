using System;

namespace ClangSharp.Interop
{
    internal unsafe ref struct MarshaledStringArray
    {
        private MarshaledString[] _values;

        public MarshaledStringArray(ReadOnlySpan<string> inputs)
        {
            if (inputs.Length == 0)
            {
                _values = null;
            }
            else
            {
                _values = new MarshaledString[inputs.Length];

                for (int i = 0; i < inputs.Length; i++)
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
                for (int i = 0; i < _values.Length; i++)
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
                for (int i = 0; i < _values.Length; i++)
                {
                    pDestination[i] = Values[i];
                }
            }
        }
    }
}
