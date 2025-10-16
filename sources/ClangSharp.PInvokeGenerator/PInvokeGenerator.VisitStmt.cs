// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using ClangSharp.Abstractions;
using ClangSharp.CSharp;
using static ClangSharp.Interop.CXBinaryOperatorKind;
using static ClangSharp.Interop.CX_CastKind;
using static ClangSharp.Interop.CX_CharacterKind;
using static ClangSharp.Interop.CX_StmtClass;
using static ClangSharp.Interop.CX_StringKind;
using static ClangSharp.Interop.CX_UnaryExprOrTypeTrait;
using static ClangSharp.Interop.CXUnaryOperatorKind;
using static ClangSharp.Interop.CXEvalResultKind;
using static ClangSharp.Interop.CXTypeKind;

namespace ClangSharp;

public partial class PInvokeGenerator
{
    private void VisitArraySubscriptExpr(ArraySubscriptExpr arraySubscriptExpr)
    {
        var outputBuilder = StartCSharpCode();
        Visit(arraySubscriptExpr.Base);
        outputBuilder.Write('[');
        Visit(arraySubscriptExpr.Idx);
        outputBuilder.Write(']');
        StopCSharpCode();
    }

    private void VisitBinaryOperator(BinaryOperator binaryOperator)
    {
        var outputBuilder = StartCSharpCode();
        Visit(binaryOperator.LHS);
        outputBuilder.Write(' ');
        outputBuilder.Write(binaryOperator.OpcodeStr);
        outputBuilder.Write(' ');

        if (binaryOperator.IsShiftOp || binaryOperator.IsShiftAssignOp)
        {
            // RHS of shift operation in C# must be an int

            // Negative shifts are undefined behavior in C/C++, but still needs to have a compilable output
            var isNegated = binaryOperator.RHS is UnaryOperator { Opcode: CXUnaryOperator_Minus };
            var sign = isNegated ? -1 : 1;

            var rhs = isNegated ? ((UnaryOperator)binaryOperator.RHS).SubExpr : binaryOperator.RHS;
            switch (rhs)
            {
                case IntegerLiteral literal:
                {
                    var value = sign * literal.Value;
                    if (value is > int.MaxValue or < int.MinValue)
                    {
                        // Literal is in int32 range
                        outputBuilder.Write("(int)(");
                        outputBuilder.Write(sign * literal.Value);
                        outputBuilder.Write(")");
                    }
                    else
                    {
                        // Literal is not in int32 range
                        outputBuilder.Write(sign * literal.Value);
                    }

                    break;
                }
                // Already the correct type or implicitly castable
                case { Type.Kind: CXType_Int or CXType_UShort or CXType_Short or CXType_Char_U or CXType_Char_S or CXType_UChar or CXType_SChar }:
                case { Type.Kind: CXType_Long } when _config is { GenerateUnixTypes: false }:
                case { Type.Kind: CXType_Char16 } when _config is { GenerateDisableRuntimeMarshalling: false }:
                case { Type.Kind: CXType_WChar } when _config is { GenerateDisableRuntimeMarshalling: false, GenerateUnixTypes: false }:
                {
                    Visit(binaryOperator.RHS);
                    break;
                }
                // Fallback
                default:
                {
                    outputBuilder.Write("(int)(");
                    Visit(binaryOperator.RHS);
                    outputBuilder.Write(")");
                    break;
                }
            }
        }
        else
        {
            Visit(binaryOperator.RHS);
        }

        StopCSharpCode();
    }

    private void VisitBreakStmt(BreakStmt breakStmt)
    {
        StartCSharpCode().Write("break");
        StopCSharpCode();
    }

    private void VisitBody(Stmt stmt)
    {
        var outputBuilder = StartCSharpCode();
        if (stmt is CompoundStmt)
        {
            Visit(stmt);
        }
        else
        {
            outputBuilder.WriteBlockStart();
            outputBuilder.WriteIndentation();
            outputBuilder.NeedsSemicolon = true;
            outputBuilder.NeedsNewline = true;

            Visit(stmt);

            outputBuilder.WriteSemicolonIfNeeded();
            outputBuilder.WriteNewlineIfNeeded();
            outputBuilder.WriteBlockEnd();
        }

        StopCSharpCode();
    }

    private void VisitCallExpr(CallExpr callExpr)
    {
        var outputBuilder = StartCSharpCode();
        var calleeDecl = callExpr.CalleeDecl;

        if ((callExpr.DirectCallee is not null) && callExpr.DirectCallee.IsInlined)
        {
            var evalResult = callExpr.Handle.Evaluate;
            var type = callExpr.Type;

            switch (evalResult.Kind)
            {
                case CXEval_Int:
                {
                    if (type.CanonicalType.Handle.IsUnsigned)
                    {
                        outputBuilder.Write(evalResult.AsUnsigned);
                    }
                    else
                    {
                        outputBuilder.Write(evalResult.AsLongLong);
                    }

                    StopCSharpCode();
                    return;
                }

                case CXEval_Float:
                {
                    if (type.CanonicalType.Kind == CXType_Float)
                    {
                        outputBuilder.Write((float)evalResult.AsDouble);
                    }
                    else
                    {
                        outputBuilder.Write(evalResult.AsDouble);
                    }

                    StopCSharpCode();
                    return;
                }

                case CXEval_StrLiteral:
                {
                    AddDiagnostic(DiagnosticLevel.Info, "Possible string constant");
                    break;
                }
            }
        }

        var functionName = "";
        var isUnusedValue = false;
        var parentName = "";
        var parentNamespace = "";

        if (calleeDecl is FunctionDecl functionDecl)
        {
            functionName = functionDecl.Name;

            if (functionDecl.IsStatic && (functionDecl.Parent is CXXRecordDecl parent))
            {
                parentName = GetRemappedCursorName(parent);
                parentNamespace = GetNamespace(parentName, parent);

                if (IsPrevContextDecl<FunctionDecl>(out var functionDeclContext, out _) && (functionDeclContext.Parent is CXXRecordDecl parentContext))
                {
                    var contextName = GetRemappedCursorName(parentContext);

                    if (string.Equals(parentName, contextName, StringComparison.Ordinal))
                    {
                        parentName = "";
                        parentNamespace = "";
                    }
                }
            }
        }

        if (!IsTypeVoid(callExpr, callExpr.Type))
        {
            isUnusedValue = IsPrevContextStmt<CompoundStmt>(out _, out _)
                         || IsPrevContextStmt<IfStmt>(out _, out _);

            if (functionName is "memcpy" or "memset")
            {
                isUnusedValue = false;
            }
        }

        if (isUnusedValue)
        {
            outputBuilder.Write("_ = ");
        }

        if (!string.IsNullOrWhiteSpace(parentName))
        {
            AddUsingDirective(outputBuilder, parentNamespace);

            outputBuilder.Write(parentName);
            outputBuilder.Write('.');
        }

        if (calleeDecl is null)
        {
            Visit(callExpr.Callee);
            VisitArgs(callExpr);
        }
        else if (calleeDecl is FieldDecl)
        {
            Visit(callExpr.Callee);
            VisitArgs(callExpr);
        }
        else if (calleeDecl is FunctionDecl)
        {
            switch (functionName)
            {
                case "memcpy":
                {
                    var args = callExpr.Args;

                    if (!Config.GenerateCompatibleCode)
                    {
                        outputBuilder.AddUsingDirective("System.Runtime.InteropServices");
                        outputBuilder.Write("NativeMemory.Copy");

                        if (args.Count == 3)
                        {
                            // Swap the operands around:
                            // * NativeMemory.Copy takes: source, dest, count
                            // * memcpy takes: dest, source, count
                            args = [args[1], args[0], args[2]];
                        }
                    }
                    else
                    {
                        outputBuilder.AddUsingDirective("System.Runtime.CompilerServices");
                        outputBuilder.Write("Unsafe.CopyBlockUnaligned");
                    }

                    VisitArgs(callExpr, args);
                    break;
                }

                case "memset":
                {
                    NamedDecl? namedDecl = null;

                    var args = callExpr.Args;

                    if (callExpr.NumArgs == 3)
                    {
                        if (IsStmtAsWritten<IntegerLiteral>(callExpr.Args[1], out var integerLiteralExpr, removeParens: true) && (integerLiteralExpr.Value == 0) &&
                            IsStmtAsWritten<UnaryExprOrTypeTraitExpr>(callExpr.Args[2], out var unaryExprOrTypeTraitExpr, removeParens: true) && (unaryExprOrTypeTraitExpr.Kind == CX_UETT_SizeOf))
                        {
                            var typeOfArgument = unaryExprOrTypeTraitExpr.TypeOfArgument;
                            var expr = callExpr.Args[0];

                            if (IsStmtAsWritten<UnaryOperator>(expr, out var unaryOperator, removeParens: true) && (unaryOperator.Opcode == CXUnaryOperator_AddrOf))
                            {
                                expr = unaryOperator.SubExpr;
                            }

                            if (IsStmtAsWritten<DeclRefExpr>(expr, out var declRefExpr, removeParens: true) && (typeOfArgument.CanonicalType == declRefExpr.Type.CanonicalType))
                            {
                                namedDecl = declRefExpr.Decl;
                            }
                            else if (IsStmtAsWritten<MemberExpr>(expr, out var memberExpr, removeParens: true) && (typeOfArgument.CanonicalType == memberExpr.Type.CanonicalType))
                            {
                                namedDecl = memberExpr.MemberDecl;
                            }
                        }

                        if (namedDecl is not null)
                        {
                            outputBuilder.Write(GetRemappedCursorName(namedDecl));
                            outputBuilder.Write(" = default");
                            break;
                        }

                        if (!Config.GenerateCompatibleCode)
                        {
                            args = [args[0], args[2], args[1]];
                        }
                    }

                    if (!Config.GenerateCompatibleCode)
                    {
                        outputBuilder.AddUsingDirective("System.Runtime.InteropServices");
                        outputBuilder.Write("NativeMemory.Fill");
                    }
                    else
                    {
                        outputBuilder.AddUsingDirective("System.Runtime.CompilerServices");
                        outputBuilder.Write("Unsafe.InitBlockUnaligned");
                    }

                    VisitArgs(callExpr, args);
                    break;
                }

                case "wcslen":
                {
                    if (_config.GenerateCompatibleCode)
                    {
                        goto default;
                    }

                    outputBuilder.AddUsingDirective("System.Runtime.InteropServices");
                    outputBuilder.Write("MemoryMarshal.CreateReadOnlySpanFromNullTerminated");

                    VisitArgs(callExpr);
                    outputBuilder.Write(".Length");
                    break;
                }

                default:
                {
                    Visit(callExpr.Callee);
                    VisitArgs(callExpr);
                    break;
                }
            }
        }
        else if (calleeDecl is ParmVarDecl)
        {
            Visit(callExpr.Callee);
            VisitArgs(callExpr);
        }
        else if (calleeDecl is VarDecl)
        {
            Visit(callExpr.Callee);
            VisitArgs(callExpr);
        }
        else
        {
            AddDiagnostic(DiagnosticLevel.Error, $"Unsupported callee declaration: '{calleeDecl.DeclKindName}'. Generated bindings may be incomplete.", calleeDecl);
        }

        void VisitArgs(CallExpr callExpr, IReadOnlyList<Expr>? args = null)
        {
            var callExprType = (callExpr.Callee is MemberExpr memberExpr)
                             ? memberExpr.MemberDecl.Type
                             : callExpr.Callee.Type;

            if (IsType<PointerType>(callExpr, callExprType, out var pointerType))
            {
                callExprType = pointerType.PointeeType;
            }

            outputBuilder.Write('(');

            args ??= callExpr.Args;
            var needsComma = false;

            var paramCount = IsType<FunctionProtoType>(callExpr, callExprType, out var functionProtoType)
                           ? (int)functionProtoType.NumParams
                           : args.Count;

            for (var i = 0; i < args.Count; i++)
            {
                var arg = args[i];

                if (needsComma && (arg is not CXXDefaultArgExpr))
                {
                    outputBuilder.Write(", ");
                }

                if ((functionProtoType is not null) && (i < paramCount))
                {
                    if (IsType<ReferenceType>(callExpr, functionProtoType.ParamTypes[i], out var referenceType))
                    {
                        if (IsStmtAsWritten<UnaryOperator>(arg, out var unaryOperator, removeParens: true) && (unaryOperator.Opcode == CXUnaryOperator_Deref))
                        {
                            var subExpr = unaryOperator.SubExpr;

                            if (subExpr is CXXThisExpr)
                            {
                                var referenceTypeName = GetTypeName(callExpr, context: null, type: referenceType, ignoreTransparentStructsWhereRequired: true, isTemplate: false, nativeTypeName: out _);

                                outputBuilder.AddUsingDirective("System.Runtime.CompilerServices");
                                outputBuilder.Write('(');
                                outputBuilder.Write(referenceTypeName);
                                outputBuilder.Write(")Unsafe.AsPointer(");

                                if (referenceType.IsLocalConstQualified)
                                {
                                    if (!_config.GenerateLatestCode)
                                    {
                                        outputBuilder.Write("ref Unsafe.AsRef(");
                                    }

                                    outputBuilder.Write("in this");

                                    if (!_config.GenerateLatestCode)
                                    {
                                        outputBuilder.Write(')');
                                    }
                                }
                                else
                                {
                                    outputBuilder.Write("ref this");
                                }
                                outputBuilder.Write(')');

                                needsComma = true;
                                continue;
                            }

                            arg = subExpr;
                        }
                        else if (IsStmtAsWritten<DeclRefExpr>(arg, out var declRefExpr, removeParens: true))
                        {
                            if (!IsTypePointerOrReference(declRefExpr.Decl))
                            {
                                outputBuilder.Write('&');
                            }
                        }
                        else if (!IsTypePointerOrReference(arg))
                        {
                            outputBuilder.Write('&');
                        }
                    }
                    else if (IsStmtAsWritten<CXXThisExpr>(arg, out _) && (functionName == "memcpy"))
                    {
                        outputBuilder.AddUsingDirective("System.Runtime.CompilerServices");
                        outputBuilder.Write("Unsafe.AsPointer(");

                        if (functionProtoType.ParamTypes[i].IsLocalConstQualified)
                        {
                            if (!_config.GenerateLatestCode)
                            {
                                outputBuilder.Write("ref Unsafe.AsRef(");
                            }

                            outputBuilder.Write("in this");

                            if (!_config.GenerateLatestCode)
                            {
                                outputBuilder.Write(')');
                            }
                        }
                        else
                        {
                            outputBuilder.Write("ref this");
                        }
                        outputBuilder.Write(')');

                        needsComma = true;
                        continue;
                    }
                }

                Visit(arg);

                if (arg is not CXXDefaultArgExpr)
                {
                    needsComma = true;
                }
            }

            outputBuilder.Write(')');
        }

        StopCSharpCode();
    }

