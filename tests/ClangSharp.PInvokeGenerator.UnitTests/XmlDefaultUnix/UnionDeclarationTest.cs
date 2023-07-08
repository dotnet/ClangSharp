// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ClangSharp.UnitTests;

public sealed class XmlDefaultUnix_UnionDeclarationTest : UnionDeclarationTest
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

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit"">
      <field name=""r"" access=""public"" offset=""0"">
        <type>{expectedManagedType}</type>
      </field>
      <field name=""g"" access=""public"" offset=""0"">
        <type>{expectedManagedType}</type>
      </field>
      <field name=""b"" access=""public"" offset=""0"">
        <type>{expectedManagedType}</type>
      </field>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
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

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit"">
      <field name=""r"" access=""public"" offset=""0"">
        <type>{expectedManagedType}</type>
      </field>
      <field name=""g"" access=""public"" offset=""0"">
        <type>{expectedManagedType}</type>
      </field>
      <field name=""b"" access=""public"" offset=""0"">
        <type>{expectedManagedType}</type>
      </field>
    </struct>
  </namespace>
</bindings>
";
        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, commandlineArgs: Array.Empty<string>());
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

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit"">
      <field name=""r"" access=""public"" offset=""0"">
        <type native=""{nativeType}"">{expectedManagedType}</type>
      </field>
      <field name=""g"" access=""public"" offset=""0"">
        <type native=""{nativeType}"">{expectedManagedType}</type>
      </field>
      <field name=""b"" access=""public"" offset=""0"">
        <type native=""{nativeType}"">{expectedManagedType}</type>
      </field>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
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

        var expectedPack = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @" pack=""1""" : "";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion1"" access=""public"" layout=""Explicit""{expectedPack}>
      <field name=""_bitfield1"" access=""public"" offset=""0"">
        <type>uint</type>
      </field>
      <field name=""o0_b0_24"" access=""public"">
        <type native=""unsigned int : 24"">uint</type>
        <get>
          <code>return <bitfieldName>_bitfield1</bitfieldName> &amp; 0x<bitwidthHexStringBacking>FFFFFFu</bitwidthHexStringBacking>;</code>
        </get>
        <set>
          <code>
            <bitfieldName>_bitfield1</bitfieldName> = (_bitfield1 &amp; ~0x<bitwidthHexStringBacking>FFFFFFu</bitwidthHexStringBacking>) | (value &amp; 0x<bitwidthHexString>FFFFFFu</bitwidthHexString>);</code>
        </set>
      </field>
      <field name=""_bitfield2"" access=""public"" offset=""0"">
        <type>uint</type>
      </field>
      <field name=""o4_b0_16"" access=""public"">
        <type native=""unsigned int : 16"">uint</type>
        <get>
          <code>return <bitfieldName>_bitfield2</bitfieldName> &amp; 0x<bitwidthHexStringBacking>FFFFu</bitwidthHexStringBacking>;</code>
        </get>
        <set>
          <code>
            <bitfieldName>_bitfield2</bitfieldName> = (_bitfield2 &amp; ~0x<bitwidthHexStringBacking>FFFFu</bitwidthHexStringBacking>) | (value &amp; 0x<bitwidthHexString>FFFFu</bitwidthHexString>);</code>
        </set>
      </field>
      <field name=""o4_b16_3"" access=""public"">
        <type native=""unsigned int : 3"">uint</type>
        <get>
          <code>return (<bitfieldName>_bitfield2</bitfieldName> &gt;&gt; <bitfieldOffset>16</bitfieldOffset>) &amp; 0x<bitwidthHexStringBacking>7u</bitwidthHexStringBacking>;</code>
        </get>
        <set>
          <code>
            <bitfieldName>_bitfield2</bitfieldName> = (_bitfield2 &amp; ~(0x<bitwidthHexStringBacking>7u</bitwidthHexStringBacking> &lt;&lt; <bitfieldOffset>16</bitfieldOffset>)) | ((value &amp; 0x<bitwidthHexString>7u</bitwidthHexString>) &lt;&lt; 16);</code>
        </set>
      </field>
      <field name=""o4_b19_3"" access=""public"">
        <type native=""int : 3"">int</type>
        <get>
          <code>return (<typeName>int</typeName>)(<bitfieldName>_bitfield2</bitfieldName> &lt;&lt; <remainingBitsMinusBitWidth>10</remainingBitsMinusBitWidth>) &gt;&gt; <currentSizeMinusBitWidth>29</currentSizeMinusBitWidth>;</code>
        </get>
        <set>
          <code>
            <bitfieldName>_bitfield2</bitfieldName> = (_bitfield2 &amp; ~(0x<bitwidthHexStringBacking>7u</bitwidthHexStringBacking> &lt;&lt; <bitfieldOffset>19</bitfieldOffset>)) | (uint)((value &amp; 0x<bitwidthHexString>7</bitwidthHexString>) &lt;&lt; 19);</code>
        </set>
      </field>
      <field name=""o8_b0_1"" access=""public"">
        <type native=""unsigned char : 1"">byte</type>
        <get>
          <code>return (<typeName>byte</typeName>)((<bitfieldName>_bitfield2</bitfieldName> &gt;&gt; <bitfieldOffset>22</bitfieldOffset>) &amp; 0x<bitwidthHexStringBacking>1u</bitwidthHexStringBacking>);</code>
        </get>
        <set>
          <code>
            <bitfieldName>_bitfield2</bitfieldName> = (_bitfield2 &amp; ~(0x<bitwidthHexStringBacking>1u</bitwidthHexStringBacking> &lt;&lt; <bitfieldOffset>22</bitfieldOffset>)) | (uint)((value &amp; 0x<bitwidthHexString>1u</bitwidthHexString>) &lt;&lt; 22);</code>
        </set>
      </field>
      <field name=""o12_b0_1"" access=""public"">
        <type native=""int : 1"">int</type>
        <get>
          <code>return (<typeName>int</typeName>)(<bitfieldName>_bitfield2</bitfieldName> &lt;&lt; <remainingBitsMinusBitWidth>8</remainingBitsMinusBitWidth>) &gt;&gt; <currentSizeMinusBitWidth>31</currentSizeMinusBitWidth>;</code>
        </get>
        <set>
          <code>
            <bitfieldName>_bitfield2</bitfieldName> = (_bitfield2 &amp; ~(0x<bitwidthHexStringBacking>1u</bitwidthHexStringBacking> &lt;&lt; <bitfieldOffset>23</bitfieldOffset>)) | (uint)((value &amp; 0x<bitwidthHexString>1</bitwidthHexString>) &lt;&lt; 23);</code>
        </set>
      </field>
      <field name=""o12_b1_1"" access=""public"">
        <type native=""int : 1"">int</type>
        <get>
          <code>return (<typeName>int</typeName>)(<bitfieldName>_bitfield2</bitfieldName> &lt;&lt; <remainingBitsMinusBitWidth>7</remainingBitsMinusBitWidth>) &gt;&gt; <currentSizeMinusBitWidth>31</currentSizeMinusBitWidth>;</code>
        </get>
        <set>
          <code>
            <bitfieldName>_bitfield2</bitfieldName> = (_bitfield2 &amp; ~(0x<bitwidthHexStringBacking>1u</bitwidthHexStringBacking> &lt;&lt; <bitfieldOffset>24</bitfieldOffset>)) | (uint)((value &amp; 0x<bitwidthHexString>1</bitwidthHexString>) &lt;&lt; 24);</code>
        </set>
      </field>
    </struct>
    <struct name=""MyUnion2"" access=""public"" layout=""Explicit"">
      <field name=""_bitfield1"" access=""public"" offset=""0"">
        <type>uint</type>
      </field>
      <field name=""o0_b0_1"" access=""public"">
        <type native=""unsigned int : 1"">uint</type>
        <get>
          <code>return <bitfieldName>_bitfield1</bitfieldName> &amp; 0x<bitwidthHexStringBacking>1u</bitwidthHexStringBacking>;</code>
        </get>
        <set>
          <code>
            <bitfieldName>_bitfield1</bitfieldName> = (_bitfield1 &amp; ~0x<bitwidthHexStringBacking>1u</bitwidthHexStringBacking>) | (value &amp; 0x<bitwidthHexString>1u</bitwidthHexString>);</code>
        </set>
      </field>
      <field name=""x"" access=""public"" offset=""0"">
        <type>int</type>
      </field>
      <field name=""_bitfield2"" access=""public"" offset=""0"">
        <type>uint</type>
      </field>
      <field name=""o8_b0_1"" access=""public"">
        <type native=""unsigned int : 1"">uint</type>
        <get>
          <code>return <bitfieldName>_bitfield2</bitfieldName> &amp; 0x<bitwidthHexStringBacking>1u</bitwidthHexStringBacking>;</code>
        </get>
        <set>
          <code>
            <bitfieldName>_bitfield2</bitfieldName> = (_bitfield2 &amp; ~0x<bitwidthHexStringBacking>1u</bitwidthHexStringBacking>) | (value &amp; 0x<bitwidthHexString>1u</bitwidthHexString>);</code>
        </set>
      </field>
    </struct>
    <struct name=""MyUnion3"" access=""public"" layout=""Explicit""{expectedPack}>
      <field name=""_bitfield"" access=""public"" offset=""0"">
        <type>uint</type>
      </field>
      <field name=""o0_b0_1"" access=""public"">
        <type native=""unsigned int : 1"">uint</type>
        <get>
          <code>return <bitfieldName>_bitfield</bitfieldName> &amp; 0x<bitwidthHexStringBacking>1u</bitwidthHexStringBacking>;</code>
        </get>
        <set>
          <code>
            <bitfieldName>_bitfield</bitfieldName> = (_bitfield &amp; ~0x<bitwidthHexStringBacking>1u</bitwidthHexStringBacking>) | (value &amp; 0x<bitwidthHexString>1u</bitwidthHexString>);</code>
        </set>
      </field>
      <field name=""o0_b1_1"" access=""public"">
        <type native=""unsigned int : 1"">uint</type>
        <get>
          <code>return (<bitfieldName>_bitfield</bitfieldName> &gt;&gt; <bitfieldOffset>1</bitfieldOffset>) &amp; 0x<bitwidthHexStringBacking>1u</bitwidthHexStringBacking>;</code>
        </get>
        <set>
          <code>
            <bitfieldName>_bitfield</bitfieldName> = (_bitfield &amp; ~(0x<bitwidthHexStringBacking>1u</bitwidthHexStringBacking> &lt;&lt; <bitfieldOffset>1</bitfieldOffset>)) | ((value &amp; 0x<bitwidthHexString>1u</bitwidthHexString>) &lt;&lt; 1);</code>
        </set>
      </field>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ExcludeTestImpl()
    {
        var inputContents = "typedef union MyUnion MyUnion;";
        var expectedOutputContents = string.Empty;
        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, excludedNames: ExcludeTestExcludedNames);
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

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit"">
      <field name=""value"" access=""public"" offset=""0"">
        <type>{expectedManagedType}</type>
      </field>
    </struct>
    <struct name=""MyOtherUnion"" access=""public"" layout=""Explicit"">
      <field name=""c"" access=""public"" offset=""0"">
        <type native=""MyUnion[3]"" count=""3"" fixed=""_c_e__FixedBuffer"">MyUnion</type>
      </field>
      <struct name=""_c_e__FixedBuffer"" access=""public"">
        <field name=""e0"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e1"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e2"" access=""public"">
          <type>MyUnion</type>
        </field>
        <indexer access=""public"">
          <type>ref MyUnion</type>
          <param name=""index"">
            <type>int</type>
          </param>
          <get>
            <code>return ref AsSpan()[index];</code>
          </get>
        </indexer>
        <function name=""AsSpan"" access=""public"">
          <type>Span&lt;MyUnion&gt;</type>
          <code>MemoryMarshal.CreateSpan(ref e0, 3);</code>
        </function>
      </struct>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
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

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit"">
      <field name=""value"" access=""public"" offset=""0"">
        <type>{expectedManagedType}</type>
      </field>
    </struct>
    <struct name=""MyOtherUnion"" access=""public"" layout=""Explicit"">
      <field name=""c"" access=""public"" offset=""0"">
        <type native=""MyUnion[2][1][3][4]"" count=""2 * 1 * 3 * 4"" fixed=""_c_e__FixedBuffer"">MyUnion</type>
      </field>
      <struct name=""_c_e__FixedBuffer"" access=""public"">
        <field name=""e0_0_0_0"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e1_0_0_0"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e0_0_1_0"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e1_0_1_0"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e0_0_2_0"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e1_0_2_0"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e0_0_0_1"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e1_0_0_1"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e0_0_1_1"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e1_0_1_1"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e0_0_2_1"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e1_0_2_1"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e0_0_0_2"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e1_0_0_2"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e0_0_1_2"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e1_0_1_2"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e0_0_2_2"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e1_0_2_2"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e0_0_0_3"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e1_0_0_3"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e0_0_1_3"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e1_0_1_3"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e0_0_2_3"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e1_0_2_3"" access=""public"">
          <type>MyUnion</type>
        </field>
        <indexer access=""public"">
          <type>ref MyUnion</type>
          <param name=""index"">
            <type>int</type>
          </param>
          <get>
            <code>return ref AsSpan()[index];</code>
          </get>
        </indexer>
        <function name=""AsSpan"" access=""public"">
          <type>Span&lt;MyUnion&gt;</type>
          <code>MemoryMarshal.CreateSpan(ref e0_0_0_0, 24);</code>
        </function>
      </struct>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
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

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit"">
      <field name=""value"" access=""public"" offset=""0"">
        <type>{expectedManagedType}</type>
      </field>
    </struct>
    <struct name=""MyOtherUnion"" access=""public"" layout=""Explicit"">
      <field name=""c"" access=""public"" offset=""0"">
        <type native=""MyBuffer"" count=""3"" fixed=""_c_e__FixedBuffer"">MyUnion</type>
      </field>
      <struct name=""_c_e__FixedBuffer"" access=""public"">
        <field name=""e0"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e1"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e2"" access=""public"">
          <type>MyUnion</type>
        </field>
        <indexer access=""public"">
          <type>ref MyUnion</type>
          <param name=""index"">
            <type>int</type>
          </param>
          <get>
            <code>return ref AsSpan()[index];</code>
          </get>
        </indexer>
        <function name=""AsSpan"" access=""public"">
          <type>Span&lt;MyUnion&gt;</type>
          <code>MemoryMarshal.CreateSpan(ref e0, 3);</code>
        </function>
      </struct>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
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

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit"">
      <field name=""value"" access=""public"" offset=""0"">
        <type native=""{nativeType}"">{expectedManagedType}</type>
      </field>
    </struct>
    <struct name=""MyOtherUnion"" access=""public"" layout=""Explicit"">
      <field name=""c"" access=""public"" offset=""0"">
        <type native=""MyUnion[3]"" count=""3"" fixed=""_c_e__FixedBuffer"">MyUnion</type>
      </field>
      <struct name=""_c_e__FixedBuffer"" access=""public"">
        <field name=""e0"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e1"" access=""public"">
          <type>MyUnion</type>
        </field>
        <field name=""e2"" access=""public"">
          <type>MyUnion</type>
        </field>
        <indexer access=""public"">
          <type>ref MyUnion</type>
          <param name=""index"">
            <type>int</type>
          </param>
          <get>
            <code>return ref AsSpan()[index];</code>
          </get>
        </indexer>
        <function name=""AsSpan"" access=""public"">
          <type>Span&lt;MyUnion&gt;</type>
          <code>MemoryMarshal.CreateSpan(ref e0, 3);</code>
        </function>
      </struct>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FixedSizedBufferPointerTestImpl(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"union MyUnion
{{
    {nativeType} c[3];
}};
";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit"">
      <field name=""c"" access=""public"" offset=""0"">
        <type native=""{nativeType}[3]"" count=""3"" fixed=""_c_e__FixedBuffer"">{expectedManagedType}</type>
      </field>
      <struct name=""_c_e__FixedBuffer"" access=""public"" unsafe=""true"">
        <field name=""e0"" access=""public"">
          <type>{expectedManagedType}</type>
        </field>
        <field name=""e1"" access=""public"">
          <type>{expectedManagedType}</type>
        </field>
        <field name=""e2"" access=""public"">
          <type>{expectedManagedType}</type>
        </field>
        <indexer access=""public"">
          <type>ref {expectedManagedType}</type>
          <param name=""index"">
            <type>int</type>
          </param>
          <get>
            <code>fixed ({expectedManagedType}* pThis = &amp;e0)
    {{
        return ref pThis[index];
    }}</code>
          </get>
        </indexer>
      </struct>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FixedSizedBufferPrimitiveTestImpl(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"union MyUnion
{{
    {nativeType} c[3];
}};
";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" unsafe=""true"" layout=""Explicit"">
      <field name=""c"" access=""public"" offset=""0"">
        <type native=""{nativeType}[3]"" count=""3"" fixed=""_c_e__FixedBuffer"">{expectedManagedType}</type>
      </field>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FixedSizedBufferPrimitiveMultidimensionalTestImpl(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"union MyUnion
{{
    {nativeType} c[2][1][3][4];
}};
";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" unsafe=""true"" layout=""Explicit"">
      <field name=""c"" access=""public"" offset=""0"">
        <type native=""{nativeType}[2][1][3][4]"" count=""2 * 1 * 3 * 4"" fixed=""_c_e__FixedBuffer"">{expectedManagedType}</type>
      </field>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task FixedSizedBufferPrimitiveTypedefTestImpl(string nativeType, string expectedManagedType)
    {
        var inputContents = $@"typedef {nativeType} MyBuffer[3];

union MyUnion
{{
    MyBuffer c;
}};
";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" unsafe=""true"" layout=""Explicit"">
      <field name=""c"" access=""public"" offset=""0"">
        <type native=""MyBuffer"" count=""3"" fixed=""_c_e__FixedBuffer"">{expectedManagedType}</type>
      </field>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }
    protected override Task GuidTestImpl()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Non-Unix doesn't support __declspec(uuid(""))
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

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion1"" access=""public"" uuid=""00000000-0000-0000-c000-000000000046"" layout=""Explicit"">
      <field name=""x"" access=""public"" offset=""0"">
        <type>int</type>
      </field>
    </struct>
    <struct name=""MyUnion2"" access=""public"" uuid=""00000000-0000-0000-c000-000000000047"" layout=""Explicit"">
      <field name=""x"" access=""public"" offset=""0"">
        <type>int</type>
      </field>
    </struct>
    <class name=""Methods"" access=""public"" static=""true"">
      <iid name=""IID_MyUnion1"" value=""0x00000000, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46"" />
      <iid name=""IID_MyUnion2"" value=""0x00000000, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x47"" />
    </class>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, excludedNames: GuidTestExcludedNames);
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

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"">
      <field name=""value"" access=""public"">
        <type>{expectedManagedType}</type>
      </field>
    </struct>
    <struct name=""MyUnion"" access=""public"" unsafe=""true"" layout=""Explicit"">
      <field name=""r"" access=""public"" offset=""0"">
        <type>{expectedManagedType}</type>
      </field>
      <field name=""g"" access=""public"" offset=""0"">
        <type>{expectedManagedType}</type>
      </field>
      <field name=""b"" access=""public"" offset=""0"">
        <type>{expectedManagedType}</type>
      </field>
      <field name=""Anonymous"" access=""public"" offset=""0"">
        <type native=""__AnonymousRecord_ClangUnsavedFile_L11_C5"">_Anonymous_e__Union</type>
      </field>
      <field name=""a"" access=""public"">
        <type>ref {expectedManagedType}</type>
        <get>
          <code>return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.a, 1));</code>
        </get>
      </field>
      <field name=""s"" access=""public"">
        <type>ref MyStruct</type>
        <get>
          <code>return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.s, 1));</code>
        </get>
      </field>
      <field name=""buffer"" access=""public"">
        <type>Span&lt;{expectedManagedType}&gt;</type>
        <get>
          <code>return MemoryMarshal.CreateSpan(ref Anonymous.buffer[0], 4);</code>
        </get>
      </field>
      <struct name=""_Anonymous_e__Union"" access=""public"" unsafe=""true"" layout=""Explicit"">
        <field name=""a"" access=""public"" offset=""0"">
          <type>{expectedManagedType}</type>
        </field>
        <field name=""s"" access=""public"" offset=""0"">
          <type>MyStruct</type>
        </field>
        <field name=""buffer"" access=""public"" offset=""0"">
          <type native=""{nativeType}[4]"" count=""4"" fixed=""_buffer_e__FixedBuffer"">{expectedManagedType}</type>
        </field>
      </struct>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
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

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit"">
      <field name=""x"" access=""public"" offset=""0"">
        <type>int</type>
      </field>
      <field name=""y"" access=""public"" offset=""0"">
        <type>int</type>
      </field>
      <field name=""Anonymous"" access=""public"" offset=""0"">
        <type native=""__AnonymousRecord_ClangUnsavedFile_L6_C5"">_Anonymous_e__Union</type>
      </field>
      <field name=""z"" access=""public"">
        <type>ref int</type>
        <get>
          <code>return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.z, 1));</code>
        </get>
      </field>
      <field name=""w"" access=""public"">
        <type>ref int</type>
        <get>
          <code>return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.Anonymous.w, 1));</code>
        </get>
      </field>
      <field name=""o0_b0_16"" access=""public"">
        <type>int</type>
        <get>
          <code>return Anonymous.Anonymous.o0_b0_16;</code>
        </get>
        <set>
          <code>Anonymous.Anonymous.o0_b0_16 = value;</code>
        </set>
      </field>
      <field name=""o0_b16_4"" access=""public"">
        <type>int</type>
        <get>
          <code>return Anonymous.Anonymous.o0_b16_4;</code>
        </get>
        <set>
          <code>Anonymous.Anonymous.o0_b16_4 = value;</code>
        </set>
      </field>
      <struct name=""_Anonymous_e__Union"" access=""public"" layout=""Explicit"">
        <field name=""z"" access=""public"" offset=""0"">
          <type>int</type>
        </field>
        <field name=""Anonymous"" access=""public"" offset=""0"">
          <type native=""__AnonymousRecord_ClangUnsavedFile_L10_C9"">_Anonymous_e__Union</type>
        </field>
        <struct name=""_Anonymous_e__Union"" access=""public"" layout=""Explicit"">
          <field name=""w"" access=""public"" offset=""0"">
            <type>int</type>
          </field>
          <field name=""_bitfield"" access=""public"" offset=""0"">
            <type>int</type>
          </field>
          <field name=""o0_b0_16"" access=""public"">
            <type native=""int : 16"">int</type>
            <get>
              <code>return (<bitfieldName>_bitfield</bitfieldName> &lt;&lt; <remainingBitsMinusBitWidth>16</remainingBitsMinusBitWidth>) &gt;&gt; <currentSizeMinusBitWidth>16</currentSizeMinusBitWidth>;</code>
            </get>
            <set>
              <code>
                <bitfieldName>_bitfield</bitfieldName> = (_bitfield &amp; ~0x<bitwidthHexStringBacking>FFFF</bitwidthHexStringBacking>) | (value &amp; 0x<bitwidthHexString>FFFF</bitwidthHexString>);</code>
            </set>
          </field>
          <field name=""o0_b16_4"" access=""public"">
            <type native=""int : 4"">int</type>
            <get>
              <code>return (<bitfieldName>_bitfield</bitfieldName> &lt;&lt; <remainingBitsMinusBitWidth>12</remainingBitsMinusBitWidth>) &gt;&gt; <currentSizeMinusBitWidth>28</currentSizeMinusBitWidth>;</code>
            </get>
            <set>
              <code>
                <bitfieldName>_bitfield</bitfieldName> = (_bitfield &amp; ~(0x<bitwidthHexStringBacking>F</bitwidthHexStringBacking> &lt;&lt; <bitfieldOffset>16</bitfieldOffset>)) | ((value &amp; 0x<bitwidthHexString>F</bitwidthHexString>) &lt;&lt; 16);</code>
            </set>
          </field>
        </struct>
      </struct>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
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

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit"">
      <field name=""r"" access=""public"" offset=""0"">
        <type>{expectedManagedType}</type>
      </field>
      <field name=""g"" access=""public"" offset=""0"">
        <type>{expectedManagedType}</type>
      </field>
      <field name=""b"" access=""public"" offset=""0"">
        <type>{expectedManagedType}</type>
      </field>
      <struct name=""MyNestedUnion"" access=""public"" layout=""Explicit"">
        <field name=""r"" access=""public"" offset=""0"">
          <type>{expectedManagedType}</type>
        </field>
        <field name=""g"" access=""public"" offset=""0"">
          <type>{expectedManagedType}</type>
        </field>
        <field name=""b"" access=""public"" offset=""0"">
          <type>{expectedManagedType}</type>
        </field>
        <field name=""a"" access=""public"" offset=""0"">
          <type>{expectedManagedType}</type>
        </field>
      </struct>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
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

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit"">
      <field name=""r"" access=""public"" offset=""0"">
        <type native=""{nativeType}"">{expectedManagedType}</type>
      </field>
      <field name=""g"" access=""public"" offset=""0"">
        <type native=""{nativeType}"">{expectedManagedType}</type>
      </field>
      <field name=""b"" access=""public"" offset=""0"">
        <type native=""{nativeType}"">{expectedManagedType}</type>
      </field>
      <struct name=""MyNestedUnion"" access=""public"" layout=""Explicit"">
        <field name=""r"" access=""public"" offset=""0"">
          <type native=""{nativeType}"">{expectedManagedType}</type>
        </field>
        <field name=""g"" access=""public"" offset=""0"">
          <type native=""{nativeType}"">{expectedManagedType}</type>
        </field>
        <field name=""b"" access=""public"" offset=""0"">
          <type native=""{nativeType}"">{expectedManagedType}</type>
        </field>
        <field name=""a"" access=""public"" offset=""0"">
          <type native=""{nativeType}"">{expectedManagedType}</type>
        </field>
      </struct>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
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

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit"">
      <field name=""Equals"" access=""public"" offset=""0"">
        <type>int</type>
      </field>
      <field name=""Dispose"" access=""public"" offset=""0"">
        <type>int</type>
      </field>
      <field name=""GetHashCode"" access=""public"" offset=""0"">
        <type>int</type>
      </field>
      <field name=""GetType"" access=""public"" offset=""0"">
        <type>int</type>
      </field>
      <field name=""MemberwiseClone"" access=""public"" offset=""0"">
        <type>int</type>
      </field>
      <field name=""ReferenceEquals"" access=""public"" offset=""0"">
        <type>int</type>
      </field>
      <field name=""ToString"" access=""public"" offset=""0"">
        <type>int</type>
      </field>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task NoDefinitionTestImpl()
    {
        var inputContents = "typedef union MyUnion MyUnion;";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit""></struct>
  </namespace>
</bindings>
";
        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task PointerToSelfTestImpl()
    {
        var inputContents = @"union example_s {
   example_s* next;
   void* data;
};";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""example_s"" access=""public"" unsafe=""true"" layout=""Explicit"">
      <field name=""next"" access=""public"" offset=""0"">
        <type>example_s*</type>
      </field>
      <field name=""data"" access=""public"" offset=""0"">
        <type>void*</type>
      </field>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task PointerToSelfViaTypedefTestImpl()
    {
        var inputContents = @"typedef union example_s example_t;

union example_s {
   example_t* next;
   void* data;
};";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""example_s"" access=""public"" unsafe=""true"" layout=""Explicit"">
      <field name=""next"" access=""public"" offset=""0"">
        <type native=""example_t *"">example_s*</type>
      </field>
      <field name=""data"" access=""public"" offset=""0"">
        <type>void*</type>
      </field>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task RemapTestImpl()
    {
        var inputContents = "typedef union _MyUnion MyUnion;";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit""></struct>
  </namespace>
</bindings>
";

        var remappedNames = new Dictionary<string, string> { ["_MyUnion"] = "MyUnion" };
        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, remappedNames: remappedNames);
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

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit"">
      <field name=""r"" access=""public"" offset=""0"">
        <type>double</type>
      </field>
      <field name=""g"" access=""public"" offset=""0"">
        <type>double</type>
      </field>
      <field name=""b"" access=""public"" offset=""0"">
        <type>double</type>
      </field>
      <field name=""Anonymous"" access=""public"" offset=""0"">
        <type native=""__AnonymousRecord_ClangUnsavedFile_L7_C5"">_Anonymous_e__Union</type>
      </field>
      <field name=""a"" access=""public"">
        <type>ref double</type>
        <get>
          <code>return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.a, 1));</code>
        </get>
      </field>
      <struct name=""_Anonymous_e__Union"" access=""public"" layout=""Explicit"">
        <field name=""a"" access=""public"" offset=""0"">
          <type>double</type>
        </field>
      </struct>
    </struct>
  </namespace>
