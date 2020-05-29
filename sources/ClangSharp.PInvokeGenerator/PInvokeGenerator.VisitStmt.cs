// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public partial class PInvokeGenerator
    {
        private void VisitArraySubscriptExpr(ArraySubscriptExpr arraySubscriptExpr)
        {
            Visit(arraySubscriptExpr.Base);
            _outputBuilder.Write('[');
            Visit(arraySubscriptExpr.Idx);
            _outputBuilder.Write(']');
        }

        private void VisitBinaryOperator(BinaryOperator binaryOperator)
        {
            Visit(binaryOperator.LHS);
            _outputBuilder.Write(' ');
            _outputBuilder.Write(binaryOperator.OpcodeStr);
            _outputBuilder.Write(' ');
            Visit(binaryOperator.RHS);
        }

        private void VisitBreakStmt(BreakStmt breakStmt)
        {
            _outputBuilder.Write("break");
        }

        private void VisitCallExpr(CallExpr callExpr)
        {
            var calleeDecl = callExpr.CalleeDecl;

            if (calleeDecl is FunctionDecl)
            {
                VisitStmt(callExpr.Callee);
                _outputBuilder.Write('(');

                var args = callExpr.Args;

                if (args.Count != 0)
                {
                    Visit(args[0]);

                    for (int i = 1; i < args.Count; i++)
                    {
                        _outputBuilder.Write(',');
                        _outputBuilder.Write(' ');

                        if ((args[i] is UnaryExprOrTypeTraitExpr unaryExprOrTypeTraitExpr) && (unaryExprOrTypeTraitExpr.Kind == CX_UnaryExprOrTypeTrait.CX_UETT_SizeOf))
                        {
                            var argumentCanonicalType = unaryExprOrTypeTraitExpr.TypeOfArgument.CanonicalType;

                            if ((argumentCanonicalType.TypeClass != CX_TypeClass.CX_TypeClass_Builtin) &&
                                (argumentCanonicalType.TypeClass != CX_TypeClass.CX_TypeClass_Enum))
                            {
                                if ((args[i].Type.Kind == CXTypeKind.CXType_UInt))
                                {
                                    _outputBuilder.Write('(');
                                    _outputBuilder.Write("uint");
                                    _outputBuilder.Write(')');
                                }
                            }
                        }

                        Visit(args[i]);
                    }
                }

                _outputBuilder.Write(")");
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported callee declaration: '{calleeDecl.Kind}'. Generated bindings may be incomplete.", calleeDecl);
            }
        }

        private void VisitCaseStmt(CaseStmt caseStmt)
        {
            _outputBuilder.Write("case");
            _outputBuilder.Write(' ');
            Visit(caseStmt.LHS);
            _outputBuilder.WriteLine(':');

            if (caseStmt.SubStmt is CompoundStmt)
            {
                Visit(caseStmt.SubStmt);
            }
            else if (caseStmt.SubStmt is SwitchCase)
            {
                _outputBuilder.WriteIndentation();

                _outputBuilder.NeedsSemicolon = true;
                Visit(caseStmt.SubStmt);
            }
            else
            {
                _outputBuilder.IncreaseIndentation();
                _outputBuilder.WriteIndentation();

                _outputBuilder.NeedsSemicolon = true;
                Visit(caseStmt.SubStmt);

                _outputBuilder.DecreaseIndentation();
            }

            _outputBuilder.NeedsNewline = true;
        }

        private void VisitCharacterLiteral(CharacterLiteral characterLiteral)
        {
            _outputBuilder.Write(characterLiteral.Value);
        }

        private void VisitCompoundStmt(CompoundStmt compoundStmt)
        {
            _outputBuilder.WriteBlockStart();
            _outputBuilder.NeedsSemicolon = true;

            VisitStmts(compoundStmt.Body);
            _outputBuilder.WriteBlockEnd();
        }

        private void VisitConditionalOperator(ConditionalOperator conditionalOperator)
        {
            Visit(conditionalOperator.Cond);
            _outputBuilder.Write(' ');
            _outputBuilder.Write('?');
            _outputBuilder.Write(' ');
            Visit(conditionalOperator.TrueExpr);
            _outputBuilder.Write(' ');
            _outputBuilder.Write(':');
            _outputBuilder.Write(' ');
            Visit(conditionalOperator.FalseExpr);
        }

        private void VisitContinueStmt(ContinueStmt continueStmt)
        {
            _outputBuilder.Write("continue");
        }

        private void VisitCXXBoolLiteralExpr(CXXBoolLiteralExpr cxxBoolLiteralExpr)
        {
            _outputBuilder.Write(cxxBoolLiteralExpr.Value);
        }

        private void VisitCXXConstCastExpr(CXXConstCastExpr cxxConstCastExpr)
        {
            // C# doesn't have a concept of const pointers so
            // ignore rather than adding a cast from T* to T*

            Visit(cxxConstCastExpr.SubExpr);
        }

        private void VisitCXXConstructExpr(CXXConstructExpr cxxConstructExpr)
        {
            var args = cxxConstructExpr.Args;

            if (args.Count != 0)
            {
                Visit(args[0]);

                for (int i = 1; i < args.Count; i++)
                {
                    _outputBuilder.Write(',');
                    _outputBuilder.Write(' ');
                    Visit(args[i]);
                }
            }
        }

        private void VisitCXXFunctionalCastExpr(CXXFunctionalCastExpr cxxFunctionalCastExpr)
        {
            if (cxxFunctionalCastExpr.SubExpr is CXXConstructExpr)
            {
                _outputBuilder.Write("new");
                _outputBuilder.Write(' ');

                var type = cxxFunctionalCastExpr.Type;
                var typeName = GetRemappedTypeName(cxxFunctionalCastExpr, type, out var nativeTypeName);

                _outputBuilder.Write(typeName);
                _outputBuilder.Write('(');
                Visit(cxxFunctionalCastExpr.SubExpr);
                _outputBuilder.Write(')');
            }
            else
            {
                VisitExplicitCastExpr(cxxFunctionalCastExpr);
            }
        }

        private void VisitCXXNullPtrLiteralExpr(CXXNullPtrLiteralExpr cxxNullPtrLiteralExpr)
        {
            _outputBuilder.Write("null");
        }

        private void VisitCXXOperatorCallExpr(CXXOperatorCallExpr cxxOperatorCallExpr)
        {
            var calleeDecl = cxxOperatorCallExpr.CalleeDecl;

            if (calleeDecl is FunctionDecl functionDecl)
            {
                if (functionDecl.DeclContext is CXXRecordDecl)
                {
                    VisitStmt(cxxOperatorCallExpr.Callee);
                    _outputBuilder.Write('.');
                }

                var name = GetRemappedCursorName(functionDecl);
                _outputBuilder.Write(name);

                _outputBuilder.Write('(');

                var args = cxxOperatorCallExpr.Args;

                if (args.Count != 0)
                {
                    var firstIndex = (functionDecl.DeclContext is CXXRecordDecl) ? 1 : 0;
                    Visit(args[firstIndex]);

                    for (int i = firstIndex + 1; i < args.Count; i++)
                    {
                        _outputBuilder.Write(',');
                        _outputBuilder.Write(' ');
                        Visit(args[i]);
                    }
                }

                _outputBuilder.Write(")");
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported callee declaration: '{calleeDecl.Kind}'. Generated bindings may be incomplete.", calleeDecl);
            }
        }

        private void VisitCXXThisExpr(CXXThisExpr cxxThisExpr)
        {
            _outputBuilder.Write("this");
        }

        private void VisitDeclRefExpr(DeclRefExpr declRefExpr)
        {
            var name = GetRemappedCursorName(declRefExpr.Decl);
            _outputBuilder.Write(EscapeAndStripName(name));
        }

        private void VisitDeclStmt(DeclStmt declStmt)
        {
            if (declStmt.IsSingleDecl)
            {
                VisitDecl(declStmt.SingleDecl);
            }
            else
            {
                VisitDecl(declStmt.Decls.First());

                foreach (var decl in declStmt.Decls.Skip(1))
                {
                    _outputBuilder.Write(',');
                    _outputBuilder.Write(' ');
                    VisitDecl(decl);
                }
            }

            _outputBuilder.NeedsNewline = true;
        }

        private void VisitDefaultStmt(DefaultStmt defaultStmt)
        {
            _outputBuilder.Write("default");
            _outputBuilder.WriteLine(':');

            if (defaultStmt.SubStmt != null)
            {
                if (defaultStmt.SubStmt is CompoundStmt)
                {
                    Visit(defaultStmt.SubStmt);
                }

                else if (defaultStmt.SubStmt is SwitchCase)
                {
                    _outputBuilder.WriteIndentation();

                    _outputBuilder.NeedsSemicolon = true;
                    Visit(defaultStmt.SubStmt);
                }
                else
                {
                    _outputBuilder.IncreaseIndentation();
                    _outputBuilder.WriteIndentation();

                    _outputBuilder.NeedsSemicolon = true;
                    Visit(defaultStmt.SubStmt);

                    _outputBuilder.DecreaseIndentation();
                }
                _outputBuilder.NeedsNewline = true;
            }
        }

        private void VisitDoStmt(DoStmt doStmt)
        {
            _outputBuilder.WriteLine("do");

            if (doStmt.Body is CompoundStmt)
            {
                Visit(doStmt.Body);
            }
            else
            {
                _outputBuilder.IncreaseIndentation();
                _outputBuilder.WriteIndentation();

                _outputBuilder.NeedsSemicolon = true;
                Visit(doStmt.Body);

                _outputBuilder.WriteSemicolonIfNeeded();
                _outputBuilder.DecreaseIndentation();
            }

            _outputBuilder.WriteIndented("while");
            _outputBuilder.Write(' ');
            _outputBuilder.Write('(');

            Visit(doStmt.Cond);
            _outputBuilder.Write(')');

            _outputBuilder.NeedsSemicolon = true;
            _outputBuilder.NeedsNewline = true;
        }

        private void VisitExplicitCastExpr(ExplicitCastExpr explicitCastExpr)
        {
            var type = explicitCastExpr.Type;
            var typeName = GetRemappedTypeName(explicitCastExpr, type, out var nativeTypeName);

            _outputBuilder.Write('(');
            _outputBuilder.Write(typeName);
            _outputBuilder.Write(')');

            Visit(explicitCastExpr.SubExpr);
        }

        private void VisitFloatingLiteral(FloatingLiteral floatingLiteral)
        {
            _outputBuilder.Write(floatingLiteral.Value);
        }

        private void VisitForStmt(ForStmt forStmt)
        {
            _outputBuilder.Write("for");
            _outputBuilder.Write(' ');
            _outputBuilder.Write('(');

            if (forStmt.ConditionVariableDeclStmt != null)
            {
                Visit(forStmt.ConditionVariableDeclStmt);
            }
            else if (forStmt.Init != null)
            {
                Visit(forStmt.Init);
            }
            _outputBuilder.Write(';');

            if (forStmt.Cond != null)
            {
                _outputBuilder.Write(' ');
                Visit(forStmt.Cond);
            }
            _outputBuilder.Write(';');

            if (forStmt.Inc != null)
            {
                _outputBuilder.Write(' ');
                Visit(forStmt.Inc);
            }
            _outputBuilder.Write(')');
            _outputBuilder.NeedsNewline = true;

            if (forStmt.Body is CompoundStmt)
            {
                Visit(forStmt.Body);
            }
            else
            {
                _outputBuilder.IncreaseIndentation();
                _outputBuilder.WriteIndentation();

                _outputBuilder.NeedsSemicolon = true;
                Visit(forStmt.Body);

                _outputBuilder.DecreaseIndentation();
            }

            _outputBuilder.NeedsNewline = true;
        }

        private void VisitIfStmt(IfStmt ifStmt)
        {
            _outputBuilder.Write("if");
            _outputBuilder.Write(' ');
            _outputBuilder.Write('(');

            Visit(ifStmt.Cond);

            _outputBuilder.WriteLine(')');

            if (ifStmt.Then is CompoundStmt)
            {
                Visit(ifStmt.Then);
            }
            else
            {
                _outputBuilder.IncreaseIndentation();
                _outputBuilder.WriteIndentation();

                _outputBuilder.NeedsSemicolon = true;
                Visit(ifStmt.Then);

                if (ifStmt.Else != null)
                {
                    _outputBuilder.WriteSemicolonIfNeeded();
                }
                _outputBuilder.DecreaseIndentation();
            }

            if (ifStmt.Else != null)
            {
                _outputBuilder.WriteIndented("else");
                _outputBuilder.NeedsNewline = true;

                if (ifStmt.Else is CompoundStmt)
                {
                    Visit(ifStmt.Else);
                }
                else if (ifStmt.Else is IfStmt)
                {
                    _outputBuilder.Write(' ');
                    _outputBuilder.NeedsNewline = false;
                    Visit(ifStmt.Else);
                }
                else
                {
                    _outputBuilder.IncreaseIndentation();
                    _outputBuilder.WriteIndentation();

                    _outputBuilder.NeedsSemicolon = true;
                    Visit(ifStmt.Else);

                    _outputBuilder.DecreaseIndentation();
                }
            }

            _outputBuilder.NeedsNewline = true;
        }

        private void VisitImplicitCastExpr(ImplicitCastExpr implicitCastExpr)
        {
            bool handled = false;

            if (implicitCastExpr.SubExpr is IntegerLiteral integerLiteral)
            {
                handled = VisitImplicitCastExpr(implicitCastExpr, integerLiteral);
            }


            if (!handled)
            {
                Visit(implicitCastExpr.SubExpr);
            }
        }

        private bool VisitImplicitCastExpr(ImplicitCastExpr implicitCastExpr, IntegerLiteral integerLiteral)
        {
            if (implicitCastExpr.Type is PointerType)
            {
                if (integerLiteral.Value.Equals("0"))
                {
                    // C# doesn't have implicit conversion from zero to a pointer
                    // so we will manually check and handle the most common case

                    _outputBuilder.Write("null");
                    return true;
                }

                return false;
            }

            if (!(implicitCastExpr.Type is BuiltinType))
            {
                return false;
            }

            var builtinType = (BuiltinType)implicitCastExpr.Type;

            if (!builtinType.IsIntegerType)
            {
                return false;
            }

            if (implicitCastExpr.DeclContext is EnumDecl enumDecl)
            {
                var enumDeclName = GetRemappedCursorName(enumDecl);
                var enumDeclIntegerTypeName = GetRemappedTypeName(enumDecl, enumDecl.IntegerType, out var nativeTypeName);

                WithType("*", ref enumDeclIntegerTypeName, ref nativeTypeName);
                WithType(enumDeclName, ref enumDeclIntegerTypeName, ref nativeTypeName);

                var integerLiteralTypeName = GetRemappedTypeName(integerLiteral, integerLiteral.Type, out _);

                if (enumDeclIntegerTypeName == integerLiteralTypeName)
                {
                    return false;
                }
            }

            switch (builtinType.Kind)
            {
                case CXTypeKind.CXType_Int:
                {
                    _outputBuilder.Write("unchecked");
                    _outputBuilder.Write('(');
                    _outputBuilder.Write('(');
                    _outputBuilder.Write("int");
                    _outputBuilder.Write(')');

                    Visit(implicitCastExpr.SubExpr);

                    _outputBuilder.Write(')');
                    return true;
                }

                default:
                {

                    return false;
                }
            }
        }

        private void VisitInitListExpr(InitListExpr initListExpr)
        {
            VisitInitListExprForType(initListExpr, initListExpr.Type);
        }

        private void VisitInitListExprForArrayType(InitListExpr initListExpr, ArrayType arrayType)
        {
            _outputBuilder.Write("new");
            _outputBuilder.Write(' ');

            var type = initListExpr.Type;
            var typeName = GetRemappedTypeName(initListExpr, type, out var nativeTypeName);

            _outputBuilder.Write(typeName);
            _outputBuilder.Write('[');

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
                _outputBuilder.Write(size);
            }

            _outputBuilder.WriteLine(']');
            _outputBuilder.WriteBlockStart();

            for (int i = 0; i < initListExpr.Inits.Count; i++)
            {
                _outputBuilder.WriteIndentation();
                Visit(initListExpr.Inits[i]);
                _outputBuilder.WriteLine(',');
            }

            for (int i = initListExpr.Inits.Count; i < size; i++)
            {
                _outputBuilder.WriteIndented("default");
                _outputBuilder.WriteLine(',');
            }

            _outputBuilder.NeedsNewline = false;
            _outputBuilder.NeedsSemicolon = false;
            _outputBuilder.DecreaseIndentation();
            _outputBuilder.WriteIndented('}');
            _outputBuilder.WriteLine(';');
        }

        private void VisitInitListExprForRecordType(InitListExpr initListExpr, RecordType recordType)
        {
            _outputBuilder.Write("new");
            _outputBuilder.Write(' ');

            var type = initListExpr.Type;
            var typeName = GetRemappedTypeName(initListExpr, type, out var nativeTypeName);

            _outputBuilder.WriteLine(typeName);
            _outputBuilder.WriteBlockStart();

            var decl = (RecordDecl)recordType.Decl;

            for (int i = 0; i < initListExpr.Inits.Count; i++)
            {
                var fieldName = GetRemappedCursorName(decl.Fields[i]);

                _outputBuilder.WriteIndented(fieldName);
                _outputBuilder.Write(' ');
                _outputBuilder.Write('=');
                _outputBuilder.Write(' ');
                Visit(initListExpr.Inits[i]);
                _outputBuilder.WriteLine(',');
            }

            _outputBuilder.NeedsNewline = false;
            _outputBuilder.NeedsSemicolon = false;
            _outputBuilder.DecreaseIndentation();
            _outputBuilder.WriteIndented('}');
            _outputBuilder.WriteLine(';');
        }

        private void VisitInitListExprForType(InitListExpr initListExpr, Type type)
        {
            if (type is ArrayType arrayType)
            {
                VisitInitListExprForArrayType(initListExpr, arrayType);
            }
            else if (type is ElaboratedType elaboratedType)
            {
                VisitInitListExprForType(initListExpr, elaboratedType.NamedType);
            }
            else if (type is RecordType recordType)
            {
                VisitInitListExprForRecordType(initListExpr, recordType);
            }
            else if (type is TypedefType typedefType)
            {
                VisitInitListExprForType(initListExpr, typedefType.Decl.UnderlyingType);
            }
            else
            {
                AddDiagnostic(DiagnosticLevel.Error, $"Unsupported init list expression type: '{type.KindSpelling}'. Generated bindings may be incomplete.", initListExpr);
            }
        }

        private void VisitIntegerLiteral(IntegerLiteral integerLiteral)
        {
            var value = integerLiteral.Value;

            if (value.EndsWith("l") || value.EndsWith("L"))
            {
                value = value.Substring(0, value.Length - 1);
            }

            _outputBuilder.Write(value);
        }

        private void VisitMemberExpr(MemberExpr memberExpr)
        {
            if (memberExpr.Base != null)
            {
                Visit(memberExpr.Base);

                bool isPointerType;

                if (memberExpr.Base is CXXThisExpr)
                {
                    isPointerType = false;
                }
                else if (memberExpr.Base is DeclRefExpr declRefExpr)
                {
                    isPointerType = (declRefExpr.Decl.Type is PointerType) || (declRefExpr.Decl.Type is ReferenceType);
                }
                else
                {
                    isPointerType = (memberExpr.Base.Type is PointerType) || (memberExpr.Base.Type is ReferenceType);
                }

                if (isPointerType)
                {
                    _outputBuilder.Write('-');
                    _outputBuilder.Write('>');
                }
                else
                {
                    _outputBuilder.Write('.');
                }
            }
            _outputBuilder.Write(GetRemappedCursorName(memberExpr.MemberDecl));
        }

        private void VisitParenExpr(ParenExpr parenExpr)
        {
            _outputBuilder.Write('(');
            Visit(parenExpr.SubExpr);
            _outputBuilder.Write(')');
        }

        private void VisitReturnStmt(ReturnStmt returnStmt)
        {
            var retValue = returnStmt.RetValue;

            if ((retValue is null) || (retValue.Type.Kind != CXTypeKind.CXType_Void))
            {
                _outputBuilder.Write("return");
            }

            if (returnStmt.RetValue != null)
            {
                _outputBuilder.Write(' ');
                Visit(returnStmt.RetValue);
            }
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

                // case CX_StmtClass.CX_StmtClass_GotoStmt:

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
                // case CX_StmtClass.CX_StmtClass_CXXUuidofExpr:

                case CX_StmtClass.CX_StmtClass_CallExpr:
                {
                    VisitCallExpr((CallExpr)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_CUDAKernelCallExpr:

                case CX_StmtClass.CX_StmtClass_CXXMemberCallExpr:
                {
                    VisitCallExpr((CallExpr)stmt);
                    break;
                }

                case CX_StmtClass.CX_StmtClass_CXXOperatorCallExpr:
                {
                    VisitCXXOperatorCallExpr((CXXOperatorCallExpr)stmt);
                    break;
                }

                // case CX_StmtClass.CX_StmtClass_UserDefinedLiteral:
                // case CX_StmtClass.CX_StmtClass_BuiltinBitCastExpr:

                case CX_StmtClass.CX_StmtClass_CStyleCastExpr:
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

                case CX_StmtClass.CX_StmtClass_CXXDynamicCastExpr:
                case CX_StmtClass.CX_StmtClass_CXXReinterpretCastExpr:
                case CX_StmtClass.CX_StmtClass_CXXStaticCastExpr:
                {
                    VisitExplicitCastExpr((ExplicitCastExpr)stmt);
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
                // case CX_StmtClass.CX_StmtClass_ImplicitValueInitExpr:

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
                // case CX_StmtClass.CX_StmtClass_StringLiteral:
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
                // case CX_StmtClass.CX_StmtClass_LabelStmt:

                case CX_StmtClass.CX_StmtClass_WhileStmt:
                {
                    VisitWhileStmt((WhileStmt)stmt);
                    break;
                }

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported statement: '{stmt.StmtClass}'. Generated bindings may be incomplete.", stmt);
                    break;
                }
            }
        }

        private void VisitStmts(IEnumerable<Stmt> stmts)
        {
            Stmt previousStmt = null;

            foreach (var stmt in stmts)
            {
                if ((previousStmt is DeclStmt declStmt) && (stmt is DeclStmt))
                {
                    _outputBuilder.NeedsNewline = false;
                }

                _outputBuilder.WriteIndentation();
                Visit(stmt);
                _outputBuilder.WriteSemicolonIfNeeded();

                previousStmt = stmt;
            }
        }

        private void VisitSwitchStmt(SwitchStmt switchStmt)
        {
            _outputBuilder.Write("switch");
            _outputBuilder.Write(' ');
            _outputBuilder.Write('(');

            Visit(switchStmt.Cond);

            _outputBuilder.WriteLine(')');

            if (switchStmt.Body is CompoundStmt)
            {
                Visit(switchStmt.Body);
            }
            else
            {
                _outputBuilder.WriteBlockStart();
                _outputBuilder.WriteIndentation();

                _outputBuilder.NeedsSemicolon = true;
                Visit(switchStmt.Body);

                _outputBuilder.WriteSemicolonIfNeeded();
                _outputBuilder.WriteBlockEnd();
            }

            _outputBuilder.NeedsNewline = true;
        }

        private void VisitUnaryExprOrTypeTraitExpr(UnaryExprOrTypeTraitExpr unaryExprOrTypeTraitExpr)
        {
            var argumentType = unaryExprOrTypeTraitExpr.TypeOfArgument;

            switch (unaryExprOrTypeTraitExpr.Kind)
            {
                case CX_UnaryExprOrTypeTrait.CX_UETT_SizeOf:
                {
                    _outputBuilder.Write("sizeof");
                    _outputBuilder.Write('(');

                    var typeName = GetRemappedTypeName(unaryExprOrTypeTraitExpr, argumentType, out _);
                    _outputBuilder.Write(typeName);

                    _outputBuilder.Write(')');
                    break;
                }

                default:
                {
                    AddDiagnostic(DiagnosticLevel.Error, $"Unsupported unary or type trait expression: '{unaryExprOrTypeTraitExpr.Kind}'. Generated bindings may be incomplete.", unaryExprOrTypeTraitExpr);
                    break;
                }
            }
        }

        private void VisitUnaryOperator(UnaryOperator unaryOperator)
        {
            switch (unaryOperator.Opcode)
            {
                case CX_UnaryOperatorKind.CX_UO_PostInc:
                case CX_UnaryOperatorKind.CX_UO_PostDec:
                {
                    Visit(unaryOperator.SubExpr);
                    _outputBuilder.Write(unaryOperator.OpcodeStr);
                    break;
                }

                case CX_UnaryOperatorKind.CX_UO_PreInc:
                case CX_UnaryOperatorKind.CX_UO_PreDec:
                case CX_UnaryOperatorKind.CX_UO_Deref:
                case CX_UnaryOperatorKind.CX_UO_Plus:
                case CX_UnaryOperatorKind.CX_UO_Minus:
                case CX_UnaryOperatorKind.CX_UO_Not:
                case CX_UnaryOperatorKind.CX_UO_LNot:
                {
                    _outputBuilder.Write(unaryOperator.OpcodeStr);
                    Visit(unaryOperator.SubExpr);
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
                        _outputBuilder.Write(unaryOperator.OpcodeStr);
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
        }

        private void VisitWhileStmt(WhileStmt whileStmt)
        {
            _outputBuilder.Write("while");
            _outputBuilder.Write(' ');
            _outputBuilder.Write('(');

            Visit(whileStmt.Cond);

            _outputBuilder.WriteLine(')');

            if (whileStmt.Body is CompoundStmt)
            {
                Visit(whileStmt.Body);
            }
            else
            {
                _outputBuilder.IncreaseIndentation();
                _outputBuilder.WriteIndentation();

                _outputBuilder.NeedsSemicolon = true;
                Visit(whileStmt.Body);

                _outputBuilder.DecreaseIndentation();
            }

            _outputBuilder.NeedsNewline = true;
        }
    }
}
