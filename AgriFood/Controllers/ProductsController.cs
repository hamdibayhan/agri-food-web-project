using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriFood.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;
using AgriFood.ViewModels;

namespace AgriFood.Controllers
{
    [Authorize(Roles = "Farmer")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id, string FarmerId)
        {
            if (id == null & FarmerId != UserId())
            {
                return NotFound();
            }

            ViewBag.IsProductOwn = IsProductOwn(FarmerId);
            var product = await _context.Products
                .Include(p => p.Farmer).Where(i => i.FarmerId == FarmerId)
                .FirstOrDefaultAsync(m => m.ID == id);
            return product == null ? NotFound() : (IActionResult)View(product);
        }
        
        public IActionResult Create(string FarmerId)
        {
            return FarmerId != UserId() ? NotFound() : (IActionResult)View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,Description,IsActive,Amount,Image,Price,FarmerId")] ProductCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    FarmerId = UserId(),
                    CreatedDate = DateTime.Now,
                    Title = model.Title,
                    Description = model.Description,
                    IsActive = model.IsActive,
                    Amount = model.Amount,
                    Price = model.Price
                };
                if (model.Image != null && model.Image.Length > 0)
                { 
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.Image.CopyToAsync(memoryStream);
                        product.Image = memoryStream.ToArray();
                    }
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = product.ID });
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id, string FarmerId)
        {
            if (id == null & FarmerId != UserId())
            {
                return NotFound();
            }

            ProductCreateEditViewModel model;

            var product = await _context.Products
                                        .Where(i => i.FarmerId == UserId())
                                        .FirstOrDefaultAsync(m => m.ID == id);

            if (product == null)
            {
                return NotFound();
            }

            model = new ProductCreateEditViewModel
            {
                ID = product.ID,
                Title = product.Title,
                Description = product.Description,
                IsActive = product.IsActive,
                Amount = product.Amount,
                Price = product.Price
            };

            ViewBag.Image = product.Image;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Description,IsActive,Amount,Image,Price,FarmerId")] ProductCreateEditViewModel model)
        {
            if (id != model.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var product = await _context.Products
                                    .Where(i => i.FarmerId == UserId())
                                    .FirstOrDefaultAsync(m => m.ID == id);
                try
                {
                    product.FarmerId = UserId();
                    product.ID = model.ID;
                    product.Title = model.Title;
                    product.Description = model.Description;
                    product.IsActive = model.IsActive;
                    product.Amount = model.Amount;
                    product.Price = model.Price;

                    if (model.Image != null && model.Image.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await model.Image.CopyToAsync(memoryStream);
                            product.Image = memoryStream.ToArray();
                        }
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(model.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = product.ID });
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id, string FarmerId)
        {
            if (id == null & FarmerId != UserId())
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Farmer).Where(i => i.FarmerId == UserId())
                .FirstOrDefaultAsync(m => m.ID == id);

            return product == null ? NotFound() : (IActionResult)View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products
                                        .Where(i => i.FarmerId == UserId())
                                        .FirstOrDefaultAsync(m => m.ID == id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Farmer", new { id = UserId() });
            
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ID == id);
        }

        private string UserId()
        {
            return _userManager.GetUserId(HttpContext.User);
        }

        private Boolean IsProductOwn(string id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            return userId == id ? true : false;
        }
    }
}
