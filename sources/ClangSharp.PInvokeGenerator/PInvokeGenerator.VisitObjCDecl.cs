// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ClangSharp.Abstractions;
using ClangSharp.CSharp;
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

                EmitObjCMessageMembers(objCProtocolDecl, escapedName, supportsClassMembers: false);
            }
            _outputBuilder.EndStruct(in desc);
        }
        StopUsingOutputBuilder();
    }

    private void VisitObjCInterfaceDecl(ObjCInterfaceDecl objCInterfaceDecl)
    {
        // Forward `@class Foo;` references carry no members; only the definition is emitted.
        if (!objCInterfaceDecl.IsThisDeclarationADefinition)
        {
            return;
        }

        var nativeName = GetCursorName(objCInterfaceDecl);
        var name = GetRemappedCursorName(objCInterfaceDecl);
        var escapedName = EscapeName(name);

        var superClass = objCInterfaceDecl.SuperClass;

        _generatedObjCBindings = true;

        // Record the full native declaration (superclass + adopted protocols) so the relationship is
        // not lost. The generated structs are all `{ void* isa }` with identical layout, so an
        // `{escapedName}*` can simply be reinterpreted as a superclass pointer to send inherited
        // messages -- inherited members are intentionally not re-emitted (unlike a COM vtbl, dispatch
        // is dynamic, so flattening would only bloat the output).
        var nativeType = new StringBuilder($"@interface {nativeName}");

        if (superClass is not null)
        {
            _ = nativeType.Append(" : ").Append(GetCursorName(superClass));
        }

        if (objCInterfaceDecl.Protocols.Count != 0)
        {
            _ = nativeType.Append(" <");

            for (var i = 0; i < objCInterfaceDecl.Protocols.Count; i++)
            {
                if (i != 0)
                {
                    _ = nativeType.Append(", ");
                }

                _ = nativeType.Append(GetCursorName(objCInterfaceDecl.Protocols[i]));
            }

            _ = nativeType.Append('>');
        }

        StartUsingOutputBuilder(name, includeTestOutput: false);
        {
            Debug.Assert(_outputBuilder is not null);

            var desc = new StructDesc {
                AccessSpecifier = GetAccessSpecifier(objCInterfaceDecl, matchStar: true),
                EscapedName = escapedName,
                IsUnsafe = true,
                NativeType = nativeType.ToString(),
                NativeInheritance = (_config.GenerateNativeInheritanceAttribute && superClass is not null) ? GetRemappedCursorName(superClass) : null,
                Location = objCInterfaceDecl.Location,
                IsNested = false,
                WriteCustomAttrs = static context => {
                    (var objCInterfaceDecl, var generator) = ((ObjCInterfaceDecl, PInvokeGenerator))context;

                    generator.WithAttributes(objCInterfaceDecl, emitGeneratedCodeAttribute: true);
                    generator.WithUsings(objCInterfaceDecl);
                },
                CustomAttrGeneratorData = (objCInterfaceDecl, this),
            };

            _outputBuilder.BeginStruct(in desc);
            {
                // Raw layer: the object's first field (`isa`), a byte-for-byte match of
                // `struct objc_object` and of every other generated Objective-C struct, which is what
                // makes the pointer reinterpret to a superclass valid.
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

                // The backing class object, used as the receiver for class (`+`) members and exposed
                // for hand-rolled sends. Resolved once by name via the runtime.
                var classOutput = _outputBuilder.BeginCSharpCode();
                classOutput.WriteNewline();
                classOutput.WriteIndented($"public static readonly void* Class = ObjectiveC.objc_getClass(\"{nativeName}\");");
                classOutput.WriteNewline();
                _outputBuilder.EndCSharpCode(classOutput);

                EmitObjCMessageMembers(objCInterfaceDecl, escapedName, supportsClassMembers: true);
            }
            _outputBuilder.EndStruct(in desc);
        }
        StopUsingOutputBuilder();
    }

    private void VisitObjCCategoryDecl(ObjCCategoryDecl objCCategoryDecl)
    {
        var classInterface = objCCategoryDecl.ClassInterface;

        // A category with no resolvable class (e.g. the base `@interface` was not parsed) has nowhere
        // to attach its members, so there is no faithful mapping to emit.
        if (classInterface is null)
        {
            AddDiagnostic(DiagnosticLevel.Warning, $"Objective-C category '{GetCursorName(objCCategoryDecl)}' has no resolvable class interface and was skipped.", objCCategoryDecl);
            return;
        }

        var categoryName = GetCursorName(objCCategoryDecl);
        var name = GetRemappedCursorName(classInterface);
        var escapedName = EscapeName(name);

        _generatedObjCBindings = true;

        // A category extends an existing class, so it reopens that class's `partial struct` and adds
        // only the new message members. The `isa` field and backing `Class` object already belong to
        // the `@interface`'s emission, so they are not repeated here.
        StartUsingOutputBuilder(name, includeTestOutput: false);
        {
            Debug.Assert(_outputBuilder is not null);

            var desc = new StructDesc {
                AccessSpecifier = GetAccessSpecifier(classInterface, matchStar: true),
                EscapedName = escapedName,
                IsUnsafe = true,
                NativeType = $"@interface {GetCursorName(classInterface)} ({categoryName})",
                Location = objCCategoryDecl.Location,
                IsNested = false,
                WriteCustomAttrs = static context => {
                    (var objCCategoryDecl, var generator) = ((ObjCCategoryDecl, PInvokeGenerator))context;

                    generator.WithAttributes(objCCategoryDecl, emitGeneratedCodeAttribute: true);
                    generator.WithUsings(objCCategoryDecl);
                },
                CustomAttrGeneratorData = (objCCategoryDecl, this),
            };

            _outputBuilder.BeginStruct(in desc);
            {
                EmitObjCMessageMembers(objCCategoryDecl, escapedName, supportsClassMembers: true, hasPrecedingMembers: false);
            }
            _outputBuilder.EndStruct(in desc);
        }
        StopUsingOutputBuilder();
    }


    // properties) that fall within the supported subset. Anything outside that subset is diagnosed
    // rather than emitted with a wrong ABI.
    //
    // Instance members dispatch on `this`; class members dispatch on the backing class object and
    // are only emitted when the container has one (i.e. an `@interface`, via `supportsClassMembers`).
    // A `@protocol` has no backing class, so its class-method/-property requirements are diagnosed
    // and skipped rather than wired to a receiver that does not exist.
    private void EmitObjCMessageMembers(ObjCContainerDecl container, string escapedParentName, bool supportsClassMembers, bool hasPrecedingMembers = true)
    {
        Debug.Assert(_outputBuilder is not null);

        var containerDesc = container switch {
            ObjCProtocolDecl => $"@protocol {GetCursorName(container)}",
            ObjCCategoryDecl category => $"@interface {GetCursorName(category.ClassInterface)} ({GetCursorName(category)})",
            _ => $"@interface {GetCursorName(container)}",
        };

        var selectors = new List<(string Field, string NativeSelector)>();
        var seenSelectors = new HashSet<string>(StringComparer.Ordinal);
        var methods = new List<ObjCMessageMember>();
        var properties = new List<ObjCPropertyMember>();

        void AddSelector(string field, string nativeSelector)
        {
            if (seenSelectors.Add(field))
            {
                selectors.Add((field, nativeSelector));
            }
        }

        void ProcessMethod(ObjCMethodDecl method, bool isClassMember)
        {
            // Property accessors are surfaced through their `@property` below.
            if (method.IsPropertyAccessor)
            {
                return;
            }

            // Variadics can't be expressed as a fixed unmanaged function-pointer signature, so they
            // are the one remaining out-of-subset shape and are diagnosed rather than mis-emitted.
            if (!IsInObjCMessageSubset(method, out var reason))
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Objective-C method '{method.Selector}' on '{containerDesc}' is not supported: {reason}. It was skipped.", method);
                return;
            }

            var nativeSelector = method.Selector;
            var memberName = EscapeName(nativeSelector.Replace(':', '_'));
            var returnTypeName = IsTypeVoid(method, method.ReturnType) ? "void" : GetRemappedTypeName(method, container, method.ReturnType, out _);

            var involvesAggregate = IsAggregateType(method.ReturnType);
            var parameters = new List<(string TypeName, string EscapedName)>();

            foreach (var parmVarDecl in method.Parameters)
            {
                var parmTypeName = GetRemappedTypeName(parmVarDecl, method, parmVarDecl.Type, out _);
                var parmName = EscapeName(GetRemappedCursorName(parmVarDecl));
                parameters.Add((parmTypeName, parmName));

                involvesAggregate |= IsAggregateType(parmVarDecl.Type);
            }

            AddSelector(memberName, nativeSelector);
            methods.Add(new ObjCMessageMember(memberName, returnTypeName, parameters, isClassMember, method.Handle.IsObjCOptional, involvesAggregate));
        }

        void ProcessProperty(ObjCPropertyDecl property, bool isClassMember)
        {
            var propertyName = EscapeName(GetRemappedCursorName(property));
            var typeName = GetRemappedTypeName(property, container, property.Type, out _);
            var involvesAggregate = IsAggregateType(property.Type);

            var getterSelector = property.GetterMethodDecl.Selector;
            var getterField = EscapeName(getterSelector.Replace(':', '_'));
            AddSelector(getterField, getterSelector);

            var isReadOnly = (property.PropertyAttributes & CXObjCPropertyAttrKind.CXObjCPropertyAttr_readonly) != 0;
            string? setterField = null;

            if (!isReadOnly)
            {
                var setterSelector = property.SetterMethodDecl.Selector;
                setterField = EscapeName(setterSelector.Replace(':', '_'));
                AddSelector(setterField, setterSelector);
            }

            properties.Add(new ObjCPropertyMember(propertyName, typeName, getterField, setterField, isClassMember, property.Handle.IsObjCOptional, involvesAggregate));
        }

        foreach (var method in container.InstanceMethods)
        {
            ProcessMethod(method, isClassMember: false);
        }

        foreach (var property in container.InstanceProperties)
        {
            ProcessProperty(property, isClassMember: false);
        }

        if (supportsClassMembers)
        {
            foreach (var method in container.ClassMethods)
            {
                ProcessMethod(method, isClassMember: true);
            }

            foreach (var property in container.ClassProperties)
            {
                ProcessProperty(property, isClassMember: true);
            }
        }
        else
        {
            foreach (var method in container.ClassMethods)
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Objective-C class method '{method.Selector}' on '{containerDesc}' is not supported: a protocol has no backing class object to dispatch on. It was skipped.", method);
            }

            foreach (var property in container.ClassProperties)
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Objective-C class property '{GetCursorName(property)}' on '{containerDesc}' is not supported: a protocol has no backing class object to dispatch on. It was skipped.", property);
            }
        }

        if (selectors.Count == 0)
        {
            return;
        }

        var output = _outputBuilder.BeginCSharpCode();

        // `Unsafe.AsPointer(ref this)` recovers the object pointer to pass as `self`.
        output.AddUsingDirective("System.Runtime.CompilerServices");

        // Raw layer: the registered selectors, kept public so nothing is hidden and any selector we
        // did not wrap can still be sent by hand via `&ObjectiveC.objc_msgSend`. The leading blank line
        // separates the cache from the preceding fields; a category reopens an otherwise-empty struct
        // body, so it is suppressed there.
        if (hasPrecedingMembers)
        {
            output.WriteNewline();
        }

        output.WriteIndentedLine("public static partial class Selectors");
        output.WriteBlockStart();
        {
            foreach (var (field, nativeSelector) in selectors)
            {
                output.WriteIndented($"public static readonly SEL {field} = ObjectiveC.sel_registerName(\"{nativeSelector}\");");
                output.WriteNewline();
            }
        }
        output.WriteBlockEnd();

        // Friendly layer: named members that hide the selector lookup and the `objc_msgSend` cast.
        // Instance members dispatch on `this` (like COM vtbl helpers dispatch on `lpVtbl`); class
        // members are `static` and dispatch on the backing class object.
        foreach (var member in methods)
        {
            output.WriteNewline();
            WriteObjCOptionalNote(output, member.IsOptional);
            WriteObjCAggregateNote(output, member.InvolvesAggregate);
            var prototype = new StringBuilder();

            for (var i = 0; i < member.Parameters.Count; i++)
            {
                var (typeName, parmName) = member.Parameters[i];

                if (i != 0)
                {
                    _ = prototype.Append(", ");
                }

                _ = prototype.Append(typeName).Append(' ').Append(parmName);
            }

            var msgSend = BuildObjCMsgSend(member.IsClassMember, escapedParentName, member.MemberName, member.ReturnTypeName, member.Parameters);
            var staticModifier = member.IsClassMember ? "static " : "";

            output.WriteIndented($"public {staticModifier}{member.ReturnTypeName} {member.MemberName}({prototype}) => {msgSend};");
            output.WriteNewline();
        }

        foreach (var property in properties)
        {
            output.WriteNewline();
            WriteObjCOptionalNote(output, property.IsOptional);
            WriteObjCAggregateNote(output, property.InvolvesAggregate);

            var getter = BuildObjCMsgSend(property.IsClassMember, escapedParentName, property.GetterField, property.TypeName, []);
            var staticModifier = property.IsClassMember ? "static " : "";

            if (property.SetterField is null)
            {
                output.WriteIndented($"public {staticModifier}{property.TypeName} {property.PropertyName} => {getter};");
                output.WriteNewline();
            }
            else
            {
                var setter = BuildObjCMsgSend(property.IsClassMember, escapedParentName, property.SetterField, "void", [(property.TypeName, "value")]);

                output.WriteIndentedLine($"public {staticModifier}{property.TypeName} {property.PropertyName}");
                output.WriteBlockStart();
                {
                    output.WriteIndented($"get => {getter};");
                    output.WriteNewline();
                    output.WriteIndented($"set => {setter};");
                    output.WriteNewline();
                }
                output.WriteBlockEnd();
            }
        }

        _outputBuilder.EndCSharpCode(output);
    }

    // Optional protocol members may be unimplemented by a conforming object, so sending the message
    // blindly can trap; flag them so callers know to guard with `respondsToSelector:`.
    private static void WriteObjCOptionalNote(CSharpOutputBuilder output, bool isOptional)
    {
        if (isOptional)
        {
            output.WriteIndentedLine("// @optional; guard with respondsToSelector: before sending.");
        }
    }

    // A by-value struct in the signature is dispatched through the plain `objc_msgSend` on the
    // assumption of the arm64 (Apple Silicon) ABI, where struct returns/arguments go through the
    // normal entry point. The legacy x86-64 ABI would need `objc_msgSend_stret`/`_fpret` for some of
    // these, which is intentionally unsupported now that Apple has dropped x86-64 macOS.
    private static void WriteObjCAggregateNote(CSharpOutputBuilder output, bool involvesAggregate)
    {
        if (involvesAggregate)
        {
            output.WriteIndentedLine("// By-value struct in signature: assumes the arm64 objc_msgSend ABI (x86-64 objc_msgSend_stret is unsupported).");
        }
    }

    // Builds the friendly-member body: a per-call-site cast of the cached `objc_msgSend` pointer to
    // the selector's signature. Instance members dispatch on `this`; class members dispatch on the
    // backing class object exposed as the `Class` field.
    private static string BuildObjCMsgSend(bool isClassMember, string escapedParentName, string selectorField, string returnTypeName, IReadOnlyList<(string TypeName, string ArgExpr)> args)
    {
        var receiverType = isClassMember ? "void*" : $"{escapedParentName}*";
        var receiverExpr = isClassMember ? "Class" : $"({escapedParentName}*)Unsafe.AsPointer(ref this)";

        var fnPtrArgs = new StringBuilder();
        var callArgs = new StringBuilder();

        _ = fnPtrArgs.Append(receiverType).Append(", SEL");
        _ = callArgs.Append(receiverExpr).Append(", Selectors.").Append(selectorField);

        foreach (var (typeName, argExpr) in args)
        {
            _ = fnPtrArgs.Append(", ").Append(typeName);
            _ = callArgs.Append(", ").Append(argExpr);
        }

        _ = fnPtrArgs.Append(", ").Append(returnTypeName);

        return $"((delegate* unmanaged<{fnPtrArgs}>)ObjectiveC.objc_msgSend)({callArgs})";
    }

    // Message-send subset: only variadics are excluded, since a C-style `...` cannot be expressed as
    // a fixed unmanaged function-pointer signature. By-value aggregates are supported via the plain
    // `objc_msgSend` under the arm64 ABI (see `WriteObjCAggregateNote`).
    private static bool IsInObjCMessageSubset(ObjCMethodDecl method, out string reason)
    {
        if (method.Handle.IsVariadic)
        {
            reason = "variadic methods cannot be expressed as a fixed function-pointer signature";
            return false;
        }

        reason = "";
        return true;
    }

    private static bool IsAggregateType(Type type)
    {
        return type.CanonicalType.Kind is CXTypeKind.CXType_Record;
    }

    private readonly struct ObjCMessageMember(string memberName, string returnTypeName, List<(string TypeName, string EscapedName)> parameters, bool isClassMember, bool isOptional, bool involvesAggregate)
    {
        public string MemberName { get; } = memberName;
        public string ReturnTypeName { get; } = returnTypeName;
        public List<(string TypeName, string EscapedName)> Parameters { get; } = parameters;
        public bool IsClassMember { get; } = isClassMember;
        public bool IsOptional { get; } = isOptional;
        public bool InvolvesAggregate { get; } = involvesAggregate;
    }

    private readonly struct ObjCPropertyMember(string propertyName, string typeName, string getterField, string? setterField, bool isClassMember, bool isOptional, bool involvesAggregate)
    {
        public string PropertyName { get; } = propertyName;
        public string TypeName { get; } = typeName;
        public string GetterField { get; } = getterField;
        public string? SetterField { get; } = setterField;
        public bool IsClassMember { get; } = isClassMember;
        public bool IsOptional { get; } = isOptional;
        public bool InvolvesAggregate { get; } = involvesAggregate;
    }
}
