using MySql.Data.MySqlClient;
using RecipeApp.Abstract.Managers;
using RecipeApp.Abstract.Models.Db;
using RecipeApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Managers
{
    public class SqlDbManager : ISqlDbManager
    {
        private readonly IDbContext _db;
        public SqlDbManager(IDbContext db)
        {
            _db = db;
        }
        public async Task AddRecipe(Recipe recipe, bool inTxn)
        {
            async Task ExecuteTags(MySqlParameter tagParam, IEnumerable<string> tags, MySqlCommand cmd)
            {
                foreach (string tag in tags)
                {
                    tagParam.Value = tag;
                    cmd.Parameters.Add(tagParam);
                    cmd.CommandText = $"INSERT INTO recipe_db.tags (FK_Name, Tag) VALUES (@name, @tag)";
                    await cmd.ExecuteNonQueryAsync();
                    cmd.Parameters.Remove(tagParam);
                }
            }
            // need to give the recipe at least one tag so that the recipe shows up in the GetAllRecipes() query
            if (recipe.Tags == null || recipe.Tags.Count() == 0)
            {
                recipe.Tags = new string[] { "" };
            }

            try
            {
                using (MySqlConnection conn = _db.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand
                    {
                        Connection = conn,
                        CommandText = "INSERT INTO recipe_db.recipes (Name, Url, Path) VALUES (@name, @url, @path)"
                    };
                    cmd.Parameters.AddWithValue("@name", recipe.Name);
                    cmd.Parameters.AddWithValue("@url", recipe.Url);
                    cmd.Parameters.AddWithValue("@path", recipe.Path);

                    MySqlParameter tagParam = new MySqlParameter
                    {
                        ParameterName = "@tag"
                    };

                    if (inTxn)
                    {
                        await cmd.ExecuteNonQueryAsync();
                        await ExecuteTags(tagParam, recipe.Tags, cmd);
                    }
                    else
                    {
                        using (var txn = await conn.BeginTransactionAsync())
                        {
                            cmd.Transaction = txn;

                            await cmd.ExecuteNonQueryAsync();
                            await ExecuteTags(tagParam, recipe.Tags, cmd);

                            txn.Commit();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetType().FullName + ": " + e.Message);
            }
        }

        public void DeleteRecipe(string recipeName, bool inTxn)
        {
            void ExecuteDelete(MySqlCommand cmd)
            {
                cmd.Parameters.AddWithValue("@name", recipeName);

                cmd.CommandText = "DELETE FROM recipe_db.tags WHERE FK_Name = @name";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM recipe_db.recipes WHERE Name = @name";
                cmd.ExecuteNonQuery();
            }
            try
            {
                using (MySqlConnection conn = _db.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand
                    {
                        Connection = conn
                    };

                    if (inTxn)
                    {
                        ExecuteDelete(cmd);
                    }
                    else
                    {
                        using (var txn = conn.BeginTransaction())
                        {
                            cmd.Transaction = txn;
                            ExecuteDelete(cmd);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IEnumerable<Recipe> GetAllRecipes()
        {
            List<Recipe> recipes = new List<Recipe>();
            try
            {
                using (MySqlConnection conn = _db.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(
                        @"select
                        recipes.name as 'Name',
                        recipes.path as 'Path',
                        recipes.url as 'Url',
                        GROUP_CONCAT(tags.Tag) as `Tags`
                    from
                        recipes,
                        tags
                    where
                        recipes.name=tags.FK_Name
                    group by
                        recipes.name",
                            conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            recipes.Add(new Recipe
                            {
                                Name = reader["Name"].ToString(),
                                Url = reader["Url"].ToString(),
                                Path = reader["Path"].ToString(),
                                Tags = reader["Tags"].ToString().Split(",")
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetType().FullName + ": " + e.Message);
            }
            return recipes;
        }

        public IEnumerable<Recipe> GetRecipesByName(string name)
        {
            List<Recipe> recipes = new List<Recipe>();
            try
            {
                using (MySqlConnection conn = _db.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand($@"SELECT recipes.name, recipes.path, recipes.url, group_concat(tags.tag)
                    FROM recipes JOIN tags ON ( recipes.name = tags.fk_name ) WHERE recipes.name = @name", conn);
                    cmd.Parameters.AddWithValue("@name", name);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            recipes.Add(new Recipe
                            {
                                Name = reader["Name"].ToString(),
                                Url = reader["Url"].ToString(),
                                Path = reader["Path"].ToString(),
                                Tags = reader["group_concat(tags.tag)"].ToString().Split(",")
                            });
                        }
                    }
                }
                if (recipes.Count == 0 || string.IsNullOrEmpty(recipes[0].Name))
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetType().FullName + ": " + e.Message);
            }
            return recipes;
        }

        public IEnumerable<Recipe> GetRecipesByTag(string tag)
        {
            List<string> names = new List<string>();
            List<Recipe> recipes = new List<Recipe>();
            try
            {
                using (MySqlConnection conn = _db.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand($"SELECT FK_Name FROM Tags WHERE Tag = @tag", conn);
                    cmd.Parameters.AddWithValue("@tag", tag);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            names.Add(reader["FK_Name"].ToString());
                        }
                    }
                }
                if (names.Count == 0)
                {
                    return null;
                }
                names.ForEach(name =>
                {
                    var result = GetRecipesByName(name);
                    if (result != null)
                    {
                        if (recipes != null)
                        {
                            result.ToList().ForEach(recipe => recipes.Add(recipe));
                        }
                        else
                        {
                            recipes = result.ToList();
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetType().FullName + ": " + e.Message);
            }
            return recipes;
        }
    }
}
