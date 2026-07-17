// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Linq;
using ClangSharp.Interop;
using NUnit.Framework;
using static ClangSharp.Interop.CXTranslationUnit_Flags;

namespace ClangSharp.UnitTests;

public sealed class PreprocessingTest : TranslationUnitTest
{
    private const CXTranslationUnit_Flags PreprocessingFlags = DefaultTranslationUnitFlags | CXTranslationUnit_DetailedPreprocessingRecord;

    [Test]
    public void MacroDefinitionRecordTest()
    {
        var inputContents = """
#define OBJECT 1
#define FUNC(a, b) ((a) + (b))
#define VARIADIC(fmt, ...) fmt
""";

        using var translationUnit = CreateTranslationUnit(inputContents, translationUnitFlags: PreprocessingFlags);

        var macros = translationUnit.TranslationUnitDecl.CursorChildren.OfType<MacroDefinitionRecord>().ToArray();

        var obj = macros.Single((macro) => macro.Name.Equals("OBJECT", StringComparison.Ordinal));
        Assert.That(obj.IsFunctionLike, Is.False);
        Assert.That(obj.NumParameters, Is.EqualTo(0));
        Assert.That(obj.Tokens, Has.Count.EqualTo(1));
        Assert.That(obj.Tokens[0], Is.EqualTo("1"));

        var func = macros.Single((macro) => macro.Name.Equals("FUNC", StringComparison.Ordinal));
        Assert.That(func.IsFunctionLike, Is.True);
        Assert.That(func.IsVariadic, Is.False);
        Assert.That(func.NumParameters, Is.EqualTo(2));
        Assert.That(func.ParameterNames, Has.Count.EqualTo(2));
        Assert.That(func.ParameterNames[0], Is.EqualTo("a"));
        Assert.That(func.ParameterNames[1], Is.EqualTo("b"));

        var variadic = macros.Single((macro) => macro.Name.Equals("VARIADIC", StringComparison.Ordinal));
        Assert.That(variadic.IsFunctionLike, Is.True);
        Assert.That(variadic.IsVariadic, Is.True);
    }

    [Test]
    public void InclusionDirectiveTest()
    {
        var inputContents = """
#pragma once
#include "ClangUnsavedFile.h"
""";

        using var translationUnit = CreateTranslationUnit(inputContents, translationUnitFlags: PreprocessingFlags);

        var inclusion = translationUnit.TranslationUnitDecl.CursorChildren.OfType<InclusionDirective>().First();

        Assert.That(inclusion.Kind, Is.EqualTo(CX_InclusionDirectiveKind.CX_IDK_Include));
        Assert.That(inclusion.WasInQuotes, Is.True);
    }
}
