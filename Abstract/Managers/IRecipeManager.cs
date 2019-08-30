using Microsoft.AspNetCore.Http;
using RecipeApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Abstract.Managers
{
    public interface IRecipeManager
    {
        Task<IEnumerable<Recipe>> GetAllRecipes();
        Task<IEnumerable<Recipe>> GetRecipesByNameOrTag(string query);
        Task<Stream> GetFile(string filePath);
        Task GenerateRecipe(Recipe recipe);
        Task DeleteRecipe(string name, string filePath);
        Task OverwriteRecipe(Recipe oldRecipe, Recipe newRecipe);

        Task WriteExistingRecipe(Recipe recipe, IFormFile file);
    }
}
