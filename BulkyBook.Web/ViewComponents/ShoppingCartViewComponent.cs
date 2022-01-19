using BulkyBook.DataAccess.IRepositories;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBook.Web.ViewComponents;

public class ShoppingCartViewComponent : ViewComponent
{
	private readonly IUnitOfWork _unitOfWork;

	public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
    {
		_unitOfWork = unitOfWork;
	}

	public async Task<IViewComponentResult> InvokeAsync()
    {
		var claimsIdentity = (ClaimsIdentity)User.Identity;
		var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
		if (claim is null)
        {
			// User not registered, or user has just logged out, safe to clear the session.
			HttpContext.Session.Clear();
			return View(0);
		}
        else
        {
			if (HttpContext.Session.GetInt32(SD.SessionCartItemCount) is null)
			{
				HttpContext.Session.SetInt32(SD.SessionCartItemCount,
					_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
			}
			return View(HttpContext.Session.GetInt32(SD.SessionCartItemCount));
		}
	}
}
