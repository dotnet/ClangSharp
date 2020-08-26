// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
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

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task BitfieldWindowsTest()
        {
            var inputContents = @"struct MyStruct1
{
    unsigned int o0_b0_24 : 24;
    unsigned int o4_b0_16 : 16;
    unsigned int o4_b16_3 : 3;
    int o4_b19_3 : 3;
    unsigned char o8_b0_1 : 1;
    int o12_b0_1 : 1;
    int o12_b1_1 : 1;
};

struct MyStruct2
{
    unsigned int o0_b0_1 : 1;
    int x;
    unsigned int o8_b0_1 : 1;
};

struct MyStruct3
{
    unsigned int o0_b0_1 : 1;
    unsigned int o0_b1_1 : 1;
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct1
    {
        public uint _bitfield1;

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

        public uint _bitfield2;

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

        [NativeTypeName(""int : 3"")]
        public int o4_b19_3
        {
            get
            {
                return (int)((_bitfield2 >> 19) & 0x7u);
            }

            set
            {
                _bitfield2 = (_bitfield2 & ~(0x7u << 19)) | (uint)((value & 0x7) << 19);
            }
        }

        public byte _bitfield3;

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

        public int _bitfield4;

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
        public uint _bitfield1;

        [NativeTypeName(""unsigned int : 1"")]
        public uint o0_b0_1
        {
            get
            {
                return _bitfield1 & 0x1u;
            }

            set
            {
                _bitfield1 = (_bitfield1 & ~0x1u) | (value & 0x1u);
            }
        }

        public int x;

        public uint _bitfield2;

        [NativeTypeName(""unsigned int : 1"")]
        public uint o8_b0_1
        {
            get
            {
                return _bitfield2 & 0x1u;
            }

            set
            {
                _bitfield2 = (_bitfield2 & ~0x1u) | (value & 0x1u);
            }
        }
    }

    public partial struct MyStruct3
    {
        public uint _bitfield;

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

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task BitfieldUnixTest()
        {
            var inputContents = @"struct MyStruct1
{
    unsigned int o0_b0_24 : 24;
    unsigned int o4_b0_16 : 16;
    unsigned int o4_b16_3 : 3;
    int o4_b19_3 : 3;
    unsigned char o4_b22_1 : 1;
    int o4_b23_1 : 1;
    int o4_b24_1 : 1;
};

struct MyStruct2
{
    unsigned int o0_b0_1 : 1;
    int x;
    unsigned int o8_b0_1 : 1;
};

struct MyStruct3
{
    unsigned int o0_b0_1 : 1;
    unsigned int o0_b1_1 : 1;
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct1
    {
        public uint _bitfield1;

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

        public uint _bitfield2;

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

        [NativeTypeName(""int : 3"")]
        public int o4_b19_3
        {
            get
            {
                return (int)((_bitfield2 >> 19) & 0x7u);
            }

            set
            {
                _bitfield2 = (_bitfield2 & ~(0x7u << 19)) | (uint)((value & 0x7) << 19);
            }
        }

        [NativeTypeName(""unsigned char : 1"")]
        public byte o4_b22_1
        {
            get
            {
                return (byte)((_bitfield2 >> 22) & 0x1u);
            }

            set
            {
                _bitfield2 = (_bitfield2 & ~(0x1u << 22)) | (uint)((value & 0x1u) << 22);
            }
        }

        [NativeTypeName(""int : 1"")]
        public int o4_b23_1
        {
            get
            {
                return (int)((_bitfield2 >> 23) & 0x1u);
            }

            set
            {
                _bitfield2 = (_bitfield2 & ~(0x1u << 23)) | (uint)((value & 0x1) << 23);
            }
        }

        [NativeTypeName(""int : 1"")]
        public int o4_b24_1
        {
            get
            {
                return (int)((_bitfield2 >> 24) & 0x1u);
            }

            set
            {
                _bitfield2 = (_bitfield2 & ~(0x1u << 24)) | (uint)((value & 0x1) << 24);
            }
        }
    }

    public partial struct MyStruct2
    {
        public uint _bitfield1;

        [NativeTypeName(""unsigned int : 1"")]
        public uint o0_b0_1
        {
            get
            {
                return _bitfield1 & 0x1u;
            }

            set
            {
                _bitfield1 = (_bitfield1 & ~0x1u) | (value & 0x1u);
            }
        }

        public int x;

        public uint _bitfield2;

        [NativeTypeName(""unsigned int : 1"")]
        public uint o8_b0_1
        {
            get
            {
                return _bitfield2 & 0x1u;
            }

            set
            {
                _bitfield2 = (_bitfield2 & ~0x1u) | (value & 0x1u);
            }
        }
    }

    public partial struct MyStruct3
    {
        public uint _bitfield;

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

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateUnixTypes);
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

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task ExcludeTest()
        {
            var inputContents = "typedef struct MyStruct MyStruct;";
            var expectedOutputContents = string.Empty;

            var excludedNames = new string[] { "MyStruct" };
            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, excludedNames: excludedNames);
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
            public MyStruct e0;
            public MyStruct e1;
            public MyStruct e2;

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

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode);
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
            public MyStruct e0;
            public MyStruct e1;
            public MyStruct e2;

            public ref MyStruct this[int index]
            {{
                get
                {{
                    return ref AsSpan()[index];
                }}
            }}

            public Span<MyStruct> AsSpan() => MemoryMarshal.CreateSpan(ref e0, 3);
        }}
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
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
            public MyStruct e0_0_0_0;
            public MyStruct e1_0_0_0;

            public MyStruct e0_0_1_0;
            public MyStruct e1_0_1_0;

            public MyStruct e0_0_2_0;
            public MyStruct e1_0_2_0;

            public MyStruct e0_0_0_1;
            public MyStruct e1_0_0_1;

            public MyStruct e0_0_1_1;
            public MyStruct e1_0_1_1;

            public MyStruct e0_0_2_1;
            public MyStruct e1_0_2_1;

            public MyStruct e0_0_0_2;
            public MyStruct e1_0_0_2;

            public MyStruct e0_0_1_2;
            public MyStruct e1_0_1_2;

            public MyStruct e0_0_2_2;
            public MyStruct e1_0_2_2;

            public MyStruct e0_0_0_3;
            public MyStruct e1_0_0_3;

            public MyStruct e0_0_1_3;
            public MyStruct e1_0_1_3;

            public MyStruct e0_0_2_3;
            public MyStruct e1_0_2_3;

            public ref MyStruct this[int index]
            {{
                get
                {{
                    return ref AsSpan()[index];
                }}
            }}

            public Span<MyStruct> AsSpan() => MemoryMarshal.CreateSpan(ref e0, 24);
        }}
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("double", "double")]
        [InlineData("short", "short")]
        [InlineData("int", "int")]
        [InlineData("float", "float")]
        public async Task FixedSizedBufferNonPrimitiveTypedefTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"struct MyStruct
{{
    {nativeType} value;
}};

