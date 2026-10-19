namespace ClangSharp.Test
{
    /// <include file='MyStruct.xml' path='doc/member[@name="MyStruct"]/*' />
    public partial struct MyStruct
    {
        /// <include file='MyStruct.xml' path='doc/member[@name="MyStruct.value"]/*' />
        public int value;
    }

    public static partial class Methods
    {
        /// <include file='Methods.xml' path='doc/member[@name="Methods.MyConstant"]/*' />
        [NativeTypeName("const MyStruct")]
        public static readonly MyStruct MyConstant = new MyStruct
        {
            value = 0,
        };
    }
}
