namespace ClangSharp.Test
{
    public static partial class Constants
    {
        /// <include file='Constants.xml' path='doc/member[@name="Constants.MyMacro"]/*' />
        [NativeTypeName("#define MY_MACRO 42")]
        public const int MyMacro = 42;
    }
}
