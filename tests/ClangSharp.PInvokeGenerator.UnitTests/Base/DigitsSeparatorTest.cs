// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public abstract class DigitsSeparatorTest : PInvokeGeneratorTest
{
    [TestCase("int", "1'024", "1_024")]
    [TestCase("int", "1'000'001", "1_000_001")]
    [TestCase("float", "1'024", "1_024")]
    [TestCase("float", "1'024.0", "1_024.0")]
    public Task StaticConstExprTest(string type, string nativeValue, string expectedValue) => StaticConstExprTestImpl(type, nativeValue, expectedValue);

    protected abstract Task StaticConstExprTestImpl(string type, string nativeValue, string expectedValue);
}
