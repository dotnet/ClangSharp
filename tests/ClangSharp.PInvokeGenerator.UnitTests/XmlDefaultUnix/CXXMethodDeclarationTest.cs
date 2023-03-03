// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ClangSharp.UnitTests;

public sealed class XmlDefaultUnix_CXXMethodDeclarationTest : CXXMethodDeclarationXmlTest
{
    protected override Task ConstructorTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    int _value;

    MyStruct(int value)
    {
        _value = value;
    }
};
";

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"">
      <field name=""_value"" access=""public"">
        <type>int</type>
      </field>
      <function name=""MyStruct"" access=""public"">
        <type>void</type>
        <param name=""value"">
          <type>int</type>
        </param>
        <code>_value = value;</code>
      </function>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ConstructorWithInitializeTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    int _x;
    int _y;
    int _z;

    MyStruct(int x) : _x(x)
    {
    }

    MyStruct(int x, int y) : _x(x), _y(y)
    {
    }

    MyStruct(int x, int y, int z) : _x(x), _y(y), _z()
    {
    }
};
";

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"">
      <field name=""_x"" access=""public"">
        <type>int</type>
      </field>
      <field name=""_y"" access=""public"">
        <type>int</type>
      </field>
      <field name=""_z"" access=""public"">
        <type>int</type>
      </field>
      <function name=""MyStruct"" access=""public"">
        <type>void</type>
        <param name=""x"">
          <type>int</type>
        </param>
        <init field=""_x"" hint=""x"">
          <code>x</code>
        </init>
        <code></code>
      </function>
      <function name=""MyStruct"" access=""public"">
        <type>void</type>
        <param name=""x"">
          <type>int</type>
        </param>
        <param name=""y"">
          <type>int</type>
        </param>
        <init field=""_x"" hint=""x"">
          <code>x</code>
        </init>
        <init field=""_y"" hint=""y"">
          <code>y</code>
        </init>
        <code></code>
      </function>
      <function name=""MyStruct"" access=""public"">
        <type>void</type>
        <param name=""x"">
          <type>int</type>
        </param>
        <param name=""y"">
          <type>int</type>
        </param>
        <param name=""z"">
          <type>int</type>
        </param>
        <init field=""_x"" hint=""x"">
          <code>x</code>
        </init>
        <init field=""_y"" hint=""y"">
          <code>y</code>
        </init>
        <code></code>
      </function>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ConversionTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    int value;

    operator int()
    {
        return value;
    }
};
";

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"">
      <field name=""value"" access=""public"">
        <type>int</type>
      </field>
      <function name=""ToInt32"" access=""public"">
        <type>int</type>
        <code>return value;</code>
      </function>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task DestructorTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    ~MyStruct()
    {
    }
};
";

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"">
      <function name=""Dispose"" access=""public"">
        <type>void</type>
        <code></code>
      </function>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
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
            if (!Environment.Is64BitProcess)
            {
                entryPoint = "?MyVoidMethod@MyStruct@@QAEXXZ";
            }
            else
            {
                entryPoint = "?MyVoidMethod@MyStruct@@QEAAXXZ";
            }
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

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task MemberCallTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    int value;

    int MyFunction1()
    {
        return value;
    }

    int MyFunction2()
    {
        return MyFunction1();
    }

    int MyFunction3()
    {
        return this->MyFunction1();
    }
};

int MyFunctionA(MyStruct x)
{
    return x.MyFunction1();
}

