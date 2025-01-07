// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Linq;
using ClangSharp.Interop;
using NUnit.Framework;
using static ClangSharp.Interop.CX_CXXAccessSpecifier;

namespace ClangSharp.UnitTests;

public sealed class DeclTest : TranslationUnitTest
{
    [TestCase("private", CX_CXXPrivate)]
    [TestCase("protected", CX_CXXProtected)]
    [TestCase("public", CX_CXXPublic)]
    public void AccessSpecDeclTest(string accessSpecifier, CX_CXXAccessSpecifier expectedAccessSpecifier)
    {
        var inputContents = $@"struct MyStruct
{{
{accessSpecifier}:
}};
";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var recordDecl = translationUnit.TranslationUnitDecl.Decls.OfType<RecordDecl>().Where((recordDecl) => recordDecl.Name.Equals("MyStruct", StringComparison.Ordinal)).Single();
        var accessSpecDecl = recordDecl.Decls.OfType<AccessSpecDecl>().Single();

        Assert.That(accessSpecDecl.Access, Is.EqualTo(expectedAccessSpecifier));
    }

    [Test]
    public void ClassTemplateDeclTest()
    {
        var inputContents = $@"template<class T>
class MyClass
{{
    T value;
}};
";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var classTemplateDecl = translationUnit.TranslationUnitDecl.Decls.OfType<ClassTemplateDecl>().Single();
        Assert.That(classTemplateDecl.Name, Is.EqualTo("MyClass"));

        var templateParameter = classTemplateDecl.TemplateParameters.Single();
        Assert.That(templateParameter.Name, Is.EqualTo("T"));
    }

    [Test]
    public void ClassTemplatePartialSpecializationDeclTest()
    {
        var inputContents = $@"template<class T, class U>
class MyClass
{{
    T value1;
    U value2;
}};

template<class U>
class MyClass<int, U>
{{
    int value1;
    U value2;
}};
";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var classTemplatePartialSpecializationDecl = translationUnit.TranslationUnitDecl.Decls.OfType<ClassTemplatePartialSpecializationDecl>().Single();
        Assert.That(classTemplatePartialSpecializationDecl.Name, Is.EqualTo("MyClass"));

        var templateParameter = classTemplatePartialSpecializationDecl.TemplateParameters.Single();
        Assert.That(templateParameter.Name, Is.EqualTo("U"));
    }

    [Test]
    [Ignore("TODO: LibClangSharp needs to be recompiled first")]
    public void TemplateParameterPackTest()
    {
        var inputContents = $@"template<class... Types>
class tuple;

tuple<int, long> SomeFunction();
";

        using var translationUnit = CreateTranslationUnit(inputContents);

        var functionDecl = translationUnit.TranslationUnitDecl.Decls.OfType<FunctionDecl>().Single();
        var tupleDecl = functionDecl.ReturnType.AsCXXRecordDecl as ClassTemplateSpecializationDecl;
        Assert.That(tupleDecl, Is.Not.Null);
        Assert.That(tupleDecl!.TemplateArgs.Count, Is.EqualTo(1));

        var packElements = tupleDecl.TemplateArgs[0].PackElements;
        Assert.That(packElements.Count, Is.EqualTo(2));
        Assert.That(packElements[0].AsType.AsString, Is.EqualTo("int"));
        Assert.That(packElements[1].AsType.AsString, Is.EqualTo("long"));
    }
}
