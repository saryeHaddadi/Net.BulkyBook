using BulkyBook.DataAccess.IRepositories;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.DbInitializer;

public class DbInitializer : IDbInitializer
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly ApplicationDbContext _db;

	public DbInitializer(
		UserManager<IdentityUser> userManager,
		RoleManager<IdentityRole> roleManager,
		ApplicationDbContext db)
	{
		_userManager = userManager;
		_roleManager = roleManager;
		_db = db;
	}

	/// <summary>
	/// Apply migrations if they are not applied
	/// Create roles if they are not created
	/// If roles were not created, create an admin user as well
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	public void Initialize()
	{
		// Apply migrations if they are not applied
		try
		{
			if (_db.Database.GetPendingMigrations().Count() > 0)
			{
				_db.Database.Migrate();
			}
		}
		catch (Exception ex)
		{

		}

		// Create roles if they are not created
		if (!_roleManager.RoleExistsAsync(SD.ROLE_ADMIN).GetAwaiter().GetResult())
		{
			_roleManager.CreateAsync(new IdentityRole(SD.ROLE_ADMIN)).GetAwaiter().GetResult();
			_roleManager.CreateAsync(new IdentityRole(SD.ROLE_INDIVIDUAL)).GetAwaiter().GetResult();
			_roleManager.CreateAsync(new IdentityRole(SD.ROLE_COMPANY)).GetAwaiter().GetResult();
			_roleManager.CreateAsync(new IdentityRole(SD.ROLE_EMPLOYEE)).GetAwaiter().GetResult();

			// If roles were not created, create an admin user as well
			var email = "admin@bulky.com";
			_userManager.CreateAsync(new ApplicationUser
			{
				UserName = email,
				Email = email,
				Name = "Admin",
				PhoneNumber = "000",
				StreetAdresse = "None",
				State = "None",
				PostalCode = "None",
				City = "None"
			}, "NotSecurePassword123+").GetAwaiter().GetResult();

			var user = _db.ApplicationUser.FirstOrDefault(u => u.Email == email);
			_userManager.AddToRoleAsync(user, SD.ROLE_ADMIN).GetAwaiter().GetResult();
		}
	}
}
