using System;
using System.Runtime.InteropServices;
using Xunit;

namespace ClangSharp.Test
{
    public class VirtualFileOverlay
    {
        class TestVFO : IDisposable
        {
            public CXVirtualFileOverlay VFO;
            string Contents;

            public TestVFO(string contents)
            {
                VFO = clang.VirtualFileOverlay_create(0);
                Contents = contents;
            }

            ~TestVFO()
            {
                Dispose(false);
            }

            public void Map(string vPath, string rPath)
            {
                var err = clang.VirtualFileOverlay_addFileMapping(VFO, vPath, rPath);
                Assert.Equal(CXErrorCode.CXError_Success, err);
            }

            public void MapError(string vPath, string rPath, CXErrorCode expErr)
            {
                var err = clang.VirtualFileOverlay_addFileMapping(VFO, vPath, rPath);
                Assert.Equal(expErr, err);
            }

            bool disposed = false;

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if(disposed)
                    return;

                if(Contents != null)
                {
                    IntPtr bufPtr;
                    uint bufSize = 0;
                    clang.VirtualFileOverlay_writeToBuffer(VFO, 0, out bufPtr, out bufSize);
                    var bufStr = Marshal.PtrToStringAnsi(bufPtr, (int)bufSize);
                    Assert.Equal(Contents, bufStr);
                    clang.free(bufPtr);
                }

                clang.VirtualFileOverlay_dispose(VFO);

                disposed = true;
            }
        }

        [Fact]
        public void Basic()
        {
            var contents =
                "{\n"
                + "  'version': 0,\n"
                + "  'roots': [\n"
                + "    {\n"
                + "      'type': 'directory',\n"
                + "      'name': \"/path/virtual\",\n"
                + "      'contents': [\n"
                + "        {\n"
                + "          'type': 'file',\n"
                + "          'name': \"foo.h\",\n"
                + "          'external-contents': \"/real/foo.h\"\n"
                + "        }\n"
                + "      ]\n"
                + "    }\n"
                + "  ]\n"
                + "}\n";

            using(TestVFO T = new TestVFO(contents))
            {
                T.Map("/path/virtual/foo.h", "/real/foo.h");
            }
        }

        [Fact]
        public void Unicode()
        {
            var contents =
                "{\n"
                + "  'version': 0,\n"
                + "  'roots': [\n"
                + "    {\n"
                + "      'type': 'directory',\n"
                + "      'name': \"/path/\\u266B\",\n"
                + "      'contents': [\n"
                + "        {\n"
                + "          'type': 'file',\n"
                + "          'name': \"\\u2602.h\",\n"
                + "          'external-contents': \"/real/\\u2602.h\"\n"
                + "        }\n"
                + "      ]\n"
                + "    }\n"
                + "  ]\n"
                + "}\n";

            using(TestVFO T = new TestVFO(contents))
            {
                T.Map("/path/♫/☂.h", "/real/☂.h");
            }
        }

        [Fact]
        public void InvalidArgs()
        {
            using(TestVFO T = new TestVFO(null))
            {
                T.MapError("/path/./virtual/../foo.h", "/real/foo.h",
                           CXErrorCode.CXError_InvalidArguments);
            }
        }

        [Fact]
        public void RemapDirectories()
        {
            var contents =
                "{\n"
                + "  'version': 0,\n"
                + "  'roots': [\n"
                + "    {\n"
                + "      'type': 'directory',\n"
                + "      'name': \"/another/dir\",\n"
                + "      'contents': [\n"
                + "        {\n"
                + "          'type': 'file',\n"
                + "          'name': \"foo2.h\",\n"
                + "          'external-contents': \"/real/foo2.h\"\n"
                + "        }\n"
                + "      ]\n"
                + "    },\n"
                + "    {\n"
                + "      'type': 'directory',\n"
                + "      'name': \"/path/virtual/dir\",\n"
                + "      'contents': [\n"
                + "        {\n"
                + "          'type': 'file',\n"
                + "          'name': \"foo1.h\",\n"
                + "          'external-contents': \"/real/foo1.h\"\n"
                + "        },\n"
                + "        {\n"
                + "          'type': 'file',\n"
                + "          'name': \"foo3.h\",\n"
                + "          'external-contents': \"/real/foo3.h\"\n"
                + "        },\n"
                + "        {\n"
                + "          'type': 'directory',\n"
                + "          'name': \"in/subdir\",\n"
                + "          'contents': [\n"
                + "            {\n"
                + "              'type': 'file',\n"
                + "              'name': \"foo4.h\",\n"
                + "              'external-contents': \"/real/foo4.h\"\n"
                + "            }\n"
                + "          ]\n"
                + "        }\n"
                + "      ]\n"
                + "    }\n"
                + "  ]\n"
                + "}\n";

            using(TestVFO T = new TestVFO(contents))
            {
                T.Map("/path/virtual/dir/foo1.h", "/real/foo1.h");
                T.Map("/another/dir/foo2.h", "/real/foo2.h");
                T.Map("/path/virtual/dir/foo3.h", "/real/foo3.h");
                T.Map("/path/virtual/dir/in/subdir/foo4.h", "/real/foo4.h");
            }
        }

