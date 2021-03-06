using System.Collections.Generic;

namespace ClangSharp.Abstractions
{
    public partial interface IOutputBuilder
    {
        bool IsTestOutput { get; }
        string Name { get; }
        string Extension { get; }
    }
}
