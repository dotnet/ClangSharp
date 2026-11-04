namespace ClangSharp.Test
{
    /// <include file='MyStruct.xml' path='doc/member[@name="MyStruct"]/*' />
    public partial struct MyStruct
    {
        /// <include file='MyStruct.xml' path='doc/member[@name="MyStruct.MyConstant"]/*' />
        [NativeTypeName("const int")]
        public const int MyConstant = 42;
    }
}
