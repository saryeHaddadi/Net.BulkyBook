using BulkyBook.DataAccess.IRepositories;
using BulkyBook.DataAccess.Repositories.Base;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
	private ApplicationDbContext _db;

	public ProductRepository(ApplicationDbContext db) : base(db)
	{
		_db = db;
	}

	public void Update(Product obj)
	{
		var objFromDb = _db.Products.FirstOrDefault(o => o.Id == obj.Id);
		if (objFromDb is not null)
		{
			objFromDb.Title = obj.Title;
			objFromDb.Description = obj.Description;
			objFromDb.ISBN = obj.ISBN;
			objFromDb.Author = obj.Author;
			objFromDb.ListPrice = obj.ListPrice;
			objFromDb.Price = obj.Price;
			objFromDb.Price50 = obj.Price50;
			objFromDb.Price100 = obj.Price100;
			objFromDb.Category = obj.Category;
			objFromDb.CoverType = obj.CoverType;
			if (obj.ImageUrl is not null)
			{
				objFromDb.ImageUrl = obj.ImageUrl;
			}
		}
		_db.Products.Update(objFromDb);
	}
}
