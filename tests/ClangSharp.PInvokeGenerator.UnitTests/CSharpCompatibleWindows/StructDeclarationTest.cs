// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[Platform("win")]
public sealed class CSharpCompatibleWindows_StructDeclarationTest : StructDeclarationTest
{
    protected override Task IncompleteArraySizeTestImpl(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} x[];
}};
";

        var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public unsafe partial struct MyStruct
    {{
        [NativeTypeName(""{nativeType}[]"")]
        public fixed {expectedManagedType} x[1];
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task BasicTestImpl(string nativeType, string expectedManagedType)
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

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task BasicTestInCModeImpl(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"typedef struct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}}  MyStruct;
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
        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, commandLineArgs: []);
    }

    protected override Task BasicWithNativeTypeNameTestImpl(string nativeType, string expectedManagedType)
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

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task BitfieldTestImpl()
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
                return (int)(_bitfield2 << 10) >> 29;
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
                return (_bitfield4 << 31) >> 31;
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
                return (_bitfield4 << 30) >> 31;
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

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task BitfieldWithNativeBitfieldAttributeTestImpl()
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
        [NativeBitfield(""o0_b0_24"", offset: 0, length: 24)]
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

        [NativeBitfield(""o4_b0_16"", offset: 0, length: 16)]
        [NativeBitfield(""o4_b16_3"", offset: 16, length: 3)]
        [NativeBitfield(""o4_b19_3"", offset: 19, length: 3)]
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
                return (int)(_bitfield2 << 10) >> 29;
            }

            set
            {
                _bitfield2 = (_bitfield2 & ~(0x7u << 19)) | (uint)((value & 0x7) << 19);
            }
        }

        [NativeBitfield(""o8_b0_1"", offset: 0, length: 1)]
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

        [NativeBitfield(""o12_b0_1"", offset: 0, length: 1)]
        [NativeBitfield(""o12_b1_1"", offset: 1, length: 1)]
        public int _bitfield4;

        [NativeTypeName(""int : 1"")]
        public int o12_b0_1
        {
            get
            {
                return (_bitfield4 << 31) >> 31;
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
                return (_bitfield4 << 30) >> 31;
            }

            set
            {
                _bitfield4 = (_bitfield4 & ~(0x1 << 1)) | ((value & 0x1) << 1);
            }
        }
    }

    public partial struct MyStruct2
    {
        [NativeBitfield(""o0_b0_1"", offset: 0, length: 1)]
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

        [NativeBitfield(""o8_b0_1"", offset: 0, length: 1)]
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
        [NativeBitfield(""o0_b0_1"", offset: 0, length: 1)]
        [NativeBitfield(""o0_b1_1"", offset: 1, length: 1)]
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

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateNativeBitfieldAttribute);
    }

    protected override Task DeclTypeTestImpl()
    {
        var inputContents = @"extern ""C"" void MyFunction();

typedef struct
{
    decltype(&MyFunction) _callback;
} MyStruct;
";

        var expectedOutputContents = @"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName(""decltype(&MyFunction)"")]
        public IntPtr _callback;
    }

    public static partial class Methods
    {
        [DllImport(""ClangSharpPInvokeGenerator"", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction();
    }
}
";
        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ExcludeTestImpl()
    {
        var inputContents = "typedef struct MyStruct MyStruct;";
        var expectedOutputContents = string.Empty;
        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, excludedNames: ExcludeTestExcludedNames);
    }

    protected override Task FixedSizedBufferNonPrimitiveTestImpl(string nativeType, string expectedManagedType)
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
        [NativeTypeName(""MyStruct[3]"")]
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

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FixedSizedBufferNonPrimitiveMultidimensionalTestImpl(string nativeType, string expectedManagedType)
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

        var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
        public {expectedManagedType} value;
    }}

    public partial struct MyOtherStruct
    {{
        [NativeTypeName(""MyStruct[2][1][3][4]"")]
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

            public unsafe ref MyStruct this[int index]
            {{
                get
                {{
                    fixed (MyStruct* pThis = &e0_0_0_0)
                    {{
                        return ref pThis[index];
                    }}
                }}
            }}
        }}
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FixedSizedBufferNonPrimitiveTypedefTestImpl(string nativeType, string expectedManagedType)
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

        var expectedOutputContents = $@"namespace ClangSharp.Test
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

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FixedSizedBufferNonPrimitiveWithNativeTypeNameTestImpl(string nativeType, string expectedManagedType)
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
        [NativeTypeName(""MyStruct[3]"")]
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

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FixedSizedBufferPointerTestImpl(string nativeType, string expectedManagedType)
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

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FixedSizedBufferPrimitiveTestImpl(string nativeType, string expectedManagedType)
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
        [NativeTypeName(""{nativeType}[3]"")]
        public fixed {expectedManagedType} c[3];
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FixedSizedBufferPrimitiveMultidimensionalTestImpl(string nativeType, string expectedManagedType)
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
        [NativeTypeName(""{nativeType}[2][1][3][4]"")]
        public fixed {expectedManagedType} c[2 * 1 * 3 * 4];
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FixedSizedBufferPrimitiveTypedefTestImpl(string nativeType, string expectedManagedType)
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

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task GuidTestImpl()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Non-Windows doesn't support __declspec(uuid(""))
            return Task.CompletedTask;
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

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, excludedNames: GuidTestExcludedNames);
    }

    protected override Task InheritanceTestImpl()
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
        public MyStruct1A Base1;

        public MyStruct1B Base2;

        public int z;

        public int w;
    }
}
";

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task InheritanceWithNativeInheritanceAttributeTestImpl()
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
    [NativeInheritance(""MyStruct1B"")]
    public partial struct MyStruct2
    {
        public MyStruct1A Base1;

        public MyStruct1B Base2;

        public int z;

        public int w;
    }
}
";

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateNativeInheritanceAttribute);
    }

    protected override Task NestedAnonymousTestImpl(string nativeType, string expectedManagedType, int line, int column)
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

        struct
        {{
            {nativeType} value1;

            struct
            {{
                {nativeType} value;
            }};
        }};

        union
        {{
            {nativeType} value2;
        }};

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

        [NativeTypeName(""__AnonymousRecord_ClangUnsavedFile_L{line}_C{column}"")]
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

        public ref {expectedManagedType} value1
        {{
            get
            {{
                fixed (_Anonymous_e__Struct._Anonymous1_e__Struct* pField = &Anonymous.Anonymous1)
                {{
                    return ref pField->value1;
                }}
            }}
        }}

        public ref {expectedManagedType} value
        {{
            get
            {{
                fixed (_Anonymous_e__Struct._Anonymous1_e__Struct._Anonymous_e__Struct* pField = &Anonymous.Anonymous1.Anonymous)
                {{
                    return ref pField->value;
                }}
            }}
        }}

        public ref {expectedManagedType} value2
        {{
            get
            {{
                fixed (_Anonymous_e__Struct._Anonymous2_e__Union* pField = &Anonymous.Anonymous2)
                {{
                    return ref pField->value2;
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

            [NativeTypeName(""__AnonymousRecord_ClangUnsavedFile_L14_C9"")]
            public _w_e__Struct w;

            [NativeTypeName(""__AnonymousRecord_ClangUnsavedFile_L19_C9"")]
            public _Anonymous1_e__Struct Anonymous1;

            [NativeTypeName(""__AnonymousRecord_ClangUnsavedFile_L29_C9"")]
            public _Anonymous2_e__Union Anonymous2;

            public MyUnion u;

            [NativeTypeName(""{nativeType}[4]"")]
            public fixed {expectedManagedType} buffer1[4];

            [NativeTypeName(""MyUnion[4]"")]
            public _buffer2_e__FixedBuffer buffer2;

            public partial struct _w_e__Struct
            {{
                public {expectedManagedType} value;
            }}

            public unsafe partial struct _Anonymous1_e__Struct
            {{
                public {expectedManagedType} value1;

                [NativeTypeName(""__AnonymousRecord_ClangUnsavedFile_L23_C13"")]
                public _Anonymous_e__Struct Anonymous;

                public partial struct _Anonymous_e__Struct
                {{
                    public {expectedManagedType} value;
                }}
            }}

            [StructLayout(LayoutKind.Explicit)]
            public partial struct _Anonymous2_e__Union
            {{
                [FieldOffset(0)]
                public {expectedManagedType} value2;
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

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task NestedAnonymousWithBitfieldTestImpl()
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

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        public int x;

        public int y;

        [NativeTypeName(""__AnonymousRecord_ClangUnsavedFile_L6_C5"")]
        public _Anonymous_e__Struct Anonymous;

        public ref int z
        {
            get
            {
                fixed (_Anonymous_e__Struct* pField = &Anonymous)
                {
                    return ref pField->z;
                }
            }
        }

        public ref int w
        {
            get
            {
                fixed (_Anonymous_e__Struct._Anonymous_e__Struct* pField = &Anonymous.Anonymous)
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

        public unsafe partial struct _Anonymous_e__Struct
        {
            public int z;

            [NativeTypeName(""__AnonymousRecord_ClangUnsavedFile_L10_C9"")]
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
                    get
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

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task NestedTestImpl(string nativeType, string expectedManagedType)
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

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task NestedWithNativeTypeNameTestImpl(string nativeType, string expectedManagedType)
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

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task NewKeywordTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    int Equals;
    int Dispose;
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

        public int Dispose;

        public new int GetHashCode;

        public new int GetType;

        public new int MemberwiseClone;

        public new int ReferenceEquals;

        public new int ToString;
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task NoDefinitionTestImpl()
    {
        var inputContents = "typedef struct MyStruct MyStruct;";

        var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public partial struct MyStruct
    {{
    }}
}}
";
        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task PackTestImpl()
    {
        const string InputContents = @"struct MyStruct1 {
    unsigned Field1;

    void* Field2;

    unsigned Field3;
};

#pragma pack(4)

struct MyStruct2 {
    unsigned Field1;

    void* Field2;

    unsigned Field3;
};
";

        var usingStatement = "using System.Runtime.InteropServices;\n\n";
        var packing = "    [StructLayout(LayoutKind.Sequential, Pack = 4)]\n";

        if (!Environment.Is64BitProcess)
        {
            usingStatement = string.Empty;
            packing = string.Empty;
        }

        var expectedOutputContents = $@"{usingStatement}namespace ClangSharp.Test
{{
    public unsafe partial struct MyStruct1
    {{
        [NativeTypeName(""unsigned int"")]
        public uint Field1;

        public void* Field2;

        [NativeTypeName(""unsigned int"")]
        public uint Field3;
    }}

{packing}    public unsafe partial struct MyStruct2
    {{
        [NativeTypeName(""unsigned int"")]
        public uint Field1;

        public void* Field2;

        [NativeTypeName(""unsigned int"")]
        public uint Field3;
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(InputContents, expectedOutputContents);
    }

    protected override Task PointerToSelfTestImpl()
    {
        var inputContents = @"struct example_s {
   example_s* next;
   void* data;
};";

        var expectedOutputContents = $@"namespace ClangSharp.Test
{{
    public unsafe partial struct example_s
    {{
        public example_s* next;

        public void* data;
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task PointerToSelfViaTypedefTestImpl()
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

        public void* data;
    }}
}}
";

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task RemapTestImpl()
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
        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, remappedNames: remappedNames);
    }

    protected override Task RemapNestedAnonymousTestImpl()
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
    public unsafe partial struct MyStruct
    {
        public double r;

        public double g;

        public double b;

        [NativeTypeName(""__AnonymousRecord_ClangUnsavedFile_L7_C5"")]
        public _Anonymous_e__Struct Anonymous;

        public ref double a
        {
            get
            {
                fixed (_Anonymous_e__Struct* pField = &Anonymous)
                {
                    return ref pField->a;
                }
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
        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, remappedNames: remappedNames);
    }

    protected override Task SkipNonDefinitionTestImpl(string nativeType, string expectedManagedType)
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

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task SkipNonDefinitionPointerTestImpl()
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

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task SkipNonDefinitionWithNativeTypeNameTestImpl(string nativeType, string expectedManagedType)
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

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task TypedefTestImpl(string nativeType, string expectedManagedType)
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

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task UsingDeclarationTestImpl()
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

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task WithAccessSpecifierTestImpl()
    {
        var inputContents = @"struct MyStruct1
{
    int Field1;
    int Field2;
};

struct MyStruct2
{
    int Field1;
    int Field2;
};

struct MyStruct3
{
    int Field1;
    int Field2;
};
";

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    internal partial struct MyStruct1
    {
        private int Field1;

        public int Field2;
    }

    internal partial struct MyStruct2
    {
        private int Field1;

        public int Field2;
    }

    public partial struct MyStruct3
    {
        private int Field1;

        internal int Field2;
    }
}
";

        var withAccessSpecifiers = new Dictionary<string, AccessSpecifier> {
            ["MyStruct1"] = AccessSpecifier.Private,
            ["MyStruct2"] = AccessSpecifier.Internal,
            ["Field1"] = AccessSpecifier.Private,
            ["MyStruct3.Field2"] = AccessSpecifier.Internal,
        };
        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents, withAccessSpecifiers: withAccessSpecifiers);
    }

    protected override Task WithPackingTestImpl()
    {
        const string InputContents = @"struct MyStruct
{
    size_t FixedBuffer[2];
};
";

        const string ExpectedOutputContents = @"using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Sequential, Pack = CustomPackValue)]
    public partial struct MyStruct
    {
        [NativeTypeName(""size_t[2]"")]
        public _FixedBuffer_e__FixedBuffer FixedBuffer;

        public partial struct _FixedBuffer_e__FixedBuffer
        {
            public UIntPtr e0;
            public UIntPtr e1;

            public unsafe ref UIntPtr this[int index]
            {
                get
                {
                    fixed (UIntPtr* pThis = &e0)
                    {
                        return ref pThis[index];
                    }
                }
            }
        }
    }
}
";

        var withPackings = new Dictionary<string, string> {
            ["MyStruct"] = "CustomPackValue"
        };
        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(InputContents, ExpectedOutputContents, withPackings: withPackings);
    }

    protected override Task SourceLocationAttributeTestImpl()
    {
        const string InputContents = @"struct MyStruct
{
    int r;
    int g;
    int b;
};
";

        const string ExpectedOutputContents = @"namespace ClangSharp.Test
{
    [SourceLocation(""ClangUnsavedFile.h"", 1, 8)]
    public partial struct MyStruct
    {
        [SourceLocation(""ClangUnsavedFile.h"", 3, 9)]
        public int r;

        [SourceLocation(""ClangUnsavedFile.h"", 4, 9)]
        public int g;

        [SourceLocation(""ClangUnsavedFile.h"", 5, 9)]
        public int b;
    }
}
";

        return ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(InputContents, ExpectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateSourceLocationAttribute);
    }
}
