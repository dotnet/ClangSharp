// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxContainerInfo
    {
        public CXIdxClientContainer ClientContainer
        {
            get
            {
                fixed (CXIdxContainerInfo* pThis = &this)
                {
                    return (CXIdxClientContainer)clang.index_getClientContainer(pThis);
                }
            }

            set
            {
                fixed (CXIdxContainerInfo* pThis = &this)
                {
                    clang.index_setClientContainer(pThis, value);
                }
            }
        }
    }
}
