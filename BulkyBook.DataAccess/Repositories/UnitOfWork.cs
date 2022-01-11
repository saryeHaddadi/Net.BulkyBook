using BulkyBook.DataAccess.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repositories;

public class UnitOfWork : IUnitOfWork
{
	private ApplicationDbContext _db;
	public ICategoryRepository Category { get; private set; }
	public ICoverTypeRepository CoverType { get; private set; }
	public IProductRepository Product { get; private set; }

	public UnitOfWork(ApplicationDbContext db)
	{
		_db = db;
		Category = new CategoryRepository(_db);
		CoverType = new CoverTypeRepository(_db);
		Product = new ProductRepository(_db);
	}

	public void Save()
	{
		_db.SaveChanges();
	}
}
