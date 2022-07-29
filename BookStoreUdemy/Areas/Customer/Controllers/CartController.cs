using BookStore.DataAccess.Repositories.IRepositories;
using BookStore.Models;
using BookStore.Models.ViewModels;
using BookStore.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

// Make sure you don't add billing portal 

using Stripe.Checkout;
using System.Security.Claims;

namespace BookStoreUdemy.Areas.Customer.Controllers
{
    [Area("Customer")]
    [AutoValidateAntiforgeryToken]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            ShoppingCartVM = new()
            {
                ListCart = await _unitOfWork.ShoppingCart.GetAll(x => x.AppUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            foreach(var item in ShoppingCartVM.ListCart)
            {
                item.Price = GetPriceBasedOnQuantity(item.Count, item.Product!.Price, item.Product.Price50, item.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (item.Count * item.Price);
            }

            return View(ShoppingCartVM);
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = await _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId);
            _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            _unitOfWork.Save();
            var count = _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == cart.AppUserId).GetAwaiter().GetResult().Count();
            HttpContext.Session.SetInt32(SD.SessionCart, count);
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId);
            if(cart!.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
            }
            else
            {
                _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
            }
            
            _unitOfWork.Save();
            var count = _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == cart.AppUserId).GetAwaiter().GetResult().Count();
            HttpContext.Session.SetInt32(SD.SessionCart, count);
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId);
            if(cart != null)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
            }

            _unitOfWork.Save();
            var count = _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == cart.AppUserId).GetAwaiter().GetResult().Count();
            HttpContext.Session.SetInt32(SD.SessionCart, count);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Summary()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = await _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.AppUser = await _unitOfWork.AppUser.GetFirstOrDefault(
                u => u.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader!.AppUser!.FirstName + " " + ShoppingCartVM.OrderHeader.AppUser.FirstName;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.AppUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.AppUser.StreetAddress!;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.AppUser.City!;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.AppUser.State!;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.AppUser.PostalCode!;

            foreach (var item in ShoppingCartVM.ListCart)
            {
                item.Price = GetPriceBasedOnQuantity(item.Count, item.Product!.Price, item.Product.Price50, item.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (item.Count * item.Price);
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            ShoppingCartVM.ListCart = await _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == userId, includeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;

            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.AppUserId = userId;

            foreach (var item in ShoppingCartVM.ListCart)
            {
                item.Price = GetPriceBasedOnQuantity(item.Count, item.Product!.Price, item.Product.Price50, item.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (item.Count * item.Price);
            }

            await _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                OrderDetails orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count,
                    
                };
                await _unitOfWork.OrderDetails.Add(orderDetail);
                _unitOfWork.Save();
            }

            // Stripe settings -----------------------------------------------------------
            string domain = "http://localhost:4758/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + "Customer/Cart/Index",
            };

            foreach (var item in ShoppingCartVM.ListCart)
            {
                var sessionLineItmes = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),                          // we need to multiply by 100
                        Currency = "usd",                                               // currenyc type
                        //Currency = "ind",                                               // currenyc type
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product!.Title,
                        },

                    },
                    Quantity = item.Count,                                              // Based on this it will count overall payment
                };
                options.LineItems.Add(sessionLineItmes);
            }

            var service = new SessionService();
            Session session = service.Create(options);                  //creates final session service

            _unitOfWork.OrderHeader.UpdatePaymentStatus(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);

            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

            //------------------------------------------------------------------------

            //_unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
            //_unitOfWork.Save();
            //return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> OrderConfirmation(int id)
        {   
            OrderHeader? orderHeader = await _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == id, includeProperties: "AppUser");

            var service = new SessionService();
            Session session = service.Get(orderHeader!.SessionId);
            // Check stripe status

            if(session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }

            await _emailSender.SendEmailAsync(orderHeader.AppUser.Email, "New Order - Confirmed", $"<p> New Order Created with Id : {id}, And will be delivered soon to your place.</p> <br> <p>Thank You!</p>");
            List<ShoppingCart> shoppingCart = (await _unitOfWork.ShoppingCart.GetAll(u => u.AppUserId == orderHeader.AppUserId)).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCart);
            HttpContext.Session.Clear();
            _unitOfWork.Save();
            return View(id);
        }

        private decimal GetPriceBasedOnQuantity(double quanity,decimal price, decimal price50, decimal price100)
        {
            if(quanity <= 50)
            {
                return price;
            }
            else if(quanity <= 100){
                return price50;
            }
            else
            {
                return price100;
            }
        }
    }
}
