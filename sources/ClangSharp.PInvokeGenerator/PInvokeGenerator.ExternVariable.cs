// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.CSharp;

namespace ClangSharp;

public partial class PInvokeGenerator
{
    // Attempts to bind a top-level `extern`/`extern const` global variable that has no initializer. The
    // generator otherwise drops these (there is nothing to constant-fold), so surfacing them requires the
    // consumer to resolve the export at runtime. Rather than have the generator own a `NativeLibrary`
    // handle (which cannot be released safely -- e.g. under a collectible `AssemblyLoadContext`), this
    // mirrors `--with-manual-import`: each global is emitted as a settable pointer field on a synthesized
    // `<Class>ManualImports` struct that the consumer populates however it resolves the rest of its
    // manual imports. The naming makes it explicit the pointers are unresolved until the caller wires
    // them up.
    //
    // This is strictly opt-in via `generate-extern-variables` because it emits API surface and only ever
    // supports pointer/primitive shapes -- a struct-by-value global would need real marshalling that isn't
    // modeled here.
    //
    // Returns true when the variable was emitted; false leaves the caller's existing no-op behavior intact.
    private bool TryVisitExternVariable(VarDecl varDecl, string nativeName)
    {
        if (!_config.GenerateExternVariables)
        {
            return false;
        }

        // Only the C# backend knows how to emit the manual-import struct.
        if (_config.OutputMode != PInvokeGeneratorOutputMode.CSharp)
        {
            return false;
        }

        // Synthetic macro-definition records are handled separately and never represent a real export.
        if (nativeName.StartsWith("ClangSharpMacro_", StringComparison.Ordinal))
        {
            return false;
        }

        var type = varDecl.Type;

        // Only pointer and primitive/enum globals are supported. Anything else (record-by-value, arrays,
        // references) would require marshalling that a raw pointer field can't model, so it is left
        // untouched.
        if (type.CanonicalType is not (BuiltinType or EnumType or PointerType))
        {
            AddDiagnostic(DiagnosticLevel.Warning, $"Extern variable '{nativeName}' has an unsupported type '{type.AsString}' for 'generate-extern-variables' and was skipped. Only pointer and primitive types are supported.", varDecl);
            return false;
        }

        var accessSpecifier = GetAccessSpecifier(varDecl, matchStar: false);
        var name = GetRemappedName(nativeName, varDecl, tryRemapOperatorName: false, out _, skipUsing: true);
        var escapedName = EscapeName(name);
        var typeName = GetRemappedTypeName(varDecl, context: null, type, out var nativeTypeName);

        // The export lives on `<Class>ManualImports`, a struct sibling to the class the symbol would
        // otherwise be routed into. Reusing the method-class wrapper (registered here) gives us the
        // namespace, access specifier, `unsafe`, and `partial struct` header for free.
        var structName = $"{GetClass(name)}ManualImports";
        _ = _topLevelClassNames.Add(structName);
        _ = _topLevelStructNames.Add(structName);

        // The pointer fields make the struct unsafe.
        _topLevelClassIsUnsafe[structName] = true;

        StartUsingOutputBuilder(structName);
        {
            Debug.Assert(_outputBuilder is CSharpOutputBuilder);
            var outputBuilder = (CSharpOutputBuilder)_outputBuilder;

            OutputExternVariable(outputBuilder, accessSpecifier.AsString(), typeName, nativeTypeName, escapedName);
        }
        StopUsingOutputBuilder();

        return true;
    }

    // Emits a single settable pointer field for an extern global on the manual-import struct, e.g.
    // `public int* kMyConstInt;`. The consumer assigns the pointer (typically off their own export
    // resolution); the generator owns no library handle. Const-ness is carried purely by `[NativeTypeName]`
    // since the field is a raw pointer into native storage.
    private static void OutputExternVariable(CSharpOutputBuilder outputBuilder, string accessSpecifier, string typeName, string nativeTypeName, string escapedName)
    {
        if (!string.IsNullOrEmpty(nativeTypeName) && (nativeTypeName != typeName))
        {
            outputBuilder.WriteIndented("[NativeTypeName(\"");
            outputBuilder.Write(EscapeString(nativeTypeName));
            outputBuilder.WriteLine("\")]");
            outputBuilder.WriteIndentation();
        }
        else
        {
            outputBuilder.WriteIndentation();
        }

        outputBuilder.Write(accessSpecifier);
        outputBuilder.Write(' ');
        outputBuilder.Write(typeName);
        outputBuilder.Write("* ");
        outputBuilder.Write(escapedName);
        outputBuilder.WriteLine(';');
        outputBuilder.NeedsNewline = true;
    }
}
