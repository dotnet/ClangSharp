// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;

namespace ClangSharp;

public interface IDeclContext
{
    IReadOnlyList<Decl> Decls { get; }

    bool IsNamespace { get; }

    bool IsStdNamespace { get; }

    bool IsTranslationUnit { get; }

    IDeclContext LexicalParent { get; }

    IDeclContext Parent { get; }

    IDeclContext RedeclContext { get; }
}
