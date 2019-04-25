namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public static partial class clang
    {
        internal struct _CXUnsavedFile
        {
            public IntPtr @Filename;
            public IntPtr @Contents;
            public int @Length;
        }

        internal struct _CXIdxEntityInfo
        {
            public CXIdxEntityKind @kind;
            public CXIdxEntityCXXTemplateKind @templateKind;
            public CXIdxEntityLanguage @lang;
            public IntPtr @name;
            public IntPtr @USR;
            public CXCursor @cursor;
            public IntPtr @attributes;
            public uint @numAttributes;
        }

        public static CXTranslationUnit createTranslationUnitFromSourceFile(CXIndex @CIdx, string @source_filename, int @num_clang_command_line_args, string[] @clang_command_line_args, uint @num_unsaved_files, CXUnsavedFile[] @unsaved_files)
        {
            var arr = new _CXUnsavedFile[unsaved_files.Length];

            try
            {
                BeginCXUnsavedFileMarshal(ref arr, ref unsaved_files);
                return createTranslationUnitFromSourceFile(CIdx, source_filename, num_clang_command_line_args, clang_command_line_args, num_unsaved_files, arr);
            }
            finally
            {
                EndCXUnsavedFileMarshal(ref arr);
            }
        }

        public static CXTranslationUnit parseTranslationUnit(CXIndex @CIdx, string @source_filename, string[] @command_line_args, int @num_command_line_args, CXUnsavedFile[] @unsaved_files, uint @num_unsaved_files, uint @options)
        {
            var arr = new _CXUnsavedFile[unsaved_files.Length];

            try
            {
                BeginCXUnsavedFileMarshal(ref arr, ref unsaved_files);
                return parseTranslationUnit(CIdx, source_filename, @command_line_args, @num_command_line_args, arr, @num_unsaved_files, options);
            }
            finally
            {
                EndCXUnsavedFileMarshal(ref arr);
            }
        }

        public static CXErrorCode parseTranslationUnit2(CXIndex @CIdx, string @source_filename, string[] @command_line_args, int @num_command_line_args, CXUnsavedFile[] @unsaved_files, uint @num_unsaved_files, uint @options, out CXTranslationUnit @out_TU)
        {
            var arr = new _CXUnsavedFile[unsaved_files.Length];

            try
            {
                BeginCXUnsavedFileMarshal(ref arr, ref unsaved_files);
                return parseTranslationUnit2(CIdx, source_filename, command_line_args, num_command_line_args, arr, num_unsaved_files, options, out out_TU);
            }
            finally
            {
                EndCXUnsavedFileMarshal(ref arr);
            }
        }

        public static CXErrorCode parseTranslationUnit2FullArgv(CXIndex @CIdx, string @source_filename, string[] @command_line_args, int @num_command_line_args, CXUnsavedFile[] @unsaved_files, uint @num_unsaved_files, uint @options, out CXTranslationUnit @out_TU)
        {
            var arr = new _CXUnsavedFile[unsaved_files.Length];

            try
            {
                BeginCXUnsavedFileMarshal(ref arr, ref unsaved_files);
                return parseTranslationUnit2FullArgv(CIdx, source_filename, command_line_args, num_command_line_args, arr, num_unsaved_files, options, out out_TU);
            }
            finally
            {
                EndCXUnsavedFileMarshal(ref arr);
            }
        }

        public static int reparseTranslationUnit(CXTranslationUnit @TU, uint @num_unsaved_files, CXUnsavedFile[] @unsaved_files, uint @options)
        {
            var arr = new _CXUnsavedFile[unsaved_files.Length];

            try
            {
                BeginCXUnsavedFileMarshal(ref arr, ref unsaved_files);
                return reparseTranslationUnit(TU, num_unsaved_files, arr, options);
            }
            finally
            {
                EndCXUnsavedFileMarshal(ref arr);
            }
        }

        public static IntPtr codeCompleteAt(CXTranslationUnit @TU, string @complete_filename, uint @complete_line, uint @complete_column, CXUnsavedFile[] @unsaved_files, uint @num_unsaved_files, uint @options)
        {
            var arr = new _CXUnsavedFile[unsaved_files.Length];

            try
            {
                BeginCXUnsavedFileMarshal(ref arr, ref unsaved_files);
                return codeCompleteAt(TU, complete_filename, complete_line, complete_column, arr, num_unsaved_files, options);
            }
            finally
            {
                EndCXUnsavedFileMarshal(ref arr);
            }
        }

        public static int indexSourceFile(CXIndexAction @param0, CXClientData @client_data, IndexerCallbacks[] @index_callbacks, uint @index_callbacks_size, uint @index_options, string @source_filename, string[] @command_line_args, int @num_command_line_args, CXUnsavedFile[] @unsaved_files, uint @num_unsaved_files, out CXTranslationUnit @out_TU, uint @TU_options)
        {
            var arr = new _CXUnsavedFile[unsaved_files.Length];
            IntPtr result = IntPtr.Zero;

            try
            {
                BeginCXUnsavedFileMarshal(ref arr, ref unsaved_files);
                BeginIndexerCallbacksMarshal(out result, ref index_callbacks);

                return indexSourceFile(param0, client_data, result, index_callbacks_size, index_options, source_filename, command_line_args, num_command_line_args, arr, num_unsaved_files, out out_TU, TU_options);
            }
            finally
            {
                EndCXUnsavedFileMarshal(ref arr);
                EndIndexerCallbacksMarshal(result);
            }
        }

        public static int indexSourceFileFullArgv(CXIndexAction @param0, CXClientData @client_data, IndexerCallbacks[] @index_callbacks, uint @index_callbacks_size, uint @index_options, string @source_filename, string[] @command_line_args, int @num_command_line_args, CXUnsavedFile[] @unsaved_files, uint @num_unsaved_files, out CXTranslationUnit @out_TU, uint @TU_options)
        {
            var arr = new _CXUnsavedFile[unsaved_files.Length];
            IntPtr result = IntPtr.Zero;

            try
            {
                BeginCXUnsavedFileMarshal(ref arr, ref unsaved_files);
                BeginIndexerCallbacksMarshal(out result, ref index_callbacks);

                return indexSourceFileFullArgv(param0, client_data, result, index_callbacks_size, index_options, source_filename, command_line_args, num_command_line_args, arr, num_unsaved_files, out out_TU, TU_options);
            }
            finally
            {
                EndCXUnsavedFileMarshal(ref arr);
                EndIndexerCallbacksMarshal(result);
            }
        }

        public static CXIdxClientEntity index_getClientEntity(ref CXIdxEntityInfo @param0)
        {
            var temp = new _CXIdxEntityInfo
            {
                kind = param0.kind,
                templateKind = param0.templateKind,
                lang = param0.lang,
                cursor = param0.cursor,
                attributes = param0.attributes,
                numAttributes = param0.numAttributes
            };

            try
            {
                temp.name = Marshal.StringToHGlobalAnsi(param0.name);
                temp.USR = Marshal.StringToHGlobalAnsi(param0.USR);
                return index_getClientEntity(ref temp);
            }
            finally
            {
                if (temp.name != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(temp.name);
                }

                if (temp.USR != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(temp.USR);
                }
            }
        }

        public static void index_setClientEntity(ref CXIdxEntityInfo @param0, CXIdxClientEntity @param1)
        {
            var temp = new _CXIdxEntityInfo
            {
                kind = param0.kind,
                templateKind = param0.templateKind,
                lang = param0.lang,
                cursor = param0.cursor,
                attributes = param0.attributes,
                numAttributes = param0.numAttributes
            };

            try
            {
                temp.name = Marshal.StringToHGlobalAnsi(param0.name);
                temp.USR = Marshal.StringToHGlobalAnsi(param0.USR);
                index_setClientEntity(ref temp, param1);
            }
            finally
            {
                if (temp.name != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(temp.name);
                }

                if (temp.USR != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(temp.USR);
                }
            }
        }

        [DllImport(libraryPath, EntryPoint = "clang_disposeSourceRangeList", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeSourceRangeList(ref CXSourceRangeList @ranges);

        [DllImport(libraryPath, EntryPoint = "clang_annotateTokens", CallingConvention = CallingConvention.Cdecl)]
        public static extern void annotateTokens(CXTranslationUnit @TU, [MarshalAs(UnmanagedType.LPArray)] CXToken[] @Tokens, uint @NumTokens, [MarshalAs(UnmanagedType.LPArray)] CXCursor[] @Cursors);

        [DllImport(libraryPath, EntryPoint = "clang_disposeTokens", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeTokens(CXTranslationUnit @TU, [MarshalAs(UnmanagedType.LPArray)] CXToken[] @Tokens, uint @NumTokens);

        [DllImport(libraryPath, EntryPoint = "clang_tokenize", CallingConvention = CallingConvention.Cdecl)]
        public static extern void tokenize(CXTranslationUnit @TU, CXSourceRange @Range, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] out CXToken[] @Tokens, out uint @NumTokens);

        [DllImport(libraryPath, EntryPoint = "clang_disposeStringSet", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeStringSet(ref CXStringSet @set);

        [DllImport(libraryPath, EntryPoint = "clang_getCompletionParent", CallingConvention = CallingConvention.Cdecl)]
        public static extern CXString getCompletionParent(CXCompletionString @completion_string, ref CXCursorKind @kind);

        [DllImport(libraryPath, EntryPoint = "clang_disposeCXPlatformAvailability", CallingConvention = CallingConvention.Cdecl)]
        public static extern void disposeCXPlatformAvailability(ref CXPlatformAvailability @availability);

        [DllImport(libraryPath, EntryPoint = "clang_getCursorPlatformAvailability", CallingConvention = CallingConvention.Cdecl)]
        public static extern int getCursorPlatformAvailability(CXCursor @cursor, [MarshalAs(UnmanagedType.Bool)] out bool @always_deprecated, out CXString @deprecated_message, [MarshalAs(UnmanagedType.Bool)] out bool @always_unavailable, out CXString @unavailable_message, [MarshalAs(UnmanagedType.LPArray)] CXPlatformAvailability[] @availability, int @availability_size);

        [DllImport(libraryPath, EntryPoint = "clang_createTranslationUnitFromSourceFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern CXTranslationUnit createTranslationUnitFromSourceFile(CXIndex @CIdx, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string @source_filename, int @num_clang_command_line_args, string[] @clang_command_line_args, uint @num_unsaved_files, [MarshalAs(UnmanagedType.LPArray)] _CXUnsavedFile[] @unsaved_files);

        [DllImport(libraryPath, EntryPoint = "clang_parseTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        private static extern CXTranslationUnit parseTranslationUnit(CXIndex @CIdx, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string @source_filename, string[] @command_line_args, int @num_command_line_args, [MarshalAs(UnmanagedType.LPArray)] _CXUnsavedFile[] @unsaved_files, uint @num_unsaved_files, uint @options);

        [DllImport(libraryPath, EntryPoint = "clang_parseTranslationUnit2", CallingConvention = CallingConvention.Cdecl)]
        private static extern CXErrorCode parseTranslationUnit2(CXIndex @CIdx, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string @source_filename, string[] @command_line_args, int @num_command_line_args, [MarshalAs(UnmanagedType.LPArray)] _CXUnsavedFile[] @unsaved_files, uint @num_unsaved_files, uint @options, out CXTranslationUnit @out_TU);

        [DllImport(libraryPath, EntryPoint = "clang_parseTranslationUnit2FullArgv", CallingConvention = CallingConvention.Cdecl)]
        private static extern CXErrorCode parseTranslationUnit2FullArgv(CXIndex @CIdx, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string @source_filename, string[] @command_line_args, int @num_command_line_args, [MarshalAs(UnmanagedType.LPArray)] _CXUnsavedFile[] @unsaved_files, uint @num_unsaved_files, uint @options, out CXTranslationUnit @out_TU);

        [DllImport(libraryPath, EntryPoint = "clang_reparseTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
        private static extern int reparseTranslationUnit(CXTranslationUnit @TU, uint @num_unsaved_files, [MarshalAs(UnmanagedType.LPArray)] _CXUnsavedFile[] @unsaved_files, uint @options);

        [DllImport(libraryPath, EntryPoint = "clang_codeCompleteAt", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr codeCompleteAt(CXTranslationUnit @TU, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string @complete_filename, uint @complete_line, uint @complete_column, [MarshalAs(UnmanagedType.LPArray)] _CXUnsavedFile[] @unsaved_files, uint @num_unsaved_files, uint @options);

        [DllImport(libraryPath, EntryPoint = "clang_indexSourceFile", CallingConvention = CallingConvention.Cdecl)]
        private static extern int indexSourceFile(CXIndexAction @param0, CXClientData @client_data, IntPtr @index_callbacks, uint @index_callbacks_size, uint @index_options, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string @source_filename, string[] @command_line_args, int @num_command_line_args, [MarshalAs(UnmanagedType.LPArray)] _CXUnsavedFile[] @unsaved_files, uint @num_unsaved_files, out CXTranslationUnit @out_TU, uint @TU_options);

        [DllImport(libraryPath, EntryPoint = "clang_indexSourceFileFullArgv", CallingConvention = CallingConvention.Cdecl)]
        private static extern int indexSourceFileFullArgv(CXIndexAction @param0, CXClientData @client_data, IntPtr @index_callbacks, uint @index_callbacks_size, uint @index_options, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string @source_filename, string[] @command_line_args, int @num_command_line_args, [MarshalAs(UnmanagedType.LPArray)] _CXUnsavedFile[] @unsaved_files, uint @num_unsaved_files, out CXTranslationUnit @out_TU, uint @TU_options);

        [DllImport(libraryPath, EntryPoint = "clang_index_getClientEntity", CallingConvention = CallingConvention.Cdecl)]
        private static extern CXIdxClientEntity index_getClientEntity(ref _CXIdxEntityInfo @param0);

        [DllImport(libraryPath, EntryPoint = "clang_index_setClientEntity", CallingConvention = CallingConvention.Cdecl)]
        private static extern void index_setClientEntity(ref _CXIdxEntityInfo @param0, CXIdxClientEntity @param1);

        private static void BeginIndexerCallbacksMarshal(out IntPtr result, ref IndexerCallbacks[] index_callbacks)
        {
            int length = index_callbacks.Length;
            int size = Marshal.SizeOf(typeof(IndexerCallbacks));

            result = Marshal.AllocHGlobal(size * length);
            var ptr = result.ToInt64();

            for (int i = 0; i < length; ++i)
            {
                var cursor = new IntPtr(ptr);
                Marshal.StructureToPtr(index_callbacks[i], cursor, false);
                ptr += Marshal.SizeOf(size);
            }
        }

        private static void EndIndexerCallbacksMarshal(IntPtr result)
        {
            if (result != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(result);
            }
        }

        private static void BeginCXUnsavedFileMarshal(ref _CXUnsavedFile[] arr, ref CXUnsavedFile[] @unsaved_files)
        {
            int size = arr.Length;
            for (int i = 0; i < size; ++i)
            {
                arr[i].Length = (int)unsaved_files[i].Length;
                arr[i].Filename = Marshal.StringToHGlobalAnsi(unsaved_files[i].Filename);
                arr[i].Contents = Marshal.StringToHGlobalAnsi(unsaved_files[i].Contents);
            }
        }

        private static void EndCXUnsavedFileMarshal(ref _CXUnsavedFile[] arr)
        {
            int size = arr.Length;
            for (int i = 0; i < size; ++i)
            {
                if (arr[i].Filename != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(arr[i].Filename);
                }

                if (arr[i].Contents != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(arr[i].Contents);
                }
            }
        }
    }
}