        [Fact]
        public void CaseInsensitive()
        {
            var contents =
                "{\n"
                + "  'version': 0,\n"
                + "  'case-sensitive': 'false',\n"
                + "  'roots': [\n"
                + "    {\n"
                + "      'type': 'directory',\n"
                + "      'name': \"/path/virtual\",\n"
                + "      'contents': [\n"
                + "        {\n"
                + "          'type': 'file',\n"
                + "          'name': \"foo.h\",\n"
                + "          'external-contents': \"/real/foo.h\"\n"
                + "        }\n"
                + "      ]\n"
                + "    }\n"
                + "  ]\n"
                + "}\n";

            using(TestVFO T = new TestVFO(contents))
            {
                T.Map("/path/virtual/foo.h", "/real/foo.h");
                clang.VirtualFileOverlay_setCaseSensitivity(T.VFO, 0);
            }
        }

        [Fact]
        public void SharedPrefix()
        {
            var contents =
                "{\n"
                + "  'version': 0,\n"
                + "  'roots': [\n"
                + "    {\n"
                + "      'type': 'directory',\n"
                + "      'name': \"/path/foo\",\n"
                + "      'contents': [\n"
                + "        {\n"
                + "          'type': 'file',\n"
                + "          'name': \"bar\",\n"
                + "          'external-contents': \"/real/bar\"\n"
                + "        },\n"
                + "        {\n"
                + "          'type': 'file',\n"
                + "          'name': \"bar.h\",\n"
                + "          'external-contents': \"/real/bar.h\"\n"
                + "        }\n"
                + "      ]\n"
                + "    },\n"
                + "    {\n"
                + "      'type': 'directory',\n"
                + "      'name': \"/path/foobar\",\n"
                + "      'contents': [\n"
                + "        {\n"
                + "          'type': 'file',\n"
                + "          'name': \"baz.h\",\n"
                + "          'external-contents': \"/real/baz.h\"\n"
                + "        }\n"
                + "      ]\n"
                + "    },\n"
                + "    {\n"
                + "      'type': 'directory',\n"
                + "      'name': \"/path\",\n"
                + "      'contents': [\n"
                + "        {\n"
                + "          'type': 'file',\n"
                + "          'name': \"foobarbaz.h\",\n"
                + "          'external-contents': \"/real/foobarbaz.h\"\n"
                + "        }\n"
                + "      ]\n"
                + "    }\n"
                + "  ]\n"
                + "}\n";

            using(TestVFO T = new TestVFO(contents))
            {
                T.Map("/path/foo/bar.h", "/real/bar.h");
                T.Map("/path/foo/bar", "/real/bar");
                T.Map("/path/foobar/baz.h", "/real/baz.h");
                T.Map("/path/foobarbaz.h", "/real/foobarbaz.h");
            }
        }

        [Fact]
        public void AdjacentDirectory()
        {
            var contents =
                "{\n"
                + "  'version': 0,\n"
                + "  'roots': [\n"
                + "    {\n"
                + "      'type': 'directory',\n"
                + "      'name': \"/path/dir1\",\n"
                + "      'contents': [\n"
                + "        {\n"
                + "          'type': 'file',\n"
                + "          'name': \"foo.h\",\n"
                + "          'external-contents': \"/real/foo.h\"\n"
                + "        },\n"
                + "        {\n"
                + "          'type': 'directory',\n"
                + "          'name': \"subdir\",\n"
                + "          'contents': [\n"
                + "            {\n"
                + "              'type': 'file',\n"
                + "              'name': \"bar.h\",\n"
                + "              'external-contents': \"/real/bar.h\"\n"
                + "            }\n"
                + "          ]\n"
                + "        }\n"
                + "      ]\n"
                + "    },\n"
                + "    {\n"
                + "      'type': 'directory',\n"
                + "      'name': \"/path/dir2\",\n"
                + "      'contents': [\n"
                + "        {\n"
                + "          'type': 'file',\n"
                + "          'name': \"baz.h\",\n"
                + "          'external-contents': \"/real/baz.h\"\n"
                + "        }\n"
                + "      ]\n"
                + "    }\n"
                + "  ]\n"
                + "}\n";

            using(TestVFO T = new TestVFO(contents))
            {
                T.Map("/path/dir1/foo.h", "/real/foo.h");
                T.Map("/path/dir1/subdir/bar.h", "/real/bar.h");
                T.Map("/path/dir2/baz.h", "/real/baz.h");
            }
        }

        [Fact]
        public void TopLevel()
        {
            var contents =
                "{\n"
                + "  'version': 0,\n"
                + "  'roots': [\n"
                + "    {\n"
                + "      'type': 'directory',\n"
                + "      'name': \"/\",\n"
                + "      'contents': [\n"
                + "        {\n"
                + "          'type': 'file',\n"
                + "          'name': \"foo.h\",\n"
                + "          'external-contents': \"/real/foo.h\"\n"
                + "        }\n"
                + "      ]\n"
                + "    }\n"
                + "  ]\n"
                + "}\n";

            using(TestVFO T = new TestVFO(contents))
            {
                T.Map("/foo.h", "/real/foo.h");
            }
        }

        [Fact]
        public void Empty()
        {
            var contents =
                "{\n"
                + "  'version': 0,\n"
                + "  'roots': [\n"
                + "  ]\n"
                + "}\n";

            using(TestVFO T = new TestVFO(contents))
            {
            }
        }
    }
}