typedef MyStruct MyBuffer[3];

struct MyOtherStruct
{{
    MyBuffer c;
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
        [NativeTypeName(""MyBuffer"")]
        public _c_e__FixedBuffer c;

        public partial struct _c_e__FixedBuffer
        {{
            public MyStruct e0;
            public MyStruct e1;
            public MyStruct e2;

            public ref MyStruct this[int index]
            {{
                get
                {{
                    return ref AsSpan()[index];
                }}
            }}

            public Span<MyStruct> AsSpan() => MemoryMarshal.CreateSpan(ref e0, 3);
        }}
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
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
            public MyStruct e0;
            public MyStruct e1;
            public MyStruct e2;

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

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode);
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
            public MyStruct e0;
            public MyStruct e1;
            public MyStruct e2;

            public ref MyStruct this[int index]
            {{
                get
                {{
                    return ref AsSpan()[index];
                }}
            }}

            public Span<MyStruct> AsSpan() => MemoryMarshal.CreateSpan(ref e0, 3);
        }}
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("double *", "double*")]
        [InlineData("short *", "short*")]
        [InlineData("int *", "int*")]
        [InlineData("float *", "float*")]
        public async Task FixedSizedBufferPointerTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"struct MyStruct
{{
    {nativeType} c[3];
}};
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        [NativeTypeName(""{nativeType}[3]"")]
        public _c_e__FixedBuffer c;

        public unsafe partial struct _c_e__FixedBuffer
        {{
            public {expectedManagedType} e0;
            public {expectedManagedType} e1;
            public {expectedManagedType} e2;

            public ref {expectedManagedType} this[int index]
            {{
                get
                {{
                    fixed ({expectedManagedType}* pThis = &e0)
                    {{
                        return ref pThis[index];
                    }}
                }}
            }}
        }}
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
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

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
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

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
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
        public async Task FixedSizedBufferPrimitiveTypedefTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"typedef {nativeType} MyBuffer[3];

