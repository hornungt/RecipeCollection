using Microsoft.AspNetCore.Http;
using RecipeApp.Abstract.Managers;
using RecipeApp.Abstract.Repositories;
using RecipeApp.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace RecipeApp.Managers
{
    public class RecipeManager : IRecipeManager
    {
        private readonly IFileSystemRepository _fileRepo;
        private readonly ISqlDbRepository _dbRepo;

        public RecipeManager(IFileSystemRepository fileRepo, ISqlDbRepository dbRepo)
        {
            _fileRepo = fileRepo;
            _dbRepo = dbRepo;
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipes()
        {
            return await _dbRepo.GetAllRecipes();
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByNameOrTag(string query)
        {
            IEnumerable<Recipe> recipes = null;
            var byName = await _dbRepo.GetRecipesByName(query);
            var namesByTag = (await _dbRepo.GetNamesByTag(query)).ToList();
            var byTag = GetRecipesByTags(namesByTag);

            if (byName == null || byName.Count() == 0)
            {
                recipes = byTag;
            }
            else if (namesByTag == null || namesByTag.Count() == 0)
            {
                recipes = byName;
            }
            else
            {
                recipes = byName.Union(byTag);
            }
            return recipes;
        }

        private IEnumerable<Recipe> GetRecipesByTags(IEnumerable<string> namesByTag)
        {
            List<Recipe> recipes = null;
            namesByTag.ToList().ForEach(async name =>
            {
                var result = (await _dbRepo.GetRecipesByName(name)).ToList();
                if (result != null)
                {
                    if (recipes != null)
                    {
                        result.ForEach(recipe => recipes.Add(recipe));
                    }
                    else
                    {
                        recipes = result;
                    }
                }
            });
            return recipes;
        }

        public async Task<Stream> GetFile(string filePath)
        {
            return await _fileRepo.GetFile(filePath);
        }

        public async Task GenerateRecipe(Recipe recipe)
        {
            using (var tx = new TransactionScope())
            {
                if (recipe.Tags.Count() == 0)
                {
                    recipe.Tags = new string[] { "" };
                }
                //create pdf, get file path to pdf, and create sql entry
                var recipeWithPath = _fileRepo.GenerateNewPDF(recipe);
                await _dbRepo.InsertRecipe(recipeWithPath);
                InsertTags(recipeWithPath);

                tx.Complete();
            }
        }

        public async Task DeleteRecipe(string name, string filePath)
        {
            using (var tx = new TransactionScope())
            {
                await _dbRepo.DeleteRecipeWithTags(name);
                _fileRepo.DeleteFile(filePath);

                tx.Complete();
            }
        }

        public async Task OverwriteRecipe(Recipe oldRecipe, Recipe newRecipe)
        {
            using (var txn = new TransactionScope())
            {
                // if newRecipe.Url exists, delete old pdf and generate new pdf
                // otherwise, must be updating a manually loaded recipe without updating to a new url (keeping the original file)

                Recipe newRecipeWithPath;
                if (!string.IsNullOrEmpty(newRecipe.Url))
                {
                    _fileRepo.DeleteFile(oldRecipe.Path);
                    newRecipeWithPath = _fileRepo.GenerateNewPDF(newRecipe);
                }
                else
                {
                    newRecipeWithPath = new Recipe
                    {
                        Name = newRecipe.Name,
                        Path = _fileRepo.RenamePDF(oldRecipe.Path, newRecipe.Name),
                        Url = newRecipe.Url,
                        Tags = newRecipe.Tags
                    };
                }
                if (newRecipeWithPath.Tags.Count() == 0)
                {
                    newRecipeWithPath.Tags = new string[] { "" };
                }

                // need to delete the old db entry and create the new one

                await _dbRepo.DeleteRecipeWithTags(oldRecipe.Name);
                await _dbRepo.InsertRecipe(newRecipeWithPath);
                InsertTags(newRecipeWithPath);

                txn.Complete();
            }
        }

        public async Task WriteExistingRecipe(Recipe recipe, IFormFile file)
        {
            using (var txn = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                recipe.Url = recipe.Url ?? "";
                if (recipe.Tags.Count() == 0)
                {
                    recipe.Tags = new string[] { "" };
                }
                var recipeWithPath = await _fileRepo.SaveExistingPDF(recipe, file);

                await _dbRepo.InsertRecipe(recipeWithPath);
                InsertTags(recipeWithPath);

                txn.Complete();
            }
        }

        private void InsertTags(Recipe recipe)
        {
            recipe.Tags.ToList().ForEach(async tag =>
            {
                await _dbRepo.InsertTag(recipe.Name, tag);
            });
        }
    }
}
