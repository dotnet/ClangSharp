// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class ImplicitConceptSpecializationDecl : Decl
{
    private readonly LazyList<TemplateArgumentLoc> _templateArgs;

    internal unsafe ImplicitConceptSpecializationDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_ImplicitConceptSpecialization)
    {
        _templateArgs = LazyList.Create<TemplateArgumentLoc>(this, Handle.NumTemplateArguments, &TemplateArgsFactory);
    }

    public uint NumTemplateArgs => unchecked((uint)Handle.NumTemplateArguments);

    public IReadOnlyList<TemplateArgumentLoc> TemplateArgs => _templateArgs;

    private static unsafe TemplateArgumentLoc TemplateArgsFactory(object self, int i)
    {
        var @this = (ImplicitConceptSpecializationDecl)self;
        return @this.TranslationUnit.GetOrCreate(@this.Handle.GetTemplateArgumentLoc(unchecked((uint)i)));
    }
}
