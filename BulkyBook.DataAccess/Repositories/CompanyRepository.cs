using BulkyBook.DataAccess.IRepositories;
using BulkyBook.DataAccess.Repositories.Base;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repositories;

public class CompanyRepository : Repository<Company>, ICompanyRepository
{
	private ApplicationDbContext _db;

	public CompanyRepository(ApplicationDbContext db) : base(db)
	{
		_db = db;
	}

	public void Update(Company obj)
	{
		_db.Company.Update(obj);
	}
}
