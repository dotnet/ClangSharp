namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        public int count;

        [CppAttributeList("_Field_size_full_(count)")]
        [NativeAnnotation("_Field_size_full_(count)")]
        public int* data;
    }
}