    private void VisitCaseStmt(CaseStmt caseStmt)
    {
        var outputBuilder = StartCSharpCode();
        outputBuilder.Write("case ");
        Visit(caseStmt.LHS);
        outputBuilder.WriteLine(':');

        if (caseStmt.SubStmt is SwitchCase)
        {
            outputBuilder.WriteIndentation();
            Visit(caseStmt.SubStmt);
        }
        else
        {
            VisitBody(caseStmt.SubStmt);
        }

        StopCSharpCode();
    }

    private void VisitCharacterLiteral(CharacterLiteral characterLiteral)
    {
        var outputBuilder = StartCSharpCode();
        switch (characterLiteral.Kind)
        {
            case CX_CLK_Ascii:
            case CX_CLK_UTF8:
            {
                if (characterLiteral.Value > ushort.MaxValue)
                {
                    outputBuilder.Write("0x");
                    outputBuilder.Write(characterLiteral.Value.ToString("X8", CultureInfo.InvariantCulture));
                }
                else if (characterLiteral.Value > byte.MaxValue)
                {
                    outputBuilder.Write("0x");
                    outputBuilder.Write(characterLiteral.Value.ToString("X4", CultureInfo.InvariantCulture));
                }
                else
                {
                    var castType = "";
                    var targetTypeName = "";
                    var targetTypeNumBits = 0;

                    if (IsPrevContextStmt<ImplicitCastExpr>(out var implicitCastExpr, out _))
                    {
                        // C# characters are effectively `ushort` while C defaults to "char" which is
                        // most typically `sbyte`. Due to this we need to insert a correct implicit
                        // cast to ensure things are correctly handled here.

                        var targetType = implicitCastExpr.Type;
                        targetTypeName = GetRemappedTypeName(implicitCastExpr, context: null, targetType, out _, skipUsing: true);
                        targetTypeNumBits = targetType.Handle.NumBits;
                    }
                    else if (IsPrevContextStmt<BinaryOperator>(out var binaryOperator, out _))
                    {
                        var targetType = binaryOperator.Type;
                        targetTypeName = GetRemappedTypeName(implicitCastExpr, context: null, targetType, out _, skipUsing: true);
                        targetTypeNumBits = targetType.Handle.NumBits;
                    }
                    else if (PreviousContext.Cursor is VarDecl varDecl)
                    {
                        var targetType = varDecl.Type;
                        targetTypeName = GetRemappedTypeName(varDecl, context: null, targetType, out _, skipUsing: true);
                        targetTypeNumBits = targetType.Handle.NumBits;
                    }

                    if (!string.IsNullOrEmpty(targetTypeName))
                    {
                        if (!IsUnsigned(targetTypeName))
                        {
                            castType = "sbyte";
                        }
                        else if (targetTypeNumBits < 16)
                        {
                            castType = "byte";
                        }

                        outputBuilder.Write('(');
                        outputBuilder.Write(castType);
                        outputBuilder.Write(")(");
                    }

                    outputBuilder.Write('\'');
                    outputBuilder.Write(EscapeCharacter((char)characterLiteral.Value));
                    outputBuilder.Write('\'');

                    if (!string.IsNullOrEmpty(castType))
                    {
                        outputBuilder.Write(')');
                    }
                }
                break;
            }

            case CX_CLK_Wide:
            {
                if (_config.GenerateUnixTypes)
                {
                    goto default;
                }

                goto case CX_CLK_UTF16;
            }

            case CX_CLK_UTF16:
            {
                if (characterLiteral.Value > ushort.MaxValue)
                {
                    outputBuilder.Write("0x");
                    outputBuilder.Write(characterLiteral.Value.ToString("X8", CultureInfo.InvariantCulture));
                }
                else
                {
                    outputBuilder.Write('\'');
                    outputBuilder.Write(EscapeCharacter((char)characterLiteral.Value));
                    outputBuilder.Write('\'');
                }
                break;
            }

            case CX_CLK_UTF32:
            {
                outputBuilder.Write("0x");
                outputBuilder.Write(characterLiteral.Value.ToString("X8", CultureInfo.InvariantCulture));
                break;
            }

            default:
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported character literal kind: '{characterLiteral.Kind}'. Generated bindings may be incomplete.", characterLiteral);
                break;
            }
        }

