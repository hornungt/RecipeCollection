using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeApp.Abstract.Managers;
using RecipeApp.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RecipeApp.Controllers
{
    [Route("api/recipes")]
    public class RecipeController : Controller
    {
        private readonly IFileSystemManager _fileManager;
        private readonly ISqlDbManager _dbManager;
        public RecipeController(IFileSystemManager fileManager, ISqlDbManager dbManager)
        {
            //need to get injected services in here for making actual db requests
            _fileManager = fileManager;
            _dbManager = dbManager;
        }

        [HttpGet]
        public IEnumerable<Recipe> GetRecipes(string query)
        {
            IEnumerable<Recipe> recipes;
            if (string.IsNullOrWhiteSpace(query))
            {
                //call manager to get all recipes entries from sql db
                recipes = _dbManager.GetAllRecipes();
            }
            else
            {
                var byName = _dbManager.GetRecipesByName(query);
                var byTag = _dbManager.GetRecipesByTag(query);

                if (byName == null || byName.Count() == 0)
                {
                    recipes = byTag;
                }
                else if (byTag == null || byTag.Count() == 0)
                {
                    recipes = byName;
                }
                else
                {
                    recipes = byName.Union(byTag);
                }
            }
            return recipes;
        }

        [HttpGet("{*filePath}")]
        public async Task<IActionResult> GetFile(string filePath)
        {
            var file = await _fileManager.GetFile(filePath);
            return File(file, "application/pdf");
        }

        [HttpPost]
        public async Task<IActionResult> AddRecipe([FromBody] Recipe recipe)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    //create pdf, get file path to pdf, and create sql entry
                    var recipeWithPath = _fileManager.GenerateNewPDF(recipe);
                    await _dbManager.AddRecipe(recipeWithPath, true);

                    tx.Complete();
                }
            }
            catch (TransactionAbortedException e)
            {
                Console.WriteLine(e.Message);
            }
            return Ok();
        }

        [HttpDelete("{name}/{*filePath}")]
        public async Task<IActionResult> DeleteRecipe([FromRoute] string name, [FromRoute] string filePath)
        {
            var temp = (Directory.GetCurrentDirectory() + "\\pdfs\\").Replace("\\", "/");
            if (name == null || filePath == null || !filePath.StartsWith((Directory.GetCurrentDirectory() + "\\pdfs\\").Replace("\\", "/")))
            {
                return BadRequest();
            }
            // transaction so that sql row is deleted and file is deleted from folder structure
            try
            {
                using (var tx = new TransactionScope())
                {
                    _dbManager.DeleteRecipe(name, true);
                    _fileManager.DeleteFile(filePath);

                    tx.Complete();
                }
            }
            catch (TransactionAbortedException e)
            {
                Console.WriteLine(e.Message);
            }
            return Ok();
        }

        //TODO: Test editing
        [HttpPut]
        public async Task<IActionResult> EditRecipe([FromBody] Recipe[] recipes)
        {
            if (recipes.Count() != 2) return BadRequest();

            var oldRecipe = recipes[0];
            var newRecipe = recipes[1];

            try
            {
                using (var txn = new TransactionScope())
                {
                    _dbManager.DeleteRecipe(oldRecipe.Name, true);
                    _fileManager.DeleteFile(oldRecipe.Path);

                    var newRecipeWithPath =_fileManager.GenerateNewPDF(newRecipe);
                    await _dbManager.AddRecipe(newRecipeWithPath, true);

                    txn.Complete();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetType().FullName + ": " + e.Message);
            }

            return Ok();
        }
    }
}
