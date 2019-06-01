using System;
using System.Collections.Generic;

namespace ClangSharp
{
    public sealed class OutputBuilderFactory
    {
        private readonly Dictionary<string, OutputBuilder> _outputBuilders;

        public OutputBuilderFactory()
        {
            _outputBuilders = new Dictionary<string, OutputBuilder>();
        }

        public IEnumerable<OutputBuilder> OutputBuilders => _outputBuilders.Values;

        public void Clear()
        {
            _outputBuilders.Clear();
        }

        public OutputBuilder Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var outputBuilder = new OutputBuilder(name);
            _outputBuilders.Add(name, outputBuilder);
            return outputBuilder;
        }

        public OutputBuilder GetOutputBuilder(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            return _outputBuilders[name];
        }

        public bool TryGetOutputBuilder(string name, out OutputBuilder outputBuilder)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            return _outputBuilders.TryGetValue(name, out outputBuilder);
        }
    }
}
