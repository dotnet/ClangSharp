// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.UnitTests
{
    public sealed class UnionDeclarationTest : PInvokeGeneratorTest
    {
        [Theory]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("float", "float")]
        public async Task BasicTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"union MyUnion
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {{
        [FieldOffset(0)]
        public {expectedManagedType} r;

        [FieldOffset(0)]
        public {expectedManagedType} g;

        [FieldOffset(0)]
        public {expectedManagedType} b;
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("unsigned char", "byte")]
        [InlineData("long long", "long")]
        [InlineData("signed char", "sbyte")]
        [InlineData("unsigned short", "ushort")]
        [InlineData("unsigned int", "uint")]
        [InlineData("unsigned long long", "ulong")]
        public async Task BasicWithNativeTypeNameTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"union MyUnion
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {{
        [FieldOffset(0)]
        [NativeTypeName(""{nativeType}"")]
        public {expectedManagedType} r;

        [FieldOffset(0)]
        [NativeTypeName(""{nativeType}"")]
        public {expectedManagedType} g;

        [FieldOffset(0)]
        [NativeTypeName(""{nativeType}"")]
        public {expectedManagedType} b;
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task ExcludeTest()
        {
            var inputContents = "typedef union MyUnion MyUnion;";
            var expectedOutputContents = string.Empty;

            var excludedNames = new string[] { "MyUnion" };
            await ValidateGeneratedBindings(inputContents, expectedOutputContents, excludedNames);
        }

        [Theory]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("float", "float")]
        public async Task FixedSizedBufferNonPrimitiveCompatibleTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"union MyUnion
{{
    {nativeType} value;
}};

union MyOtherUnion
{{
    MyUnion c[3];
}};
";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {{
        [FieldOffset(0)]
        public {expectedManagedType} value;
    }}

    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyOtherUnion
    {{
        [FieldOffset(0)]
        [NativeTypeName(""MyUnion [3]"")]
        public _c_e__FixedBuffer c;

        public partial struct _c_e__FixedBuffer
        {{
            public MyUnion e0;
            public MyUnion e1;
            public MyUnion e2;

            public unsafe ref MyUnion this[int index]
            {{
                get
                {{
                    fixed (MyUnion* pThis = &e0)
                    {{
                        return ref pThis[index];
                    }}
                }}
            }}
        }}
    }}
}}
";

            await ValidateGeneratedCompatibleBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("float", "float")]
        public async Task FixedSizedBufferNonPrimitiveTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"union MyUnion
{{
    {nativeType} value;
}};

union MyOtherUnion
{{
    MyUnion c[3];
}};
";

            var expectedOutputContents = $@"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {{
        [FieldOffset(0)]
        public {expectedManagedType} value;
    }}

    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyOtherUnion
    {{
        [FieldOffset(0)]
        [NativeTypeName(""MyUnion [3]"")]
        public _c_e__FixedBuffer c;

        public partial struct _c_e__FixedBuffer
        {{
            public MyUnion e0;
            public MyUnion e1;
            public MyUnion e2;

            public ref MyUnion this[int index]
            {{
                get
                {{
                    return ref AsSpan()[index];
                }}
            }}

            public Span<MyUnion> AsSpan() => MemoryMarshal.CreateSpan(ref e0, 3);
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("unsigned char", "byte")]
        [InlineData("long long", "long")]
        [InlineData("signed char", "sbyte")]
        [InlineData("unsigned short", "ushort")]
        [InlineData("unsigned int", "uint")]
        [InlineData("unsigned long long", "ulong")]
        public async Task FixedSizedBufferNonPrimitiveWithNativeTypeNameCompatibleTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"union MyUnion
{{
    {nativeType} value;
}};

union MyOtherUnion
{{
    MyUnion c[3];
}};
";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {{
        [FieldOffset(0)]
        [NativeTypeName(""{nativeType}"")]
        public {expectedManagedType} value;
    }}

    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyOtherUnion
    {{
        [FieldOffset(0)]
        [NativeTypeName(""MyUnion [3]"")]
        public _c_e__FixedBuffer c;

        public partial struct _c_e__FixedBuffer
        {{
            public MyUnion e0;
            public MyUnion e1;
            public MyUnion e2;

            public unsafe ref MyUnion this[int index]
            {{
                get
                {{
                    fixed (MyUnion* pThis = &e0)
                    {{
                        return ref pThis[index];
                    }}
                }}
            }}
        }}
    }}
}}
";

            await ValidateGeneratedCompatibleBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("unsigned char", "byte")]
        [InlineData("long long", "long")]
        [InlineData("signed char", "sbyte")]
        [InlineData("unsigned short", "ushort")]
        [InlineData("unsigned int", "uint")]
        [InlineData("unsigned long long", "ulong")]
        public async Task FixedSizedBufferNonPrimitiveWithNativeTypeNameTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"union MyUnion
{{
    {nativeType} value;
}};

