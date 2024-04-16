using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreFront.DATA.EF.Models;
using System.Data;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Authorization;
using GadgetStore.UI.MVC.Utilities;
using System.Drawing;
using X.PagedList;
using X.PagedList.Mvc.Core;

namespace StoreFront.UI.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly StoreFrontContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(StoreFrontContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        // GET: Products
        public async Task<IActionResult> Index()
        {
            //var storeFrontContext = _context.Products.Include(p => p.Category).Include(p => p.ProductStatus);
            var products =
                _context.Products.Where(p => p.UnitsInStock > 0 && p.ProductPrice > 3)//SELECT * FROM Products WHERE UnitsInStock > 0 and Price > $3
                .Include(p => p.Category)//similar to a JOIN on the Category table
                .Include(p => p.OrderProducts);//similar to a JOIN on the OrderProducts table



            return View(await products.ToListAsync());
        }




        [AllowAnonymous]
        public async Task<IActionResult> TiledProducts(string searchTerm, int categoryId = 0, int page = 1)
        {
            int pageSize = 6;

            var products =
               _context.Products.Where(p => p.UnitsInStock > 0)//SELECT * FROM Products WHERE Discontinued != true
               .Include(p => p.Category)//similar to a JOIN on the Category table
               .Include(p => p.OrderProducts).ToList();//similar to a JOIN on the OrderProducts table

            #region Optional Category Filter

            //Create a VIewData object to send a list of categories to the view
            //This is similar to what gets scaffolded in Products/Create()
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewBag.Category = 0;

            //At this point we need to add int categoryId as a parameter to the TiledProducts() Action
            if (categoryId != 0)
            {
                products = products.Where(p => p.CategoryId == categoryId).ToList();
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", categoryId);
                ViewBag.Category = categoryId;



            }


            #endregion

            #region Optional Search Filter

            if (!String.IsNullOrEmpty(searchTerm))
            {
                //In these scopes, there *is* a search term
                products = products.Where(p =>
                p.ProductName.ToLower().Contains(searchTerm.ToLower()) ||
                p.Category.CategoryName.ToLower().Contains(searchTerm.ToLower()) ||
                p.ProductDescript.ToLower().Contains(searchTerm.ToLower())).ToList();

                //ViewBag for total # of results
                ViewBag.NbrResults = products.Count;
                //ViewBag to repeat the user's search term back to them
                ViewBag.SearchTerm = searchTerm;
            }
            else
            {
                ViewBag.NbrResults = null;
                ViewBag.SearchTerm = null;
            }

            #endregion

            return View(products.ToPagedList(page, pageSize));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Discontinued()
        {
            var products = _context.Products.Where(p => p.UnitsInStock == 0)
                .Include(p => p.Category)
                .Include(p => p.OrderProducts);

            return View(await products.ToListAsync());
            //To create the view, right-click the Action -> View (Choose List Template)
        }




        [AllowAnonymous]
        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductStatus)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["ProductStatusId"] = new SelectList(_context.ProductStatuses, "ProductStatusId", "ProductStatusName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductPrice,ProductDescript,UnitsInStock,CategoryId,ProductStatusId,ProductImage,Image")] Product product)
        {
            if (ModelState.IsValid)
            {

                if (product.Image != null)
                {
                    string ext = Path.GetExtension(product.Image.FileName);

                    string[] validExts = { ".jpg", ".jpeg", ".gif", ".png" };

                    if (validExts.Contains(ext.ToLower()) && product.Image.Length < 4_194_303)
                    {
               
                        product.ProductImage = Guid.NewGuid() + ext;

                        string webRootPath = _webHostEnvironment.WebRootPath;
                       
                        string fullImagePath = webRootPath + "/images/";

                       
                        using (var memoryStream = new MemoryStream())
                        {
                            await product.Image.CopyToAsync(memoryStream);
                            using (var img = Image.FromStream(memoryStream))
                            {
                               
                                int maxImageSize = 500;
                                int maxThumbSize = 100;

                            ImageUtility.ResizeImage(fullImagePath, product.ProductImage, img, maxImageSize, maxThumbSize);
                                
                            }
                        }
                    }
                }
                else
                {
                    product.ProductImage = "noimage.png";
                }












                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["ProductStatusId"] = new SelectList(_context.ProductStatuses, "ProductStatusId", "ProductStatusName", product.ProductStatusId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["ProductStatusId"] = new SelectList(_context.ProductStatuses, "ProductStatusId", "ProductStatusName", product.ProductStatusId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductPrice,ProductDescript,UnitsInStock,CategoryId,ProductStatusId,ProductImage,Image")] Product product)
        {
            #region File Upload - EDIT

           
            string oldImageName = product.ProductImage;

  
            if (product.Image != null)
            {
                
                string ext = Path.GetExtension(product.Image.FileName);

                string[] validExts = { ".jpg", ".jpeg", ".gif", ".png" };

            
                if (validExts.Contains(ext.ToLower()) && product.Image.Length < 4_194_303)
                {
              
                    product.ProductImage = Guid.NewGuid() + ext;
             
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string fullPath = webRootPath + "/images/";

                
                    if (oldImageName != "noimage.png")
                    {
                        ImageUtility.Delete(fullPath, oldImageName);
                    }

               
                    using (var memoryStream = new MemoryStream())
                    {
                        await product.Image.CopyToAsync(memoryStream);
                        using (var img = Image.FromStream(memoryStream))
                        {
                            int maxImageSize = 500;
                            int maxThumbSize = 100;
                            ImageUtility.ResizeImage(fullPath, product.ProductImage, img, maxImageSize, maxThumbSize);
                        }
                    }
                }
            }
            #endregion

            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["ProductStatusId"] = new SelectList(_context.ProductStatuses, "ProductStatusId", "ProductStatusName", product.ProductStatusId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductStatus)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'StoreFrontContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
