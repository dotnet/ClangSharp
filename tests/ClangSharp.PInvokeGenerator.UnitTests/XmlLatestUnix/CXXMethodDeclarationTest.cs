// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[Platform("unix")]
public sealed class XmlLatestUnix_CXXMethodDeclarationTest : CXXMethodDeclarationXmlTest
{
    protected override Task DefaultParameterInheritedFromTemplateTestImpl()
    {
        // NOTE: This is a regression test where a struct inherits a function from a template with a default parameter.
        const string InputContents = @"template <typename T>
struct MyTemplate
{
    int* DoWork(int* value = nullptr)
    {
        return value;
    }
};

struct MyStruct : public MyTemplate<int>
{};
";

        var entryPoint = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "__ZN8$MyTemplateDoWorkEv" : "_ZN10MyTemplateIiE6DoWorkEPi";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"" native=""struct MyStruct : MyTemplate&lt;int&gt;"" unsafe=""true"">
      <function name=""DoWork"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""ThisCall"" entrypoint=""{entryPoint}"" static=""true"" unsafe=""true"">
        <type>int*</type>
        <param name=""pThis"">
          <type>MyStruct*</type>
        </param>
        <param name=""value"">
          <type>int*</type>
          <init>
            <code>null</code>
          </init>
        </param>
      </function>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlLatestUnixBindingsAsync(InputContents, expectedOutputContents);
    }

    protected override Task InstanceTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    void MyVoidMethod();

    int MyInt32Method()
    {
        return 0;
    }

