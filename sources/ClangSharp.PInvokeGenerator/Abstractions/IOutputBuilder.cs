namespace ClangSharp.Abstractions;

internal partial interface IOutputBuilder
{
    bool IsTestOutput { get; }
    string Name { get; }
    string Extension { get; }
}
