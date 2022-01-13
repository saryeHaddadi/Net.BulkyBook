using BulkyBook.DataAccess.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class CompanyController : Controller
{
	private readonly IUnitOfWork _unitOfWork;
	public CompanyController(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	[HttpGet]
	public IActionResult Index()
	{
		return View();
	}

	#region API
	[HttpGet]
	public IActionResult GetAll()
	{
		var companyList = _unitOfWork.Company.GetAll();
		return Json(new { data = companyList });
	}

	[HttpDelete]
	public IActionResult Delete(int? id)
	{
		var company = _unitOfWork.Company.GetFirstOrDefault(c => c.Id == id);
		if (company is null)
		{
			return Json(new { success = false, message = "Error while deleting company" });
		}
		_unitOfWork.Company.Remove(company);
		_unitOfWork.Save();
		return Json(new { success = true, message = "Company deleted successfully" });
	}
	#endregion
}
