namespace ClangSharp
{
    using System.Runtime.InteropServices;

    public partial struct CXString
    {
        public override string ToString()
        {
            string retval = Marshal.PtrToStringAnsi(this.data);
            Methods.clang_disposeString(this);
            return retval;
        }
    } 
}