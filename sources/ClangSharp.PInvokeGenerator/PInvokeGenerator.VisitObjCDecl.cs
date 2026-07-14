// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ClangSharp.Abstractions;
using ClangSharp.Interop;

namespace ClangSharp;

public partial class PInvokeGenerator
{
    private void VisitObjCProtocolDecl(ObjCProtocolDecl objCProtocolDecl)
    {
        // Forward `@protocol Foo;` references carry no members; only the definition is emitted.
        if (!objCProtocolDecl.IsThisDeclarationADefinition)
        {
            return;
        }

        var nativeName = GetCursorName(objCProtocolDecl);
        var name = GetRemappedCursorName(objCProtocolDecl);
        var escapedName = EscapeName(name);

        _generatedObjCBindings = true;

        StartUsingOutputBuilder(name, includeTestOutput: false);
        {
            Debug.Assert(_outputBuilder is not null);

            var desc = new StructDesc {
                AccessSpecifier = GetAccessSpecifier(objCProtocolDecl, matchStar: true),
                EscapedName = escapedName,
                IsUnsafe = true,
                NativeType = $"@protocol {nativeName}",
                Location = objCProtocolDecl.Location,
                IsNested = false,
                WriteCustomAttrs = static context => {
                    (var objCProtocolDecl, var generator) = ((ObjCProtocolDecl, PInvokeGenerator))context;

                    generator.WithAttributes(objCProtocolDecl, emitGeneratedCodeAttribute: true);
                    generator.WithUsings(objCProtocolDecl);
                },
                CustomAttrGeneratorData = (objCProtocolDecl, this),
            };

            _outputBuilder.BeginStruct(in desc);
            {
                // Raw layer: the object's first field (`isa`), a byte-for-byte match of
                // `struct objc_object`. Dispatch itself is done through the Objective-C runtime
                // (`objc_msgSend`) rather than a fixed-offset table, so unlike COM's `lpVtbl` this
                // field is only the identity/class hook, not the callable surface.
                var fieldDesc = new FieldDesc {
                    AccessSpecifier = AccessSpecifier.Public,
                    NativeTypeName = "Class",
                    EscapedName = "isa",
                    ParentName = escapedName,
                    Offset = null,
                    NeedsNewKeyword = false,
                };

                _outputBuilder.BeginField(in fieldDesc);
                _outputBuilder.WriteRegularField("void*", "isa");
                _outputBuilder.EndField(in fieldDesc);

                EmitObjCMessageMembers(objCProtocolDecl, escapedName);
            }
            _outputBuilder.EndStruct(in desc);
        }
        StopUsingOutputBuilder();
    }

    // Emits the raw `Selectors` cache plus the friendly `objc_msgSend` members for the instance
    // methods that fall within the v1 scalar/pointer/void subset. Anything outside that subset is
    // diagnosed rather than emitted with a wrong ABI.
    private void EmitObjCMessageMembers(ObjCProtocolDecl objCProtocolDecl, string escapedParentName)
    {
        Debug.Assert(_outputBuilder is not null);

        var emitted = new List<ObjCMessageMember>();

        foreach (var method in objCProtocolDecl.InstanceMethods)
        {
            // Property accessors are surfaced via their properties in a later phase; class methods,
            // variadics, and by-value aggregate returns/parameters need dispatch machinery that is
            // deliberately out of scope for the first slice.
            if (method.IsPropertyAccessor)
            {
                continue;
            }

            if (!IsInObjCMessageSubset(method, out var reason))
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Objective-C method '{method.Selector}' on '@protocol {GetCursorName(objCProtocolDecl)}' is not supported: {reason}. It was skipped.", method);
                continue;
            }

            var nativeSelector = method.Selector;
            var memberName = EscapeName(nativeSelector.Replace(':', '_'));
            var returnTypeName = IsTypeVoid(method, method.ReturnType) ? "void" : GetRemappedTypeName(method, objCProtocolDecl, method.ReturnType, out _);

            var parameters = new List<(string TypeName, string EscapedName)>();

            foreach (var parmVarDecl in method.Parameters)
            {
                var parmTypeName = GetRemappedTypeName(parmVarDecl, method, parmVarDecl.Type, out _);
                var parmName = EscapeName(GetRemappedCursorName(parmVarDecl));
                parameters.Add((parmTypeName, parmName));
            }

            emitted.Add(new ObjCMessageMember(memberName, nativeSelector, returnTypeName, parameters));
        }

