namespace ClangSharp.Abstractions
{
    public partial interface IOutputBuilder
    {
        void BeginInnerValue();
        void EndInnerValue();

        void BeginInnerCast();
        void WriteCastType(string targetTypeName);
        void EndInnerCast();

        void BeginUnchecked();
        void EndUnchecked();

        void BeginConstant(string accessSpecifier, string typeName, string escapedName, bool isAnonymousEnum);
        void BeginConstantValue();
        void WriteConstantValue(long value);
        void WriteConstantValue(ulong value);
        void EndConstantValue();
        void EndConstant(bool isAnonymousEnum);

        void BeginEnum(string accessSpecifier, string typeName, string escapedName);
        void EndEnum();

        void BeginField(string accessSpecifier, string nativeTypeName, string escapedName, int? offset);
        void WriteFixedCountField(string typeName, string escapedName, string fixedName, string count);
        void WriteRegularField(string typeName, string escapedName);
        void EndField();
    }
}