struct MyStruct
{{
    MyBuffer c;
}};
";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public unsafe partial struct MyStruct
    {{
        [NativeTypeName(""MyBuffer"")]
        public fixed {expectedManagedType} c[3];
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task GuidTest()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Unix doesn't support __declspec(uuid(""))
                return;
            }

            var inputContents = $@"#define DECLSPEC_UUID(x) __declspec(uuid(x))

struct __declspec(uuid(""00000000-0000-0000-C000-000000000046"")) MyStruct1
{{
    int x;
}};

struct DECLSPEC_UUID(""00000000-0000-0000-C000-000000000047"") MyStruct2
{{
    int x;
}};
";

            var expectedOutputContents = $@"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [Guid(""00000000-0000-0000-C000-000000000046"")]
    public partial struct MyStruct1
    {{
        public int x;
    }}

    [Guid(""00000000-0000-0000-C000-000000000047"")]
    public partial struct MyStruct2
    {{
        public int x;
    }}

    public static partial class Methods
    {{
        public static readonly Guid IID_MyStruct1 = new Guid(0x00000000, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        public static readonly Guid IID_MyStruct2 = new Guid(0x00000000, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x47);
    }}
}}
";

            var excludedNames = new string[] { "DECLSPEC_UUID" };
            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, excludedNames: excludedNames);
        }

        [Fact]
        public async Task InheritanceTest()
        {
            var inputContents = @"struct MyStruct1A
{
    int x;
    int y;
};

struct MyStruct1B
{
    int x;
    int y;
};

struct MyStruct2 : MyStruct1A, MyStruct1B
{
    int z;
    int w;
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct1A
    {
        public int x;

        public int y;
    }

    public partial struct MyStruct1B
    {
        public int x;

        public int y;
    }

    [NativeTypeName(""struct MyStruct2 : MyStruct1A, MyStruct1B"")]
    public partial struct MyStruct2
    {
        public MyStruct1A __AnonymousBase_ClangUnsavedFile_L13_C20;

        public MyStruct1B __AnonymousBase_ClangUnsavedFile_L13_C32;

        public int z;

        public int w;
    }
}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("double", "double", 10, 5)]
        [InlineData("short", "short", 10, 5)]
        [InlineData("int", "int", 10, 5)]
        [InlineData("float", "float", 10, 5)]
        public async Task NestedAnonymousTest(string nativeType, string expectedManagedType, int line, int column)
        {
            var inputContents = $@"typedef union {{
    {nativeType} value;
}} MyUnion;

