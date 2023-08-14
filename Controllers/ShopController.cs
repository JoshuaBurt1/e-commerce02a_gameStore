using Microsoft.AspNetCore.Mvc;
using Mage.Data;
using Mage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Microsoft.AspNetCore.Http;

namespace MajorGamer.Controllers
{
    public class ShopController : Controller
    {
        //Property for our database connection
        private ApplicationDbContext _context;
        private IConfiguration _configuration;

        //Constructor
        public ShopController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.OrderBy(category => category.Name).ToListAsync();
            return View(categories);
        }
        public async Task<IActionResult> Details(int? id)
        {
            var categoryWithGames = await _context.Categories.Include(category => category.Games).FirstOrDefaultAsync(category => category.Id == id);
            return View(categoryWithGames);
        }
        public async Task<IActionResult> GameDetails(int? id)
        {
            var game = await _context.Games.FirstOrDefaultAsync(game => game.Id == id);
            return View(game);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(int gameId, int quantity)
        {
            //Get our logged in user
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //Attempt to get a cart
            var cart = await _context.Carts.FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);
            //Check that an active cart does not exist
            if (cart == null)
            {
                cart = new Cart { UserId = userId};
                await _context.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            //Find our product
            var game = await _context.Games.FirstOrDefaultAsync(game => game.Id == gameId);
            //if no product found
            if(game == null)
            {
                return NotFound();
            }

            //Create a new cart item
            var cartItem = new CartItem
            {
                Cart = cart,
                Game = game,
                Quantity = quantity,
                Price = (decimal)game.Price,
            };

            if (ModelState.IsValid)
            {
                await _context.AddAsync(cartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction("ViewMyCart");
            }

            return NotFound();
        }

        [Authorize]
        public async Task<IActionResult> ViewMyCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cart = await _context.Carts.Include(cart => cart.User).Include(cart => cart.CartItems).ThenInclude(cartItem => cartItem.Game).FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);
            
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteCartItem(int cartItemId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cart = await _context.Carts.FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);

            if (cart == null) return NotFound();

            var cartItem = await _context.CartItems.Include(cartItem => cartItem.Game).FirstOrDefaultAsync(cartItem => cartItem.Cart == cart && cartItem.Id == cartItemId);
            
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                return RedirectToAction("ViewMyCart");
            }
            return NotFound();
        }

        //Checkout screen, do not modify
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cart = await _context.Carts.Include(cart => cart.User).Include(cart => cart.CartItems).ThenInclude(cartItem => cartItem.Game).FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);
        
            var order = new Order
            {
                UserId = userId,
                Cart = cart,
                Total = ((decimal)(cart.CartItems.Sum(cartItem => (cartItem.Price * cartItem.Quantity)))),
                ShippingAddress = "",
                PaymentMethod = PaymentMethods.VISA,
            };
            ViewData["PaymentMethods"] = new SelectList(Enum.GetValues(typeof(PaymentMethods)));
            ViewData["ShippingCost"] = new SelectList(Enum.GetValues(typeof(ShippingCost)));

            return View(order);
        }

        //Stripe Payment screen, modify for shipping
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Payment(ShippingCost shippingCost, string shippingAddress, PaymentMethods paymentMethod)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cart = await _context.Carts.Include(cart => cart.CartItems).FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);
            if (cart == null) return NotFound();
            //Add order data to the session
            HttpContext.Session.SetString("ShippingAddress", shippingAddress);
            HttpContext.Session.SetString("ShippingCost", shippingCost.ToString());
            HttpContext.Session.SetString("PaymentMethod", paymentMethod.ToString());
            var shippingCostNum = 0;
            if (shippingCost.ToString() == "Standard")
            {
                shippingCostNum = 10;
            }
            if (shippingCost.ToString() == "Expedited")
            {
                shippingCostNum = 20;
            }
            if (shippingCost.ToString() == "SameDay")
            {
                shippingCostNum = 30;
            }
            if (shippingCost.ToString() == "International")
            {
                shippingCostNum = 40;
            }

            //Set Stripe API key
            StripeConfiguration.ApiKey = _configuration.GetSection("Stripe")["SecretKey"];
            //Create our Stripe options
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)((cart.CartItems.Sum(cartItem => cartItem.Quantity * cartItem.Price)+shippingCostNum) * 100 ),
                            Currency = "cad",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "MajorGamer Purchase",
                            },
                        },
                        Quantity = 1,
                    },
                },
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                Mode = "payment",
                SuccessUrl = "https://" + Request.Host + "/Shop/SaveOrder",
                CancelUrl = "https://" + Request.Host + "/Shop/ViewMyCart",
            };
            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        public async Task<IActionResult> SaveOrder(ShippingCost shippingCost)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cart = await _context.Carts.Include(cart => cart.CartItems).FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);
            //Get our data out of the session
            var paymentMethod = HttpContext.Session.GetString("PaymentMethod");
            var shippingAddress = HttpContext.Session.GetString("ShippingAddress");
            var shipCost = HttpContext.Session.GetString("ShippingCost");
            var shippingCostNum = 0;
            if (shipCost.ToString() == "Standard")
            {
                shippingCostNum = 10;
            }
            if (shipCost.ToString() == "Expedited")
            {
                shippingCostNum = 20;
            }
            if (shipCost.ToString() == "SameDay")
            {
                shippingCostNum = 30;
            }
            if (shipCost.ToString() == "International")
            {
                shippingCostNum = 40;
            }

            ////////////////////////////////////

            var order = new Order
            {
                UserId = userId,
                Cart = cart,
                Total = cart.CartItems.Sum(cartItem => cartItem.Quantity * cartItem.Price)+ shippingCostNum,
                ShippingAddress = shippingAddress,
                ShippingCost = (ShippingCost)Enum.Parse(typeof(ShippingCost), shipCost),
                PaymentMethod = (PaymentMethods)Enum.Parse(typeof(PaymentMethods), paymentMethod),
                PaymentReceived = true
            };
            await _context.AddAsync(order);
            await _context.SaveChangesAsync();

            cart.Active = false;
            _context.Update(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction("OrderDetails", new { id = order.Id });
        }

        [Authorize]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var order = await _context.Orders.Include(order => order.User).Include(order => order.Cart).ThenInclude(cart => cart.CartItems).ThenInclude(cartItem => cartItem.Game).FirstOrDefaultAsync(order => order.UserId == userId && order.Id == id);

            if (order == null) return NotFound();
            return View(order);
        }

        [Authorize]
        public async Task<IActionResult> Orders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var orders = await _context.Orders.OrderByDescending(order => order.Id).Where(order => order.UserId == userId).ToListAsync();

            if (orders == null) return NotFound();
            return View(orders);
        }
    }
}
