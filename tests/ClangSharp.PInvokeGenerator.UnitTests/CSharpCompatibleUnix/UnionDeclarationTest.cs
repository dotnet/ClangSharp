// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class CSharpCompatibleUnix_UnionDeclarationTest : UnionDeclarationTest
    {
        public override Task BasicTest(string nativeType, string expectedManagedType)
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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task BasicTestInCMode(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"typedef union
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}}  MyUnion;
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
            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents, commandlineArgs: Array.Empty<string>());
        }

        public override Task BasicWithNativeTypeNameTest(string nativeType, string expectedManagedType)
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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task BitfieldTest()
        {
            var inputContents = @"union MyUnion1
{
    unsigned int o0_b0_24 : 24;
    unsigned int o4_b0_16 : 16;
    unsigned int o4_b16_3 : 3;
    int o4_b19_3 : 3;
    unsigned char o4_b22_1 : 1;
    int o4_b23_1 : 1;
    int o4_b24_1 : 1;
};

union MyUnion2
{
    unsigned int o0_b0_1 : 1;
    int x;
    unsigned int o8_b0_1 : 1;
};

union MyUnion3
{
    unsigned int o0_b0_1 : 1;
    unsigned int o0_b1_1 : 1;
};
";
            string expectedPack = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ", Pack = 1" : "";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit{expectedPack})]
    public partial struct MyUnion1
    {{
        [FieldOffset(0)]
        public uint _bitfield1;

        [NativeTypeName(""unsigned int : 24"")]
        public uint o0_b0_24
        {{
            get
            {{
                return _bitfield1 & 0xFFFFFFu;
            }}

            set
            {{
                _bitfield1 = (_bitfield1 & ~0xFFFFFFu) | (value & 0xFFFFFFu);
            }}
        }}

        [FieldOffset(0)]
        public uint _bitfield2;

        [NativeTypeName(""unsigned int : 16"")]
        public uint o4_b0_16
        {{
            get
            {{
                return _bitfield2 & 0xFFFFu;
            }}

            set
            {{
                _bitfield2 = (_bitfield2 & ~0xFFFFu) | (value & 0xFFFFu);
            }}
        }}

        [NativeTypeName(""unsigned int : 3"")]
        public uint o4_b16_3
        {{
            get
            {{
                return (_bitfield2 >> 16) & 0x7u;
            }}

            set
            {{
                _bitfield2 = (_bitfield2 & ~(0x7u << 16)) | ((value & 0x7u) << 16);
            }}
        }}

        [NativeTypeName(""int : 3"")]
        public int o4_b19_3
        {{
            get
            {{
                return (int)((_bitfield2 >> 19) & 0x7u);
            }}

            set
            {{
                _bitfield2 = (_bitfield2 & ~(0x7u << 19)) | (uint)((value & 0x7) << 19);
            }}
        }}

        [NativeTypeName(""unsigned char : 1"")]
        public byte o4_b22_1
        {{
            get
            {{
                return (byte)((_bitfield2 >> 22) & 0x1u);
            }}

            set
            {{
                _bitfield2 = (_bitfield2 & ~(0x1u << 22)) | (uint)((value & 0x1u) << 22);
            }}
        }}

        [NativeTypeName(""int : 1"")]
        public int o4_b23_1
        {{
            get
            {{
                return (int)((_bitfield2 >> 23) & 0x1u);
            }}

            set
            {{
                _bitfield2 = (_bitfield2 & ~(0x1u << 23)) | (uint)((value & 0x1) << 23);
            }}
        }}

        [NativeTypeName(""int : 1"")]
        public int o4_b24_1
        {{
            get
            {{
                return (int)((_bitfield2 >> 24) & 0x1u);
            }}

            set
            {{
                _bitfield2 = (_bitfield2 & ~(0x1u << 24)) | (uint)((value & 0x1) << 24);
            }}
        }}
    }}

    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion2
    {{
        [FieldOffset(0)]
        public uint _bitfield1;

        [NativeTypeName(""unsigned int : 1"")]
        public uint o0_b0_1
        {{
            get
            {{
                return _bitfield1 & 0x1u;
            }}

            set
            {{
                _bitfield1 = (_bitfield1 & ~0x1u) | (value & 0x1u);
            }}
        }}

        [FieldOffset(0)]
        public int x;

        [FieldOffset(0)]
        public uint _bitfield2;

        [NativeTypeName(""unsigned int : 1"")]
        public uint o8_b0_1
        {{
            get
            {{
                return _bitfield2 & 0x1u;
            }}

            set
            {{
                _bitfield2 = (_bitfield2 & ~0x1u) | (value & 0x1u);
            }}
        }}
    }}

    [StructLayout(LayoutKind.Explicit{expectedPack})]
    public partial struct MyUnion3
    {{
        [FieldOffset(0)]
        public uint _bitfield;

        [NativeTypeName(""unsigned int : 1"")]
        public uint o0_b0_1
        {{
            get
            {{
                return _bitfield & 0x1u;
            }}

            set
            {{
                _bitfield = (_bitfield & ~0x1u) | (value & 0x1u);
            }}
        }}

        [NativeTypeName(""unsigned int : 1"")]
        public uint o0_b1_1
        {{
            get
            {{
                return (_bitfield >> 1) & 0x1u;
            }}

            set
            {{
                _bitfield = (_bitfield & ~(0x1u << 1)) | ((value & 0x1u) << 1);
            }}
        }}
    }}
}}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ExcludeTest()
        {
            var inputContents = "typedef union MyUnion MyUnion;";
            var expectedOutputContents = string.Empty;

            var excludedNames = new string[] { "MyUnion" };
            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents, excludedNames: excludedNames);
        }

        public override Task FixedSizedBufferNonPrimitiveTest(string nativeType, string expectedManagedType)
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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task FixedSizedBufferNonPrimitiveMultidimensionalTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"union MyUnion
{{
    {nativeType} value;
}};

