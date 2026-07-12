// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CX_VariableCaptureKind;

namespace ClangSharp;

public partial class CapturedStmt
{
    public sealed class Capture
    {
        private readonly CapturedStmt _parentStmt;
        private readonly uint _index;
        private ValueLazy<Capture, VarDecl> _capturedVar;

        internal unsafe Capture(CapturedStmt parentStmt, uint index)
        {
            _parentStmt = parentStmt;
            _index = index;

            _capturedVar = new ValueLazy<Capture, VarDecl>(&CapturedVarFactory);
        }

        public VarDecl CapturedVar => _capturedVar.GetValue(this);

        public CX_VariableCaptureKind CaptureKind => _parentStmt.Handle.GetCaptureKind(_index);

        public bool CapturesThis => CaptureKind == CX_VCK_This;

        public bool CapturesVariable => CaptureKind == CX_VCK_ByRef;

        public bool CapturesVariableByCopy => CaptureKind == CX_VCK_ByCopy;

        public bool CapturesVariableArrayType => CaptureKind == CX_VCK_VLAType;
    
    private static unsafe VarDecl CapturedVarFactory(Capture self) => self._parentStmt.TranslationUnit.GetOrCreate<VarDecl>(self._parentStmt.Handle.GetCapturedVar(self._index));
}
}
