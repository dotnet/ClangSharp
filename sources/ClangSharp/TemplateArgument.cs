// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTemplateArgumentKind;
using static ClangSharp.Interop.CX_TemplateArgumentDependence;

namespace ClangSharp;

public sealed unsafe class TemplateArgument : IDisposable
{
    private readonly Lazy<ValueDecl> _asDecl;
    private readonly Lazy<Expr> _asExpr;
    private readonly Lazy<TemplateName> _asTemplate;
    private readonly Lazy<TemplateName> _asTemplateOrTemplatePattern;
    private readonly Lazy<Type> _asType;
    private readonly Lazy<Type> _integralType;
    private readonly Lazy<Type> _nonTypeTemplateArgumentType;
    private readonly Lazy<Type> _nullPtrType;
    private readonly Lazy<IReadOnlyList<TemplateArgument>> _packElements;
    private readonly Lazy<TemplateArgument> _packExpansionPattern;
    private readonly Lazy<Type> _paramTypeForDecl;
    private readonly Lazy<TranslationUnit> _translationUnit;

    internal TemplateArgument(CX_TemplateArgument handle)
    {
        Handle = handle;

        _translationUnit = new Lazy<TranslationUnit>(() => TranslationUnit.GetOrCreate(Handle.tu));

        _asDecl = new Lazy<ValueDecl>(() =>  _translationUnit.Value.GetOrCreate<ValueDecl>(Handle.AsDecl));
        _asExpr = new Lazy<Expr>(() => _translationUnit.Value.GetOrCreate<Expr>(Handle.AsExpr));
        _asTemplate = new Lazy<TemplateName>(() => _translationUnit.Value.GetOrCreate(Handle.AsTemplate));
        _asTemplateOrTemplatePattern = new Lazy<TemplateName>(() => _translationUnit.Value.GetOrCreate(Handle.AsTemplateOrTemplatePattern));
        _asType = new Lazy<Type>(() => _translationUnit.Value.GetOrCreate<Type>(Handle.AsType));
        _integralType = new Lazy<Type>(() => _translationUnit.Value.GetOrCreate<Type>(Handle.IntegralType));
        _nonTypeTemplateArgumentType = new Lazy<Type>(() => _translationUnit.Value.GetOrCreate<Type>(Handle.NonTypeTemplateArgumentType));
        _nullPtrType = new Lazy<Type>(() => _translationUnit.Value.GetOrCreate<Type>(Handle.NullPtrType));
        _packExpansionPattern = new Lazy<TemplateArgument>(() => _translationUnit.Value.GetOrCreate(Handle.PackExpansionPattern));

        _packElements = new Lazy<IReadOnlyList<TemplateArgument>>(() => {
            var numPackElements = Handle.NumPackElements;
            var packElements = new List<TemplateArgument>(numPackElements);

            for (var i = 0; i < numPackElements; i++)
            {
                var packElement = _translationUnit.Value.GetOrCreate(Handle.GetPackElement(unchecked((uint)i)));
                packElements.Add(packElement);
            }

            return packElements;
        });

        _paramTypeForDecl = new Lazy<Type>(() => _translationUnit.Value.GetOrCreate<Type>(Handle.ParamTypeForDecl));
    }

    ~TemplateArgument() => Dispose(isDisposing: false);

    public ValueDecl AsDecl => _asDecl.Value;

    public Expr AsExpr => _asExpr.Value;

    public long AsIntegral => Handle.AsIntegral;

    public TemplateName AsTemplate => _asTemplate.Value;

    public TemplateName AsTemplateOrTemplatePattern => _asTemplateOrTemplatePattern.Value;

    public Type AsType => _asType.Value;

    public bool ContainsUnexpandedParameterPack => (Dependence & CX_TAD_UnexpandedPack) != 0;

    public CX_TemplateArgumentDependence Dependence => Handle.Dependence;

    public Type IntegralType => _integralType.Value;

    public bool IsDependent => (Dependence & CX_TAD_Dependent) != 0;

    public bool IsInstantiationDependent => (Dependence & CX_TAD_Instantiation) != 0;

    public bool IsNull => Kind == CXTemplateArgumentKind_Null;

    public bool IsPackExpansion
    {
        get
        {
            switch (Kind)
{
                case CXTemplateArgumentKind_Null:
                case CXTemplateArgumentKind_Declaration:
                case CXTemplateArgumentKind_Integral:
                case CXTemplateArgumentKind_Pack:
                case CXTemplateArgumentKind_Template:
                case CXTemplateArgumentKind_NullPtr:
                {
                    return false;
                }

                case CXTemplateArgumentKind_TemplateExpansion:
                {
                    return true;
                }

                case CXTemplateArgumentKind_Type:
                {
                    return AsType is PackExpansionType;
                }

                case CXTemplateArgumentKind_Expression:
                {
                    return AsExpr is PackExpansionExpr;
                }
            }

            Debug.Fail("Invalid TemplateArgument Kind!");
            return false;
        }
    }

    public CX_TemplateArgument Handle { get; }

    public CXTemplateArgumentKind Kind => Handle.kind;

    public Type NonTypeTemplateArgumentType => _nonTypeTemplateArgumentType.Value;

    public Type NullPtrType => _nullPtrType.Value;

    public IReadOnlyList<TemplateArgument> PackElements => _packElements.Value;

    public TemplateArgument PackExpansionPattern => _packExpansionPattern.Value;

    public Type ParamTypeForDecl => _paramTypeForDecl.Value;

    public TranslationUnit TranslationUnit => _translationUnit.Value;

    public void Dispose()
    {
        Dispose(isDisposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool isDisposing) => Handle.Dispose();
}
