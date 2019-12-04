// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.UnitTests
{
    public sealed class CXXMethodDeclarationTest : PInvokeGeneratorTest
    {
        [Fact]
        public async Task ConstructorTest()
        {
            var inputContents = @"struct MyStruct
{
    int _value;

    MyStruct(int value)
    {
        _value = value;
    }
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int _value;

        public MyStruct(int value)
        {
            _value = value;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task ConstructorWithInitializeTest()
        {
            var inputContents = @"struct MyStruct
{
    int _x;
    int _y;
    int _z;

    MyStruct(int x) : _x(x)
    {
    }

    MyStruct(int x, int y) : _x(x), _y(y)
    {
    }

    MyStruct(int x, int y, int z) : _x(x), _y(y), _z()
    {
    }
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int _x;

        public int _y;

        public int _z;

        public MyStruct(int x)
        {
            _x = x;
        }

        public MyStruct(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public MyStruct(int x, int y, int z)
        {
            _x = x;
            _y = y;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task DestructorTest()
        {
            var inputContents = @"struct MyStruct
{
    ~MyStruct()
    {
    }
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public void Finalize()
        {
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task InstanceTest()
        {
            var inputContents = @"struct MyStruct
{
    void MyVoidMethod();

    int MyInt32Method()
    {
        return 0;
    }

    void* MyVoidStarMethod()
    {
        return nullptr;
    }
};
";
            var callConv = "Cdecl";
            var entryPoint = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "__ZN8MyStruct12MyVoidMethodEv" : "_ZN8MyStruct12MyVoidMethodEv";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (!Environment.Is64BitProcess)
                {
                    callConv = "ThisCall";
                    entryPoint = "?MyVoidMethod@MyStruct@@QAEXXZ";
                }
                else
                {
                    entryPoint = "?MyVoidMethod@MyStruct@@QEAAXXZ";
                }
            }

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        [DllImport(Methods.LibraryPath, CallingConvention = CallingConvention.{callConv}, EntryPoint = ""{entryPoint}"", ExactSpelling = true)]
        public static extern void MyVoidMethod();

        public int MyInt32Method()
        {{
            return 0;
        }}

        [return: NativeTypeName(""void *"")]
        public unsafe void* MyVoidStarMethod()
        {{
            return null;
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task MemberCallTest()
        {
            var inputContents = @"struct MyStruct
{
    int value;

    int MyFunction1()
    {
        return value;
    }

    int MyFunction2()
    {
        return MyFunction1();
    }

    int MyFunction3()
    {
        return this->MyFunction1();
    }
};

int MyFunctionA(MyStruct x)
{
    return x.MyFunction1();
}

int MyFunctionB(MyStruct* x)
{
    return x->MyFunction2();
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int value;

        public int MyFunction1()
        {
            return value;
        }

        public int MyFunction2()
        {
            return MyFunction1();
        }

        public int MyFunction3()
        {
            return this.MyFunction1();
        }
    }

    public static unsafe partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        public static int MyFunctionA(MyStruct x)
        {
            return x.MyFunction1();
        }

        public static int MyFunctionB([NativeTypeName(""MyStruct *"")] MyStruct* x)
        {
            return x->MyFunction2();
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task MemberTest()
        {
            var inputContents = @"struct MyStruct
{
    int value;

    int MyFunction()
    {
        return value;
    }
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int value;

        public int MyFunction()
        {
            return value;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task NewKeywordTest()
        {
            var inputContents = @"struct MyStruct
{
    int Equals() { return 0; }
    int Equals(int obj) { return 0; }
    int Finalize() { return 0; }
    int Finalize(int obj) { return 0; }
    int GetHashCode() { return 0; }
    int GetHashCode(int obj) { return 0; }
    int GetType() { return 0; }
    int GetType(int obj) { return 0; }
    int MemberwiseClone() { return 0; }
    int MemberwiseClone(int obj) { return 0; }
    int ReferenceEquals() { return 0; }
    int ReferenceEquals(int obj) { return 0; }
    int ToString() { return 0; }
    int ToString(int obj) { return 0; }
};";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        public int Equals()
        {{
            return 0;
        }}

        public int Equals(int obj)
        {{
            return 0;
        }}

        public int Finalize()
        {{
            return 0;
        }}

        public int Finalize(int obj)
        {{
            return 0;
        }}

        public new int GetHashCode()
        {{
            return 0;
        }}

        public int GetHashCode(int obj)
        {{
            return 0;
        }}

        public new int GetType()
        {{
            return 0;
        }}

        public int GetType(int obj)
        {{
            return 0;
        }}

        public new int MemberwiseClone()
        {{
            return 0;
        }}

        public int MemberwiseClone(int obj)
        {{
            return 0;
        }}

        public int ReferenceEquals()
        {{
            return 0;
        }}

        public int ReferenceEquals(int obj)
        {{
            return 0;
        }}

        public new int ToString()
        {{
            return 0;
        }}

        public int ToString(int obj)
        {{
            return 0;
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task NewKeywordVirtualTest()
        {
            var inputContents = @"struct MyStruct
{
    virtual int GetType(int obj) = 0;
    virtual int GetType() = 0;
    virtual int GetType(int objA, int objB) = 0;
};";

            var callConv = "Cdecl";
            var callConvAttr = "";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !Environment.Is64BitProcess)
            {
                callConv = "ThisCall";
                callConvAttr = " __attribute__((thiscall))";
            }

            var expectedOutputContents = $@"using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public unsafe partial struct MyStruct
    {{
        public readonly Vtbl* lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        public delegate int _GetType(MyStruct* pThis, int obj);

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        public delegate int _GetType1(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        public delegate int _GetType2(MyStruct* pThis, int objA, int objB);

        public int GetType(int obj)
        {{
            return Marshal.GetDelegateForFunctionPointer<_GetType>(lpVtbl->GetType)((MyStruct*)Unsafe.AsPointer(ref this), obj);
        }}

        public new int GetType()
        {{
            return Marshal.GetDelegateForFunctionPointer<_GetType1>(lpVtbl->GetType1)((MyStruct*)Unsafe.AsPointer(ref this));
        }}

        public int GetType(int objA, int objB)
        {{
            return Marshal.GetDelegateForFunctionPointer<_GetType2>(lpVtbl->GetType2)((MyStruct*)Unsafe.AsPointer(ref this), objA, objB);
        }}

        public partial struct Vtbl
        {{
            [NativeTypeName(""int (int){callConvAttr}"")]
            public new IntPtr GetType;

            [NativeTypeName(""int (){callConvAttr}"")]
            public IntPtr GetType1;

            [NativeTypeName(""int (int, int){callConvAttr}"")]
            public IntPtr GetType2;
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task StaticTest()
        {
            var inputContents = @"struct MyStruct
{
    static void MyVoidMethod();

    static int MyInt32Method()
    {
        return 0;
    }

    static void* MyVoidStarMethod()
    {
        return nullptr;
    }
};
";

            var entryPoint = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "?MyVoidMethod@MyStruct@@SAXXZ" : "_ZN8MyStruct12MyVoidMethodEv";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                entryPoint = $"_{entryPoint}";
            }

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        [DllImport(Methods.LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = ""{entryPoint}"", ExactSpelling = true)]
        public static extern void MyVoidMethod();

        public static int MyInt32Method()
        {{
            return 0;
        }}

        [return: NativeTypeName(""void *"")]
        public static unsafe void* MyVoidStarMethod()
        {{
            return null;
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task ThisTest()
        {
            var inputContents = @"struct MyStruct
{
    int value;

    int MyFunction()
    {
        return this->value;
    }
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int value;

        public int MyFunction()
        {
            return this.value;
        }
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task UnsafeDoesNotImpactDllImportTest()
        {
            var inputContents = @"struct MyStruct
{
    void* MyVoidStarMethod()
    {
        return nullptr;
    }
};

void MyFunction();";

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [return: NativeTypeName(""void *"")]
        public unsafe void* MyVoidStarMethod()
        {
            return null;
        }
    }

    public static partial class Methods
    {
        private const string LibraryPath = ""ClangSharpPInvokeGenerator"";

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = ""MyFunction"", ExactSpelling = true)]
        public static extern void MyFunction();
    }
}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task VirtualCompatibleTest()
        {
            var inputContents = @"struct MyStruct
{
    virtual void MyVoidMethod() = 0;

    virtual char MyInt8Method()
    {
        return 0;
    }

    virtual int MyInt32Method();
};
";

            var callConv = "Cdecl";
            var callConvAttr = "";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !Environment.Is64BitProcess)
            {
                callConv = "ThisCall";
                callConvAttr = " __attribute__((thiscall))";
            }

            var expectedOutputContents = $@"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public unsafe partial struct MyStruct
    {{
        public readonly Vtbl* lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        public delegate void _MyVoidMethod(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        [return: NativeTypeName(""char"")]
        public delegate sbyte _MyInt8Method(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        public delegate int _MyInt32Method(MyStruct* pThis);

        public void MyVoidMethod()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                Marshal.GetDelegateForFunctionPointer<_MyVoidMethod>(lpVtbl->MyVoidMethod)(pThis);
            }}
        }}

        [return: NativeTypeName(""char"")]
        public sbyte MyInt8Method()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_MyInt8Method>(lpVtbl->MyInt8Method)(pThis);
            }}
        }}

        public int MyInt32Method()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_MyInt32Method>(lpVtbl->MyInt32Method)(pThis);
            }}
        }}

        public partial struct Vtbl
        {{
            [NativeTypeName(""void (){callConvAttr}"")]
            public IntPtr MyVoidMethod;

            [NativeTypeName(""char (){callConvAttr}"")]
            public IntPtr MyInt8Method;

            [NativeTypeName(""int (){callConvAttr}"")]
            public IntPtr MyInt32Method;
        }}
    }}
}}
";

            await ValidateGeneratedCompatibleBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task VirtualTest()
        {
            var inputContents = @"struct MyStruct
{
    virtual void MyVoidMethod() = 0;

    virtual char MyInt8Method()
    {
        return 0;
    }

    virtual int MyInt32Method();

    virtual void* MyVoidStarMethod() = 0;
};
";

            var callConv = "Cdecl";
            var callConvAttr = "";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !Environment.Is64BitProcess)
            {
                callConv = "ThisCall";
                callConvAttr = " __attribute__((thiscall))";
            }

            var expectedOutputContents = $@"using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public unsafe partial struct MyStruct
    {{
        public readonly Vtbl* lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        public delegate void _MyVoidMethod(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        [return: NativeTypeName(""char"")]
        public delegate sbyte _MyInt8Method(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        public delegate int _MyInt32Method(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.{callConv})]
        [return: NativeTypeName(""void *"")]
        public delegate void* _MyVoidStarMethod(MyStruct* pThis);

        public void MyVoidMethod()
        {{
            Marshal.GetDelegateForFunctionPointer<_MyVoidMethod>(lpVtbl->MyVoidMethod)((MyStruct*)Unsafe.AsPointer(ref this));
        }}

        [return: NativeTypeName(""char"")]
        public sbyte MyInt8Method()
        {{
            return Marshal.GetDelegateForFunctionPointer<_MyInt8Method>(lpVtbl->MyInt8Method)((MyStruct*)Unsafe.AsPointer(ref this));
        }}

        public int MyInt32Method()
        {{
            return Marshal.GetDelegateForFunctionPointer<_MyInt32Method>(lpVtbl->MyInt32Method)((MyStruct*)Unsafe.AsPointer(ref this));
        }}

        [return: NativeTypeName(""void *"")]
        public void* MyVoidStarMethod()
        {{
            return Marshal.GetDelegateForFunctionPointer<_MyVoidStarMethod>(lpVtbl->MyVoidStarMethod)((MyStruct*)Unsafe.AsPointer(ref this));
        }}

        public partial struct Vtbl
        {{
            [NativeTypeName(""void (){callConvAttr}"")]
            public IntPtr MyVoidMethod;

            [NativeTypeName(""char (){callConvAttr}"")]
            public IntPtr MyInt8Method;

            [NativeTypeName(""int (){callConvAttr}"")]
            public IntPtr MyInt32Method;

            [NativeTypeName(""void *(){callConvAttr}"")]
            public IntPtr MyVoidStarMethod;
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }
    }
}
