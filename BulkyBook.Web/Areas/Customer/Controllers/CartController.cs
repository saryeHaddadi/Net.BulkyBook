using BulkyBook.DataAccess.IRepositories;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBook.Web.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{
	private readonly IUnitOfWork _unitOfWork;
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
