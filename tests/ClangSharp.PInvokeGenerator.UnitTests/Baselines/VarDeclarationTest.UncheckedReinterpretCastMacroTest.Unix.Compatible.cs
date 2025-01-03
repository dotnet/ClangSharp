namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        [NativeTypeName("#define Macro1 reinterpret_cast<int*>(-1)")]
        public static readonly int* Macro1 = unchecked((int*)(-1));
    }
}
