// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ClangSharp.Abstractions;
using ClangSharp.CSharp;
using static ClangSharp.Interop.CX_CastKind;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CX_StorageClass;
using static ClangSharp.Interop.CX_StringKind;
using static ClangSharp.Interop.CX_UnaryExprOrTypeTrait;
using static ClangSharp.Interop.CXUnaryOperatorKind;
using static ClangSharp.Interop.CXEvalResultKind;
using static ClangSharp.Interop.CXTypeKind;

namespace ClangSharp;

public partial class PInvokeGenerator
{
    private void VisitRecordDecl(RecordDecl recordDecl)
    {
        if (recordDecl.IsInjectedClassName)
        {
            // We shouldn't process injected records
            return;
        }

        var nativeName = GetCursorName(recordDecl);
        var name = GetRemappedCursorName(recordDecl);
        var escapedName = EscapeName(name);

        StartUsingOutputBuilder(name, includeTestOutput: true);
        {
            var cxxRecordDecl = recordDecl as CXXRecordDecl;
            _cxxRecordDeclContext = cxxRecordDecl;

            var hasVtbl = false;
            var hasBaseVtbl = false;

            if (cxxRecordDecl is not null)
            {
                hasVtbl = HasVtbl(cxxRecordDecl, out hasBaseVtbl);

                if (HasVirtualBase(cxxRecordDecl))
                {
                    AddDiagnostic(DiagnosticLevel.Warning, "Virtual inheritance is not currently modeled. The generated layout and vtable dispatch for this type may be incorrect.", cxxRecordDecl);
                }
            }

            var alignment = Math.Max(recordDecl.TypeForDecl.Handle.AlignOf, 1);
            var maxAlignm = recordDecl.Fields.Any() ? recordDecl.Fields.Max((fieldDecl) => Math.Max(fieldDecl.Type.Handle.AlignOf, 1)) : alignment;

            var isTopLevelStruct = _config.WithTypes.TryGetValue(StripNameEscape(name), out var withType) && withType.Equals("struct", StringComparison.Ordinal);
            var generateTestsClass = !recordDecl.IsAnonymousStructOrUnion && recordDecl.DeclContext is not RecordDecl;
            var testOutputStarted = false;

            var nullableUuid = (Guid?)null;
            var uuidName = "";

            if (TryGetUuid(recordDecl, out var uuid))
            {
                nullableUuid = uuid;

                uuidName = _uuidNameOverrides.TryGetValue(nativeName, out var overrideName)
                    ? overrideName
                    : GetRemappedName($"IID_{nativeName}", recordDecl, tryRemapOperatorName: false, out _, skipUsing: true);

                _uuidsToGenerate.Add(uuidName, uuid);

                if ((_testOutputBuilder is not null) && (uuid != Guid.Empty))
                {
                    StartTestOutput(ref testOutputStarted, generateTestsClass, isTopLevelStruct);

                    var className = GetClass(uuidName);

                    _testOutputBuilder.AddUsingDirective("System");

                    if (_config.DontUseUsingStaticsForGuidMember)
                    {
                        _testOutputBuilder.AddUsingDirective($"{GetNamespace(className)}");
                    }
                    else
                    {
                        _testOutputBuilder.AddUsingDirective($"static {GetNamespace(className)}.{className}");
                    }

                    _testOutputBuilder.WriteIndented("/// <summary>Validates that the <see cref=\"Guid\" /> of the <see cref=\"");
                    _testOutputBuilder.Write(escapedName);
                    _testOutputBuilder.WriteLine("\" /> struct is correct.</summary>");

                    WithTestAttribute();

                    _testOutputBuilder.WriteIndentedLine("public static void GuidOfTest()");
                    _testOutputBuilder.WriteBlockStart();

                    if (_config.GenerateTestsNUnit)
                    {
                        _testOutputBuilder.WriteIndented("Assert.That");
                    }
                    else if (_config.GenerateTestsXUnit)
                    {
                        _testOutputBuilder.WriteIndented("Assert.Equal");
                    }

                    _testOutputBuilder.Write("(typeof(");
                    _testOutputBuilder.Write(escapedName);
                    _testOutputBuilder.Write(").GUID, ");

                    if (_config.GenerateTestsNUnit)
                    {
                        _testOutputBuilder.Write("Is.EqualTo(");
                    }

                    var usableUuidName = uuidName;
                    if (_config.DontUseUsingStaticsForGuidMember)
                    {
                        usableUuidName = $"{className}.{usableUuidName}";
                    }

                    _testOutputBuilder.Write(usableUuidName);

                    if (_config.GenerateTestsNUnit)
                    {
                        _testOutputBuilder.Write(')');
                    }

                    _testOutputBuilder.Write(')');
                    _testOutputBuilder.WriteSemicolon();
                    _testOutputBuilder.WriteNewline();
                    _testOutputBuilder.WriteBlockEnd();
                    _testOutputBuilder.NeedsNewline = true;
                }
            }

            var hasGuidMember = _config.GenerateGuidMember && !string.IsNullOrWhiteSpace(uuidName);

            var equalityFields = GetEqualityFields(recordDecl, cxxRecordDecl);

            var layoutKind = recordDecl.IsUnion
                ? LayoutKind.Explicit
                : LayoutKind.Sequential;

            long alignment32 = -1;
            long alignment64 = -1;

            GetTypeSize(recordDecl, recordDecl.TypeForDecl, ref alignment32, ref alignment64, out var size32, out var size64);

            string[]? baseTypeNames = null;

            if (!TryGetRemappedValue(recordDecl, _config._withBases, optOuts: null, out var extraBaseTypeNames, matchStar: true))
            {
                extraBaseTypeNames = null;
            }

            string? nativeNameWithExtras = null, nativeInheritance = null;
            if ((cxxRecordDecl is not null) && cxxRecordDecl.Bases.Any())
            {
                var nativeTypeNameBuilder = new StringBuilder();
                var baseTypeNamesBuilder = new List<string>();

                _ = nativeTypeNameBuilder.Append(recordDecl.IsUnion ? "union " : "struct ");
                _ = nativeTypeNameBuilder.Append(nativeName);
                _ = nativeTypeNameBuilder.Append(" : ");

                var seenPrimaryVtblBase = false;

                for (var i = 0; i < cxxRecordDecl.Bases.Count; i++)
                {
                    if (i != 0)
                    {
                        _ = nativeTypeNameBuilder.Append(", ");
                    }

                    var cxxBaseSpecifier = cxxRecordDecl.Bases[i];
                    var baseName = GetRemappedCursorName(cxxBaseSpecifier.Referenced, out var nativeBaseName, skipUsing: !_config.GenerateMarkerInterfaces);
                    _ = nativeTypeNameBuilder.Append(nativeBaseName);

                    // Only the primary polymorphic base is flattened into this type, so only its marker
                    // interface is inherited. Other polymorphic bases are exposed via subobject fields and
                    // must not be listed here or the type would be required to reimplement their methods.
                    var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);
                    var isPolymorphic = HasVtbl(baseCxxRecordDecl, out var baseHasBaseVtbl) || baseHasBaseVtbl;

                    if (isPolymorphic && !seenPrimaryVtblBase)
                    {
                        baseTypeNamesBuilder.Add(baseName);
                        seenPrimaryVtblBase = true;
                    }
                }

                nativeNameWithExtras = nativeTypeNameBuilder.ToString();
                nativeInheritance = GetCursorName(cxxRecordDecl.Bases[cxxRecordDecl.Bases.Count - 1].Referenced);
                baseTypeNames = [.. baseTypeNamesBuilder];
            }

            if (!TryGetRemappedValue(recordDecl, _config._withPackings, optOuts: null, out var pack))
            {
                pack = alignment < maxAlignm ? alignment.ToString(CultureInfo.InvariantCulture) : null;
            }

            var requestedAlignment = (int)(recordDecl.MaxAlignment / 8);
            int? nativeAlignment = null;

            if (requestedAlignment > maxAlignm)
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Struct '{name}' requests {requestedAlignment} byte alignment which .NET cannot honor; over-alignment can only lower, never raise, alignment, so the runtime will align it to {maxAlignm} bytes.", recordDecl);

                if (_config.GenerateNativeAlignmentAttribute)
                {
                    nativeAlignment = requestedAlignment;
                    pack ??= requestedAlignment.ToString(CultureInfo.InvariantCulture);
                }
            }

            var desc = new StructDesc {
                AccessSpecifier = GetAccessSpecifier(recordDecl, matchStar: true),
                EscapedName = escapedName,
                IsUnsafe = IsUnsafe(recordDecl) || hasGuidMember,
                HasVtbl = hasVtbl || hasBaseVtbl,
                IsUnion = recordDecl.IsUnion,
                HasEquality = equalityFields is not null,
                Layout = new() {
                    Alignment32 = alignment32,
                    Alignment64 = alignment64,
                    Size32 = size32,
                    Size64 = size64,
                    Pack = pack,
                    MaxFieldAlignment = maxAlignm,
                    Kind = layoutKind
                },
                Uuid = nullableUuid,
                NativeType = nativeNameWithExtras,
                ExtraBaseTypeNames = ((hasVtbl || hasBaseVtbl) && _config.GenerateMarkerInterfaces) ? null : extraBaseTypeNames,
                NativeInheritance = _config.GenerateNativeInheritanceAttribute ? nativeInheritance : null,
                NativeAlignment = nativeAlignment,
                Location = recordDecl.Location,
                IsNested = recordDecl.DeclContext is TagDecl,
                WriteCustomAttrs = static context => {
                    (var recordDecl, var generator) = ((RecordDecl, PInvokeGenerator))context;

                    generator.WithAttributes(recordDecl, emitGeneratedCodeAttribute: true);
                    generator.WithUsings(recordDecl);
                },
                CustomAttrGeneratorData = (recordDecl, this),
            };
            Debug.Assert(_outputBuilder is not null);

