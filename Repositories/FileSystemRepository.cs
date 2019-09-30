using Microsoft.AspNetCore.Http;
using RecipeApp.Abstract.Repositories;
using RecipeApp.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace RecipeApp.Repositories
{
    public class FileSystemRepository : IFileSystemRepository
    {

        public FileSystemRepository()
        {

        }

        public Recipe GenerateNewPDF(Recipe recipe)
        {
            try
            {
                var destination = recipe.Name + ".pdf";
                using (Process process = new Process())
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;

                    var date = DateTime.Now;
                    var dir = Directory.GetCurrentDirectory();
                    var pdfsDir = dir + $"\\pdfs\\{date:yyyy}\\{date:MM}\\{date:dd}\\";
                    Directory.CreateDirectory(pdfsDir);
                    var fileDir = dir + "\\ClientApp\\src\\generators\\generate-pdf.js";
                    destination = pdfsDir + destination;

                    process.StartInfo.FileName = "node.exe";
                    process.StartInfo.Arguments = $"{fileDir} {recipe.Url} \"{destination}\"";

                    process.Start();
                }
                recipe.Path = destination;
                return recipe;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        public async Task<Recipe> SaveExistingPDF(Recipe recipe, IFormFile file)
        {
            try
            {
                var date = DateTime.Now;
                var dir = Directory.GetCurrentDirectory();
                var destination = Path.Combine(dir, "pdfs", $"{date:yyy}", $"{date:MM}", $"{date:dd}");
                Directory.CreateDirectory(destination);
                var pdfDestination = Path.Combine(destination, recipe.Name + ".pdf");

                using (var stream = new FileStream(pdfDestination, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                recipe.Path = pdfDestination;

                return recipe;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        public string RenamePDF(string oldPath, string newName)
        {
            var path = oldPath.Remove(oldPath.LastIndexOf("\\"));
            var newFileName = newName + ".pdf";
            var newPath = Path.Combine(path, newFileName);

            File.Move(oldPath, newPath);

            return newPath;
        }

        public async Task<Stream> GetFile(string filePath)
        {
            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            var dir = Directory.GetCurrentDirectory().Replace('\\', '/');
            if (filePath == null)
            {
                throw new FileNotFoundException("No file path given");
            }
            else if (!filePath.StartsWith(dir))
            {
                throw new FileNotFoundException("Invalid file path");
            }
            if (isLinux)
            {
                filePath.Replace('\\', Path.DirectorySeparatorChar);
            }
            var memory = new MemoryStream();
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            memory.Position = 0;
            return memory;
        }

        public void DeleteFile(string filePath)
        {
            filePath = filePath.Replace("\\", "/");
            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            var dir = Directory.GetCurrentDirectory().Replace('\\', '/'); // "C:/Users/tthor/source/repos/RecipeApp"
            if (filePath == null)
            {
                throw new FileNotFoundException("No file path given");
            }
            else if (!filePath.StartsWith(dir))
            {
                throw new FileNotFoundException("Invalid file path");
            }
            if (isLinux)
            {
                filePath.Replace('\\', Path.DirectorySeparatorChar);
            }
            try
            {
                File.Delete(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetType().FullName + ": " + e.Message);
            }
        }
    }
}
