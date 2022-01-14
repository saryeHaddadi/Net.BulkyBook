using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.IRepositories;

public interface IUnitOfWork
{
	ICategoryRepository Category { get; }
	ICoverTypeRepository CoverType { get; }
	IProductRepository Product { get; }
	ICompanyRepository Company { get; }
	IShoppingCartRepository ShoppingCart { get; }
	IApplicationUserRepository ApplicationUser { get; }
	IOrderHeaderRepository OrderHeader { get; }
	IOrderDetailRepository OrderDetail { get; }
	void Save();
}
