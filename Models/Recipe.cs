using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Models
{
    public class Recipe
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public IEnumerable<string> Tags { get; set; }

        public string Url { get; set; }
    }
}
