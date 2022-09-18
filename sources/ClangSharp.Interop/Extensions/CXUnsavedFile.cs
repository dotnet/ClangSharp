// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ClangSharp.Interop;

public unsafe partial struct CXUnsavedFile : IDisposable
{
    public static CXUnsavedFile Create(string filename, string contents)
    {
        IntPtr pFilename, pContents;
        int filenameLength, contentsLength;

        if ((filename is null) || (filename.Length == 0))
        {
            pFilename = IntPtr.Zero;
        }
        else
        {
            var filenameBytes = Encoding.UTF8.GetBytes(filename);
            filenameLength = filenameBytes.Length;
            pFilename = Marshal.AllocHGlobal(filenameLength + 1);
            Marshal.Copy(filenameBytes, 0, pFilename, filenameLength);
            Marshal.WriteByte(pFilename, filenameLength, 0);
        }

        if ((contents is null) || (contents.Length == 0))
        {
            contentsLength = 0;
            pContents = IntPtr.Zero;
        }
        else
        {
            var contentsBytes = Encoding.UTF8.GetBytes(contents);
            contentsLength = contentsBytes.Length;
            pContents = Marshal.AllocHGlobal(contentsLength + 1);
            Marshal.Copy(contentsBytes, 0, pContents, contentsLength);
            Marshal.WriteByte(pContents, contentsLength, 0);
        }

        return new CXUnsavedFile()
        {
            Filename = (sbyte*)pFilename,
            Contents = (sbyte*)pContents,
            Length = (UIntPtr)contentsLength
        };
    }

    public void Dispose()
    {
        if (Filename != null)
        {
            Marshal.FreeHGlobal((IntPtr)Filename);
            Filename = null;
        }

        if (Contents != null)
        {
            Marshal.FreeHGlobal((IntPtr)Contents);
            Contents = null;
            Length = (UIntPtr)0;
        }
    }

    public ReadOnlySpan<byte> ContentsSpan => new ReadOnlySpan<byte>(Contents, (int)Length);

    public string FilenameString
    {
        get
        {
            var pFilename = Filename;

            if (pFilename is null)
            {
                return string.Empty;
            }

            return SpanExtensions.AsString(pFilename);
        }
    }
}