struct MyStruct
{{
    {nativeType} x;
    {nativeType} y;

    struct
    {{
        {nativeType} z;

        struct
        {{
            {nativeType} value;
        }} w;

        MyUnion u;
        {nativeType} buffer1[4];
        MyUnion buffer2[4];
    }};
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

    public unsafe partial struct MyStruct
    {{
        public {expectedManagedType} x;

        public {expectedManagedType} y;

        [NativeTypeName(""MyStruct::(anonymous struct at ClangUnsavedFile.h:{line}:{column})"")]
        public _Anonymous_e__Struct Anonymous;

        public ref {expectedManagedType} z
        {{
            get
            {{
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.z, 1));
            }}
        }}

        public ref _Anonymous_e__Struct._w_e__Struct w
        {{
            get
            {{
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.w, 1));
            }}
        }}

        public ref MyUnion u
        {{
            get
            {{
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.u, 1));
            }}
        }}

        public Span<{expectedManagedType}> buffer1
        {{
            get
            {{
                return MemoryMarshal.CreateSpan(ref Anonymous.buffer1[0], 4);
            }}
        }}

        public Span<MyUnion> buffer2
        {{
            get
            {{
                return Anonymous.buffer2.AsSpan();
            }}
        }}

        public unsafe partial struct _Anonymous_e__Struct
        {{
            public {expectedManagedType} z;

            [NativeTypeName(""struct (anonymous struct at ClangUnsavedFile.h:14:9)"")]
            public _w_e__Struct w;

            public MyUnion u;

            [NativeTypeName(""{nativeType} [4]"")]
            public fixed {expectedManagedType} buffer1[4];

            [NativeTypeName(""MyUnion [4]"")]
            public _buffer2_e__FixedBuffer buffer2;

            public partial struct _w_e__Struct
            {{
                public {expectedManagedType} value;
            }}

            public partial struct _buffer2_e__FixedBuffer
            {{
                public MyUnion e0;
                public MyUnion e1;
                public MyUnion e2;
                public MyUnion e3;

                public ref MyUnion this[int index]
                {{
                    get
                    {{
                        return ref AsSpan()[index];
                    }}
                }}

                public Span<MyUnion> AsSpan() => MemoryMarshal.CreateSpan(ref e0, 4);
            }}
        }}
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Theory]
        [InlineData("double", "double", 10, 5)]
        [InlineData("short", "short", 10, 5)]
        [InlineData("int", "int", 10, 5)]
        [InlineData("float", "float", 10, 5)]
        public async Task NestedAnonymousCompatibleTest(string nativeType, string expectedManagedType, int line, int column)
        {
            var inputContents = $@"typedef union {{
    {nativeType} value;
}} MyUnion;