</bindings>
";

        var remappedNames = new Dictionary<string, string> {
            ["__AnonymousField_ClangUnsavedFile_L7_C5"] = "Anonymous",
            ["__AnonymousRecord_ClangUnsavedFile_L7_C5"] = "_Anonymous_e__Union"
        };
        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, remappedNames: remappedNames);
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

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit"">
      <field name=""r"" access=""public"" offset=""0"">
        <type>{expectedManagedType}</type>
      </field>
      <field name=""g"" access=""public"" offset=""0"">
        <type>{expectedManagedType}</type>
      </field>
      <field name=""b"" access=""public"" offset=""0"">
        <type>{expectedManagedType}</type>
      </field>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task SkipNonDefinitionPointerTestImpl()
    {
        var inputContents = @"typedef union MyUnion* MyUnionPtr;
typedef union MyUnion& MyUnionRef;
";

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit""></struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
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

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit"">
      <field name=""r"" access=""public"" offset=""0"">
        <type native=""{nativeType}"">{expectedManagedType}</type>
      </field>
      <field name=""g"" access=""public"" offset=""0"">
        <type native=""{nativeType}"">{expectedManagedType}</type>
      </field>
      <field name=""b"" access=""public"" offset=""0"">
        <type native=""{nativeType}"">{expectedManagedType}</type>
      </field>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
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

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit"">
      <field name=""r"" access=""public"" offset=""0"">
        <type native=""MyTypedefAlias"">{expectedManagedType}</type>
      </field>
      <field name=""g"" access=""public"" offset=""0"">
        <type native=""MyTypedefAlias"">{expectedManagedType}</type>
      </field>
      <field name=""b"" access=""public"" offset=""0"">
        <type native=""MyTypedefAlias"">{expectedManagedType}</type>
      </field>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }
}
