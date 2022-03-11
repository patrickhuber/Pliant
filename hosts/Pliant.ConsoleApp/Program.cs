using Pliant.Forest;
using Pliant.Languages.Pdl;
using Pliant.Runtime;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace Pliant.ConsoleApp
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var grammarOption = new Option<FileInfo>( "-g" ).ExistingOnly();
            grammarOption.IsRequired = true;

            var fileOption = new Option<FileInfo>("-f").ExistingOnly();
            fileOption.IsRequired = true;

            var rootCommand = new RootCommand
            {                                
                grammarOption,
                fileOption
            };

            rootCommand.Description = "Pliant Parsing Tool";

            // The name of the parameters here MUST match the name of the option
            rootCommand.Handler = CommandHandler.Create((FileSystemInfo g, FileSystemInfo f)=> 
            {
                if (g is null)
                    throw new ArgumentNullException(nameof(g));
                if (f is null)
                    throw new ArgumentNullException(nameof(f));

                using var grammarStream = File.OpenRead(g.FullName);
                using var grammarReader = new StreamReader(grammarStream);
                var grammarParser = new PdlParser();
                var definition = grammarParser.Parse(grammarReader);
                var grammar = new PdlGrammarGenerator().Generate(definition);

                using var inputReader = File.OpenRead(f.FullName);
                using var textReader = new StreamReader(inputReader);
                var parseEngine = new ParseEngine(grammar);
                var parseRunner = new ParseRunner(parseEngine, textReader);

                parseRunner.RunToEnd();

                var parseForest = parseEngine.GetParseForestRootNode();
                var visitor = new LoggingForestNodeVisitor(Console.Out);
                parseForest.Accept(visitor);
            });

            // Parse the incoming args and invoke the handler
            return await rootCommand.InvokeAsync(args);
        }
    }
}
