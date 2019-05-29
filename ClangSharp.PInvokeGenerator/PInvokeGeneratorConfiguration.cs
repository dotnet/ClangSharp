using System;
using System.Collections.Generic;

namespace ClangSharp
{
    public sealed class PInvokeGeneratorConfiguration
    {
        private const string DefaultMethodClassName = "Methods";

        private readonly PInvokeGeneratorConfigurationOptions _options;

        public PInvokeGeneratorConfiguration(string libraryPath, string namespaceName, string outputLocation, PInvokeGeneratorConfigurationOptions options = PInvokeGeneratorConfigurationOptions.None, string[] excludedNames = null, string methodClassName = null, string methodPrefixToStrip = null, IReadOnlyDictionary<string, string> remappedNames = null)
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

            if (remappedNames is null)
            {
                remappedNames = new Dictionary<string, string>();
            }

            _options = options;

            ExcludedNames = excludedNames;
            LibraryPath = libraryPath;
            MethodClassName = methodClassName;
            MethodPrefixToStrip = methodPrefixToStrip;
            Namespace = namespaceName;
            OutputLocation = outputLocation;
            RemappedNames = remappedNames;
        }

        public string[] ExcludedNames { get; }

        public bool GenerateMultipleFiles => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles);

        public bool GenerateUnsafeCode => _options.HasFlag(PInvokeGeneratorConfigurationOptions.GenerateUnsafeCode);

        public string LibraryPath { get;}

        public string MethodClassName { get; }

        public string MethodPrefixToStrip { get;}

        public string Namespace { get; }

        public string OutputLocation { get; }

        public IReadOnlyDictionary<string, string> RemappedNames { get; }
    }
}