struct MyStruct
{{
    {nativeType} x;
    {nativeType} y;

    struct
    {{
        {nativeType} z;

        struct
        {{
            {nativeType} value;
        }} w;

        MyUnion u;
        {nativeType} buffer1[4];
        MyUnion buffer2[4];
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
        public {expectedManagedType} value;
    }}

    public unsafe partial struct MyStruct
    {{
        public {expectedManagedType} x;

        public {expectedManagedType} y;

        [NativeTypeName(""MyStruct::(anonymous struct at ClangUnsavedFile.h:{line}:{column})"")]
        public _Anonymous_e__Struct Anonymous;

        public ref {expectedManagedType} z
        {{
            get
            {{
                fixed (_Anonymous_e__Struct* pField = &Anonymous)
                {{
                    return ref pField->z;
                }}
            }}
        }}

        public ref _Anonymous_e__Struct._w_e__Struct w
        {{
            get
            {{
                fixed (_Anonymous_e__Struct* pField = &Anonymous)
                {{
                    return ref pField->w;
                }}
            }}
        }}

        public ref MyUnion u
        {{
            get
            {{
                fixed (_Anonymous_e__Struct* pField = &Anonymous)
                {{
                    return ref pField->u;
                }}
            }}
        }}

        public ref {expectedManagedType} buffer1
        {{
            get
            {{
                fixed (_Anonymous_e__Struct* pField = &Anonymous)
                {{
                    return ref pField->buffer1[0];
                }}
            }}
        }}

        public ref _Anonymous_e__Struct._buffer2_e__FixedBuffer buffer2
        {{
            get
            {{
                fixed (_Anonymous_e__Struct* pField = &Anonymous)
                {{
                    return ref pField->buffer2;
                }}
            }}
        }}

        public unsafe partial struct _Anonymous_e__Struct
        {{
            public {expectedManagedType} z;

            [NativeTypeName(""struct (anonymous struct at ClangUnsavedFile.h:14:9)"")]
            public _w_e__Struct w;

            public MyUnion u;

            [NativeTypeName(""{nativeType} [4]"")]
            public fixed {expectedManagedType} buffer1[4];

            [NativeTypeName(""MyUnion [4]"")]
            public _buffer2_e__FixedBuffer buffer2;

            public partial struct _w_e__Struct
            {{
                public {expectedManagedType} value;
            }}

            public partial struct _buffer2_e__FixedBuffer
            {{
                public MyUnion e0;
                public MyUnion e1;
                public MyUnion e2;
                public MyUnion e3;

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
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode);
        }

        [Fact]
        public async Task NestedAnonymousWithBitfieldTest()
        {
            var inputContents = @"struct MyStruct
{
    int x;
    int y;

    struct
    {
        int z;

        struct
        {
            int w;
            int o0_b0_16 : 16;
            int o0_b16_4 : 4;
        };
    };
};
";

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public int x;

        public int y;

        [NativeTypeName(""MyStruct::(anonymous struct at ClangUnsavedFile.h:6:5)"")]
        public _Anonymous_e__Struct Anonymous;

        public ref int z
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.z, 1));
            }
        }

        public ref int w
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.Anonymous.w, 1));
            }
        }

        public int o0_b0_16
        {
            get
            {
                return Anonymous.Anonymous.o0_b0_16;
            }

            set
            {
                Anonymous.Anonymous.o0_b0_16 = value;
            }
        }

        public int o0_b16_4
        {
            get
            {
                return Anonymous.Anonymous.o0_b16_4;
            }

            set
            {
                Anonymous.Anonymous.o0_b16_4 = value;
            }
        }

        public partial struct _Anonymous_e__Struct
        {
            public int z;

            [NativeTypeName(""MyStruct::(anonymous struct at ClangUnsavedFile.h:10:9)"")]
            public _Anonymous_e__Struct Anonymous;

            public partial struct _Anonymous_e__Struct
            {
                public int w;

                public int _bitfield;

                [NativeTypeName(""int : 16"")]
                public int o0_b0_16
                {
                    get
                    {
                        return _bitfield & 0xFFFF;
                    }

                    set
                    {
                        _bitfield = (_bitfield & ~0xFFFF) | (value & 0xFFFF);
                    }
                }

                [NativeTypeName(""int : 4"")]
                public int o0_b16_4
                {
                    get
                    {
                        return (_bitfield >> 16) & 0xF;
                    }

                    set
                    {
                        _bitfield = (_bitfield & ~(0xF << 16)) | ((value & 0xF) << 16);
                    }
                }
            }
        }
    }
}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task NewKeywordTest()
        {
            var inputContents = @"struct MyStruct
{
    int Equals;
    int Finalize;
    int GetHashCode;
    int GetType;
    int MemberwiseClone;
    int ReferenceEquals;
    int ToString;
};";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        public new int Equals;

        public int Finalize;

        public new int GetHashCode;

        public new int GetType;

        public new int MemberwiseClone;

        public new int ReferenceEquals;

        public new int ToString;
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
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

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
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

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
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
            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task PointerToSelfTest()
        {
            var inputContents = @"struct example_s {
   example_s* next;
   void* data;
};";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public unsafe partial struct example_s
    {{
        [NativeTypeName(""example_s *"")]
        public example_s* next;

        [NativeTypeName(""void *"")]
        public void* data;
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task PointerToSelfViaTypedefTest()
        {
            var inputContents = @"typedef struct example_s example_t;

struct example_s {
   example_t* next;
   void* data;
};";

            var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public unsafe partial struct example_s
    {{
        [NativeTypeName(""example_t *"")]
        public example_s* next;

        [NativeTypeName(""void *"")]
        public void* data;
    }}
}}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
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
            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, remappedNames: remappedNames);
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

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        public double r;

        public double g;

        public double b;

        [NativeTypeName(""MyStruct::(anonymous struct at ClangUnsavedFile.h:7:5)"")]
        public _Anonymous_e__Struct Anonymous;

        public ref double a
        {
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.a, 1));
            }
        }

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
            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, remappedNames: remappedNames);
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

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task SkipNonDefinitionPointerTest()
        {
            var inputContents = @"typedef struct MyStruct* MyStructPtr;
typedef struct MyStruct& MyStructRef;
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
    }
}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
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

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
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

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }

        [Fact]
        public async Task UsingDeclarationTest()
        {
            var inputContents = @"struct MyStruct1A
{
    void MyMethod() { }
};

struct MyStruct1B : MyStruct1A
{
    using MyStruct1A::MyMethod;
};
";

            var expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct1A
    {
        public void MyMethod()
        {
        }
    }

    [NativeTypeName(""struct MyStruct1B : MyStruct1A"")]
    public partial struct MyStruct1B
    {
        public void MyMethod()
        {
        }
    }
}
";

            await ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents);
        }
    }
}
