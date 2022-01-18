using BulkyBook.DataAccess.IRepositories;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Web.Areas.Admin;

[Area("Admin")]
public class OrderController : Controller
{
	public readonly IUnitOfWork _unitOfWork;
	public OrderController(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public IActionResult Index()
	{
		return View();
	}


	#region API Calls
	[HttpGet]
	public IActionResult GetAll()
	{
		var orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: new string[] { nameof(ApplicationUser) });
		return Json(new { data = orderHeaders });
	}
	#endregion
}
