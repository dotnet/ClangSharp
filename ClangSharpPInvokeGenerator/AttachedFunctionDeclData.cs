using System.Diagnostics;

namespace ClangSharpPInvokeGenerator
{
    public struct AttachedFunctionDeclData
    {
        public AttachedFunctionDeclData(int parmCount)
        {
            Debug.Assert(parmCount != -1);

            IsDeprecated = false;
            IsDllExport = false;
            IsDllImport = false;
            ParmCount = parmCount;
            RemainingParmCount = parmCount;
        }

        public bool IsDeprecated { get; set; }

        public bool IsDllExport { get; set; }

        public bool IsDllImport { get; set; }

        public int ParmCount { get; }

        public int RemainingParmCount { get; set; }
    }
}