    void* MyVoidStarMethod()
    {
        return nullptr;
    }
};
";
        var entryPoint = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "__ZN8MyStruct12MyVoidMethodEv" : "_ZN8MyStruct12MyVoidMethodEv";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            entryPoint = Environment.Is64BitProcess
                       ? "?MyVoidMethod@MyStruct@@QEAAXXZ"
                       : "?MyVoidMethod@MyStruct@@QAEXXZ";
        }

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"" unsafe=""true"">
      <function name=""MyVoidMethod"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""ThisCall"" entrypoint=""{entryPoint}"" static=""true"">
        <type>void</type>
        <param name=""pThis"">
          <type>MyStruct*</type>
        </param>
      </function>
      <function name=""MyInt32Method"" access=""public"">
        <type>int</type>
        <code>return 0;</code>
      </function>
      <function name=""MyVoidStarMethod"" access=""public"" unsafe=""true"">
        <type>void*</type>
        <code>return null;</code>
      </function>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task NewKeywordVirtualTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    virtual int GetType(int obj) = 0;
    virtual int GetType() = 0;
    virtual int GetType(int objA, int objB) = 0;
};";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"" vtbl=""true"" unsafe=""true"">
      <field name=""lpVtbl"" access=""public"">
        <type>void**</type>
      </field>
      <function name=""GetType"" access=""public"" unsafe=""true"">
        <type>int</type>
        <param name=""obj"">
          <type>int</type>
        </param>
        <body>
          <code>return ((delegate* unmanaged[Thiscall]&lt;MyStruct*, int, int&gt;)(lpVtbl[<vtbl explicit=""False"">0</vtbl>]))(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>, <param name=""obj"">obj</param>);</code>
        </body>
      </function>
      <function name=""GetType"" access=""public"" unsafe=""true"">
        <type>int</type>
        <body>
          <code>return ((delegate* unmanaged[Thiscall]&lt;MyStruct*, int&gt;)(lpVtbl[<vtbl explicit=""False"">1</vtbl>]))(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>);</code>
        </body>
      </function>
      <function name=""GetType"" access=""public"" unsafe=""true"">
        <type>int</type>
        <param name=""objA"">
          <type>int</type>
        </param>
        <param name=""objB"">
          <type>int</type>
        </param>
        <body>
          <code>return ((delegate* unmanaged[Thiscall]&lt;MyStruct*, int, int, int&gt;)(lpVtbl[<vtbl explicit=""False"">2</vtbl>]))(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>, <param name=""objA"">objA</param>, <param name=""objB"">objB</param>);</code>
        </body>
      </function>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task NewKeywordVirtualWithExplicitVtblTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    virtual int GetType(int obj) = 0;
    virtual int GetType() = 0;
    virtual int GetType(int objA, int objB) = 0;
};";

        var nativeCallConv = "";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !Environment.Is64BitProcess)
        {
            nativeCallConv = " __attribute__((thiscall))";
        }

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"" vtbl=""true"" unsafe=""true"">
      <field name=""lpVtbl"" access=""public"">
        <type>Vtbl*</type>
      </field>
      <function name=""GetType"" access=""public"" unsafe=""true"">
        <type>int</type>
        <param name=""obj"">
          <type>int</type>
        </param>
        <body>
          <code>return lpVtbl-&gt;<vtbl explicit=""True"">GetType</vtbl>(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>, <param name=""obj"">obj</param>);</code>
        </body>
      </function>
      <function name=""GetType"" access=""public"" unsafe=""true"">
        <type>int</type>
        <body>
          <code>return lpVtbl-&gt;<vtbl explicit=""True"">GetType1</vtbl>(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>);</code>
        </body>
      </function>
      <function name=""GetType"" access=""public"" unsafe=""true"">
        <type>int</type>
        <param name=""objA"">
          <type>int</type>
        </param>
        <param name=""objB"">
          <type>int</type>
        </param>
        <body>
          <code>return lpVtbl-&gt;<vtbl explicit=""True"">GetType2</vtbl>(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>, <param name=""objA"">objA</param>, <param name=""objB"">objB</param>);</code>
        </body>
      </function>
      <vtbl>
        <field name=""GetType"" access=""public"">
          <type native=""int (int){nativeCallConv}"">delegate* unmanaged[Thiscall]&lt;MyStruct*, int, int&gt;</type>
        </field>
        <field name=""GetType1"" access=""public"">
          <type native=""int (){nativeCallConv}"">delegate* unmanaged[Thiscall]&lt;MyStruct*, int&gt;</type>
        </field>
        <field name=""GetType2"" access=""public"">
          <type native=""int (int, int){nativeCallConv}"">delegate* unmanaged[Thiscall]&lt;MyStruct*, int, int, int&gt;</type>
        </field>
      </vtbl>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls);
    }

    protected override Task NewKeywordVirtualWithExplicitVtblAndMarkerInterfaceTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    virtual int GetType(int obj) = 0;
    virtual int GetType() = 0;
    virtual int GetType(int objA, int objB) = 0;
};";

        var nativeCallConv = "";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !Environment.Is64BitProcess)
        {
            nativeCallConv = " __attribute__((thiscall))";
        }

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"" vtbl=""true"" unsafe=""true"">
      <field name=""lpVtbl"" access=""public"">
        <type>Vtbl&lt;MyStruct&gt;*</type>
      </field>
      <function name=""GetType"" access=""public"" unsafe=""true"">
        <type>int</type>
        <param name=""obj"">
          <type>int</type>
        </param>
        <body>
          <code>return lpVtbl-&gt;<vtbl explicit=""True"">GetType</vtbl>(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>, <param name=""obj"">obj</param>);</code>
        </body>
      </function>
      <function name=""GetType"" access=""public"" unsafe=""true"">
        <type>int</type>
        <body>
          <code>return lpVtbl-&gt;<vtbl explicit=""True"">GetType1</vtbl>(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>);</code>
        </body>
      </function>
      <function name=""GetType"" access=""public"" unsafe=""true"">
        <type>int</type>
        <param name=""objA"">
          <type>int</type>
        </param>
        <param name=""objB"">
          <type>int</type>
        </param>
        <body>
          <code>return lpVtbl-&gt;<vtbl explicit=""True"">GetType2</vtbl>(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>, <param name=""objA"">objA</param>, <param name=""objB"">objB</param>);</code>
        </body>
      </function>
      <interface>
        <function name=""GetType"" access=""public"" unsafe=""true"">
          <type>int</type>
          <param name=""obj"">
            <type>int</type>
          </param>
        </function>
        <function name=""GetType"" access=""public"" unsafe=""true"">
          <type>int</type>
        </function>
        <function name=""GetType"" access=""public"" unsafe=""true"">
          <type>int</type>
          <param name=""objA"">
            <type>int</type>
          </param>
          <param name=""objB"">
            <type>int</type>
          </param>
        </function>
      </interface>
      <vtbl>
        <field name=""GetType"" access=""public"">
          <type native=""int (int){nativeCallConv}"">delegate* unmanaged[Thiscall]&lt;TSelf*, int, int&gt;</type>
        </field>
        <field name=""GetType1"" access=""public"">
          <type native=""int (){nativeCallConv}"">delegate* unmanaged[Thiscall]&lt;TSelf*, int&gt;</type>
        </field>
        <field name=""GetType2"" access=""public"">
          <type native=""int (int, int){nativeCallConv}"">delegate* unmanaged[Thiscall]&lt;TSelf*, int, int, int&gt;</type>
        </field>
      </vtbl>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls | PInvokeGeneratorConfigurationOptions.GenerateMarkerInterfaces);
    }

    protected override Task StaticTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    static void MyVoidMethod();

    static int MyInt32Method()
    {
        return 0;
    }

    static void* MyVoidStarMethod()
    {
        return nullptr;
    }
};
";

        var entryPoint = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "?MyVoidMethod@MyStruct@@SAXXZ" : "_ZN8MyStruct12MyVoidMethodEv";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            entryPoint = $"_{entryPoint}";
        }

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"" unsafe=""true"">
      <function name=""MyVoidMethod"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" entrypoint=""{entryPoint}"" static=""true"">
        <type>void</type>
      </function>
      <function name=""MyInt32Method"" access=""public"" static=""true"">
        <type>int</type>
        <code>return 0;</code>
      </function>
      <function name=""MyVoidStarMethod"" access=""public"" static=""true"" unsafe=""true"">
        <type>void*</type>
        <code>return null;</code>
      </function>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task VirtualTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    virtual void MyVoidMethod() = 0;

    virtual signed char MyInt8Method()
    {
        return 0;
    }

    virtual int MyInt32Method();

    virtual void* MyVoidStarMethod() = 0;
};
";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"" vtbl=""true"" unsafe=""true"">
      <field name=""lpVtbl"" access=""public"">
        <type>void**</type>
      </field>
      <function name=""MyVoidMethod"" access=""public"" unsafe=""true"">
        <type>void</type>
        <body>
          <code>((delegate* unmanaged[Thiscall]&lt;MyStruct*, void&gt;)(lpVtbl[<vtbl explicit=""False"">0</vtbl>]))(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>);</code>
        </body>
      </function>
      <function name=""MyInt8Method"" access=""public"" unsafe=""true"">
        <type native=""signed char"">sbyte</type>
        <body>
          <code>return ((delegate* unmanaged[Thiscall]&lt;MyStruct*, sbyte&gt;)(lpVtbl[<vtbl explicit=""False"">1</vtbl>]))(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>);</code>
        </body>
      </function>
      <function name=""MyInt32Method"" access=""public"" unsafe=""true"">
        <type>int</type>
        <body>
          <code>return ((delegate* unmanaged[Thiscall]&lt;MyStruct*, int&gt;)(lpVtbl[<vtbl explicit=""False"">2</vtbl>]))(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>);</code>
        </body>
      </function>
      <function name=""MyVoidStarMethod"" access=""public"" unsafe=""true"">
        <type>void*</type>
        <body>
          <code>return ((delegate* unmanaged[Thiscall]&lt;MyStruct*, void*&gt;)(lpVtbl[<vtbl explicit=""False"">3</vtbl>]))(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>);</code>
        </body>
      </function>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task VirtualWithVtblIndexAttributeTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    virtual void MyVoidMethod() = 0;

    virtual signed char MyInt8Method()
    {
        return 0;
    }

    virtual int MyInt32Method();

    virtual void* MyVoidStarMethod() = 0;
};
";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"" vtbl=""true"" unsafe=""true"">
      <field name=""lpVtbl"" access=""public"">
        <type>void**</type>
      </field>
      <function name=""MyVoidMethod"" access=""public"" unsafe=""true"" vtblindex=""0"">
        <type>void</type>
        <body>
          <code>((delegate* unmanaged[Thiscall]&lt;MyStruct*, void&gt;)(lpVtbl[<vtbl explicit=""False"">0</vtbl>]))(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>);</code>
        </body>
      </function>
      <function name=""MyInt8Method"" access=""public"" unsafe=""true"" vtblindex=""1"">
        <type native=""signed char"">sbyte</type>
        <body>
          <code>return ((delegate* unmanaged[Thiscall]&lt;MyStruct*, sbyte&gt;)(lpVtbl[<vtbl explicit=""False"">1</vtbl>]))(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>);</code>
        </body>
      </function>
      <function name=""MyInt32Method"" access=""public"" unsafe=""true"" vtblindex=""2"">
        <type>int</type>
        <body>
          <code>return ((delegate* unmanaged[Thiscall]&lt;MyStruct*, int&gt;)(lpVtbl[<vtbl explicit=""False"">2</vtbl>]))(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>);</code>
        </body>
      </function>
      <function name=""MyVoidStarMethod"" access=""public"" unsafe=""true"" vtblindex=""3"">
        <type>void*</type>
        <body>
          <code>return ((delegate* unmanaged[Thiscall]&lt;MyStruct*, void*&gt;)(lpVtbl[<vtbl explicit=""False"">3</vtbl>]))(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>);</code>
        </body>
      </function>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateVtblIndexAttribute);
    }

    protected override Task ValidateBindingsAsync(string inputContents, string expectedOutputContents) => ValidateGeneratedXmlLatestUnixBindingsAsync(inputContents, expectedOutputContents);
}
