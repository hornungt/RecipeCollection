using MySql.Data.MySqlClient;
using RecipeApp.Abstract.Models.Db;
using RecipeApp.Abstract.Repositories;
using RecipeApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Repositories
{
    public class SqlDbRepository : ISqlDbRepository
    {
        private readonly IDbContext _db;
        public SqlDbRepository(IDbContext db)
        {
            _db = db;
        }

        public async Task<int> InsertRecipe(Recipe recipe)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand
                {
                    Connection = conn,
                    CommandText = "INSERT INTO recipe_db.recipes (Name, Url, Path) VALUES (@name, @url, @path)"
                };
                cmd.Parameters.AddWithValue("@name", recipe.Name);
                cmd.Parameters.AddWithValue("@url", recipe.Url);
                cmd.Parameters.AddWithValue("@path", recipe.Path);


                return await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<int> InsertTag(string recipeName, string tag)
        {
            if (tag == null)
            {
                throw new ArgumentNullException("tag");
            }
            var val = -1;
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand
                {
                    Connection = conn,
                    CommandText = "INSERT INTO recipe_db.tags (FK_Name, Tag) VALUES (@name, @tag)"
                };
                cmd.Parameters.AddWithValue("@name", recipeName);
                cmd.Parameters.AddWithValue("@tag", tag);
                cmd.CommandText = "INSERT INTO recipe_db.tags (FK_Name, Tag) VALUES (@name, @tag)";
                val = await cmd.ExecuteNonQueryAsync();
                cmd.Parameters.RemoveAt("@tag");
            }
            return val;
        }

        /*
         * Should use a MySqlTransaction, but must also work with the file system for transactions
         * Cannot use a MySqlTransaction inside a TransactionScope -> error is thrown
         * Not sure the best way to work around so that all edits to permanent storage are saved as a unit
         * Currently, do not use this method outside of a TransactionScope
         */
        public async Task DeleteRecipeWithTags(string recipeName)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand
                {
                    Connection = conn
                };
                cmd.Parameters.AddWithValue("@name", recipeName);

                cmd.CommandText = "DELETE FROM recipe_db.tags WHERE FK_Name = @name";
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = "DELETE FROM recipe_db.recipes WHERE Name = @name";
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipes()
        {
            IEnumerable<Recipe> recipes = new List<Recipe>();
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(
                    @"select
                        recipes.name as 'Name',
                        recipes.path as 'Path',
                        recipes.url as 'Url',
                        group_concat(tags.Tag)
                    from
                        recipes,
                        tags
                    where
                        recipes.name=tags.FK_Name
                    group by
                        recipes.name",
                        conn);

                recipes = await ExecuteSqlDbReaderAsync(cmd);
            }
            return recipes;
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByName(string name)
        {
            IEnumerable<Recipe> recipes = null;
            using (MySqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(
                    @"SELECT recipes.name as 'Name',
                        recipes.path as 'Path',
                        recipes.url as 'Url',
                        group_concat(tags.tag)
                    FROM
                        recipes
                    JOIN 
                        tags
                    ON ( recipes.name = tags.fk_name ) 
                    WHERE recipes.name = @name",
                    conn);
                cmd.Parameters.AddWithValue("@name", name);

                recipes = await ExecuteSqlDbReaderAsync(cmd);
            }
            return recipes;
        }

        public async Task<IEnumerable<string>> GetNamesByTag(string tag)
        {
            List<string> names = new List<string>();
            using (MySqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"SELECT FK_Name FROM Tags WHERE Tag = @tag", conn);
                cmd.Parameters.AddWithValue("@tag", tag);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        names.Add(reader["FK_Name"].ToString());
                    }
                }
            }
            return names;
        }

        private async Task<IEnumerable<Recipe>> ExecuteSqlDbReaderAsync(MySqlCommand cmd)
        {
            List<Recipe> recipes = new List<Recipe>();
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (reader.Read())
                {
                    var recipe = new Recipe
                    {
                        Name = reader["Name"].ToString(),
                        Url = reader["Url"].ToString(),
                        Path = reader["Path"].ToString(),
                        Tags = reader["group_concat(tags.tag)"].ToString().Split(",")
                    };
                    // need check for empty recipe -> SQL query returns row of empty string fields for some reason
                    if (!(recipe.Name == "" && recipe.Path == "" && recipe.Url == "" && recipe.Tags.Count() == 1 && recipe.Tags.First() == ""))
                    {
                        recipes.Add(recipe);
                    }
                }
            }
            return recipes;
        }
    }
}
