// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CompoundAssignOperator : BinaryOperator
    {
        internal CompoundAssignOperator(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CompoundAssignOperator)
        {
        }

        protected override string GetOpcode()
        {
            var tokens = Handle.TranslationUnit.Tokenize(Extent);

            Debug.Assert(tokens.Length >= 3);

            int operatorIndex = -1;
            int parenDepth = 0;

            for (int index = 0; (index < tokens.Length) && (operatorIndex == -1); index++)
            {
                var token = tokens[index];

                if (token.Kind != CXTokenKind.CXToken_Punctuation)
                {
                    continue;
                }

                var punctuation = tokens[index].GetSpelling(Handle.TranslationUnit).ToString();

                switch (punctuation)
                {
                    case "%=":
                    case "&=":
                    case "*=":
                    case "+=":
                    case "-=":
                    case "/=":
                    case "<<=":
                    case ">>=":
                    case "^=":
                    case "|=":
                    {
                        if (parenDepth == 0)
                        {
                            operatorIndex = index;
                        }
                        break;
                    }

                    case "(":
                    {
                        parenDepth++;
                        break;
                    }

                    case ")":
                    {
                        parenDepth--;
                        break;
                    }

                    default:
                    {
                        Debug.WriteLine($"Unhandled punctuation kind: {punctuation}.");
                        break;
                    }
                }
            }

            Debug.Assert(operatorIndex != -1);
            Debug.Assert(tokens[operatorIndex].Kind == CXTokenKind.CXToken_Punctuation);
            return tokens[operatorIndex].GetSpelling(Handle.TranslationUnit).ToString();
        }
    }
}
