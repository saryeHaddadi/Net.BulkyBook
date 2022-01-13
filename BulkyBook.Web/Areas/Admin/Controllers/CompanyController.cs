using BulkyBook.DataAccess.IRepositories;
using BulkyBook.Models;
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

	[HttpGet]
	public IActionResult Create()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Create(Company company)
	{
		if (ModelState.IsValid)
		{
			_unitOfWork.Company.Add(company);
			_unitOfWork.Save();
			TempData["success"] = "Company created successfully";
			return RedirectToAction("Index");
		}
		return View(company);

	}

	[HttpGet]
	public IActionResult Edit(int? id)
	{
		if (id is null || id == 0)
		{
			return NotFound();
		}

		var company = _unitOfWork.Company.GetFirstOrDefault(c => c.Id == id);
		if (company is null)
		{
			return NotFound();
		}

		return View(company);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Edit(Company company)
	{
		if (ModelState.IsValid)
		{
			_unitOfWork.Company.Update(company);
			_unitOfWork.Save();
			TempData["success"] = "Company edited successfully";
			return RedirectToAction("Index");
		}
		return View(company);

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
