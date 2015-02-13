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

    public partial struct CXType
    {
        public override string ToString()
        {
            return Methods.clang_getTypeSpelling(this).ToString();
        }
    }

    public partial struct CXCursor
    {
        public override string ToString()
        {
            return Methods.clang_getCursorSpelling(this).ToString();
        }
    }

    public partial struct CXDiagnostic
    {
        public override string ToString()
        {
            return Methods.clang_getDiagnosticSpelling(this).ToString();
        }
    }
}