using CommandLine;
using CommandLine.Text;

namespace MSCover2Xml.Console
{
    class Options: CommandLineOptionsBase
    {
        [Option("i", "input", HelpText = "MS test coverage input file.", Required = true)]
        public string InputFile { get; set; }
        [Option("o", "output", HelpText = "XML output file.", Required = true)]
        public string OutputFile { get; set; }
        [Option("x", "executables", HelpText = "Semicolon-delimited list of executable paths.", Required = false)]
        public string ExecutablePaths { get; set; }
        [Option("s", "symbols", HelpText = "Semicolon-delimited list of symbol paths.", Required = false)]
        public string SymbolPaths { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}