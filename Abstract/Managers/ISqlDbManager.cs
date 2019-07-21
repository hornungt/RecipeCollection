
using RecipeApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeApp.Abstract.Managers
{
    public interface ISqlDbManager
    {
        IEnumerable<Recipe> GetRecipesByName(string name);
        IEnumerable<Recipe> GetRecipesByTag(string tag);
        IEnumerable<Recipe> GetAllRecipes();
        Task AddRecipe(Recipe recipe, bool inTxn);
        void DeleteRecipe(string recipeName, bool inTxn);
        
    }
}