int MyFunctionB(MyStruct* x)
{
    return x->MyFunction2();
}
";

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"">
      <field name=""value"" access=""public"">
        <type>int</type>
      </field>
      <function name=""MyFunction1"" access=""public"">
        <type>int</type>
        <code>return value;</code>
      </function>
      <function name=""MyFunction2"" access=""public"">
        <type>int</type>
        <code>return MyFunction1();</code>
      </function>
      <function name=""MyFunction3"" access=""public"">
        <type>int</type>
        <code>return this.MyFunction1();</code>
      </function>
    </struct>
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunctionA"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""x"">
          <type>MyStruct</type>
        </param>
        <code>return x.MyFunction1();</code>
      </function>
      <function name=""MyFunctionB"" access=""public"" static=""true"" unsafe=""true"">
        <type>int</type>
        <param name=""x"">
          <type>MyStruct*</type>
        </param>
        <code>return x-&gt;MyFunction2();</code>
      </function>
    </class>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task MemberTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    int value;

    int MyFunction()
    {
        return value;
    }
};
";

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"">
      <field name=""value"" access=""public"">
        <type>int</type>
      </field>
      <function name=""MyFunction"" access=""public"">
        <type>int</type>
        <code>return value;</code>
      </function>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task NewKeywordTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    int Equals() { return 0; }
    int Equals(int obj) { return 0; }
    int Dispose() { return 0; }
    int Dispose(int obj) { return 0; }
    int GetHashCode() { return 0; }
    int GetHashCode(int obj) { return 0; }
    int GetType() { return 0; }
    int GetType(int obj) { return 0; }
    int MemberwiseClone() { return 0; }
    int MemberwiseClone(int obj) { return 0; }
    int ReferenceEquals() { return 0; }
    int ReferenceEquals(int obj) { return 0; }
    int ToString() { return 0; }
    int ToString(int obj) { return 0; }
};";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"">
      <function name=""Equals"" access=""public"">
        <type>int</type>
        <code>return 0;</code>
      </function>
      <function name=""Equals"" access=""public"">
        <type>int</type>
        <param name=""obj"">
          <type>int</type>
        </param>
        <code>return 0;</code>
      </function>
      <function name=""Dispose"" access=""public"">
        <type>int</type>
        <code>return 0;</code>
      </function>
      <function name=""Dispose"" access=""public"">
        <type>int</type>
        <param name=""obj"">
          <type>int</type>
        </param>
        <code>return 0;</code>
      </function>
      <function name=""GetHashCode"" access=""public"">
        <type>int</type>
        <code>return 0;</code>
      </function>
      <function name=""GetHashCode"" access=""public"">
        <type>int</type>
        <param name=""obj"">
          <type>int</type>
        </param>
        <code>return 0;</code>
      </function>
      <function name=""GetType"" access=""public"">
        <type>int</type>
        <code>return 0;</code>
      </function>
      <function name=""GetType"" access=""public"">
        <type>int</type>
        <param name=""obj"">
          <type>int</type>
        </param>
        <code>return 0;</code>
      </function>
      <function name=""MemberwiseClone"" access=""public"">
        <type>int</type>
        <code>return 0;</code>
      </function>
      <function name=""MemberwiseClone"" access=""public"">
        <type>int</type>
        <param name=""obj"">
          <type>int</type>
        </param>
        <code>return 0;</code>
      </function>
      <function name=""ReferenceEquals"" access=""public"">
        <type>int</type>
        <code>return 0;</code>
      </function>
      <function name=""ReferenceEquals"" access=""public"">
        <type>int</type>
        <param name=""obj"">
          <type>int</type>
        </param>
        <code>return 0;</code>
      </function>
      <function name=""ToString"" access=""public"">
        <type>int</type>
        <code>return 0;</code>
      </function>
      <function name=""ToString"" access=""public"">
        <type>int</type>
        <param name=""obj"">
          <type>int</type>
        </param>
        <code>return 0;</code>
      </function>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
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
        <param name=""objA"">
          <type>int</type>
        </param>
        <param name=""objB"">
          <type>int</type>
        </param>
        <body>
          <code>return ((delegate* unmanaged[Thiscall]&lt;MyStruct*, int, int, int&gt;)(lpVtbl[<vtbl explicit=""False"">0</vtbl>]))(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>, <param name=""objA"">objA</param>, <param name=""objB"">objB</param>);</code>
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
        <param name=""obj"">
          <type>int</type>
        </param>
        <body>
          <code>return ((delegate* unmanaged[Thiscall]&lt;MyStruct*, int, int&gt;)(lpVtbl[<vtbl explicit=""False"">2</vtbl>]))(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>, <param name=""obj"">obj</param>);</code>
        </body>
      </function>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
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
        <param name=""objA"">
          <type>int</type>
        </param>
        <param name=""objB"">
          <type>int</type>
        </param>
        <body>
          <code>return lpVtbl-&gt;<vtbl explicit=""True"">GetType</vtbl>(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>, <param name=""objA"">objA</param>, <param name=""objB"">objB</param>);</code>
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
        <param name=""obj"">
          <type>int</type>
        </param>
        <body>
          <code>return lpVtbl-&gt;<vtbl explicit=""True"">GetType2</vtbl>(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>, <param name=""obj"">obj</param>);</code>
        </body>
      </function>
      <vtbl>
        <field name=""GetType"" access=""public"">
          <type native=""int (int, int){nativeCallConv}"">delegate* unmanaged[Thiscall]&lt;MyStruct*, int, int, int&gt;</type>
        </field>
        <field name=""GetType1"" access=""public"">
          <type native=""int (){nativeCallConv}"">delegate* unmanaged[Thiscall]&lt;MyStruct*, int&gt;</type>
        </field>
        <field name=""GetType2"" access=""public"">
          <type native=""int (int){nativeCallConv}"">delegate* unmanaged[Thiscall]&lt;MyStruct*, int, int&gt;</type>
        </field>
      </vtbl>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls);
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
        <param name=""objA"">
          <type>int</type>
        </param>
        <param name=""objB"">
          <type>int</type>
        </param>
        <body>
          <code>return lpVtbl-&gt;<vtbl explicit=""True"">GetType</vtbl>(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>, <param name=""objA"">objA</param>, <param name=""objB"">objB</param>);</code>
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
        <param name=""obj"">
          <type>int</type>
        </param>
        <body>
          <code>return lpVtbl-&gt;<vtbl explicit=""True"">GetType2</vtbl>(<param special=""thisPtr"">(MyStruct*)Unsafe.AsPointer(ref this)</param>, <param name=""obj"">obj</param>);</code>
        </body>
      </function>
      <interface>
        <function name=""GetType"" access=""public"" unsafe=""true"">
          <type>int</type>
          <param name=""objA"">
            <type>int</type>
          </param>
          <param name=""objB"">
            <type>int</type>
          </param>
        </function>
        <function name=""GetType"" access=""public"" unsafe=""true"">
          <type>int</type>
        </function>
        <function name=""GetType"" access=""public"" unsafe=""true"">
          <type>int</type>
          <param name=""obj"">
            <type>int</type>
          </param>
        </function>
      </interface>
      <vtbl>
        <field name=""GetType"" access=""public"">
          <type native=""int (int, int){nativeCallConv}"">delegate* unmanaged[Thiscall]&lt;TSelf*, int, int, int&gt;</type>
        </field>
        <field name=""GetType1"" access=""public"">
          <type native=""int (){nativeCallConv}"">delegate* unmanaged[Thiscall]&lt;TSelf*, int&gt;</type>
        </field>
        <field name=""GetType2"" access=""public"">
          <type native=""int (int){nativeCallConv}"">delegate* unmanaged[Thiscall]&lt;TSelf*, int, int&gt;</type>
        </field>
      </vtbl>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls | PInvokeGeneratorConfigurationOptions.GenerateMarkerInterfaces);
    }

    protected override Task OperatorTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    int value;

    MyStruct(int value) : value(value)
    {
    }

    MyStruct operator+(MyStruct rhs)
    {
        return MyStruct(value + rhs.value);
    }
};

