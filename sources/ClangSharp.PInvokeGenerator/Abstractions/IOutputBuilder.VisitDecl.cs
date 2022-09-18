using ClangSharp.CSharp;

namespace ClangSharp.Abstractions;

internal partial interface IOutputBuilder
{
    bool IsUncheckedContext { get; }

    void BeginInnerValue();
    void EndInnerValue();

    void BeginInnerCast();
    void WriteCastType(string targetTypeName);
    void EndInnerCast();

    void BeginUnchecked();
    void EndUnchecked();

    void BeginValue(in ValueDesc desc);
    void WriteConstantValue(long value);
    void WriteConstantValue(ulong value);
    void EndValue(in ValueDesc desc);

    void BeginEnum(in EnumDesc desc);
    void EndEnum(in EnumDesc desc);

    void BeginField(in FieldDesc desc);
    void WriteFixedCountField(string typeName, string escapedName, string fixedName, string count);
    void WriteRegularField(string typeName, string escapedName);
    void EndField(in FieldDesc desc);

    void BeginFunctionOrDelegate(in FunctionOrDelegateDesc info, ref bool isMethodClassUnsafe);
    void BeginFunctionInnerPrototype(in FunctionOrDelegateDesc info);
    void BeginParameter(in ParameterDesc info);
    void BeginParameterDefault();
    void EndParameterDefault();
    void EndParameter(in ParameterDesc info);
    void WriteParameterSeparator();
    void EndFunctionInnerPrototype(in FunctionOrDelegateDesc info);
    void BeginConstructorInitializer(string memberRefName, string memberInitName);
    void EndConstructorInitializer();
    void BeginBody(bool isExpressionBody = false);
    void BeginConstructorInitializers();
    void EndConstructorInitializers();
    void BeginInnerFunctionBody();
    void EndInnerFunctionBody();
    void EndBody(bool isExpressionBody = false);
    void EndFunctionOrDelegate(in FunctionOrDelegateDesc info);

    void BeginStruct(in StructDesc info);
    void BeginMarkerInterface(string[]? baseTypeNames);
    void EndMarkerInterface();
    void BeginExplicitVtbl();
    void EndExplicitVtbl();
    void EndStruct(in StructDesc info);

    void EmitCompatibleCodeSupport();
    void EmitFnPtrSupport();
    void EmitSystemSupport();

    CSharpOutputBuilder BeginCSharpCode();
    void EndCSharpCode(CSharpOutputBuilder output);

    void BeginGetter(bool aggressivelyInlined);
    void EndGetter();
    void BeginSetter(bool aggressivelyInlined);
    void EndSetter();

    void BeginIndexer(AccessSpecifier accessSpecifier, bool isUnsafe, bool needsUnscopedRef);
    void WriteIndexer(string typeName);
    void BeginIndexerParameters();
    void EndIndexerParameters();
    void EndIndexer();

    void BeginDereference();
    void EndDereference();
}