union MyOtherUnion
{{
    MyUnion c[3];
}};
";

            var expectedOutputContents = $@"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {{
        [FieldOffset(0)]
        [NativeTypeName(""{nativeType}"")]
        public {expectedManagedType} value;
    }}

    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyOtherUnion
    {{
        [FieldOffset(0)]
        [NativeTypeName(""MyUnion [3]"")]
        public _c_e__FixedBuffer c;

        public partial struct _c_e__FixedBuffer
        {{
            public MyUnion e0;
            public MyUnion e1;
            public MyUnion e2;

            public ref MyUnion this[int index]
            {{
                get
                {{
                    return ref AsSpan()[index];
                }}
            }}

            public Span<MyUnion> AsSpan() => MemoryMarshal.CreateSpan(ref e0, 3);
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("unsigned char", "byte")]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("long long", "long")]
        [InlineData("signed char", "sbyte")]
        [InlineData("float", "float")]
        [InlineData("unsigned short", "ushort")]
        [InlineData("unsigned int", "uint")]
        [InlineData("unsigned long long", "ulong")]
        public async Task FixedSizedBufferPrimitiveTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"union MyUnion
{{
    {nativeType} c[3];
}};
";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct MyUnion
    {{
        [FieldOffset(0)]
        [NativeTypeName(""{nativeType} [3]"")]
        public fixed {expectedManagedType} c[3];
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("double", "double", 11, 5)]
        [InlineData("short", "short", 11, 5)]
        [InlineData("int", "int", 11, 5)]
        [InlineData("float", "float", 11, 5)]
        public async Task NestedAnonymousTest(string nativeType, string expectedManagedType, int line, int column)
        {
            var inputContents = $@"typedef struct {{
    {nativeType} value;
}} MyStruct;

union MyUnion
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;

    union
    {{
        {nativeType} a;

        MyStruct s;

        {nativeType} buffer[4];
    }};
}};
";

            var expectedOutputContents = $@"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        public {expectedManagedType} value;
    }}

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct MyUnion
    {{
        [FieldOffset(0)]
        public {expectedManagedType} r;

        [FieldOffset(0)]
        public {expectedManagedType} g;

        [FieldOffset(0)]
        public {expectedManagedType} b;

        [FieldOffset(0)]
        [NativeTypeName(""MyUnion::(anonymous union at ClangUnsavedFile.h:{line}:{column})"")]
        public _Anonymous_e__Union Anonymous;

        public ref {expectedManagedType} a
        {{
            get
            {{
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.a, 1));
            }}
        }}

        public ref MyStruct s
        {{
            get
            {{
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.s, 1));
            }}
        }}

        public Span<{expectedManagedType}> buffer
        {{
            get
            {{
                return MemoryMarshal.CreateSpan(ref Anonymous.buffer[0], 4);
            }}
        }}

        [StructLayout(LayoutKind.Explicit)]
        public unsafe partial struct _Anonymous_e__Union
        {{
            [FieldOffset(0)]
            public {expectedManagedType} a;

            [FieldOffset(0)]
            public MyStruct s;

            [FieldOffset(0)]
            [NativeTypeName(""{nativeType} [4]"")]
            public fixed {expectedManagedType} buffer[4];
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("float", "float")]
        public async Task NestedTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"union MyUnion
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;

    union MyNestedUnion
    {{
        {nativeType} r;
        {nativeType} g;
        {nativeType} b;
        {nativeType} a;
    }};
}};
";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {{
        [FieldOffset(0)]
        public {expectedManagedType} r;

        [FieldOffset(0)]
        public {expectedManagedType} g;

        [FieldOffset(0)]
        public {expectedManagedType} b;

        [StructLayout(LayoutKind.Explicit)]
        public partial struct MyNestedUnion
        {{
            [FieldOffset(0)]
            public {expectedManagedType} r;

            [FieldOffset(0)]
            public {expectedManagedType} g;

            [FieldOffset(0)]
            public {expectedManagedType} b;

            [FieldOffset(0)]
            public {expectedManagedType} a;
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("unsigned char", "byte")]
        [InlineData("long long", "long")]
        [InlineData("signed char", "sbyte")]
        [InlineData("unsigned short", "ushort")]
        [InlineData("unsigned int", "uint")]
        [InlineData("unsigned long long", "ulong")]
        public async Task NestedWithNativeTypeNameTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"union MyUnion
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;

    union MyNestedUnion
    {{
        {nativeType} r;
        {nativeType} g;
        {nativeType} b;
        {nativeType} a;
    }};
}};
";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {{
        [FieldOffset(0)]
        [NativeTypeName(""{nativeType}"")]
        public {expectedManagedType} r;

        [FieldOffset(0)]
        [NativeTypeName(""{nativeType}"")]
        public {expectedManagedType} g;

        [FieldOffset(0)]
        [NativeTypeName(""{nativeType}"")]
        public {expectedManagedType} b;

        [StructLayout(LayoutKind.Explicit)]
        public partial struct MyNestedUnion
        {{
            [FieldOffset(0)]
            [NativeTypeName(""{nativeType}"")]
            public {expectedManagedType} r;

            [FieldOffset(0)]
            [NativeTypeName(""{nativeType}"")]
            public {expectedManagedType} g;

            [FieldOffset(0)]
            [NativeTypeName(""{nativeType}"")]
            public {expectedManagedType} b;

            [FieldOffset(0)]
            [NativeTypeName(""{nativeType}"")]
            public {expectedManagedType} a;
        }}
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task NoDefinitionTest()
        {
            var inputContents = "typedef union MyUnion MyUnion;";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {{
    }}
}}
";
            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task RemapTest()
        {
            var inputContents = "typedef union _MyUnion MyUnion;";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {{
    }}
}}
";

            var remappedNames = new Dictionary<string, string> { ["_MyUnion"] = "MyUnion" };
            await ValidateGeneratedBindings(inputContents, expectedOutputContents, excludedNames: null, remappedNames);
        }

        [Fact]
        public async Task RemapNestedAnonymousTest()
        {
            var inputContents = @"union MyUnion
{
    double r;
    double g;
    double b;

    union
    {
        double a;
    };
};";

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        public double r;

        [FieldOffset(0)]
        public double g;

        [FieldOffset(0)]
        public double b;

        [FieldOffset(0)]
        [NativeTypeName(""MyUnion::(anonymous union at ClangUnsavedFile.h:7:5)"")]
        public _Anonymous_e__Union Anonymous;

        public ref double a
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.a, 1));
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            public double a;
        }
    }
}
";

            var remappedNames = new Dictionary<string, string> {
                ["__AnonymousField_ClangUnsavedFile_L7_C5"] = "Anonymous",
                ["__AnonymousRecord_ClangUnsavedFile_L7_C5"] = "_Anonymous_e__Union"
            };
            await ValidateGeneratedBindings(inputContents, expectedOutputContents, excludedNames: null, remappedNames);
        }

        [Theory]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("float", "float")]
        public async Task SkipNonDefinitionTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"typedef union MyUnion MyUnion;

