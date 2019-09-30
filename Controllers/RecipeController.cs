using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecipeApp.Abstract.Managers;
using RecipeApp.Models;
using Newtonsoft.Json;

namespace RecipeApp.Controllers
{
    [Route("api/recipes")]
    public class RecipeController : Controller
    {
        private readonly IRecipeManager _recipeManager;
        public RecipeController(IRecipeManager recipeManager)
        {
            _recipeManager = recipeManager;
        }

        [HttpGet]
        public async Task<IEnumerable<Recipe>> GetRecipes(string query)
        {
            // search for all recipes when no query is specified
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return await _recipeManager.GetAllRecipes();
                }
                else
                {
                    return await _recipeManager.GetRecipesByNameOrTag(query);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return new List<Recipe>();
        }

        [HttpGet("{*filePath}")]
        public async Task<IActionResult> GetFile(string filePath)
        {
            try
            {
                var file = await _recipeManager.GetFile(filePath);
                return File(file, "application/pdf");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return BadRequest();
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> AddRecipe([FromBody] Recipe recipe)
        {
            if (string.IsNullOrEmpty(recipe.Name))
            {
                return BadRequest();
            }
            try
            {
                await _recipeManager.GenerateRecipe(recipe);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Ok();
        }

        [HttpDelete("{name}/{*filePath}")]
        public async Task<IActionResult> DeleteRecipe([FromRoute] string name, [FromRoute] string filePath)
        {
            if (name == null || filePath == null || !filePath.StartsWith((Directory.GetCurrentDirectory() + "\\pdfs\\").Replace("\\", "/")))
            {
                return BadRequest();
            }
            // transaction so that sql row is deleted and file is deleted from folder structure
            try
            {
                await _recipeManager.DeleteRecipe(name, filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> EditRecipe([FromBody] Recipe[] recipes)
        {
            if (recipes.Count() != 2) return BadRequest();

            var oldRecipe = recipes[0];
            var newRecipe = recipes[1];

            // can't have an empty new url if the old url is not empty
            if (string.IsNullOrEmpty(newRecipe.Url) && !string.IsNullOrEmpty(oldRecipe.Url))
            {
                return BadRequest();
            }

            try
            {
                await _recipeManager.OverwriteRecipe(oldRecipe, newRecipe);
            }
            catch (Exception e)
            {
                Console.WriteLine( e.Message);
            }

            return Ok();
        }

        [HttpPost("upload-local"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadLocalRecipe()
        {
            try
            {
                var file = Response.HttpContext.Request.Form.Files[0];
                var recipeJson = Response.HttpContext.Request.Form.Single((kvp) => kvp.Key.Equals("recipe")).Value;
                var recipe = JsonConvert.DeserializeObject(recipeJson, typeof(Recipe)) as Recipe;

                await _recipeManager.WriteExistingRecipe(recipe, file);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetType() + ": invalid recipe upload request");
            }

            return Ok();
        }
    }
}
