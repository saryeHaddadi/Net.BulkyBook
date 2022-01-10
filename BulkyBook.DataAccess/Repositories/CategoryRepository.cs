using BulkyBook.DataAccess.IRepositories;
using BulkyBook.DataAccess.Repositories.Base;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
	private ApplicationDbContext _db;

	public CategoryRepository(ApplicationDbContext db) : base(db)
	{
		_db = db;
	}

	public void Save()
	{
		_db.SaveChanges();
	}

	public void Update(Category obj)
	{
		_db.Categories.Update(obj);
	}
}
