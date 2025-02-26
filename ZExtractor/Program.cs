using System;
using System.IO;
using System.Threading;
using ICSharpCode.SharpZipLib.Lzw;
using SharpCompress.Archives;
using SharpCompress.Common;

class Program
{
    static void Main(string[] args)
    {
        // Sleep for 5 minutes before execution (useful for debugging in containers)
        //Thread.Sleep(300000);

        string dataFolder = "/app/data";
        string extractDirectory = Path.Combine(dataFolder, "extracted");

        try
        {
            // Ensure extraction directory exists
            Directory.CreateDirectory(extractDirectory);

            // Scan directory for .Z files
            string[] compressedFiles = Directory.GetFiles(dataFolder, "*.Z");

            if (compressedFiles.Length == 0)
            {
                Console.WriteLine("No .Z files found in the directory.");
                return;
            }

            foreach (string compressedFilePath in compressedFiles)
            {
                Console.WriteLine($"Processing file: {compressedFilePath}");

                if (compressedFilePath.EndsWith(".tar.Z"))
                {
                    ExtractTarZFile(compressedFilePath, extractDirectory);
                }
                else
                {
                    ExtractZFile(compressedFilePath);
                }
            }

            Console.WriteLine("All extractions complete.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void ExtractZFile(string compressedFilePath)
    {
        try
        {
            string extractedFilePath = compressedFilePath.Replace(".Z", "");
            Console.WriteLine($"Detected .Z file. Extracting {compressedFilePath}...");

            using (FileStream input = File.OpenRead(compressedFilePath))
            using (FileStream output = File.Create(extractedFilePath))
            using (var decompressor = new LzwInputStream(input))
            {
                decompressor.CopyTo(output);
            }

            Console.WriteLine($"Successfully decompressed: {extractedFilePath}");

            // Print extracted file content
            if (File.Exists(extractedFilePath))
            {
                Console.WriteLine("Extracted File Content:");
                Console.WriteLine("------------------------");
                Console.WriteLine(File.ReadAllText(extractedFilePath));
                Console.WriteLine("------------------------");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting {compressedFilePath}: {ex.Message}");
        }
    }

    static void ExtractTarZFile(string compressedFilePath, string extractDirectory)
    {
        try
        {
            string extractedTarFile = compressedFilePath.Replace(".Z", "");
            Console.WriteLine($"Detected .tar.Z file. Extracting {compressedFilePath}...");

            // Step 1: Decompress .Z to .tar
            using (FileStream input = File.OpenRead(compressedFilePath))
            using (FileStream output = File.Create(extractedTarFile))
            using (var decompressor = new LzwInputStream(input))
            {
                decompressor.CopyTo(output);
            }

            Console.WriteLine($"Decompressed {compressedFilePath} to {extractedTarFile}");

            // Step 2: Extract .tar using SharpCompress
            using (Stream tarStream = File.OpenRead(extractedTarFile))
            using (var archive = ArchiveFactory.Open(tarStream))
            {
                foreach (var entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        entry.WriteToDirectory(extractDirectory, new ExtractionOptions { ExtractFullPath = true, Overwrite = true });
                        Console.WriteLine($"Extracted: {entry.Key}");

                        // ✅ Print content of extracted file (if it's a text file)
                        string extractedTextFile = Path.Combine(extractDirectory, entry.Key);
                        if (File.Exists(extractedTextFile))
                        {
                            Console.WriteLine($"Content of {entry.Key}:");
                            Console.WriteLine("------------------------");
                            Console.WriteLine(File.ReadAllText(extractedTextFile));
                            Console.WriteLine("------------------------");
                        }
                    }
                }
            }
            Console.WriteLine("Extraction complete.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting {compressedFilePath}: {ex.Message}");
        }
    }
}
