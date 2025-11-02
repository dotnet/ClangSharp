// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;

namespace ClangSharp.UnitTests;

public abstract class CXXMethodDeclarationCSharpTest : CXXMethodDeclarationTest
{
    protected override Task ConstructorTestImpl()
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ConstructorWithInitializeTestImpl()
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ConversionTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    int value;
    int* pointer;
    int** pointer2;
    operator int()
    {
        return value;
    }
    operator int*()
    {
        return pointer;
    }
    operator int**()
    {
        return pointer2;
    }
};
";

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        public int value;

        public int* pointer;

        public int** pointer2;

        public int ToInt32()
        {
            return value;
        }

        public int* ToInt32Pointer()
        {
            return pointer;
        }

        public int** ToInt32Pointer2()
        {
            return pointer2;
        }
    }
}
";

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task DestructorTestImpl()
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task MacrosExpansionTestImpl()
    {
        var inputContents = @"typedef struct
{
    unsigned char *buf;
    int size;
} context_t;

int buf_close(void *pContext)
{
    ((context_t*)pContext)->buf=0;
    return 0;
}
";

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public unsafe partial struct context_t
    {
        [NativeTypeName(""unsigned char *"")]
        public byte* buf;

        public int size;
    }

    public static unsafe partial class Methods
    {
        public static int buf_close(void* pContext)
        {
            ((context_t*)(pContext))->buf = null;
            return 0;
        }
    }
}
";

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task MemberCallTestImpl()
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task MemberTestImpl()
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task NewKeywordTestImpl()
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task OperatorTestImpl()
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task OperatorCallTestImpl()
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ThisTestImpl()
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task UnsafeDoesNotImpactDllImportTestImpl()
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
    public unsafe partial struct MyStruct
    {
        public void* MyVoidStarMethod()
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
    }

}