        StopCSharpCode();
    }

    private void VisitCompoundStmt(CompoundStmt compoundStmt)
    {
        var outputBuilder = StartCSharpCode();
        outputBuilder.WriteBlockStart();

        VisitStmts(compoundStmt.Body);

        outputBuilder.WriteSemicolonIfNeeded();
        outputBuilder.WriteNewlineIfNeeded();
        outputBuilder.WriteBlockEnd();
        StopCSharpCode();
    }

    private void VisitConditionalOperator(ConditionalOperator conditionalOperator)
    {
        var outputBuilder = StartCSharpCode();
        Visit(conditionalOperator.Cond);
        outputBuilder.Write(" ? ");
        Visit(conditionalOperator.TrueExpr);
        outputBuilder.Write(" : ");
        Visit(conditionalOperator.FalseExpr);
        StopCSharpCode();
    }

    private void VisitContinueStmt(ContinueStmt continueStmt)
    {
        var outputBuilder = StartCSharpCode();
        outputBuilder.Write("continue");
        StopCSharpCode();
    }

    private void VisitCXXBoolLiteralExpr(CXXBoolLiteralExpr cxxBoolLiteralExpr)
    {
        var outputBuilder = StartCSharpCode();
        outputBuilder.Write(cxxBoolLiteralExpr.ValueString);
        StopCSharpCode();
    }

    private void VisitCXXConstCastExpr(CXXConstCastExpr cxxConstCastExpr) =>
        // C# doesn't have a concept of const pointers so
        // ignore rather than adding a cast from T* to T*

        Visit(cxxConstCastExpr.SubExprAsWritten);

    private void VisitCXXConstructExpr(CXXConstructExpr cxxConstructExpr)
    {
        var outputBuilder = StartCSharpCode();
        var isCopyOrMoveConstructor = cxxConstructExpr.Constructor is { IsCopyConstructor: true } or { IsMoveConstructor: true };
        var isUnmanagedConstant = false;

        if (IsPrevContextDecl<VarDecl>(out _, out var userData))
        {
            isUnmanagedConstant = (userData is ValueDesc valueDesc) && (valueDesc.Kind == ValueKind.Unmanaged) && valueDesc.IsConstant && valueDesc.IsCopy;
        }

        if (!isCopyOrMoveConstructor)
        {
            outputBuilder.Write("new ");

            var constructorName = GetRemappedCursorName(cxxConstructExpr.Constructor);

            outputBuilder.Write(constructorName);
            outputBuilder.Write('(');
        }

        var args = cxxConstructExpr.Args;

        if (args.Count != 0)
        {
            if (isUnmanagedConstant)
            {
                outputBuilder.Write("ref ");
            }

            var needsComma = false;

            for (var i = 0; i < args.Count; i++)
            {
                var arg = args[i];

                if (needsComma && (arg is not CXXDefaultArgExpr))
                {
                    outputBuilder.Write(", ");
                }

                Visit(arg);

                if (arg is not CXXDefaultArgExpr)
                {
                    needsComma = true;
                }
            }
        }

        if (!isCopyOrMoveConstructor)
        {
            outputBuilder.Write(')');
        }

        StopCSharpCode();
    }

    private static void VisitCXXDefaultArgExpr(CXXDefaultArgExpr cxxDefaultArgExpr)
    {
        // Nothing to handle as these just represent optional parameters that
        // aren't properly defined
    }

    private void VisitCXXDependentScopeMemberExpr(CXXDependentScopeMemberExpr cxxDependentScopeMemberExpr)
    {
        var outputBuilder = StartCSharpCode();

        if (!cxxDependentScopeMemberExpr.IsImplicitAccess)
        {
            var @base = cxxDependentScopeMemberExpr.Base;
            Debug.Assert(@base is not null);
            Visit(@base);

            if (@base is CXXThisExpr)
            {
                outputBuilder.Write('.');
            }
            else if (@base is DeclRefExpr declRefExpr)
            {
                if (IsTypePointerOrReference(declRefExpr.Decl))
                {
                    outputBuilder.Write("->");
                }
                else
                {
                    outputBuilder.Write('.');
                }
            }
            else if (IsTypePointerOrReference(@base))
            {
                outputBuilder.Write("->");
            }
            else
            {
                outputBuilder.Write('.');
            }
        }

        outputBuilder.Write(GetRemappedName(cxxDependentScopeMemberExpr.MemberName, cxxDependentScopeMemberExpr, tryRemapOperatorName: false, out _, skipUsing: true));
        StopCSharpCode();
    }

    private void VisitCXXFunctionalCastExpr(CXXFunctionalCastExpr cxxFunctionalCastExpr)
    {
        if (cxxFunctionalCastExpr.SubExpr is CXXConstructExpr cxxConstructExpr)
        {
            Visit(cxxConstructExpr);
        }
        else
        {
            VisitExplicitCastExpr(cxxFunctionalCastExpr);
        }
    }

    private void VisitCXXNewExpr(CXXNewExpr cxxNewExpr)
    {
        var outputBuilder = StartCSharpCode();
        outputBuilder.Write("cxx_new<");


        var allocatedTypeName = GetRemappedTypeName(cxxNewExpr, null, cxxNewExpr.AllocatedType, out _);
        outputBuilder.Write(allocatedTypeName);

        outputBuilder.Write(">(sizeof(");
        outputBuilder.Write(allocatedTypeName);
        outputBuilder.Write("))");

        if (cxxNewExpr.ConstructExpr is not null)
        {
            outputBuilder.WriteSemicolon();
            outputBuilder.WriteNewline();

            outputBuilder.WriteIndented(" = ");
            VisitCXXConstructExpr(cxxNewExpr.ConstructExpr);
        }
        StopCSharpCode();
    }

    private void VisitCXXNullPtrLiteralExpr(CXXNullPtrLiteralExpr cxxNullPtrLiteralExpr)
    {
        var outputBuilder = StartCSharpCode();
        outputBuilder.Write("null");
        StopCSharpCode();
    }

    private void VisitCXXOperatorCallExpr(CXXOperatorCallExpr cxxOperatorCallExpr)
    {
        var outputBuilder = StartCSharpCode();
        var calleeDecl = cxxOperatorCallExpr.CalleeDecl;

        if (calleeDecl is FunctionDecl functionDecl)
        {
            var functionDeclName = GetCursorName(functionDecl);
            var args = cxxOperatorCallExpr.Args;

            if (IsEnumOperator(functionDecl, functionDeclName))
            {
                switch (functionDeclName)
                {
                    case "operator|":
                    case "operator|=":
                    case "operator&":
                    case "operator&=":
                    case "operator^":
                    case "operator^=":
                    {
                        Visit(args[0]);
                        outputBuilder.Write(' ');
                        outputBuilder.Write(functionDeclName.AsSpan()[8..]);
                        outputBuilder.Write(' ');
                        Visit(args[1]);
                        StopCSharpCode();
                        return;
                    }

                    case "operator~":
                    {
                        outputBuilder.Write(functionDeclName.AsSpan()[8..]);
                        Visit(args[0]);
                        StopCSharpCode();
                        return;
                    }

                    default:
                    {
                        break;
                    }
                }
            }

            var name = GetRemappedCursorName(functionDecl);
            var firstIndex = 0;

            if (functionDecl.DeclContext is CXXRecordDecl)
            {
                Visit(cxxOperatorCallExpr.Args[0]);
                firstIndex++;
            }

            switch (name)
            {
                case "operator=":
                {
                    if (!functionDecl.IsDefaulted)
                    {
                        goto default;
                    }

                    var expr = args[firstIndex];

                    if (firstIndex != 0)
                    {
                        outputBuilder.Write(" = ");

                        if (!IsTypePointerOrReference(args[firstIndex - 1]))
                        {
                            if (IsType<ReferenceType>(expr))
                            {
                                outputBuilder.Write('*');
                            }
                            else if (IsStmtAsWritten<DeclRefExpr>(expr, out var declRefExpr, removeParens: true))
                            {
                                if (IsTypePointerOrReference(declRefExpr.Decl))
                                {
                                    outputBuilder.Write('*');
                                }
                            }
                        }
                    }

                    Visit(expr);
                    break;
                }

                default:
                {
                    if (firstIndex != 0)
                    {
                        outputBuilder.Write('.');
                    }

                    outputBuilder.Write(name);

                    outputBuilder.Write('(');

                    if (args.Count > firstIndex)
                    {
                        Visit(args[firstIndex]);

                        for (var i = firstIndex + 1; i < args.Count; i++)
                        {
                            outputBuilder.Write(", ");
                            Visit(args[i]);
                        }
                    }

                    outputBuilder.Write(')');
                    break;
                }
            }
        }
        else
        {
            AddDiagnostic(DiagnosticLevel.Error, $"Unsupported callee declaration: '{calleeDecl.DeclKindName}'. Generated bindings may be incomplete.", calleeDecl);
        }

        StopCSharpCode();
    }

    private static void VisitCXXPseudoDestructorExpr(CXXPseudoDestructorExpr cxxPseudoDestructorExpr)
    {
        // Nothing to generate for pseudo destructors
    }

    private void VisitCXXTemporaryObjectExpr(CXXTemporaryObjectExpr cxxTemporaryObjectExpr) => VisitCXXConstructExpr(cxxTemporaryObjectExpr);

    private void VisitCXXThisExpr(CXXThisExpr cxxThisExpr)
    {
        var outputBuilder = StartCSharpCode();
        outputBuilder.Write("this");
        StopCSharpCode();
    }

    private void VisitCXXUnresolvedConstructExpr(CXXUnresolvedConstructExpr cxxUnresolvedConstructExpr)
    {
        var outputBuilder = StartCSharpCode();
        outputBuilder.Write("new ");

        var constructorName = GetRemappedTypeName(cxxUnresolvedConstructExpr, null, cxxUnresolvedConstructExpr.TypeAsWritten, out _);
        outputBuilder.Write(constructorName);

        outputBuilder.Write('(');
        var args = cxxUnresolvedConstructExpr.Args;

        if (args.Count != 0)
        {
            Visit(args[0]);

            for (var i = 1; i < args.Count; i++)
            {
                outputBuilder.Write(", ");
                Visit(args[i]);
            }
        }

        outputBuilder.Write(')');
        StopCSharpCode();
    }

    private void VisitCXXUuidofExpr(CXXUuidofExpr cxxUuidofExpr)
    {
        var outputBuilder = StartCSharpCode();
        outputBuilder.Write("typeof(");

        Type type;

        if (cxxUuidofExpr.IsTypeOperand)
        {
            type = cxxUuidofExpr.TypeOperand;
        }
        else
        {
            var exprOperand = cxxUuidofExpr.ExprOperand;
            Debug.Assert(exprOperand is not null);
            type = exprOperand.Type;
        }

        var typeName = GetRemappedTypeName(cxxUuidofExpr, context: null, type, out _);
        outputBuilder.Write(typeName);

        outputBuilder.Write(").GUID");

        StopCSharpCode();
    }

    private void VisitDeclRefExpr(DeclRefExpr declRefExpr)
    {
        var outputBuilder = StartCSharpCode();

        var name = GetRemappedCursorName(declRefExpr.Decl, out _, skipUsing: true);
        var escapedName = EscapeName(name);

        if (declRefExpr.Decl is EnumConstantDecl enumConstantDecl)
        {
            if ((declRefExpr.DeclContext != enumConstantDecl.DeclContext) && (enumConstantDecl.DeclContext is NamedDecl namedDecl))
            {
                var enumName = GetRemappedCursorName(namedDecl, out _, skipUsing: true);

                if (!_config.DontUseUsingStaticsForEnums)
                {
                    if (enumName.StartsWith("__AnonymousEnum_", StringComparison.Ordinal))
                    {
                        var className = GetClass(enumName);

                        if ((outputBuilder.Name != className) && (namedDecl.Parent is not TagDecl))
                        {
                            outputBuilder.AddUsingDirective($"static {GetNamespace(enumName, namedDecl)}.{className}");
                        }
                    }
                    else
                    {
                        outputBuilder.AddUsingDirective($"static {GetNamespace(enumName, namedDecl)}.{enumName}");
                    }
                }
                else
                {
                    outputBuilder.Write(enumName);
                    outputBuilder.Write(".");
                }
            }
        }
        else
        {
            if (TryGetClass(name, out var className))
            {
                if (TryGetNamespace(className, out var namespaceName) && ((_currentNamespace != namespaceName) || (_currentClass != className)))
                {
                    outputBuilder.AddUsingDirective($"static {namespaceName}.{className}");
                }
            }
            else if (TryGetNamespace(name, out var namespaceName) && (_currentNamespace != namespaceName))
            {
                outputBuilder.AddUsingDirective(namespaceName);
            }

            if (declRefExpr.Decl is FunctionDecl)
            {
                escapedName = EscapeAndStripMethodName(name);
            }
        }

        outputBuilder.Write(escapedName);

        StopCSharpCode();
    }

    private void VisitDeclStmt(DeclStmt declStmt)
    {
        var outputBuilder = StartCSharpCode();

        if (declStmt.IsSingleDecl)
        {
            var singleDecl = declStmt.SingleDecl;
            Debug.Assert(singleDecl is not null);
            Visit(singleDecl);
        }
        else
        {
            Visit(declStmt.Decls[0]);

            foreach (var decl in declStmt.Decls.Skip(1))
            {
                outputBuilder.Write(", ");
                Visit(decl);
            }
        }

        StopCSharpCode();
    }

    private void VisitDefaultStmt(DefaultStmt defaultStmt)
    {
        var outputBuilder = StartCSharpCode();
        outputBuilder.WriteLine("default:");

        if (defaultStmt.SubStmt is SwitchCase)
        {
            outputBuilder.WriteIndentation();
            Visit(defaultStmt.SubStmt);
        }
        else
        {
            VisitBody(defaultStmt.SubStmt);
        }

        StopCSharpCode();
    }

    private void VisitDependentScopeDeclRefExpr(DependentScopeDeclRefExpr dependentScopeDeclRefExpr)
    {
        var outputBuilder = StartCSharpCode();

        var name = GetRemappedName(dependentScopeDeclRefExpr.DeclName, dependentScopeDeclRefExpr, tryRemapOperatorName: true, out _, skipUsing: true);
        outputBuilder.Write(EscapeName(name));

        StopCSharpCode();
    }

    private void VisitDoStmt(DoStmt doStmt)
    {
        var outputBuilder = StartCSharpCode();
        outputBuilder.WriteLine("do");

        VisitBody(doStmt.Body);

        outputBuilder.WriteIndented("while (");

        Visit(doStmt.Cond);

        outputBuilder.Write(')');
        outputBuilder.WriteSemicolon();
        outputBuilder.WriteNewline();

        outputBuilder.NeedsNewline = true;

        StopCSharpCode();
    }

    private void VisitExplicitCastExpr(ExplicitCastExpr explicitCastExpr)
    {
        var outputBuilder = StartCSharpCode();

        if (IsPrevContextDecl<EnumConstantDecl>(out _, out _) && IsType<EnumType>(explicitCastExpr, out var enumType))
        {
            outputBuilder.Write('(');
            var enumUnderlyingTypeName = GetRemappedTypeName(explicitCastExpr, context: null, enumType.Decl.IntegerType, out _);
            outputBuilder.Write(enumUnderlyingTypeName);
            outputBuilder.Write(')');
        }

        var type = explicitCastExpr.Type;
        var typeName = GetRemappedTypeName(explicitCastExpr, context: null, type, out _);

        if (IsPrevContextDecl<VarDecl>(out var varDecl, out _))
        {
            var cursorName = GetCursorName(varDecl);

            if (cursorName.StartsWith("ClangSharpMacro_", StringComparison.Ordinal) && _config.WithTransparentStructs.TryGetValue(typeName, out var transparentStruct))
            {
                if (!IsPrimitiveValue(explicitCastExpr, type) || IsConstant(typeName, varDecl.Init))
                {
                    typeName = transparentStruct.Name;
                }
            }
        }

        if (typeName.Equals("IntPtr", StringComparison.Ordinal))
        {
            typeName = "nint";
        }
        else if (typeName.Equals("UIntPtr", StringComparison.Ordinal))
        {
            typeName = "nuint";
        }

        outputBuilder.Write('(');
        outputBuilder.Write(typeName);
        outputBuilder.Write(')');

        ParenthesizeStmt(explicitCastExpr.SubExprAsWritten);

        StopCSharpCode();
    }

    private void VisitExprWithCleanups(ExprWithCleanups exprWithCleanups)
    {
        Visit(exprWithCleanups.SubExpr);
        Visit(exprWithCleanups.Objects);
    }

    private void VisitFloatingLiteral(FloatingLiteral floatingLiteral)
    {
        var outputBuilder = StartCSharpCode();
        if (floatingLiteral.ValueString.EndsWith(".f", StringComparison.Ordinal))
        {
            outputBuilder.WriteNumberLiteral(floatingLiteral.ValueString.AsSpan()[..^1]);
            outputBuilder.Write("0f");
        }
        else
        {
            outputBuilder.WriteNumberLiteral(floatingLiteral.ValueString);

            if (floatingLiteral.ValueString.EndsWith('.'))
            {
                outputBuilder.Write('0');
            }
        }

        StopCSharpCode();
    }

    private void VisitForStmt(ForStmt forStmt)
    {
        var outputBuilder = StartCSharpCode();
        outputBuilder.Write("for (");

        if (forStmt.ConditionVariableDeclStmt != null)
        {
            Visit(forStmt.ConditionVariableDeclStmt);
        }
        else if (forStmt.Init != null)
        {
            Visit(forStmt.Init);
        }

        outputBuilder.WriteSemicolon();

        if (forStmt.Cond != null)
        {
            outputBuilder.Write(' ');
            Visit(forStmt.Cond);
        }

        outputBuilder.WriteSemicolon();

        if (forStmt.Inc != null)
        {
            outputBuilder.Write(' ');
            Visit(forStmt.Inc);
        }

        outputBuilder.WriteLine(')');

        VisitBody(forStmt.Body);
        StopCSharpCode();
    }

    private void VisitGotoStmt(GotoStmt gotoStmt)
    {
        var outputBuilder = StartCSharpCode();
        outputBuilder.Write("goto ");
        outputBuilder.Write(gotoStmt.Label.Name);
        StopCSharpCode();
    }

    private void VisitIfStmt(IfStmt ifStmt)
    {
        var outputBuilder = StartCSharpCode();
        outputBuilder.Write("if (");

        Visit(ifStmt.Cond);

        outputBuilder.WriteLine(')');

        VisitBody(ifStmt.Then);

        if (ifStmt.Else != null)
        {
            outputBuilder.WriteIndented("else");

            if (ifStmt.Else is IfStmt)
            {
                outputBuilder.Write(' ');
                Visit(ifStmt.Else);
            }
            else
            {
                outputBuilder.WriteNewline();
                VisitBody(ifStmt.Else);
            }
        }

        StopCSharpCode();
    }

    private void VisitImplicitCastExpr(ImplicitCastExpr implicitCastExpr)
    {
        var outputBuilder = StartCSharpCode();
        var subExpr = implicitCastExpr.SubExprAsWritten;

        switch (implicitCastExpr.CastKind)
        {
            case CX_CK_NullToPointer:
            {
                outputBuilder.Write("null");
                break;
            }

            case CX_CK_PointerToBoolean:
            {
                if ((subExpr is UnaryOperator unaryOperator) && (unaryOperator.Opcode == CXUnaryOperator_LNot))
                {
                    Visit(subExpr);
                }
                else
                {
                    ParenthesizeStmt(subExpr);
                    outputBuilder.Write(" != null");
                }
                break;
            }

            case CX_CK_IntegralCast:
            {
                if (subExpr.Type.CanonicalType.Kind == CXType_Bool)
                {
                    goto case CX_CK_BooleanToSignedIntegral;
                }
                else
                {
                    goto default;
                }
            }

            case CX_CK_IntegralToBoolean:
            {
                if ((subExpr is UnaryOperator unaryOperator) && (unaryOperator.Opcode == CXUnaryOperator_LNot))
                {
                    Visit(subExpr);
                }
                else
                {
                    ParenthesizeStmt(subExpr);
                    outputBuilder.Write(" != 0");
                }
                break;
            }

            case CX_CK_BooleanToSignedIntegral:
            {
                var needsCast = implicitCastExpr.Type.Handle.SizeOf < 4;

                if (needsCast)
                {
                    outputBuilder.Write("(byte)(");
                }

                ParenthesizeStmt(subExpr);
                outputBuilder.Write(" ? 1 : 0");

                if (needsCast)
                {
                    outputBuilder.Write(')');
                }

                break;
            }

            default:
            {
                if (IsStmtAsWritten<DeclRefExpr>(subExpr, out var declRefExpr, removeParens: true) && (declRefExpr.Decl is EnumConstantDecl))
                {
                    ForEnumConstantDecl(implicitCastExpr);
                }
                else
                {
                    VisitStmt(subExpr);
                }
                break;
            }
        }

        void ForEnumConstantDecl(ImplicitCastExpr implicitCastExpr)
        {
            var subExpr = implicitCastExpr.SubExprAsWritten;

            if (IsPrevContextStmt<BinaryOperator>(out var binaryOperator, out _) && ((binaryOperator.Opcode == CXBinaryOperator_EQ) || (binaryOperator.Opcode == CXBinaryOperator_NE)))
            {
                Visit(subExpr);
                subExpr = null;
            }
            else if (IsPrevContextStmt<CaseStmt>(out _, out _))
            {
                var previousContext = _context.Last;
                Debug.Assert(previousContext is not null);

                previousContext = previousContext.Previous;
                Debug.Assert(previousContext is not null);

                do
                {
                    previousContext = previousContext.Previous;
                    Debug.Assert(previousContext is not null);
                }
                while (previousContext.Value.Cursor is ParenExpr or ImplicitCastExpr or CaseStmt or CompoundStmt);

                var value = previousContext.Value;

                if ((value.Cursor is SwitchStmt switchStmt) && IsType<EnumType>(switchStmt.Cond.IgnoreImplicit))
                {
                    Visit(subExpr);
                    subExpr = null;
                }
            }
            else if (IsPrevContextDecl<EnumConstantDecl>(out _, out _))
            {
                Visit(subExpr);
                subExpr = null;
            }

            if (subExpr is not null)
            {
                var type = implicitCastExpr.Type;

                var typeName = GetRemappedTypeName(implicitCastExpr, context: null, type, out _);

                outputBuilder.Write('(');
                outputBuilder.Write(typeName);
                outputBuilder.Write(')');

                ParenthesizeStmt(subExpr);
            }
        }

        StopCSharpCode();
    }

    private void VisitImplicitValueInitExpr(ImplicitValueInitExpr implicitValueInitExpr)
    {
        StartCSharpCode().Write("default");
        StopCSharpCode();
    }

    private void VisitInitListExpr(InitListExpr initListExpr)
    {
        var outputBuilder = StartCSharpCode();

        if (IsType<ArrayType>(initListExpr, out var arrayType))
        {
            ForArrayType(outputBuilder, initListExpr, arrayType);
        }
        else if (IsType<BuiltinType>(initListExpr, out var builtinType))
        {
            ForBuiltinType(outputBuilder, initListExpr);
        }
        else if (IsType<PointerType>(initListExpr, out var pointerType))
        {
            ForPointerType(outputBuilder, initListExpr);
        }
        else if (IsType<RecordType>(initListExpr, out var recordType))
        {
            ForRecordType(outputBuilder, initListExpr, recordType);
        }
        else
        {
            AddDiagnostic(DiagnosticLevel.Error, $"Unsupported init list expression type: '{initListExpr.Type.TypeClassSpelling}'. Generated bindings may be incomplete.", initListExpr);
        }

        StopCSharpCode();

        long CalculateRootSize(CSharpOutputBuilder outputBuilder, InitListExpr initListExpr, ArrayType? arrayType, bool isUnmanagedConstant)
        {
            long rootSize = -1;

            while (arrayType is not null)
            {
                if (!isUnmanagedConstant)
                {
                    outputBuilder.Write('[');
                }
                long size = -1;

                if (IsTypeConstantOrIncompleteArray(initListExpr, arrayType))
                {
                    size = Math.Max((arrayType as ConstantArrayType)?.Size ?? 0, 1);
                }
                else
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported array type kind: '{initListExpr.Type.KindSpelling}'. Generated bindings may be incomplete.", initListExpr);
                }

                if (rootSize == -1)
                {
                    if (size != -1)
                    {
                        rootSize = size;

                        if (!isUnmanagedConstant)
                        {
                            outputBuilder.Write(size);
                        }
                    }
                    else
                    {
                        rootSize = 0;
                    }
                }

                if (!isUnmanagedConstant)
                {
                    outputBuilder.Write(']');
                }
                arrayType = arrayType.ElementType as ArrayType;
            }

            return rootSize;
        }

        void ForArrayType(CSharpOutputBuilder outputBuilder, InitListExpr initListExpr, ArrayType arrayType)
        {
            var type = initListExpr.Type;
            var typeName = GetRemappedTypeName(initListExpr, context: null, type, out _);
            var isUnmanagedConstant = false;
            var escapedName = "";

            if (IsPrevContextDecl<VarDecl>(out _, out var userData))
            {
                if (userData is ValueDesc valueDesc)
                {
                    escapedName = valueDesc.EscapedName;
                    isUnmanagedConstant = (valueDesc.Kind == ValueKind.Unmanaged) && valueDesc.IsConstant;
                }
            }

            if (_config.GenerateUnmanagedConstants && isUnmanagedConstant && (_testStmtOutputBuilder is null))
            {
                HandleUnmanagedConstant(outputBuilder, initListExpr, arrayType, typeName, escapedName);
            }
            else
            {
                outputBuilder.Write("new ");
                outputBuilder.Write(typeName);

                var rootSize = CalculateRootSize(outputBuilder, initListExpr, arrayType, isUnmanagedConstant: false);

                outputBuilder.WriteNewline();
                outputBuilder.WriteBlockStart();

                for (var i = 0; i < initListExpr.Inits.Count; i++)
                {
                    outputBuilder.WriteIndentation();
                    Visit(initListExpr.Inits[i]);
                    outputBuilder.WriteLine(',');
                }

                for (var i = initListExpr.Inits.Count; i < rootSize; i++)
                {
                    outputBuilder.WriteIndentedLine("default,");
                }

                outputBuilder.DecreaseIndentation();
                outputBuilder.WriteIndented('}');
                outputBuilder.NeedsSemicolon = true;
            }
        }

        void ForBuiltinType(CSharpOutputBuilder outputBuilder, InitListExpr initListExpr)
        {
            var inits = initListExpr.Inits;

            if (inits.Count == 1)
            {
                Visit(inits[0]);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported init list expression count for builtin type: '{inits.Count}'. Generated bindings may be incomplete.", initListExpr);
            }

            outputBuilder.NeedsSemicolon = true;
        }

        void ForRecordType(CSharpOutputBuilder outputBuilder, InitListExpr initListExpr, RecordType recordType)
        {
            var type = initListExpr.Type;
            var typeName = GetRemappedTypeName(initListExpr, context: null, type, out _);
            var isUnmanagedConstant = false;
            var escapedName = "";
            var skipInitializer = false;

            if (IsPrevContextDecl<VarDecl>(out _, out var userData))
            {
                if (userData is ValueDesc valueDesc)
                {
                    escapedName = valueDesc.EscapedName;
                    isUnmanagedConstant = (valueDesc.Kind == ValueKind.Unmanaged) && valueDesc.IsConstant;
                }
            }
            else if (IsPrevContextDecl<CXXConstructorDecl>(out _, out userData, includeLast: true))
            {
                if (userData is bool boolValue)
                {
                    skipInitializer = boolValue;
                }
            }

            if (_config.GenerateUnmanagedConstants && isUnmanagedConstant && (_testStmtOutputBuilder is null))
            {
                HandleUnmanagedConstant(outputBuilder, initListExpr, recordType, typeName, escapedName);
            }
            else
            {
                if (!skipInitializer)
                {
                    outputBuilder.Write("new ");
                    outputBuilder.Write(typeName);
                }

                if (typeName.Equals("Guid", StringComparison.Ordinal))
                {
                    outputBuilder.Write('(');

                    Visit(initListExpr.Inits[0]);

                    outputBuilder.Write(", ");
                    Visit(initListExpr.Inits[1]);

                    outputBuilder.Write(", ");
                    Visit(initListExpr.Inits[2]);

                    initListExpr = (InitListExpr)initListExpr.Inits[3];

                    for (var i = 0; i < initListExpr.Inits.Count; i++)
                    {
                        outputBuilder.Write(", ");
                        Visit(initListExpr.Inits[i]);
                    }

                    outputBuilder.Write(')');
                }
                else
                {
                    if (!skipInitializer)
                    {
                        outputBuilder.WriteNewline();
                        outputBuilder.WriteBlockStart();
                    }

                    var decl = recordType.Decl;

                    for (var i = 0; i < initListExpr.Inits.Count; i++)
                    {
                        var init = initListExpr.Inits[i];

                        if (init is ImplicitValueInitExpr)
                        {
                            continue;
                        }

                        var fieldDecl = decl.Fields[i];
                        var fieldName = GetRemappedCursorName(fieldDecl);

                        outputBuilder.WriteIndented(fieldName);
                        outputBuilder.Write(" = ");

                        if (!IsTypePointerOrReference(fieldDecl) && (init is Expr initExpr))
                        {
                            if ((initExpr is CXXConstructExpr cxxConstructExpr) && cxxConstructExpr.Constructor.IsDefaulted)
                            {
                                initExpr = cxxConstructExpr.Args[0];
                            }

                            if (IsType<ReferenceType>(initExpr))
                            {
                                outputBuilder.Write('*');
                            }
                            else if (IsStmtAsWritten<DeclRefExpr>(initExpr, out var declRefExpr, removeParens: true))
                            {
                                if (IsTypePointerOrReference(declRefExpr.Decl))
                                {
                                    outputBuilder.Write('*');
                                }
                            }
                        }

                        Visit(init);
                        outputBuilder.WriteLine(skipInitializer ? ';' : ',');
                    }

                    if (!skipInitializer)
                    {
                        outputBuilder.DecreaseIndentation();
                        outputBuilder.WriteIndented('}');
                        outputBuilder.NeedsSemicolon = true;
                    }
                }
            }
        }

        void ForPointerType(CSharpOutputBuilder outputBuilder, InitListExpr initListExpr)
        {
            var inits = initListExpr.Inits;

            if (inits.Count == 1)
            {
                Visit(inits[0]);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported init list expression count for pointer type: '{inits.Count}'. Generated bindings may be incomplete.", initListExpr);
            }

            outputBuilder.NeedsSemicolon = true;
        }

        void HandleInitListExpr(CSharpOutputBuilder outputBuilder, InitListExpr initListExpr)
        {
            var inits = initListExpr.Inits;

            if (initListExpr.NumInits > 0)
            {
                HandleInitStmt(outputBuilder, inits[0]);
            }

            for (var i = 1; i < inits.Count; i++)
            {
                outputBuilder.WriteLine(',');
                HandleInitStmt(outputBuilder, inits[i]);
            }
        }

        void HandleInitStmt(CSharpOutputBuilder outputBuilder, Stmt init)
        {
            if (init is InitListExpr nestedInitListExpr)
            {
                HandleInitListExpr(outputBuilder, nestedInitListExpr);
            }
            else
            {
                outputBuilder.WriteIndentation();

                var evaluation = init.Handle.Evaluate;

                switch (evaluation.Kind)
                {
                    case CXEval_Int:
                    {
                        var sizeInChars = ((Expr)init).Type.Handle.SizeOf;
                        outputBuilder.WriteValueAsBytes(evaluation.AsUnsigned, (int)sizeInChars);
                        break;
                    }

                    case CXEval_Float:
                    {
                        var sizeInChars = ((Expr)init).Type.Handle.SizeOf;

                        if (sizeInChars == 4)
                        {
                            var value = (float)evaluation.AsDouble;
                            outputBuilder.WriteValueAsBytes(Unsafe.As<float, uint>(ref value), (int)sizeInChars);
                        }
                        else if (sizeInChars == 8)
                        {
                            var value = evaluation.AsDouble;
                            outputBuilder.WriteValueAsBytes(Unsafe.As<double, ulong>(ref value), (int)sizeInChars);
                        }
                        else
                        {
                            goto default;
                        }

                        break;
                    }

                    case CXEval_StrLiteral:
                    {
                        outputBuilder.Write('"');
                        outputBuilder.Write(evaluation.AsStr);
                        outputBuilder.Write('"');
                        break;
                    }

                    default:
                    {
                        AddDiagnostic(DiagnosticLevel.Error, $"Unsupported evaluation kind: '{evaluation.Kind}'. Generated bindings may be incomplete.", init);
                        break;
                    }
                }
            }
        }

        void HandleUnmanagedConstant(CSharpOutputBuilder outputBuilder, InitListExpr initListExpr, Type type, string typeName, string escapedName)
        {
            outputBuilder.AddUsingDirective("System");
            outputBuilder.AddUsingDirective("System.Diagnostics");
            outputBuilder.AddUsingDirective("System.Runtime.CompilerServices");
            outputBuilder.AddUsingDirective("System.Runtime.InteropServices");

            outputBuilder.WriteIndented("ReadOnlySpan<byte> data = ");

            if (!_config.GenerateCompatibleCode)
            {
                outputBuilder.WriteLine("[");
            }
            else
            {
                outputBuilder.WriteLine("new byte[] {");
            }

            outputBuilder.IncreaseIndentation();

            HandleInitListExpr(outputBuilder, initListExpr);

            outputBuilder.WriteNewline();
            outputBuilder.DecreaseIndentation();

            if (!_config.GenerateCompatibleCode)
            {
                outputBuilder.WriteIndented(']');
            }
            else
            {
                outputBuilder.WriteIndented('}');
            }

            outputBuilder.WriteLine(';');

            outputBuilder.NeedsNewline = true;
            long rootSize = -1;

            if (IsType<ArrayType>(initListExpr, type, out var arrayType))
            {
                rootSize = CalculateRootSize(outputBuilder, initListExpr, arrayType, isUnmanagedConstant: true);

                outputBuilder.WriteIndented("Debug.Assert(data.Length == (Unsafe.SizeOf<");
                outputBuilder.Write(typeName);
                outputBuilder.Write(">() * ");
                outputBuilder.Write(rootSize);
                outputBuilder.WriteLine("));");

                outputBuilder.WriteIndented("return MemoryMarshal.CreateReadOnlySpan<");
                outputBuilder.Write(typeName);
                outputBuilder.Write(">(");
            }
            else
            {
                outputBuilder.WriteIndented("Debug.Assert(data.Length == Unsafe.SizeOf<");
                outputBuilder.Write(typeName);
                outputBuilder.WriteLine(">());");

                outputBuilder.WriteIndented("return ");
            }

            outputBuilder.Write("ref Unsafe.As<byte, ");
            outputBuilder.Write(typeName);
            outputBuilder.Write(">(ref MemoryMarshal.GetReference(data))");

            if (rootSize != -1)
            {
                outputBuilder.Write(", ");
                outputBuilder.Write(rootSize);
                outputBuilder.Write(")");
            }
            outputBuilder.WriteLine(';');

            StartUsingOutputBuilder(outputBuilder.Name, includeTestOutput: true);

            if (_testOutputBuilder != null)
            {
                _testOutputBuilder.AddUsingDirective("System");
                _testOutputBuilder.AddUsingDirective($"static {GetNamespace(outputBuilder.Name)}.{outputBuilder.Name}");

                _testOutputBuilder.WriteIndented("/// <summary>Validates that the value of the <see cref=\"");
                _testOutputBuilder.Write(escapedName);
                _testOutputBuilder.WriteLine("\" /> property is correct.</summary>");

                WithTestAttribute();

                _testOutputBuilder.WriteIndented("public static void ");
                _testOutputBuilder.Write(escapedName);
                _testOutputBuilder.WriteLine("Test()");
                _testOutputBuilder.WriteBlockStart();

                if (typeName.Equals("Guid", StringComparison.Ordinal))
                {
                    if (_config.GenerateTestsNUnit)
                    {
                        _testOutputBuilder.WriteIndented("Assert.That");
                    }
                    else if (_config.GenerateTestsXUnit)
                    {
                        _testOutputBuilder.WriteIndented("Assert.Equal");
                    }

                    _testOutputBuilder.Write('(');
                    _testOutputBuilder.Write(escapedName);
                    _testOutputBuilder.Write(", ");

                    if (_config.GenerateTestsNUnit)
                    {
                        _testOutputBuilder.Write("Is.EqualTo(");
                    }

                    _testOutputBuilder.Write("new Guid(");

                    var tmp = _outputBuilder;
                    _outputBuilder = _testOutputBuilder;
                    {
                        Visit(initListExpr.Inits[0]);

                        _testOutputBuilder.Write(", ");
                        Visit(initListExpr.Inits[1]);

                        _testOutputBuilder.Write(", ");
                        Visit(initListExpr.Inits[2]);

                        initListExpr = (InitListExpr)initListExpr.Inits[3];

                        for (var i = 0; i < initListExpr.Inits.Count; i++)
                        {
                            _testOutputBuilder.Write(", ");
                            Visit(initListExpr.Inits[i]);
                        }
                    }
                    _outputBuilder = tmp;

                    _testOutputBuilder.Write(")");

                    if (_config.GenerateTestsNUnit)
                    {
                        _testOutputBuilder.Write(')');
                    }

                    _testOutputBuilder.Write(')');
                    _testOutputBuilder.WriteSemicolon();
                    _testOutputBuilder.WriteNewline();
                }
                else if (IsType<RecordType>(initListExpr, type, out var recordType))
                {
                    var decl = recordType.Decl;

                    for (var i = 0; i < initListExpr.Inits.Count; i++)
                    {
                        var init = initListExpr.Inits[i];
                        var fieldName = GetRemappedCursorName(decl.Fields[i]);

                        if (_config.GenerateTestsNUnit)
                        {
                            _testOutputBuilder.WriteIndented("Assert.That");
                        }
                        else if (_config.GenerateTestsXUnit)
                        {
                            _testOutputBuilder.WriteIndented("Assert.Equal");
                        }

                        _testOutputBuilder.Write('(');
                        _testOutputBuilder.Write(escapedName);
                        _testOutputBuilder.Write('.');
                        _testOutputBuilder.Write(fieldName);
                        _testOutputBuilder.Write(", ");

                        if (_config.GenerateTestsNUnit)
                        {
                            _testOutputBuilder.Write("Is.EqualTo(");
                        }

                        var tmp = _outputBuilder;
                        _outputBuilder = _testOutputBuilder;
                        {
                            Visit(init);
                        }
                        _outputBuilder = tmp;

                        if (_config.GenerateTestsNUnit)
                        {
                            _testOutputBuilder.Write(')');
                        }

                        _testOutputBuilder.Write(')');
                        _testOutputBuilder.WriteSemicolon();
                        _testOutputBuilder.WriteNewline();
                    }
                }
                else
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported type kind: '{type.TypeClassSpelling}'. Generated bindings may be incomplete.", initListExpr);
                }

                _testOutputBuilder.WriteBlockEnd();
                _testOutputBuilder.NeedsNewline = true;
            }

            StopUsingOutputBuilder();
        }
    }

    private void VisitIntegerLiteral(IntegerLiteral integerLiteral)
    {
        var valueString = integerLiteral.ValueString.AsSpan();
        var valueSuffix = "";

        if (valueString.EndsWith("ui8", StringComparison.OrdinalIgnoreCase))
        {
            valueString = valueString[..^3];
        }
        else if (valueString.EndsWith("i8", StringComparison.OrdinalIgnoreCase))
        {
            valueString = valueString[..^2];
        }
        else if (valueString.EndsWith("ui16", StringComparison.OrdinalIgnoreCase))
        {
            valueString = valueString[..^4];
        }
        else if (valueString.EndsWith("i16", StringComparison.OrdinalIgnoreCase))
        {
            valueString = valueString[..^3];
        }
        else if (valueString.EndsWith("ui32", StringComparison.OrdinalIgnoreCase))
        {
            valueString = valueString[..^4];
            valueSuffix = "U";
        }
        else if (valueString.EndsWith("i32", StringComparison.OrdinalIgnoreCase))
        {
            valueString = valueString[..^3];
        }
        else if (valueString.EndsWith("ui64", StringComparison.OrdinalIgnoreCase))
        {
            valueString = valueString[..^4];
            valueSuffix = "UL";
        }
        else if (valueString.EndsWith("i64", StringComparison.OrdinalIgnoreCase))
        {
            valueString = valueString[..^3];
            valueSuffix = "L";
        }
        else if (
            valueString.EndsWith("ull", StringComparison.OrdinalIgnoreCase) ||
            valueString.EndsWith("llu", StringComparison.OrdinalIgnoreCase))
        {
            valueString = valueString[..^3];
            valueSuffix = "UL";
        }
        else if (valueString.EndsWith("ll", StringComparison.OrdinalIgnoreCase))
        {
            valueString = valueString[..^2];
            valueSuffix = "L";
        }
        else if (
            valueString.EndsWith("ul", StringComparison.OrdinalIgnoreCase) ||
            valueString.EndsWith("lu", StringComparison.OrdinalIgnoreCase))
        {
            valueString = valueString[..^2];
            valueSuffix = "U";
        }
        else if (valueString.EndsWith('u') || valueString.EndsWith('U'))
        {
            valueString = valueString[..^1];
            valueSuffix = "U";
        }
        else if (valueString.EndsWith('l') || valueString.EndsWith('L'))
        {
            valueString = valueString[..^1];
        }

        var outputBuilder = StartCSharpCode();
        outputBuilder.WriteNumberLiteral(valueString);
        outputBuilder.Write(valueSuffix);
        StopCSharpCode();
    }

    private void VisitLabelStmt(LabelStmt labelStmt)
    {
        var outputBuilder = StartCSharpCode();
        outputBuilder.WriteLabel(labelStmt.Decl.Name);

        outputBuilder.WriteIndentation();
        Visit(labelStmt.SubStmt);
        StopCSharpCode();
    }

    private void VisitMaterializeTemporaryExpr(MaterializeTemporaryExpr materializeTemporaryExpr) => Visit(materializeTemporaryExpr.SubExpr);

    private void VisitMemberExpr(MemberExpr memberExpr)
    {
        var outputBuilder = StartCSharpCode();
        var baseFieldName = "";

        if ((memberExpr.Base is ImplicitCastExpr implicitCastExpr) && (implicitCastExpr.CastKind is CX_CK_DerivedToBase or CX_CK_DerivedToBaseMemberPointer or CX_CK_UncheckedDerivedToBase))
        {
            if (memberExpr.MemberDecl is CXXMethodDecl cxxMethodDecl)
            {
                var parent = cxxMethodDecl.Parent;
                Debug.Assert(parent is not null);

                if ((_cxxRecordDeclContext is not null) && (_cxxRecordDeclContext != parent) && HasField(parent))
                {
                    var cxxBaseSpecifier = _cxxRecordDeclContext.Bases.Where((baseSpecifier) => baseSpecifier.Referenced == parent).SingleOrDefault();

                    if ((cxxBaseSpecifier is not null) && IsBaseExcluded(_cxxRecordDeclContext, GetRecordDecl(cxxBaseSpecifier), cxxBaseSpecifier, out baseFieldName))
                    {
                        baseFieldName = "";
                    }
                }
            }
            else if (memberExpr.MemberDecl is FieldDecl fieldDecl)
            {
                var parent = fieldDecl.Parent;
                Debug.Assert(parent is not null);

                if ((_cxxRecordDeclContext is not null) && (_cxxRecordDeclContext != parent))
                {
                    var cxxBaseSpecifier = _cxxRecordDeclContext.Bases.Where((baseSpecifier) => baseSpecifier.Referenced == parent).SingleOrDefault();

                    if ((cxxBaseSpecifier is not null) && IsBaseExcluded(_cxxRecordDeclContext, GetRecordDecl(cxxBaseSpecifier), cxxBaseSpecifier, out baseFieldName))
                    {
                        baseFieldName = "";
                    }
                }
            }
        }

        if (!memberExpr.IsImplicitAccess || !string.IsNullOrWhiteSpace(baseFieldName))
        {
            var memberExprBase = memberExpr.Base.IgnoreImplicit;

            if (!string.IsNullOrWhiteSpace(baseFieldName))
            {
                outputBuilder.Write(baseFieldName);
            }
            else
            {
                Visit(memberExprBase);
            }

            if (IsStmtAsWritten<CXXThisExpr>(memberExprBase, out _, removeParens: true))
            {
                outputBuilder.Write('.');
            }
            else if (memberExprBase is DeclRefExpr declRefExpr)
            {
                if (IsTypePointerOrReference(declRefExpr.Decl))
                {
                    outputBuilder.Write("->");
                }
                else
                {
                    outputBuilder.Write('.');
                }
            }
            else if (IsTypePointerOrReference(memberExpr.Base))
            {
                outputBuilder.Write("->");
            }
            else
            {
                outputBuilder.Write('.');
            }
        }

        outputBuilder.Write(GetRemappedCursorName(memberExpr.MemberDecl));
        StopCSharpCode();
    }

    private static void VisitNullStmt(NullStmt nullStmt)
    {
        // null statements are empty by definition, so nothing to do
    }

    private void VisitPackExpansionExpr(PackExpansionExpr packExpansionExpr) => Visit(packExpansionExpr.Pattern);

    private void VisitParenExpr(ParenExpr parenExpr)
    {
        var outputBuilder = StartCSharpCode();
        outputBuilder.Write('(');
        Visit(parenExpr.SubExpr);
        outputBuilder.Write(')');
        StopCSharpCode();
    }

    private void VisitParenListExpr(ParenListExpr parenListExpr)
    {
        var outputBuilder = StartCSharpCode();
        outputBuilder.Write('(');

        var needsComma = false;

        foreach (var expr in parenListExpr.Exprs)
        {
            if (needsComma)
            {
                outputBuilder.Write(", ");
            }

            Visit(expr);
            needsComma = true;
        }

        outputBuilder.Write(')');
        StopCSharpCode();
    }

    private void VisitReturnStmt(ReturnStmt returnStmt)
    {
        var outputBuilder = StartCSharpCode();

        if (IsPrevContextDecl<FunctionDecl>(out var functionDecl, out _) && !IsTypeVoid(functionDecl, functionDecl.ReturnType))
        {
            outputBuilder.Write("return");
            var retValue = returnStmt.RetValue;

            if (retValue != null)
            {
                outputBuilder.Write(' ');

                if (!IsTypePointerOrReference(functionDecl) && IsType<ReferenceType>(retValue))
                {
                    outputBuilder.Write('*');
                }
                else if (IsType<ReferenceType>(functionDecl, functionDecl.ReturnType, out var referenceType))
                {
                    if (IsStmtAsWritten<UnaryOperator>(retValue, out var unaryOperator, removeParens: true) && (unaryOperator.Opcode == CXUnaryOperator_Deref))
                    {
                        var subExpr = unaryOperator.SubExpr;

                        if (subExpr is CXXThisExpr)
                        {
                            var referenceTypeName = GetTypeName(returnStmt, context: null, type: referenceType, ignoreTransparentStructsWhereRequired: true, isTemplate: false, nativeTypeName: out _);

                            outputBuilder.AddUsingDirective("System.Runtime.CompilerServices");
                            outputBuilder.Write('(');
                            outputBuilder.Write(referenceTypeName);
                            outputBuilder.Write(")Unsafe.AsPointer(");

                            if (referenceType.IsLocalConstQualified)
                            {
                                if (!_config.GenerateLatestCode)
                                {
                                    outputBuilder.Write("ref Unsafe.AsRef(");
                                }

                                outputBuilder.Write("in this");

                                if (!_config.GenerateLatestCode)
                                {
                                    outputBuilder.Write(')');
                                }
                            }
                            else
                            {
                                outputBuilder.Write("ref this");
                            }
                            outputBuilder.Write(')');

                            StopCSharpCode();
                            return;
                        }

                        retValue = subExpr;
                    }
                }

                Visit(retValue);
            }
        }
        else if (returnStmt.RetValue != null)
        {
            Visit(returnStmt.RetValue);
        }
        else
        {
            outputBuilder.Write("return");
        }

        StopCSharpCode();
    }

    private void VisitStmt(Stmt stmt)
    {
        switch (stmt.StmtClass)
        {
            // case CX_StmtClass_GCCAsmStmt:
            // case CX_StmtClass_MSAsmStmt:

            case CX_StmtClass_BreakStmt:
            {
                VisitBreakStmt((BreakStmt)stmt);
                break;
            }

            // case CX_StmtClass_CXXCatchStmt:
            // case CX_StmtClass_CXXForRangeStmt:
            // case CX_StmtClass_CXXTryStmt:
            // case CX_StmtClass_CapturedStmt:

            case CX_StmtClass_CompoundStmt:
            {
                VisitCompoundStmt((CompoundStmt)stmt);
                break;
            }

            case CX_StmtClass_ContinueStmt:
            {
                VisitContinueStmt((ContinueStmt)stmt);
                break;
            }

            // case CX_StmtClass_CoreturnStmt:
            // case CX_StmtClass_CoroutineBodyStmt:

            case CX_StmtClass_DeclStmt:
            {
                VisitDeclStmt((DeclStmt)stmt);
                break;
            }

            case CX_StmtClass_DoStmt:
            {
                VisitDoStmt((DoStmt)stmt);
                break;
            }

            case CX_StmtClass_ForStmt:
            {
                VisitForStmt((ForStmt)stmt);
                break;
            }

            case CX_StmtClass_GotoStmt:
            {
                VisitGotoStmt((GotoStmt)stmt);
                break;
            }

            case CX_StmtClass_IfStmt:
            {
                VisitIfStmt((IfStmt)stmt);
                break;
            }

            // case CX_StmtClass_IndirectGotoStmt:
            // case CX_StmtClass_MSDependentExistsStmt:

            case CX_StmtClass_NullStmt:
            {
                VisitNullStmt((NullStmt)stmt);
                break;
            }

            // case CX_StmtClass_OMPAtomicDirective:
            // case CX_StmtClass_OMPBarrierDirective:
            // case CX_StmtClass_OMPCancelDirective:
            // case CX_StmtClass_OMPCancellationPointDirective:
            // case CX_StmtClass_OMPCriticalDirective:
            // case CX_StmtClass_OMPFlushDirective:
            // case CX_StmtClass_OMPDistributeDirective:
            // case CX_StmtClass_OMPDistributeParallelForDirective:
            // case CX_StmtClass_OMPDistributeParallelForSimdDirective:
            // case CX_StmtClass_OMPDistributeSimdDirective:
            // case CX_StmtClass_OMPForDirective:
            // case CX_StmtClass_OMPForSimdDirective:
            // case CX_StmtClass_OMPParallelForDirective:
            // case CX_StmtClass_OMPParallelForSimdDirective:
            // case CX_StmtClass_OMPSimdDirective:
            // case CX_StmtClass_OMPTargetParallelForSimdDirective:
            // case CX_StmtClass_OMPTargetSimdDirective:
            // case CX_StmtClass_OMPTargetTeamsDistributeDirective:
            // case CX_StmtClass_OMPTargetTeamsDistributeParallelForDirective:
            // case CX_StmtClass_OMPTargetTeamsDistributeParallelForSimdDirective:
            // case CX_StmtClass_OMPTargetTeamsDistributeSimdDirective:
            // case CX_StmtClass_OMPTaskLoopDirective:
            // case CX_StmtClass_OMPTaskLoopSimdDirective:
            // case CX_StmtClass_OMPTeamsDistributeDirective:
            // case CX_StmtClass_OMPTeamsDistributeParallelForDirective:
            // case CX_StmtClass_OMPTeamsDistributeParallelForSimdDirective:
            // case CX_StmtClass_OMPTeamsDistributeSimdDirective:
            // case CX_StmtClass_OMPMasterDirective:
            // case CX_StmtClass_OMPOrderedDirective:
            // case CX_StmtClass_OMPParallelDirective:
            // case CX_StmtClass_OMPParallelSectionsDirective:
            // case CX_StmtClass_OMPSectionDirective:
            // case CX_StmtClass_OMPSectionsDirective:
            // case CX_StmtClass_OMPSingleDirective:
            // case CX_StmtClass_OMPTargetDataDirective:
            // case CX_StmtClass_OMPTargetDirective:
            // case CX_StmtClass_OMPTargetEnterDataDirective:
            // case CX_StmtClass_OMPTargetExitDataDirective:
            // case CX_StmtClass_OMPTargetParallelDirective:
            // case CX_StmtClass_OMPTargetParallelForDirective:
            // case CX_StmtClass_OMPTargetTeamsDirective:
            // case CX_StmtClass_OMPTargetUpdateDirective:
            // case CX_StmtClass_OMPTaskDirective:
            // case CX_StmtClass_OMPTaskgroupDirective:
            // case CX_StmtClass_OMPTaskwaitDirective:
            // case CX_StmtClass_OMPTaskyieldDirective:
            // case CX_StmtClass_OMPTeamsDirective:
            // case CX_StmtClass_ObjCAtCatchStmt:
            // case CX_StmtClass_ObjCAtFinallyStmt:
            // case CX_StmtClass_ObjCAtSynchronizedStmt:
            // case CX_StmtClass_ObjCAtThrowStmt:
            // case CX_StmtClass_ObjCAtTryStmt:
            // case CX_StmtClass_ObjCAutoreleasePoolStmt:
            // case CX_StmtClass_ObjCForCollectionStmt:

            case CX_StmtClass_ReturnStmt:
            {
                VisitReturnStmt((ReturnStmt)stmt);
                break;
            }

            // case CX_StmtClass_SEHExceptStmt:
            // case CX_StmtClass_SEHFinallyStmt:
            // case CX_StmtClass_SEHLeaveStmt:
            // case CX_StmtClass_SEHTryStmt:

            case CX_StmtClass_CaseStmt:
            {
                VisitCaseStmt((CaseStmt)stmt);
                break;
            }

            case CX_StmtClass_DefaultStmt:
            {
                VisitDefaultStmt((DefaultStmt)stmt);
                break;
            }

            case CX_StmtClass_SwitchStmt:
            {
                VisitSwitchStmt((SwitchStmt)stmt);
                break;
            }

            // case CX_StmtClass_AttributedStmt:
            // case CX_StmtClass_BinaryConditionalOperator:

            case CX_StmtClass_ConditionalOperator:
            {
                VisitConditionalOperator((ConditionalOperator)stmt);
                break;
            }

            // case CX_StmtClass_AddrLabelExpr:
            // case CX_StmtClass_ArrayInitIndexExpr:
            // case CX_StmtClass_ArrayInitLoopExpr:

            case CX_StmtClass_ArraySubscriptExpr:
            {
                VisitArraySubscriptExpr((ArraySubscriptExpr)stmt);
                break;
            }

            // case CX_StmtClass_ArrayTypeTraitExpr:
            // case CX_StmtClass_AsTypeExpr:
            // case CX_StmtClass_AtomicExpr:

            case CX_StmtClass_BinaryOperator:
            case CX_StmtClass_CompoundAssignOperator:
            {
                VisitBinaryOperator((BinaryOperator)stmt);
                break;
            }

            // case CX_StmtClass_BlockExpr:
            // case CX_StmtClass_CXXBindTemporaryExpr:

            case CX_StmtClass_CXXBoolLiteralExpr:
            {
                VisitCXXBoolLiteralExpr((CXXBoolLiteralExpr)stmt);
                break;
            }

            case CX_StmtClass_CXXConstructExpr:
            {
                VisitCXXConstructExpr((CXXConstructExpr)stmt);
                break;
            }

            case CX_StmtClass_CXXTemporaryObjectExpr:
            {
                VisitCXXTemporaryObjectExpr((CXXTemporaryObjectExpr)stmt);
                break;
            }

            case CX_StmtClass_CXXDefaultArgExpr:
            {
                VisitCXXDefaultArgExpr((CXXDefaultArgExpr)stmt);
                break;
            }

            // case CX_StmtClass_CXXDefaultInitExpr:
            // case CX_StmtClass_CXXDeleteExpr:

            case CX_StmtClass_CXXDependentScopeMemberExpr:
            {
                VisitCXXDependentScopeMemberExpr((CXXDependentScopeMemberExpr)stmt);
                break;
            }

            // case CX_StmtClass_CXXFoldExpr:
            // case CX_StmtClass_CXXInheritedCtorInitExpr:

            case CX_StmtClass_CXXNewExpr:
            {
                VisitCXXNewExpr((CXXNewExpr)stmt);
                break;
            }

            // case CX_StmtClass_CXXNoexceptExpr:

            case CX_StmtClass_CXXNullPtrLiteralExpr:
            {
                VisitCXXNullPtrLiteralExpr((CXXNullPtrLiteralExpr)stmt);
                break;
            }

            case CX_StmtClass_CXXPseudoDestructorExpr:
            {
                VisitCXXPseudoDestructorExpr((CXXPseudoDestructorExpr)stmt);
                break;
            }

            // case CX_StmtClass_CXXScalarValueInitExpr:
            // case CX_StmtClass_CXXStdInitializerListExpr:

            case CX_StmtClass_CXXThisExpr:
            {
                VisitCXXThisExpr((CXXThisExpr)stmt);
                break;
            }

            // case CX_StmtClass_CXXThrowExpr:
            // case CX_StmtClass_CXXTypeidExpr:

            case CX_StmtClass_CXXUnresolvedConstructExpr:
            {
                VisitCXXUnresolvedConstructExpr((CXXUnresolvedConstructExpr)stmt);
                break;
            }

            case CX_StmtClass_CXXUuidofExpr:
            {
                VisitCXXUuidofExpr((CXXUuidofExpr)stmt);
                break;
            }

            case CX_StmtClass_CallExpr:
            case CX_StmtClass_CXXMemberCallExpr:
            {
                VisitCallExpr((CallExpr)stmt);
                break;
            }

            // case CX_StmtClass_CUDAKernelCallExpr:

            case CX_StmtClass_CXXOperatorCallExpr:
            {
                VisitCXXOperatorCallExpr((CXXOperatorCallExpr)stmt);
                break;
            }

            // case CX_StmtClass_UserDefinedLiteral:
            // case CX_StmtClass_BuiltinBitCastExpr:

            case CX_StmtClass_CStyleCastExpr:
            case CX_StmtClass_CXXDynamicCastExpr:
            case CX_StmtClass_CXXReinterpretCastExpr:
            case CX_StmtClass_CXXStaticCastExpr:
            {
                VisitExplicitCastExpr((ExplicitCastExpr)stmt);
                break;
            }

            case CX_StmtClass_CXXFunctionalCastExpr:
            {
                VisitCXXFunctionalCastExpr((CXXFunctionalCastExpr)stmt);
                break;
            }

            case CX_StmtClass_CXXConstCastExpr:
            {
                VisitCXXConstCastExpr((CXXConstCastExpr)stmt);
                break;
            }

            // case CX_StmtClass_ObjCBridgedCastExpr:

            case CX_StmtClass_ImplicitCastExpr:
            {
                VisitImplicitCastExpr((ImplicitCastExpr)stmt);
                break;
            }

            case CX_StmtClass_CharacterLiteral:
            {
                VisitCharacterLiteral((CharacterLiteral)stmt);
                break;
            }

            // case CX_StmtClass_ChooseExpr:
            // case CX_StmtClass_CompoundLiteralExpr:
            // case CX_StmtClass_ConvertVectorExpr:
            // case CX_StmtClass_CoawaitExpr:
            // case CX_StmtClass_CoyieldExpr:

            case CX_StmtClass_DeclRefExpr:
            {
                VisitDeclRefExpr((DeclRefExpr)stmt);
                break;
            }

            // case CX_StmtClass_DependentCoawaitExpr:

            case CX_StmtClass_DependentScopeDeclRefExpr:
            {
                VisitDependentScopeDeclRefExpr((DependentScopeDeclRefExpr)stmt);
                break;
            }

            // case CX_StmtClass_DesignatedInitExpr:
            // case CX_StmtClass_DesignatedInitUpdateExpr:
            // case CX_StmtClass_ExpressionTraitExpr:
            // case CX_StmtClass_ExtVectorElementExpr:
            // case CX_StmtClass_FixedPointLiteral:

            case CX_StmtClass_FloatingLiteral:
            {
                VisitFloatingLiteral((FloatingLiteral)stmt);
                break;
            }

            // case CX_StmtClass_ConstantExpr:

            case CX_StmtClass_ExprWithCleanups:
            {
                VisitExprWithCleanups((ExprWithCleanups)stmt);
                break;
            }

            // case CX_StmtClass_FunctionParmPackExpr:
            // case CX_StmtClass_GNUNullExpr:
            // case CX_StmtClass_GenericSelectionExpr:
            // case CX_StmtClass_ImaginaryLiteral:

            case CX_StmtClass_ImplicitValueInitExpr:
            {
                VisitImplicitValueInitExpr((ImplicitValueInitExpr)stmt);
                break;
            }

            case CX_StmtClass_InitListExpr:
            {
                VisitInitListExpr((InitListExpr)stmt);
                break;
            }

            case CX_StmtClass_IntegerLiteral:
            {
                VisitIntegerLiteral((IntegerLiteral)stmt);
                break;
            }

            // case CX_StmtClass_LambdaExpr:
            // case CX_StmtClass_MSPropertyRefExpr:
            // case CX_StmtClass_MSPropertySubscriptExpr:

            case CX_StmtClass_MaterializeTemporaryExpr:
            {
                VisitMaterializeTemporaryExpr((MaterializeTemporaryExpr)stmt);
                break;
            }

            case CX_StmtClass_MemberExpr:
            {
                VisitMemberExpr((MemberExpr)stmt);
                break;
            }

            // case CX_StmtClass_NoInitExpr:
            // case CX_StmtClass_ArraySectionExpr:
            // case CX_StmtClass_ObjCArrayLiteral:
            // case CX_StmtClass_ObjCAvailabilityCheckExpr:
            // case CX_StmtClass_ObjCBoolLiteralExpr:
            // case CX_StmtClass_ObjCBoxedExpr:
            // case CX_StmtClass_ObjCDictionaryLiteral:
            // case CX_StmtClass_ObjCEncodeExpr:
            // case CX_StmtClass_ObjCIndirectCopyRestoreExpr:
            // case CX_StmtClass_ObjCIsaExpr:
            // case CX_StmtClass_ObjCIvarRefExpr:
            // case CX_StmtClass_ObjCMessageExpr:
            // case CX_StmtClass_ObjCPropertyRefExpr:
            // case CX_StmtClass_ObjCProtocolExpr:
            // case CX_StmtClass_ObjCSelectorExpr:
            // case CX_StmtClass_ObjCStringLiteral:
            // case CX_StmtClass_ObjCSubscriptRefExpr:

            case CX_StmtClass_OffsetOfExpr:
            {
                VisitOffsetOfExpr((OffsetOfExpr)stmt);
                break;
            }

            // case CX_StmtClass_OpaqueValueExpr:

            case CX_StmtClass_UnresolvedLookupExpr:
            {
                VisitUnresolvedLookupExpr((UnresolvedLookupExpr)stmt);
                break;
            }

            case CX_StmtClass_UnresolvedMemberExpr:
            {
                VisitUnresolvedMemberExpr((UnresolvedMemberExpr)stmt);
                break;
            }

            case CX_StmtClass_PackExpansionExpr:
            {
                VisitPackExpansionExpr((PackExpansionExpr)stmt);
                break;
            }

            case CX_StmtClass_ParenExpr:
            {
                VisitParenExpr((ParenExpr)stmt);
                break;
            }

            case CX_StmtClass_ParenListExpr:
            {
                VisitParenListExpr((ParenListExpr)stmt);
                break;
            }

            // case CX_StmtClass_PredefinedExpr:
            // case CX_StmtClass_PseudoObjectExpr:
            // case CX_StmtClass_ShuffleVectorExpr:
            // case CX_StmtClass_SizeOfPackExpr:
            // case CX_StmtClass_SourceLocExpr:
            // case CX_StmtClass_StmtExpr:

            case CX_StmtClass_StringLiteral:
            {
                VisitStringLiteral((StringLiteral)stmt);
                break;
            }

            case CX_StmtClass_SubstNonTypeTemplateParmExpr:
            {
                VisitSubstNonTypeTemplateParmExpr((SubstNonTypeTemplateParmExpr)stmt);
                break;
            }

            // case CX_StmtClass_SubstNonTypeTemplateParmPackExpr:
            // case CX_StmtClass_TypeTraitExpr:
            // case CX_StmtClass_TypoExpr:

            case CX_StmtClass_UnaryExprOrTypeTraitExpr:
            {
                VisitUnaryExprOrTypeTraitExpr((UnaryExprOrTypeTraitExpr)stmt);
                break;
            }

            case CX_StmtClass_UnaryOperator:
            {
                VisitUnaryOperator((UnaryOperator)stmt);
                break;
            }

            // case CX_StmtClass_VAArgExpr:

            case CX_StmtClass_LabelStmt:
            {
                VisitLabelStmt((LabelStmt)stmt);
                break;
            }

            case CX_StmtClass_WhileStmt:
            {
                VisitWhileStmt((WhileStmt)stmt);
                break;
            }

            default:
            {
                var context = string.Empty;

                if (IsPrevContextDecl<NamedDecl>(out var namedDecl, out _))
                {
                    context = $" in {GetCursorQualifiedName(namedDecl)}";
                }

                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported statement: '{stmt.StmtClassName}'{context}. Generated bindings may be incomplete.", stmt);
                break;
            }
        }
    }

    private void VisitStmts(IReadOnlyList<Stmt> stmts)
    {
        var outputBuilder = StartCSharpCode();
        var lastIndex = stmts.Count - 1;
        var previousStmt = null as Stmt;

        for (var i = 0; i < lastIndex; i++)
        {
            var stmt = stmts[i];

            if ((previousStmt is DeclStmt) && (stmt is not DeclStmt))
            {
                outputBuilder.NeedsNewline = true;
            }

            outputBuilder.WriteIndentation();
            outputBuilder.NeedsSemicolon = true;
            outputBuilder.NeedsNewline = true;

            Visit(stmts[i]);

            outputBuilder.WriteSemicolonIfNeeded();
            outputBuilder.WriteNewline();

            previousStmt = stmt;
        }

        if (lastIndex != -1)
        {
            var stmt = stmts[lastIndex];

            if ((previousStmt is DeclStmt) && (stmt is not DeclStmt))
            {
                outputBuilder.NeedsNewline = true;
            }

            outputBuilder.WriteIndentation();
            outputBuilder.NeedsSemicolon = true;
            outputBuilder.NeedsNewline = true;

            Visit(stmt);

            outputBuilder.WriteSemicolonIfNeeded();
            outputBuilder.WriteNewlineIfNeeded();
        }

        StopCSharpCode();
    }

    private void VisitStringLiteral(StringLiteral stringLiteral)
    {
        var outputBuilder = StartCSharpCode();
        switch (stringLiteral.Kind)
        {
            case CX_SLK_Ordinary:
            case CX_SLK_UTF8:
            {
                if (!Config.GenerateCompatibleCode)
                {
                    outputBuilder.Write('"');
                    outputBuilder.Write(EscapeString(stringLiteral.String));
                    outputBuilder.Write('"');
                    outputBuilder.Write("u8");

                    if (IsPrevContextDecl<VarDecl>(out _, out var userData))
                    {
                        if (userData is ValueDesc valueDesc)
                        {
                            if ((valueDesc.Kind == ValueKind.String) && valueDesc.TypeName.Equals("byte[]", StringComparison.Ordinal))
                            {
                                outputBuilder.Write(".ToArray()");
                            }
                        }
                    }
                }
                else
                {
                    outputBuilder.Write("new byte[] { ");

                    var bytes = Encoding.UTF8.GetBytes(stringLiteral.String);

                    foreach (var b in bytes)
                    {
                        outputBuilder.Write("0x");
                        outputBuilder.Write(b.ToString("X2", CultureInfo.InvariantCulture));
                        outputBuilder.Write(", ");
                    }

                    outputBuilder.Write("0x00 }");
                }
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
                outputBuilder.Write('"');
                outputBuilder.Write(EscapeString(stringLiteral.String));
                outputBuilder.Write('"');
                break;
            }

            case CX_SLK_UTF32:
            {
                if (!_config.GenerateCompatibleCode)
                {
                    outputBuilder.Write('[');
                }
                else
                {
                    outputBuilder.Write("new uint[] { ");
                }

                var bytes = Encoding.UTF32.GetBytes(stringLiteral.String);
                var codepoints = MemoryMarshal.Cast<byte, uint>(bytes);

                foreach (var codepoint in codepoints)
                {
                    outputBuilder.Write("0x");
                    outputBuilder.Write(codepoint.ToString("X8", CultureInfo.InvariantCulture));
                    outputBuilder.Write(", ");
                }

                outputBuilder.Write("0x00000000");

                if (!_config.GenerateCompatibleCode)
                {
                    outputBuilder.Write(']');
                }
                else
                {
                    outputBuilder.Write(" }");
                }
                break;
            }

            default:
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported string literal kind: '{stringLiteral.Kind}'. Generated bindings may be incomplete.", stringLiteral);
                break;
            }
        }

        StopCSharpCode();
    }

    private void VisitSubstNonTypeTemplateParmExpr(SubstNonTypeTemplateParmExpr substNonTypeTemplateParmExpr) => Visit(substNonTypeTemplateParmExpr.Replacement);

    private void VisitSwitchStmt(SwitchStmt switchStmt)
    {
        var outputBuilder = StartCSharpCode();

        outputBuilder.Write("switch (");
        Visit(switchStmt.Cond);
        outputBuilder.WriteLine(')');

        VisitBody(switchStmt.Body);
        StopCSharpCode();
    }

    private void VisitUnaryExprOrTypeTraitExpr(UnaryExprOrTypeTraitExpr unaryExprOrTypeTraitExpr)
    {
        var outputBuilder = StartCSharpCode();
        var argumentType = unaryExprOrTypeTraitExpr.TypeOfArgument;

        long alignment32 = -1;
        long alignment64 = -1;

        GetTypeSize(unaryExprOrTypeTraitExpr, argumentType, ref alignment32, ref alignment64, out var size32, out var size64);

        switch (unaryExprOrTypeTraitExpr.Kind)
        {
            case CX_UETT_SizeOf:
            {
                if ((size32 == size64) && IsPrevContextDecl<VarDecl>(out _, out _))
                {
                    outputBuilder.Write(size32);
                }
                else
                {
                    if (_topLevelClassNames.Contains(outputBuilder.Name))
                    {
                        _topLevelClassIsUnsafe[outputBuilder.Name] = true;
                    }

                    var parentType = null as Type;
                    var parentTypeIsVariableSized = false;

                    if (IsPrevContextStmt<CallExpr>(out var callExpr, out _))
                    {
                        var args = callExpr.Args;
                        var index = -1;

                        for (var i = 0; i < args.Count; i++)
                        {
                            var arg = args[i];

                            if (IsStmtAsWritten(arg, unaryExprOrTypeTraitExpr, removeParens: true))
                            {
                                index = i;
                                break;
                            }
                        }

                        if (index == -1)
                        {
                            AddDiagnostic(DiagnosticLevel.Error, $"Failed to locate index of '{unaryExprOrTypeTraitExpr}' in call expression. Generated bindings may be incomplete.", callExpr);
                        }

                        var calleeDecl = callExpr.CalleeDecl;

                        if (calleeDecl is null)
                        {
                            parentType = callExpr.Callee.Type;
                        }
                        else if (calleeDecl is FunctionDecl functionDecl)
                        {
                            switch (functionDecl.Name)
                            {
                                case "memcpy":
                                {
                                    parentTypeIsVariableSized = true;
                                    break;
                                }

                                case "memset":
                                {
                                    parentTypeIsVariableSized = true;
                                    break;
                                }
                            }

                            parentType = functionDecl.Parameters[index].Type;
                        }
                        else
                        {
                            AddDiagnostic(DiagnosticLevel.Error, $"Unsupported callee declaration: '{calleeDecl.DeclKindName}'. Generated bindings may be incomplete.", calleeDecl);
                        }
                    }
                    else if (IsPrevContextStmt<Expr>(out var expr, out _))
                    {
                        parentType = expr.Type;
                    }
                    else if (IsPrevContextDecl<TypeDecl>(out var typeDecl, out _))
                    {
                        parentType = typeDecl.TypeForDecl;
                    }
                    else if (IsPrevContextDecl<ValueDecl>(out var valueDecl, out _))
                    {
                        parentType = valueDecl.Type;
                    }

                    var needsCast = false;
                    var typeName = GetRemappedTypeName(unaryExprOrTypeTraitExpr, context: null, argumentType, out _);

                    if (parentType is not null)
                    {
                        if ((parentType.Handle.SizeOf == 8) && IsPrevContextDecl<VarDecl>(out var varDecl, out _))
                        {
                            var cursorName = GetCursorName(varDecl).AsSpan();

                            if (cursorName.StartsWith("ClangSharpMacro_", StringComparison.Ordinal))
                            {
                                cursorName = cursorName["ClangSharpMacro_".Length..];
                                parentTypeIsVariableSized |= _config._withTypes.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(cursorName, out var remappedTypeName) && (remappedTypeName.Equals("int", StringComparison.Ordinal) || remappedTypeName.Equals("uint", StringComparison.Ordinal));
                            }
                        }

                        needsCast = IsType<BuiltinType>(unaryExprOrTypeTraitExpr, parentType, out var builtinType) &&
                                    ((builtinType.Kind == CXType_UInt) || (builtinType.Kind == CXType_ULong));
                        needsCast &= !IsSupportedFixedSizedBufferType(typeName);
                        needsCast &= !IsType<EnumType>(unaryExprOrTypeTraitExpr, argumentType);
                        needsCast |= parentTypeIsVariableSized;
                    }

                    if (needsCast)
                    {
                        outputBuilder.Write("(uint)(");
                    }

                    outputBuilder.Write("sizeof(");
                    outputBuilder.Write(typeName);
                    outputBuilder.Write(')');

                    if (needsCast)
                    {
                        outputBuilder.Write(')');
                    }
                }
                break;
            }

            case CX_UETT_AlignOf:
            case CX_UETT_PreferredAlignOf:
            {
                if (alignment32 == alignment64)
                {
                    outputBuilder.Write(alignment32);
                }
                else
                {
                    outputBuilder.Write("Environment.Is64BitProcess ? ");
                    outputBuilder.Write(alignment64);
                    outputBuilder.Write(" : ");
                    outputBuilder.Write(alignment32);
                }

                break;
            }

            default:
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported unary or type trait expression: '{unaryExprOrTypeTraitExpr.StmtClassName}'. Generated bindings may be incomplete.", unaryExprOrTypeTraitExpr);
                break;
            }
        }

        StopCSharpCode();
    }

    private void VisitUnaryOperator(UnaryOperator unaryOperator)
    {
        var outputBuilder = StartCSharpCode();
        switch (unaryOperator.Opcode)
        {
            case CXUnaryOperator_PostInc:
            case CXUnaryOperator_PostDec:
            {
                Visit(unaryOperator.SubExpr);
                outputBuilder.Write(unaryOperator.OpcodeStr);
                break;
            }

            case CXUnaryOperator_Deref:
            {
                if (_topLevelClassNames.Contains(outputBuilder.Name))
                {
                    _topLevelClassIsUnsafe[outputBuilder.Name] = true;
                }

                outputBuilder.Write(unaryOperator.OpcodeStr);
                Visit(unaryOperator.SubExpr);
                break;
            }

            case CXUnaryOperator_PreInc:
            case CXUnaryOperator_PreDec:
            case CXUnaryOperator_Plus:
            case CXUnaryOperator_Minus:
            case CXUnaryOperator_Not:
            {
                outputBuilder.Write(unaryOperator.OpcodeStr);
                Visit(unaryOperator.SubExpr);
                break;
            }

            case CXUnaryOperator_LNot:
            {
                var subExpr = GetExprAsWritten(unaryOperator.SubExpr, removeParens: true);

                if (IsType<BuiltinType>(subExpr, out var builtinType) && builtinType.IsIntegerType && (builtinType.Kind != CXType_Bool))
                {
                    var needsParens = IsStmtAsWritten<BinaryOperator>(subExpr, out _);

                    if (needsParens)
                    {
                        outputBuilder.Write('(');
                    }
                    Visit(subExpr);

                    if (needsParens)
                    {
                        outputBuilder.Write(')');
                    }
                    outputBuilder.Write(" == 0");
                }
                else if (IsTypePointerOrReference(subExpr))
                {
                    var needsParens = !IsPrevContextStmt<ParenExpr>(out _, out _, preserveParen: true) &&
                                      !IsPrevContextStmt<IfStmt>(out _, out _, preserveParen: true);

                    if (needsParens)
                    {
                        outputBuilder.Write('(');
                    }

                    Visit(subExpr);
                    outputBuilder.Write(" == null");

                    if (needsParens)
                    {
                        outputBuilder.Write(')');
                    }
                }
                else
                {
                    outputBuilder.Write(unaryOperator.OpcodeStr);
                    Visit(unaryOperator.SubExpr);
                }
                break;
            }

            case CXUnaryOperator_AddrOf:
            {
                if ((unaryOperator.SubExpr is DeclRefExpr declRefExpr) && IsType<LValueReferenceType>(declRefExpr.Decl))
                {
                    Visit(unaryOperator.SubExpr);
                }
                else
                {
                    outputBuilder.Write(unaryOperator.OpcodeStr);
                    Visit(unaryOperator.SubExpr);
                }
                break;
            }

            default:
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported unary operator opcode: '{unaryOperator.OpcodeStr}'. Generated bindings may be incomplete.", unaryOperator);
                break;
            }
        }

        StopCSharpCode();
    }

    private void VisitOffsetOfExpr(OffsetOfExpr offsetOfExpr)
    {
        if (_config.GenerateDisableRuntimeMarshalling)
        {
            AddDiagnostic(DiagnosticLevel.Warning, $"OffsetOf is unsupported when DisableRuntimeMarshalling is enabled. Generated bindings may be incomplete.", offsetOfExpr);
        }

        var outputBuilder = StartCSharpCode();

        outputBuilder.AddUsingDirective("System.Runtime.InteropServices");
        outputBuilder.Write("Marshal.OffsetOf<");
        outputBuilder.Write(GetRemappedTypeName(offsetOfExpr, context: null, offsetOfExpr.TypeSourceInfoType, out var _));
        outputBuilder.Write(">(\"");
        Visit(offsetOfExpr.Referenced ?? offsetOfExpr.CursorChildren[1]);
        outputBuilder.Write("\")");

        StopCSharpCode();
    }

    private void VisitUnresolvedLookupExpr(UnresolvedLookupExpr unresolvedLookupExpr)
    {
        var outputBuilder = StartCSharpCode();
        outputBuilder.Write(GetRemappedName(unresolvedLookupExpr.Name, unresolvedLookupExpr, tryRemapOperatorName: false, out _, skipUsing: true));
        StopCSharpCode();
    }

    private void VisitUnresolvedMemberExpr(UnresolvedMemberExpr unresolvedMemberExpr)
    {
        var outputBuilder = StartCSharpCode();

        if (!unresolvedMemberExpr.IsImplicitAccess)
        {
            var @base = unresolvedMemberExpr.Base;
            Debug.Assert(@base is not null);
            Visit(@base);

            if (@base is CXXThisExpr)
            {
                outputBuilder.Write('.');
                return;
            }
            else if (@base is DeclRefExpr declRefExpr)
            {
                if (IsTypePointerOrReference(declRefExpr.Decl))
                {
                    outputBuilder.Write("->");
                }
                else
                {
                    outputBuilder.Write('.');
                }
            }
            else if (IsTypePointerOrReference(@base))
            {
                outputBuilder.Write("->");
            }
            else
            {
                outputBuilder.Write('.');
            }
        }

        outputBuilder.Write(GetRemappedName(unresolvedMemberExpr.MemberName, unresolvedMemberExpr, tryRemapOperatorName: true, out _, skipUsing: true));
        StopCSharpCode();
    }

    private void VisitWhileStmt(WhileStmt whileStmt)
    {
        var outputBuilder = StartCSharpCode();

        outputBuilder.Write("while (");

        Visit(whileStmt.Cond);

        outputBuilder.WriteLine(')');

        VisitBody(whileStmt.Body);

        StopCSharpCode();
    }
}
