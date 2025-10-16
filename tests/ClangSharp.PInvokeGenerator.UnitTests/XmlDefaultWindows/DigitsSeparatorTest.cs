// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests.XmlDefaultWindows;

[Platform("win")]
public sealed class DigitsSeparatorTest : UnitTests.DigitsSeparatorTest
{
    protected override Task StaticConstExprTestImpl(string type, string nativeValue, string expectedValue)
    {
        var inputContents = $@"class MyClass
{{
    private:

      static constexpr {type} x = {nativeValue};
}};
";

        var expectedOutputContents = $@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes"" ?>
<bindings>
  <namespace name=""ClangSharp.Test"">
    <struct name=""MyClass"" access=""public"">
      <constant name=""x"" access=""private"">
        <type primitive=""True"">{type}</type>
        <value>
          <code>{expectedValue}</code>
        </value>
      </constant>
    </struct>
  </namespace>
</bindings>
";
        return ValidateGeneratedXmlDefaultWindowsBindingsAsync(
            inputContents,
            expectedOutputContents);
    }
}
