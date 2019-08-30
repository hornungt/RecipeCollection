using RecipeApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeApp.Abstract.Repositories
{
    public interface ISqlDbRepository
    {
        Task<int> InsertRecipe(Recipe recipe);

        Task<int> InsertTag(string recipeName, string tag);

        Task DeleteRecipeWithTags(string recipeName);

        Task<IEnumerable<Recipe>> GetAllRecipes();

        Task<IEnumerable<Recipe>> GetRecipesByName(string name);

        Task<IEnumerable<string>> GetNamesByTag(string tag);
    }
}
