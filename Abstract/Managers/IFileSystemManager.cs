using RecipeApp.Models;
using System.IO;
using System.Threading.Tasks;

namespace RecipeApp.Abstract.Managers
{
    public interface IFileSystemManager
    {
        Task<Stream> GetFile(string filePath);
        Recipe GenerateNewPDF(Recipe recipe);
        void DeleteFile(string filePath);
    }
}
