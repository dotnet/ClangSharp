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

        [Fact]
        public async Task BitfieldTest()
        {
            var inputContents = @"struct MyStruct1
{
    unsigned int o0_b0_24 : 24;
    unsigned int o4_b0_16 : 16;
    unsigned int o4_b16_3 : 3;
    unsigned char o8_b0_1 : 1;
    int o12_b0_1 : 1;
    int o12_b1_1 : 1;
};

struct MyStruct2
{
    unsigned int o0_b0_1 : 1;
    unsigned int o0_b1_1 : 1;
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct1
    {
        internal uint _bitfield1;

        [NativeTypeName(""unsigned int : 24"")]
        public uint o0_b0_24
        {
            get
            {
                return _bitfield1 & 0xFFFFFFu;
            }

            set
            {
                _bitfield1 = (_bitfield1 & ~0xFFFFFFu) | (value & 0xFFFFFFu);
            }
        }

        internal uint _bitfield2;

        [NativeTypeName(""unsigned int : 16"")]
        public uint o4_b0_16
        {
            get
            {
                return _bitfield2 & 0xFFFFu;
            }

            set
            {
                _bitfield2 = (_bitfield2 & ~0xFFFFu) | (value & 0xFFFFu);
            }
        }

        [NativeTypeName(""unsigned int : 3"")]
        public uint o4_b16_3
        {
            get
            {
                return (_bitfield2 >> 16) & 0x7u;
            }

            set
            {
                _bitfield2 = (_bitfield2 & ~(0x7u << 16)) | ((value & 0x7u) << 16);
            }
        }

        internal byte _bitfield3;

        [NativeTypeName(""unsigned char : 1"")]
        public byte o8_b0_1
        {
            get
            {
                return (byte)(_bitfield3 & 0x1u);
            }

            set
            {
                _bitfield3 = (byte)((_bitfield3 & ~0x1u) | (value & 0x1u));
            }
        }

        internal int _bitfield4;

        [NativeTypeName(""int : 1"")]
        public int o12_b0_1
        {
            get
            {
                return _bitfield4 & 0x1;
            }

            set
            {
                _bitfield4 = (_bitfield4 & ~0x1) | (value & 0x1);
            }
        }

        [NativeTypeName(""int : 1"")]
        public int o12_b1_1
        {
            get
            {
                return (_bitfield4 >> 1) & 0x1;
            }

            set
            {
                _bitfield4 = (_bitfield4 & ~(0x1 << 1)) | ((value & 0x1) << 1);
            }
        }
    }

    public partial struct MyStruct2
    {
        internal uint _bitfield;

        [NativeTypeName(""unsigned int : 1"")]
        public uint o0_b0_1
        {
            get
            {
                return _bitfield & 0x1u;
            }

            set
            {
                _bitfield = (_bitfield & ~0x1u) | (value & 0x1u);
            }
        }

        [NativeTypeName(""unsigned int : 1"")]
        public uint o0_b1_1
        {
            get
            {
                return (_bitfield >> 1) & 0x1u;
            }

            set
            {
                _bitfield = (_bitfield & ~(0x1u << 1)) | ((value & 0x1u) << 1);
            }
        }
    }
}
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
        public async Task FixedSizedBufferNonPrimitiveCompatibleTest(string nativeType, string expectedManagedType)
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

            await ValidateGeneratedCompatibleBindings(inputContents, expectedOutputContents);
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

            var expectedOutputContents = $@"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
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

            public ref MyStruct this[int index] => ref AsSpan()[index];

            public Span<MyStruct> AsSpan() => MemoryMarshal.CreateSpan(ref e0, 3);
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
        public async Task FixedSizedBufferNonPrimitiveMultidimensionalTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"struct MyStruct
{{
    {nativeType} value;
}};

struct MyOtherStruct
{{
    MyStruct c[2][1][3][4];
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

    public partial struct MyOtherStruct
    {{
        [NativeTypeName(""MyStruct [2][1][3][4]"")]
        public _c_e__FixedBuffer c;

        public partial struct _c_e__FixedBuffer
        {{
            internal MyStruct e0_0_0_0;
            internal MyStruct e1_0_0_0;

            internal MyStruct e0_0_1_0;
            internal MyStruct e1_0_1_0;

            internal MyStruct e0_0_2_0;
            internal MyStruct e1_0_2_0;

            internal MyStruct e0_0_0_1;
            internal MyStruct e1_0_0_1;

            internal MyStruct e0_0_1_1;
            internal MyStruct e1_0_1_1;

            internal MyStruct e0_0_2_1;
            internal MyStruct e1_0_2_1;

            internal MyStruct e0_0_0_2;
            internal MyStruct e1_0_0_2;

            internal MyStruct e0_0_1_2;
            internal MyStruct e1_0_1_2;

            internal MyStruct e0_0_2_2;
            internal MyStruct e1_0_2_2;

            internal MyStruct e0_0_0_3;
            internal MyStruct e1_0_0_3;

            internal MyStruct e0_0_1_3;
            internal MyStruct e1_0_1_3;

            internal MyStruct e0_0_2_3;
            internal MyStruct e1_0_2_3;

            public ref MyStruct this[int index] => ref AsSpan()[index];

            public Span<MyStruct> AsSpan() => MemoryMarshal.CreateSpan(ref e0, 24);
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
            var inputContents = $@"struct MyStruct
{{
    {nativeType} value;
}};

struct MyOtherStruct
{{
    MyStruct c[3];
}};
";

            var expectedOutputContents = $@"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
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

            public ref MyStruct this[int index] => ref AsSpan()[index];

            public Span<MyStruct> AsSpan() => MemoryMarshal.CreateSpan(ref e0, 3);
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
        public async Task FixedSizedBufferPrimitiveMultidimensionalTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"struct MyStruct
{{
    {nativeType} c[2][1][3][4];
}};
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public unsafe partial struct MyStruct
    {{
        [NativeTypeName(""{nativeType} [2][1][3][4]"")]
        public fixed {expectedManagedType} c[2 * 1 * 3 * 4];
    }}
}}
";

            await ValidateGeneratedBindings(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("double", "double", 7, 5)]
        [InlineData("short", "short", 7, 5)]
        [InlineData("int", "int", 7, 5)]
        [InlineData("float", "float", 7, 5)]
        public async Task NestedAnonymousTest(string nativeType, string expectedManagedType, int line, int column)
        {
            var inputContents = $@"struct MyStruct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;

    struct
    {{
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

        [NativeTypeName(""MyStruct::(anonymous struct at ClangUnsavedFile.h:{line}:{column})"")]
        public __AnonymousRecord_ClangUnsavedFile_L{line}_C{column} __AnonymousField_ClangUnsavedFile_L{line}_C{column};

        public partial struct __AnonymousRecord_ClangUnsavedFile_L{line}_C{column}
        {{
            public {expectedManagedType} a;
        }}
    }}
}}
";

            var expectedDiagnostics = new Diagnostic[] {
                new Diagnostic(DiagnosticLevel.Info, $"Anonymous declaration found in 'GetCursorName'. Falling back to '__AnonymousRecord_ClangUnsavedFile_L{line}_C{column}'.", $"Line {line}, Column {column} in ClangUnsavedFile.h")
            };
            await ValidateGeneratedBindings(inputContents, expectedOutputContents, expectedDiagnostics: expectedDiagnostics);
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

        [Fact]
        public async Task RemapNestedAnonymousTest()
        {
            var inputContents = @"struct MyStruct
{
    double r;
    double g;
    double b;

    struct
    {
        double a;
    };
};";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public double r;

        public double g;

        public double b;

        [NativeTypeName(""MyStruct::(anonymous struct at ClangUnsavedFile.h:7:5)"")]
        public _Anonymous_e__Struct Anonymous;

        public partial struct _Anonymous_e__Struct
        {
            public double a;
        }
    }
}
";

            var remappedNames = new Dictionary<string, string> {
                ["__AnonymousField_ClangUnsavedFile_L7_C5"] = "Anonymous",
                ["__AnonymousRecord_ClangUnsavedFile_L7_C5"] = "_Anonymous_e__Struct"
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
