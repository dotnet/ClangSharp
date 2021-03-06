// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClangSharp.Interop;

namespace ClangSharp
{
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
            Visit(binaryOperator.RHS);
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

            if (calleeDecl is FunctionDecl functionDecl)
            {
                switch (functionDecl.Name)
                {
                    case "memcpy":
                    {
                        outputBuilder.AddUsingDirective("System.Runtime.CompilerServices");
                        outputBuilder.Write("Unsafe.CopyBlockUnaligned");
                        VisitArgs(callExpr);
                        break;
                    }

                    case "memset":
                    {
                        outputBuilder.AddUsingDirective("System.Runtime.CompilerServices");
                        outputBuilder.Write("Unsafe.InitBlockUnaligned");
                        VisitArgs(callExpr);
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
            else if (calleeDecl is FieldDecl)
            {
                Visit(callExpr.Callee);
                VisitArgs(callExpr);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported callee declaration: '{calleeDecl?.Kind}'. Generated bindings may be incomplete.", calleeDecl);
            }

            void VisitArgs(CallExpr callExpr)
            {
                outputBuilder.Write('(');

                var args = callExpr.Args;

                if (args.Count != 0)
                {
                    Visit(args[0]);

                    for (int i = 1; i < args.Count; i++)
                    {
                        outputBuilder.Write(", ");
                        Visit(args[i]);
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
                case CX_CharacterKind.CX_CLK_Ascii:
                case CX_CharacterKind.CX_CLK_UTF8:
                {
                    if (characterLiteral.Value > ushort.MaxValue)
                    {
                        outputBuilder.Write("0x");
                        outputBuilder.Write(characterLiteral.Value.ToString("X8"));
                    }
                    else if (characterLiteral.Value > byte.MaxValue)
                    {
                        outputBuilder.Write("0x");
                        outputBuilder.Write(characterLiteral.Value.ToString("X4"));
                    }
                    else
                    {
                        var isPreviousExplicitCast = IsPrevContextStmt<ExplicitCastExpr>(out _);

                        if (!isPreviousExplicitCast)
                        {
                            outputBuilder.Write("(byte)(");
                        }

                        outputBuilder.Write('\'');
                        outputBuilder.Write(EscapeCharacter((char)characterLiteral.Value));
                        outputBuilder.Write('\'');

                        if (!isPreviousExplicitCast)
                        {
                            outputBuilder.Write(')');
                        }
                    }
                    break;
                }

                case CX_CharacterKind.CX_CLK_Wide:
                {
                    if (_config.GenerateUnixTypes)
                    {
                        goto default;
                    }

                    goto case CX_CharacterKind.CX_CLK_UTF16;
                }

                case CX_CharacterKind.CX_CLK_UTF16:
                {
                    if (characterLiteral.Value > ushort.MaxValue)
                    {
                        outputBuilder.Write("0x");
                        outputBuilder.Write(characterLiteral.Value.ToString("X8"));
                    }
                    else
                    {
                        outputBuilder.Write('\'');
                        outputBuilder.Write(EscapeCharacter((char)characterLiteral.Value));
                        outputBuilder.Write('\'');
                    }
                    break;
                }

                case CX_CharacterKind.CX_CLK_UTF32:
                {
                    outputBuilder.Write("0x");
                    outputBuilder.Write(characterLiteral.Value.ToString("X8"));
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

        private void VisitCXXConstCastExpr(CXXConstCastExpr cxxConstCastExpr)
        {
            // C# doesn't have a concept of const pointers so
            // ignore rather than adding a cast from T* to T*

            Visit(cxxConstCastExpr.SubExprAsWritten);
        }

        private void VisitCXXConstructExpr(CXXConstructExpr cxxConstructExpr)
        {
            var outputBuilder = StartCSharpCode();
            var isCopyOrMoveConstructor = cxxConstructExpr.Constructor is { IsCopyConstructor: true } or { IsMoveConstructor: true };

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
                Visit(args[0]);

                for (int i = 1; i < args.Count; i++)
                {
                    outputBuilder.Write(", ");
                    Visit(args[i]);
                }
            }

            if (!isCopyOrMoveConstructor)
            {
                outputBuilder.Write(')');
            }

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
                if (functionDecl.DeclContext is CXXRecordDecl)
                {
                    Visit(cxxOperatorCallExpr.Args[0]);
                    outputBuilder.Write('.');
                }

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
                            outputBuilder.Write(functionDeclName.Substring(8));
                            outputBuilder.Write(' ');
                            Visit(args[1]);
                            return;
                        }

                        case "operator~":
                        {
                            outputBuilder.Write(functionDeclName.Substring(8));
                            Visit(args[0]);
                            return;
                        }

                        default:
                        {
                            break;
                        }
                    }
                }

                var name = GetRemappedCursorName(functionDecl);
                outputBuilder.Write(name);

                outputBuilder.Write('(');

                if (args.Count != 0)
                {
                    var firstIndex = (functionDecl.DeclContext is CXXRecordDecl) ? 1 : 0;
                    Visit(args[firstIndex]);

                    for (int i = firstIndex + 1; i < args.Count; i++)
                    {
                        outputBuilder.Write(", ");
                        Visit(args[i]);
                    }
                }

                outputBuilder.Write(')');
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported callee declaration: '{calleeDecl.Kind}'. Generated bindings may be incomplete.", calleeDecl);
            }

            StopCSharpCode();
        }

        private void VisitCXXThisExpr(CXXThisExpr cxxThisExpr)
        {
            StartCSharpCode().Write("this");
            StopCSharpCode();
        }

        private void VisitCXXUuidofExpr(CXXUuidofExpr cxxUuidofExpr)
        {
            var outputBuilder = StartCSharpCode();
            outputBuilder.Write("typeof(");

            var type = cxxUuidofExpr.IsTypeOperand ? cxxUuidofExpr.TypeOperand : cxxUuidofExpr.ExprOperand.Type;
            var typeName = GetRemappedTypeName(cxxUuidofExpr, context: null, type, out _);
            outputBuilder.Write(typeName);

            outputBuilder.Write(").GUID");

            StopCSharpCode();
        }

        private void VisitDeclRefExpr(DeclRefExpr declRefExpr)
        {
            var outputBuilder = StartCSharpCode();
            if ((declRefExpr.Decl is EnumConstantDecl enumConstantDecl) && (declRefExpr.DeclContext != enumConstantDecl.DeclContext) && (enumConstantDecl.DeclContext is NamedDecl namedDecl))
            {
                var enumName = GetRemappedCursorName(namedDecl);

                if (!_config.DontUseUsingStaticsForEnums)
                {
                    outputBuilder.AddUsingDirective($"static {_config.Namespace}.{enumName}");
                }
                else
                {
                    outputBuilder.Write(enumName);
                    outputBuilder.Write(".");
                }
            }

            var name = GetRemappedCursorName(declRefExpr.Decl);
            var escapedName = (declRefExpr.Decl is FunctionDecl) ? EscapeAndStripName(name) : EscapeName(name);
            outputBuilder.Write(escapedName);

            StopCSharpCode();
        }

        private void VisitDeclStmt(DeclStmt declStmt)
        {
            var outputBuilder = StartCSharpCode();
            if (declStmt.IsSingleDecl)
            {
                Visit(declStmt.SingleDecl);
            }
            else
            {
                Visit(declStmt.Decls.First());

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
            if (IsPrevContextDecl<EnumConstantDecl>(out var _) && explicitCastExpr.Type is EnumType enumType)
            {
                outputBuilder.Write('(');
                var enumUnderlyingTypeName = GetRemappedTypeName(explicitCastExpr, context: null, enumType.Decl.IntegerType, out _);
                outputBuilder.Write(enumUnderlyingTypeName);
                outputBuilder.Write(')');
            }

            var type = explicitCastExpr.Type;

            var typeName = GetRemappedTypeName(explicitCastExpr, context: null, type, out var nativeTypeName);

            outputBuilder.Write('(');
            outputBuilder.Write(typeName);
            outputBuilder.Write(')');

            ParenthesizeStmt(explicitCastExpr.SubExprAsWritten);

            StopCSharpCode();
        }

        private void VisitFloatingLiteral(FloatingLiteral floatingLiteral)
        {
            var outputBuilder = StartCSharpCode();
            if (floatingLiteral.ValueString.EndsWith(".f"))
            {
                outputBuilder.Write(floatingLiteral.ValueString.Substring(0, floatingLiteral.ValueString.Length - 1));
                outputBuilder.Write("0f");
            }
            else
            {
                outputBuilder.Write(floatingLiteral.ValueString);

                if (floatingLiteral.ValueString.EndsWith("."))
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
                case CX_CastKind.CX_CK_NullToPointer:
                {
                    outputBuilder.Write("null");
                    break;
                }

                case CX_CastKind.CX_CK_PointerToBoolean:
                {
                    if ((subExpr is UnaryOperator unaryOperator) && (unaryOperator.Opcode == CX_UnaryOperatorKind.CX_UO_LNot))
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

                case CX_CastKind.CX_CK_IntegralCast:
                {
                    if (subExpr.Type.CanonicalType.Kind == CXTypeKind.CXType_Bool)
                    {
                        goto case CX_CastKind.CX_CK_BooleanToSignedIntegral;
                    }
                    else
                    {
                        goto default;
                    }
                }

                case CX_CastKind.CX_CK_IntegralToBoolean:
                {
                    if ((subExpr is UnaryOperator unaryOperator) && (unaryOperator.Opcode == CX_UnaryOperatorKind.CX_UO_LNot))
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

                case CX_CastKind.CX_CK_BooleanToSignedIntegral:
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
                    if ((subExpr is DeclRefExpr declRefExpr) && (declRefExpr.Decl is EnumConstantDecl enumConstantDecl))
                    {
                        ForEnumConstantDecl(implicitCastExpr, enumConstantDecl);
                    }
                    else
                    {
                        Visit(subExpr);
                    }
                    break;
                }
            }

            void ForEnumConstantDecl(ImplicitCastExpr implicitCastExpr, EnumConstantDecl enumConstantDecl)
            {
                var subExpr = implicitCastExpr.SubExprAsWritten;

                if (IsPrevContextStmt<BinaryOperator>(out var binaryOperator) && ((binaryOperator.Opcode == CX_BinaryOperatorKind.CX_BO_EQ) || (binaryOperator.Opcode == CX_BinaryOperatorKind.CX_BO_NE)))
                {
                    Visit(subExpr);
                }
                else if (IsPrevContextDecl<EnumConstantDecl>(out _))
                {
                    Visit(subExpr);
                }
                else
                {
                    var type = implicitCastExpr.Type;
                    var typeName = GetRemappedTypeName(implicitCastExpr, context: null, type, out var nativeTypeName);

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
            ForType(initListExpr, initListExpr.Type);

            void ForArrayType(InitListExpr initListExpr, ArrayType arrayType)
            {
                outputBuilder.Write("new ");

                var type = initListExpr.Type;
                var typeName = GetRemappedTypeName(initListExpr, context: null, type, out var nativeTypeName);

                outputBuilder.Write(typeName);
                outputBuilder.Write('[');

                long size = -1;

                if (arrayType is ConstantArrayType constantArrayType)
                {
                    size = constantArrayType.Size;
                }
                else
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported array type kind: '{type.KindSpelling}'. Generated bindings may be incomplete.", initListExpr);
                }

                if (size != -1)
                {
                    outputBuilder.Write(size);
                }

                outputBuilder.WriteLine(']');
                outputBuilder.WriteBlockStart();

                for (int i = 0; i < initListExpr.Inits.Count; i++)
                {
                    outputBuilder.WriteIndentation();
                    Visit(initListExpr.Inits[i]);
                    outputBuilder.WriteLine(',');
                }

                for (int i = initListExpr.Inits.Count; i < size; i++)
                {
                    outputBuilder.WriteIndentedLine("default,");
                }

                outputBuilder.DecreaseIndentation();
                outputBuilder.WriteIndented('}');
                outputBuilder.NeedsSemicolon = true;
            }

            void ForRecordType(InitListExpr initListExpr, RecordType recordType)
            {
                outputBuilder.Write("new ");

                var type = initListExpr.Type;
                var typeName = GetRemappedTypeName(initListExpr, context: null, type, out var nativeTypeName);

                outputBuilder.Write(typeName);

                if (typeName == "Guid")
                {
                    outputBuilder.Write('(');

                    Visit(initListExpr.Inits[0]);

                    outputBuilder.Write(", ");

                    Visit(initListExpr.Inits[1]);

                    outputBuilder.Write(", ");

                    Visit(initListExpr.Inits[2]);
                    initListExpr = (InitListExpr)initListExpr.Inits[3];

                    for (int i = 0; i < initListExpr.Inits.Count; i++)
                    {
                        outputBuilder.Write(", ");

                        Visit(initListExpr.Inits[i]);
                    }

                    outputBuilder.Write(')');
                }
                else
                {
                    outputBuilder.WriteNewline();
                    outputBuilder.WriteBlockStart();

                    var decl = recordType.Decl;

                    for (int i = 0; i < initListExpr.Inits.Count; i++)
                    {
                        var init = initListExpr.Inits[i];

                        if (init is ImplicitValueInitExpr)
                        {
                            continue;
                        }

                        var fieldName = GetRemappedCursorName(decl.Fields[i]);

                        outputBuilder.WriteIndented(fieldName);
                        outputBuilder.Write(" = ");
                        Visit(init);
                        outputBuilder.WriteLine(',');
                    }

                    outputBuilder.DecreaseIndentation();
                    outputBuilder.WriteIndented('}');
                    outputBuilder.NeedsSemicolon = true;
                }
            }

            void ForType(InitListExpr initListExpr, Type type)
            {
                if (type is ArrayType arrayType)
                {
                    ForArrayType(initListExpr, arrayType);
                }
                else if (type is ElaboratedType elaboratedType)
                {
                    ForType(initListExpr, elaboratedType.NamedType);
                }
                else if (type is RecordType recordType)
                {
                    ForRecordType(initListExpr, recordType);
                }
                else if (type is TypedefType typedefType)
                {
                    ForType(initListExpr, typedefType.Decl.UnderlyingType);
                }
                else
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported init list expression type: '{type.KindSpelling}'. Generated bindings may be incomplete.", initListExpr);
                }
            }

            StopCSharpCode();
        }

        private void VisitIntegerLiteral(IntegerLiteral integerLiteral)
        {
            var valueString = integerLiteral.ValueString;

            if (valueString.EndsWith("l", StringComparison.OrdinalIgnoreCase))
            {
                valueString = valueString.Substring(0, valueString.Length - 1);
            }
            else if (valueString.EndsWith("ui8", StringComparison.OrdinalIgnoreCase))
            {
                valueString = valueString.Substring(0, valueString.Length - 3);
            }
            else if (valueString.EndsWith("i8", StringComparison.OrdinalIgnoreCase))
            {
                valueString = valueString.Substring(0, valueString.Length - 2);
            }
            else if (valueString.EndsWith("ui16", StringComparison.OrdinalIgnoreCase))
            {
                valueString = valueString.Substring(0, valueString.Length - 4);
            }
            else if (valueString.EndsWith("i16", StringComparison.OrdinalIgnoreCase))
            {
                valueString = valueString.Substring(0, valueString.Length - 3);
            }
            else if (valueString.EndsWith("i32", StringComparison.OrdinalIgnoreCase))
            {
                valueString = valueString.Substring(0, valueString.Length - 3);
            }
            else if (valueString.EndsWith("i64", StringComparison.OrdinalIgnoreCase))
            {
                valueString = valueString.Substring(0, valueString.Length - 3) + "L";
            }

            if (valueString.EndsWith("ul", StringComparison.OrdinalIgnoreCase))
            {
                valueString = valueString.Substring(0, valueString.Length - 2) + "UL";
            }
            else if (valueString.EndsWith("l", StringComparison.OrdinalIgnoreCase))
            {
                valueString = valueString.Substring(0, valueString.Length - 1) + "L";
            }
            else if (valueString.EndsWith("u", StringComparison.OrdinalIgnoreCase))
            {
                valueString = valueString.Substring(0, valueString.Length - 1) + "U";
            }

            StartCSharpCode().Write(valueString);
            StopCSharpCode();
        }

        private void VisitLabelStmt(LabelStmt labelStmt)
        {
            var outputBuilder = StartCSharpCode();
            outputBuilder.Write(labelStmt.Decl.Name);
            outputBuilder.WriteLine(':');

            outputBuilder.WriteIndentation();
            Visit(labelStmt.SubStmt);
            StopCSharpCode();
        }

        private void VisitMemberExpr(MemberExpr memberExpr)
        {
            var outputBuilder = StartCSharpCode();
            if (!memberExpr.IsImplicitAccess)
            {
                Visit(memberExpr.Base);

                Type type;

                if (memberExpr.Base is CXXThisExpr)
                {
                    type = null;
                }
                else if (memberExpr.Base is DeclRefExpr declRefExpr)
                {
                    type = declRefExpr.Decl.Type.CanonicalType;
                }
                else
                {
                    type = memberExpr.Base.Type.CanonicalType;
                }

                if ((type != null) && ((type is PointerType) || (type is ReferenceType)))
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

        private void VisitParenExpr(ParenExpr parenExpr)
        {
            var outputBuilder = StartCSharpCode();
            outputBuilder.Write('(');
            Visit(parenExpr.SubExpr);
            outputBuilder.Write(')');
            StopCSharpCode();
        }

        private void VisitReturnStmt(ReturnStmt returnStmt)
        {
            var outputBuilder = StartCSharpCode();
            if (IsPrevContextDecl<FunctionDecl>(out var functionDecl) && (functionDecl.ReturnType.CanonicalType.Kind != CXTypeKind.CXType_Void))
            {
                outputBuilder.Write("return");

                if (returnStmt.RetValue != null)
                {
                    outputBuilder.Write(' ');
                    Visit(returnStmt.RetValue);
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
                // case CX_StmtClass.CX_StmtClass_GCCAsmStmt:
                // case CX_StmtClass.CX_StmtClass_MSAsmStmt:

                case CX_StmtClass.CX_StmtClass_BreakStmt:
                {
                    VisitBreakStmt((BreakStmt)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_CXXCatchStmt:
                // case CX_StmtClass.CX_StmtClass_CXXForRangeStmt:
                // case CX_StmtClass.CX_StmtClass_CXXTryStmt:
                // case CX_StmtClass.CX_StmtClass_CapturedStmt:

                case CX_StmtClass.CX_StmtClass_CompoundStmt:
                {
                    VisitCompoundStmt((CompoundStmt)stmt);
                    break;
                }

                case CX_StmtClass.CX_StmtClass_ContinueStmt:
                {
                    VisitContinueStmt((ContinueStmt)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_CoreturnStmt:
                // case CX_StmtClass.CX_StmtClass_CoroutineBodyStmt:

                case CX_StmtClass.CX_StmtClass_DeclStmt:
                {
                    VisitDeclStmt((DeclStmt)stmt);
                    break;
                }

                case CX_StmtClass.CX_StmtClass_DoStmt:
                {
                    VisitDoStmt((DoStmt)stmt);
                    break;
                }

                case CX_StmtClass.CX_StmtClass_ForStmt:
                {
                    VisitForStmt((ForStmt)stmt);
                    break;
                }

                case CX_StmtClass.CX_StmtClass_GotoStmt:
                {
                    VisitGotoStmt((GotoStmt)stmt);
                    break;
                }

                case CX_StmtClass.CX_StmtClass_IfStmt:
                {
                    VisitIfStmt((IfStmt)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_IndirectGotoStmt:
                // case CX_StmtClass.CX_StmtClass_MSDependentExistsStmt:
                // case CX_StmtClass.CX_StmtClass_NullStmt:
                // case CX_StmtClass.CX_StmtClass_OMPAtomicDirective:
                // case CX_StmtClass.CX_StmtClass_OMPBarrierDirective:
                // case CX_StmtClass.CX_StmtClass_OMPCancelDirective:
                // case CX_StmtClass.CX_StmtClass_OMPCancellationPointDirective:
                // case CX_StmtClass.CX_StmtClass_OMPCriticalDirective:
                // case CX_StmtClass.CX_StmtClass_OMPFlushDirective:
                // case CX_StmtClass.CX_StmtClass_OMPDistributeDirective:
                // case CX_StmtClass.CX_StmtClass_OMPDistributeParallelForDirective:
                // case CX_StmtClass.CX_StmtClass_OMPDistributeParallelForSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPDistributeSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPForDirective:
                // case CX_StmtClass.CX_StmtClass_OMPForSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPParallelForDirective:
                // case CX_StmtClass.CX_StmtClass_OMPParallelForSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetParallelForSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetTeamsDistributeDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetTeamsDistributeParallelForDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetTeamsDistributeParallelForSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetTeamsDistributeSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTaskLoopDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTaskLoopSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTeamsDistributeDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTeamsDistributeParallelForDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTeamsDistributeParallelForSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTeamsDistributeSimdDirective:
                // case CX_StmtClass.CX_StmtClass_OMPMasterDirective:
                // case CX_StmtClass.CX_StmtClass_OMPOrderedDirective:
                // case CX_StmtClass.CX_StmtClass_OMPParallelDirective:
                // case CX_StmtClass.CX_StmtClass_OMPParallelSectionsDirective:
                // case CX_StmtClass.CX_StmtClass_OMPSectionDirective:
                // case CX_StmtClass.CX_StmtClass_OMPSectionsDirective:
                // case CX_StmtClass.CX_StmtClass_OMPSingleDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetDataDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetEnterDataDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetExitDataDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetParallelDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetParallelForDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetTeamsDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTargetUpdateDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTaskDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTaskgroupDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTaskwaitDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTaskyieldDirective:
                // case CX_StmtClass.CX_StmtClass_OMPTeamsDirective:
                // case CX_StmtClass.CX_StmtClass_ObjCAtCatchStmt:
                // case CX_StmtClass.CX_StmtClass_ObjCAtFinallyStmt:
                // case CX_StmtClass.CX_StmtClass_ObjCAtSynchronizedStmt:
                // case CX_StmtClass.CX_StmtClass_ObjCAtThrowStmt:
                // case CX_StmtClass.CX_StmtClass_ObjCAtTryStmt:
                // case CX_StmtClass.CX_StmtClass_ObjCAutoreleasePoolStmt:
                // case CX_StmtClass.CX_StmtClass_ObjCForCollectionStmt:

                case CX_StmtClass.CX_StmtClass_ReturnStmt:
                {
                    VisitReturnStmt((ReturnStmt)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_SEHExceptStmt:
                // case CX_StmtClass.CX_StmtClass_SEHFinallyStmt:
                // case CX_StmtClass.CX_StmtClass_SEHLeaveStmt:
                // case CX_StmtClass.CX_StmtClass_SEHTryStmt:

                case CX_StmtClass.CX_StmtClass_CaseStmt:
                {
                    VisitCaseStmt((CaseStmt)stmt);
                    break;
                }

                case CX_StmtClass.CX_StmtClass_DefaultStmt:
                {
                    VisitDefaultStmt((DefaultStmt)stmt);
                    break;
                }

                case CX_StmtClass.CX_StmtClass_SwitchStmt:
                {
                    VisitSwitchStmt((SwitchStmt)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_AttributedStmt:
                // case CX_StmtClass.CX_StmtClass_BinaryConditionalOperator:

                case CX_StmtClass.CX_StmtClass_ConditionalOperator:
                {
                    VisitConditionalOperator((ConditionalOperator)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_AddrLabelExpr:
                // case CX_StmtClass.CX_StmtClass_ArrayInitIndexExpr:
                // case CX_StmtClass.CX_StmtClass_ArrayInitLoopExpr:

                case CX_StmtClass.CX_StmtClass_ArraySubscriptExpr:
                {
                    VisitArraySubscriptExpr((ArraySubscriptExpr)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_ArrayTypeTraitExpr:
                // case CX_StmtClass.CX_StmtClass_AsTypeExpr:
                // case CX_StmtClass.CX_StmtClass_AtomicExpr:

                case CX_StmtClass.CX_StmtClass_BinaryOperator:
                case CX_StmtClass.CX_StmtClass_CompoundAssignOperator:
                {
                    VisitBinaryOperator((BinaryOperator)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_BlockExpr:
                // case CX_StmtClass.CX_StmtClass_CXXBindTemporaryExpr:

                case CX_StmtClass.CX_StmtClass_CXXBoolLiteralExpr:
                {
                    VisitCXXBoolLiteralExpr((CXXBoolLiteralExpr)stmt);
                    break;
                }

                case CX_StmtClass.CX_StmtClass_CXXConstructExpr:
                {
                    VisitCXXConstructExpr((CXXConstructExpr)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_CXXTemporaryObjectExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDefaultArgExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDefaultInitExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDeleteExpr:
                // case CX_StmtClass.CX_StmtClass_CXXDependentScopeMemberExpr:
                // case CX_StmtClass.CX_StmtClass_CXXFoldExpr:
                // case CX_StmtClass.CX_StmtClass_CXXInheritedCtorInitExpr:
                // case CX_StmtClass.CX_StmtClass_CXXNewExpr:
                // case CX_StmtClass.CX_StmtClass_CXXNoexceptExpr:

                case CX_StmtClass.CX_StmtClass_CXXNullPtrLiteralExpr:
                {
                    VisitCXXNullPtrLiteralExpr((CXXNullPtrLiteralExpr)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_CXXPseudoDestructorExpr:
                // case CX_StmtClass.CX_StmtClass_CXXScalarValueInitExpr:
                // case CX_StmtClass.CX_StmtClass_CXXStdInitializerListExpr:

                case CX_StmtClass.CX_StmtClass_CXXThisExpr:
                {
                    VisitCXXThisExpr((CXXThisExpr)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_CXXThrowExpr:
                // case CX_StmtClass.CX_StmtClass_CXXTypeidExpr:
                // case CX_StmtClass.CX_StmtClass_CXXUnresolvedConstructExpr:

                case CX_StmtClass.CX_StmtClass_CXXUuidofExpr:
                {
                    VisitCXXUuidofExpr((CXXUuidofExpr)stmt);
                    break;
                }

                case CX_StmtClass.CX_StmtClass_CallExpr:
                case CX_StmtClass.CX_StmtClass_CXXMemberCallExpr:
                {
                    VisitCallExpr((CallExpr)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_CUDAKernelCallExpr:

                case CX_StmtClass.CX_StmtClass_CXXOperatorCallExpr:
                {
                    VisitCXXOperatorCallExpr((CXXOperatorCallExpr)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_UserDefinedLiteral:
                // case CX_StmtClass.CX_StmtClass_BuiltinBitCastExpr:

                case CX_StmtClass.CX_StmtClass_CStyleCastExpr:
                case CX_StmtClass.CX_StmtClass_CXXDynamicCastExpr:
                case CX_StmtClass.CX_StmtClass_CXXReinterpretCastExpr:
                case CX_StmtClass.CX_StmtClass_CXXStaticCastExpr:
                {
                    VisitExplicitCastExpr((ExplicitCastExpr)stmt);
                    break;
                }

                case CX_StmtClass.CX_StmtClass_CXXFunctionalCastExpr:
                {
                    VisitCXXFunctionalCastExpr((CXXFunctionalCastExpr)stmt);
                    break;
                }

                case CX_StmtClass.CX_StmtClass_CXXConstCastExpr:
                {
                    VisitCXXConstCastExpr((CXXConstCastExpr)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_ObjCBridgedCastExpr:

                case CX_StmtClass.CX_StmtClass_ImplicitCastExpr:
                {
                    VisitImplicitCastExpr((ImplicitCastExpr)stmt);
                    break;
                }

                case CX_StmtClass.CX_StmtClass_CharacterLiteral:
                {
                    VisitCharacterLiteral((CharacterLiteral)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_ChooseExpr:
                // case CX_StmtClass.CX_StmtClass_CompoundLiteralExpr:
                // case CX_StmtClass.CX_StmtClass_ConvertVectorExpr:
                // case CX_StmtClass.CX_StmtClass_CoawaitExpr:
                // case CX_StmtClass.CX_StmtClass_CoyieldExpr:

                case CX_StmtClass.CX_StmtClass_DeclRefExpr:
                {
                    VisitDeclRefExpr((DeclRefExpr)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_DependentCoawaitExpr:
                // case CX_StmtClass.CX_StmtClass_DependentScopeDeclRefExpr:
                // case CX_StmtClass.CX_StmtClass_DesignatedInitExpr:
                // case CX_StmtClass.CX_StmtClass_DesignatedInitUpdateExpr:
                // case CX_StmtClass.CX_StmtClass_ExpressionTraitExpr:
                // case CX_StmtClass.CX_StmtClass_ExtVectorElementExpr:
                // case CX_StmtClass.CX_StmtClass_FixedPointLiteral:

                case CX_StmtClass.CX_StmtClass_FloatingLiteral:
                {
                    VisitFloatingLiteral((FloatingLiteral)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_ConstantExpr:
                // case CX_StmtClass.CX_StmtClass_ExprWithCleanups:
                // case CX_StmtClass.CX_StmtClass_FunctionParmPackExpr:
                // case CX_StmtClass.CX_StmtClass_GNUNullExpr:
                // case CX_StmtClass.CX_StmtClass_GenericSelectionExpr:
                // case CX_StmtClass.CX_StmtClass_ImaginaryLiteral:

                case CX_StmtClass.CX_StmtClass_ImplicitValueInitExpr:
                {
                    VisitImplicitValueInitExpr((ImplicitValueInitExpr)stmt);
                    break;
                }

                case CX_StmtClass.CX_StmtClass_InitListExpr:
                {
                    VisitInitListExpr((InitListExpr)stmt);
                    break;
                }

                case CX_StmtClass.CX_StmtClass_IntegerLiteral:
                {
                    VisitIntegerLiteral((IntegerLiteral)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_LambdaExpr:
                // case CX_StmtClass.CX_StmtClass_MSPropertyRefExpr:
                // case CX_StmtClass.CX_StmtClass_MSPropertySubscriptExpr:
                // case CX_StmtClass.CX_StmtClass_MaterializeTemporaryExpr:

                case CX_StmtClass.CX_StmtClass_MemberExpr:
                {
                    VisitMemberExpr((MemberExpr)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_NoInitExpr:
                // case CX_StmtClass.CX_StmtClass_OMPArraySectionExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCArrayLiteral:
                // case CX_StmtClass.CX_StmtClass_ObjCAvailabilityCheckExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCBoolLiteralExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCBoxedExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCDictionaryLiteral:
                // case CX_StmtClass.CX_StmtClass_ObjCEncodeExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCIndirectCopyRestoreExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCIsaExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCIvarRefExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCMessageExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCPropertyRefExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCProtocolExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCSelectorExpr:
                // case CX_StmtClass.CX_StmtClass_ObjCStringLiteral:
                // case CX_StmtClass.CX_StmtClass_ObjCSubscriptRefExpr:
                // case CX_StmtClass.CX_StmtClass_OffsetOfExpr:
                // case CX_StmtClass.CX_StmtClass_OpaqueValueExpr:
                // case CX_StmtClass.CX_StmtClass_UnresolvedLookupExpr:
                // case CX_StmtClass.CX_StmtClass_UnresolvedMemberExpr:
                // case CX_StmtClass.CX_StmtClass_PackExpansionExpr:

                case CX_StmtClass.CX_StmtClass_ParenExpr:
                {
                    VisitParenExpr((ParenExpr)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_ParenListExpr:
                // case CX_StmtClass.CX_StmtClass_PredefinedExpr:
                // case CX_StmtClass.CX_StmtClass_PseudoObjectExpr:
                // case CX_StmtClass.CX_StmtClass_ShuffleVectorExpr:
                // case CX_StmtClass.CX_StmtClass_SizeOfPackExpr:
                // case CX_StmtClass.CX_StmtClass_SourceLocExpr:
                // case CX_StmtClass.CX_StmtClass_StmtExpr:

                case CX_StmtClass.CX_StmtClass_StringLiteral:
                {
                    VisitStringLiteral((StringLiteral)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_SubstNonTypeTemplateParmExpr:
                // case CX_StmtClass.CX_StmtClass_SubstNonTypeTemplateParmPackExpr:
                // case CX_StmtClass.CX_StmtClass_TypeTraitExpr:
                // case CX_StmtClass.CX_StmtClass_TypoExpr:

                case CX_StmtClass.CX_StmtClass_UnaryExprOrTypeTraitExpr:
                {
                    VisitUnaryExprOrTypeTraitExpr((UnaryExprOrTypeTraitExpr)stmt);
                    break;
                }

                case CX_StmtClass.CX_StmtClass_UnaryOperator:
                {
                    VisitUnaryOperator((UnaryOperator)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_VAArgExpr:

                case CX_StmtClass.CX_StmtClass_LabelStmt:
                {
                    VisitLabelStmt((LabelStmt)stmt);
                    break;
                }

                case CX_StmtClass.CX_StmtClass_WhileStmt:
                {
                    VisitWhileStmt((WhileStmt)stmt);
                    break;
                }

                default:
                {
                    var context = string.Empty;

                    if (IsPrevContextDecl<NamedDecl>(out var namedDecl))
                    {
                        context = $" in {GetCursorQualifiedName(namedDecl)}";
                    }

                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported statement: '{stmt.StmtClass}'{context}. Generated bindings may be incomplete.", stmt);
                    break;
                }
            }
        }

        private void VisitStmts(IReadOnlyList<Stmt> stmts)
        {
            var outputBuilder = StartCSharpCode();
            var lastIndex = stmts.Count - 1;
            var previousStmt = null as Stmt;

            for (int i = 0; i < lastIndex; i++)
            {
                var stmt = stmts[i];

                if ((previousStmt is DeclStmt) && !(stmt is DeclStmt))
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

                if ((previousStmt is DeclStmt) && !(stmt is DeclStmt))
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
                case CX_CharacterKind.CX_CLK_Ascii:
                case CX_CharacterKind.CX_CLK_UTF8:
                {
                    outputBuilder.Write("new byte[] { ");

                    var bytes = Encoding.UTF8.GetBytes(stringLiteral.String);

                    foreach (var b in bytes)
                    {
                        outputBuilder.Write("0x");
                        outputBuilder.Write(b.ToString("X2"));
                        outputBuilder.Write(", ");
                    }

                    outputBuilder.Write("0x00 }");
                    break;
                }

                case CX_CharacterKind.CX_CLK_Wide:
                {
                    if (_config.GenerateUnixTypes)
                    {
                        goto default;
                    }

                    goto case CX_CharacterKind.CX_CLK_UTF16;
                }

                case CX_CharacterKind.CX_CLK_UTF16:
                {
                    outputBuilder.Write('"');
                    outputBuilder.Write(EscapeString(stringLiteral.String));
                    outputBuilder.Write('"');
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

            long size32;
            long size64;

            long alignment32 = -1;
            long alignment64 = -1;

            GetTypeSize(unaryExprOrTypeTraitExpr, argumentType, ref alignment32, ref alignment64, out size32, out size64);

            switch (unaryExprOrTypeTraitExpr.Kind)
            {
                case CX_UnaryExprOrTypeTrait.CX_UETT_SizeOf:
                {
                    if ((size32 == size64) && IsPrevContextDecl<VarDecl>(out _))
                    {
                        outputBuilder.Write(size32);
                    }
                    else
                    {
                        if (_outputBuilder.Name == _config.MethodClassName)
                        {
                            _isMethodClassUnsafe = true;
                        }

                        var parentType = null as Type;

                        if (IsPrevContextStmt<CallExpr>(out var callExpr))
                        {
                            var index = callExpr.Args.IndexOf(unaryExprOrTypeTraitExpr);
                            var calleeDecl = callExpr.CalleeDecl;

                            if (calleeDecl is FunctionDecl functionDecl)
                            {
                                parentType = functionDecl.Parameters[index].Type.CanonicalType;
                            }
                            else
                            {
                                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported callee declaration: '{calleeDecl?.Kind}'. Generated bindings may be incomplete.", calleeDecl);
                            }

                        }
                        else if (IsPrevContextStmt<Expr>(out var expr))
                        {
                            parentType = expr.Type.CanonicalType;
                        }
                        else if (IsPrevContextDecl<TypeDecl>(out var typeDecl))
                        {
                            parentType = typeDecl.TypeForDecl.CanonicalType;
                        }

                        var needsCast = false;
                        var typeName = GetRemappedTypeName(unaryExprOrTypeTraitExpr, context: null, argumentType, out _);

                        if (parentType != null)
                        {
                            needsCast = (parentType.Kind == CXTypeKind.CXType_UInt);
                            needsCast |= (parentType.Kind == CXTypeKind.CXType_ULong);
                            needsCast &= !IsSupportedFixedSizedBufferType(typeName);
                            needsCast &= (argumentType.CanonicalType.Kind != CXTypeKind.CXType_Enum);
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

                case CX_UnaryExprOrTypeTrait.CX_UETT_AlignOf:
                case CX_UnaryExprOrTypeTrait.CX_UETT_PreferredAlignOf:
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
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported unary or type trait expression: '{unaryExprOrTypeTraitExpr.Kind}'. Generated bindings may be incomplete.", unaryExprOrTypeTraitExpr);
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
                case CX_UnaryOperatorKind.CX_UO_PostInc:
                case CX_UnaryOperatorKind.CX_UO_PostDec:
                {
                    Visit(unaryOperator.SubExpr);
                    outputBuilder.Write(unaryOperator.OpcodeStr);
                    break;
                }

                case CX_UnaryOperatorKind.CX_UO_PreInc:
                case CX_UnaryOperatorKind.CX_UO_PreDec:
                case CX_UnaryOperatorKind.CX_UO_Deref:
                case CX_UnaryOperatorKind.CX_UO_Plus:
                case CX_UnaryOperatorKind.CX_UO_Minus:
                case CX_UnaryOperatorKind.CX_UO_Not:
                {
                    outputBuilder.Write(unaryOperator.OpcodeStr);
                    Visit(unaryOperator.SubExpr);
                    break;
                }

                case CX_UnaryOperatorKind.CX_UO_LNot:
                {
                    var subExpr = GetExprAsWritten(unaryOperator.SubExpr, removeParens: true);
                    var canonicalType = subExpr.Type.CanonicalType;

                    if (canonicalType.IsIntegerType && (canonicalType.Kind != CXTypeKind.CXType_Bool))
                    {
                        Visit(subExpr);
                        outputBuilder.Write(" == 0");
                    }
                    else if ((canonicalType is PointerType) || (canonicalType is ReferenceType))
                    {
                        Visit(subExpr);
                        outputBuilder.Write(" == null");
                    }
                    else
                    {
                        outputBuilder.Write(unaryOperator.OpcodeStr);
                        Visit(unaryOperator.SubExpr);
                    }
                    break;
                }

                case CX_UnaryOperatorKind.CX_UO_AddrOf:
                {
                    if ((unaryOperator.SubExpr is DeclRefExpr declRefExpr) && (declRefExpr.Decl.Type is LValueReferenceType))
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
}