            if (!isTopLevelStruct)
            {
                _outputBuilder.BeginStruct(in desc);
            }
            else
            {
                if (!_topLevelClassAttributes.TryGetValue(name, out var withAttributes))
                {
                    withAttributes = [];
                }

                if (!_topLevelClassUsings.TryGetValue(name, out var withUsings))
                {
                    withUsings = new HashSet<string>(StringComparer.Ordinal);
                }

                if (desc.LayoutAttribute is not null)
                {
                    withAttributes.Add($"StructLayout(LayoutKind.{desc.LayoutAttribute.Value}{((desc.Layout.Pack is not null) ? $", Pack = {desc.Layout.Pack}" : "")})");
                    _ = withUsings.Add("System.Runtime.InteropServices");
                }

                if (desc.Uuid.HasValue)
                {
                    withAttributes.Add($"Guid(\"{nullableUuid.GetValueOrDefault().ToString("D", CultureInfo.InvariantCulture).ToUpperInvariant()}\")");
                    _ = withUsings.Add("System.Runtime.InteropServices");
                }

                var nativeTypeName = desc.NativeType;

                if (nativeTypeName is not null)
                {
                    foreach (var entry in _config.NativeTypeNamesToStrip)
                    {
                        nativeTypeName = nativeTypeName.Replace(entry, "", StringComparison.Ordinal);
                    }

                    if (!string.IsNullOrWhiteSpace(nativeTypeName))
                    {
                        withAttributes.Add($"NativeTypeName(\"{EscapeString(nativeTypeName)}\")");
                        _ = withUsings.Add(GetNamespace("NativeTypeNameAttribute"));
                    }
                }

                if (_config.GenerateNativeInheritanceAttribute && (desc.NativeInheritance is not null))
                {
                    withAttributes.Add($"NativeInheritance(\"{desc.NativeInheritance}\")");
                    _ = withUsings.Add(GetNamespace("NativeInheritanceAttribute"));
                }

                if (_config.GenerateNativeAlignmentAttribute && (desc.NativeAlignment is not null))
                {
                    withAttributes.Add($"NativeAlignment({desc.NativeAlignment.GetValueOrDefault().ToString(CultureInfo.InvariantCulture)})");
                    _ = withUsings.Add(GetNamespace("NativeAlignmentAttribute"));
                }

                if (_config.GenerateSourceLocationAttribute && (desc.Location is not null))
                {
                    desc.Location.Value.GetFileLocation(out var file, out var line, out var column, out _);
                    withAttributes.Add($"SourceLocation(\"{EscapeString(file.Name.ToString())}\", {line}, {column})");
                    _ = withUsings.Add(GetNamespace("SourceLocationAttribute"));
                }

                if (withAttributes.Count != 0)
                {
                    _topLevelClassAttributes[name] = withAttributes;
                }

                if (withUsings.Count != 0)
                {
                    _topLevelClassUsings[name] = withUsings;
                }

                if (desc.IsUnsafe)
                {
                    _topLevelClassIsUnsafe[name] = true;
                }

                if (hasGuidMember)
                {
                    _topLevelClassHasGuidMember[name] = true;
                }
            }

            if (hasGuidMember)
            {
                var valueDesc = new ValueDesc {
                    AccessSpecifier = AccessSpecifier.None,
                    TypeName = "Guid*",
                    EscapedName = "INativeGuid.NativeGuid",
                    ParentName = name,
                    Kind = ValueKind.GuidMember,
                    Flags = ValueFlags.Initializer,
                };

                var uuidClassName = GetClass(uuidName);

                _outputBuilder.EmitUsingDirective("System");
                _outputBuilder.EmitUsingDirective("System.Runtime.CompilerServices");

                if (_config.DontUseUsingStaticsForGuidMember)
                {
                    _outputBuilder.EmitUsingDirective($"{GetNamespace(uuidClassName)}");
                }
                else
                {
                    _outputBuilder.EmitUsingDirective($"static {GetNamespace(uuidClassName)}.{uuidClassName}");
                }

                var usableUuidName = uuidName;
                if (_config.DontUseUsingStaticsForGuidMember)
                {
                    usableUuidName = $"{uuidClassName}.{usableUuidName}";
                }

                _outputBuilder.BeginValue(in valueDesc);

                var code = _outputBuilder.BeginCSharpCode();

                code.Write("(Guid*)Unsafe.AsPointer(");

                if (!_config.GenerateLatestCode)
                {
                    code.Write("ref Unsafe.AsRef(");
                }

                code.Write("in ");
                code.Write(usableUuidName);
                code.Write(')');

                if (!_config.GenerateLatestCode)
                {
                    code.Write(')');
                }

                _outputBuilder.EndCSharpCode(code);

                _outputBuilder.EndValue(in valueDesc);
            }

            if ((hasVtbl || hasBaseVtbl) && ((cxxRecordDecl is null) || (GetVtblPtrAccessPrefix(cxxRecordDecl).Length == 0)))
            {
                var fieldDesc = new FieldDesc {
                    AccessSpecifier = AccessSpecifier.Public,
                    NativeTypeName = null,
                    EscapedName = "lpVtbl",
                    Offset = null,
                    NeedsNewKeyword = false,
                };
                _outputBuilder.BeginField(in fieldDesc);

                if (_config.GenerateExplicitVtbls)
                {
                    if (_config.GenerateMarkerInterfaces && !_config.GenerateCompatibleCode)
                    {
                        _outputBuilder.WriteRegularField($"Vtbl<{nativeName}>*", "lpVtbl");
                    }
                    else
                    {
                        _outputBuilder.WriteRegularField("Vtbl*", "lpVtbl");
                    }
                }
                else
                {
                    _outputBuilder.WriteRegularField("void**", "lpVtbl");
                }

                _outputBuilder.EndField(in fieldDesc);
            }

