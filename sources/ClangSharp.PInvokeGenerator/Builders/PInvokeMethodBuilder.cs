using System.Collections.Generic;
using System.Runtime.InteropServices;
using ClangSharp.Interop;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

using NativeType = ClangSharp.Type;
using ManagedType = System.Type;
using System.Diagnostics;

namespace ClangSharp
{
    public sealed class PInvokeMethodBuilder
    {
        private readonly PInvokeBuilder _builder;

        public FunctionDecl NativeMethod { get; }

        public PInvokeMethodBuilder(PInvokeBuilder builder,
            FunctionDecl nativeMethod)
        {
            _builder = builder;
            NativeMethod = nativeMethod;
        }

        public MethodDeclarationSyntax Build()
        {
            return MethodDeclaration(
                attributeLists: new SyntaxList<AttributeListSyntax>(
                    BuildAttributes()),
                modifiers: new SyntaxTokenList(GetMethodModifiers(NativeMethod)),
                returnType: BuildManagedType(NativeMethod.ReturnType),
                explicitInterfaceSpecifier: null,
                identifier: Identifier(NativeMethod.Name),
                typeParameterList: null,
                parameterList: ParameterList(SeparatedList(
                    BuildParameters())),
                constraintClauses: default,
                body: null,
                expressionBody: null,
                semicolonToken: Token(SyntaxKind.SemicolonToken)
            ).WithLeadingTrivia(
                TriviaList(BuildComments(NativeMethod)));

            static IEnumerable<SyntaxToken> GetMethodModifiers(FunctionDecl method)
            {
                yield return Token(SyntaxKind.PublicKeyword);

                if (method.ReturnType.Kind == CXTypeKind.CXType_Pointer)
                    yield return Token(SyntaxKind.UnsafeKeyword);

                yield return Token(SyntaxKind.StaticKeyword);
                yield return Token(SyntaxKind.ExternKeyword);
            }
        }

