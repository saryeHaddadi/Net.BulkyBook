using BulkyBook.DataAccess.IRepositories;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBook.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
	private readonly IUnitOfWork _unitOfWork;

	public ProductController(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public IActionResult Index()
	{
		var ProductsList = _unitOfWork.Product.GetAll();
		return View(ProductsList);
	}


	[HttpGet]
	public IActionResult Upsert(int? id)
	{
		var product = new Product();
		var categoryList = _unitOfWork.Category.GetAll().Select(
			u => new SelectListItem
			{
				Text = u.Name,
				Value = u.Id.ToString()
			});
		var coverTypeList = _unitOfWork.CoverType.GetAll().Select(
			u => new SelectListItem
			{
				Text = u.Name,
				Value = u.Id.ToString()
			});

		if (id is null || id == 0)
		{
			// Create Product
			ViewBag.CategoryList = categoryList;
			return View(product);
		}
		else
		{
			// Update Product
		}

		return View(product);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Upsert(Product Product)
	{
		if (ModelState.IsValid)
		{
			_unitOfWork.Product.Update(Product);
			_unitOfWork.Save();
			TempData["success"] = "Product edited successfully";
			return RedirectToAction("Index");
		}
		return View(Product);
	}

	[HttpGet]
	public IActionResult Delete(int? id)
	{
		if (id is null || id == 0)
		{
			return NotFound();
		}

		var ProductDetails = _unitOfWork.Product.GetFirstOrDefault(o => o.Id == id);

		if (ProductDetails is null)
		{
			return NotFound();
		}

		return View(ProductDetails);
	}

	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public IActionResult DeletePost(int? id)
	{
		var Product = _unitOfWork.Product.GetFirstOrDefault(c => c.Id == id);
		if (Product is null)
		{
			return NotFound();
		}

		_unitOfWork.Product.Remove(Product);
		_unitOfWork.Save();
		TempData["success"] = "Product deleted successfully";
		return RedirectToAction("Index");
	}

}
