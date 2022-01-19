using BulkyBook.DataAccess.IRepositories;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
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
			OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeader.Id == orderId, includeProperties: new string[] { nameof(Models.Product) }),
		};
		return View(orderVM);
	}

	[ActionName("Details")]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult DetailsPayNow()
	{
		orderVM.OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderVM.OrderHeader.Id, includeProperties: new string[] { nameof(ApplicationUser) });
		orderVM.OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeader.Id == orderVM.OrderHeader.Id, includeProperties: new string[] { nameof(ApplicationUser) });

		var domain = "https://localhost:44323/";
		var options = new SessionCreateOptions
		{
			PaymentMethodTypes = new List<string>
			{
				"card",
			},
			LineItems = new List<SessionLineItemOptions>(),
			Mode = "payment",
			SuccessUrl = domain + $"Admin/Order/PaymentConfirmation?orderHeaderId={orderVM.OrderHeader.Id}",
			CancelUrl = domain + $"Admin/Order/Details?orderId={orderVM.OrderHeader.Id}",
		};

		foreach (var item in orderVM.OrderDetail)
		{
			var sessionLineItem = new SessionLineItemOptions
			{
				PriceData = new SessionLineItemPriceDataOptions
				{
					UnitAmount = (long)(item.Price * 100), // 20.00 -> 2000
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
		_unitOfWork.OrderHeader.UpdateStripePaymentId(orderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
		_unitOfWork.Save();
		Response.Headers.Add("Location", session.Url);
		return new StatusCodeResult(303);
	}

	public IActionResult PaymentConfirmation(int orderHeaderId)
	{
		var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderHeaderId);
		if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayement)
		{
			var service = new SessionService();
			var session = service.Get(orderHeader.SessionId);

			if (string.Equals(session.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase))
			{
				_unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
				_unitOfWork.Save();
			}
		}

		return View(orderHeaderId);
	}

	[HttpPost]
	[Authorize(Roles = SD.ROLE_ADMIN + "," + SD.ROLE_EMPLOYEE)]
	[ValidateAntiForgeryToken]
	public IActionResult UpdateOrderDetail()
	{
		var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderVM.OrderHeader.Id, tracked: false);
		orderHeader.Name = orderVM.OrderHeader.Name;
		orderHeader.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
		orderHeader.StreetAdresse = orderVM.OrderHeader.StreetAdresse;
		orderHeader.City = orderVM.OrderHeader.City;
		orderHeader.State = orderVM.OrderHeader.State;
		orderHeader.PostalCode = orderVM.OrderHeader.PostalCode;
		if (orderVM.OrderHeader.Carrier is not null)
		{
			orderHeader.Carrier = orderVM.OrderHeader.Carrier;
		}
		if (orderVM.OrderHeader.TrackingNumber is not null)
		{
			orderHeader.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
		}

		_unitOfWork.OrderHeader.Update(orderHeader);
		_unitOfWork.Save();
		TempData["Success"] = "Order Details Updated Successfully.";
		return RedirectToAction("Details", "Order", new { orderId = orderHeader.Id });
	}

	[HttpPost]
	[Authorize(Roles = SD.ROLE_ADMIN + "," + SD.ROLE_EMPLOYEE)]
	[ValidateAntiForgeryToken]
	public IActionResult StartProcessing()
	{
		_unitOfWork.OrderHeader.UpdateStatus(orderVM.OrderHeader.Id, SD.StatusInProcess);
		_unitOfWork.Save();
		TempData["Success"] = "Order Status Updated Successfully.";
		return RedirectToAction("Details", "Order", new { orderId = orderVM.OrderHeader.Id });
	}

	[HttpPost]
	[Authorize(Roles = SD.ROLE_ADMIN + "," + SD.ROLE_EMPLOYEE)]
	[ValidateAntiForgeryToken]
	public IActionResult ShipOrder()
	{
		var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderVM.OrderHeader.Id, tracked: false);
		orderHeader.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
		orderHeader.Carrier = orderVM.OrderHeader.Carrier;
		orderHeader.OrderStatus = SD.StatusShipped;
		orderHeader.ShippingDate = DateTime.Now;
		if(orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayement)
        {
			orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
        }
		_unitOfWork.OrderHeader.Update(orderHeader);
		_unitOfWork.Save();
		TempData["Success"] = "Order Shipped Successfully.";
		return RedirectToAction("Details", "Order", new { orderId = orderVM.OrderHeader.Id });
	}

	[HttpPost]
	[Authorize(Roles = SD.ROLE_ADMIN + "," + SD.ROLE_EMPLOYEE)]
	[ValidateAntiForgeryToken]
	public IActionResult CancelOrder()
	{
		var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderVM.OrderHeader.Id, tracked: false);
		if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
        {
			var options = new Stripe.RefundCreateOptions
			{
				Reason = RefundReasons.RequestedByCustomer,
				PaymentIntent = orderHeader.PaymentIntentId
			};
			var service = new RefundService();
			var refund = service.Create(options);
			_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
        }
        else
        {
			_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
		}
		_unitOfWork.Save();
		TempData["Success"] = "Order Cancelled Successfully.";
		return RedirectToAction("Details", "Order", new { orderId = orderVM.OrderHeader.Id });
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
			// Note: the flow for Pending & Completed is not perfect.
			// Our point here is to show how .net is binding everything together.
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
