// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class CTest : PInvokeGeneratorTest
{
    [Test]
    public Task BasicTest()
    {
        var inputContents = @"typedef enum MyEnum {
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
} enum_t;

typedef struct MyStruct {
    enum_t _field;
} struct_t;
";
        string expectedOutputContents;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            expectedOutputContents = @"namespace ClangSharp.Test
{
    public enum MyEnum
    {
        MyEnum_Value0,
        MyEnum_Value1,
        MyEnum_Value2,
    }

    public partial struct MyStruct
    {
        [NativeTypeName(""enum_t"")]
        public MyEnum _field;
    }
}
";
        }
        else
        {
            expectedOutputContents = @"namespace ClangSharp.Test
{
    [NativeTypeName(""unsigned int"")]
    public enum MyEnum : uint
    {
        MyEnum_Value0,
        MyEnum_Value1,
        MyEnum_Value2,
    }

    public partial struct MyStruct
    {
        [NativeTypeName(""enum_t"")]
        public MyEnum _field;
    }
}
";
        }

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task EnumTest()
    {
        var inputContents = @"enum {
    VALUE1 = 0,
    VALUE2,
    VALUE3
};

struct MyStruct {
    enum {
        VALUEA = 0,
        VALUEB,
        VALUEC
    } field;
};
";
        string expectedOutputContents;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName(""__AnonymousEnum_ClangUnsavedFile_L8_C5"")]
        public int field;

        public const int VALUEA = 0;
        public const int VALUEB = 1;
        public const int VALUEC = 2;
    }

    public static partial class Methods
    {
        public const int VALUE1 = 0;
        public const int VALUE2 = 1;
        public const int VALUE3 = 2;
    }
}
";
        }
        else
        {
            expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName(""__AnonymousEnum_ClangUnsavedFile_L8_C5"")]
        public uint field;

        public const uint VALUEA = 0;
        public const uint VALUEB = 1;
        public const uint VALUEC = 2;
    }

    public static partial class Methods
    {
        public const uint VALUE1 = 0;
        public const uint VALUE2 = 1;
        public const uint VALUE3 = 2;
    }
}
";
        }

        var diagnostics = new[] {
            new Diagnostic(DiagnosticLevel.Info, "Found anonymous enum: __AnonymousEnum_ClangUnsavedFile_L1_C1. Mapping values as constants in: Methods", "Line 1, Column 1 in ClangUnsavedFile.h"),
            new Diagnostic(DiagnosticLevel.Info, "Found anonymous enum: __AnonymousEnum_ClangUnsavedFile_L8_C5. Mapping values as constants in: Methods", "Line 8, Column 5 in ClangUnsavedFile.h")
        };
        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, commandLineArgs: DefaultCClangCommandLineArgs, expectedDiagnostics: diagnostics, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task StructTest()
    {
        var inputContents = @"typedef struct _MyStruct
{
    int _field;
} MyStruct;

typedef struct _MyOtherStruct
{
    MyStruct _field1;
    MyStruct* _field2;
} MyOtherStruct;

typedef struct _MyStructWithAnonymousStruct
{
    struct {
        int _field;
    } _anonymousStructField1;
} MyStructWithAnonymousStruct;

typedef struct _MyStructWithAnonymousUnion
{
    union {
        int _field1;
        int* _field2;
    } union1;
} MyStructWithAnonymousUnion;
";
        var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct _MyStruct
    {
        public int _field;
    }

    public unsafe partial struct _MyOtherStruct
    {
        [NativeTypeName(""MyStruct"")]
        public _MyStruct _field1;

        [NativeTypeName(""MyStruct *"")]
        public _MyStruct* _field2;
    }

    public partial struct _MyStructWithAnonymousStruct
    {
        [NativeTypeName(""__AnonymousRecord_ClangUnsavedFile_L14_C5"")]
        public __anonymousStructField1_e__Struct _anonymousStructField1;

        public partial struct __anonymousStructField1_e__Struct
        {
            public int _field;
        }
    }

    public unsafe partial struct _MyStructWithAnonymousUnion
    {
        [NativeTypeName(""__AnonymousRecord_ClangUnsavedFile_L21_C5"")]
        public _union1_e__Union union1;

        [StructLayout(LayoutKind.Explicit)]
        public unsafe partial struct _union1_e__Union
        {
            [FieldOffset(0)]
            public int _field1;

            [FieldOffset(0)]
            public int* _field2;
        }
    }
}
";
        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task UnionTest()
    {
        var inputContents = @"typedef union _MyUnion
{
    int _field;
} MyUnion;

typedef union _MyOtherUnion
{
    MyUnion _field1;
    MyUnion* _field2;
} MyOtherUnion;
";
        var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct _MyUnion
    {
        [FieldOffset(0)]
        public int _field;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct _MyOtherUnion
    {
        [FieldOffset(0)]
        [NativeTypeName(""MyUnion"")]
        public _MyUnion _field1;

        [FieldOffset(0)]
        [NativeTypeName(""MyUnion *"")]
        public _MyUnion* _field2;
    }
}
";
        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task MacroTest()
    {
        var inputContents = @"#define MyMacro1 5
#define MyMacro2 MyMacro1";

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName(""#define MyMacro1 5"")]
        public const int MyMacro1 = 5;

        [NativeTypeName(""#define MyMacro2 MyMacro1"")]
        public const int MyMacro2 = 5;
    }
}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task UnsignedIntBitshiftTest()
    {
        var inputContents = @"
const int Signed = 1;
const long SignedLong = 1;
const unsigned int Unsigned = 1;

const int ShiftSigned = 1 << Signed;
const int ShiftSignedLong = 1 << SignedLong;
const int ShiftUnsigned = 1 << Unsigned;

const int Char = 1 << 'a';

const int Byte = 1 << (signed char)1;
const int UByte = 1 << (unsigned char)1;

const int CInt = 1 << 1;
const int CUint = 1 << 1U;

const int Negative = 1 << -1;

const int OutOfRangePos = 1 << 10000000000;
const int OutOfRangeNeg = 1 << -10000000000;

const int IntMax = 1 << 2147483647;
const int IntMin = 1 << -2147483648;

const int LongMax = 1 << 9223372036854775807;
const int LongMin = 1 << -9223372036854775808;

const int ULongMax = 1 << 18446744073709551615;

const int Hexadecimal = 1 << 0x01;

#define Left 1 << 1U
#define Right 1 >> 1U

#define Int 1 << 1
#define Long 1 << 1L
#define LongLong 1 << 1LL
#define ULong 1 << 1UL
#define ULongLong 1 << 1ULL

#define Complex ((((unsigned int)(0)) << 29U) | (((unsigned int)(1)) << 22U) | (((unsigned int)(0)) << 12U) | ((unsigned int)(0)))
";

        // Non-ideal cases:
        // UChar
        // IntMin
        // ULongMax
        // Hexadecimal
        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName(""const int"")]
        public const int Signed = 1;

        [NativeTypeName(""const long"")]
        public const int SignedLong = 1;

        [NativeTypeName(""const unsigned int"")]
        public const uint Unsigned = 1;

        [NativeTypeName(""const int"")]
        public const int ShiftSigned = 1 << Signed;

        [NativeTypeName(""const int"")]
        public const int ShiftSignedLong = 1 << SignedLong;

        [NativeTypeName(""const int"")]
        public const int ShiftUnsigned = 1 << (int)(Unsigned);

        [NativeTypeName(""const int"")]
        public const int Char = 1 << (sbyte)('a');

        [NativeTypeName(""const int"")]
        public const int Byte = 1 << (sbyte)(1);

        [NativeTypeName(""const int"")]
        public const int UByte = unchecked(1 << (byte)(1));

        [NativeTypeName(""const int"")]
        public const int CInt = 1 << 1;

        [NativeTypeName(""const int"")]
        public const int CUint = 1 << 1;

        [NativeTypeName(""const int"")]
        public const int Negative = 1 << -1;

        [NativeTypeName(""const int"")]
        public const int OutOfRangePos = unchecked(1 << (int)(10000000000));

        [NativeTypeName(""const int"")]
        public const int OutOfRangeNeg = unchecked(1 << (int)(-10000000000));

        [NativeTypeName(""const int"")]
        public const int IntMax = 1 << 2147483647;

        [NativeTypeName(""const int"")]
        public const int IntMin = unchecked(1 << -2147483648);

        [NativeTypeName(""const int"")]
        public const int LongMax = unchecked(1 << (int)(9223372036854775807));

        [NativeTypeName(""const int"")]
        public const int LongMin = unchecked(1 << (int)(-9223372036854775808));

        [NativeTypeName(""const int"")]
        public const int ULongMax = 1 << -1;

        [NativeTypeName(""const int"")]
        public const int Hexadecimal = 1 << 1;

        [NativeTypeName(""#define Left 1 << 1U"")]
        public const int Left = 1 << 1;

        [NativeTypeName(""#define Right 1 >> 1U"")]
        public const int Right = 1 >> 1;

        [NativeTypeName(""#define Int 1 << 1"")]
        public const int Int = 1 << 1;

        [NativeTypeName(""#define Long 1 << 1L"")]
        public const int Long = 1 << 1;

        [NativeTypeName(""#define LongLong 1 << 1LL"")]
        public const int LongLong = 1 << 1;

        [NativeTypeName(""#define ULong 1 << 1UL"")]
        public const int ULong = 1 << 1;

        [NativeTypeName(""#define ULongLong 1 << 1ULL"")]
        public const int ULongLong = 1 << 1;

        [NativeTypeName(""#define Complex ((((unsigned int)(0)) << 29U) | (((unsigned int)(1)) << 22U) | (((unsigned int)(0)) << 12U) | ((unsigned int)(0)))"")]
        public const uint Complex = ((((uint)(0)) << 29) | (((uint)(1)) << 22) | (((uint)(0)) << 12) | ((uint)(0)));
    }
}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }
}