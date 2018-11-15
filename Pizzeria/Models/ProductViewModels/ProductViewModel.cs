using Pizzeria.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pizzeria.Models.ProductViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
