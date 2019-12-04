// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ForStmt : Stmt
    {
        private readonly Lazy<DeclStmt> _conditionVariableDeclStmt;
        private readonly Lazy<Stmt> _init;
        private readonly Lazy<Expr> _cond;
        private readonly Lazy<Expr> _inc;
        private readonly Lazy<Stmt> _body;

        internal ForStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ForStmt, CX_StmtClass.CX_StmtClass_ForStmt)
        {
            _conditionVariableDeclStmt = new Lazy<DeclStmt>(() => (Children.Count != 1) ? Children.OfType<DeclStmt>().SingleOrDefault() : null);
            _init = new Lazy<Stmt>(() => {
                if ((ConditionVariableDeclStmt is null) && (Children.Count != 1))
                {
                    if (Children.Count == 4)
                    {
                        return Children.ElementAt(0);
                    }

                    var stmt = Children.OfType<Stmt>().First();

                    if ((stmt != Children.Last()) && (stmt is Expr expr) && (expr.Type.Kind != CXTypeKind.CXType_Bool))
                    {
                        return stmt;
                    }
                }
                return null;
            });
            _cond = new Lazy<Expr>(() => {
                if (Children.Count != 1)
                {
                    if (Children.Count == 4)
                    {
                        return (Expr)Children.ElementAt(1);
                    }

                    var expr = Children.OfType<Expr>().Where((expr) => expr.Type.Kind == CXTypeKind.CXType_Bool).FirstOrDefault();

                    if (expr != Children.Last())
                    {
                        return expr;
                    }
                }
                return null;
            });
            _inc = new Lazy<Expr>(() => {
                if (Children.Count != 1)
                {
                    if (Children.Count == 4)
                    {
                        return (Expr)Children.ElementAt(2);
                    }

                    var expr = Children.OfType<Expr>().Where((expr) => (expr != Init) && (expr != Cond)).FirstOrDefault();

                    if (expr != Children.Last())
                    {
                        return expr;
                    }
                }
                return null;
            });
            _body = new Lazy<Stmt>(() => Children.Last());
        }

        public DeclStmt ConditionVariableDeclStmt => _conditionVariableDeclStmt.Value;

        public Stmt Init => _init.Value;

        public Expr Cond => _cond.Value;

        public Expr Inc => _inc.Value;

        public Stmt Body => _body.Value;
    }
}