MyStruct operator-(MyStruct lhs, MyStruct rhs)
{
    return MyStruct(lhs.value - rhs.value);
}
";

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"">
      <field name=""value"" access=""public"">
        <type>int</type>
      </field>
      <function name=""MyStruct"" access=""public"">
        <type>void</type>
        <param name=""value"">
          <type>int</type>
        </param>
        <init field=""value"" hint=""value"">
          <code>value</code>
        </init>
        <code></code>
      </function>
      <function name=""Add"" access=""public"">
        <type>MyStruct</type>
        <param name=""rhs"">
          <type>MyStruct</type>
        </param>
        <code>return new MyStruct(value + rhs.value);</code>
      </function>
    </struct>
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""Subtract"" access=""public"" static=""true"">
        <type>MyStruct</type>
        <param name=""lhs"">
          <type>MyStruct</type>
        </param>
        <param name=""rhs"">
          <type>MyStruct</type>
        </param>
        <code>return new MyStruct(lhs.value - rhs.value);</code>
      </function>
    </class>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task OperatorCallTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    int value;

    MyStruct(int value) : value(value)
    {
    }

    MyStruct operator+(MyStruct rhs)
    {
        return MyStruct(value + rhs.value);
    }
};

MyStruct MyFunction1(MyStruct lhs, MyStruct rhs)
{
    return lhs + rhs;
}

