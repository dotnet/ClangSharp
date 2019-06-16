using System.Collections.Generic;

namespace ClangSharp
{
    public interface IDeclContext
    {
        IDeclContext LexicalParent { get; }

        IDeclContext Parent { get; }

        IReadOnlyList<Decl> Decls { get; }
    }
}