            if (cxxRecordDecl is not null)
            {
                foreach (var (cxxBaseSpecifier, _) in EnumerateBaseSubobjects(cxxRecordDecl))
                {
                    var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);

                    if (!IsBaseExcluded(cxxRecordDecl, baseCxxRecordDecl, cxxBaseSpecifier, out var baseFieldName))
                    {
                        var parent = GetRemappedCursorName(baseCxxRecordDecl);

                        var fieldDesc = new FieldDesc {
                            AccessSpecifier = GetAccessSpecifier(baseCxxRecordDecl, matchStar: true),
                            NativeTypeName = null,
                            EscapedName = baseFieldName,
                            Offset = null,
                            NeedsNewKeyword = false,
                            InheritedFrom = parent,
                            Location = cxxBaseSpecifier.Location,
                        };

                        _outputBuilder.BeginField(in fieldDesc);
                        _outputBuilder.WriteRegularField(parent, baseFieldName);
                        _outputBuilder.EndField(in fieldDesc);
                    }
                }
            }

            if ((_testOutputBuilder is not null) && generateTestsClass && !_config.GenerateDisableRuntimeMarshalling)
            {
                StartTestOutput(ref testOutputStarted, generateTestsClass, isTopLevelStruct);

                _testOutputBuilder.WriteIndented("/// <summary>Validates that the <see cref=\"");
                _testOutputBuilder.Write(escapedName);
                _testOutputBuilder.WriteLine("\" /> struct is blittable.</summary>");

                WithTestAttribute();

                _testOutputBuilder.WriteIndentedLine("public static void IsBlittableTest()");
                _testOutputBuilder.WriteBlockStart();

                WithTestAssertEqual($"sizeof({escapedName})", $"Marshal.SizeOf<{escapedName}>()");

                _testOutputBuilder.WriteBlockEnd();
                _testOutputBuilder.NeedsNewline = true;

                _testOutputBuilder.WriteIndented("/// <summary>Validates that the <see cref=\"");
                _testOutputBuilder.Write(escapedName);
                _testOutputBuilder.WriteLine("\" /> struct has the right <see cref=\"LayoutKind\" />.</summary>");

                WithTestAttribute();

                _testOutputBuilder.WriteIndented("public static void IsLayout");

                if (recordDecl.IsUnion)
                {
                    _testOutputBuilder.Write("Explicit");
                }
                else
                {
                    _testOutputBuilder.Write("Sequential");
                }

                _testOutputBuilder.WriteLine("Test()");
                _testOutputBuilder.WriteBlockStart();

                WithTestAssertTrue($"typeof({escapedName}).Is{(recordDecl.IsUnion ? "ExplicitLayout" : "LayoutSequential")}");

                _testOutputBuilder.WriteBlockEnd();
                _testOutputBuilder.NeedsNewline = true;

                if ((size32 == 0 || size64 == 0) && !TryGetUuid(recordDecl, out _))
                {
                    AddDiagnostic(DiagnosticLevel.Info, $"{escapedName} has a size of 0", recordDecl);
                }

                _testOutputBuilder.WriteIndented("/// <summary>Validates that the <see cref=\"");
                _testOutputBuilder.Write(escapedName);
                _testOutputBuilder.WriteLine("\" /> struct has the correct size.</summary>");

                WithTestAttribute();

                _testOutputBuilder.WriteIndentedLine("public static void SizeOfTest()");
                _testOutputBuilder.WriteBlockStart();

                if (size32 != size64)
                {
                    _testOutputBuilder.AddUsingDirective("System");

                    _testOutputBuilder.WriteIndentedLine("if (Environment.Is64BitProcess)");
                    _testOutputBuilder.WriteBlockStart();

                    WithTestAssertEqual($"{Math.Max(size64, 1)}", $"sizeof({escapedName})");

                    _testOutputBuilder.WriteBlockEnd();
                    _testOutputBuilder.WriteIndentedLine("else");
                    _testOutputBuilder.WriteBlockStart();
                }

                WithTestAssertEqual($"{Math.Max(size32, 1)}", $"sizeof({escapedName})");

                if (size32 != size64)
                {
                    _testOutputBuilder.WriteBlockEnd();
                }

                _testOutputBuilder.WriteBlockEnd();
            }

            var bitfieldDescs = GetBitfieldDescs(recordDecl);
            var bitfieldIndex = (bitfieldDescs.Length == 1) ? -1 : 0;

            var bitfieldPreviousSize = 0L;
            var bitfieldRemainingBits = 0L;

            foreach (var fieldDecl in recordDecl.Fields)
            {
                if (fieldDecl.IsBitField)
                {
                    if (fieldDecl.IsZeroLengthBitField)
                    {
                        bitfieldPreviousSize = 0;
                        bitfieldRemainingBits = 0;
                    }
                    else
                    {
                        VisitBitfieldDecl(fieldDecl, bitfieldDescs, recordDecl, contextName: "", ref bitfieldIndex, ref bitfieldPreviousSize, ref bitfieldRemainingBits);
                    }
                }
                else
                {
                    bitfieldPreviousSize = 0;
                    bitfieldRemainingBits = 0;
                }

                Visit(fieldDecl);
                _outputBuilder.WriteDivider();
            }

            Visit(recordDecl.IndirectFields);

            if (cxxRecordDecl is not null)
            {
                foreach (var cxxConstructorDecl in cxxRecordDecl.Ctors)
                {
                    if (!IsExcluded(cxxConstructorDecl))
                    {
                        Visit(cxxConstructorDecl);
                        _outputBuilder.WriteDivider();
                    }
                }

                if (cxxRecordDecl.HasUserDeclaredDestructor)
                {
                    var cxxDestructorDecl = cxxRecordDecl.Destructor;

                    if (cxxDestructorDecl == null)
                    {
                        AddDiagnostic(DiagnosticLevel.Warning, "Record has user declared destructor, but Destructor property was null. Generated bindings may be incomplete.", cxxRecordDecl);
                    }
                    else if (!cxxDestructorDecl.IsVirtual && !IsExcluded(cxxDestructorDecl))
                    {
                        Visit(cxxDestructorDecl);
                        _outputBuilder.WriteDivider();
                    }
                }

                if (hasVtbl || hasBaseVtbl)
                {
                    OutputDelegateSignatures(cxxRecordDecl, cxxRecordDecl, new HashSet<string>(StringComparer.Ordinal));
                }
            }

            var excludedCursors = recordDecl.Fields.AsEnumerable<Cursor>().Concat(recordDecl.IndirectFields);

            if (cxxRecordDecl is not null)
            {
                OutputMethods(cxxRecordDecl, cxxRecordDecl);
                excludedCursors = excludedCursors.Concat(cxxRecordDecl.Methods);
            }

            Visit(recordDecl.Decls, excludedCursors);

            foreach (var array in recordDecl.Fields.Where(IsTypeConstantOrIncompleteArray))
            {
                VisitConstantOrIncompleteArrayFieldDecl(recordDecl, array);
            }

            if (hasVtbl || hasBaseVtbl)
            {
                Debug.Assert(cxxRecordDecl is not null);

                if (!_config.GenerateCompatibleCode)
                {
                    _outputBuilder.EmitCompatibleCodeSupport();
                }

                if (_config.ExcludeFnptrCodegen)
                {
                    _outputBuilder.EmitFnPtrSupport();
                }

                OutputVtblHelperMethods(cxxRecordDecl, cxxRecordDecl, new HashSet<string>(StringComparer.Ordinal));

                if (_config.GenerateMarkerInterfaces)
                {
                    if (_outputBuilder is CSharpOutputBuilder csharpOutputBuilder)
                    {
                        csharpOutputBuilder.NeedsNewline = true;
                    }

                    _outputBuilder.BeginMarkerInterface(baseTypeNames, extraBaseTypeNames);
                    OutputMarkerInterfaces(cxxRecordDecl, cxxRecordDecl);
                    _outputBuilder.EndMarkerInterface();
                }

                if (_config.GenerateExplicitVtbls || _config.GenerateTrimmableVtbls)
                {
                    if (_outputBuilder is CSharpOutputBuilder csharpOutputBuilder)
                    {
                        csharpOutputBuilder.NeedsNewline = true;
                    }

                    _outputBuilder.BeginExplicitVtbl();
                    OutputVtblEntries(cxxRecordDecl, cxxRecordDecl, new HashSet<string>(StringComparer.Ordinal));
                    _outputBuilder.EndExplicitVtbl();
                }
            }

            if ((equalityFields is not null) && (_outputBuilder is CSharpOutputBuilder csharpEqualityBuilder))
            {
                // A nested type emitted just above (e.g. a promoted anonymous record) leaves the builder
                // without a pending newline, so force the blank-line separator before the members.
                csharpEqualityBuilder.NeedsNewline = true;
                OutputEqualityMethods(csharpEqualityBuilder, escapedName, equalityFields);
            }

            if (!isTopLevelStruct)
            {
                _outputBuilder.EndStruct(in desc);

                if ((_testOutputBuilder is not null) && generateTestsClass && testOutputStarted)
                {
                    _testOutputBuilder.WriteBlockEnd();
                }
            }

            _cxxRecordDeclContext = null;
        }
        StopUsingOutputBuilder();

        string FixupNameForMultipleHits(CXXMethodDecl cxxMethodDecl)
        {
            var remappedName = GetRemappedCursorName(cxxMethodDecl);
            var overloadIndex = GetOverloadIndex(cxxMethodDecl);

            if (overloadIndex != 0)
            {
                remappedName = $"{remappedName}{overloadIndex}";
            }
            return remappedName;
        }

        // The vtbl emission passes flatten the primary polymorphic base's vtable into the derived type (it
        // shares, and extends, the derived type's vtable pointer) so the derived type re-exposes every
        // inherited virtual method at its native index. This holds whether the pointer lives in the derived
        // type's own `lpVtbl` (vtbl-only primary base) or inside a field-bearing primary base subobject; only
        // the physical location of the pointer differs (see `GetVtblPtrAccessPrefix`). Every other polymorphic
        // base becomes a subobject with its own vtable pointer and is not flattened here.
        IEnumerable<CXXRecordDecl> EnumerateFlattenedVtblBases(CXXRecordDecl cxxRecordDecl)
        {
            foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
            {
                var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);

                if (HasVtbl(baseCxxRecordDecl, out var baseHasBaseVtbl) || baseHasBaseVtbl)
                {
                    yield return baseCxxRecordDecl;
                    yield break;
                }
            }
        }

        void OutputDelegateSignatures(CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl, HashSet<string> emittedMemberNames)
        {
            if (!_config.ExcludeFnptrCodegen)
            {
                return;
            }

            foreach (var vtblBase in EnumerateFlattenedVtblBases(cxxRecordDecl))
            {
                OutputDelegateSignatures(rootCxxRecordDecl, vtblBase, emittedMemberNames);
            }

            var cxxMethodDecls = cxxRecordDecl.Methods;

            if (cxxMethodDecls.Count != 0)
            {
                foreach (var cxxMethodDecl in cxxMethodDecls.OrderBy((cxxmd) => cxxmd.VtblIndex))
                {
                    if (!cxxMethodDecl.IsVirtual)
                    {
                        continue;
                    }

                    if (IsExcluded(cxxMethodDecl, out var isExcludedByConflictingDefinition))
                    {
                        continue;
                    }

                    var remappedName = FixupNameForMultipleHits(cxxMethodDecl);

                    if (!emittedMemberNames.Add(GetVtblMemberDeduplicationKey(remappedName, cxxMethodDecl)))
                    {
                        continue;
                    }

                    _outputBuilder.WriteDivider();

                    Debug.Assert(CurrentContext.Cursor == rootCxxRecordDecl);
                    Visit(cxxMethodDecl);
                }
            }
        }

        void OutputMethods(CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl)
        {
            foreach (var cxxBaseSpecifier in cxxRecordDecl.Bases)
            {
                var baseCxxRecordDecl = GetRecordDecl(cxxBaseSpecifier);
                OutputMethods(rootCxxRecordDecl, baseCxxRecordDecl);
            }

            var cxxMethodDecls = cxxRecordDecl.Methods;

            if (cxxMethodDecls.Count != 0)
            {
                foreach (var cxxMethodDecl in cxxMethodDecls.OrderBy((cxxmd) => cxxmd.VtblIndex))
                {
                    if (cxxMethodDecl.IsVirtual)
                    {
                        continue;
                    }

                    if (cxxMethodDecl is CXXConstructorDecl or CXXDestructorDecl)
                    {
                        continue;
                    }

                    Debug.Assert(CurrentContext.Cursor == rootCxxRecordDecl);
                    Visit(cxxMethodDecl);
                    _outputBuilder.WriteDivider();
                }
            }
        }

        void OutputMarkerInterface(CXXRecordDecl cxxRecordDecl, CXXMethodDecl cxxMethodDecl)
        {
            if (!cxxMethodDecl.IsVirtual)
            {
                return;
            }

            if (IsExcluded(cxxMethodDecl, out var isExcludedByConflictingDefinition))
            {
                return;
            }

            if (_config.GenerateTrimmableVtbls && cxxMethodDecl.Parameters.Any((parmVarDecl) => IsType<PointerType>(parmVarDecl, parmVarDecl.Type, out var pointerType) &&
                                                                                                IsType<FunctionType>(parmVarDecl, pointerType.PointeeType, out _)))
            {
                // This breaks trimming right now
                return;
            }

            var currentContext = _context.AddLast((cxxMethodDecl, null));

            var returnType = cxxMethodDecl.ReturnType;
            var returnTypeName = GetRemappedTypeName(cxxMethodDecl, cxxRecordDecl, returnType, out var nativeTypeName);

            var remappedName = FixupNameForMultipleHits(cxxMethodDecl);
            var name = GetRemappedCursorName(cxxMethodDecl);
            var needsReturnFixup = false;
            var needsCastToTransparentStruct = false;

            if (!IsTypeVoid(cxxMethodDecl, returnType))
            {
                needsReturnFixup = NeedsReturnFixup(cxxMethodDecl);
                needsCastToTransparentStruct = _config.WithTransparentStructs.TryGetValue(returnTypeName, out var transparentStruct) && IsTransparentStructHandle(transparentStruct.Kind);
            }

            var desc = new FunctionOrDelegateDesc {
                AccessSpecifier = AccessSpecifier.Public,
                EscapedName = EscapeAndStripMethodName(name),
                IsMemberFunction = true,
                NativeTypeName = nativeTypeName,
                HasFnPtrCodeGen = !_config.ExcludeFnptrCodegen,
                IsCtxCxxRecord = true,
                IsCxxRecordCtxUnsafe = IsUnsafe(cxxRecordDecl),
                IsUnsafe = true,
                NeedsReturnFixup = needsReturnFixup,
                ReturnType = returnTypeName,
                VtblIndex = _config.GenerateVtblIndexAttribute ? cxxMethodDecl.VtblIndex : -1,
                Location = cxxMethodDecl.Location,
                WriteCustomAttrs = static context => {
                    (var cxxMethodDecl, var generator) = ((CXXMethodDecl, PInvokeGenerator))context;

                    generator.WithAttributes(cxxMethodDecl);
                    generator.WithUsings(cxxMethodDecl);
                },
                CustomAttrGeneratorData = (cxxMethodDecl, this),
            };

            var isUnsafe = true;
            _outputBuilder.BeginFunctionOrDelegate(in desc, ref isUnsafe);

            _outputBuilder.BeginFunctionInnerPrototype(in desc);

            Visit(cxxMethodDecl.Parameters);

            _outputBuilder.EndFunctionInnerPrototype(in desc);
            _outputBuilder.EndFunctionOrDelegate(in desc);

            Debug.Assert(_context.Last == currentContext);
            _context.RemoveLast();
        }

        void OutputMarkerInterfaces(CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl)
        {
            // Don't process the base types because that's exposed via inheritance instead
            var cxxMethodDecls = cxxRecordDecl.Methods;

            if (cxxMethodDecls.Count != 0)
            {
                foreach (var cxxMethodDecl in cxxMethodDecls.OrderBy((cxxmd) => cxxmd.VtblIndex))
                {
                    OutputMarkerInterface(rootCxxRecordDecl, cxxMethodDecl);
                }
            }
        }

        void OutputVtblEntries(CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl, HashSet<string> emittedMemberNames)
        {
            foreach (var vtblBase in EnumerateFlattenedVtblBases(cxxRecordDecl))
            {
                OutputVtblEntries(rootCxxRecordDecl, vtblBase, emittedMemberNames);
            }

            var cxxMethodDecls = cxxRecordDecl.Methods;

            if (cxxMethodDecls.Count != 0)
            {
                foreach (var cxxMethodDecl in cxxMethodDecls.OrderBy((cxxmd) => cxxmd.VtblIndex))
                {
                    OutputVtblEntry(rootCxxRecordDecl, cxxMethodDecl, emittedMemberNames);
                }
            }
        }

        void OutputVtblEntry(CXXRecordDecl cxxRecordDecl, CXXMethodDecl cxxMethodDecl, HashSet<string> emittedMemberNames)
        {
            if (!cxxMethodDecl.IsVirtual)
            {
                return;
            }

            if (IsExcluded(cxxMethodDecl, out var isExcludedByConflictingDefinition))
            {
                return;
            }

            var cxxMethodDeclTypeName = GetRemappedTypeName(cxxMethodDecl, cxxRecordDecl, cxxMethodDecl.Type, out var nativeTypeName, skipUsing: false, ignoreTransparentStructsWhereRequired: true);

            if (_config.GenerateMarkerInterfaces && !_config.ExcludeFnptrCodegen)
            {
                var cxxRecordDeclName = GetRemappedCursorName(cxxRecordDecl);
                cxxMethodDeclTypeName = cxxMethodDeclTypeName.Replace($"<{cxxRecordDeclName}*,", "<TSelf*,", StringComparison.Ordinal);
            }

            var remappedName = FixupNameForMultipleHits(cxxMethodDecl);
            var escapedName = EscapeAndStripMethodName(remappedName);

            if (!emittedMemberNames.Add(GetVtblMemberDeduplicationKey(escapedName, cxxMethodDecl)))
            {
                return;
            }

            var desc = new FieldDesc {
                AccessSpecifier = AccessSpecifier.Public,
                NativeTypeName = nativeTypeName,
                EscapedName = escapedName,
                Offset = null,
                NeedsNewKeyword = NeedsNewKeyword(remappedName),
                Location = cxxMethodDecl.Location,
                WriteCustomAttrs = static context => {
                    (var cxxMethodDecl, var generator) = ((CXXMethodDecl, PInvokeGenerator))context;

                    generator.WithAttributes(cxxMethodDecl);
                    generator.WithUsings(cxxMethodDecl);
                },
                CustomAttrGeneratorData = (cxxMethodDecl, this),
            };

            _outputBuilder.BeginField(in desc);
            _outputBuilder.WriteRegularField(cxxMethodDeclTypeName, escapedName);
            _outputBuilder.EndField(in desc);

            _outputBuilder.WriteDivider();
        }

        void OutputVtblHelperMethod(CXXRecordDecl cxxRecordDecl, CXXMethodDecl cxxMethodDecl, HashSet<string> emittedMemberNames)
        {
            if (!cxxMethodDecl.IsVirtual)
            {
                return;
            }

            if (IsExcluded(cxxMethodDecl, out var isExcludedByConflictingDefinition))
            {
                return;
            }

            if (!emittedMemberNames.Add(GetVtblMemberDeduplicationKey(EscapeAndStripMethodName(GetRemappedCursorName(cxxMethodDecl)), cxxMethodDecl)))
            {
                return;
            }

            var currentContext = _context.AddLast((cxxMethodDecl, null));

            var returnType = cxxMethodDecl.ReturnType;
            var returnTypeName = GetRemappedTypeName(cxxMethodDecl, cxxRecordDecl, returnType, out var nativeTypeName);

            var remappedName = FixupNameForMultipleHits(cxxMethodDecl);
            var name = GetRemappedCursorName(cxxMethodDecl);
            var needsReturnFixup = false;
            var needsCastToTransparentStruct = false;
            var cxxRecordDeclName = GetRemappedCursorName(cxxRecordDecl);
            var parentName = cxxRecordDeclName;
            var isInherited = false;
            var vtblPtrAccessPrefix = GetVtblPtrAccessPrefix(cxxRecordDecl);

            var parent = cxxMethodDecl.Parent;
            Debug.Assert(parent is not null);

            if (parent != cxxRecordDecl)
            {
                parentName = GetRemappedCursorName(parent);
                isInherited = true;
            }

            if (!IsTypeVoid(cxxMethodDecl, returnType))
            {
                needsReturnFixup = NeedsReturnFixup(cxxMethodDecl);
                needsCastToTransparentStruct = _config.WithTransparentStructs.TryGetValue(returnTypeName, out var transparentStruct) && IsTransparentStructHandle(transparentStruct.Kind);
            }

            var desc = new FunctionOrDelegateDesc {
                AccessSpecifier = AccessSpecifier.Public,
                IsAggressivelyInlined = _config.GenerateAggressiveInlining,
                EscapedName = EscapeAndStripMethodName(name),
                ParentName = parentName,
                IsMemberFunction = true,
                IsInherited = isInherited,
                NativeTypeName = nativeTypeName,
                NeedsNewKeyword = NeedsNewKeyword(name, cxxMethodDecl.Parameters),
                HasFnPtrCodeGen = !_config.ExcludeFnptrCodegen,
                IsCtxCxxRecord = true,
                IsCxxRecordCtxUnsafe = IsUnsafe(cxxRecordDecl),
                IsReadOnly = IsReadonly(cxxMethodDecl),
                IsUnsafe = true,
                NeedsReturnFixup = needsReturnFixup,
                ReturnType = returnTypeName,
                VtblIndex = _config.GenerateVtblIndexAttribute ? cxxMethodDecl.VtblIndex : -1,
                Location = cxxMethodDecl.Location,
                HasBody = true,
                WriteCustomAttrs = static context => {
                    (var cxxMethodDecl, var generator) = ((CXXMethodDecl, PInvokeGenerator))context;

                    generator.WithAttributes(cxxMethodDecl);
                    generator.WithUsings(cxxMethodDecl);
                },
                CustomAttrGeneratorData = (cxxMethodDecl, this),
            };

            var isUnsafe = true;
            _outputBuilder.BeginFunctionOrDelegate(in desc, ref isUnsafe);

            _outputBuilder.BeginFunctionInnerPrototype(in desc);

            Visit(cxxMethodDecl.Parameters);

            _outputBuilder.EndFunctionInnerPrototype(in desc);
            _outputBuilder.BeginBody();

            var escapedCXXRecordDeclName = EscapeName(cxxRecordDeclName);

            _outputBuilder.BeginInnerFunctionBody();
            var body = _outputBuilder.BeginCSharpCode();

            if (_config.GenerateCompatibleCode)
            {
                body.Write("fixed (");
                body.Write(escapedCXXRecordDeclName);
                body.WriteLine("* pThis = &this)");
                body.WriteBlockStart();
                body.WriteIndentation();
            }

            if (needsReturnFixup)
            {
                body.BeginMarker("fixup", new KeyValuePair<string, object>("type", "*result"));
                body.Write(returnTypeName);
                body.EndMarker("fixup");
                body.Write(" result");
                body.WriteSemicolon();
                body.WriteNewline();
                body.WriteIndentation();
            }

            if (!IsTypeVoid(cxxMethodDecl, returnType))
            {
                body.Write("return ");
            }

            if (needsCastToTransparentStruct)
            {
                body.Write("((");
                body.Write(returnTypeName);
                body.Write(")(");
            }

            if (needsReturnFixup)
            {
                body.Write('*');
            }

            if (_config.ExcludeFnptrCodegen)
            {
                body.Write("Marshal.GetDelegateForFunctionPointer<");
                body.BeginMarker("delegate");
                body.Write(PrefixAndStripMethodName(name, GetOverloadIndex(cxxMethodDecl)));
                body.EndMarker("delegate");
                body.Write(">(");
            }

            if (_config.GenerateExplicitVtbls)
            {
                if (vtblPtrAccessPrefix.Length != 0)
                {
                    // The shared vtable pointer lives inside a field-bearing primary base subobject and is
                    // typed as that base's `Vtbl`; reinterpret it as this type's fuller `Vtbl` so the entries
                    // this type appends onto the shared vtable resolve at their native offsets.
                    var vtblTypeName = (_config.GenerateMarkerInterfaces && !_config.GenerateCompatibleCode) ? $"Vtbl<{nativeName}>" : "Vtbl";
                    body.Write("((");
                    body.Write(vtblTypeName);
                    body.Write("*)");
                    body.Write(vtblPtrAccessPrefix);
                    body.Write("lpVtbl)->");
                }
                else
                {
                    body.Write("lpVtbl->");
                }

                body.BeginMarker("vtbl", new KeyValuePair<string, object>("explicit", true));
                body.Write(EscapeAndStripMethodName(remappedName));
                body.EndMarker("vtbl");
            }
            else
            {
                var cxxMethodDeclTypeName = GetRemappedTypeName(cxxMethodDecl, cxxRecordDecl, cxxMethodDecl.Type, out _, skipUsing: false, ignoreTransparentStructsWhereRequired: true);

                if (!_config.ExcludeFnptrCodegen)
                {
                    body.Write('(');
                }

                body.Write('(');
                body.Write(cxxMethodDeclTypeName);
                body.Write(")(");
                body.Write(vtblPtrAccessPrefix);
                body.Write("lpVtbl[");
                body.BeginMarker("vtbl", new KeyValuePair<string, object>("explicit", false));
                body.Write(cxxMethodDecl.VtblIndex);
                body.EndMarker("vtbl");
                body.Write("])");

                if (!_config.ExcludeFnptrCodegen)
                {
                    body.Write(')');
                }
            }

            if (_config.ExcludeFnptrCodegen)
            {
                body.Write(')');
            }

            body.Write('(');

            body.BeginMarker("param", new KeyValuePair<string, object>("special", "thisPtr"));
            if (_config.GenerateCompatibleCode)
            {
                body.Write("pThis");
            }
            else
            {
                body.Write('(');
                body.Write(escapedCXXRecordDeclName);
                body.Write("*)Unsafe.AsPointer(");

                if (IsReadonly(cxxMethodDecl))
                {
                    if (!_config.GenerateLatestCode)
                    {
                        body.Write("ref Unsafe.AsRef(");
                    }

                    body.Write("in this");

                    if (!_config.GenerateLatestCode)
                    {
                        body.Write(')');
                    }
                }
                else
                {
                    body.Write("ref this");
                }
                body.Write(')');
            }
            body.EndMarker("param");

            if (needsReturnFixup)
            {
                body.BeginMarker("param", new KeyValuePair<string, object>("special", "retFixup"));
                body.Write(", &result");
                body.EndMarker("param");
            }

            var parmVarDecls = cxxMethodDecl.Parameters;

            for (var index = 0; index < parmVarDecls.Count; index++)
            {
                body.Write(", ");

                var parmVarDeclName = GetRemappedCursorName(parmVarDecls[index]);
                var escapedParmVarDeclName = EscapeName(parmVarDeclName);

                if (parmVarDeclName.Equals("param", StringComparison.Ordinal))
                {
                    escapedParmVarDeclName += index;
                }

                body.BeginMarker("param", new KeyValuePair<string, object>("name", escapedParmVarDeclName));
                body.Write(escapedParmVarDeclName);
                body.EndMarker("param");
            }

            body.Write(')');

            if (!_config.GenerateDisableRuntimeMarshalling && returnTypeName.Equals("bool", StringComparison.Ordinal))
            {
                // bool is not blittable when DisableRuntimeMarshalling is not specified, so we shouldn't use it for P/Invoke signatures
                body.Write(" != 0");
            }

            if (needsCastToTransparentStruct)
            {
                body.Write("))");
            }

            body.WriteSemicolon();
            body.WriteNewline();

            if (_config.GenerateCompatibleCode)
            {
                body.WriteBlockEnd();
            }

            _outputBuilder.EndCSharpCode(body);
            _outputBuilder.EndInnerFunctionBody();
            _outputBuilder.EndBody();
            _outputBuilder.EndFunctionOrDelegate(in desc);

            Debug.Assert(_context.Last == currentContext);
            _context.RemoveLast();
        }

        void OutputVtblHelperMethods(CXXRecordDecl rootCxxRecordDecl, CXXRecordDecl cxxRecordDecl, HashSet<string> emittedMemberNames)
        {
            foreach (var vtblBase in EnumerateFlattenedVtblBases(cxxRecordDecl))
            {
                OutputVtblHelperMethods(rootCxxRecordDecl, vtblBase, emittedMemberNames);
            }

            var cxxMethodDecls = cxxRecordDecl.Methods;

            if (cxxMethodDecls.Count != 0)
            {
                foreach (var cxxMethodDecl in cxxMethodDecls.OrderBy((cxxmd) => cxxmd.VtblIndex))
                {
                    _outputBuilder.WriteDivider();
                    OutputVtblHelperMethod(rootCxxRecordDecl, cxxMethodDecl, emittedMemberNames);
                }
            }
        }

        void VisitBitfieldDecl(FieldDecl fieldDecl, BitfieldDesc[] bitfieldDescs, RecordDecl recordDecl, string contextName, ref int index, ref long previousSize, ref long remainingBits)
        {
            Debug.Assert(fieldDecl.IsBitField);

            var typeName = GetRemappedTypeName(fieldDecl, context: null, fieldDecl.Type, out var nativeTypeName);

            if (string.IsNullOrWhiteSpace(nativeTypeName))
            {
                nativeTypeName = typeName;
            }

            nativeTypeName += $" : {fieldDecl.BitWidthValue}";

            var currentSize = fieldDecl.Type.Handle.SizeOf;
            var bitfieldName = "_bitfield";

            // A `bool` bitfield cannot be shifted or masked directly and needs an integer
            // backing store. It is emitted using the same unsigned-integer path as the
            // equivalently-sized integer type, with the public accessor kept as `bool` and
            // the conversion done at the get/set boundary.
            var isBooleanBitfield = fieldDecl.Type.CanonicalType.Kind == CXType_Bool;

            Type typeBacking;
            string typeNameBacking;

            var parent = fieldDecl.Parent;
            Debug.Assert(parent is not null);

            if ((!_config.GenerateUnixTypes && (currentSize != previousSize)) || (fieldDecl.BitWidthValue > remainingBits))
            {
                if (index >= 0)
                {
                    index++;
                    bitfieldName += index.ToString(CultureInfo.InvariantCulture);
                }

                remainingBits = currentSize * 8;
                previousSize = 0;

                var bitfieldDesc = (index > 0) ? bitfieldDescs[index - 1] : bitfieldDescs[0];
                typeBacking = bitfieldDesc.TypeBacking;
                typeNameBacking = GetRemappedTypeName(fieldDecl, context: null, typeBacking, out _);

                if (isBooleanBitfield)
                {
                    typeNameBacking = "byte";
                }

                if (parent == recordDecl)
                {
                    var fieldDesc = new FieldDesc {
                        AccessSpecifier = AccessSpecifier.Public,
                        NativeTypeName = null,
                        EscapedName = bitfieldName,
                        Offset = parent.IsUnion ? 0 : null,
                        NeedsNewKeyword = false,
                        Location = fieldDecl.Location,
                        WriteCustomAttrs = static context => {
                            (var bitfieldDesc, var generator) = ((BitfieldDesc, PInvokeGenerator))context;

                            if (!generator.Config.GenerateNativeBitfieldAttribute)
                            {
                                return;
                            }

                            var outputBuilder = generator._outputBuilder;
                            Debug.Assert(outputBuilder is not null);

                            foreach (var bitfieldRegion in bitfieldDesc.Regions)
                            {
                                outputBuilder.WriteCustomAttribute($"NativeBitfield(\"{bitfieldRegion.Name}\", offset: {bitfieldRegion.Offset}, length: {bitfieldRegion.Length})");
                            }

                            var namespaceName = generator.GetNamespace("NativeBitfieldAttribute");
                            generator.AddUsingDirective(outputBuilder, namespaceName);
                        },
                        CustomAttrGeneratorData = (bitfieldDesc, this),
                    };
                    _outputBuilder.BeginField(in fieldDesc);
                    _outputBuilder.WriteRegularField(typeNameBacking, bitfieldName);
                    _outputBuilder.EndField(in fieldDesc);
                }
            }
            else
            {
                currentSize = Math.Max(previousSize, currentSize);

                if (_config.GenerateUnixTypes && (currentSize > previousSize))
                {
                    remainingBits += (currentSize - previousSize) * 8;
                }

                if (index >= 0)
                {
                    bitfieldName += index.ToString(CultureInfo.InvariantCulture);
                }

                var bitfieldDesc = (index > 0) ? bitfieldDescs[index - 1] : bitfieldDescs[0];
                typeBacking = bitfieldDesc.TypeBacking;
                typeNameBacking = GetRemappedTypeName(fieldDecl, context: null, typeBacking, out _);

                if (isBooleanBitfield)
                {
                    typeNameBacking = "byte";
                }
            }

            var bitfieldOffset = (currentSize * 8) - remainingBits;
            var bitwidthHexStringBacking = ((1 << fieldDecl.BitWidthValue) - 1).ToString("X", CultureInfo.InvariantCulture);

            if (!IsType<BuiltinType>(fieldDecl, typeBacking, out var builtinTypeBacking))
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported bitfield type: '{typeBacking.TypeClassSpelling}'. Generated bindings may be incomplete.", fieldDecl);
                return;
            }

            var isTypeBackingSigned = false;

            switch (builtinTypeBacking.Kind)
            {
                case CXType_Bool:
                case CXType_Char_U:
                case CXType_UChar:
                case CXType_UShort:
                case CXType_UInt:
                {
                    bitwidthHexStringBacking += "u";
                    break;
                }

                case CXType_ULong:
                {
                    if (_config.GenerateUnixTypes)
                    {
                        goto default;
                    }

                    goto case CXType_UInt;
                }

                case CXType_ULongLong:
                {
                    if (typeNameBacking is "nuint" or "UIntPtr")
                    {
                        goto case CXType_UInt;
                    }

                    bitwidthHexStringBacking += "UL";
                    break;
                }

                case CXType_Char_S:
                case CXType_SChar:
                case CXType_Short:
                case CXType_Int:
                {
                    isTypeBackingSigned = true;
                    break;
                }

                case CXType_Long:
                {
                    isTypeBackingSigned = true;

                    if (_config.GenerateUnixTypes)
                    {
                        goto default;
                    }
                    break;
                }

                case CXType_LongLong:
                {
                    isTypeBackingSigned = true;

                    if (typeNameBacking is "nint" or "IntPtr")
                    {
                        goto case CXType_Int;
                    }

                    bitwidthHexStringBacking += "L";
                    break;
                }

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported bitfield type: '{builtinTypeBacking.TypeClassSpelling}'. Generated bindings may be incomplete.", fieldDecl);
                    break;
                }
            }

            var bitwidthHexString = ((1 << fieldDecl.BitWidthValue) - 1).ToString("X", CultureInfo.InvariantCulture);
            var type = fieldDecl.Type;

            if (IsType<BuiltinType>(fieldDecl, type, out var builtinType))
            {
                type = builtinType;
            }
            else if (IsType<EnumType>(fieldDecl, type, out var enumType))
            {
                if (IsType<BuiltinType>(fieldDecl, enumType.Decl.IntegerType, out builtinType))
                {
                    type = enumType;
                }
                else
                {
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported bitfield type: '{enumType.Decl.IntegerType.TypeClassSpelling}'. Generated bindings may be incomplete.", fieldDecl);
                    return;
                }
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported bitfield type: '{fieldDecl.Type.TypeClassSpelling}'. Generated bindings may be incomplete.", fieldDecl);
                return;
            }

            var isTypeSigned = false;

            switch (builtinType.Kind)
            {
                case CXType_Bool:
                case CXType_Char_U:
                case CXType_UChar:
                case CXType_UShort:
                case CXType_UInt:
                {
                    bitwidthHexString += "u";
                    break;
                }

                case CXType_ULong:
                {
                    if (_config.GenerateUnixTypes)
                    {
                        goto default;
                    }

                    goto case CXType_UInt;
                }

                case CXType_ULongLong:
                {
                    if (typeNameBacking is "nuint" or "UIntPtr")
                    {
                        goto case CXType_UInt;
                    }

                    bitwidthHexString += "UL";
                    break;
                }

                case CXType_Char_S:
                case CXType_SChar:
                case CXType_Short:
                case CXType_Int:
                {
                    isTypeSigned = true;
                    break;
                }

                case CXType_Long:
                {
                    isTypeSigned = true;

                    if (_config.GenerateUnixTypes)
                    {
                        goto default;
                    }
                    break;
                }

                case CXType_LongLong:
                {
                    isTypeSigned = true;

                    if (typeNameBacking is "nint" or "IntPtr")
                    {
                        goto case CXType_Int;
                    }

                    bitwidthHexString += "L";
                    break;
                }

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Warning, $"Unsupported bitfield type: '{builtinType.TypeClassSpelling}'. Generated bindings may be incomplete.", fieldDecl);
                    break;
                }
            }

            var accessSpecifier = GetAccessSpecifier(fieldDecl, matchStar: false);
            var name = GetRemappedCursorName(fieldDecl);
            var escapedName = EscapeName(name);

            // The public accessor keeps the original type (e.g. `bool`) while the shift/mask
            // arithmetic is done against the integer backing type; the two only differ for a
            // `bool` bitfield, which is converted at the get/set boundary below.
            var fieldTypeName = typeName;

            if (isBooleanBitfield)
            {
                typeName = typeNameBacking;
            }

            var desc = new FieldDesc {
                AccessSpecifier = accessSpecifier,
                NativeTypeName = nativeTypeName,
                EscapedName = escapedName,
                ParentName = GetRemappedCursorName(parent),
                Offset = null,
                NeedsNewKeyword = false,
                Location = fieldDecl.Location,
                HasBody = true,
                WriteCustomAttrs = static context => {
                    (var fieldDecl, var generator) = ((FieldDecl, PInvokeGenerator))context;

                    generator.WithAttributes(fieldDecl);
                    generator.WithUsings(fieldDecl);
                },
                CustomAttrGeneratorData = (fieldDecl, this),
            };

            _outputBuilder.WriteDivider();
            _outputBuilder.BeginField(in desc);
            _outputBuilder.WriteRegularField(fieldTypeName, escapedName);
            _outputBuilder.BeginBody();

            var recordDeclName = GetCursorName(recordDecl);

            // Small types become uint32/int32s after shifting
            var isSmallType = fieldDecl.Type.Handle.SizeOf < 4;
            var isSmallTypeBacking = currentSize < 4;

            // Check if field/backing types match
            var isTypeMismatch = type != builtinTypeBacking;

            // Signed types are sign extended when shifted
            var isUnsignedToSigned = !isTypeBackingSigned && isTypeSigned;

            // Check if type is directly shiftable/maskable
            // Remapped types are not guaranteed to be shiftable or maskable
            // Enums are maskable, but not shiftable
            var isTypeLikelyRemapped = !isTypeMismatch && (typeName != typeNameBacking);
            var isTypeAnEnum = IsType<EnumType>(fieldDecl);

            // Main cases:
            // backing  int, current int            (value << cns) >> cns
            // backing  int, current uint           (uint)((value >> cns) & msk)

            // backing uint, current int            (int)(value << cns) >> cns
            // backing uint, current uint           (value >> cns) & msk

            // backing uint, current byte           (byte)((value >> cns) & msk)
            // backing uint, current sbyte          (sbyte)((sbyte)(value << cns) >> cns)

            // Getter
            {
                _outputBuilder.BeginGetter(_config.GenerateAggressiveInlining,
                    isReadOnly: !Config.GenerateCompatibleCode);
                var code = _outputBuilder.BeginCSharpCode();

                code.WriteIndented("return ");

                var needsCastToFinal = (isSmallType || isTypeMismatch || isUnsignedToSigned) && !isTypeAnEnum;

                // This is to handle the "backing uint, current sbyte" case
                var needsCastToFinalAgain = isUnsignedToSigned && isSmallType;
                if (needsCastToFinalAgain)
                {
                    code.Write('(');
                    code.BeginMarker("typeName");
                    code.Write(typeName);
                    code.EndMarker("typeName");
                    code.Write(")(");
                }

                if (needsCastToFinal && !isBooleanBitfield)
                {
                    code.Write('(');
                    code.BeginMarker("typeName");
                    code.Write(typeName);
                    code.EndMarker("typeName");
                    code.Write(")");
                }

                var needsCastToFinalParen = needsCastToFinal && !isUnsignedToSigned;
                if (needsCastToFinalParen)
                {
                    code.Write('(');
                }

                var needsCastBeforeOp = isTypeLikelyRemapped || isTypeAnEnum;
                if (needsCastBeforeOp)
                {
                    code.Write('(');
                    code.BeginMarker("typeName");
                    code.Write(typeName);
                    code.EndMarker("typeName");
                    code.Write(")(");
                }

                if (isTypeSigned)
                {
                    code.Write('(');

                    if (!string.IsNullOrWhiteSpace(contextName))
                    {
                        code.BeginMarker("contextName");
                        code.Write(contextName);
                        code.EndMarker("contextName");
                        code.Write('.');
                    }

                    code.BeginMarker("bitfieldName");
                    code.Write(bitfieldName);
                    code.EndMarker("bitfieldName");

                    code.Write(" << ");
                    code.BeginMarker("remainingBitsMinusBitWidth");
                    code.Write(remainingBits - fieldDecl.BitWidthValue);
                    code.EndMarker("remainingBitsMinusBitWidth");

                    code.Write(')');

                    code.Write(" >> ");
                    code.BeginMarker("currentSizeMinusBitWidth");
                    code.Write((currentSize * 8) - fieldDecl.BitWidthValue);
                    code.EndMarker("currentSizeMinusBitWidth");
                }
                else
                {
                    var needsOffset = bitfieldOffset != 0;
                    if (needsOffset)
                    {
                        code.Write('(');
                    }

                    if (!string.IsNullOrWhiteSpace(contextName))
                    {
                        code.BeginMarker("contextName");
                        code.Write(contextName);
                        code.EndMarker("contextName");
                        code.Write('.');
                    }

                    code.BeginMarker("bitfieldName");
                    code.Write(bitfieldName);
                    code.EndMarker("bitfieldName");

                    if (needsOffset)
                    {
                        code.Write(" >> ");
                        code.BeginMarker("bitfieldOffset");
                        code.Write(bitfieldOffset);
                        code.EndMarker("bitfieldOffset");

                        code.Write(')');
                    }

                    code.Write(" & 0x");
                    code.BeginMarker("bitwidthHexStringBacking");
                    code.Write(bitwidthHexStringBacking);
                    code.EndMarker("bitwidthHexStringBacking");
                }

                if (needsCastBeforeOp)
                {
                    code.Write(')');
                }

                if (needsCastToFinalParen)
                {
                    code.Write(')');
                }

                if (needsCastToFinalAgain)
                {
                    code.Write(')');
                }

                if (isBooleanBitfield)
                {
                    code.Write(" != 0");
                }

                code.WriteSemicolon();
                code.WriteNewline();
                _outputBuilder.EndCSharpCode(code);
                _outputBuilder.EndGetter();
            }

            // Setter
            {
                _outputBuilder.BeginSetter(_config.GenerateAggressiveInlining);
                var code = _outputBuilder.BeginCSharpCode();
                code.WriteIndentation();

                if (!string.IsNullOrWhiteSpace(contextName))
                {
                    code.BeginMarker("contextName");
                    code.Write(contextName);
                    code.EndMarker("contextName");
                    code.Write('.');
                }

                code.BeginMarker("bitfieldName");
                code.Write(bitfieldName);
                code.EndMarker("bitfieldName");

                code.Write(" = ");

                var needsCastToFinal = isSmallTypeBacking;
                if (needsCastToFinal)
                {
                    code.Write('(');
                    code.BeginMarker("typeNameBacking");
                    code.Write(typeNameBacking);
                    code.EndMarker("typeNameBacking");
                    code.Write(")(");
                }

                // Zero out target bits
                code.Write('(');

                if (!string.IsNullOrWhiteSpace(contextName))
                {
                    code.Write(contextName);
                    code.Write('.');
                }

                code.Write(bitfieldName);

                code.Write(" & ~");

                if (bitfieldOffset != 0)
                {
                    code.Write('(');
                }

                code.Write("0x");
                code.BeginMarker("bitwidthHexStringBacking");
                code.Write(bitwidthHexStringBacking);
                code.EndMarker("bitwidthHexStringBacking");

                if (bitfieldOffset != 0)
                {
                    code.Write(" << ");
                    code.BeginMarker("bitfieldOffset");
                    code.Write(bitfieldOffset);
                    code.EndMarker("bitfieldOffset");
                    code.Write(')');
                }

                // Write to target bits
                code.Write(") | ");

                var needsCastBeforeLogicalOr = isTypeMismatch && !isTypeAnEnum;
                if (needsCastBeforeLogicalOr)
                {
                    code.Write('(');
                    code.Write(typeNameBacking);
                    code.Write(")");
                }

                code.Write('(');

                if (bitfieldOffset != 0)
                {
                    code.Write('(');
                }

                var needsCastBeforeOp = isTypeLikelyRemapped || isTypeAnEnum;
                if (needsCastBeforeOp)
                {
                    code.Write('(');
                    code.Write(typeNameBacking);
                    code.Write(")(value)");
                }
                else if (isBooleanBitfield)
                {
                    code.Write("(value ? 1 : 0)");
                }
                else
                {
                    code.Write("value");
                }

                code.Write(" & 0x");
                code.BeginMarker("bitwidthHexString");
                code.Write(bitwidthHexString);
                code.EndMarker("bitwidthHexString");

                if (bitfieldOffset != 0)
                {
                    code.Write(") << ");
                    code.Write(bitfieldOffset);
                }

                code.Write(')');

                if (needsCastToFinal)
                {
                    code.Write(')');
                }

                code.WriteSemicolon();
                code.WriteNewline();
                _outputBuilder.EndCSharpCode(code);
                _outputBuilder.EndSetter();
            }

            _outputBuilder.EndBody();
            _outputBuilder.EndField(in desc);
            _outputBuilder.WriteDivider();

            remainingBits -= fieldDecl.BitWidthValue;
            previousSize = Math.Max(previousSize, currentSize);
        }

        void VisitConstantOrIncompleteArrayFieldDecl(RecordDecl recordDecl, FieldDecl constantOrIncompleteArray)
        {
            if (!IsTypeConstantOrIncompleteArray(constantOrIncompleteArray, out var arrayType))
            {
                AddDiagnostic(DiagnosticLevel.Error, "Expected constant or incomplete array. Generated bindings may be incomplete", constantOrIncompleteArray);
                return;
            }

            var outputBuilder = _outputBuilder;
            var arrayTypeName = GetRemappedTypeName(constantOrIncompleteArray, context: null, arrayType, out _);

            if (IsSupportedFixedSizedBufferType(arrayTypeName))
            {
                return;
            }

            _outputBuilder.WriteDivider();

            var alignment = Math.Max(recordDecl.TypeForDecl.Handle.AlignOf, 1);
            var maxAlignm = recordDecl.Fields.Any() ? recordDecl.Fields.Max((fieldDecl) => Math.Max(fieldDecl.Type.Handle.AlignOf, 1)) : alignment;

            var accessSpecifier = GetAccessSpecifier(constantOrIncompleteArray, matchStar: false);
            var elementType = arrayType.ElementType;
            var isUnsafeElementType = IsTypePointerOrReference(constantOrIncompleteArray, arrayType.ElementType) &&
                                      !arrayTypeName.Equals("IntPtr", StringComparison.Ordinal) && !arrayTypeName.Equals("UIntPtr", StringComparison.Ordinal);

            var name = GetArtificialFixedSizedBufferName(constantOrIncompleteArray);
            var escapedName = EscapeName(name);

            var arraySize = Math.Max((arrayType as ConstantArrayType)?.Size ?? 0, 1);
            var totalSize = arraySize;
            var totalSizeString = $"{arraySize}";
            var sizePerDimension = new List<(long index, long size)>() { (0, arraySize) };

            while (IsTypeConstantOrIncompleteArray(recordDecl, elementType, out var subArrayType))
            {
                var subArraySize = Math.Max((subArrayType as ConstantArrayType)?.Size ?? 0, 1);
                totalSize *= subArraySize;
                totalSizeString += $" * {subArraySize}";
                sizePerDimension.Add((0, subArraySize));

                elementType = subArrayType.ElementType;
            }

            long alignment32 = -1;
            long alignment64 = -1;

            GetTypeSize(constantOrIncompleteArray, constantOrIncompleteArray.Type, ref alignment32, ref alignment64, out var size32, out var size64);

            if ((size32 == 0 || size64 == 0) && _testOutputBuilder != null)
            {
                AddDiagnostic(DiagnosticLevel.Info, $"{escapedName} (constant array field) has a size of 0", constantOrIncompleteArray);
            }

            if (_config.GenerateCompatibleCode || (totalSize <= 1) || isUnsafeElementType)
            {
                totalSizeString = null;
            }

            var desc = new StructDesc {
                AccessSpecifier = accessSpecifier,
                EscapedName = escapedName,
                IsUnsafe = isUnsafeElementType,
                Layout = new() {
                    Alignment32 = alignment32,
                    Alignment64 = alignment64,
                    Size32 = size32,
                    Size64 = size64,
                    Pack = alignment < maxAlignm ? alignment.ToString(CultureInfo.InvariantCulture) : null,
                    MaxFieldAlignment = maxAlignm,
                    Kind = LayoutKind.Sequential
                },
                Location = constantOrIncompleteArray.Location,
                IsNested = true,
                WriteCustomAttrs = static context => {
                    (var fieldDecl, var outputBuilder, var generator, var totalSizeString) = ((FieldDecl, IOutputBuilder, PInvokeGenerator, string?))context;

                    generator.WithAttributes(fieldDecl);
                    generator.WithUsings(fieldDecl);

                    if (totalSizeString is not null)
                    {
                        outputBuilder.WriteCustomAttribute($"InlineArray({totalSizeString})");
                    }
                },
                CustomAttrGeneratorData = (constantOrIncompleteArray, _outputBuilder, this, totalSizeString),
            };

            _outputBuilder.BeginStruct(in desc);

            var firstFieldName = "";
            var numFieldsToEmit = (totalSizeString is not null) ? Math.Min(totalSize, 1) : totalSize;

            for (long i = 0; i < numFieldsToEmit; i++)
            {
                var dimension = sizePerDimension[0];
                var firstDimension = dimension.index++;
                var fieldName = $"e{firstDimension}";
                sizePerDimension[0] = dimension;

                var separateStride = false;
                for (var d = 1; d < sizePerDimension.Count; d++)
                {
                    dimension = sizePerDimension[d];
                    fieldName += $"_{dimension.index}";
                    sizePerDimension[d] = dimension;

                    var previousDimension = sizePerDimension[d - 1];

                    if (previousDimension.index == previousDimension.size)
                    {
                        previousDimension.index = 0;
                        dimension.index++;
                        sizePerDimension[d - 1] = previousDimension;
                        separateStride = true;
                    }

                    sizePerDimension[d] = dimension;
                }

                if (string.IsNullOrEmpty(firstFieldName))
                {
                    firstFieldName = fieldName;
                }

                var fieldDesc = new FieldDesc {
                    AccessSpecifier = accessSpecifier,
                    NativeTypeName = null,
                    EscapedName = fieldName,
                    Offset = null,
                    NeedsNewKeyword = false,
                    Location = constantOrIncompleteArray.Location,
                };

                _outputBuilder.BeginField(in fieldDesc);
                _outputBuilder.WriteRegularField(arrayTypeName, fieldName);
                _outputBuilder.EndField(in fieldDesc);
                if (!separateStride)
                {
                    _outputBuilder.SuppressDivider();
                }
            }

            var generateCompatibleCode = _config.GenerateCompatibleCode;

            if (generateCompatibleCode || isUnsafeElementType)
            {
                // Always emit the int indexer; when GenerateFixedBufferIndexerOverloads is set, also
                // emit uint, nint, and nuint overloads so callers can index without casting. Pointer
                // element access accepts all four types, so the body is identical.
                var indexTypes = _config.GenerateFixedBufferIndexerOverloads
                                 ? (ReadOnlySpan<string>)["int", "uint", "nint", "nuint"]
                                 : (ReadOnlySpan<string>)["int"];

                foreach (var indexType in indexTypes)
                {
                    _outputBuilder.BeginIndexer(AccessSpecifier.Public, isUnsafe: generateCompatibleCode && !isUnsafeElementType, needsUnscopedRef: false);
                    _outputBuilder.WriteIndexer($"ref {arrayTypeName}");
                    _outputBuilder.BeginIndexerParameters();
                    var param = new ParameterDesc {
                        Name = "index",
                        Type = indexType,
                    };
                    _outputBuilder.BeginParameter(in param);
                    _outputBuilder.EndParameter(in param);
                    _outputBuilder.EndIndexerParameters();
                    _outputBuilder.BeginBody();

                    _outputBuilder.BeginGetter(_config.GenerateAggressiveInlining, isReadOnly: false);
                    var code = _outputBuilder.BeginCSharpCode();

                    code.WriteIndented("fixed (");
                    code.Write(arrayTypeName);
                    code.Write("* pThis = &");
                    code.Write(firstFieldName);
                    code.WriteLine(')');
                    code.WriteBlockStart();
                    code.WriteIndented("return ref pThis[index]");
                    code.WriteSemicolon();
                    code.WriteNewline();
                    code.WriteBlockEnd();
                    _outputBuilder.EndCSharpCode(code);

                    _outputBuilder.EndGetter();
                    _outputBuilder.EndBody();
                    _outputBuilder.EndIndexer();
                }
            }
            else if (totalSizeString is null)
            {
                // This non-compatible path uses Unsafe.Add/AsSpan()[index], which only accept an int
                // index, so it only emits the int indexer. Adding uint/nint/nuint overloads here is
                // deferred -- it needs casts or different helpers -- see #450 which covered the
                // compatible fixed (T* pThis...) path above.
                _outputBuilder.BeginIndexer(AccessSpecifier.Public, isUnsafe: false, needsUnscopedRef: !_config.GenerateCompatibleCode);
                _outputBuilder.WriteIndexer($"ref {arrayTypeName}");
                _outputBuilder.BeginIndexerParameters();
                var param = new ParameterDesc {
                    Name = "index",
                    Type = "int",
                };
                _outputBuilder.BeginParameter(in param);
                _outputBuilder.EndParameter(in param);
                _outputBuilder.EndIndexerParameters();
                _outputBuilder.BeginBody();

                _outputBuilder.BeginGetter(_config.GenerateAggressiveInlining, isReadOnly: false);
                var code = _outputBuilder.BeginCSharpCode();
                code.AddUsingDirective("System");
                code.AddUsingDirective("System.Runtime.InteropServices");

                code.WriteIndented("return ref ");

                if (arraySize == 1)
                {
                    code.AddUsingDirective("System.Runtime.CompilerServices");
                    code.Write("Unsafe.Add(ref e0, index)");
                }
                else
                {
                    code.Write("AsSpan()[index]");
                }

                code.WriteSemicolon();
                code.WriteNewline();
                _outputBuilder.EndCSharpCode(code);

                _outputBuilder.EndGetter();
                _outputBuilder.EndBody();
                _outputBuilder.EndIndexer();

                var function = new FunctionOrDelegateDesc {
                    AccessSpecifier = AccessSpecifier.Public,
                    EscapedName = "AsSpan",
                    IsAggressivelyInlined = _config.GenerateAggressiveInlining,
                    IsStatic = false,
                    IsMemberFunction = true,
                    ReturnType = $"Span<{arrayTypeName}>",
                    Location = constantOrIncompleteArray.Location,
                    HasBody = true,
                    NeedsUnscopedRef = !_config.GenerateCompatibleCode,
                };

                var isUnsafe = false;
                _outputBuilder.BeginFunctionOrDelegate(in function, ref isUnsafe);

                _outputBuilder.BeginFunctionInnerPrototype(in function);

                if (arraySize == 1)
                {
                    param = new ParameterDesc {
                        Name = "length",
                        Type = "int",
                    };

                    _outputBuilder.BeginParameter(in param);
                    _outputBuilder.EndParameter(in param);
                }

                _outputBuilder.EndFunctionInnerPrototype(in function);
                _outputBuilder.BeginBody(true);
                code = _outputBuilder.BeginCSharpCode();

                code.Write("MemoryMarshal.CreateSpan(ref ");
                code.Write(firstFieldName);
                code.Write(", ");

                if (arraySize == 1)
                {
                    code.Write("length");
                }
                else
                {
                    code.Write(totalSize);
                }

                code.Write(')');
                code.WriteSemicolon();
                _outputBuilder.EndBody(true);
                _outputBuilder.EndCSharpCode(code);
                _outputBuilder.EndFunctionOrDelegate(in function);
            }

            _outputBuilder.EndStruct(in desc);
        }

        void StartTestOutput(ref bool testOutputStarted, bool generateTestsClass, bool isTopLevelStruct)
        {
            if ((_testOutputBuilder is not null) && generateTestsClass && !isTopLevelStruct && !testOutputStarted)
            {
                _testOutputBuilder.WriteIndented("/// <summary>Provides validation of the <see cref=\"");
                _testOutputBuilder.Write(escapedName);
                _testOutputBuilder.WriteLine("\" /> struct.</summary>");

                WithAttributes(recordDecl, onlySupportedOSPlatform: true, isTestOutput: true);

                _testOutputBuilder.WriteIndented("public static unsafe partial class ");
                _testOutputBuilder.Write(escapedName);
                _testOutputBuilder.WriteLine("Tests");
                _testOutputBuilder.WriteBlockStart();

                testOutputStarted = true;
            }
        }
    }

    // Multiple base classes can each contribute a virtual member that maps to the same C# name and
    // signature (most notably each base's virtual destructor becoming `Dispose`). The single `lpVtbl`
    // model flattens every base into one vtable, so the same member would otherwise be emitted more
    // than once, which is a compile error. This builds a key of the emitted name plus the canonical
    // parameter types so that legitimate overloads remain distinct. See https://github.com/dotnet/ClangSharp/issues/592
    private static string GetVtblMemberDeduplicationKey(string emittedName, CXXMethodDecl cxxMethodDecl)
    {
        var builder = new StringBuilder(emittedName);
        _ = builder.Append('(');

        var parameters = cxxMethodDecl.Parameters;

        for (var index = 0; index < parameters.Count; index++)
        {
            if (index != 0)
            {
                _ = builder.Append(',');
            }
            _ = builder.Append(parameters[index].Type.CanonicalType.AsString);
        }

        _ = builder.Append(')');
        return builder.ToString();
    }
}
