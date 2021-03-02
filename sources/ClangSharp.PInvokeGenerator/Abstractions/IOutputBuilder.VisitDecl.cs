using System;
using System.Collections.Generic;
using ClangSharp.CSharp;

namespace ClangSharp.Abstractions
{
    public partial interface IOutputBuilder
    {
        void BeginInnerValue();
        void EndInnerValue();

        void BeginInnerCast();
        void WriteCastType(string targetTypeName);
        void EndInnerCast();

        void BeginUnchecked();
        void EndUnchecked();

        void BeginConstant(string accessSpecifier, string typeName, string escapedName, bool isAnonymousEnum);
        void BeginConstantValue();
        void WriteConstantValue(long value);
        void WriteConstantValue(ulong value);
        void EndConstantValue();
        void EndConstant(bool isAnonymousEnum);

        void BeginEnum(string accessSpecifier, string typeName, string escapedName);
        void EndEnum();

        void BeginField(string accessSpecifier, string nativeTypeName, string escapedName, int? offset,
            bool needsNewKeyword, string inheritedFrom = null);
        void WriteFixedCountField(string typeName, string escapedName, string fixedName, string count);
        void WriteRegularField(string typeName, string escapedName);
        void EndField();

        void BeginFunctionOrDelegate<TCustomAttrGeneratorData>(in FunctionOrDelegateDesc<TCustomAttrGeneratorData> info);
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
        void BeginFunctionBody();
        void BeginConstructorInitializers();
        void EndConstructorInitializers();
        void BeginInnerFunctionBody();
        void EndInnerFunctionBody();
        void EndFunctionBody();
        void EndFunctionOrDelegate(bool isVirtual, bool isBodyless);

        void BeginStruct<TCustomAttrGeneratorData>(in StructDesc<TCustomAttrGeneratorData> info);
        void BeginExplicitVtbl();
        void EndExplicitVtbl();
        void EndStruct();

        void EmitCompatibleCodeSupport();
        void EmitFnPtrSupport();

        CSharpOutputBuilder BeginCSharpCode();
        void EndCSharpCode(CSharpOutputBuilder output);
    }
}
