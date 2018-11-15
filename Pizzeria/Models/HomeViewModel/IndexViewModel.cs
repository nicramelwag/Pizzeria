﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pizzeria.Models.HomeViewModel
{
    public class IndexViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Coupons> Coupons { get; set; }

        //public string StatusMessage { get; set; }
    }
}
