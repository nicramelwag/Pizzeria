using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Data;
using Pizzeria.Models.HomeViewModel;

namespace Pizzeria.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                Products = await _db.Products.Include(m => m.Category).ToListAsync(),
                Categories = _db.Category.OrderBy(c => c.DisplayOrder),
                //Coupons = _db.Coupons.Where(c => c.isActive == true).ToList()
            };
            return View(IndexVM);
        }
    }
}