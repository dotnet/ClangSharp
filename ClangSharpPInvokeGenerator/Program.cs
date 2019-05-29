using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading.Tasks;

namespace ClangSharp
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
                AddExcludeOption(s_rootCommand);
                AddFileOption(s_rootCommand);
                AddIncludeOption(s_rootCommand);
                AddLibraryOption(s_rootCommand);
                AddMethodClassNameOption(s_rootCommand);
                AddNamespaceOption(s_rootCommand);
                AddOutputOption(s_rootCommand);
                AddPrefixStripOption(s_rootCommand);
                AddRemapOption(s_rootCommand);
            }
            return await s_rootCommand.InvokeAsync(args);
        }

        public static int Run(InvocationContext context)
        {
            var additionalArgs = context.ParseResult.ValueForOption<string[]>("additional");
            var configSwitches = context.ParseResult.ValueForOption<string[]>("config");
            var defines = context.ParseResult.ValueForOption<string[]>("define");
            var excludedNames = context.ParseResult.ValueForOption<string[]>("exclude");
            var files = context.ParseResult.ValueForOption<string[]>("file");
            var includeDirs = context.ParseResult.ValueForOption<string[]>("include");
            var libraryPath = context.ParseResult.ValueForOption<string>("libraryPath");
            var methodClassName = context.ParseResult.ValueForOption<string>("methodClassName");
            var methodPrefixToStrip = context.ParseResult.ValueForOption<string>("prefixStrip");
            var namespaceName = context.ParseResult.ValueForOption<string>("namespace");
            var outputLocation = context.ParseResult.ValueForOption<string>("output");
            var remappedNameValuePairs = context.ParseResult.ValueForOption<string[]>("remap");

            var errorList = new List<string>();

            if (!files.Any())
            {
                errorList.Add("Error: No input C/C++ files provided. Use --file or -f");
            }

            if (string.IsNullOrWhiteSpace(libraryPath))
            {
                errorList.Add("Error: No library path location provided. Use --libraryPath or -l");
            }

            if (string.IsNullOrWhiteSpace(namespaceName))
            {
                errorList.Add("Error: No namespace provided. Use --namespace or -n");
            }

            if (string.IsNullOrWhiteSpace(outputLocation))
            {
                errorList.Add("Error: No output file location provided. Use --output or -o");
            }

            var remappedNames = new Dictionary<string, string>();

            foreach (var remappedNameValuePair in remappedNameValuePairs)
            {
                var parts = remappedNameValuePair.Split('=');

                if (parts.Length != 2)
                {
                    errorList.Add($"Error: Invalid remap argument: {remappedNameValuePair}. Expected 'name=value'");
                    continue;
                }

                remappedNames[parts[0].TrimEnd()] = parts[1].TrimStart();
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

            var clangCommandLineArgs = new string[]
            {
                "-xc++",                                // The input files are C++
                "-Wno-pragma-once-outside-header"       // We are processing files which may be header files
            };

            clangCommandLineArgs = clangCommandLineArgs.Concat(includeDirs.Select(x => "-I" + x)).ToArray();
            clangCommandLineArgs = clangCommandLineArgs.Concat(defines.Select(x => "-D" + x)).ToArray();
            clangCommandLineArgs = clangCommandLineArgs.Concat(additionalArgs).ToArray();

            var translationFlags = CXTranslationUnit_Flags.CXTranslationUnit_None;

            translationFlags |= CXTranslationUnit_Flags.CXTranslationUnit_SkipFunctionBodies;                   // Don't traverse function bodies
            translationFlags |= CXTranslationUnit_Flags.CXTranslationUnit_IncludeAttributedTypes;               // Include attributed types in CXType
            translationFlags |= CXTranslationUnit_Flags.CXTranslationUnit_VisitImplicitAttributes;              // Implicit attributes should be visited

            var configOptions = PInvokeGeneratorConfigurationOptions.None;

            foreach (var configSwitch in configSwitches)
            {
                if (configSwitch.EndsWith("multi-file"))
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateMultipleFiles;
                }
                else if (configSwitch.Equals("unsafe"))
                {
                    configOptions |= PInvokeGeneratorConfigurationOptions.GenerateUnsafeCode;
                }
            }

            var config = new PInvokeGeneratorConfiguration(libraryPath, namespaceName, outputLocation, configOptions, excludedNames, methodClassName, methodPrefixToStrip, remappedNames);

            int exitCode = 0;

            using (var pinvokeGenerator = new PInvokeGenerator(config))
            {
                foreach (var file in files)
                {
                    var translationUnitError = CXTranslationUnit.Parse(pinvokeGenerator.IndexHandle, file, clangCommandLineArgs, Array.Empty<CXUnsavedFile>(), translationFlags, out CXTranslationUnit translationUnitHandle);
                    var skipProcessing = false;

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

                        exitCode = -1;
                        continue;
                    }

                    using (translationUnitHandle)
                    {
                        Console.WriteLine($"Processing '{file}'");
                        pinvokeGenerator.GenerateBindings(translationUnitHandle);
                    }
                }

                if (pinvokeGenerator.Diagnostics.Count != 0)
                {
                    Console.WriteLine("Diagnostics for binding generation:");

                    foreach (var diagnostic in pinvokeGenerator.Diagnostics)
                    {
                        Console.Write("    ");
                        Console.WriteLine(diagnostic);

                        if (diagnostic.Level == DiagnosticLevel.Warning)
                        {
                            if (exitCode >= 0)
                            {
                                exitCode++;
                            }
                        }
                        else if (diagnostic.Level == DiagnosticLevel.Error)
                        {
                            if (exitCode >= 0)
                            {
                                exitCode = -1;
                            }
                            else
                            {
                                exitCode--;
                            }
                        }
                    }
                }
            }

            return exitCode;
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

        private static void AddExcludeOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.OneOrMore;
            argument.Name = "name";
            argument.SetDefaultValue(Array.Empty<string>());

            var option = new Option("--exclude", "A declaration name to exclude from binding generation.", argument);
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

        private static void AddRemapOption(RootCommand rootCommand)
        {
            var argument = new Argument();
            argument.ArgumentType = typeof(string);
            argument.Arity = ArgumentArity.OneOrMore;
            argument.Name = "name=value";
            argument.SetDefaultValue(Array.Empty<string>());

            var option = new Option("--remap", "A declaration name to be remapped to another name during binding generation.", argument);
            option.AddAlias("-r");

            rootCommand.AddOption(option);
        }
    }
}
