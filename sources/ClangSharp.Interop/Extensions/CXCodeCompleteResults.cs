// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXCodeCompleteResults : IDisposable
    {
        public static CXCodeComplete_Flags DefaultOptions => (CXCodeComplete_Flags)clang.defaultCodeCompleteOptions();

        public CXString ContainerUsr
        {
            get
            {
                fixed (CXCodeCompleteResults* pThis = &this)
                {
                    return clang.codeCompleteGetContainerUSR(pThis);
                }
            }
        }

        public ulong Contexts
        {
            get
            {
                fixed (CXCodeCompleteResults* pThis = &this)
                {
                    return clang.codeCompleteGetContexts(pThis);
                }
            }
        }

        public uint NumDiagnostics
        {
            get
            {
                fixed (CXCodeCompleteResults* pThis = &this)
                {
                    return clang.codeCompleteGetNumDiagnostics(pThis);
                }
            }
        }

        public CXString ObjCSelector
        {
            get
            {
                fixed (CXCodeCompleteResults* pThis = &this)
                {
                    return clang.codeCompleteGetObjCSelector(pThis);
                }
            }
        }

        public void Dispose()
        {
            fixed (CXCodeCompleteResults* pThis = &this)
            {
                clang.disposeCodeCompleteResults(pThis);
            }
        }

        public CXCursorKind GetContaininerKind(out bool isIncomplete)
        {
            fixed (CXCodeCompleteResults* pThis = &this)
            {
                uint isIncompleteOut;
                var result = clang.codeCompleteGetContainerKind(pThis, &isIncompleteOut);

                isIncomplete = isIncompleteOut != 0;
                return result;
            }
        }

        public CXDiagnostic GetDiagnostic(uint index)
        {
            fixed (CXCodeCompleteResults* pThis = &this)
            {
                return (CXDiagnostic)clang.codeCompleteGetDiagnostic(pThis, index);
            }
        }

        public CXString GetFixIt(uint completionIndex, uint fixitIndex, out CXSourceRange replacementRange)
        {
            fixed (CXCodeCompleteResults* pThis = &this)
            fixed (CXSourceRange* pReplacementRange = &replacementRange)
            {
                return clang.getCompletionFixIt(pThis, completionIndex, fixitIndex, pReplacementRange);
            }
        }

        public uint GetNumFixIts(uint completionIndex)
        {
            fixed (CXCodeCompleteResults* pThis = &this)
            {
                return clang.getCompletionNumFixIts(pThis, completionIndex);
            }
        }

        public void Sort() => clang.sortCodeCompletionResults(Results, NumResults);
    }
}