        if (emitted.Count == 0)
        {
            return;
        }

        var output = _outputBuilder.BeginCSharpCode();

        // `Unsafe.AsPointer(ref this)` recovers the object pointer to pass as `self`.
        output.AddUsingDirective("System.Runtime.CompilerServices");

        // Raw layer: the registered selectors, kept public so nothing is hidden and any selector we
        // did not wrap can still be sent by hand via `&ObjectiveC.objc_msgSend`.
        output.WriteNewline();
        output.WriteIndentedLine("public static class Selectors");
        output.WriteBlockStart();
        {
            foreach (var member in emitted)
            {
                output.WriteIndented($"public static readonly SEL {member.MemberName} = ObjectiveC.sel_registerName(\"{member.NativeSelector}\");");
                output.WriteNewline();
            }
        }
        output.WriteBlockEnd();

        // Friendly layer: named members that hide the selector lookup and the `objc_msgSend` cast,
        // dispatched on `this` exactly like the COM vtbl helpers dispatch on `lpVtbl`.
        foreach (var member in emitted)
        {
            output.WriteNewline();

            var prototype = new StringBuilder();
            var fnPtrArgs = new StringBuilder();
            var callArgs = new StringBuilder();

            _ = fnPtrArgs.Append(escapedParentName).Append("*, SEL");
            _ = callArgs.Append('(').Append(escapedParentName).Append("*)Unsafe.AsPointer(ref this), Selectors.").Append(member.MemberName);

            for (var i = 0; i < member.Parameters.Count; i++)
            {
                var (typeName, parmName) = member.Parameters[i];

                if (i != 0)
                {
                    _ = prototype.Append(", ");
                }

                _ = prototype.Append(typeName).Append(' ').Append(parmName);
                _ = fnPtrArgs.Append(", ").Append(typeName);
                _ = callArgs.Append(", ").Append(parmName);
            }

            _ = fnPtrArgs.Append(", ").Append(member.ReturnTypeName);

            output.WriteIndented($"public {member.ReturnTypeName} {member.MemberName}({prototype}) => ");
            output.Write($"((delegate* unmanaged<{fnPtrArgs}>)ObjectiveC.objc_msgSend)({callArgs});");
            output.WriteNewline();
        }

        _outputBuilder.EndCSharpCode(output);
    }

    // v1 message-send subset: no variadics and no by-value aggregate return/parameter types, so the
    // plain `objc_msgSend` entry point is ABI-correct (aggregate returns would need `_stret`).
    private static bool IsInObjCMessageSubset(ObjCMethodDecl method, out string reason)
    {
        if (method.Handle.IsVariadic)
        {
            reason = "variadic methods are not yet supported";
            return false;
        }

        if (IsAggregateType(method.ReturnType))
        {
            reason = "by-value aggregate return types are not yet supported (would require objc_msgSend_stret)";
            return false;
        }

        foreach (var parmVarDecl in method.Parameters)
        {
            if (IsAggregateType(parmVarDecl.Type))
            {
                reason = "by-value aggregate parameter types are not yet supported";
                return false;
            }
        }

        reason = "";
        return true;
    }

    private static bool IsAggregateType(Type type)
    {
        return type.CanonicalType.Kind is CXTypeKind.CXType_Record;
    }

    private readonly struct ObjCMessageMember(string memberName, string nativeSelector, string returnTypeName, List<(string TypeName, string EscapedName)> parameters)
    {
        public string MemberName { get; } = memberName;
        public string NativeSelector { get; } = nativeSelector;
        public string ReturnTypeName { get; } = returnTypeName;
        public List<(string TypeName, string EscapedName)> Parameters { get; } = parameters;
    }
}
