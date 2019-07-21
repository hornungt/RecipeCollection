using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Abstract.Models.Db
{
    public interface IDbContext
    {
        string ConnectionString { get; set; }
        MySqlConnection GetConnection();
    }
}
