using BulkyBook.DataAccess.IRepositories;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBook.Web.Areas.Admin.Controllers;

[Area(nameof(Admin))]
[Authorize]
public class OrderController : Controller
{
	public readonly IUnitOfWork _unitOfWork;
	[BindProperty]
	public OrderViewModel orderVM { get; set; }

	public OrderController(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public IActionResult Index()
	{
		return View();
	}

	public IActionResult Details(int orderId)
	{
		orderVM = new OrderViewModel()
		{
			OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId, includeProperties: new string[] { nameof(ApplicationUser) }),
			OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.Id == orderId, includeProperties: new string[] { nameof(Product) }),
		};
		return View(orderVM);
	}


	#region API Calls
	[HttpGet]
	public IActionResult GetAll(string status)
	{
		IEnumerable<OrderHeader> orderHeaders;

		if (User.IsInRole(SD.ROLE_ADMIN) || User.IsInRole(SD.ROLE_EMPLOYEE))
		{
			orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: new string[] { nameof(ApplicationUser) });
		}
		else
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			orderHeaders = _unitOfWork.OrderHeader.GetAll(u=>u.ApplicationUserId == claim.Value, includeProperties: new string[] { nameof(ApplicationUser) });
		}

		switch (status)
		{
			case "inprocess":
				orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayement);
				break;
			case "pending":
				orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
				break;
			case "completed":
				orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
				break;
			case "approved":
				orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
				break;
			default:
				break;
		}

		return Json(new { data = orderHeaders });
	}
	#endregion
}
