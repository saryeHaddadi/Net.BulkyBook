using BulkyBook.DataAccess.IRepositories.Base;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.IRepositories;

public interface IOrderHeaderRepository : IRepository<OrderHeader>
{
	void Update(OrderHeader obj);
	void UpdateStatus(int id, string orderStatus, string paymentStatus = null);
}
