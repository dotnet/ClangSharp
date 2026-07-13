using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [Obsolete("Use Bar() or Baz() instead")]
        public static extern void MyFunction();
    }
}