        private IEnumerable<AttributeListSyntax> BuildAttributes()
        {
            if (NativeMethod.ReturnType != NativeMethod.ReturnType.CanonicalType)
                yield return AttributeList(
                    AttributeTargetSpecifier(
                        Identifier("return")),
                    SingletonSeparatedList(
                        Attribute(
                            IdentifierName("NativeTypeName"),
                            AttributeArgumentList(
                                SingletonSeparatedList(
                                    AttributeArgument(
                                        LiteralExpression(SyntaxKind.StringLiteralExpression,
                                            Literal(NativeMethod.ReturnType.AsString))
                                    )
                                )
                            ))
                    ));

            yield return AttributeList(
                SingletonSeparatedList(
                    Attribute(
                        QualifiedName(
                            QualifiedName(
                                QualifiedName(
                                    IdentifierName("System"),
                                    IdentifierName("Runtime")
                                ),
                                IdentifierName("InteropServices")
                            ),
                            IdentifierName("DllImport")
                        ),
                        AttributeArgumentList(
                            SeparatedList(GenerateDllImportArguments(NativeMethod, _builder))
                        ))
                ));

            static IEnumerable<AttributeArgumentSyntax> GenerateDllImportArguments(
                FunctionDecl decl, PInvokeBuilder builder)
            {
                yield return AttributeArgument(
                        LiteralExpression(SyntaxKind.StringLiteralExpression,
                            Literal(builder.LibraryName)));

                var callingConvention = CallingConvention.Cdecl;

                if (decl.HasAttrs)
                {
                    foreach (var attr in decl.Attrs)
                    {
                        if (attr.Kind == CX_AttrKind.CX_AttrKind_CDecl)
                        {
                            callingConvention = CallingConvention.Cdecl;
                        }
                    }
                }

                yield return AttributeArgument(
                    AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName("CallingConvention"),
                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("System"),
                                        IdentifierName("Runtime")
                                    ),
                                    IdentifierName("InteropServices")
                                ),
                                IdentifierName("CallingConvention")
                            ),
                            IdentifierName(callingConvention.ToString())
                        )
                    )
                );
            }
        }

        private IEnumerable<ParameterSyntax> BuildParameters()
        {
            foreach (var parameter in NativeMethod.Parameters)
            {
                var builder = new PInvokeParameterBuilder(_builder, parameter);

                yield return builder.Build();
            }
        }

        private static IEnumerable<SyntaxTrivia> BuildComments(FunctionDecl method)
        {
            method.Location.GetPresumedLocation(out var fileName, out var line, out var col);

            yield return Comment($"// Defined in: {fileName} ({line}:{col})");

            var comment = method.Handle.ParsedComment;

            if (comment.Kind == CXCommentKind.CXComment_Null)
                yield break;



            yield return Trivia(
                DocumentationCommentTrivia(
                    SyntaxKind.SingleLineDocumentationCommentTrivia,
                    List(GetComments(comment)),
                    Token(SyntaxKind.EndOfDocumentationCommentToken)
                )
            );

            static IEnumerable<XmlNodeSyntax> GetComments(CXComment comment,
                bool isFirstParagraph = true)
            {
                if (comment.Kind == CXCommentKind.CXComment_FullComment)
                {
                    yield return XmlText(" ")
                        .WithLeadingTrivia(TriviaList(
                            DocumentationCommentExterior("///")
                        ));
                }

                for (uint x = 0; x < comment.NumChildren; x++)
                {
                    var child = comment.GetChild(x);

                    switch (child.Kind)
                    {
                        case CXCommentKind.CXComment_Text:
                            var text = child.TextComment_Text.CString.Trim();

                            if (text.Length > 0)
                            {
                                yield return XmlText(TokenList(
                                    XmlTextLiteral($" {text}"),
                                    XmlTextNewLine("\r\n"),
                                    XmlTextLiteral(" "))
                                )
                                .WithLeadingTrivia(TriviaList(
                                    DocumentationCommentExterior("///")
                                ));
                            }
                            break;
                        case CXCommentKind.CXComment_InlineCommand:
                            //break;
                        case CXCommentKind.CXComment_HTMLStartTag:
                            //break;
                        case CXCommentKind.CXComment_HTMLEndTag:
                            goto default;
                            //break;
                        case CXCommentKind.CXComment_Paragraph:
                            if (isFirstParagraph)
                            {
                                var summary = XmlSummaryElement(
                                    List(
                                        GetComments(child, false)
                                    )
                                );

                                if (!summary.Content.Any(SyntaxKind.XmlText))
                                    break;

                                yield return summary;
                                isFirstParagraph = false;
                            }
                            else
                            {
                                foreach (var elem in GetComments(child, isFirstParagraph))
                                    yield return elem;
                            }
                            break;
                        case CXCommentKind.CXComment_BlockCommand:
                            foreach (var elem in GetBlockComment(child))
                                yield return elem;

                            break;
                        case CXCommentKind.CXComment_ParamCommand:
                            yield return XmlText(" ")
                                .WithLeadingTrivia(TriviaList(
                                    DocumentationCommentExterior("///")
                                ));

                            yield return XmlParamElement(
                                child.ParamCommandComment_ParamName.CString.Trim(),
                                List(
                                    GetComments(child, false)
                                )
                            );

                            yield return XmlText(
                                XmlTextNewLine("\r\n")
                                    .WithoutTrivia());
                            break;
                        case CXCommentKind.CXComment_TParamCommand:
                            //break;
                        case CXCommentKind.CXComment_VerbatimBlockCommand:
                            //break;
                        case CXCommentKind.CXComment_VerbatimBlockLine:
                            //break;
                        case CXCommentKind.CXComment_VerbatimLine:
                            //break;
                        case CXCommentKind.CXComment_FullComment:
                            //break;

                        default:
                            foreach (var elem in GetComments(child, isFirstParagraph))
                                yield return elem;
                            break;
                    }
                }

                if (comment.Kind == CXCommentKind.CXComment_FullComment)
                {
                    yield return XmlText(
                        XmlTextNewLine("\r\n")
                            .WithoutTrivia());
                }

                static IEnumerable<XmlNodeSyntax> GetBlockComment(CXComment comment)
                {
                    switch (comment.BlockCommandComment_CommandName.CString)
                    {
                        case "see":
                        case "sa":
                            var cref = comment.BlockCommandComment_Paragraph.GetChild(0);
                            yield return XmlText(" ")
                                .WithLeadingTrivia(TriviaList(
                                    DocumentationCommentExterior("///")
                                ));
                            yield return XmlEmptyElement(
                                XmlName("seealso"),
                                SingletonList<XmlAttributeSyntax>(
                                    XmlCrefAttribute(
                                        NameMemberCref(
                                            ParseName(
                                                cref.TextComment_Text.CString.Trim(),
                                                consumeFullText: false)
                                        )
                                    )
                                )
                            );

                            break;
                        case "brief":
                            yield return XmlSummaryElement(
                                List(
                                    GetComments(comment, false)
                                )
                            );
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static TypeSyntax BuildManagedType(NativeType nativeType)
        {
            // Ensure we're dealing with the canonical type for this, rather
            // than an alias.
            nativeType = nativeType.CanonicalType;

            return GetManagedType(nativeType.Handle);

            static TypeSyntax GetManagedType(CXType type)
            {
                switch (type.kind)
                {
                    case CXTypeKind.CXType_Pointer:
                        return Pointer(type);
                    case CXTypeKind.CXType_SChar:
                        return PredefinedType(
                            Token(SyntaxKind.SByteKeyword));
                    case CXTypeKind.CXType_UChar:
                        return PredefinedType(
                            Token(SyntaxKind.ByteKeyword));
                    case CXTypeKind.CXType_Short:
                        return PredefinedType(
                            Token(SyntaxKind.ShortKeyword));
                    case CXTypeKind.CXType_UShort:
                        return PredefinedType(
                            Token(SyntaxKind.UShortKeyword));
                    case CXTypeKind.CXType_Int:
                        return PredefinedType(
                            Token(SyntaxKind.IntKeyword));
                    case CXTypeKind.CXType_UInt:
                        return PredefinedType(
                            Token(SyntaxKind.UIntKeyword));
                    case CXTypeKind.CXType_Long:
                    case CXTypeKind.CXType_ULong:
                        Debug.Fail("No .NET type correctly maps to long/unsigned long");
                        return null;
                    case CXTypeKind.CXType_LongLong:
                        return PredefinedType(
                            Token(SyntaxKind.LongKeyword));
                    case CXTypeKind.CXType_ULongLong:
                        return PredefinedType(
                            Token(SyntaxKind.ULongKeyword));
                }

                Debug.Fail($"Unsupported type: {type.kind} ({type.Spelling.CString})");
                return null;
            }

            static TypeSyntax Pointer(CXType type)
            {
                return PointerType(GetManagedType(type.PointeeType));
            }
        }
    }
}
