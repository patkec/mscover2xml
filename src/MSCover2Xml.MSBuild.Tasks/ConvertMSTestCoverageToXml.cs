using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;


namespace MSCover2Xml.MSBuild.Tasks
{
    public class ConvertMSTestCoverageToXml: Task
    {
        /// <summary>
        /// Gets or sets a value that indicates if CoverageDS format should be used.
        /// </summary>
        public bool UseDataSetFormat { get; set; }
        /// <summary>
        /// Gets or sets a list of MS test coverage files which should be converted to XML.
        /// </summary>
        [Required]
        public ITaskItem[] CoverageFiles { get; set; }
        /// <summary>
        /// Gets or sets optional symbols directory to be used during conversion.
        /// </summary>
        public ITaskItem SymbolsDirectory { get; set; }
        /// <summary>
        /// Gets or sets optional output directory.
        /// </summary>
        public ITaskItem OutputDirectory { get; set; }
        /// <summary>
        /// Gets a list of converted XML files.
        /// </summary>
        [Output]
        public ITaskItem[] ConvertedFiles { get; private set; }

        public ConvertMSTestCoverageToXml(): base(new ResourceManager(typeof(Strings)))
        {
        }

        public override bool Execute()
        {
            var list = new List<ITaskItem>();
            var coverageFiles = CoverageFiles;
            foreach (var taskItem in coverageFiles)
            {
                try
                {
                    string itemSpec = taskItem.ItemSpec;
                    if (File.Exists(itemSpec))
                    {
                        Log.LogMessageFromResources(MessageImportance.Normal, "ConvertingCoverageFile", new object[] { itemSpec });
                        string outputFile = GetOutputFile(itemSpec);
                        string symbolsDirectory = GetSymbolsDirectory(itemSpec);

                        if (UseDataSetFormat)
                            CoverageReport.WriteDataSetXml(itemSpec, outputFile, new[] { symbolsDirectory }, new[] { symbolsDirectory });
                        else
                            CoverageReport.WriteXml(itemSpec, outputFile, new[] { symbolsDirectory }, new[] { symbolsDirectory });
                        
                        list.Add(new TaskItem(outputFile));
                        Log.LogMessageFromResources(MessageImportance.Normal, "WrittenXmlCoverageFile", new object[] { outputFile });
                    }
                    else
                    {
                        Log.LogMessageFromResources(MessageImportance.Normal, "SkippingNonExistentFile", new object[] { itemSpec });
                    }
                }
                catch (Exception exception)
                {
                    Log.LogErrorFromException(exception, true);
                }
            }
            ConvertedFiles = list.ToArray();
            return !Log.HasLoggedErrors;
        }

        private string GetOutputFile(string sourceFile)
        {
            var result = Path.ChangeExtension(sourceFile, "xml");
            if (OutputDirectory != null)
                result = Path.Combine(OutputDirectory.ItemSpec, Path.GetFileName(result));
            return result;
        }

        private string GetSymbolsDirectory(string sourceFile)
        {
            if (SymbolsDirectory != null)
                return SymbolsDirectory.ItemSpec;
            return Path.GetDirectoryName(sourceFile);
        }
    }
}