union MyOtherUnion
{{
    MyUnion c[2][1][3][4];
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
        [NativeTypeName(""MyUnion [2][1][3][4]"")]
        public _c_e__FixedBuffer c;

        public partial struct _c_e__FixedBuffer
        {{
            public MyUnion e0_0_0_0;
            public MyUnion e1_0_0_0;

            public MyUnion e0_0_1_0;
            public MyUnion e1_0_1_0;

            public MyUnion e0_0_2_0;
            public MyUnion e1_0_2_0;

            public MyUnion e0_0_0_1;
            public MyUnion e1_0_0_1;

            public MyUnion e0_0_1_1;
            public MyUnion e1_0_1_1;

            public MyUnion e0_0_2_1;
            public MyUnion e1_0_2_1;

            public MyUnion e0_0_0_2;
            public MyUnion e1_0_0_2;

            public MyUnion e0_0_1_2;
            public MyUnion e1_0_1_2;

            public MyUnion e0_0_2_2;
            public MyUnion e1_0_2_2;

            public MyUnion e0_0_0_3;
            public MyUnion e1_0_0_3;

            public MyUnion e0_0_1_3;
            public MyUnion e1_0_1_3;

            public MyUnion e0_0_2_3;
            public MyUnion e1_0_2_3;

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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task FixedSizedBufferNonPrimitiveTypedefTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"union MyUnion
{{
    {nativeType} value;
}};

typedef MyUnion MyBuffer[3];

union MyOtherUnion
{{
    MyBuffer c;
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
        [NativeTypeName(""MyBuffer"")]
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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task FixedSizedBufferNonPrimitiveWithNativeTypeNameTest(string nativeType, string expectedManagedType)
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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task FixedSizedBufferPointerTest(string nativeType, string expectedManagedType)
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
    public partial struct MyUnion
    {{
        [FieldOffset(0)]
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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task FixedSizedBufferPrimitiveTest(string nativeType, string expectedManagedType)
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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task FixedSizedBufferPrimitiveMultidimensionalTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"union MyUnion
{{
    {nativeType} c[2][1][3][4];
}};
";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct MyUnion
    {{
        [FieldOffset(0)]
        [NativeTypeName(""{nativeType} [2][1][3][4]"")]
        public fixed {expectedManagedType} c[2 * 1 * 3 * 4];
    }}
}}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task FixedSizedBufferPrimitiveTypedefTest(string nativeType, string expectedManagedType)
        {
            var inputContents = $@"typedef {nativeType} MyBuffer[3];

union MyUnion
{{
    MyBuffer c;
}};
";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct MyUnion
    {{
        [FieldOffset(0)]
        [NativeTypeName(""MyBuffer"")]
        public fixed {expectedManagedType} c[3];
    }}
}}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }
        public override Task GuidTest()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Non-Windows doesn't support __declspec(uuid(""))
                return Task.CompletedTask;
            }

            var inputContents = $@"#define DECLSPEC_UUID(x) __declspec(uuid(x))

union __declspec(uuid(""00000000-0000-0000-C000-000000000046"")) MyUnion1
{{
    int x;
}};

union DECLSPEC_UUID(""00000000-0000-0000-C000-000000000047"") MyUnion2
{{
    int x;
}};
";

            var expectedOutputContents = $@"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    [Guid(""00000000-0000-0000-C000-000000000046"")]
    public partial struct MyUnion1
    {{
        [FieldOffset(0)]
        public int x;
    }}

    [StructLayout(LayoutKind.Explicit)]
    [Guid(""00000000-0000-0000-C000-000000000047"")]
    public partial struct MyUnion2
    {{
        [FieldOffset(0)]
        public int x;
    }}

    public static partial class Methods
    {{
        public static readonly Guid IID_MyUnion1 = new Guid(0x00000000, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

        public static readonly Guid IID_MyUnion2 = new Guid(0x00000000, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x47);
    }}
}}
";

            var excludedNames = new string[] { "DECLSPEC_UUID" };
            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents, excludedNames: excludedNames);
        }

        public override Task NestedAnonymousTest(string nativeType, string expectedManagedType, int line, int column)
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

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

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
                fixed (_Anonymous_e__Union* pField = &Anonymous)
                {{
                    return ref pField->a;
                }}
            }}
        }}

        public ref MyStruct s
        {{
            get
            {{
                fixed (_Anonymous_e__Union* pField = &Anonymous)
                {{
                    return ref pField->s;
                }}
            }}
        }}

        public ref {expectedManagedType} buffer
        {{
            get
            {{
                fixed (_Anonymous_e__Union* pField = &Anonymous)
                {{
                    return ref pField->buffer[0];
                }}
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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task NestedAnonymousWithBitfieldTest()
        {
            var inputContents = @"union MyUnion
{
    int x;
    int y;

    union
    {
        int z;

        union
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
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        public int x;

        [FieldOffset(0)]
        public int y;

        [FieldOffset(0)]
        [NativeTypeName(""MyUnion::(anonymous union at ClangUnsavedFile.h:6:5)"")]
        public _Anonymous_e__Union Anonymous;

        public ref int z
        {
            get
            {
                fixed (_Anonymous_e__Union* pField = &Anonymous)
                {
                    return ref pField->z;
                }
            }
        }

        public ref int w
        {
            get
            {
                fixed (_Anonymous_e__Union._Anonymous_e__Union* pField = &Anonymous.Anonymous)
                {
                    return ref pField->w;
                }
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

        [StructLayout(LayoutKind.Explicit)]
        public partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            public int z;

            [FieldOffset(0)]
            [NativeTypeName(""MyUnion::(anonymous union at ClangUnsavedFile.h:10:9)"")]
            public _Anonymous_e__Union Anonymous;

            [StructLayout(LayoutKind.Explicit)]
            public partial struct _Anonymous_e__Union
            {
                [FieldOffset(0)]
                public int w;

                [FieldOffset(0)]
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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }


        public override Task NestedTest(string nativeType, string expectedManagedType)
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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task NestedWithNativeTypeNameTest(string nativeType, string expectedManagedType)
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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task NewKeywordTest()
        {
            var inputContents = @"union MyUnion
{
    int Equals;
    int Finalize;
    int GetHashCode;
    int GetType;
    int MemberwiseClone;
    int ReferenceEquals;
    int ToString;
};";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {{
        [FieldOffset(0)]
        public new int Equals;

        [FieldOffset(0)]
        public int Finalize;

        [FieldOffset(0)]
        public new int GetHashCode;

        [FieldOffset(0)]
        public new int GetType;

        [FieldOffset(0)]
        public new int MemberwiseClone;

        [FieldOffset(0)]
        public new int ReferenceEquals;

        [FieldOffset(0)]
        public new int ToString;
    }}
}}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task NoDefinitionTest()
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
            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task PointerToSelfTest()
        {
            var inputContents = @"union example_s {
   example_s* next;
   void* data;
};";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct example_s
    {{
        [FieldOffset(0)]
        [NativeTypeName(""example_s *"")]
        public example_s* next;

        [FieldOffset(0)]
        [NativeTypeName(""void *"")]
        public void* data;
    }}
}}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task PointerToSelfViaTypedefTest()
        {
            var inputContents = @"typedef union example_s example_t;

union example_s {
   example_t* next;
   void* data;
};";

            var expectedOutputContents = $@"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct example_s
    {{
        [FieldOffset(0)]
        [NativeTypeName(""example_t *"")]
        public example_s* next;

        [FieldOffset(0)]
        [NativeTypeName(""void *"")]
        public void* data;
    }}
}}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task RemapTest()
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
            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents, remappedNames: remappedNames);
        }

        public override Task RemapNestedAnonymousTest()
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
                fixed (_Anonymous_e__Union* pField = &Anonymous)
                {
                    return ref pField->a;
                }
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
            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents, remappedNames: remappedNames);
        }

        public override Task SkipNonDefinitionTest(string nativeType, string expectedManagedType)
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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task SkipNonDefinitionPointerTest()
        {
            var inputContents = @"typedef union MyUnion* MyUnionPtr;
typedef union MyUnion& MyUnionRef;
";

            var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
    }
}
";

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task SkipNonDefinitionWithNativeTypeNameTest(string nativeType, string expectedManagedType)
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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task TypedefTest(string nativeType, string expectedManagedType)
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

            return ValidateGeneratedCSharpCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
        }
    }
}
