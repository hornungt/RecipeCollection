using MySql.Data.MySqlClient;
using RecipeApp.Abstract.Models.Db;

namespace RecipeApp.Models.Db
{
    public class DbContext : IDbContext
    {
        public string ConnectionString { get; set; }

        public DbContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
