using BulkyBook.DataAccess.IRepositories;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.ROLE_ADMIN)]
public class CoverTypeController : Controller
{
	private readonly IUnitOfWork _unitOfWork;

	public CoverTypeController(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public IActionResult Index()
	{
		var coverTypesList = _unitOfWork.CoverType.GetAll();
		return View(coverTypesList);
	}

	[HttpGet]
	public IActionResult Create()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Create(CoverType coverType)
	{
		if(ModelState.IsValid)
		{
			_unitOfWork.CoverType.Add(coverType);
			_unitOfWork.Save();
			TempData["success"] = "Cover Type created successfully";
			return RedirectToAction("Index");
		}
		return View(coverType);
	}

	[HttpGet]
	public IActionResult Edit(int? id)
	{
		if (id is null || id == 0)
		{
			return NotFound();
		}

		var coverTypeDetails = _unitOfWork.CoverType.GetFirstOrDefault(o => o.Id == id);

		if (coverTypeDetails is null)
		{
			return NotFound();
		}

		return View(coverTypeDetails);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Edit(CoverType coverType)
	{
		if (ModelState.IsValid)
		{
			_unitOfWork.CoverType.Update(coverType);
			_unitOfWork.Save();
			TempData["success"] = "Cover Type edited successfully";
			return RedirectToAction("Index");
		}
		return View(coverType);
	}

	[HttpGet]
	public IActionResult Delete(int? id)
	{
		if (id is null || id == 0)
		{
			return NotFound();
		}

		var coverTypeDetails = _unitOfWork.CoverType.GetFirstOrDefault(o => o.Id == id);

		if (coverTypeDetails is null)
		{
			return NotFound();
		}

		return View(coverTypeDetails);
	}

	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public IActionResult DeletePost(int? id)
	{
		var coverType = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);
		if (coverType is null)
		{
			return NotFound();
		}

		_unitOfWork.CoverType.Remove(coverType);
		_unitOfWork.Save();
		TempData["success"] = "Cover Type deleted successfully";
		return RedirectToAction("Index");
	}

}
