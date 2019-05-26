using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading.Tasks;
using ClangSharp;

namespace ClangSharpPInvokeGenerator
{
    public class Program
    {
        private static RootCommand s_rootCommand;

        public static async Task<int> Main(params string[] args)
        {
            s_rootCommand = new RootCommand();
            {
                s_rootCommand.Description = "ClangSharp P/Invoke Binding Generator";
                s_rootCommand.Handler = CommandHandler.Create(typeof(Program).GetMethod(nameof(Run)));

                AddAdditionalOption(s_rootCommand);
                AddConfigOption(s_rootCommand);
                AddDefineOption(s_rootCommand);
                AddExcludeFunctionOption(s_rootCommand);
                AddFileOption(s_rootCommand);
                AddIncludeOption(s_rootCommand);
                AddLibraryOption(s_rootCommand);
                AddMethodClassNameOption(s_rootCommand);
                AddNamespaceOption(s_rootCommand);
                AddOutputOption(s_rootCommand);
                AddPrefixStripOption(s_rootCommand);
            }
            return await s_rootCommand.InvokeAsync(args);
        }

        public static int Run(InvocationContext context)
        {
            var additionalArgs = context.ParseResult.ValueForOption<string[]>("additional");
            var config = new ConfigurationOptions(context.ParseResult.ValueForOption<string[]>("config"))
            {
                ExcludedFunctions = context.ParseResult.ValueForOption<string[]>("excludeFunction"),
                LibraryPath = context.ParseResult.ValueForOption<string>("libraryPath"),
                MethodClassName = context.ParseResult.ValueForOption<string>("methodClassName"),
                Namespace = context.ParseResult.ValueForOption<string>("namespace"),
                OutputLocation = context.ParseResult.ValueForOption<string>("output"),
                MethodPrefixToStrip = context.ParseResult.ValueForOption<string>("prefixStrip"),
            };
            var defines = context.ParseResult.ValueForOption<string[]>("define");
            var files = context.ParseResult.ValueForOption<string[]>("file");
            var includeDirs = context.ParseResult.ValueForOption<string[]>("include");

            var errorList = new List<string>();

            if (!files.Any())
            {
                errorList.Add("Error: No input C/C++ files provided. Use --file or -f");
            }

            if (string.IsNullOrWhiteSpace(config.Namespace))
            {
                errorList.Add("Error: No namespace provided. Use --namespace or -n");
            }

            if (string.IsNullOrWhiteSpace(config.OutputLocation))
            {
                errorList.Add("Error: No output file location provided. Use --output or -o");
            }

            if (string.IsNullOrWhiteSpace(config.LibraryPath))
            {
                errorList.Add("Error: No library path location provided. Use --libraryPath or -l");
            }

            if (errorList.Any())
            {
                foreach (var error in errorList)
                {
                    context.Console.Error.WriteLine(error);
                }
                context.Console.Error.WriteLine();

                new HelpBuilder(context.Console).Write(s_rootCommand);
                return -1;
            }

            var arr = new string[]
            {
                "-xc++",                                // The input files are C++
                "-Wno-pragma-once-outside-header"       // We are processing files which may be header files
            };

            arr = arr.Concat(includeDirs.Select(x => "-I" + x)).ToArray();
            arr = arr.Concat(defines.Select(x => "-D" + x)).ToArray();
            arr = arr.Concat(additionalArgs).ToArray();

            var translationFlags = CXTranslationUnit_Flags.CXTranslationUnit_None;

            translationFlags |= CXTranslationUnit_Flags.CXTranslationUnit_SkipFunctionBodies;                   // Don't traverse function bodies
            translationFlags |= CXTranslationUnit_Flags.CXTranslationUnit_IncludeAttributedTypes;               // Include attributed types in CXType
            translationFlags |= CXTranslationUnit_Flags.CXTranslationUnit_VisitImplicitAttributes;              // Implicit attributes should be visited

            using (var createIndex = CXIndex.Create())
            using (var cursorWriter = new CursorWriter(config))
            {
                foreach (var file in files)
                {
                    var translationUnitError = CXTranslationUnit.Parse(createIndex, file, arr, Array.Empty<CXUnsavedFile>(), translationFlags, out CXTranslationUnit translationUnitHandle);
                    bool skipProcessing = false;

                    if (translationUnitError != CXErrorCode.CXError_Success)
                    {
                        Console.WriteLine($"Error: Parsing failed for '{file}' due to '{translationUnitError}'.");
                        skipProcessing = true;
                    }
                    else if (translationUnitHandle.NumDiagnostics != 0)
                    {
                        Console.WriteLine($"Diagnostics for '{file}':");

                        for (uint i = 0; i < translationUnitHandle.NumDiagnostics; ++i)
                        {
                            using (var diagnostic = translationUnitHandle.GetDiagnostic(i))
                            {
                                Console.Write("    ");
                                Console.WriteLine(diagnostic.Format(CXDiagnosticDisplayOptions.CXDiagnostic_DisplayOption).ToString());

                                skipProcessing |= (diagnostic.Severity == CXDiagnosticSeverity.CXDiagnostic_Error);
                                skipProcessing |= (diagnostic.Severity == CXDiagnosticSeverity.CXDiagnostic_Fatal);
                            }
                        }
                    }

                    if (skipProcessing)
                    {
                        Console.WriteLine($"Skipping '{file}' due to one or more errors listed above.");
                        Console.WriteLine();
                        continue;
                    }

                    using (translationUnitHandle)
                    {
                        var translationUnit = new TranslationUnit(translationUnitHandle.Cursor);
                        translationUnit.Visit(clientData: default);
                        cursorWriter.Visit(translationUnit);
                    }
                }
            }

            return 0;
        }

