using BulkyBook.DataAccess.IRepositories;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBook.Web.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly IUnitOfWork _unitOfWork;

	public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
	{
		_logger = logger;
		_unitOfWork = unitOfWork;
	}

	public IActionResult Index()
	{
		var productList = _unitOfWork.Product.GetAll(includeProperties: new string[] { nameof(Product.Category), nameof(Product.CoverType)});
		return View(productList);
	}

	public IActionResult Details(int productId)
	{
		ShoppingCart cart = new()
		{
			Product = _unitOfWork.Product.GetFirstOrDefault(
				u => u.Id == productId,
				includeProperties: new string[] { nameof(Product.Category), nameof(Product.CoverType) }),
			ProductId = productId,
			Count = 1
		};
		return View(cart);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize]
	public IActionResult Details(ShoppingCart cart)
	{
		var claimsIdentity = (ClaimsIdentity)User.Identity;
		var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
		cart.ApplicationUserId = claim.Value;

		var cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
			u => u.ApplicationUserId == cart.ApplicationUserId && u.ProductId == cart.ProductId);

		if (cartFromDb is null)
		{
			_unitOfWork.ShoppingCart.Add(cart);
			_unitOfWork.Save();
			HttpContext.Session.SetInt32(SD.SessionCartItemCount,
				_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
		}
		else
		{
			_unitOfWork.ShoppingCart.IncrementCount(cartFromDb, cart.Count);
			_unitOfWork.Save();
		}

		return RedirectToAction(nameof(Index));
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
