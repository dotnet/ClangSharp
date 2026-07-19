// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests.Baseline;

[TestFixtureSource(nameof(Variants))]
public sealed class StructDeclarationTest : BaselineTest
{
    private static readonly string[] ExcludeTestExcludedNames = ["MyStruct"];
    private static readonly string[] ExcludeWildcardTestExcludedNames = ["Foo*", "*Legacy", "B?r"];
    private static readonly string[] GuidTestExcludedNames = ["DECLSPEC_UUID"];
    private static readonly string[] GuidConstTestExcludedNames = ["DECLSPEC_UUID", "GUID"];
    private static readonly string[] GuidDefineAliasTestExcludedNames = ["DECLSPEC_UUID", "EXTERN_C"];

    public StructDeclarationTest(BaselineVariant variant) : base(variant)
    {
    }

    protected override string Area => "StructDeclaration";

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task IncompleteArraySizeTest(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} x[];
}};
";

        return ValidateAsync(nameof(IncompleteArraySizeTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task BasicTest(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

        return ValidateAsync(nameof(BasicTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task BasicTestInCMode(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"typedef struct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}}  MyStruct;
";

        return ValidateAsync(nameof(BasicTestInCMode), inputContents, discriminator: $"{nativeType}_{expectedManagedType}", commandLineArgs: []);
    }

    [TestCase("unsigned char", "byte")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    [TestCase("bool", "byte")]
    public Task BasicWithNativeTypeNameTest(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

        return ValidateAsync(nameof(BasicWithNativeTypeNameTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [Test]
    public Task BitfieldTest()
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

        return ValidateAsync(nameof(BitfieldTest), inputContents);
    }

    [Test]
    public Task BitfieldWithNativeBitfieldAttributeTest()
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

        return ValidateAsync(nameof(BitfieldWithNativeBitfieldAttributeTest), inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateNativeBitfieldAttribute);
    }

    [Test]
    public Task ZeroLengthBitfieldTest()
    {
        var inputContents = @"struct MyStruct
{
    unsigned int o0_b0_1 : 1;
    unsigned int : 0;
    unsigned int o4_b0_1 : 1;
};
";

        return ValidateAsync(nameof(ZeroLengthBitfieldTest), inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateNativeBitfieldAttribute);
    }

    [Test]
    public Task DeclTypeTest()
    {
        var inputContents = @"extern ""C"" void MyFunction();

typedef struct
{
    decltype(&MyFunction) _callback;
} MyStruct;
";

        return ValidateAsync(nameof(DeclTypeTest), inputContents);
    }

    [Test]
    public Task DependentTemplateBaseTest()
    {
        // Resolving the dependent base relies on TemplateSpecializationType.TemplateName, which the
        // win-arm64 prebuilt returns invalid; skip until the native lib is rebuilt.
        SkipUntilNativeRebuild();

        // A dependent template-specialization base (`Base<T>` within a class template) previously
        // crashed `GetRecordDecl` with an `InvalidCastException` when both template bindings and
        // empty-record exclusion were enabled, as the base resolves to a `ClassTemplateDecl`
        // rather than a `CXXRecordDecl`. See microsoft/win32metadata#1686.
        var inputContents = @"template <typename T>
struct Base
{
    int value;
};

template <typename T>
struct Derived : Base<T>
{
};
";

        return ValidateAsync(nameof(DependentTemplateBaseTest), inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateTemplateBindings | PInvokeGeneratorConfigurationOptions.ExcludeEmptyRecords);
    }

    [Test]
    public Task ExcludeTest()
    {
        var inputContents = "typedef struct MyStruct MyStruct;";

        return ValidateAsync(nameof(ExcludeTest), inputContents, excludedNames: ExcludeTestExcludedNames);
    }

    [Test]
    public Task ExcludeWildcardTest()
    {
        var inputContents = @"typedef struct Foo1 Foo1;
typedef struct FooBar FooBar;
typedef struct KeepLegacy KeepLegacy;
typedef struct Bar Bar;
typedef struct Keep Keep;";

        // 'Foo*' matches by prefix, '*Legacy' by suffix, and 'B?r' matches a single character, so
        // only 'Keep' survives.
        return ValidateAsync(nameof(ExcludeWildcardTest), inputContents, excludedNames: ExcludeWildcardTestExcludedNames);
    }

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task FixedSizedBufferNonPrimitiveTest(string nativeType, string expectedManagedType)
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

        return ValidateAsync(nameof(FixedSizedBufferNonPrimitiveTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task FixedSizedBufferNonPrimitiveMultidimensionalTest(string nativeType, string expectedManagedType)
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

        return ValidateAsync(nameof(FixedSizedBufferNonPrimitiveMultidimensionalTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task FixedSizedBufferNonPrimitiveTypedefTest(string nativeType, string expectedManagedType)
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

        return ValidateAsync(nameof(FixedSizedBufferNonPrimitiveTypedefTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("unsigned char", "byte")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    [TestCase("bool", "byte")]
    public Task FixedSizedBufferNonPrimitiveWithNativeTypeNameTest(string nativeType, string expectedManagedType)
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

        return ValidateAsync(nameof(FixedSizedBufferNonPrimitiveWithNativeTypeNameTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("double *", "double*")]
    [TestCase("short *", "short*")]
    [TestCase("int *", "int*")]
    [TestCase("float *", "float*")]
    public Task FixedSizedBufferPointerTest(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} c[3];
}};
";

        return ValidateAsync(nameof(FixedSizedBufferPointerTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("unsigned char", "byte")]
    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("float", "float")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    public Task FixedSizedBufferPrimitiveTest(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} c[3];
}};
";

        return ValidateAsync(nameof(FixedSizedBufferPrimitiveTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("unsigned char", "byte")]
    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("float", "float")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    public Task FixedSizedBufferPrimitiveMultidimensionalTest(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} c[2][1][3][4];
}};
";

        return ValidateAsync(nameof(FixedSizedBufferPrimitiveMultidimensionalTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("unsigned char", "byte")]
    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("float", "float")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    public Task FixedSizedBufferPrimitiveTypedefTest(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"typedef {nativeType} MyBuffer[3];

struct MyStruct
{{
    MyBuffer c;
}};
";

        return ValidateAsync(nameof(FixedSizedBufferPrimitiveTypedefTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [Test]
    public Task GuidTest()
    {
        if (Variant.Os != BaselineOs.Windows)
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

        return ValidateAsync(nameof(GuidTest), inputContents, excludedNames: GuidTestExcludedNames);
    }

    [Test]
    public Task GuidCoclassForwardDeclConstTest()
    {
        if (Variant.Os != BaselineOs.Windows)
        {
            // Non-Windows doesn't support __declspec(uuid(""))
            return Task.CompletedTask;
        }

        // Real COM headers (e.g. ShObjIdl_core.h) declare the coclass as a forward declaration and pair
        // it with an `EXTERN_C const CLSID CLSID_Foo;` global. The uuid is recovered into a single
        // `CLSID_FileOpenDialog` constant even though only a forward declaration is present.

        var inputContents = $@"#define DECLSPEC_UUID(x) __declspec(uuid(x))
#define EXTERN_C extern ""C""

struct GUID
{{
    unsigned long  Data1;
    unsigned short Data2;
    unsigned short Data3;
    unsigned char  Data4[8];
}};

typedef GUID CLSID;

EXTERN_C const CLSID CLSID_FileOpenDialog;

class DECLSPEC_UUID(""dc1c5a9c-e88a-4dde-a5a1-60f82a20aef7"") FileOpenDialog;
";

        var remappedNames = new Dictionary<string, string> { ["GUID"] = "Guid" };
        return ValidateAsync(nameof(GuidCoclassForwardDeclConstTest), inputContents, excludedNames: GuidConstTestExcludedNames, remappedNames: remappedNames);
    }

    [Test]
    public Task GuidCoclassConstTest()
    {
        if (Variant.Os != BaselineOs.Windows)
        {
            // Non-Windows doesn't support __declspec(uuid(""))
            return Task.CompletedTask;
        }

        // A coclass with an associated `CLSID_` global recovers its uuid value into a single
        // `CLSID_FileOpenDialog` constant (no `IID_FileOpenDialog`).

        var inputContents = $@"#define DECLSPEC_UUID(x) __declspec(uuid(x))

struct GUID
{{
    unsigned long  Data1;
    unsigned short Data2;
    unsigned short Data3;
    unsigned char  Data4[8];
}};

typedef GUID CLSID;

struct DECLSPEC_UUID(""9f81f860-3900-421a-a231-e7522f9d7f4a"") FileOpenDialog
{{
    int x;
}};

extern ""C"" const CLSID CLSID_FileOpenDialog;
";

        var remappedNames = new Dictionary<string, string> { ["GUID"] = "Guid" };
        return ValidateAsync(nameof(GuidCoclassConstTest), inputContents, excludedNames: GuidConstTestExcludedNames, remappedNames: remappedNames);
    }

    [Test]
    public Task GuidInterfaceConstTest()
    {
        if (Variant.Os != BaselineOs.Windows)
        {
            // Non-Windows doesn't support __declspec(uuid(""))
            return Task.CompletedTask;
        }

        // An interface with an associated `IID_` global emits a single `IID_IFileDialog` constant
        // (the const name matches the default `IID_` scrape name, so it dedups to one).

        var inputContents = $@"#define DECLSPEC_UUID(x) __declspec(uuid(x))

struct GUID
{{
    unsigned long  Data1;
    unsigned short Data2;
    unsigned short Data3;
    unsigned char  Data4[8];
}};

typedef GUID IID;

struct DECLSPEC_UUID(""42f85136-db7e-439c-85f1-e4075d135fc8"") IFileDialog
{{
    int x;
}};

extern ""C"" const IID IID_IFileDialog;
";

        var remappedNames = new Dictionary<string, string> { ["GUID"] = "Guid" };
        return ValidateAsync(nameof(GuidInterfaceConstTest), inputContents, excludedNames: GuidConstTestExcludedNames, remappedNames: remappedNames);
    }

    [Test]
    public Task GuidInterfaceWithGuidConstTest()
    {
        // Some COM interfaces (e.g. ITextHost, win32metadata #1952) ship a `EXTERN_C const IID IID_Foo;`
        // global but no `DECLSPEC_UUID` annotation, so the uuid value is not present in the headers. When the
        // value is supplied out-of-band via `--with-guid`, it composes with the const-GUID global recovery to
        // emit a single `IID_ITextHost` constant and a `[Guid]` attribute on the interface.

        var inputContents = $@"#define EXTERN_C extern ""C""

struct GUID
{{
    unsigned long  Data1;
    unsigned short Data2;
    unsigned short Data3;
    unsigned char  Data4[8];
}};

typedef GUID IID;

struct ITextHost
{{
    int x;
}};

EXTERN_C const IID IID_ITextHost;
";

        var remappedNames = new Dictionary<string, string> { ["GUID"] = "Guid" };
        var withGuids = new Dictionary<string, Guid> { ["ITextHost"] = new Guid("c5bdd8d0-d26e-11ce-a89e-00aa006cadc5") };
        return ValidateAsync(nameof(GuidInterfaceWithGuidConstTest), inputContents, excludedNames: GuidConstTestExcludedNames, remappedNames: remappedNames, withGuids: withGuids);
    }

    [Test]
    public Task GuidInterfaceDuplicateIidTest()
    {
        if (Variant.Os != BaselineOs.Windows)
        {
            // Non-Windows doesn't support __declspec(uuid(""))
            return Task.CompletedTask;
        }

        // A `static const IID&` alias initialized from `__uuidof` names the same `IID_` symbol the interface's uuid
        // already emits. Only one definition may be generated; the alias var must not add a second (CS0102).
        var inputContents = @"#define DECLSPEC_UUID(x) __declspec(uuid(x))

struct _GUID
{
    unsigned long  Data1;
    unsigned short Data2;
    unsigned short Data3;
    unsigned char  Data4[8];
};

typedef struct _GUID GUID;
typedef GUID IID;

struct DECLSPEC_UUID(""e504a81c-6b01-4a88-8e1e-000000000000"") ITransferTarget
{
    int x;
};

static const IID& IID_ITransferTarget = __uuidof(ITransferTarget);
";

        var remappedNames = new Dictionary<string, string> { ["_GUID"] = "Guid", ["GUID"] = "Guid" };
        return ValidateAsync(nameof(GuidInterfaceDuplicateIidTest), inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateUnmanagedConstants, excludedNames: GuidTestExcludedNames, remappedNames: remappedNames);
    }

    [Test]
    public Task GuidDefineAliasRefReadonlyTest()
    {
        if (Variant.Os != BaselineOs.Windows)
        {
            // Non-Windows doesn't support __declspec(uuid(""))
            return Task.CompletedTask;
        }

        // A `#define IID_X IID_Y` alias references another constant's storage, so it stays a `ref readonly Guid`
        // alias. A `#define X (*(const GUID*)(n))` computed-pointer deref (as `MAKEDIPROP` expands to) targets an
        // arbitrary address, so it is exposed as a `Guid*` pointer rather than an invalid managed byref.
        var inputContents = @"#define DECLSPEC_UUID(x) __declspec(uuid(x))
#define EXTERN_C extern ""C""

struct _GUID
{
    unsigned long  Data1;
    unsigned short Data2;
    unsigned short Data3;
    unsigned char  Data4[8];
};

typedef struct _GUID GUID;
typedef GUID IID;

struct DECLSPEC_UUID(""79eac9e0-baf9-11ce-8c82-00aa004ba90b"") IInternet
{
    int x;
};

EXTERN_C const IID IID_IInternet;

#define IID_IOInet IID_IInternet
#define DIPROP_BUFFERSIZE (*(const GUID *)(1))
";

        var remappedNames = new Dictionary<string, string> { ["_GUID"] = "Guid", ["GUID"] = "Guid" };
        return ValidateAsync(nameof(GuidDefineAliasRefReadonlyTest), inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateUnmanagedConstants | PInvokeGeneratorConfigurationOptions.GenerateMacroBindings, excludedNames: GuidDefineAliasTestExcludedNames, remappedNames: remappedNames);
    }

    [Test]
    public Task InheritanceTest()
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

        return ValidateAsync(nameof(InheritanceTest), inputContents);
    }

    [Test]
    public Task InheritanceWithNativeInheritanceAttributeTest()
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

        return ValidateAsync(nameof(InheritanceWithNativeInheritanceAttributeTest), inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateNativeInheritanceAttribute);
    }

    [Test]
    public Task MultiLevelComInheritanceTest()
    {
        // When an intermediate base is reached through a forward `typedef struct X X;` (as MIDL interfaces are
        // declared), clang binds the base to a non-definition redeclaration whose own members would otherwise be
        // dropped. All inherited vtbl slots must still flatten in order (CS0535 when they don't).
        var inputContents = @"struct IUnknown
{
    virtual int QueryInterface() = 0;
    virtual unsigned int AddRef() = 0;
    virtual unsigned int Release() = 0;
};

struct ISequentialStream : public IUnknown
{
    virtual int Read() = 0;
    virtual int Write() = 0;
};

typedef struct IStream IStream;

struct IStream : public ISequentialStream
{
    virtual int Seek() = 0;
    virtual int Clone() = 0;
};

struct IStream;

struct IStreamAsync : public IStream
{
    virtual int ReadAsync() = 0;
};
";

        return ValidateAsync(nameof(MultiLevelComInheritanceTest), inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateVtblIndexAttribute);
    }

    [Test]
    public Task TemplateArgumentNamespaceUsingTest()
    {
        // A namespaced type (System.Numerics.Matrix4x4) used only as a generic-pointer-wrapper template argument
        // must still emit its `using`; dropping it leaves the `Matrix4x4` reference unresolved (CS0246).
        var inputContents = @"namespace ABI { namespace Windows { namespace Foundation {
    template<typename T> struct IReference { T value; };
    namespace Numerics { struct Matrix4x4 { float m[16]; }; }
}}}

struct ISpatial
{
    virtual ABI::Windows::Foundation::IReference<ABI::Windows::Foundation::Numerics::Matrix4x4>* GetTransform() = 0;
};
";

        var remappedNames = new Dictionary<string, string> { ["ABI.Windows.Foundation.Numerics.Matrix4x4"] = "Matrix4x4" };
        var withNamespaces = new Dictionary<string, string> { ["Matrix4x4"] = "System.Numerics" };
        return ValidateAsync(nameof(TemplateArgumentNamespaceUsingTest), inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateGenericPointerWrapper | PInvokeGeneratorConfigurationOptions.GenerateMarkerInterfaces, remappedNames: remappedNames, withNamespaces: withNamespaces);
    }

    [TestCase("double", "double", 10, 5)]
    [TestCase("short", "short", 10, 5)]
    [TestCase("int", "int", 10, 5)]
    [TestCase("float", "float", 10, 5)]
    public Task NestedAnonymousTest(string nativeType, string expectedManagedType, int line, int column)
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

        return ValidateAsync(nameof(NestedAnonymousTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}_{line}_{column}");
    }

    [Test]
    public Task NestedAnonymousWithBitfieldTest()
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

        return ValidateAsync(nameof(NestedAnonymousWithBitfieldTest), inputContents);
    }

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task NestedTest(string nativeType, string expectedManagedType)
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

        return ValidateAsync(nameof(NestedTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("unsigned char", "byte")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    [TestCase("bool", "byte")]
    public Task NestedWithNativeTypeNameTest(string nativeType, string expectedManagedType)
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

        return ValidateAsync(nameof(NestedWithNativeTypeNameTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [Test]
    public Task NewKeywordTest()
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

        return ValidateAsync(nameof(NewKeywordTest), inputContents);
    }

    [Test]
    public Task NoDefinitionTest()
    {
        var inputContents = "typedef struct MyStruct MyStruct;";

        return ValidateAsync(nameof(NoDefinitionTest), inputContents);
    }

    [Test]
    public Task OverAlignmentTest()
    {
        var inputContents = @"struct __attribute__((aligned(16))) MyStruct
{
    int x;
};
";

        return ValidateAsync(nameof(OverAlignmentTest), inputContents, expectedDiagnostics: OverAlignmentExpectedDiagnostics());
    }

    [Test]
    public Task OverAlignmentWithNativeAlignmentAttributeTest()
    {
        var inputContents = @"struct __attribute__((aligned(16))) MyStruct
{
    int x;
};
";

        return ValidateAsync(nameof(OverAlignmentWithNativeAlignmentAttributeTest), inputContents, additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateNativeAlignmentAttribute, expectedDiagnostics: OverAlignmentExpectedDiagnostics());
    }

    private static Diagnostic[] OverAlignmentExpectedDiagnostics()
    {
        return [new Diagnostic(DiagnosticLevel.Warning, "Struct 'MyStruct' requests 16 byte alignment which .NET cannot honor; over-alignment can only lower, never raise, alignment, so the runtime will align it to 4 bytes.", "Line 1, Column 37 in ClangUnsavedFile.h")];
    }

    [Test]
    public Task PackTest()
    {
        return ValidateAsync(nameof(PackTest), @"struct MyStruct1 {
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
");
    }

    [Test]
    public Task PointerToSelfTest()
    {
        var inputContents = @"struct example_s {
   example_s* next;
   void* data;
};";

        return ValidateAsync(nameof(PointerToSelfTest), inputContents);
    }

    [Test]
    public Task PointerToSelfViaTypedefTest()
    {
        var inputContents = @"typedef struct example_s example_t;

struct example_s {
   example_t* next;
   void* data;
};";

        return ValidateAsync(nameof(PointerToSelfViaTypedefTest), inputContents);
    }

    [Test]
    public Task RemapTest()
    {
        var inputContents = "typedef struct _MyStruct MyStruct;";

        var remappedNames = new Dictionary<string, string> { ["_MyStruct"] = "MyStruct" };
        return ValidateAsync(nameof(RemapTest), inputContents, remappedNames: remappedNames);
    }

    [Test]
    public Task RemapNestedAnonymousTest()
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

        var remappedNames = new Dictionary<string, string> {
            ["__AnonymousField_ClangUnsavedFile_L7_C5"] = "Anonymous",
            ["__AnonymousRecord_ClangUnsavedFile_L7_C5"] = "_Anonymous_e__Struct"
        };
        return ValidateAsync(nameof(RemapNestedAnonymousTest), inputContents, remappedNames: remappedNames);
    }

    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("float", "float")]
    public Task SkipNonDefinitionTest(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"typedef struct MyStruct MyStruct;

struct MyStruct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

        return ValidateAsync(nameof(SkipNonDefinitionTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [Test]
    public Task SkipNonDefinitionPointerTest()
    {
        var inputContents = @"typedef struct MyStruct* MyStructPtr;
typedef struct MyStruct& MyStructRef;
";

        return ValidateAsync(nameof(SkipNonDefinitionPointerTest), inputContents);
    }

    [TestCase("unsigned char", "byte")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    [TestCase("bool", "byte")]
    public Task SkipNonDefinitionWithNativeTypeNameTest(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"typedef struct MyStruct MyStruct;

struct MyStruct
{{
    {nativeType} r;
    {nativeType} g;
    {nativeType} b;
}};
";

        return ValidateAsync(nameof(SkipNonDefinitionWithNativeTypeNameTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [TestCase("unsigned char", "byte")]
    [TestCase("double", "double")]
    [TestCase("short", "short")]
    [TestCase("int", "int")]
    [TestCase("long long", "long")]
    [TestCase("signed char", "sbyte")]
    [TestCase("float", "float")]
    [TestCase("unsigned short", "ushort")]
    [TestCase("unsigned int", "uint")]
    [TestCase("unsigned long long", "ulong")]
    [TestCase("bool", "byte")]
    public Task TypedefTest(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"typedef {nativeType} MyTypedefAlias;

struct MyStruct
{{
    MyTypedefAlias r;
    MyTypedefAlias g;
    MyTypedefAlias b;
}};
";

        return ValidateAsync(nameof(TypedefTest), inputContents, discriminator: $"{nativeType}_{expectedManagedType}");
    }

    [Test]
    public Task UsingDeclarationTest()
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

        return ValidateAsync(nameof(UsingDeclarationTest), inputContents);
    }

    [Test]
    public Task WithAccessSpecifierTest()
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

        var withAccessSpecifiers = new Dictionary<string, AccessSpecifier> {
            ["MyStruct1"] = AccessSpecifier.Private,
            ["MyStruct2"] = AccessSpecifier.Internal,
            ["Field1"] = AccessSpecifier.Private,
            ["MyStruct3.Field2"] = AccessSpecifier.Internal,
        };
        return ValidateAsync(nameof(WithAccessSpecifierTest), inputContents, withAccessSpecifiers: withAccessSpecifiers);
    }

    [Test]
    public Task WithAccessSpecifierWildcardTest()
    {
        var inputContents = @"struct MyStruct1
{
    int Field1;
};

struct MyStruct2
{
    int Field1;
};

struct Other
{
    int Field1;
};
";

        // Exercises the precedence rules: the bare '*' catch-all makes everything Internal, the
        // 'MyStruct*' glob narrows those to Private, and the exact 'MyStruct2' key wins over the glob.
        var withAccessSpecifiers = new Dictionary<string, AccessSpecifier> {
            ["*"] = AccessSpecifier.Internal,
            ["MyStruct*"] = AccessSpecifier.Private,
            ["MyStruct2"] = AccessSpecifier.Public,
        };
        return ValidateAsync(nameof(WithAccessSpecifierWildcardTest), inputContents, withAccessSpecifiers: withAccessSpecifiers);
    }

    [Test]
    public Task WithPackingTest()
    {
        var withPackings = new Dictionary<string, string> {
            ["MyStruct"] = "CustomPackValue"
        };

        // Non-Windows targets don't predefine size_t for a header-only parse, so the Unix variants declare it
        // (matching the legacy per-OS input); the resulting int-sized member also exercises the Unix layout.
        var inputContents = Variant.Os == BaselineOs.Unix ? @"typedef int size_t;

struct MyStruct
{
    size_t FixedBuffer[2];
};
" : @"struct MyStruct
{
    size_t FixedBuffer[2];
};
";

        return ValidateAsync(nameof(WithPackingTest), inputContents, withPackings: withPackings);
    }

    [Test]
    public Task SourceLocationAttributeTest()
    {
        return ValidateAsync(nameof(SourceLocationAttributeTest), @"struct MyStruct
{
    int r;
    int g;
    int b;
};
", additionalConfigOptions: PInvokeGeneratorConfigurationOptions.GenerateSourceLocationAttribute);
    }

    [Test]
    public Task AnonStructAndAnonStructArray()
    {
        var inputContents = @"typedef struct _MyStruct
{
    struct { int First; };
    struct { int Second; } MyArray[2];
} MyStruct;
";

        return ValidateAsync(nameof(AnonStructAndAnonStructArray), inputContents);
    }

    [Test]
    public Task DeeplyNestedAnonStructs()
    {
        var inputContents = @"typedef struct _MyStruct
{
    struct { struct {
        struct { int Value1; };
        struct { int Value2; };
    }; };
} MyStruct;";

        return ValidateAsync(nameof(DeeplyNestedAnonStructs), inputContents);
    }
}
