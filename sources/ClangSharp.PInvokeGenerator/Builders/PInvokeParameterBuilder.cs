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
    public sealed class PInvokeParameterBuilder
    {
        private readonly PInvokeBuilder _builder;

        public ParmVarDecl NativeParam { get; }

        public PInvokeParameterBuilder(PInvokeBuilder builder,
            ParmVarDecl nativeParam)
        {
            _builder = builder;
            NativeParam = nativeParam;
        }

        public ParameterSyntax Build()
        {
            return Parameter(
                List(BuildAttributes()),
                TokenList(),
                type: BuildManagedType(NativeParam.Type),
                identifier: Identifier(NativeParam.Name),
                @default: null
            );
        }

        private IEnumerable<AttributeListSyntax> BuildAttributes()
        {
            if (NativeParam.Type != NativeParam.Type.CanonicalType)
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
                                            Literal(NativeParam.Type.AsString))
                                    )
                                )
                            ))
                    ));
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

                    case CXTypeKind.CXType_Char_S:
                        return PredefinedType(
                            Token(SyntaxKind.CharKeyword));

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
