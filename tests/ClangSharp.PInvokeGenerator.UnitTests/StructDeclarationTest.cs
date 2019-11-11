// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ClangSharp.UnitTests
{
    public sealed class StructDeclarationTest : PInvokeGeneratorTest
    {
        [Theory]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("float", "float")]
        public async Task BasicTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"struct MyStruct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        public {expectedManagedType} r;

        public {expectedManagedType} g;

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
            var inputContents = $@"struct MyStruct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        [NativeTypeName(""{nativeType}"")]
        public {expectedManagedType} r;

        [NativeTypeName(""{nativeType}"")]
        public {expectedManagedType} g;

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
            var inputContents = "typedef struct MyStruct MyStruct;";
            var expectedOutputContents = string.Empty;

            var excludedNames = new string[] { "MyStruct" };
            await ValidateGeneratedBindings(inputContents, expectedOutputContents, excludedNames);
        }

        [Theory]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("float", "float")]
        public async Task FixedSizedBufferNonPrimitiveTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"struct MyStruct
{{
    {nativeType} value;
}};

struct MyOtherStruct
{{
    MyStruct c[3];
}};
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        public {expectedManagedType} value;
    }}

    public partial struct MyOtherStruct
    {{
        [NativeTypeName(""MyStruct [3]"")]
        public _c_e__FixedBuffer c;

        public partial struct _c_e__FixedBuffer
        {{
            internal MyStruct e0;
            internal MyStruct e1;
            internal MyStruct e2;

            public unsafe ref MyStruct this[int index]
            {{
                get
                {{
                    fixed (MyStruct* pThis = &e0)
                    {{
                        return ref pThis[index];
                    }}
                }}
            }}
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
        public async Task FixedSizedBufferNonPrimitiveWithNativeTypeNameTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"struct MyStruct
{{
    {nativeType} value;
}};

struct MyOtherStruct
{{
    MyStruct c[3];
}};
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        [NativeTypeName(""{nativeType}"")]
        public {expectedManagedType} value;
    }}

    public partial struct MyOtherStruct
    {{
        [NativeTypeName(""MyStruct [3]"")]
        public _c_e__FixedBuffer c;

        public partial struct _c_e__FixedBuffer
        {{
            internal MyStruct e0;
            internal MyStruct e1;
            internal MyStruct e2;

            public unsafe ref MyStruct this[int index]
            {{
                get
                {{
                    fixed (MyStruct* pThis = &e0)
                    {{
                        return ref pThis[index];
                    }}
                }}
            }}
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
            var inputContents = $@"struct MyStruct
{{
    {nativeType} c[3];
}};
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public unsafe partial struct MyStruct
    {{
        [NativeTypeName(""{nativeType} [3]"")]
        public fixed {expectedManagedType} c[3];
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
            var inputContents = $@"struct MyStruct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;

    struct MyNestedStruct
    {{
        {nativeType} r;
        {nativeType} g;
        {nativeType} b;
        {nativeType} a;
    }};
}};
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        public {expectedManagedType} r;

        public {expectedManagedType} g;

        public {expectedManagedType} b;

        public partial struct MyNestedStruct
        {{
            public {expectedManagedType} r;

            public {expectedManagedType} g;

            public {expectedManagedType} b;

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
            var inputContents = $@"struct MyStruct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;

    struct MyNestedStruct
    {{
        {nativeType} r;
        {nativeType} g;
        {nativeType} b;
        {nativeType} a;
    }};
}};
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        [NativeTypeName(""{nativeType}"")]
        public {expectedManagedType} r;

        [NativeTypeName(""{nativeType}"")]
        public {expectedManagedType} g;

        [NativeTypeName(""{nativeType}"")]
        public {expectedManagedType} b;

        public partial struct MyNestedStruct
        {{
            [NativeTypeName(""{nativeType}"")]
            public {expectedManagedType} r;

            [NativeTypeName(""{nativeType}"")]
            public {expectedManagedType} g;

            [NativeTypeName(""{nativeType}"")]
            public {expectedManagedType} b;

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
            var inputContents = "typedef struct MyStruct MyStruct;";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
    }}
}}
";
            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task RemapTest()
        {
            var inputContents = "typedef struct _MyStruct MyStruct;";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
    }}
}}
";

            var remappedNames = new Dictionary<string, string> { ["_MyStruct"] = "MyStruct" };
            await ValidateGeneratedBindings(inputContents, expectedOutputContents, excludedNames: null, remappedNames);
        }

        [Theory]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("float", "float")]
        public async Task SkipNonDefinitionTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"typedef struct MyStruct MyStruct;

struct MyStruct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        public {expectedManagedType} r;

        public {expectedManagedType} g;

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
            var inputContents = $@"typedef struct MyStruct MyStruct;

struct MyStruct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        [NativeTypeName(""{nativeType}"")]
        public {expectedManagedType} r;

        [NativeTypeName(""{nativeType}"")]
        public {expectedManagedType} g;

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

struct MyStruct
{{
    MyTypedefAlias r;
    MyTypedefAlias g;
    MyTypedefAlias b;
}};
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        [NativeTypeName(""MyTypedefAlias"")]
        public {expectedManagedType} r;

        [NativeTypeName(""MyTypedefAlias"")]
        public {expectedManagedType} g;

        [NativeTypeName(""MyTypedefAlias"")]
        public {expectedManagedType} b;
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }
    }
}
