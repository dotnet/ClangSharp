// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class ImplicitConceptSpecializationDecl : Decl
{
    private readonly Lazy<IReadOnlyList<TemplateArgumentLoc>> _templateArgs;

    internal ImplicitConceptSpecializationDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_ImplicitConceptSpecialization)
    {
        _templateArgs = new Lazy<IReadOnlyList<TemplateArgumentLoc>>(() => {
            var templateArgCount = Handle.NumTemplateArguments;
            var templateArgs = new List<TemplateArgumentLoc>(templateArgCount);

            for (var i = 0; i < templateArgCount; i++)
            {
                var templateArg = TranslationUnit.GetOrCreate(Handle.GetTemplateArgumentLoc(unchecked((uint)i)));
                templateArgs.Add(templateArg);
            }

            return templateArgs;
        });
    }

    public uint NumTemplateArgs => unchecked((uint)Handle.NumTemplateArguments);

    public IReadOnlyList<TemplateArgumentLoc> TemplateArgs => _templateArgs.Value;
}
