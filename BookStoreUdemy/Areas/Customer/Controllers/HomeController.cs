using BookStore.DataAccess.Repositories.IRepositories;
using BookStore.Models;
using BookStore.Models.ViewModels;
using BookStore.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BookStoreUdemy.Areas.Customer.Controllers
{
    [Area("Customer")]
    [AutoValidateAntiforgeryToken]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> products = await _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType"); 
            return View(products);
        }

        [Route("Customer/Home/Details/{productId}")]
        [HttpGet]
        public async Task<IActionResult> Details(int productId)                        // here id - automatically binding to model, so changed to productId
        {
            ShoppingCart cart = new()
            {
                Product = await _unitOfWork.Product.GetFirstOrDefault(x => x.Id == productId, includeProperties: "Category,CoverType"),
                ProductId = productId,
                Count = 1
            };

            return View(cart);

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart cart)
        {
            // Only authrized user will be able to access this, so value will never be nulls
            // var claimsIdentity = (ClaimsIdentity)User.Identity!;
            // var claimId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            // cart.AppUserId = claimId!.Value;

            // cart.AppUserId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            string userID = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            cart.AppUserId = userID;

            ShoppingCart? cartFromDb = await _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.AppUserId == cart.AppUserId && x.ProductId == cart.ProductId);

            if (cartFromDb == null)
            {
                await _unitOfWork.ShoppingCart.Add(cart);
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == userID).GetAwaiter().GetResult().Count());
            }
            else
            {
                cartFromDb.Count = _unitOfWork.ShoppingCart.IncrementCount(cartFromDb, cart.Count);
            }
            _unitOfWork.Save();

            TempData["success"] = @"Product added to cart successfully, Total "+ cart.Count +  " products";
            return RedirectToAction(nameof(Index));
            // return RedirectToAction("Index");
            // return RedirectToAction("MethodName", "ControllerName", "RouteParameters");              Syntax for RedirectToAction mehtod

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}