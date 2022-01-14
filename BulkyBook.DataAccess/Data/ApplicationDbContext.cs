using BulkyBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess;

public class ApplicationDbContext : IdentityDbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{

	}
	public DbSet<ApplicationUser> ApplicationUser { get; set; }

	public DbSet<Category> Category { get; set; }
	public DbSet<CoverType> CoverType { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<Company> Company { get; set; }
	public DbSet<ShoppingCart> ShoppingCart { get; set; }
	public DbSet<OrderHeader> OrderHeaders { get; set; }
	public DbSet<OrderDetail> OrderDetails { get; set; }

}

