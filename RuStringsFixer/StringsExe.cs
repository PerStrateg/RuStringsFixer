using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RuStringsFixer
{
    class StringsExe
    {
        public static ArrayList files;

        static StringsExe()
        {
            files = new ArrayList();
        }

        /// <summary>
        /// Extract Embedded resource files to a given path
        /// </summary>
        /// <param name="embeddedFileName">Name of the embedded resource file</param>
        /// <param name="destinationPath">Path and file to export resource to</param>
        public static void ExtractResource(String embeddedFileName, String destinationPath)
        {
            Directory.CreateDirectory(destinationPath);

            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            string[] arrResources = currentAssembly.GetManifestResourceNames();
            foreach (string resourceName in arrResources)
                if (resourceName.ToUpper().EndsWith(embeddedFileName.ToUpper()))
                {
                    Stream resourceToSave = currentAssembly.GetManifestResourceStream(resourceName);
                    FileStream output = File.OpenWrite(destinationPath + "\\" + embeddedFileName);
                    files.Add(output);

                    resourceToSave.CopyTo(output);
                    resourceToSave.Close();
                }

            foreach (FileStream f in files)
            {
                f.Close();
            }
        }

        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static void DeleteDirectory(string path, string fullpath)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(fullpath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            di.Delete();

            //if (Directory.Exists(path))
            //{
            //    Directory.Delete(path);
            //}
        }

        public static bool CheckIfStringsExt(string file)
        {
            return (file.Substring(file.Length - 7).ToUpper().Equals("STRINGS"));
        }

        public static int StartProcess(string processName, string commandLineArgs = null)
        {
            Process process = new Process();
            process.StartInfo.FileName = processName;
            process.StartInfo.Arguments = commandLineArgs;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.ErrorDialog = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();

            return process.Id;
        }

        public static string StringsToCsv(string orig, string tempPath)
        {
            String line = "";
            
            line += "\"" + orig + "\"" + " ";
            line += "\"" + tempPath + "file.csv" + "\"" + " ";
            line += "/E1251";

            return line;
        }

        public static void ConvertUTF8ToAnsi(string inputFilePath, string outputFilePath)
        {
            string fileContent = File.ReadAllText(inputFilePath, Encoding.UTF8);
            File.WriteAllText(outputFilePath, fileContent, Encoding.Default);
        }

        public static void ConvertAnsiToUTF8(string inputFilePath, string outputFilePath)
        {
            string fileContent = File.ReadAllText(inputFilePath, Encoding.Default);
            File.WriteAllText(outputFilePath, fileContent, Encoding.UTF8);
        }

        public static string CsvToStrings(string file, string dest)
        {
            String line = "";

            line += "\"" + file + "\"" + " ";
            line += "\"" + dest + "\"" + " ";
            line += "/E1251";

            return line;
        }

    }
}