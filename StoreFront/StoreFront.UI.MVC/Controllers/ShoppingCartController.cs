using StoreFront.DATA.EF.Models;//Added for access to the DB Context
using Microsoft.AspNetCore.Identity;//Added for access to the UserManager
using StoreFront.UI.MVC.Models;//Added for access to the CartItemViewModel
using Newtonsoft.Json;//Added for easier management of the shopping cart

using Microsoft.AspNetCore.Mvc;

namespace StoreFront.UI.MVC.Controllers
{
    public class ShoppingCartController : Controller
    {
        //Fields
        private readonly StoreFrontContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        //Constructor
        public ShoppingCartController(StoreFrontContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var sessionCart = HttpContext.Session.GetString("cart");

            
            Dictionary<int, CartItemViewModel> shoppingCart = null;

            
            if (sessionCart == null || sessionCart.Count() == 0)
            {
                
                shoppingCart = new Dictionary<int, CartItemViewModel>();

                ViewBag.Message = "There are no items in your cart.";
            }
            else
            {
                ViewBag.Message = null;
                shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
            }

            return View(shoppingCart);
        }

        public IActionResult AddToCart(int id)
        {
            Dictionary<int, CartItemViewModel> shoppingCart = null;


            var sessionCart = HttpContext.Session.GetString("cart");

            if (String.IsNullOrEmpty(sessionCart))
            {
                shoppingCart = new Dictionary<int, CartItemViewModel>();
            }
            else
            {
                shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
            }

            

            Product product = _context.Products.Find(id);

            
            CartItemViewModel civm = new CartItemViewModel(1, product);

            
            if (shoppingCart.ContainsKey(product.ProductId)) {
           
                shoppingCart[product.ProductId].Qty++;
            }
            else
            {
                shoppingCart.Add(product.ProductId, civm);
            }

            
            string jsonCart = JsonConvert.SerializeObject(shoppingCart);
            HttpContext.Session.SetString("cart", jsonCart);

            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int id)
        {
            var sessionCart = HttpContext.Session.GetString("cart");
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
            shoppingCart.Remove(id);
            if(shoppingCart.Count == 0) 
            {
                HttpContext.Session.Remove("cart");
            }
            else
            {
                string jsonCart = JsonConvert.SerializeObject(shoppingCart);
                HttpContext.Session.SetString("cart", jsonCart);
            }
            
            return RedirectToAction("Index");
        }


        public IActionResult UpdateCart(int productId, int qty)
        {
            //Retrieve the cart from session storage
            var sessionCart = HttpContext.Session.GetString("cart");

            //Deserialize from JSON to C#
            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            //update the qty for our specific dictionary key
            shoppingCart[productId].Qty = qty;

            //update the JSON string stored in session with the new Qty, then return user to Index action
            string jsonCart = JsonConvert.SerializeObject(shoppingCart);
            HttpContext.Session.SetString("cart", jsonCart);
            return RedirectToAction("Index");

        }


        public async Task<IActionResult> SubmitOrder() 
        {
            string? userId = (await _userManager.GetUserAsync(HttpContext.User))?.Id;
            CustomerDetail cd = _context.CustomerDetails.Find(userId);
            Order o = new Order()
            {
                CustomerId = userId,
                OrderDate = DateTime.Now,
                ShipToName = cd.FirstName + " " + cd.LastName,
                ShipCity = cd.City,
                ShipState = cd.State,
                ShipZip = cd.Zip
            };

            _context.Orders.Add(o);

            var sessionCart = HttpContext.Session.GetString("cart");

            Dictionary<int, CartItemViewModel> shoppingCart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

            foreach (var item in shoppingCart)
            {
                OrderProduct op = new OrderProduct()
                {
                    OrderId = o.OrderId,
                    ProductId = item.Key,
                    ProductPrice = item.Value.Product.ProductPrice,
                    Quantity = (short)item.Value.Qty
                };

                o.OrderProducts.Add(op);

            }

            //Commit our changes to the DB
            _context.SaveChanges();
            return RedirectToAction("Index", "Orders");
        }
    }
}