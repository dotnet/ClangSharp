// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;

namespace ClangSharp.UnitTests;

public abstract class CXXMethodDeclarationXmlTest: CXXMethodDeclarationTest
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
    }

    protected override Task MacrosExpansionTestImpl()
    {
        var inputContents = @"typedef struct
{
    unsigned char *buf;
    int size;
} context_t;

int buf_close(void *pcontext)
{
    ((context_t*)pcontext)->buf=0;
    return 0;
}
";

        var expectedOutputContents = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""context_t"" access=""public"" unsafe=""true"">
      <field name=""buf"" access=""public"">
        <type native=""unsigned char *"">byte*</type>
      </field>
      <field name=""size"" access=""public"">
        <type>int</type>
      </field>
    </struct>
    <class name=""Methods"" access=""public"" static=""true"">
      <function name=""buf_close"" access=""public"" static=""true"" unsafe=""true"">
        <type>int</type>
        <param name=""pcontext"">
          <type>void*</type>
        </param>
        <code>((context_t*)(<value>pcontext</value>))-&gt;buf = null;
      return 0;</code>
      </function>
    </class>
  </namespace>
</bindings>
";

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
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

        return ValidateBindingsAsync(inputContents, expectedOutputContents);
    }
}
