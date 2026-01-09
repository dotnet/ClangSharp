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
    public void Attribute_ObjCRuntimeName()
    {
        AssertNeedNewClangSharp();

        var inputContents = $$"""
__attribute__((objc_runtime_name("MyRenamedClass")))
@interface MyClass
@end

__attribute__((objc_runtime_name("MyRenamedProtocol")))
@protocol MyProtocol
@end
""";
        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();

        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");
        var myClassAttrs = myClass.Attrs;
        Assert.That(myClassAttrs.Count, Is.EqualTo(1), "myClassAttrs.Count");
        var runtimeNameAttr = myClassAttrs[0];
        Assert.That(runtimeNameAttr.Kind, Is.EqualTo(CX_AttrKind.CX_AttrKind_ObjCRuntimeName), "myClass Attr Kind");
        Assert.That(runtimeNameAttr.ObjCRuntimeNameMetadataName, Is.EqualTo("MyRenamedClass"), "myClass Attr ObjCRuntimeNameMetadataName");

        var protocols = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCProtocolDecl>().ToList();
        var myProtocol = protocols.SingleOrDefault(v => v.Name == "MyProtocol")!;
        Assert.That(myProtocol, Is.Not.Null, "MyProtocol");
        var myProtocolAttrs = myProtocol.Attrs;
        Assert.That(myProtocolAttrs.Count, Is.EqualTo(1), "myProtocolAttrs.Count");
        runtimeNameAttr = myProtocolAttrs[0];
        Assert.That(runtimeNameAttr.Kind, Is.EqualTo(CX_AttrKind.CX_AttrKind_ObjCRuntimeName), "myProtocol Attr Kind");
        Assert.That(runtimeNameAttr.ObjCRuntimeNameMetadataName, Is.EqualTo("MyRenamedProtocol"), "myProtocol Attr ObjCRuntimeNameMetadataName");
    }

    [Test]
    public void Type_IsObjCInstanceType()
    {
        AssertNeedNewClangSharp();

        var inputContents = $@"
@interface MyClass
    -(instancetype) instanceMethod;
    +(MyClass*) staticMethod;
@end
";

        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();
        Assert.That(classes.Count, Is.GreaterThanOrEqualTo(1), $"At least one class");
        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");

        var methodInstanceMethod = myClass.Methods.SingleOrDefault(v => v.Name == "instanceMethod")!;
        Assert.That(methodInstanceMethod, Is.Not.Null, "methodInstanceMethod");
        Assert.That(methodInstanceMethod.ReturnType.IsObjCInstanceType, Is.True, "methodInstanceMethod.ReturnType.IsObjCInstanceType");

        var methodStaticMethod = myClass.Methods.SingleOrDefault(v => v.Name == "staticMethod")!;
        Assert.That(methodStaticMethod, Is.Not.Null, "methodStaticMethod");
        Assert.That(methodStaticMethod.ReturnType.IsObjCInstanceType, Is.False, "methodStaticMethod.ReturnType.IsObjCInstanceType");
    }

    [Test]
    public void Method_Selector()
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
    public void ClassWithProtocols()
    {
        AssertNeedNewClangSharp();

        var inputContents = $@"
@protocol P1
@end
@protocol P2
@end

@interface MyClass <P1, P2>
@end
";

        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();
        Assert.That(classes.Count, Is.GreaterThanOrEqualTo(1), $"At least one class");
        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");
        var protocols = myClass.Protocols.ToList();
        Assert.That(protocols.Count, Is.EqualTo(2), "protocols.Count");
        Assert.That(protocols[0].Name, Is.EqualTo("P1"), "protocols[0].Name");
        Assert.That(protocols[1].Name, Is.EqualTo("P2"), "protocols[1].Name");
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
    public void TypeParams()
    {
        AssertNeedNewClangSharp();

        var inputContents = $$"""
@interface NSObject
@end

@interface MyClass<A, B> : NSObject
@end

@interface MyClass<T, Y> (MyCategory)
@end

""";
        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();

        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");
        Assert.That(myClass.TypeParamList, Is.Not.Null, "myClass TypeParamList");
        var myClassTypeParams = myClass.TypeParamList.ToList();
        Assert.That(myClassTypeParams.Count, Is.EqualTo(2), "myClassTypeParams.Count");
        Assert.That(myClassTypeParams[0].Name, Is.EqualTo("A"), "myClassTypeParams[0].Name");
        Assert.That(myClassTypeParams[1].Name, Is.EqualTo("B"), "myClassTypeParams[1].Name");

        var categories = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCCategoryDecl>().ToList();
        var myCategory = categories.SingleOrDefault(v => v.Name == "MyCategory")!;
        Assert.That(myCategory, Is.Not.Null, "MyCategory");
        Assert.That(myCategory.TypeParamList, Is.Not.Null, "myCategory TypeParamList");
        var myCategoryTypeParams = myCategory.TypeParamList.ToList();
        Assert.That(myCategoryTypeParams.Count, Is.EqualTo(2), "myCategoryTypeParams.Count");
        Assert.That(myCategoryTypeParams[0].Name, Is.EqualTo("T"), "myCategoryTypeParams[0].Name");
        Assert.That(myCategoryTypeParams[1].Name, Is.EqualTo("Y"), "myCategoryTypeParams[1].Name");
    }

    [Test]
    public void PointerTypes()
    {
        AssertNeedNewClangSharp();

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
        AssertNeedNewClangSharp();

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
    public void Attribute_AvailabilityAttributes()
    {
        AssertNeedNewClangSharp();

        var inputContents = $$"""
__attribute__((availability(ios,unavailable,message="Use another class")))
__attribute__((availability(tvos,introduced=10.0)))
__attribute__((availability(macosx,introduced=10.15.3)))
__attribute__((availability(maccatalyst,introduced=14.3.0)))
@interface MyClass
    @property int P1
        __attribute__((availability(ios,unavailable,message="Not this property")))
        __attribute__((availability(tvos,obsoleted=11.0,message="Obsoleted on tvOS")))
        __attribute__((availability(macosx,obsoleted=11.0,message="Obsoleted on macOS")))
        __attribute__((availability(maccatalyst,obsoleted=15.0,message="Obsoleted on Mac Catalyst")))
        ;

    -(void) instanceMethod
        __attribute__((availability(ios,unavailable,message="Not this instance method")))
        __attribute__((availability(tvos,deprecated=12.0,message="Deprecated on tvOS")))
        __attribute__((availability(macosx,deprecated=12.0,message="Deprecated on macOS")))
        __attribute__((availability(maccatalyst,deprecated=16.0,message="Deprecated on Mac Catalyst")))
        ;

    +(void) staticMethod __attribute__((unavailable("elsewhere")))
        __attribute__((availability(ios,unavailable,message="Not this static method")))
        __attribute__((availability(tvos,introduced=10.0,deprecated=11.0,obsoleted=12.0,message="Gone on tvOS")))
        __attribute__((availability(macosx,introduced=10.0.1,deprecated=11.0.2,obsoleted=12.0.3,message="Gone on macOS")))
        __attribute__((availability(maccatalyst,introduced=10.0.0,deprecated=11.0.0,obsoleted=12.0.0,message="Gone on Mac Catalyst")))
        ;
@end
""";
        using var translationUnit = CreateTranslationUnit(inputContents, "objective-c++");

        var classes = translationUnit.TranslationUnitDecl.Decls.OfType<ObjCInterfaceDecl>().ToList();
        Assert.That(classes.Count, Is.GreaterThanOrEqualTo(1), $"At least one class");
        var myClass = classes.SingleOrDefault(v => v.Name == "MyClass")!;
        Assert.That(myClass, Is.Not.Null, "MyClass");

        var assertVersionTuple = new Action<VersionTuple?, VersionTuple?, string>((VersionTuple? actual, VersionTuple? expected, string info) => {
            if (expected is null)
            {
                if (actual is not null)
                {
                    Assert.Fail($"Expected null VersionTuple, got: {actual} -- {info}");
                }
                return;
            }
            else
            {
                if (actual is null)
                {
                    Assert.Fail($"Expected non-null VersionTuple ({expected.Value}), got null -- {info}");
                    return;
                }
            }

            Assert.That(actual.ToString(), Is.EqualTo(expected.ToString()), info);
        });

        var assertAttribute = new Action<Attr, string?, string?, VersionTuple?, VersionTuple?, VersionTuple?, bool, string>((Attr attrib, string? message, string? platform, VersionTuple? introduced, VersionTuple? deprecated, VersionTuple? obsoleted, bool unavailable, string info) => {
            Assert.That(attrib, Is.Not.Null, $"attrib: {info}");

            info += $" (platform: {attrib.AvailabilityAttributePlatformIdentifierName})";
            Assert.That(attrib.AvailabilityAttributeMessage, Is.EqualTo(message), $"Message: {info}");
            Assert.That(attrib.AvailabilityAttributePlatformIdentifierName, Is.EqualTo(platform), $"PlatformIdentifierName: {info}");
            assertVersionTuple(attrib.AvailabilityAttributeIntroduced, introduced, $"Introduced: {info}");
            assertVersionTuple(attrib.AvailabilityAttributeDeprecated, deprecated, $"Deprecated: {info}");
            assertVersionTuple(attrib.AvailabilityAttributeObsoleted, obsoleted, $"Obsoleted: {info}");
            Assert.That(attrib.AvailabilityAttributeUnavailable, Is.EqualTo(unavailable), $"Unavailable: {info}");
        });

        Assert.Multiple(() => {
            var myClassAttrs = myClass.Attrs;
            Assert.That(myClassAttrs.Count, Is.EqualTo(4), "myClassAttrs.Count");
            assertAttribute(myClassAttrs[0], "Use another class", "ios", new VersionTuple(0), new VersionTuple(0), new VersionTuple(0), true, "myClass Attr - iOS");
            assertAttribute(myClassAttrs[1], "", "tvos", new VersionTuple(10, 0), new VersionTuple(0), new VersionTuple(0), false, "myClass Attr - tvOS");
            assertAttribute(myClassAttrs[2], "", "macos", new VersionTuple(10, 15, 3), new VersionTuple(0), new VersionTuple(0), false, "myClass Attr - macOS");
            assertAttribute(myClassAttrs[3], "", "maccatalyst", new VersionTuple(14, 3, 0), new VersionTuple(0), new VersionTuple(0), false, "myClass Attr - Mac Catalyst");

            var methodP1 = myClass.Methods.SingleOrDefault(v => v.Name == "P1")!;
            Assert.That(methodP1, Is.Not.Null, "methodP1");
            var methodP1Attrs = methodP1.Attrs;
            Assert.That(methodP1Attrs.Count, Is.EqualTo(4), "methodP1Attrs.Count");
            assertAttribute(methodP1Attrs[0], "Not this property", "ios", new VersionTuple(0), new VersionTuple(0), new VersionTuple(0), true, "methodP1 Attr - P1 - iOS");
            assertAttribute(methodP1Attrs[1], "Obsoleted on tvOS", "tvos", new VersionTuple(0), new VersionTuple(0), new VersionTuple(11, 0), false, "methodP1 Attr - tvOS");
            assertAttribute(methodP1Attrs[2], "Obsoleted on macOS", "macos", new VersionTuple(0), new VersionTuple(0), new VersionTuple(11, 0), false, "methodP1 Attr - macOS");
            assertAttribute(methodP1Attrs[3], "Obsoleted on Mac Catalyst", "maccatalyst", new VersionTuple(0), new VersionTuple(0), new VersionTuple(15, 0), false, "methodP1 Attr - Mac Catalyst");

            var methodSetP1 = myClass.Methods.SingleOrDefault(v => v.Name == "setP1:")!;
            Assert.That(methodSetP1, Is.Not.Null, "methodP1");
            var methodSetP1Attrs = methodSetP1.Attrs;
            Assert.That(methodSetP1Attrs.Count, Is.EqualTo(4), "methodSetP1Attrs.Count");
            assertAttribute(methodSetP1Attrs[0], "Not this property", "ios", new VersionTuple(0), new VersionTuple(0), new VersionTuple(0), true, "methodSetP1Attrs Attr - P1 - iOS");
            assertAttribute(methodSetP1Attrs[1], "Obsoleted on tvOS", "tvos", new VersionTuple(0), new VersionTuple(0), new VersionTuple(11, 0), false, "methodSetP1Attrs Attr - tvOS");
            assertAttribute(methodSetP1Attrs[2], "Obsoleted on macOS", "macos", new VersionTuple(0), new VersionTuple(0), new VersionTuple(11, 0), false, "methodSetP1Attrs Attr - macOS");
            assertAttribute(methodSetP1Attrs[3], "Obsoleted on Mac Catalyst", "maccatalyst", new VersionTuple(0), new VersionTuple(0), new VersionTuple(15, 0), false, "methodSetP1Attrs Attr - Mac Catalyst");

            var methodInstanceMethod = myClass.Methods.SingleOrDefault(v => v.Name == "instanceMethod")!;
            Assert.That(methodInstanceMethod, Is.Not.Null, "methodP1");
            var methodInstanceMethodAttrs = methodInstanceMethod.Attrs;
            Assert.That(methodInstanceMethodAttrs.Count, Is.EqualTo(4), "methodInstanceMethodAttrs.Count");
            assertAttribute(methodInstanceMethodAttrs[0], "Not this instance method", "ios", new VersionTuple(0), new VersionTuple(0), new VersionTuple(0), true, "methodInstanceMethodAttrs Attr - P1 - iOS");
            assertAttribute(methodInstanceMethodAttrs[1], "Deprecated on tvOS", "tvos", new VersionTuple(0), new VersionTuple(12, 0), new VersionTuple(0), false, "methodInstanceMethodAttrs Attr - tvOS");
            assertAttribute(methodInstanceMethodAttrs[2], "Deprecated on macOS", "macos", new VersionTuple(0), new VersionTuple(12, 0), new VersionTuple(0), false, "methodInstanceMethodAttrs Attr - macOS");
            assertAttribute(methodInstanceMethodAttrs[3], "Deprecated on Mac Catalyst", "maccatalyst", new VersionTuple(0), new VersionTuple(16, 0), new VersionTuple(0), false, "methodInstanceMethodAttrs Attr - Mac Catalyst");

            var methodStaticMethod = myClass.Methods.SingleOrDefault(v => v.Name == "staticMethod")!;
            Assert.That(methodStaticMethod, Is.Not.Null, "methodP1");
            var methodStaticMethodAttrs = methodStaticMethod.Attrs;
            Assert.That(methodStaticMethodAttrs.Count, Is.EqualTo(5), "methodStaticMethodAttrs.Count");
            assertAttribute(methodStaticMethodAttrs[0], "elsewhere", "", null, null, null, false, "methodStaticMethodAttrs Attr - P1 - unavailable");
            assertAttribute(methodStaticMethodAttrs[1], "Not this static method", "ios", new VersionTuple(0), new VersionTuple(0), new VersionTuple(0), true, "methodStaticMethodAttrs Attr - P1 - iOS");
            assertAttribute(methodStaticMethodAttrs[2], "Gone on tvOS", "tvos", new VersionTuple(10, 0), new VersionTuple(11, 0), new VersionTuple(12, 0), false, "methodStaticMethodAttrs Attr - tvOS");
            assertAttribute(methodStaticMethodAttrs[3], "Gone on macOS", "macos", new VersionTuple(10, 0, 1), new VersionTuple(11, 0, 2), new VersionTuple(12, 0, 3), false, "methodStaticMethodAttrs Attr - macOS");
            assertAttribute(methodStaticMethodAttrs[4], "Gone on Mac Catalyst", "maccatalyst", new VersionTuple(10, 0, 0), new VersionTuple(11, 0, 0), new VersionTuple(12, 0, 0), false, "methodStaticMethodAttrs Attr - Mac Catalyst");
        });
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
