namespace ClangSharp.Test
{
    public enum Target
    {
        Target_A,
        Target_B,
        Target_First = Target_A,
        Target_Combined = Target_A | Target_B,
        Target_Computed = 2,
    }
}
