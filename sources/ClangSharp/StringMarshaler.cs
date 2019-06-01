namespace ClangSharp
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class StringMarshaler : ICustomMarshaler
    {
        private static readonly StringMarshaler staticInstance = new StringMarshaler();

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return staticInstance;
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            // We don't know when the string is done, so we have to read byte-by-byte :(
            // Start at 128 bytes capacity to avoid a crazy number of allocs...
            var data = new MemoryStream(128);
            var offset = 0;
            while (true)
            {
                var b = Marshal.ReadByte(pNativeData, offset);
                if (b == 0)
                {
                    break;
                }

                data.WriteByte(b);
                offset++;
            }

            if (data.Length >= int.MaxValue)
            {
                throw new Exception("Crazy-long string!");
            }

            return Encoding.UTF8.GetString(data.GetBuffer(), 0, (int)data.Length);
        }

        public IntPtr MarshalManagedToNative(object managedObj)
        {
            var str = (string)managedObj;
            var bytes = Encoding.UTF8.GetBytes(str);

            var ptr = Marshal.AllocCoTaskMem(bytes.Length + 1);
            Marshal.Copy(bytes, 0, ptr, bytes.Length); // utf-8 bytes
            Marshal.WriteByte(ptr, bytes.Length, 0); // null terminator

            return ptr;
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            // Nothing to do
        }

        public void CleanUpManagedData(object managedObj)
        {
            // Nothing to do
        }

        public int GetNativeDataSize()
        {
            return -1;
        }
    }
}
