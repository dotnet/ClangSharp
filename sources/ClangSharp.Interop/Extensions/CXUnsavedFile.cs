// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ClangSharp.Interop;

public unsafe partial struct CXUnsavedFile : IDisposable
{
    public static CXUnsavedFile Create(string filename, string contents)
    {
        sbyte* pFilename, pContents;
        nuint contentsLength;

        if (string.IsNullOrEmpty(filename))
        {
            pFilename = null;
        }
        else
        {
            var maxFilenameLength = Encoding.UTF8.GetMaxByteCount(filename.Length);
            pFilename = (sbyte*)NativeMemory.Alloc((uint)maxFilenameLength + 1);
            var filenameLength = (uint)Encoding.UTF8.GetBytes(filename, new Span<byte>(pFilename, maxFilenameLength));
            pFilename[filenameLength] = 0;
        }

        if (string.IsNullOrEmpty(contents))
        {
            contentsLength = 0;
            pContents = null;
        }
        else
        {
            var maxContentsLength = Encoding.UTF8.GetMaxByteCount(contents.Length);
            pContents = (sbyte*)NativeMemory.Alloc((uint)maxContentsLength + 1);
            contentsLength = (uint)Encoding.UTF8.GetBytes(contents, new Span<byte>(pContents, maxContentsLength));
            pContents[contentsLength] = 0;
        }

        return new CXUnsavedFile()
        {
            Filename = pFilename,
            Contents = pContents,
            Length = contentsLength
        };
    }

    public static CXUnsavedFile Create(string filename, CXTranslationUnit translationUnit, CXFile baseFile, string additionalContents)
    {
        sbyte* pFilename, pContents;
        nuint contentsLength;

        if (string.IsNullOrEmpty(filename))
        {
            pFilename = null;
        }
        else
        {
            var maxFilenameLength = Encoding.UTF8.GetMaxByteCount(filename.Length);
            pFilename = (sbyte*)NativeMemory.Alloc((uint)maxFilenameLength + 1);
            var filenameLength = (uint)Encoding.UTF8.GetBytes(filename, new Span<byte>(pFilename, maxFilenameLength));
            pFilename[filenameLength] = 0;
        }

        var baseFileContents = translationUnit.GetFileContents(baseFile, out _);
        var maxContentsLength = baseFileContents.Length + Encoding.UTF8.GetMaxByteCount((additionalContents?.Length).GetValueOrDefault());

        pContents = (sbyte*)NativeMemory.Alloc((uint)maxContentsLength + 1);

        var contents = new Span<byte>(pContents, maxContentsLength);
        baseFileContents.CopyTo(contents);

        contentsLength = (uint)(baseFileContents.Length + Encoding.UTF8.GetBytes(additionalContents, contents[baseFileContents.Length..]));
        pContents[contentsLength] = 0;

        return new CXUnsavedFile() {
            Filename = pFilename,
            Contents = pContents,
            Length = contentsLength
        };
    }

    public void Dispose()
    {
        if (Filename != null)
        {
            NativeMemory.Free(Filename);
            Filename = null;
        }

        if (Contents != null)
        {
            NativeMemory.Free(Contents);
            Contents = null;
            Length = 0;
        }
    }

    public readonly ReadOnlySpan<byte> ContentsSpan => new ReadOnlySpan<byte>(Contents, (int)Length);

    public readonly string FilenameString
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
