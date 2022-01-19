using BulkyBook.DataAccess.IRepositories;
using BulkyBook.DataAccess.Repositories.Base;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repositories;

public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
{
	private ApplicationDbContext _db;

	public OrderHeaderRepository(ApplicationDbContext db) : base(db)
	{
		_db = db;
	}

	public void Update(OrderHeader obj)
	{
		_db.OrderHeader.Update(obj);
	}

	public void UpdateStatus(int id, string orderStatus, string paymentStatus = null)
	{
		var order = _db.OrderHeader.FirstOrDefault(u => u.Id == id);
		if (order is not null)
		{
			order.OrderStatus = orderStatus;
			if (paymentStatus is not null)
			{
				order.PaymentStatus = paymentStatus;
			}
		}
	}

	public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
	{
		var order = _db.OrderHeader.FirstOrDefault(u => u.Id == id);
		order.PaymentDate = DateTime.Now;
		order.SessionId = sessionId;
		order.PaymentIntentId = paymentIntentId;
	}
}
