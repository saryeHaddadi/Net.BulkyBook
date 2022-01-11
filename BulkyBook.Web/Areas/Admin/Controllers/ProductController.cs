using BulkyBook.DataAccess.IRepositories;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
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
		ProductViewModel productVM = new ProductViewModel()
		{
			Product = new(),
			CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
			{
				Text = x.Name,
				Value = x.Id.ToString()
			}),
			CoverTypeList = _unitOfWork.CoverType.GetAll().Select(x => new SelectListItem
			{
				Text = x.Name,
				Value = x.Id.ToString()
			})
		};

		if (id is null || id == 0)
		{
			// Create Product
			//ViewBag.CategoryList = categoryList;
			//ViewData["CoverTypeList"] = coverTypeList;
			return View(productVM);
		}
		else
		{
			// Update Product
		}

		return View(productVM);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Upsert(ProductViewModel productVM, IFormFile file)
	{
		if (ModelState.IsValid)
		{
			//_unitOfWork.Product.Update(Product);
			_unitOfWork.Save();
			TempData["success"] = "Product edited successfully";
			return RedirectToAction("Index");
		}
		return View(productVM);
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