union MyUnion
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {{
        [FieldOffset(0)]
        public {expectedManagedType} r;

        [FieldOffset(0)]
        public {expectedManagedType} g;

        [FieldOffset(0)]
        public {expectedManagedType} b;
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("unsigned char", "byte")]
        [InlineData("long long", "long")]
        [InlineData("signed char", "sbyte")]
        [InlineData("unsigned short", "ushort")]
        [InlineData("unsigned int", "uint")]
        [InlineData("unsigned long long", "ulong")]
        public async Task SkipNonDefinitionWithNativeTypeNameTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"typedef union MyUnion MyUnion;

union MyUnion
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {{
        [FieldOffset(0)]
        [NativeTypeName(""{nativeType}"")]
        public {expectedManagedType} r;

        [FieldOffset(0)]
        [NativeTypeName(""{nativeType}"")]
        public {expectedManagedType} g;

        [FieldOffset(0)]
        [NativeTypeName(""{nativeType}"")]
        public {expectedManagedType} b;
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("unsigned char", "byte")]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("long long", "long")]
        [InlineData("signed char", "sbyte")]
        [InlineData("float", "float")]
        [InlineData("unsigned short", "ushort")]
        [InlineData("unsigned int", "uint")]
        [InlineData("unsigned long long", "ulong")]
        public async Task TypedefTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"typedef {nativeType} MyTypedefAlias;

union MyUnion
{{
    MyTypedefAlias r;
    MyTypedefAlias g;
    MyTypedefAlias b;
}};
";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {{
        [FieldOffset(0)]
        [NativeTypeName(""MyTypedefAlias"")]
        public {expectedManagedType} r;

        [FieldOffset(0)]
        [NativeTypeName(""MyTypedefAlias"")]
        public {expectedManagedType} g;

        [FieldOffset(0)]
        [NativeTypeName(""MyTypedefAlias"")]
        public {expectedManagedType} b;
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }
    }
}
