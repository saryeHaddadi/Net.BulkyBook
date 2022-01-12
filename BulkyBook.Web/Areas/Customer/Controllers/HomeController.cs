using BulkyBook.DataAccess.IRepositories;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

	public IActionResult Details(int id)
	{
		ShoppingCart cart = new()
		{
			Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id, includeProperties: new string[] { nameof(Product.Category), nameof(Product.CoverType) }),
			Count = 1
		};
		return View(cart);
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
