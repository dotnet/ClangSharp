using System.Runtime.InteropServices;

namespace ClangSharp.Test;

public partial struct Point
{
    public int x;

    public int y;
}

public enum Color
{
    Red,
    Green,
}

public static partial class Methods
{
    [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    public static extern void MyFunction();
}
