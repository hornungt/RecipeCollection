using Microsoft.AspNetCore.Http;
using RecipeApp.Models;
using System.IO;
using System.Threading.Tasks;

namespace RecipeApp.Abstract.Repositories
{
    public interface IFileSystemRepository
    {
        Task<Stream> GetFile(string filePath);
        Recipe GenerateNewPDF(Recipe recipe);
        Task<Recipe> SaveExistingPDF(Recipe recipe, IFormFile file);
        void DeleteFile(string filePath);

        string RenamePDF(string oldPath, string newName);
    }
}
