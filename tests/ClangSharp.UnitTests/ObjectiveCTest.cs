// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Linq;
using ClangSharp.Interop;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

[Platform("macosx")]
public sealed class ObjectiveCTest : TranslationUnitTest
{
    [Test]
    public void Method_Selector()
    {
        var inputContents = $@"
@interface MyClass
    @property int P1;
    -(void) instanceMethod;
    +(void) staticMethod;
@end
";

        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();
        Assert.That(classes.Count, Is.GreaterThanOrEqualTo(1), $"At least one class");
        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");

        var methodP1 = myClass.Methods.SingleOrDefault(v => v.Name == "P1")!;
        Assert.That(methodP1, Is.Not.Null, "methodP1");
        Assert.That(methodP1.Selector, Is.EqualTo("P1"), "methodP1.Selector");

        var methodSetP1 = myClass.Methods.SingleOrDefault(v => v.Name == "setP1:")!;
        Assert.That(methodSetP1, Is.Not.Null, "methodSetP1");
        Assert.That(methodSetP1.Selector, Is.EqualTo("setP1:"), "methodSetP1.Selector");

        var methodInstanceMethod = myClass.Methods.SingleOrDefault(v => v.Name == "instanceMethod")!;
        Assert.That(methodInstanceMethod, Is.Not.Null, "methodInstanceMethod");
        Assert.That(methodInstanceMethod.Selector, Is.EqualTo("instanceMethod"), "methodInstanceMethod.Selector");

        var methodStaticMethod = myClass.Methods.SingleOrDefault(v => v.Name == "staticMethod")!;
        Assert.That(methodStaticMethod, Is.Not.Null, "methodStaticMethod");
        Assert.That(methodStaticMethod.Selector, Is.EqualTo("staticMethod"), "methodStaticMethod.Selector");
    }

    [Test]
    public void Category_TypeParamList()
    {
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
        var inputContents = $@"
@interface MyClass
    @property int P1;
    -(void) instanceMethod;
    +(void) staticMethod;
@end
";

        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();
        Assert.That(classes.Count, Is.GreaterThanOrEqualTo(1), $"At least one class");
        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");

        var methodP1 = myClass.Methods.SingleOrDefault(v => v.Name == "P1")!;
        Assert.That(methodP1, Is.Not.Null, "methodP1");
        Assert.That(methodP1.IsPropertyAccessor, Is.True, "methodP1.IsPropertyAccessor");

        var methodSetP1 = myClass.Methods.SingleOrDefault(v => v.Name == "setP1:")!;
        Assert.That(methodSetP1, Is.Not.Null, "methodSetP1");
        Assert.That(methodSetP1.IsPropertyAccessor, Is.True, "methodSetP1.IsPropertyAccessor");

        var methodInstanceMethod = myClass.Methods.SingleOrDefault(v => v.Name == "instanceMethod")!;
        Assert.That(methodInstanceMethod, Is.Not.Null, "methodInstanceMethod");
        Assert.That(methodInstanceMethod.IsPropertyAccessor, Is.False, "methodInstanceMethod.IsPropertyAccessor");

        var methodStaticMethod = myClass.Methods.SingleOrDefault(v => v.Name == "staticMethod")!;
        Assert.That(methodStaticMethod, Is.Not.Null, "methodStaticMethod");
        Assert.That(methodStaticMethod.IsPropertyAccessor, Is.False, "methodStaticMethod.IsPropertyAccessor");
    }

    [Test]
    public void PointerTypes()
    {
        var inputContents = """
@interface MyClass
-(void)instanceMethod:(MyClass **)ptrToPtrToMyClass;
@end
""";
        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();

        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");

        var instanceMethod = myClass.Methods.SingleOrDefault(v => v.Name == "instanceMethod:")!;
        Assert.That(instanceMethod, Is.Not.Null, "instanceMethod");
        var parameters = instanceMethod.Parameters.ToList();
        Assert.That(parameters.Count, Is.EqualTo(1), "parameters.Count");
        var parameter = parameters[0];
        Assert.That(parameter.Name, Is.EqualTo("ptrToPtrToMyClass"), "parameter.Name");
        var parameterType = parameter.Type;
        Assert.That(parameterType.Kind, Is.EqualTo(CXTypeKind.CXType_Pointer), "parameterType.Kind");
        var pointeeType = parameterType.PointeeType;
        Assert.That(pointeeType.Kind, Is.EqualTo(CXTypeKind.CXType_ObjCObjectPointer), "pointeeType.Kind");
        var pointee2Type = pointeeType.PointeeType;
        Assert.That(pointee2Type.Kind, Is.EqualTo(CXTypeKind.CXType_ObjCInterface), "pointee2Type.Kind");
    }