        private static void AddAdditionalOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.OneOrMore;
            argument.Name = "arg";
            argument.SetDefaultValue(Array.Empty<string>());

            var option = new Option("--additional", "An argument to pass to Clang when parsing the input files.", argument);
            option.AddAlias("-a");

            rootCommand.AddOption(option);
        }

        private static void AddConfigOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.OneOrMore;
            argument.Name = "config";
            argument.SetDefaultValue(Array.Empty<string>());

            var option = new Option("--config", "A configuration option that controls how the bindings are generated.", argument);
            option.AddAlias("-c");

            rootCommand.AddOption(option);
        }

        private static void AddDefineOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.OneOrMore;
            argument.Name = "macro";
            argument.SetDefaultValue(Array.Empty<string>());

            var option = new Option("--define", "A macro for Clang to define when parsing the input files.", argument);
            option.AddAlias("-d");

            rootCommand.AddOption(option);
        }

        private static void AddExcludeFunctionOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.OneOrMore;
            argument.Name = "name";
            argument.SetDefaultValue(Array.Empty<string>());

            var option = new Option("--excludeFunction", "A function to exclude from binding generation.", argument);
            option.AddAlias("-e");

            rootCommand.AddOption(option);
        }

        private static void AddFileOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.OneOrMore;
            argument.Name = "file";
            argument.SetDefaultValue(Array.Empty<string>());

            var option = new Option("--file", "A file to parse and generate bindings for.", argument);
            option.AddAlias("-f");

            rootCommand.AddOption(option);
        }

        private static void AddIncludeOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.OneOrMore;
            argument.Name = "directory";
            argument.SetDefaultValue(Array.Empty<string>());

            var option = new Option("--include", "A directory for clang to use when resolving #include directives.", argument);
            option.AddAlias("-i");

            rootCommand.AddOption(option);
        }

        private static void AddLibraryOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.ExactlyOne;
            argument.Name = "dllName";
            argument.SetDefaultValue(string.Empty);

            var option = new Option("--libraryPath", "The string to use in the DllImport attribute used when generating bindings.", argument);
            option.AddAlias("-l");

            rootCommand.AddOption(option);
        }

        private static void AddMethodClassNameOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.ExactlyOne;
            argument.Name = "className";
            argument.SetDefaultValue("Methods");

            var option = new Option("--methodClassName", "The name of the static class that will contain the generated method bindings.", argument);
            option.AddAlias("-m");

            rootCommand.AddOption(option);
        }

        private static void AddNamespaceOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.ExactlyOne;
            argument.Name = "namespace";
            argument.SetDefaultValue(string.Empty);

            var option = new Option("--namespace", "The namespace in which to place the generated bindings.", argument);
            option.AddAlias("-n");

            rootCommand.AddOption(option);
        }

        private static void AddOutputOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.ExactlyOne;
            argument.Name = "file";
            argument.SetDefaultValue(string.Empty);

            var option = new Option("--output", "The output location to write the generated bindings to.", argument);
            option.AddAlias("-o");

            rootCommand.AddOption(option);
        }

        private static void AddPrefixStripOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.ExactlyOne;
            argument.Name = "prefix";
            argument.SetDefaultValue(string.Empty);

            var option = new Option("--prefixStrip", "The prefix to strip from the generated method bindings.", argument);
            option.AddAlias("-p");

            rootCommand.AddOption(option);
        }
    }
}
