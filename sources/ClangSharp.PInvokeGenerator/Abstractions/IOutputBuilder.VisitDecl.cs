using System;
using System.Collections.Generic;
using ClangSharp.CSharp;

namespace ClangSharp.Abstractions
{
    internal partial interface IOutputBuilder
    {
        void BeginInnerValue();
        void EndInnerValue();

        void BeginInnerCast();
        void WriteCastType(string targetTypeName);
        void EndInnerCast();

        void BeginUnchecked();
        void EndUnchecked();

        void BeginConstant(in ConstantDesc desc);
        void BeginConstantValue(bool isGetOnlyProperty = false);
        void WriteConstantValue(long value);
        void WriteConstantValue(ulong value);
        void EndConstantValue();
        void EndConstant(bool isConstant);

        void BeginEnum(AccessSpecifier accessSpecifier, string typeName, string escapedName, string nativeTypeName);
        void EndEnum();

        void BeginField(in FieldDesc desc);
        void WriteFixedCountField(string typeName, string escapedName, string fixedName, string count);
        void WriteRegularField(string typeName, string escapedName);
        void EndField(bool isBodyless = true);

        void BeginFunctionOrDelegate<TCustomAttrGeneratorData>(in FunctionOrDelegateDesc<TCustomAttrGeneratorData> info,
            ref bool isMethodClassUnsafe);
        void WriteReturnType(string typeString);
        void BeginFunctionInnerPrototype(string escapedName);
        void BeginParameter<TCustomAttrGeneratorData>(in ParameterDesc<TCustomAttrGeneratorData> info);
        void BeginParameterDefault();
        void EndParameterDefault();
        void EndParameter();
        void WriteParameterSeparator();
        void EndFunctionInnerPrototype();
        void BeginConstructorInitializer(string memberRefName, string memberInitName);
        void EndConstructorInitializer();
        void BeginBody(bool isExpressionBody = false);
        void BeginConstructorInitializers();
        void EndConstructorInitializers();
        void BeginInnerFunctionBody();
        void EndInnerFunctionBody();
        void EndBody(bool isExpressionBody = false);
        void EndFunctionOrDelegate(bool isVirtual, bool isBodyless);

        void BeginStruct<TCustomAttrGeneratorData>(in StructDesc<TCustomAttrGeneratorData> info);
        void BeginExplicitVtbl();
        void EndExplicitVtbl();
        void EndStruct();

        void EmitCompatibleCodeSupport();
        void EmitFnPtrSupport();
        void EmitSystemSupport();

        CSharpOutputBuilder BeginCSharpCode();
        void EndCSharpCode(CSharpOutputBuilder output);

        void BeginGetter(bool aggressivelyInlined);
        void EndGetter();
        void BeginSetter(bool aggressivelyInlined);
        void EndSetter();

        void BeginIndexer(AccessSpecifier accessSpecifier, bool isUnsafe);
        void WriteIndexer(string typeName);
        void BeginIndexerParameters();
        void EndIndexerParameters();
        void EndIndexer();

        void BeginDereference();
        void EndDereference();
    }
}
