using System;
using System.Collections.Generic;
using System.IO;

namespace ClangSharp
{
    public sealed class PInvokeGeneratorConfiguration
    {
        private const string DefaultMethodClassName = "Methods";

        private readonly Dictionary<string, string> _remappedNames;
        private readonly PInvokeGeneratorConfigurationOptions _options;

        public PInvokeGeneratorConfiguration(string libraryPath, string namespaceName, string outputLocation, PInvokeGeneratorConfigurationOptions options = PInvokeGeneratorConfigurationOptions.None, string[] excludedNames = null, string headerFile = null, string methodClassName = null, string methodPrefixToStrip = null, IReadOnlyDictionary<string, string> remappedNames = null, string[] traversalNames = null)
        {
            if (excludedNames is null)
            {
                excludedNames = Array.Empty<string>();
            }

            if (string.IsNullOrWhiteSpace(libraryPath))
            {
                throw new ArgumentNullException(nameof(libraryPath));
            }

            if (string.IsNullOrWhiteSpace(methodClassName))
            {
                methodClassName = DefaultMethodClassName;
            }

            if (string.IsNullOrWhiteSpace(methodPrefixToStrip))
            {
                methodPrefixToStrip = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(namespaceName))
            {
                throw new ArgumentNullException(nameof(namespaceName));
            }

            if (string.IsNullOrWhiteSpace(outputLocation))
            {
                throw new ArgumentNullException(nameof(outputLocation));
            }

            if (traversalNames is null)
            {
                traversalNames = Array.Empty<string>();
            }

            _options = options;

            ExcludedNames = excludedNames;
            HeaderText = string.IsNullOrWhiteSpace(headerFile) ? string.Empty : File.ReadAllText(headerFile);
            LibraryPath = libraryPath;
            MethodClassName = methodClassName;
            MethodPrefixToStrip = methodPrefixToStrip;
            Namespace = namespaceName;
            OutputLocation = Path.GetFullPath(outputLocation);
            TraversalNames = traversalNames;

            if (!_options.HasFlag(PInvokeGeneratorConfigurationOptions.NoDefaultRemappings))
            {
                _remappedNames = new Dictionary<string, string>()
                {
                    ["intptr_t"] = "IntPtr",
                    ["ptrdiff_t"] = "IntPtr",
                    ["size_t"] = "UIntPtr",
                    ["uintptr_t"] = "UIntPtr",
                };
            }

            if (remappedNames != null)
            {
                foreach (var remappedName in remappedNames)
                {
                    // Use the indexer, rather than Add, so that any
                    // default mappings can be overwritten if desired.
                    _remappedNames[remappedName.Key] = remappedName.Value;
                }
            }
        }

        public string[] ExcludedNames { get; }

        public bool GenerateMultipleFiles => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles);

        public bool GenerateUnixTypes => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateUnixTypes);

        public string HeaderText { get; }

        public string LibraryPath { get;}

        public string MethodClassName { get; }

        public string MethodPrefixToStrip { get;}

        public string Namespace { get; }

        public string OutputLocation { get; }

        public IReadOnlyDictionary<string, string> RemappedNames => _remappedNames;

        public string[] TraversalNames { get; }
    }
}
