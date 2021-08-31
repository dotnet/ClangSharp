// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class CSharpCompatibleUnix_CXXMethodDeclarationTest : CXXMethodDeclarationTest
    {
        public override Task ConstructorTest()
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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ConstructorWithInitializeTest()
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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ConstructorImportTest()
        {
            var inputContents = @"struct MyStruct
{
    MyStruct();
    MyStruct(int a);
};
";

            var entryPoint1 = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? (Environment.Is64BitProcess ? "??0MyStruct@@QEAA@XZ" : "??0MyStruct@@QAE@XZ")
                : "_ZN8MyStructC1Ev";
            var entryPoint2 = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? (Environment.Is64BitProcess ? "??0MyStruct@@QEAA@H@Z" : "??0MyStruct@@QAE@H@Z")
                : "_ZN8MyStructC1Ei";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                entryPoint1 = $"_{entryPoint1}";
                entryPoint2 = $"_{entryPoint2}";
            }

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.ThisCall, EntryPoint = ""{entryPoint1}"", ExactSpelling = true)]
        public static extern void Constructor(MyStruct* pThis);

        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.ThisCall, EntryPoint = ""{entryPoint2}"", ExactSpelling = true)]
        public static extern void Constructor(MyStruct* pThis, int a);
    }}
}}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ConversionTest()
        {
            var inputContents = @"struct MyStruct
{
    int value;

    operator int()
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

        public int ToInt32()
        {
            return value;
        }
    }
}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task DestructorTest()
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
        public void Dispose()
        {
        }
    }
}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task InstanceTest()
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
            var entryPoint = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "__ZN8MyStruct12MyVoidMethodEv" : "_ZN8MyStruct12MyVoidMethodEv";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (!Environment.Is64BitProcess)
                {
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
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.ThisCall, EntryPoint = ""{entryPoint}"", ExactSpelling = true)]
        public static extern void MyVoidMethod(MyStruct* pThis);

        public int MyInt32Method()
        {{
            return 0;
        }}

        public unsafe void* MyVoidStarMethod()
        {{
            return null;
        }}
    }}
}}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task MemberCallTest()
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
        public static int MyFunctionA(MyStruct x)
        {
            return x.MyFunction1();
        }

        public static int MyFunctionB(MyStruct* x)
        {
            return x->MyFunction2();
        }
    }
}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task MemberTest()
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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task NewKeywordTest()
        {
            var inputContents = @"struct MyStruct
{
    int Equals() { return 0; }
    int Equals(int obj) { return 0; }
    int Dispose() { return 0; }
    int Dispose(int obj) { return 0; }
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

        public int Dispose()
        {{
            return 0;
        }}

        public int Dispose(int obj)
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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task NewKeywordVirtualTest()
        {
            var inputContents = @"struct MyStruct
{
    virtual int GetType(int obj) = 0;
    virtual int GetType() = 0;
    virtual int GetType(int objA, int objB) = 0;
};";

            var expectedOutputContents = $@"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public unsafe partial struct MyStruct
    {{
        public void** lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType(MyStruct* pThis, int obj);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType1(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType2(MyStruct* pThis, int objA, int objB);

        public int GetType(int obj)
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_GetType>((IntPtr)(lpVtbl[{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? 2 : 0)}]))(pThis, obj);
            }}
        }}

        public new int GetType()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_GetType1>((IntPtr)(lpVtbl[1]))(pThis);
            }}
        }}

        public int GetType(int objA, int objB)
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_GetType2>((IntPtr)(lpVtbl[{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? 0 : 2)}]))(pThis, objA, objB);
            }}
        }}
    }}
}}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task NewKeywordVirtualWithExplicitVtblTest()
        {
            var inputContents = @"struct MyStruct
{
    virtual int GetType(int obj) = 0;
    virtual int GetType() = 0;
    virtual int GetType(int objA, int objB) = 0;
};";

            var nativeCallConv = "";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !Environment.Is64BitProcess)
            {
                nativeCallConv = " __attribute__((thiscall))";
            }

            var expectedOutputContents = $@"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public unsafe partial struct MyStruct
    {{
        public Vtbl* lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType(MyStruct* pThis, int obj);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType1(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType2(MyStruct* pThis, int objA, int objB);

        public int GetType(int obj)
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_GetType>(lpVtbl->GetType)(pThis, obj);
            }}
        }}

        public new int GetType()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_GetType1>(lpVtbl->GetType1)(pThis);
            }}
        }}

        public int GetType(int objA, int objB)
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_GetType2>(lpVtbl->GetType2)(pThis, objA, objB);
            }}
        }}

        public partial struct Vtbl
        {{
            [NativeTypeName(""{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "int (int, int)" : "int (int)")}{nativeCallConv}"")]
            public {(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "" : "new ")}IntPtr GetType{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "2" : "")};

            [NativeTypeName(""int (){nativeCallConv}"")]
            public IntPtr GetType1;

            [NativeTypeName(""{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "int (int)" : "int (int, int)")}{nativeCallConv}"")]
            public {(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "new " : "")}IntPtr GetType{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "" : "2")};
        }}
    }}
}}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls);
        }

        public override Task OperatorTest()
        {
            var inputContents = @"struct MyStruct
{
    int value;

    MyStruct(int value) : value(value)
    {
    }

    MyStruct operator+(MyStruct rhs)
    {
        return MyStruct(value + rhs.value);
    }
};

MyStruct operator-(MyStruct lhs, MyStruct rhs)
{
    return MyStruct(lhs.value - rhs.value);
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int value;

        public MyStruct(int value)
        {
            this.value = value;
        }

        public MyStruct Add(MyStruct rhs)
        {
            return new MyStruct(value + rhs.value);
        }
    }

    public static partial class Methods
    {
        public static MyStruct Subtract(MyStruct lhs, MyStruct rhs)
        {
            return new MyStruct(lhs.value - rhs.value);
        }
    }
}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task OperatorCallTest()
        {
            var inputContents = @"struct MyStruct
{
    int value;

    MyStruct(int value) : value(value)
    {
    }

    MyStruct operator+(MyStruct rhs)
    {
        return MyStruct(value + rhs.value);
    }
};

MyStruct MyFunction1(MyStruct lhs, MyStruct rhs)
{
    return lhs + rhs;
}

MyStruct operator-(MyStruct lhs, MyStruct rhs)
{
    return MyStruct(lhs.value - rhs.value);
}

MyStruct MyFunction2(MyStruct lhs, MyStruct rhs)
{
    return lhs - rhs;
}
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int value;

        public MyStruct(int value)
        {
            this.value = value;
        }

        public MyStruct Add(MyStruct rhs)
        {
            return new MyStruct(value + rhs.value);
        }
    }

    public static partial class Methods
    {
        public static MyStruct MyFunction1(MyStruct lhs, MyStruct rhs)
        {
            return lhs.Add(rhs);
        }

        public static MyStruct Subtract(MyStruct lhs, MyStruct rhs)
        {
            return new MyStruct(lhs.value - rhs.value);
        }

        public static MyStruct MyFunction2(MyStruct lhs, MyStruct rhs)
        {
            return Subtract(lhs, rhs);
        }
    }
}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task StaticTest()
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
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, EntryPoint = ""{entryPoint}"", ExactSpelling = true)]
        public static extern void MyVoidMethod();

        public static int MyInt32Method()
        {{
            return 0;
        }}

        public static unsafe void* MyVoidStarMethod()
        {{
            return null;
        }}
    }}
}}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ThisTest()
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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task UnsafeDoesNotImpactDllImportTest()
        {
            var inputContents = @"struct MyStruct
{
    void* MyVoidStarMethod()
    {
        return nullptr;
    }
};

extern ""C"" void MyFunction();";

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public unsafe void* MyVoidStarMethod()
        {
            return null;
        }
    }

    public static partial class Methods
    {
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction();
    }
}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task VirtualTest()
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

            var expectedOutputContents = $@"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public unsafe partial struct MyStruct
    {{
        public void** lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void _MyVoidMethod(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        [return: NativeTypeName(""char"")]
        public delegate sbyte _MyInt8Method(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _MyInt32Method(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void* _MyVoidStarMethod(MyStruct* pThis);

        public void MyVoidMethod()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                Marshal.GetDelegateForFunctionPointer<_MyVoidMethod>((IntPtr)(lpVtbl[0]))(pThis);
            }}
        }}

        [return: NativeTypeName(""char"")]
        public sbyte MyInt8Method()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_MyInt8Method>((IntPtr)(lpVtbl[1]))(pThis);
            }}
        }}

        public int MyInt32Method()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_MyInt32Method>((IntPtr)(lpVtbl[2]))(pThis);
            }}
        }}

        public void* MyVoidStarMethod()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_MyVoidStarMethod>((IntPtr)(lpVtbl[3]))(pThis);
            }}
        }}
    }}
}}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task VirtualWithVtblIndexAttributeTest()
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

            var expectedOutputContents = $@"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public unsafe partial struct MyStruct
    {{
        public void** lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void _MyVoidMethod(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        [return: NativeTypeName(""char"")]
        public delegate sbyte _MyInt8Method(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _MyInt32Method(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void* _MyVoidStarMethod(MyStruct* pThis);

        [VtblIndex(0)]
        public void MyVoidMethod()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                Marshal.GetDelegateForFunctionPointer<_MyVoidMethod>((IntPtr)(lpVtbl[0]))(pThis);
            }}
        }}

        [VtblIndex(1)]
        [return: NativeTypeName(""char"")]
        public sbyte MyInt8Method()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_MyInt8Method>((IntPtr)(lpVtbl[1]))(pThis);
            }}
        }}

        [VtblIndex(2)]
        public int MyInt32Method()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_MyInt32Method>((IntPtr)(lpVtbl[2]))(pThis);
            }}
        }}

        [VtblIndex(3)]
        public void* MyVoidStarMethod()
        {{
            fixed (MyStruct* pThis = &this)
            {{
                return Marshal.GetDelegateForFunctionPointer<_MyVoidStarMethod>((IntPtr)(lpVtbl[3]))(pThis);
            }}
        }}
    }}
}}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateVtblIndexAttribute);
        }
    }
}
