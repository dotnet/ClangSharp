namespace ClangSharp.Test
{
    public enum element_type
    {
        boolean,
        char,
    }

    public enum other_type
    {
        boolean,
        wchar,
    }

    public enum consumer_type
    {
        a = element_type.boolean,
        b = other_type.wchar,
    }
}
