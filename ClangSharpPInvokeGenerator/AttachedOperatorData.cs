using ClangSharp;
using System.Diagnostics;

namespace ClangSharpPInvokeGenerator
{
    public struct AttachedOperatorData
    {
        public AttachedOperatorData(CXCursorKind kind, string @operator)
        {
            Kind = kind;
            Operator = @operator;

            if (kind == CXCursorKind.CXCursor_UnaryOperator)
            {
                RemainingOperandCount = 1;
            }
            else
            {
                Debug.Assert(kind == CXCursorKind.CXCursor_BinaryOperator);
                RemainingOperandCount = 2;
            }
        }

        public CXCursorKind Kind { get; }

        public string Operator { get; }

        public int RemainingOperandCount { get; set; }
    }
}
