namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        [NativeTypeName("#define Macro1 ((int*) -1)")]
        public static readonly int* Macro1 = unchecked((int*)(-1));
    }
}
