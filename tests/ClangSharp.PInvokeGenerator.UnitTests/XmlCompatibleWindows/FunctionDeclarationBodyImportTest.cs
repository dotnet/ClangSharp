// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ClangSharp.UnitTests
{
    public sealed class XmlCompatibleWindows_FunctionDeclarationBodyImportTest : FunctionDeclarationBodyImportTest
    {
        public override Task ArraySubscriptTest()
        {
            var inputContents = @"int MyFunction(int* pData, int index)
{
    return pData[index];
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"" unsafe=""true"">
        <type>int</type>
        <param name=""pData"">
          <type>int*</type>
        </param>
        <param name=""index"">
          <type>int</type>
        </param>
        <code>return pData[index];</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task BasicTest()
        {
            var inputContents = @"void MyFunction()
{
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>void</type>
        <code></code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task BinaryOperatorBasicTest(string opcode)
        {
            var inputContents = $@"int MyFunction(int x, int y)
{{
    return x {opcode} y;
}}
";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""x"">
          <type>int</type>
        </param>
        <param name=""y"">
          <type>int</type>
        </param>
        <code>return x {EscapeXml(opcode)} y;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task BinaryOperatorCompareTest(string opcode)
        {
            var inputContents = $@"bool MyFunction(int x, int y)
{{
    return x {opcode} y;
}}
";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>bool</type>
        <param name=""x"">
          <type>int</type>
        </param>
        <param name=""y"">
          <type>int</type>
        </param>
        <code>return x {EscapeXml(opcode)} y;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task BinaryOperatorBooleanTest(string opcode)
        {
            var inputContents = $@"bool MyFunction(bool x, bool y)
{{
    return x {opcode} y;
}}
";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>bool</type>
        <param name=""x"">
          <type>bool</type>
        </param>
        <param name=""y"">
          <type>bool</type>
        </param>
        <code>return x {EscapeXml(opcode)} y;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task BreakTest()
        {
            var inputContents = @"int MyFunction(int value)
{
    while (true)
    {
        break;
    }

    return 0;
}
";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""value"">
          <type>int</type>
        </param>
        <code>while (true)
      {{
          break;
      }}

      return 0;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CallFunctionTest()
        {
            var inputContents = @"void MyCalledFunction()
{
}

void MyFunction()
{
    MyCalledFunction();
}
";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyCalledFunction"" access=""public"" static=""true"">
        <type>void</type>
        <code></code>
      </function>
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>void</type>
        <code>MyCalledFunction();</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CallFunctionWithArgsTest()
        {
            var inputContents = @"void MyCalledFunction(int x, int y)
{
}

void MyFunction()
{
    MyCalledFunction(0, 1);
}
";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyCalledFunction"" access=""public"" static=""true"">
        <type>void</type>
        <param name=""x"">
          <type>int</type>
        </param>
        <param name=""y"">
          <type>int</type>
        </param>
        <code></code>
      </function>
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>void</type>
        <code>MyCalledFunction(0, 1);</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CaseTest()
        {
            var inputContents = @"int MyFunction(int value)
{
    switch (value)
    {
        case 0:
        {
            return 0;
        }

        case 1:
        case 2:
        {
            return 3;
        }
    }

    return -1;
}
";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""value"">
          <type>int</type>
        </param>
        <code>switch (value)
      {{
          case 0:
          {{
              return 0;
          }}

          case 1:
          case 2:
          {{
              return 3;
          }}
      }}

      return -1;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CaseNoCompoundTest()
        {
            var inputContents = @"int MyFunction(int value)
{
    switch (value)
    {
        case 0:
            return 0;

        case 2:
        case 3:
            return 5;
    }

    return -1;
}
";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""value"">
          <type>int</type>
        </param>
        <code>switch (value)
      {{
          case 0:
          {{
              return 0;
          }}

          case 2:
          case 3:
          {{
              return 5;
          }}
      }}

      return -1;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CompareMultipleEnumTest()
        {
            var inputContents = @"enum MyEnum : int
{
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
};

static inline int MyFunction(MyEnum x)
{
    return x == MyEnum_Value0 ||
           x == MyEnum_Value1 ||
           x == MyEnum_Value2;
}
";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <enumeration name=""MyEnum"" access=""public"">
      <type>int</type>
      <enumerator name=""MyEnum_Value0"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
      <enumerator name=""MyEnum_Value1"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
      <enumerator name=""MyEnum_Value2"" access=""public"">
        <type primitive=""False"">int</type>
      </enumerator>
    </enumeration>
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""x"">
          <type>MyEnum</type>
        </param>
        <code>return (<value>x == MyEnum_Value0 || x == MyEnum_Value1 || x == MyEnum_Value2</value>) ? 1 : 0;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ConditionalOperatorTest()
        {
            var inputContents = @"int MyFunction(bool condition, int lhs, int rhs)
{
    return condition ? lhs : rhs;
}
";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""condition"">
          <type>bool</type>
        </param>
        <param name=""lhs"">
          <type>int</type>
        </param>
        <param name=""rhs"">
          <type>int</type>
        </param>
        <code>return condition ? lhs : rhs;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ContinueTest()
        {
            var inputContents = @"int MyFunction(int value)
{
    while (true)
    {
        continue;
    }

    return 0;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""value"">
          <type>int</type>
        </param>
        <code>while (true)
      {
          continue;
      }

      return 0;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CStyleFunctionalCastTest()
        {
            var inputContents = @"int MyFunction(float input)
{
    return (int)input;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""input"">
          <type>float</type>
        </param>
        <code>return (int)(<value>input</value>);</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CxxFunctionalCastTest()
        {
            var inputContents = @"int MyFunction(float input)
{
    return int(input);
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""input"">
          <type>float</type>
        </param>
        <code>return (int)(<value>input</value>);</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CxxConstCastTest()
        {
            var inputContents = @"void* MyFunction(const void* input)
{
    return const_cast<void*>(input);
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"" unsafe=""true"">
        <type>void*</type>
        <param name=""input"">
          <type>void*</type>
        </param>
        <code>return input;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CxxDynamicCastTest()
        {
            var inputContents = @"struct MyStructA
{
    virtual void MyMethod() = 0;
};

struct MyStructB : MyStructA { };

MyStructB* MyFunction(MyStructA* input)
{
    return dynamic_cast<MyStructB*>(input);
}
";

            var callConv = "Cdecl";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !Environment.Is64BitProcess)
            {
                callConv = "ThisCall";
            }

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStructA"" access=""public"" vtbl=""true"" unsafe=""true"">
      <field name=""lpVtbl"" access=""public"">
        <type>void**</type>
      </field>
      <delegate name=""_MyMethod"" access=""public"" convention=""{callConv}"">
        <type>void</type>
        <param name=""pThis"">
          <type>MyStructA*</type>
        </param>
      </delegate>
      <function name=""MyMethod"" access=""public"" unsafe=""true"">
        <type>void</type>
        <body>
          <code>fixed (MyStructA* pThis = &amp;this)
    {{
        Marshal.GetDelegateForFunctionPointer&lt;<delegate>_MyMethod</delegate>&gt;((IntPtr)(lpVtbl[<vtbl explicit=""False"">0</vtbl>]))(<param special=""thisPtr"">pThis</param>);
    }}</code>
        </body>
      </function>
    </struct>
    <struct name=""MyStructB"" access=""public"" native=""struct MyStructB : MyStructA"" vtbl=""true"" unsafe=""true"">
      <field name=""lpVtbl"" access=""public"">
        <type>void**</type>
      </field>
      <delegate name=""_MyMethod"" access=""public"" convention=""{callConv}"">
        <type>void</type>
        <param name=""pThis"">
          <type>MyStructB*</type>
        </param>
      </delegate>
      <function name=""MyMethod"" access=""public"" unsafe=""true"">
        <type>void</type>
        <body>
          <code>fixed (MyStructB* pThis = &amp;this)
    {{
        Marshal.GetDelegateForFunctionPointer&lt;<delegate>_MyMethod</delegate>&gt;((IntPtr)(lpVtbl[<vtbl explicit=""False"">0</vtbl>]))(<param special=""thisPtr"">pThis</param>);
    }}</code>
        </body>
      </function>
    </struct>
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"" unsafe=""true"">
        <type>MyStructB*</type>
        <param name=""input"">
          <type>MyStructA*</type>
        </param>
        <code>return (MyStructB*)(<value>input</value>);</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CxxReinterpretCastTest()
        {
            var inputContents = @"int* MyFunction(void* input)
{
    return reinterpret_cast<int*>(input);
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"" unsafe=""true"">
        <type>int*</type>
        <param name=""input"">
          <type>void*</type>
        </param>
        <code>return (int*)(<value>input</value>);</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task CxxStaticCastTest()
        {
            var inputContents = @"int* MyFunction(void* input)
{
    return static_cast<int*>(input);
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"" unsafe=""true"">
        <type>int*</type>
        <param name=""input"">
          <type>void*</type>
        </param>
        <code>return (int*)(<value>input</value>);</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task DeclTest()
        {
            var inputContents = @"\
int MyFunction()
{
    int x = 0;
    int y = 1, z = 2;
    return x + y + z;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <code>int x = 0;
      int y = 1, z = 2;

      return x + y + z;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task DoTest()
        {
            var inputContents = @"int MyFunction(int count)
{
    int i = 0;

    do
    {
        i++;
    }
    while (i < count);

    return i;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""count"">
          <type>int</type>
        </param>
        <code>int i = 0;

      do
      {
          i++;
      }
      while (i &lt; count);

      return i;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task DoNonCompoundTest()
        {
            var inputContents = @"int MyFunction(int count)
{
    int i = 0;

    while (i < count)
        i++;

    return i;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""count"">
          <type>int</type>
        </param>
        <code>int i = 0;

      while (i &lt; count)
      {
          i++;
      }

      return i;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ForTest()
        {
            var inputContents = @"int MyFunction(int count)
{
    for (int i = 0; i < count; i--)
    {
        i += 2;
    }

    int x;

    for (x = 0; x < count; x--)
    {
        x += 2;
    }

    x = 0;

    for (; x < count; x--)
    {
        x += 2;
    }

    for (int i = 0;;i--)
    {
        i += 2;
    }

    for (x = 0;;x--)
    {
        x += 2;
    }

    for (int i = 0; i < count;)
    {
        i++;
    }

    for (x = 0; x < count;)
    {
        x++;
    }

    // x = 0;
    // 
    // for (;; x--)
    // {
    //     x += 2;
    // }

    x = 0;

    for (; x < count;)
    {
        x++;
    }

    for (int i = 0;;)
    {
        i++;
    }

    for (x = 0;;)
    {
        x++;
    }

    for (;;)
    {
        return -1;
    }
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""count"">
          <type>int</type>
        </param>
        <code>for (int i = 0; i &lt; count; i--)
      {
          i += 2;
      }

      int x;

      for (x = 0; x &lt; count; x--)
      {
          x += 2;
      }

      x = 0;
      for (; x &lt; count; x--)
      {
          x += 2;
      }

      for (int i = 0;; i--)
      {
          i += 2;
      }

      for (x = 0;; x--)
      {
          x += 2;
      }

      for (int i = 0; i &lt; count;)
      {
          i++;
      }

      for (x = 0; x &lt; count;)
      {
          x++;
      }

      x = 0;
      for (; x &lt; count;)
      {
          x++;
      }

      for (int i = 0;;)
      {
          i++;
      }

      for (x = 0;;)
      {
          x++;
      }

      for (;;)
      {
          return -1;
      }</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ForNonCompoundTest()
        {
            var inputContents = @"int MyFunction(int count)
{
    for (int i = 0; i < count; i--)
        i += 2;

    int x;

    for (x = 0; x < count; x--)
        x += 2;

    x = 0;

    for (; x < count; x--)
        x += 2;

    for (int i = 0;;i--)
        i += 2;

    for (x = 0;;x--)
        x += 2;

    for (int i = 0; i < count;)
        i++;

    for (x = 0; x < count;)
        x++;

    // x = 0;
    // 
    // for (;; x--)
    //     x += 2;

    x = 0;

    for (; x < count;)
        x++;

    for (int i = 0;;)
        i++;

    for (x = 0;;)
        x++;

    for (;;)
        return -1;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""count"">
          <type>int</type>
        </param>
        <code>for (int i = 0; i &lt; count; i--)
      {
          i += 2;
      }

      int x;

      for (x = 0; x &lt; count; x--)
      {
          x += 2;
      }

      x = 0;
      for (; x &lt; count; x--)
      {
          x += 2;
      }

      for (int i = 0;; i--)
      {
          i += 2;
      }

      for (x = 0;; x--)
      {
          x += 2;
      }

      for (int i = 0; i &lt; count;)
      {
          i++;
      }

      for (x = 0; x &lt; count;)
      {
          x++;
      }

      x = 0;
      for (; x &lt; count;)
      {
          x++;
      }

      for (int i = 0;;)
      {
          i++;
      }

      for (x = 0;;)
      {
          x++;
      }

      for (;;)
      {
          return -1;
      }</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task IfTest()
        {
            var inputContents = @"int MyFunction(bool condition, int lhs, int rhs)
{
    if (condition)
    {
        return lhs;
    }

    return rhs;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""condition"">
          <type>bool</type>
        </param>
        <param name=""lhs"">
          <type>int</type>
        </param>
        <param name=""rhs"">
          <type>int</type>
        </param>
        <code>if (condition)
      {
          return lhs;
      }

      return rhs;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task IfElseTest()
        {
            var inputContents = @"int MyFunction(bool condition, int lhs, int rhs)
{
    if (condition)
    {
        return lhs;
    }
    else
    {
        return rhs;
    }
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""condition"">
          <type>bool</type>
        </param>
        <param name=""lhs"">
          <type>int</type>
        </param>
        <param name=""rhs"">
          <type>int</type>
        </param>
        <code>if (condition)
      {
          return lhs;
      }
      else
      {
          return rhs;
      }</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task IfElseIfTest()
        {
            var inputContents = @"int MyFunction(bool condition1, int a, int b, bool condition2, int c)
{
    if (condition1)
    {
        return a;
    }
    else if (condition2)
    {
        return b;
    }

    return c;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""condition1"">
          <type>bool</type>
        </param>
        <param name=""a"">
          <type>int</type>
        </param>
        <param name=""b"">
          <type>int</type>
        </param>
        <param name=""condition2"">
          <type>bool</type>
        </param>
        <param name=""c"">
          <type>int</type>
        </param>
        <code>if (condition1)
      {
          return a;
      }
      else if (condition2)
      {
          return b;
      }

      return c;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task IfElseNonCompoundTest()
        {
            var inputContents = @"int MyFunction(bool condition, int lhs, int rhs)
{
    if (condition)
        return lhs;
    else
        return rhs;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""condition"">
          <type>bool</type>
        </param>
        <param name=""lhs"">
          <type>int</type>
        </param>
        <param name=""rhs"">
          <type>int</type>
        </param>
        <code>if (condition)
      {
          return lhs;
      }
      else
      {
          return rhs;
      }</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task InitListForArrayTest()
        {
            var inputContents = @"
void MyFunction()
{
    int x[4] = { 1, 2, 3, 4 };
    int y[4] = { 1, 2, 3 };
    int z[] = { 1, 2 };
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>void</type>
        <code>int[] x = new int[4]
      {
          1,
          2,
          3,
          4,
      };
      int[] y = new int[4]
      {
          1,
          2,
          3,
          default,
      };
      int[] z = new int[2]
      {
          1,
          2,
      };</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task InitListForRecordDeclTest()
        {
            var inputContents = @"struct MyStruct
{
    float x;
    float y;
    float z;
    float w;
};

MyStruct MyFunction1()
{
    return { 1.0f, 2.0f, 3.0f, 4.0f };
}

MyStruct MyFunction2()
{
    return { 1.0f, 2.0f, 3.0f };
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"">
      <field name=""x"" access=""public"">
        <type>float</type>
      </field>
      <field name=""y"" access=""public"">
        <type>float</type>
      </field>
      <field name=""z"" access=""public"">
        <type>float</type>
      </field>
      <field name=""w"" access=""public"">
        <type>float</type>
      </field>
    </struct>
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction1"" access=""public"" static=""true"">
        <type>MyStruct</type>
        <code>return new MyStruct
      {
          x = 1.0f,
          y = 2.0f,
          z = 3.0f,
          w = 4.0f,
      };</code>
      </function>
      <function name=""MyFunction2"" access=""public"" static=""true"">
        <type>MyStruct</type>
        <code>return new MyStruct
      {
          x = 1.0f,
          y = 2.0f,
          z = 3.0f,
      };</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task MemberTest()
        {
            var inputContents = @"struct MyStruct
{
    int value;
};

int MyFunction1(MyStruct instance)
{
    return instance.value;
}

int MyFunction2(MyStruct* instance)
{
    return instance->value;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"">
      <field name=""value"" access=""public"">
        <type>int</type>
      </field>
    </struct>
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction1"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""instance"">
          <type>MyStruct</type>
        </param>
        <code>return instance.value;</code>
      </function>
      <function name=""MyFunction2"" access=""public"" static=""true"" unsafe=""true"">
        <type>int</type>
        <param name=""instance"">
          <type>MyStruct*</type>
        </param>
        <code>return instance-&gt;value;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task RefToPtrTest()
        {
            var inputContents = @"struct MyStruct {
    int value;
};

bool MyFunction(const MyStruct& lhs, const MyStruct& rhs)
{
    return lhs.value == rhs.value;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"">
      <field name=""value"" access=""public"">
        <type>int</type>
      </field>
    </struct>
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"" unsafe=""true"">
        <type>bool</type>
        <param name=""lhs"">
          <type>MyStruct*</type>
        </param>
        <param name=""rhs"">
          <type>MyStruct*</type>
        </param>
        <code>return lhs-&gt;value == rhs-&gt;value;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ReturnCXXNullPtrTest()
        {
            var inputContents = @"void* MyFunction()
{
    return nullptr;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"" unsafe=""true"">
        <type>void*</type>
        <code>return null;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ReturnCXXBooleanLiteralTest(string value)
        {
            var inputContents = $@"bool MyFunction()
{{
    return {value};
}}
";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>bool</type>
        <code>return {value};</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ReturnFloatingLiteralDoubleTest(string value)
        {
            var inputContents = $@"double MyFunction()
{{
    return {value};
}}
";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>double</type>
        <code>return {value};</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ReturnFloatingLiteralSingleTest(string value)
        {
            var inputContents = $@"float MyFunction()
{{
    return {value};
}}
";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>float</type>
        <code>return {value};</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ReturnEmptyTest()
        {
            var inputContents = @"void MyFunction()
{
    return;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>void</type>
        <code>return;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ReturnIntegerLiteralInt32Test()
        {
            var inputContents = @"int MyFunction()
{
    return -1;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <code>return -1;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task AccessUnionMemberTest()
        {
            var inputContents = @"union MyUnion
{
    struct { int a; };
};

void MyFunction()
{
    MyUnion myUnion;  
    myUnion.a = 10;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyUnion"" access=""public"" layout=""Explicit"">
      <field name=""Anonymous"" access=""public"" offset=""0"">
        <type native=""MyUnion::(anonymous struct at ClangUnsavedFile.h:3:5)"">_Anonymous_e__Struct</type>
      </field>
      <field name=""a"" access=""public"">
        <type>ref int</type>
        <get>
          <code>fixed (_Anonymous_e__Struct* pField = &amp;Anonymous)
    {
        return ref pField-&gt;a;
    }</code>
        </get>
      </field>
      <struct name=""_Anonymous_e__Struct"" access=""public"">
        <field name=""a"" access=""public"">
          <type>int</type>
        </field>
      </struct>
    </struct>
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>void</type>
        <code>MyUnion myUnion = new MyUnion();

      myUnion.Anonymous.a = 10;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task ReturnStructTest()
        {
            var inputContents = @"struct MyStruct
{
    double r;
    double g;
    double b;
};

MyStruct MyFunction()
{
    MyStruct myStruct;
    return myStruct;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyStruct"" access=""public"">
      <field name=""r"" access=""public"">
        <type>double</type>
      </field>
      <field name=""g"" access=""public"">
        <type>double</type>
      </field>
      <field name=""b"" access=""public"">
        <type>double</type>
      </field>
    </struct>
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>MyStruct</type>
        <code>MyStruct myStruct = new MyStruct();

      return myStruct;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task SwitchTest()
        {
            var inputContents = @"int MyFunction(int value)
{
    switch (value)
    {
        default:
        {
            return 0;
        }
    }
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""value"">
          <type>int</type>
        </param>
        <code>switch (value)
      {
          default:
          {
              return 0;
          }
      }</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task SwitchNonCompoundTest()
        {
            var inputContents = @"int MyFunction(int value)
{
    switch (value)
        default:
        {
            return 0;
        }

    switch (value)
        default:
            return 0;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""value"">
          <type>int</type>
        </param>
        <code>switch (value)
      {
          default:
          {
              return 0;
          }
      }

      switch (value)
      {
          default:
          {
              return 0;
          }
      }</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task UnaryOperatorAddrOfTest()
        {
            var inputContents = @"int* MyFunction(int value)
{
    return &value;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"" unsafe=""true"">
        <type>int*</type>
        <param name=""value"">
          <type>int</type>
        </param>
        <code>return &amp;value;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task UnaryOperatorDerefTest()
        {
            var inputContents = @"int MyFunction(int* value)
{
    return *value;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"" unsafe=""true"">
        <type>int</type>
        <param name=""value"">
          <type>int*</type>
        </param>
        <code>return *value;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task UnaryOperatorLogicalNotTest()
        {
            var inputContents = @"bool MyFunction(bool value)
{
    return !value;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>bool</type>
        <param name=""value"">
          <type>bool</type>
        </param>
        <code>return !value;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task UnaryOperatorPostfixTest(string opcode)
        {
            var inputContents = $@"int MyFunction(int value)
{{
    return value{opcode};
}}
";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""value"">
          <type>int</type>
        </param>
        <code>return value{opcode};</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task UnaryOperatorPrefixTest(string opcode)
        {
            var inputContents = $@"int MyFunction(int value)
{{
    return {opcode}value;
}}
";

            var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""value"">
          <type>int</type>
        </param>
        <code>return {opcode}value;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task WhileTest()
        {
            var inputContents = @"int MyFunction(int count)
{
    int i = 0;

    while (i < count)
    {
        i++;
    }

    return i;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""count"">
          <type>int</type>
        </param>
        <code>int i = 0;

      while (i &lt; count)
      {
          i++;
      }

      return i;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }

        public override Task WhileNonCompoundTest()
        {
            var inputContents = @"int MyFunction(int count)
{
    int i = 0;

    while (i < count)
        i++;

    return i;
}
";

            var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""MyFunction"" access=""public"" static=""true"">
        <type>int</type>
        <param name=""count"">
          <type>int</type>
        </param>
        <code>int i = 0;

      while (i &lt; count)
      {
          i++;
      }

      return i;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

            return ValidateGeneratedXmlCompatibleWindowsBindingsAsync(inputContents, expectedOutputContents);
        }
    }
}
