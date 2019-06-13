using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXFile : IEquatable<CXFile>
    {
        public CXString Name => clang.getFileName(this);

        public long Time => clang.getFileTime(this);
        public bool GetUniqueId(out CXFileUniqueID id)
        {
            fixed (CXFileUniqueID* pId = &id)
            {
                return clang.getFileUniqueID(this, pId) != 0;
            }
        }

        public override string ToString() => Name.ToString();

        public CXString TryGetRealPathName() => clang.File_tryGetRealPathName(this);
    }
}
