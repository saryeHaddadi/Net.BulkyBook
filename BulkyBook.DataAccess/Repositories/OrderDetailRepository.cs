using BulkyBook.DataAccess.IRepositories;
using BulkyBook.DataAccess.Repositories.Base;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repositories;

public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
{
	private ApplicationDbContext _db;

	public OrderDetailRepository(ApplicationDbContext db) : base(db)
	{
		_db = db;
	}

	public void Update(OrderDetail obj)
	{
		_db.OrderDetail.Update(obj);
	}

}
