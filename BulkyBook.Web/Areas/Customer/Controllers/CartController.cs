using BulkyBook.DataAccess.IRepositories;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyBook.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{
	private readonly IUnitOfWork _unitOfWork;
	//[BindProperty]
	public ShoppingCartViewModel _shoppingCartVM { get; set; }
	public int _orderTotal { get; set; }

	public CartController(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public IActionResult Index()
	{
		var claimsIdentity = (ClaimsIdentity)User.Identity;
		var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

		_shoppingCartVM = new ShoppingCartViewModel()
		{
			ListCart = _unitOfWork.ShoppingCart.GetAll(
				u => u.ApplicationUserId == claim.Value,
				includeProperties: new string[] { nameof(Product) }),
			OrderHeader = new()
		};
		foreach(var cartItem in _shoppingCartVM.ListCart)
		{
			cartItem.Price = GetPriceBasedOnQuantity(cartItem.Count, cartItem.Product.Price, cartItem.Product.Price50, cartItem.Product.Price100);
			_shoppingCartVM.OrderHeader.OrderTotal += cartItem.Price * cartItem.Count;
		}

		return View(_shoppingCartVM);
	}

	public IActionResult Summary()
	{
		var claimsIdentity = (ClaimsIdentity)User.Identity;
		var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

		_shoppingCartVM = new ShoppingCartViewModel()
		{
			ListCart = _unitOfWork.ShoppingCart.GetAll(
				u => u.ApplicationUserId == claim.Value,
				includeProperties: new string[] { nameof(Product) }),
			OrderHeader = new()
		};

		_shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);
		_shoppingCartVM.OrderHeader.Name = _shoppingCartVM.OrderHeader.ApplicationUser.Name;
		_shoppingCartVM.OrderHeader.PhoneNumber = _shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
		_shoppingCartVM.OrderHeader.StreetAdresse = _shoppingCartVM.OrderHeader.ApplicationUser.StreetAdresse;
		_shoppingCartVM.OrderHeader.City = _shoppingCartVM.OrderHeader.ApplicationUser.City;
		_shoppingCartVM.OrderHeader.State = _shoppingCartVM.OrderHeader.ApplicationUser.State;
		_shoppingCartVM.OrderHeader.PostalCode = _shoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

		foreach (var cartItem in _shoppingCartVM.ListCart)
		{
			cartItem.Price = GetPriceBasedOnQuantity(cartItem.Count, cartItem.Product.Price, cartItem.Product.Price50, cartItem.Product.Price100);
			_shoppingCartVM.OrderHeader.OrderTotal += cartItem.Price * cartItem.Count;
		}

		return View(_shoppingCartVM);
	}

	
	[HttpPost]
	[ActionName("Summary")]
	[ValidateAntiForgeryToken]
	public IActionResult SummaryPost(ShoppingCartViewModel shoppingCartVM)
	{
		var claimsIdentity = (ClaimsIdentity)User.Identity;
		var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

		shoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(
				u => u.ApplicationUserId == claim.Value,
				includeProperties: new string[] { nameof(Product) });

		shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
		shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
		shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
		shoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;

		foreach (var cartItem in shoppingCartVM.ListCart)
		{
			cartItem.Price = GetPriceBasedOnQuantity(cartItem.Count, cartItem.Product.Price, cartItem.Product.Price50, cartItem.Product.Price100);
			shoppingCartVM.OrderHeader.OrderTotal += cartItem.Price * cartItem.Count;
		}

		_unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
		_unitOfWork.Save();

		foreach (var cart in shoppingCartVM.ListCart)
		{
			var orderDetail = new OrderDetail()
			{
				ProductId = cart.ProductId,
				OrderId = shoppingCartVM.OrderHeader.Id,
				Price = cart.Price,
				Count = cart.Count
			};
			_unitOfWork.OrderDetail.Add(orderDetail);
		}
		_unitOfWork.Save();

		// Strip settings
		var domain = "https://localhost:44323/";
		var options = new SessionCreateOptions
		{
			PaymentMethodTypes = new List<string>
			{
				"card",
			},
			LineItems = new List<SessionLineItemOptions>(),
			Mode = "payment",
			SuccessUrl = domain+$"Customer/Cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
			CancelUrl = domain+"Customer/Cart/Index",
		};

		foreach(var item in shoppingCartVM.ListCart)
		{
			var sessionLineItem = new SessionLineItemOptions
			{
				PriceData = new SessionLineItemPriceDataOptions
				{
					UnitAmount = (long) (item.Price*100), // 20.00 -> 2000
					Currency = "usd",
					ProductData = new SessionLineItemPriceDataProductDataOptions
					{
						Name = item.Product.Title,
					},

				},
				Quantity = item.Count,
			};
			options.LineItems.Add(sessionLineItem);
		}

		var service = new SessionService();
		var session = service.Create(options);
		_unitOfWork.OrderHeader.UpdateStripePaymentId(shoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
		_unitOfWork.Save();
		Response.Headers.Add("Location", session.Url);
		return new StatusCodeResult(303);


		//_unitOfWork.ShoppingCart.RemoveRange(shoppingCartVM.ListCart);
		//_unitOfWork.Save();
		//return RedirectToAction("Index", "Home");
	}

	public IActionResult OrderConfirmation(int id)
	{
		var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
		var service = new SessionService();
		var session = service.Get(orderHeader.SessionId);

		if (string.Equals(session.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase))
		{
			_unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
			_unitOfWork.Save();
		}

		var shoppingCartItems = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId);
		_unitOfWork.ShoppingCart.RemoveRange(shoppingCartItems.ToList());
		_unitOfWork.Save();
		return View(id);
	}

	public IActionResult Plus(int cartItemId)
	{
		var cartItem = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartItemId);
		_unitOfWork.ShoppingCart.IncrementCount(cartItem, 1);
		_unitOfWork.Save();
		return RedirectToAction(nameof(Index));
	}

	public IActionResult Minus(int cartItemId)
	{
		var cartItem = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartItemId);
		if (cartItem.Count <= 1)
		{
			_unitOfWork.ShoppingCart.Remove(cartItem);
		}
		else
		{
			_unitOfWork.ShoppingCart.DecrementCount(cartItem, 1);
		}
		_unitOfWork.Save();
		return RedirectToAction(nameof(Index));
	}

	public IActionResult Remove(int cartItemId)
	{
		var cartItem = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartItemId);
		_unitOfWork.ShoppingCart.Remove(cartItem);
		_unitOfWork.Save();
		return RedirectToAction(nameof(Index));
	}

	#region Helper
	private decimal GetPriceBasedOnQuantity(int quantity, decimal price, decimal price50, decimal price100)
	{
		switch (quantity)
		{
			case < 50:
				return price;
			case < 100:
				return price50;
			default:
				return price100;
		}
	}
	#endregion
}
