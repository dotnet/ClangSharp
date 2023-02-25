// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;

namespace ClangSharp.UnitTests;

public sealed class XmlCompatibleUnix_DeprecatedToObsoleteTest : DeprecatedToObsoleteTest
{
    protected override Task SimpleStructMembersImpl(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"struct MyStruct
{{
    {nativeType} r;
    
    [[deprecated]]
    {nativeType} g;

    [[deprecated(""This is obsolete."")]]
    {nativeType} b;

    {nativeType} a;
}};
";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"">
      <field name=""r"" access=""public"">
        <type>int</type>
      </field>
      <field name=""g"" access=""public"">
        <attribute>Obsolete</attribute>
        <type>int</type>
      </field>
      <field name=""b"" access=""public"">
        <attribute>Obsolete(""This is obsolete."")</attribute>
        <type>int</type>
      </field>
      <field name=""a"" access=""public"">
        <type>int</type>
      </field>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task StructDeclImpl()
    {
        var inputContents = $@"struct MyStruct0
{{
    int r;
}};

struct [[deprecated]] MyStruct1
{{
    int r;
}};

struct [[deprecated(""This is obsolete."")]] MyStruct2
{{
    int r;
}};

struct MyStruct3
{{
    int r;
}};
";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct0"" access=""public"">
      <field name=""r"" access=""public"">
        <type>int</type>
      </field>
    </struct>
    <struct name=""MyStruct1"" access=""public"">
      <attribute>Obsolete</attribute>
      <field name=""r"" access=""public"">
        <type>int</type>
      </field>
    </struct>
    <struct name=""MyStruct2"" access=""public"">
      <attribute>Obsolete(""This is obsolete."")</attribute>
      <field name=""r"" access=""public"">
        <type>int</type>
      </field>
    </struct>
    <struct name=""MyStruct3"" access=""public"">
      <field name=""r"" access=""public"">
        <type>int</type>
      </field>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task SimpleTypedefStructMembersImpl(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"typedef struct
{{
    {nativeType} r;

    [[deprecated]]
    {nativeType} g;

    [[deprecated(""This is obsolete."")]]
    {nativeType} b;

    {nativeType} a;
}} MyStruct;
";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"">
      <field name=""r"" access=""public"">
        <type>int</type>
      </field>
      <field name=""g"" access=""public"">
        <attribute>Obsolete</attribute>
        <type>int</type>
      </field>
      <field name=""b"" access=""public"">
        <attribute>Obsolete(""This is obsolete."")</attribute>
        <type>int</type>
      </field>
      <field name=""a"" access=""public"">
        <type>int</type>
      </field>
    </struct>
  </namespace>
</bindings>
";
        return ValidateGeneratedXmlCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task TypedefStructDeclImpl()
    {
        var inputContents = $@"typedef struct
{{
    int r;
}} MyStruct0;

[[deprecated]] typedef struct
{{
    int r;
}} MyStruct1;

[[deprecated(""This is obsolete."")]] typedef struct
{{
    int r;
}} MyStruct2;

typedef struct
{{
    int r;
}} MyStruct3;
";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct0"" access=""public"">
      <field name=""r"" access=""public"">
        <type>int</type>
      </field>
    </struct>
    <struct name=""MyStruct1"" access=""public"">
      <attribute>Obsolete</attribute>
      <field name=""r"" access=""public"">
        <type>int</type>
      </field>
    </struct>
    <struct name=""MyStruct2"" access=""public"">
      <attribute>Obsolete(""This is obsolete."")</attribute>
      <field name=""r"" access=""public"">
        <type>int</type>
      </field>
    </struct>
    <struct name=""MyStruct3"" access=""public"">
      <field name=""r"" access=""public"">
        <type>int</type>
      </field>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task SimpleEnumMembersImpl()
    {
        var inputContents = $@"enum MyEnum : int
{{
    MyEnum_Value0,
    MyEnum_Value1 [[deprecated]],
    MyEnum_Value2 [[deprecated(""This is obsolete."")]],
    MyEnum_Value3,
}};
";

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <enumeration name=""MyEnum"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum_Value0"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
      <enumerator name=""MyEnum_Value1"" access=""public"">
        <attribute>Obsolete</attribute>
        <type primitive=""False"">int</type>
      </enumerator>
      <enumerator name=""MyEnum_Value2"" access=""public"">
        <attribute>Obsolete(""This is obsolete."")</attribute>
        <type primitive=""False"">int</type>
      </enumerator>
      <enumerator name=""MyEnum_Value3"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task EnumDeclImpl()
    {
        var inputContents = $@"enum MyEnum0 : int
{{
    MyEnum_Value0,
}};

enum [[deprecated]] MyEnum1 : int
{{
    MyEnum_Value1,
}};

enum [[deprecated(""This is obsolete."")]] MyEnum2 : int
{{
    MyEnum_Value2,
}};


enum MyEnum3 : int
{{
    MyEnum_Value3,
}};
";

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <enumeration name=""MyEnum0"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum_Value0"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
    </enumeration>
    <enumeration name=""MyEnum1"" access=""public"">
      <attribute>Obsolete</attribute>
      <type>int</type>
      <enumerator name=""MyEnum_Value1"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
    </enumeration>
    <enumeration name=""MyEnum2"" access=""public"">
      <attribute>Obsolete(""This is obsolete."")</attribute>
      <type>int</type>
      <enumerator name=""MyEnum_Value2"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
    </enumeration>
    <enumeration name=""MyEnum3"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum_Value3"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
    </enumeration>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task SimpleVarDeclImpl(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"
{nativeType} MyVariable0 = 0;

[[deprecated]]
{nativeType} MyVariable1 = 0;

[[deprecated(""This is obsolete."")]]
{nativeType} MyVariable2 = 0;

{nativeType} MyVariable3 = 0;";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <field name=""MyVariable0"" access=""public"">
        <type primitive=""True"">int</type>
        <value>
          <code>0</code>
        </value>
      </field>
      <field name=""MyVariable1"" access=""public"">
        <attribute>Obsolete</attribute>
        <type primitive=""True"">int</type>
        <value>
          <code>0</code>
        </value>
      </field>
      <field name=""MyVariable2"" access=""public"">
        <attribute>Obsolete(""This is obsolete."")</attribute>
        <type primitive=""True"">int</type>
        <value>
          <code>0</code>
        </value>
      </field>
      <field name=""MyVariable3"" access=""public"">
        <type primitive=""True"">int</type>
        <value>
          <code>0</code>
        </value>
      </field>
    </class>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FuncDeclImpl()
    {
        var inputContents = @"
void MyFunction0()
{
}

[[deprecated]]
void MyFunction1()
{
}

[[deprecated(""This is obsolete."")]]
void MyFunction2()
{
}

void MyFunction3()
{
}
";

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction0"" access=""public"" static=""true"">
        <type>void</type>
        <code></code>
      </function>
      <function name=""MyFunction1"" access=""public"" static=""true"">
        <attribute>Obsolete</attribute>
        <type>void</type>
        <code></code>
      </function>
      <function name=""MyFunction2"" access=""public"" static=""true"">
        <attribute>Obsolete(""This is obsolete."")</attribute>
        <type>void</type>
        <code></code>
      </function>
      <function name=""MyFunction3"" access=""public"" static=""true"">
        <type>void</type>
        <code></code>
      </function>
    </class>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task InstanceFuncImpl()
    {
        var inputContents = @"struct MyStruct
{
    int MyFunction0() { return 0; }

    [[deprecated]]
    int MyFunction1() { return 0; }

    [[deprecated(""This is obsolete."")]]
    int MyFunction2() { return 0; }

    int MyFunction3() { return 0; }
};";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"">
      <function name=""MyFunction0"" access=""public"">
        <type>int</type>
        <code>return 0;</code>
      </function>
      <function name=""MyFunction1"" access=""public"">
        <attribute>Obsolete</attribute>
        <type>int</type>
        <code>return 0;</code>
      </function>
      <function name=""MyFunction2"" access=""public"">
        <attribute>Obsolete(""This is obsolete."")</attribute>
        <type>int</type>
        <code>return 0;</code>
      </function>
      <function name=""MyFunction3"" access=""public"">
        <type>int</type>
        <code>return 0;</code>
      </function>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FuncPtrDeclImpl()
    {
        var inputContents = @"
typedef void (*Callback0)();
[[deprecated]] typedef void (*Callback1)();
[[deprecated(""This is obsolete."")]] typedef void (*Callback2)();
typedef void (*Callback3)();

struct MyStruct0 {
    Callback0 _callback;
};
struct MyStruct1 {
    Callback1 _callback;
};
struct MyStruct2 {
    Callback2 _callback;
};
struct MyStruct3 {
    Callback3 _callback;
};
";

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <delegate name=""Callback0"" access=""public"" convention=""Cdecl"" static=""true"">
      <type>void</type>
    </delegate>
    <delegate name=""Callback1"" access=""public"" convention=""Cdecl"" static=""true"">
      <attribute>Obsolete</attribute>
      <type>void</type>
    </delegate>
    <delegate name=""Callback2"" access=""public"" convention=""Cdecl"" static=""true"">
      <attribute>Obsolete(""This is obsolete."")</attribute>
      <type>void</type>
    </delegate>
    <delegate name=""Callback3"" access=""public"" convention=""Cdecl"" static=""true"">
      <type>void</type>
    </delegate>
    <struct name=""MyStruct0"" access=""public"">
      <field name=""_callback"" access=""public"">
        <type native=""Callback0"">IntPtr</type>
      </field>
    </struct>
    <struct name=""MyStruct1"" access=""public"">
      <field name=""_callback"" access=""public"">
        <attribute>Obsolete</attribute>
        <type native=""Callback1"">IntPtr</type>
      </field>
    </struct>
    <struct name=""MyStruct2"" access=""public"">
      <field name=""_callback"" access=""public"">
        <attribute>Obsolete(""This is obsolete."")</attribute>
        <type native=""Callback2"">IntPtr</type>
      </field>
    </struct>
    <struct name=""MyStruct3"" access=""public"">
      <field name=""_callback"" access=""public"">
        <type native=""Callback3"">IntPtr</type>
      </field>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FuncDllImportImpl()
    {
        var inputContents = @"
extern ""C"" void MyFunction0();
extern ""C"" [[deprecated]] void MyFunction1();
extern ""C"" [[deprecated(""This is obsolete."")]] void MyFunction2();
extern ""C"" void MyFunction3();";

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction0"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" static=""true"">
        <type>void</type>
      </function>
      <function name=""MyFunction1"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" static=""true"">
        <attribute>Obsolete</attribute>
        <type>void</type>
      </function>
      <function name=""MyFunction2"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" static=""true"">
        <attribute>Obsolete(""This is obsolete."")</attribute>
        <type>void</type>
      </function>
      <function name=""MyFunction3"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" static=""true"">
        <type>void</type>
      </function>
    </class>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlCompatibleUnixBindingsAsync(inputContents, expectedOutputContents);
    }
}
