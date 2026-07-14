namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        public int value;

        public int* pointer;

        public int** pointer2;

        public int*** pointer3;

        public int ToInt32()
        {
            return value;
        }

        public int* ToInt32Pointer()
        {
            return pointer;
        }

        public int** ToInt32PointerPointer()
        {
            return pointer2;
        }

        public int*** ToInt32PointerPointerPointer()
        {
            return pointer3;
        }
    }
}
