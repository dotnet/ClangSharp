using System.Diagnostics;

namespace ClangSharpPInvokeGenerator
{
    public struct AttachedFunctionDeclData
    {
        public AttachedFunctionDeclData(int parmCount)
        {
            Debug.Assert(parmCount != -1);

            ParmCount = parmCount;
            RemainingParmCount = parmCount;
        }

        public int ParmCount { get; }

        public int RemainingParmCount { get; set; }
    }
}
