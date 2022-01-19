using BulkyBook.DataAccess;
using BulkyBook.DataAccess.IRepositories;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Web.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.ROLE_ADMIN)]
public class CategoryController : Controller
{
	private readonly IUnitOfWork _unitOfWork;

	public CategoryController(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public IActionResult Index()
	{
		IEnumerable<Category> objCategoryList = _unitOfWork.Category.GetAll();
		return View(objCategoryList);
	}

	[HttpGet]
	public IActionResult Create()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Create(Category obj)
	{
		if (obj.Name == obj.DisplayOrder.ToString())
		{
			ModelState.AddModelError("Name", "DisplayOrder cannot exactly mach the Name.");
		}
		if (ModelState.IsValid)
		{
			_unitOfWork.Category.Add(obj);
			_unitOfWork.Save();
			TempData["success"] = "Category created successfully";
			return RedirectToAction("Index");
		}
		return View(obj);
	}

	[HttpGet]
	public IActionResult Edit(int? id)
	{
		if (id == null || id == 0)
		{
			return NotFound();
		}

		var category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
		//var category = _db.Categories.SingleOrDefault(c => c.Id == id);
		//var category = _db.Categories.Find(id);

		if (category == null)
		{
			return NotFound();
		}

		return View(category);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Edit(Category obj)
	{
		if (obj.Name == obj.DisplayOrder.ToString())
		{
			ModelState.AddModelError("Name", "DisplayOrder cannot exactly mach the Name.");
		}
		if (ModelState.IsValid)
		{
			_unitOfWork.Category.Update(obj);
			_unitOfWork.Save();
			TempData["success"] = "Category updated successfully";
			return RedirectToAction("Index");
		}
		return View(obj);
	}


	[HttpGet]
	public IActionResult Delete(int? id)
	{
		if (id == null || id == 0)
		{
			return NotFound();
		}

		var category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
		//var category = _db.Categories.SingleOrDefault(c => c.Id == id);
		//var category = _db.Categories.Find(id);

		if (category == null)
		{
			return NotFound();
		}

		return View(category);
	}

	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public IActionResult DeletePOST(int? id)
	{

		var obj = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
		if (obj == null)
		{
			return NotFound();
		}
		_unitOfWork.Category.Remove(obj);
		_unitOfWork.Save();
		TempData["success"] = "Category deleted successfully";
		return RedirectToAction("Index");

	}

}

