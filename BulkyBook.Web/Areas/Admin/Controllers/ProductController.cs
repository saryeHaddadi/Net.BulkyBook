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
	private readonly IWebHostEnvironment _hostEnvironment;

	public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
	{
		_unitOfWork = unitOfWork;
		_hostEnvironment = hostEnvironment;
	}

	public IActionResult Index()
	{
		var ProductsList = _unitOfWork.Product.GetAll();
		return View(ProductsList);
	}


	[HttpGet]
	public IActionResult Upsert(int? id)
	{
		var productVM = new ProductViewModel()
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
			productVM.Product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
			return View(productVM);
		}
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Upsert(ProductViewModel obj, IFormFile file)
	{
		if (ModelState.IsValid || (ModelState.ErrorCount == 1 && file is null && obj.Product.ImageUrl is not null))
		{
			var wwwRootPath = _hostEnvironment.WebRootPath;
			var targetDirectory = @"images\products";

			if (file is not null)
			{
				var fileName = Guid.NewGuid().ToString();
				var targetRelativePath = Path.Combine(targetDirectory, fileName + Path.GetExtension(file.FileName));
				var targetFullPath = Path.Combine(wwwRootPath, targetRelativePath);

				if (obj.Product.ImageUrl is not null)
				{
					var oldImageFullPath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.Substring(1)); // Remove leading slash, no more wwwroot's perspective
					if (System.IO.File.Exists(oldImageFullPath))
					{
						System.IO.File.Delete(oldImageFullPath);
					}
				}

				using (var fileStream = new FileStream(targetFullPath, FileMode.Create))
				{
						file.CopyTo(fileStream);
				}

				obj.Product.ImageUrl = @"\" + targetRelativePath; // Convert to absult Path (wwwroot perspective)
			}

			if (obj.Product.Id == 0)
			{
				_unitOfWork.Product.Add(obj.Product);
			}
			else
			{
				_unitOfWork.Product.Update(obj.Product);
			}

			_unitOfWork.Save();
			TempData["success"] = "Product created successfully";
			return RedirectToAction("Index");
		}

		// Review the reload when the odel is Invalid.
		// Main issues:
		// - relaod the image
		// - display the content of the dropdowns
		obj = new ProductViewModel()
		{
			Product = obj.Product,
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
		return View(obj);
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

	#region API Calls
	[HttpGet]
	public IActionResult GetAll()
	{
		var productList = _unitOfWork.Product.GetAll(includeProperties: new string[] {nameof(Product.Category)});
		return Json(new { data = productList });
	}
	#endregion
}
