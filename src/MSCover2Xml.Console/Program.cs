using System;
using CommandLine;

namespace MSCover2Xml.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (!CommandLineParser.Default.ParseArguments(args, options))
                return;
            
            var symbolPaths = (options.SymbolPaths ?? string.Empty).Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            var executablePaths = (options.ExecutablePaths ?? string.Empty).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            System.Console.WriteLine("Generating coverage report from {0} ...", options.InputFile);
            PrintPaths("Symbols", symbolPaths);
            PrintPaths("Executables", executablePaths);

            try
            {
                System.Console.WriteLine("Generating XML at {0} ...", options.OutputFile);

                if (options.UseDataSetFormat)
                    CoverageReport.WriteDataSetXml(options.InputFile, options.OutputFile, executablePaths, symbolPaths);
                else
                    CoverageReport.WriteXml(options.InputFile, options.OutputFile, executablePaths, symbolPaths);

                System.Console.WriteLine("Done");
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine("ERROR: " + ex.Message);
            }
        }

        private static void PrintPaths(string pathName, string[] paths)
        {
            System.Console.Write(pathName + ": ");

            if (paths.Length == 0)
                System.Console.WriteLine("/");
            foreach (var path in paths)
                System.Console.WriteLine("\t" + path);
        }
    }
}
