using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Data;
using Pizzeria.Models;
using Pizzeria.Models.ProductViewModels;
using Pizzeria.Utility;

namespace Tangy.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;

        [BindProperty]
        public ProductViewModel ProductVM { get; set; }


        public ProductsController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            ProductVM = new ProductViewModel()
            {
                Categories = _db.Category.ToList(),
                Product = new Product()
            };
        }


        //GET : Products
        public async Task<IActionResult> Index()
        {
            var menuItems = _db.Products.Include(m => m.Category);
            return View(await menuItems.ToListAsync());
        }

        //GET : Product Create
        public IActionResult Create()
        {
            return View(ProductVM);
        }

        //POST : Product Create
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if(!ModelState.IsValid)
            {
                return View(ProductVM);
            }

            _db.Products.Add(ProductVM.Product);
            await _db.SaveChangesAsync();

            //Image Being Saved
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var productFromDb = _db.Products.Find(ProductVM.Product.Id);

            if(files[0]!=null && files[0].Length>0)
            {
                //when user uploads an image
                var uploads = Path.Combine(webRootPath, "images");
                var extension = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));

                using (var filestream = new FileStream(Path.Combine(uploads, ProductVM.Product.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                productFromDb.Image = @"\images\" + ProductVM.Product.Id + extension;
            }
            else
            {
                //when user does not upload image
                var uploads = Path.Combine(webRootPath, @"images\"+ SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + ProductVM.Product.Id + ".png");
                productFromDb.Image= @"\images\" + ProductVM.Product.Id + ".png";
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        //GET : Edit Product
        public async Task<IActionResult> Edit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            ProductVM.Product = await _db.Products.Include(m => m.Category).SingleOrDefaultAsync(m => m.Id == id);

            if(ProductVM.Product == null)
            {
                return NotFound();
            }

            return View(ProductVM);
        }

        //POST : Edit MenuItems
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {

            if (id!= ProductVM.Product.Id)
            {
                return NotFound();
            }

            if(ModelState.IsValid)
            {
                try
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;
                    var menuItemFromDb = _db.Products.Where(m => m.Id == ProductVM.Product.Id).FirstOrDefault();

                    if(files[0].Length>0 && files[0]!=null)
                    {
                        //if user uploads a new image
                        var uploads = Path.Combine(webRootPath, "images");

                        var extension_New = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));

                        var extension_Old = menuItemFromDb.Image.Substring(menuItemFromDb.Image.LastIndexOf("."), menuItemFromDb.Image.Length - menuItemFromDb.Image.LastIndexOf("."));

                        if(System.IO.File.Exists(Path.Combine(uploads,ProductVM.Product.Id+extension_Old)))
                        {
                            System.IO.File.Delete(Path.Combine(uploads, ProductVM.Product.Id + extension_Old));
                        }
                        using (var filestream = new FileStream(Path.Combine(uploads, ProductVM.Product.Id + extension_New), FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }
                        ProductVM.Product.Image = @"\images\" + ProductVM.Product.Id + extension_New;
                    }

                    if(ProductVM.Product.Image !=null)
                    {
                        menuItemFromDb.Image = ProductVM.Product.Image;
                    }
                    menuItemFromDb.Name = ProductVM.Product.Name;
                    menuItemFromDb.Description = ProductVM.Product.Description;
                    menuItemFromDb.Price = ProductVM.Product.Price;
                    menuItemFromDb.CategoryId = ProductVM.Product.CategoryId;
                    await _db.SaveChangesAsync();
                }
                catch(Exception)
                {

                }
                return RedirectToAction(nameof(Index));
            }
            return View(ProductVM); 

        }

        //GET : Details Product
        public async Task<IActionResult> Details(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            ProductVM.Product = await _db.Products.Include(m => m.Category).SingleOrDefaultAsync(m => m.Id == id);

            if(ProductVM.Product == null)
            {
                return NotFound();
            }

            return View(ProductVM);
        }

        //GET : Delete Product
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductVM.Product = await _db.Products.Include(m => m.Category).SingleOrDefaultAsync(m => m.Id == id);

            if (ProductVM.Product == null)
            {
                return NotFound();
            }

            return View(ProductVM);
        }

        //POST Delete Product
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            Product product = await _db.Products.FindAsync(id);

            if(product!=null)
            {
                var uploads = Path.Combine(webRootPath, "images");
                var extension = product.Image.Substring(product.Image.LastIndexOf("."), product.Image.Length - product.Image.LastIndexOf("."));

                var imagePath = Path.Combine(uploads, product.Id + extension);
                if(System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }


    }
}