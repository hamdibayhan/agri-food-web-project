using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriFood.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AgriFood.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> FarmerOrders(string FarmerId)
        {
            if (FarmerId != UserId())
            {
                return NotFound();
            }

            var applicationDbContext = _context.Orders
                                               .Include(o => o.Customer)
                                               .Include(o => o.Product)
                                               .Where(o => o.Product.FarmerId == FarmerId)
                                               .OrderByDescending(o => o.OrderDate);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Farmer, Customer")]
        public async Task<IActionResult> CustomerOrders(string CustomerId)
        {
            if (CustomerId != UserId())
            {
                return NotFound();
            }

            var applicationDbContext = _context.Orders
                                               .Include(o => o.Product)
                                               .Where(o => o.CustomerId == CustomerId)
                                               .OrderByDescending(o => o.OrderDate);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Farmer, Customer")]
        public IActionResult Create()
        {
            var model = new Order { };
            return PartialView("_OrderModalPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Farmer, Customer")]
        public async Task<IActionResult> Create([Bind("ID,Address,Amount,OrderDate,Price,CustomerId,ProductId")] Order order)
        {

            if(order.Amount == 0)
            {
                ModelState.AddModelError("Amount", "You cannot give order as 0 amount");
            }

            var product =  await _context.Products.FindAsync(order.ProductId);
            var diff_amount = product.Amount - order.Amount;

            if(diff_amount < 0)
            {
                ModelState.AddModelError("Amount", "You cannot take this product because product not enought for your order amount");
            }
            
            if (ModelState.IsValid)
            {
                order.OrderDate = DateTime.Now;
                order.Price = product.Price * order.Amount;
                order.CustomerId = UserId();
                _context.Add(order);

                product.Amount = diff_amount;
                _context.Update(product);

                await _context.SaveChangesAsync();
                return PartialView("_OrderModalPartial", order);
            }

            ViewBag.ProductPrice = product.Price;
            ViewBag.ProductId = order.ProductId;
            
            return PartialView("_OrderModalPartial", order);
        }

        private string UserId()
        {
            return _userManager.GetUserId(HttpContext.User);
        }
    }
}