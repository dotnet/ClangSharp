// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

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
