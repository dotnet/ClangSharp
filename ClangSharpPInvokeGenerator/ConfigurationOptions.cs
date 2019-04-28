using System;
using System.Collections.Generic;

namespace ClangSharpPInvokeGenerator
{
    internal sealed class ConfigurationOptions
    {
        public List<string> ExcludedFunctions { get; } = new List<string>();

        public string LibraryPath { get; set; } = string.Empty;

        public string MethodClassName { get; set; } = "Methods";

        public string MethodPrefixToStrip { get; set; } = string.Empty;

        public string Namespace { get; set; } = string.Empty;
    }
}
