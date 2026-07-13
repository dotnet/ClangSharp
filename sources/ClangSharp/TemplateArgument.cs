// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_TemplateArgumentDependence;
using static ClangSharp.Interop.CXTemplateArgumentKind;

namespace ClangSharp;

public sealed unsafe class TemplateArgument : IDisposable
{
    private ValueLazy<TemplateArgument, ValueDecl> _asDecl;
    private ValueLazy<TemplateArgument, Expr> _asExpr;
    private ValueLazy<TemplateArgument, TemplateName> _asTemplate;
    private ValueLazy<TemplateArgument, TemplateName> _asTemplateOrTemplatePattern;
    private ValueLazy<TemplateArgument, Type> _asType;
    private ValueLazy<TemplateArgument, Type> _integralType;
    private ValueLazy<TemplateArgument, Type> _nonTypeTemplateArgumentType;
    private ValueLazy<TemplateArgument, Type> _nullPtrType;
    private readonly LazyList<TemplateArgument> _packElements;
    private ValueLazy<TemplateArgument, TemplateArgument> _packExpansionPattern;
    private ValueLazy<TemplateArgument, Type> _paramTypeForDecl;
    private ValueLazy<TemplateArgument, TranslationUnit> _translationUnit;

    internal TemplateArgument(CX_TemplateArgument handle)
    {
        Handle = handle;

        _translationUnit = new ValueLazy<TemplateArgument, TranslationUnit>(&TranslationUnitFactory);

        _asDecl = new ValueLazy<TemplateArgument, ValueDecl>(&AsDeclFactory);
        _asExpr = new ValueLazy<TemplateArgument, Expr>(&AsExprFactory);
        _asTemplate = new ValueLazy<TemplateArgument, TemplateName>(&AsTemplateFactory);
        _asTemplateOrTemplatePattern = new ValueLazy<TemplateArgument, TemplateName>(&AsTemplateOrTemplatePatternFactory);
        _asType = new ValueLazy<TemplateArgument, Type>(&AsTypeFactory);
        _integralType = new ValueLazy<TemplateArgument, Type>(&IntegralTypeFactory);
        _nonTypeTemplateArgumentType = new ValueLazy<TemplateArgument, Type>(&NonTypeTemplateArgumentTypeFactory);
        _nullPtrType = new ValueLazy<TemplateArgument, Type>(&NullPtrTypeFactory);
        _packExpansionPattern = new ValueLazy<TemplateArgument, TemplateArgument>(&PackExpansionPatternFactory);
        _packElements = LazyList.Create<TemplateArgument>(this, Handle.NumPackElements, &PackElementsFactory);
        _paramTypeForDecl = new ValueLazy<TemplateArgument, Type>(&ParamTypeForDeclFactory);
    }

    ~TemplateArgument() => Dispose(isDisposing: false);

    public ValueDecl AsDecl => _asDecl.GetValue(this);

    public Expr AsExpr => _asExpr.GetValue(this);

    public long AsIntegral => Handle.AsIntegral;

    public TemplateName AsTemplate => _asTemplate.GetValue(this);

    public TemplateName AsTemplateOrTemplatePattern => _asTemplateOrTemplatePattern.GetValue(this);

    public Type AsType => _asType.GetValue(this);

    public bool ContainsUnexpandedParameterPack => (Dependence & CX_TAD_UnexpandedPack) != 0;

    public CX_TemplateArgumentDependence Dependence => Handle.Dependence;

    public Type IntegralType => _integralType.GetValue(this);

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

    public Type NonTypeTemplateArgumentType => _nonTypeTemplateArgumentType.GetValue(this);

    public Type NullPtrType => _nullPtrType.GetValue(this);

    public IReadOnlyList<TemplateArgument> PackElements => _packElements;

    public TemplateArgument PackExpansionPattern => _packExpansionPattern.GetValue(this);

    public Type ParamTypeForDecl => _paramTypeForDecl.GetValue(this);

    public TranslationUnit TranslationUnit => _translationUnit.GetValue(this);

    public void Dispose()
    {
        Dispose(isDisposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool isDisposing) => Handle.Dispose();

    private static unsafe Type ParamTypeForDeclFactory(TemplateArgument self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ParamTypeForDecl);

    private static unsafe TemplateArgument PackExpansionPatternFactory(TemplateArgument self) => self.TranslationUnit.GetOrCreate(self.Handle.PackExpansionPattern);

    private static unsafe Type NullPtrTypeFactory(TemplateArgument self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.NullPtrType);

    private static unsafe Type NonTypeTemplateArgumentTypeFactory(TemplateArgument self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.NonTypeTemplateArgumentType);

    private static unsafe Type IntegralTypeFactory(TemplateArgument self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.IntegralType);

    private static unsafe Type AsTypeFactory(TemplateArgument self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.AsType);

    private static unsafe TemplateName AsTemplateOrTemplatePatternFactory(TemplateArgument self) => self.TranslationUnit.GetOrCreate(self.Handle.AsTemplateOrTemplatePattern);

    private static unsafe TemplateName AsTemplateFactory(TemplateArgument self) => self.TranslationUnit.GetOrCreate(self.Handle.AsTemplate);

    private static unsafe Expr AsExprFactory(TemplateArgument self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.AsExpr);

    private static unsafe ValueDecl AsDeclFactory(TemplateArgument self) => self.TranslationUnit.GetOrCreate<ValueDecl>(self.Handle.AsDecl);

    private static unsafe TranslationUnit TranslationUnitFactory(TemplateArgument self) => TranslationUnit.GetOrCreate(self.Handle.tu);

    private static unsafe TemplateArgument PackElementsFactory(object self, int i)
    {
        var @this = (TemplateArgument)self;
        return @this.TranslationUnit.GetOrCreate(@this.Handle.GetPackElement(unchecked((uint)i)));
    }
}
