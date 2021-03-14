namespace ClangSharp.Abstractions
{
    public partial interface IOutputBuilder
    {
        void WriteDivider(bool force = false);
        void SuppressDivider();

        void WriteCustomAttribute(string attribute);
        void WriteIid(string iidName, string iidValue);
        void EmitUsingDirective(string directive);
    }
}
