using System;

namespace ClangSharp.Interop
{
    internal unsafe struct MarshaledStringArray : IDisposable
    {
        public MarshaledStringArray(string[] inputs)
        {
            if ((inputs is null) || (inputs.Length == 0))
            {
                Count = 0;
                Values = null;
            }
            else
            {
                Count = inputs.Length;
                Values = new MarshaledString[Count];

                for (int i = 0; i < Count; i++)
                {
                    Values[i] = new MarshaledString(inputs[i]);
                }
            }
        }

        public int Count { get; private set; }

        public MarshaledString[] Values { get; private set; }

        public void Dispose()
        {
            if (Values != null)
            {
                for (int i = 0; i < Values.Length; i++)
                {
                    Values[i].Dispose();
                }

                Values = null;
                Count = 0;
            }
        }

        public void Fill(sbyte** pDestination)
        {
            for (int i = 0; i < Count; i++)
            {
                pDestination[i] = Values[i];
            }
        }
    }
}
