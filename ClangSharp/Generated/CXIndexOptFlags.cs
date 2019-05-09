namespace ClangSharp
{
    public enum CXIndexOptFlags
    {
        CXIndexOpt_None = 0,
        CXIndexOpt_SuppressRedundantRefs = 1,
        CXIndexOpt_IndexFunctionLocalSymbols = 2,
        CXIndexOpt_IndexImplicitTemplateInstantiations = 4,
        CXIndexOpt_SuppressWarnings = 8,
        CXIndexOpt_SkipParsedBodiesInSession = 16,
    }
}
