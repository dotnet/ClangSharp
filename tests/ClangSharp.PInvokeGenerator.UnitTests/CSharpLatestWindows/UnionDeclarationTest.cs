// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[Platform("win")]
public sealed class CSharpLatestWindows_UnionDeclarationTest : UnionDeclarationTest
{
    protected override Task BasicTestImpl(string nativeType, string expectedManagedType)
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

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task BasicTestInCModeImpl(string nativeType, string expectedManagedType)
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
        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, commandLineArgs: []);
    }

    protected override Task BasicWithNativeTypeNameTestImpl(string nativeType, string expectedManagedType)
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

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task BitfieldTestImpl()
    {
        var inputContents = @"union MyUnion1
{
    unsigned int o0_b0_24 : 24;
    unsigned int o4_b0_16 : 16;
    unsigned int o4_b16_3 : 3;
    int o4_b19_3 : 3;
    unsigned char o8_b0_1 : 1;
    int o12_b0_1 : 1;
    int o12_b1_1 : 1;
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

        var expectedPack = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ", Pack = 1" : "";

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
            readonly get
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
            readonly get
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
            readonly get
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
            readonly get
            {{
                return (int)(_bitfield2 << 10) >> 29;
            }}

            set
            {{
                _bitfield2 = (_bitfield2 & ~(0x7u << 19)) | (uint)((value & 0x7) << 19);
            }}
        }}

        [FieldOffset(0)]
        public byte _bitfield3;

        [NativeTypeName(""unsigned char : 1"")]
        public byte o8_b0_1
        {{
            readonly get
            {{
                return (byte)(_bitfield3 & 0x1u);
            }}

            set
            {{
                _bitfield3 = (byte)((_bitfield3 & ~0x1u) | (value & 0x1u));
            }}
        }}

        [FieldOffset(0)]
        public int _bitfield4;

        [NativeTypeName(""int : 1"")]
        public int o12_b0_1
        {{
            readonly get
            {{
                return (_bitfield4 << 31) >> 31;
            }}

            set
            {{
                _bitfield4 = (_bitfield4 & ~0x1) | (value & 0x1);
            }}
        }}

        [NativeTypeName(""int : 1"")]
        public int o12_b1_1
        {{
            readonly get
            {{
                return (_bitfield4 << 30) >> 31;
            }}

            set
            {{
                _bitfield4 = (_bitfield4 & ~(0x1 << 1)) | ((value & 0x1) << 1);
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
            readonly get
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
            readonly get
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
            readonly get
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
            readonly get
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

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ExcludeTestImpl()
    {
        var inputContents = "typedef union MyUnion MyUnion;";
        var expectedOutputContents = string.Empty;
        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, excludedNames: ExcludeTestExcludedNames);
    }

    protected override Task FixedSizedBufferNonPrimitiveTestImpl(string nativeType, string expectedManagedType)
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

        var expectedOutputContents = $@"using System.Runtime.CompilerServices;
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
        [NativeTypeName(""MyUnion[3]"")]
        public _c_e__FixedBuffer c;

        [InlineArray(3)]
        public partial struct _c_e__FixedBuffer
        {{
            public MyUnion e0;
        }}
    }}
}}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FixedSizedBufferNonPrimitiveMultidimensionalTestImpl(string nativeType, string expectedManagedType)
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

        var expectedOutputContents = $@"using System.Runtime.CompilerServices;
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
        [NativeTypeName(""MyUnion[2][1][3][4]"")]
        public _c_e__FixedBuffer c;

        [InlineArray(2 * 1 * 3 * 4)]
        public partial struct _c_e__FixedBuffer
        {{
            public MyUnion e0_0_0_0;
        }}
    }}
}}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FixedSizedBufferNonPrimitiveTypedefTestImpl(string nativeType, string expectedManagedType)
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

        var expectedOutputContents = $@"using System.Runtime.CompilerServices;
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
        [NativeTypeName(""MyBuffer"")]
        public _c_e__FixedBuffer c;

        [InlineArray(3)]
        public partial struct _c_e__FixedBuffer
        {{
            public MyUnion e0;
        }}
    }}
}}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FixedSizedBufferNonPrimitiveWithNativeTypeNameTestImpl(string nativeType, string expectedManagedType)
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

        var expectedOutputContents = $@"using System.Runtime.CompilerServices;
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
        [NativeTypeName(""MyUnion[3]"")]
        public _c_e__FixedBuffer c;

        [InlineArray(3)]
        public partial struct _c_e__FixedBuffer
        {{
            public MyUnion e0;
        }}
    }}
}}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FixedSizedBufferPointerTestImpl(string nativeType, string expectedManagedType)
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

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FixedSizedBufferPrimitiveTestImpl(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"union MyUnion
{{
    {nativeType} c[3];
}};
";

        var expectedOutputContents = $@"using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {{
        [FieldOffset(0)]
        [NativeTypeName(""{nativeType}[3]"")]
        public _c_e__FixedBuffer c;

        [InlineArray(3)]
        public partial struct _c_e__FixedBuffer
        {{
            public {expectedManagedType} e0;
        }}
    }}
}}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FixedSizedBufferPrimitiveMultidimensionalTestImpl(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"union MyUnion
{{
    {nativeType} c[2][1][3][4];
}};
";

        var expectedOutputContents = $@"using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {{
        [FieldOffset(0)]
        [NativeTypeName(""{nativeType}[2][1][3][4]"")]
        public _c_e__FixedBuffer c;

        [InlineArray(2 * 1 * 3 * 4)]
        public partial struct _c_e__FixedBuffer
        {{
            public {expectedManagedType} e0_0_0_0;
        }}
    }}
}}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FixedSizedBufferPrimitiveTypedefTestImpl(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"typedef {nativeType} MyBuffer[3];

union MyUnion
{{
    MyBuffer c;
}};
";

        var expectedOutputContents = $@"using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {{
        [FieldOffset(0)]
        [NativeTypeName(""MyBuffer"")]
        public _c_e__FixedBuffer c;

        [InlineArray(3)]
        public partial struct _c_e__FixedBuffer
        {{
            public {expectedManagedType} e0;
        }}
    }}
}}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }
    protected override Task GuidTestImpl()
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

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, excludedNames: GuidTestExcludedNames);
    }

    protected override Task NestedAnonymousTestImpl(string nativeType, string expectedManagedType, int line, int column)
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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        public {expectedManagedType} value;
    }}

    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {{
        [FieldOffset(0)]
        public {expectedManagedType} r;

        [FieldOffset(0)]
        public {expectedManagedType} g;

        [FieldOffset(0)]
        public {expectedManagedType} b;

        [FieldOffset(0)]
        [NativeTypeName(""__AnonymousRecord_ClangUnsavedFile_L{line}_C{column}"")]
        public _Anonymous_e__Union Anonymous;

        [UnscopedRef]
        public ref {expectedManagedType} a
        {{
            get
            {{
                return ref Anonymous.a;
            }}
        }}

        [UnscopedRef]
        public ref MyStruct s
        {{
            get
            {{
                return ref Anonymous.s;
            }}
        }}

        [UnscopedRef]
        public Span<{expectedManagedType}> buffer
        {{
            get
            {{
                return Anonymous.buffer;
            }}
        }}

        [StructLayout(LayoutKind.Explicit)]
        public partial struct _Anonymous_e__Union
        {{
            [FieldOffset(0)]
            public {expectedManagedType} a;

            [FieldOffset(0)]
            public MyStruct s;

            [FieldOffset(0)]
            [NativeTypeName(""{nativeType}[4]"")]
            public _buffer_e__FixedBuffer buffer;

            [InlineArray(4)]
            public partial struct _buffer_e__FixedBuffer
            {{
                public {expectedManagedType} e0;
            }}
        }}
    }}
}}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task NestedAnonymousWithBitfieldTestImpl()
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

        var expectedOutputContents = @"using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

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
        [NativeTypeName(""__AnonymousRecord_ClangUnsavedFile_L6_C5"")]
        public _Anonymous_e__Union Anonymous;

        [UnscopedRef]
        public ref int z
        {
            get
            {
                return ref Anonymous.z;
            }
        }

        [UnscopedRef]
        public ref int w
        {
            get
            {
                return ref Anonymous.Anonymous.w;
            }
        }

        public int o0_b0_16
        {
            readonly get
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
            readonly get
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
            [NativeTypeName(""__AnonymousRecord_ClangUnsavedFile_L10_C9"")]
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
                    readonly get
                    {
                        return (_bitfield << 16) >> 16;
                    }

                    set
                    {
                        _bitfield = (_bitfield & ~0xFFFF) | (value & 0xFFFF);
                    }
                }

                [NativeTypeName(""int : 4"")]
                public int o0_b16_4
                {
                    readonly get
                    {
                        return (_bitfield << 12) >> 28;
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

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }


    protected override Task NestedTestImpl(string nativeType, string expectedManagedType)
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

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task NestedWithNativeTypeNameTestImpl(string nativeType, string expectedManagedType)
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

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task NewKeywordTestImpl()
    {
        var inputContents = @"union MyUnion
{
    int Equals;
    int Dispose;
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
        public int Dispose;

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

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task NoDefinitionTestImpl()
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
        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task PointerToSelfTestImpl()
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
        public example_s* next;

        [FieldOffset(0)]
        public void* data;
    }}
}}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task PointerToSelfViaTypedefTestImpl()
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
        public void* data;
    }}
}}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task RemapTestImpl()
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
        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, remappedNames: remappedNames);
    }

    protected override Task RemapNestedAnonymousTestImpl()
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

        var expectedOutputContents = @"using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

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
        [NativeTypeName(""__AnonymousRecord_ClangUnsavedFile_L7_C5"")]
        public _Anonymous_e__Union Anonymous;

        [UnscopedRef]
        public ref double a
        {
            get
            {
                return ref Anonymous.a;
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
        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, remappedNames: remappedNames);
    }

    protected override Task SkipNonDefinitionTestImpl(string nativeType, string expectedManagedType)
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

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task SkipNonDefinitionPointerTestImpl()
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

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task SkipNonDefinitionWithNativeTypeNameTestImpl(string nativeType, string expectedManagedType)
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

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task TypedefTestImpl(string nativeType, string expectedManagedType)
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

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents);
    }
}
