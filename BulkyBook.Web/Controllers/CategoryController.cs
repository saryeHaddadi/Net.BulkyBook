using BulkyBook.DataAccess;
using BulkyBook.DataAccess.IRepositories;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Web.Controllers;

public class CategoryController : Controller
{
	private readonly ICategoryRepository _db;

	public CategoryController(ICategoryRepository db)
	{
			_db = db;
	}

	public IActionResult Index()
	{
		IEnumerable<Category> objCategoryList = _db.GetAll();
		return View(objCategoryList);
	}

	//GET
	public IActionResult Create()
	{
		return View();
	}

	//POST
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
			_db.Add(obj);
			_db.Save();
			TempData["success"] = "Category created successfully";
			return RedirectToAction("Index");
		}
		return View(obj);
	}

	//GET
	public IActionResult Edit(int? id)
	{
		if (id == null || id == 0)
		{
			return NotFound();
		}

		var category = _db.GetFirstOrDefault(c => c.Id == id);
		//var category = _db.Categories.SingleOrDefault(c => c.Id == id);
		//var category = _db.Categories.Find(id);

		if (category == null)
		{
			return NotFound();
		}

		return View(category);
	}

	//POST
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
			_db.Update(obj);
			_db.Save();
			TempData["success"] = "Category updated successfully";
			return RedirectToAction("Index");
		}
		return View(obj);
	}


	//GET
	public IActionResult Delete(int? id)
	{
		if (id == null || id == 0)
		{
			return NotFound();
		}

		var category = _db.GetFirstOrDefault(c => c.Id == id);
		//var category = _db.Categories.SingleOrDefault(c => c.Id == id);
		//var category = _db.Categories.Find(id);

		if (category == null)
		{
			return NotFound();
		}

		return View(category);
	}

	//POST
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public IActionResult DeletePOST(int? id)
	{

		var obj = _db.GetFirstOrDefault(c => c.Id == id);
		if (obj == null)
		{
			return NotFound();
		}
		_db.Remove(obj);
		_db.Save();
		TempData["success"] = "Category deleted successfully";
		return RedirectToAction("Index");

	}

}

