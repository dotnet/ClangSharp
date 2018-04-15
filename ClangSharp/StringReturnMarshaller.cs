namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    internal class StringReturnMarshaller : ICustomMarshaler
    {
        private static readonly StringReturnMarshaller staticInstance = new StringReturnMarshaller();

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return staticInstance;
        }

        public Object MarshalNativeToManaged(IntPtr pNativeData)
        {
            return Marshal.PtrToStringAnsi(pNativeData);
        }

        public IntPtr MarshalManagedToNative(Object ManagedObj)
        {
            throw new NotImplementedException();
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            return;
        }

        public void CleanUpManagedData(Object ManagedObj)
        {
            return;
        }

        public int GetNativeDataSize()
        {
            return -1;
        }
    }
}
