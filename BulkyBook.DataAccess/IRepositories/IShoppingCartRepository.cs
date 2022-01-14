using BulkyBook.DataAccess.IRepositories.Base;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.IRepositories;

public interface IShoppingCartRepository : IRepository<ShoppingCart>
{
	int IncrementCount(ShoppingCart shoppingCart, int count);
	int DecrementCount(ShoppingCart shoppingCart, int count);
}
