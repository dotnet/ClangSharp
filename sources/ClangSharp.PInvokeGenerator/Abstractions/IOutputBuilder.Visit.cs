namespace ClangSharp.Abstractions
{
    public partial interface IOutputBuilder
    {
        void WriteCustomAttribute(string attribute);
        void WriteDivider(bool force = false);
        void WriteIid(string iidName, string iidValue);
        void EmitUsingDirective(string directive);
    }
}
