using BulkyBook.DataAccess.IRepositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repositories.Base;

public class Repository<T> : IRepository<T> where T : class
{
	private readonly ApplicationDbContext _db;
	internal DbSet<T> _dbSet;

	public Repository(ApplicationDbContext db)
	{
		_db = db;
		_dbSet = db.Set<T>();
	}

	public void Add(T entity)
	{
		_dbSet.Add(entity);
	}

	public IEnumerable<T> GetAll()
	{
		IQueryable<T> query = _dbSet;
		return query.ToList();
	}

	public T GetFirstOrDefault(Expression<Func<T, bool>> filter)
	{
		IQueryable<T> query = _dbSet;
		query = query.Where(filter);
		return query.FirstOrDefault();
	}

	public void Remove(T entity)
	{
		_dbSet.Remove(entity);
	}

	public void RemoveRange(IEnumerable<T> entities)
	{
		_dbSet.RemoveRange(entities);
	}
}
