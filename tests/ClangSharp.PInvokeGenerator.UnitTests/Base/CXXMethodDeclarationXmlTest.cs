// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public abstract class CXXMethodDeclarationXmlTest: CXXMethodDeclarationTest
{
    [Test]
    public override Task MacrosExpansionTest()
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
}
