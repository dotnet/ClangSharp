// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Linq;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[Platform("macosx")]
public sealed class ObjectiveCTest : TranslationUnitTest
{
    [Test]
    public void Category_TypeParamList()
    {
        AssertNeedNewClangSharp();

        var inputContents = $@"
@interface MyClass
@end
@interface MyClass (MyCategory)
@end
";

        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var categories = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCCategoryDecl>().ToList ();
        Assert.That (categories.Count, Is.EqualTo (1), "Count");
        foreach (var c in categories) {
            Assert.That(c.TypeParamList.Count, Is.EqualTo (0), "TypeParamList.Count");
        }
    }

    [Test]
    public void Method_IsPropertyAccessor()
    {
        AssertNeedNewClangSharp();

        var inputContents = $@"
@interface MyClass
    @property int P1;
    -(void) instanceMethod;
    +(void) staticMethod;
@end
";

        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList ();
        Assert.That (classes.Count, Is.GreaterThanOrEqualTo (1), $"At least one class");
        var myClass = classes.SingleOrDefault (v => v.Name == "MyClass")!;
        Assert.That (myClass, Is.Not.Null, "MyClass");

        var methodP1 = myClass.Methods.SingleOrDefault (v => v.Name == "P1")!;
        Assert.That (methodP1, Is.Not.Null, "methodP1");
        Assert.That (methodP1.IsPropertyAccessor, Is.True, "methodP1.IsPropertyAccessor");

        var methodSetP1 = myClass.Methods.SingleOrDefault (v => v.Name == "setP1:")!;
        Assert.That (methodSetP1, Is.Not.Null, "methodSetP1");
        Assert.That (methodSetP1.IsPropertyAccessor, Is.True, "methodSetP1.IsPropertyAccessor");

        var methodInstanceMethod = myClass.Methods.SingleOrDefault (v => v.Name == "instanceMethod")!;
        Assert.That (methodInstanceMethod, Is.Not.Null, "methodInstanceMethod");
        Assert.That (methodInstanceMethod.IsPropertyAccessor, Is.False, "methodInstanceMethod.IsPropertyAccessor");

        var methodStaticMethod = myClass.Methods.SingleOrDefault (v => v.Name == "staticMethod")!;
        Assert.That (methodStaticMethod, Is.Not.Null, "methodStaticMethod");
        Assert.That (methodStaticMethod.IsPropertyAccessor, Is.False, "methodStaticMethod.IsPropertyAccessor");
    }

    private static void AssertNeedNewClangSharp ()
    {
        var forceRun = !string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("FORCE_RUN"));
        if (forceRun) {
            return;
        }
        Assert.Ignore ("TODO: this needs a new version of libClangSharp published.");
    }
}
