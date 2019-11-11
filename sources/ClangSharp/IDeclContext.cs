// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

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