    [Test]
    public void BlockTypes()
    {
        var inputContents = $$"""
@interface MyClass
-(MyClass *(^)(id))instanceMethod1;
-(MyClass *(^)(SEL))instanceMethod2;
-(MyClass *(^)(Class))instanceMethod3;
@end
""";
        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();

        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");

        var info = new[] {
            (Name: "instanceMethod1", Type: CXTypeKind.CXType_ObjCId),
            (Name: "instanceMethod2", Type: CXTypeKind.CXType_ObjCSel),
            (Name: "instanceMethod3", Type: CXTypeKind.CXType_ObjCClass),
        };

        foreach (var i in info)
        {
            var instanceMethod = myClass.Methods.SingleOrDefault(v => v.Name == i.Name)!;
            Assert.That(instanceMethod, Is.Not.Null, "instanceMethod");
            var returnType = instanceMethod.ReturnType;
            Assert.That(returnType.Kind, Is.EqualTo(CXTypeKind.CXType_BlockPointer), "returnType.Kind");
            var pointeeType = returnType.PointeeType;
            Assert.That(pointeeType.Kind, Is.EqualTo(CXTypeKind.CXType_FunctionProto), "pointeeType.Kind");
            var functionProtoType = (FunctionProtoType)pointeeType;
            Assert.That(functionProtoType.ParamTypes.Count, Is.EqualTo(1), "functionProtoType.ParamTypes.Count()");
            var paramType = functionProtoType.ParamTypes[0];
            Assert.That(paramType.Kind, Is.EqualTo(i.Type), "paramType.Kind");
            var elaboratedParamType = (ElaboratedType)paramType;
            Assert.That(elaboratedParamType, Is.Not.Null, "elaboratedParamType");
            Assert.That(elaboratedParamType.Desugar, Is.Not.Null, "elaboratedParamType.Desugar");
        }
    }

    [Test]
    public void Attribute_PrettyPrint()
    {
        AssertNeedNewClangSharp();

        var inputContents = $@"
__attribute__((availability(ios,introduced=10.0)))
@interface MyClass
    @property int P1 __attribute__((availability(ios,introduced=11.0)));
    -(void) instanceMethod __attribute__((availability(ios,introduced=12.0)));
    +(void) staticMethod __attribute__((availability(ios,introduced=13.0)));
@end
";

        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();
        Assert.That(classes.Count, Is.GreaterThanOrEqualTo(1), $"At least one class");
        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");
        var myClassAttrs = myClass.Attrs;
        Assert.That(myClassAttrs.Count, Is.EqualTo(1), "myClassAttrs.Count");
        Assert.That(myClassAttrs[0].PrettyPrint(), Is.EqualTo("__attribute__((availability(ios, introduced=10.0)))"), "myClass.Attr.PrettyPrint");

        var methodP1 = myClass.Methods.SingleOrDefault(v => v.Name == "P1")!;
        Assert.That(methodP1, Is.Not.Null, "methodP1");
        var methodP1Attrs = methodP1.Attrs;
        Assert.That(methodP1Attrs.Count, Is.EqualTo(1), "methodP1Attrs.Count");
        Assert.That(methodP1Attrs[0].PrettyPrint(), Is.EqualTo("__attribute__((availability(ios, introduced=11.0)))"), "methodP1.Attr.PrettyPrint");

        var methodSetP1 = myClass.Methods.SingleOrDefault(v => v.Name == "setP1:")!;
        Assert.That(methodSetP1, Is.Not.Null, "methodSetP1");
        var methodSetP1Attrs = methodSetP1.Attrs;
        Assert.That(methodSetP1Attrs.Count, Is.EqualTo(1), "methodSetP1Attrs.Count");
        Assert.That(methodSetP1Attrs[0].PrettyPrint(), Is.EqualTo("__attribute__((availability(ios, introduced=11.0)))"), "methodSetP1.Attr.PrettyPrint");

        var methodInstanceMethod = myClass.Methods.SingleOrDefault(v => v.Name == "instanceMethod")!;
        Assert.That(methodInstanceMethod, Is.Not.Null, "methodInstanceMethod");
        var methodInstanceMethodAttrs = methodInstanceMethod.Attrs;
        Assert.That(methodInstanceMethodAttrs.Count, Is.EqualTo(1), "methodInstanceMethodAttrs.Count");
        Assert.That(methodInstanceMethodAttrs[0].PrettyPrint(), Is.EqualTo("__attribute__((availability(ios, introduced=12.0)))"), "methodInstanceMethod.Attr.PrettyPrint");

        var methodStaticMethod = myClass.Methods.SingleOrDefault(v => v.Name == "staticMethod")!;
        Assert.That(methodStaticMethod, Is.Not.Null, "methodStaticMethod");
        var methodStaticMethodAttrs = methodStaticMethod.Attrs;
        Assert.That(methodStaticMethodAttrs.Count, Is.EqualTo(1), "methodStaticMethodAttrs.Count");
        Assert.That(methodStaticMethodAttrs[0].PrettyPrint(), Is.EqualTo("__attribute__((availability(ios, introduced=13.0)))"), "methodStaticMethod.Attr.PrettyPrint");
    }
}
