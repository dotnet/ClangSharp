namespace ClangSharp.Test
{
    public unsafe partial struct MethodsManualImports
    {
        [NativeTypeName("const int")]
        public int* kMyConstInt;

        public int* MyMutableInt;

        public void** MyMutablePointer;
    }
}
