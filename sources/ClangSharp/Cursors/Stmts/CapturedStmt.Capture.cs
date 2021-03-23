// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public partial class CapturedStmt
    {
        public sealed class Capture
        {
            private readonly CapturedStmt _parentStmt;
            private readonly uint _index;
            private readonly Lazy<VarDecl> _capturedVar;

            internal Capture(CapturedStmt parentStmt, uint index)
            {
                _parentStmt = parentStmt;
                _index = index;

                _capturedVar = new Lazy<VarDecl>(() => _parentStmt.TranslationUnit.GetOrCreate<VarDecl>(_parentStmt.Handle.GetCapturedVar(_index)));
            }

            public VarDecl CapturedVar => _capturedVar.Value;

            public CX_VariableCaptureKind CaptureKind => _parentStmt.Handle.GetCaptureKind(_index);

            public bool CapturesThis => CaptureKind == CX_VariableCaptureKind.CX_VCK_This;

            public bool CapturesVariable => CaptureKind == CX_VariableCaptureKind.CX_VCK_ByRef;

            public bool CapturesVariableByCopy => CaptureKind == CX_VariableCaptureKind.CX_VCK_ByCopy;

            public bool CapturesVariableArrayType => CaptureKind == CX_VariableCaptureKind.CX_VCK_VLAType;
        }
    }
}