MyStruct operator-(MyStruct lhs, MyStruct rhs)
{
    return MyStruct(lhs.value - rhs.value);
}

MyStruct MyFunction2(MyStruct lhs, MyStruct rhs)
{
    return lhs - rhs;
}
";

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"">
      <field name=""value"" access=""public"">
        <type>int</type>
      </field>
      <function name=""MyStruct"" access=""public"">
        <type>void</type>
        <param name=""value"">
          <type>int</type>
        </param>
        <init field=""value"" hint=""value"">
          <code>value</code>
        </init>
        <code></code>
      </function>
      <function name=""Add"" access=""public"">
        <type>MyStruct</type>
        <param name=""rhs"">
          <type>MyStruct</type>
        </param>
        <code>return new MyStruct(value + rhs.value);</code>
      </function>
    </struct>
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction1"" access=""public"" static=""true"">
        <type>MyStruct</type>
        <param name=""lhs"">
          <type>MyStruct</type>
        </param>
        <param name=""rhs"">
          <type>MyStruct</type>
        </param>
        <code>return lhs.Add(rhs);</code>
      </function>
      <function name=""Subtract"" access=""public"" static=""true"">
        <type>MyStruct</type>
        <param name=""lhs"">
          <type>MyStruct</type>
        </param>
        <param name=""rhs"">
          <type>MyStruct</type>
        </param>
        <code>return new MyStruct(lhs.value - rhs.value);</code>
      </function>
      <function name=""MyFunction2"" access=""public"" static=""true"">
        <type>MyStruct</type>
        <param name=""lhs"">
          <type>MyStruct</type>
        </param>
        <param name=""rhs"">
          <type>MyStruct</type>
        </param>
        <code>return Subtract(lhs, rhs);</code>
      </function>
    </class>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
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

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task ThisTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    int value;

    int MyFunction()
    {
        return this->value;
    }
};
";

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"">
      <field name=""value"" access=""public"">
        <type>int</type>
      </field>
      <function name=""MyFunction"" access=""public"">
        <type>int</type>
        <code>return this.value;</code>
      </function>
    </struct>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task UnsafeDoesNotImpactDllImportTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    void* MyVoidStarMethod()
    {
        return nullptr;
    }
};

extern ""C"" void MyFunction();";

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"" unsafe=""true"">
      <function name=""MyVoidStarMethod"" access=""public"" unsafe=""true"">
        <type>void*</type>
        <code>return null;</code>
      </function>
    </struct>
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" lib=""ClangSharpPInvokeGenerator"" convention=""Cdecl"" static=""true"">
        <type>void</type>
      </function>
    </class>
  </namespace>
</bindings>
";

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task VirtualTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    virtual void MyVoidMethod() = 0;

    virtual char MyInt8Method()
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
        <type native=""char"">sbyte</type>
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

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task VirtualWithVtblIndexAttributeTestImpl()
    {
        var inputContents = @"struct MyStruct
{
    virtual void MyVoidMethod() = 0;

    virtual char MyInt8Method()
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
        <type native=""char"">sbyte</type>
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

        return ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateVtblIndexAttribute);
    }

    protected override Task ValidateBindingsAsync(string inputContents, string expectedOutputContents) => ValidateGeneratedXmlDefaultUnixBindingsAsync(inputContents, expectedOutputContents);
}
