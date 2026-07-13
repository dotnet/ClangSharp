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
    private void VisitVarDecl(VarDecl varDecl)
    {
        if (IsPrevContextStmt<DeclStmt>(out var declStmt, out _))
        {
            ForDeclStmt(varDecl, declStmt);
        }
        else if (IsPrevContextDecl<TranslationUnitDecl>(out _, out _) || IsPrevContextDecl<LinkageSpecDecl>(out _, out _) || IsPrevContextDecl<NamespaceDecl>(out _, out _) || IsPrevContextDecl<RecordDecl>(out _, out _))
        {
            if (!varDecl.HasInit)
            {
                // Nothing to do if a top level const declaration doesn't have an initializer
                return;
            }

            var type = varDecl.Type;
            var isMacroDefinitionRecord = false;

            var nativeName = GetCursorName(varDecl);
            if (nativeName.StartsWith("ClangSharpMacro_", StringComparison.Ordinal))
            {
                nativeName = nativeName["ClangSharpMacro_".Length..];
                isMacroDefinitionRecord = true;

                if (varDecl.Init is null)
                {
                    AddDiagnostic(DiagnosticLevel.Warning, $"Macro definition '{nativeName}' could not be resolved to a supported expression. Generated bindings may be incomplete.", varDecl);
                    return;
                }

                type = varDecl.Init.Type;
            }

            var accessSpecifier = GetAccessSpecifier(varDecl, matchStar: false);
            var name = GetRemappedName(nativeName, varDecl, tryRemapOperatorName: false, out var wasRemapped, skipUsing: true);
            var escapedName = EscapeName(name);

            if (isMacroDefinitionRecord)
            {
                if (IsStmtAsWritten<DeclRefExpr>(varDecl.Init, out var declRefExpr, removeParens: true))
                {
                    if ((declRefExpr.Decl is NamedDecl namedDecl) && (name == GetCursorName(namedDecl)))
                    {
                        return;
                    }
                }
                else if (IsStmtAsWritten<RecoveryExpr>(varDecl.Init, out _, removeParens: true))
                {
                    AddDiagnostic(DiagnosticLevel.Warning, $"Macro definition '{nativeName}' could not be resolved to a supported expression. Generated bindings may be incomplete.", varDecl);
                    return;
                }
            }

            var openedOutputBuilder = false;
            var className = GetClass(name);

            if (_outputBuilder is null)
            {
                openedOutputBuilder = true;

                if (IsUnsafe(varDecl, type) && (!varDecl.HasInit || !IsStmtAsWritten<StringLiteral>(varDecl.Init, out _, removeParens: true)))
                {
                    _topLevelClassIsUnsafe[className] = true;
                }
            }

            var typeName = GetTargetTypeName(varDecl, out var nativeTypeName);

            if (isMacroDefinitionRecord)
            {
                var nativeTypeNameBuilder = new StringBuilder("#define");

                _ = nativeTypeNameBuilder.Append(' ');
                _ = nativeTypeNameBuilder.Append(nativeName);
                _ = nativeTypeNameBuilder.Append(' ');

                var macroValue = GetSourceRangeContents(varDecl.TranslationUnit.Handle, varDecl.Init.Extent);
                _ = nativeTypeNameBuilder.Append(macroValue.Replace(@"\\", "\\", StringComparison.Ordinal));

                nativeTypeName = nativeTypeNameBuilder.ToString();
            }

            var kind = ValueKind.Unknown;
            var flags = ValueFlags.None;

            if (varDecl.HasInit)
            {
                flags |= ValueFlags.Initializer;
            }

            if (type.IsLocalConstQualified || isMacroDefinitionRecord || IsTypeConstantOrIncompleteArray(varDecl))
            {
                flags |= ValueFlags.Constant;
            }

            if (IsStmtAsWritten<StringLiteral>(varDecl.Init, out var stringLiteral, removeParens: true))
            {
                kind = ValueKind.String;

                switch (stringLiteral.Kind)
                {
                    case CX_SLK_Ordinary:
                    case CX_SLK_UTF8:
                    {
                        typeName = (flags & ValueFlags.Constant) != 0 ? "ReadOnlySpan<byte>" : "byte[]";
                        break;
                    }

                    case CX_SLK_Wide:
                    {
                        if (_config.GenerateUnixTypes)
                        {
                            goto case CX_SLK_UTF32;
                        }
                        else
                        {
                            goto case CX_SLK_UTF16;
                        }
                    }

                    case CX_SLK_UTF16:
                    {
                        kind = ValueKind.Primitive;
                        typeName = "string";
                        break;
                    }

                    case CX_SLK_UTF32:
                    {
                        typeName = (!_config.GenerateCompatibleCode && (flags & ValueFlags.Constant) != 0) ? "ReadOnlySpan<uint>" : "uint[]";
                        break;
                    }

                    default:
                    {
                        AddDiagnostic(DiagnosticLevel.Error, $"Unsupported string literal kind: '{stringLiteral.Kind}'. Generated bindings may be incomplete.", stringLiteral);
                        break;
                    }
                }
            }
            else if (IsPrimitiveValue(varDecl, type))
            {
                kind = ValueKind.Primitive;

                if ((flags & ValueFlags.Constant) != 0 && !IsConstant(typeName, varDecl.Init))
                {
                    flags |= ValueFlags.Copy;
                }
                else if (_config.WithTransparentStructs.TryGetValue(typeName, out var transparentStruct))
                {
                    typeName = transparentStruct.Name;
                }
            }
            else if ((varDecl.StorageClass == CX_SC_Static) || openedOutputBuilder)
            {
                kind = ValueKind.Unmanaged;

                if (varDecl.HasInit)
                {
                    if ((varDecl.Init is CXXConstructExpr cxxConstructExpr) && cxxConstructExpr.Constructor.IsCopyConstructor)
                    {
                        if (cxxConstructExpr.Args[0] is CXXUuidofExpr)
                        {
                            // It's easiest just to let _uuidsToGenerate handle it
                            return;
                        }
                        flags |= ValueFlags.Copy;
                    }
                }

                if (IsType<ArrayType>(varDecl, type, out var arrayType))
                {
                    flags |= ValueFlags.Array;

                    if (!_config.GenerateUnmanagedConstants)
                    {
                        do
                        {
                            typeName += "[]";
                            arrayType = arrayType.ElementType as ArrayType;
                        }
                        while (arrayType is not null);
                    }
                }
            }

            if (typeName.Equals("Guid", StringComparison.Ordinal))
            {
                _ = _generatedUuids.Add(name);
            }

            var desc = new ValueDesc {
                AccessSpecifier = accessSpecifier,
                TypeName = typeName,
                EscapedName = escapedName,
                NativeTypeName = nativeTypeName,
                Kind = kind,
                Flags = flags,
                Location = varDecl.Location,
                WriteCustomAttrs = static context => {
                    (var varDecl, var generator) = ((VarDecl, PInvokeGenerator))context;

                    generator.WithAttributes(varDecl);
                    generator.WithUsings(varDecl);
                },
                CustomAttrGeneratorData = (varDecl, this),
            };

            if (openedOutputBuilder)
            {
                StartUsingOutputBuilder(className);
                Debug.Assert(_outputBuilder is not null);

                if ((kind == ValueKind.String) && typeName.StartsWith("ReadOnlySpan<", StringComparison.Ordinal))
                {
                    _outputBuilder.EmitSystemSupport();
                }
            }

            Debug.Assert(_outputBuilder is not null);

            _outputBuilder.BeginValue(in desc);

            var currentContext = _context.Last;
            Debug.Assert(currentContext is not null);
            currentContext.Value = (currentContext.Value.Cursor, desc);

            if (varDecl.HasInit)
            {
                var dereference = IsType<PointerType>(varDecl, type, out var pointerType) &&
                                  IsType<FunctionType>(varDecl, pointerType.PointeeType) &&
                                  isMacroDefinitionRecord;

                if (dereference)
                {
                    _outputBuilder.BeginDereference();
                }

                Visit(varDecl.Init);

                if (dereference)
                {
                    _outputBuilder.EndDereference();
                }
            }

            _outputBuilder.EndValue(in desc);

            if (openedOutputBuilder)
            {
                StopUsingOutputBuilder();
            }
            else
            {
                _outputBuilder.WriteDivider();
            }
        }
        else if (IsPrevContextDecl<FunctionDecl>(out _, out _))
        {
            // This should be handled in the function body as part of a DeclStmt
        }
        else
        {
            _ = IsPrevContextDecl<Decl>(out var previousContext, out _);
            AddDiagnostic(DiagnosticLevel.Error, $"Unsupported variable declaration parent: '{previousContext?.CursorKindSpelling}'. Generated bindings may be incomplete.", previousContext);
        }

        void ForDeclStmt(VarDecl varDecl, DeclStmt declStmt)
        {
            var outputBuilder = StartCSharpCode();
            var name = GetRemappedCursorName(varDecl);
            var escapedName = EscapeName(name);

            if (varDecl == declStmt.Decls[0])
            {
                var type = varDecl.Type;
                var typeName = GetRemappedTypeName(varDecl, context: null, type, out _);

                outputBuilder.Write(typeName);

                if (IsType<ArrayType>(varDecl, type))
                {
                    outputBuilder.Write("[]");
                }

                outputBuilder.Write(' ');
            }

            outputBuilder.Write(escapedName);

            if (varDecl.HasInit)
            {
                outputBuilder.Write(" = ");
                Visit(varDecl.Init);
            }

            StopCSharpCode();
        }
    }
}
