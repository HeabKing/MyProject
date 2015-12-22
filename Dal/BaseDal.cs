using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Model;

namespace Dal
{
	/// <summary>
	/// Dal 层实体的通用操作
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BaseDal<T> where T : class, new()
	{
		// 线程唯一
		readonly DbContext _dbContext = DbContextFactory.Context;

		#region 增 - 一个实体

		/// <summary>
		/// 添加实体 - 一个
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public T Add(T model)
		{
			_dbContext.Set<T>().Add(model);
			// 返回受影响的行数
			// 把Id赋值给model
			_dbContext.SaveChanges();
			return model;
		}

		#endregion

		#region 删 - 一个实体
		/// <summary>
		/// 删除 - 根据Id - 直接删除
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public int Del(T model)
		{
			_dbContext.Set<T>().Attach(model); // 将实体以Unchanged状态添加到上下文
			_dbContext.Set<T>().Remove(model);
			return _dbContext.SaveChanges();
		}

		/// <summary>
		/// 删除 - 根据Id - 先查后删
		/// </summary>
		/// <param name="primaryKey"></param>
		/// <returns>成功: 1 失败: 异常</returns>
		public int Del(object primaryKey)
		{
			T model = _dbContext.Set<T>().Find(primaryKey);
			_dbContext.Set<T>().Remove(model);
			return _dbContext.SaveChanges();
		}
		#endregion

		#region 删 - 一个集合
		/// <summary>
		/// 删除 - 批量删除
		/// 先查出来 然后一条一条的删除
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public int Del(Expression<Func<T, bool>> predicate)
		{
			var set = _dbContext.Set<T>().Where(predicate);
			_dbContext.Set<T>().RemoveRange(set);
			return _dbContext.SaveChanges();
			//await _db.Set<T>().Where(predicate).ForEachAsync(m => _db.Set<T>().Remove(m));
			//return await _db.SaveChangesAsync();
		}
		#endregion

		#region 改 - 一个实体

		/// <summary>
		/// 改 - 一个实体 - 先查后改
		/// </summary>
		/// <param name="mdl">不需要主键, 只写要修改的字段</param>
		/// <param name="key">主键</param>
		/// <param name="updateProNames">要修改的属性名</param>
		/// <returns></returns>
		public int Update(object key, T mdl, params string[] updateProNames)
		{
			// 如果没写属性 - 异常
			if (updateProNames == null || updateProNames.Length < 1)
			{
				throw new Exception("请至少指定一个要更改的属性!");
			}
			// 获取类型对象
			Type t = mdl.GetType();
			// 从数据库获取指定的数据
			T entity = _dbContext.Set<T>().Find(key);
			if (entity == null)
			{
				throw new Exception("指定的要修改的实体在数据库中未找到!");
			}
			// 开始更改
			updateProNames.ToList().ForEach(p =>
			{
				PropertyInfo pro = t.GetProperty(p);
				if (pro == null)
				{
					throw new Exception("输入的更改列表中有拼写错误!");
				}
				object value = pro.GetValue(mdl, null);
				pro.SetValue(entity, value, null);
			});
			return _dbContext.SaveChanges();
		}

		#region 该 - 一个实体 - 直接更改

		// <summary>
		// 更改一条数据, 需指定更改的字段
		// 这个函数在第二次执行同一个主键的数据时会报异常, ObjectStatusManager只能同时跟踪一个主键相同的对象
		// 改成Virtual, 让派生类通过Id查找出来再去改把
		// </summary>
		// <param name="model">要更改的数据</param>
		// <param name="modifiedProNames">更改的字段名</param>
		// <returns></returns>
		//public virtual int Modify(T model, params string[] modifiedProNames)
		//{
		//	DbEntityEntry<T> entry = _db.Entry(model);
		//	entry.State = EntityState.Unchanged;
		//	foreach (string item in modifiedProNames)
		//	{
		//		entry.Property(item).IsModified = true;
		//	}
		//	if (modifiedProNames.Length < 1)
		//	{
		//		throw new Exception(GetType() + " : 请指定要修改的属性的名字!");
		//	}

		//	return _db.SaveChanges();
		//}

		#endregion

		#endregion

		#region 改 - 一个集合
		/// <summary>
		/// 改 - 一个集合 - 先查后改
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="mdl"></param>
		/// <param name="updateProNames"></param>
		/// <returns></returns>
		public int Update(Expression<Func<T, bool>> predicate, T mdl, params string[] updateProNames)
		{
			// 如果没写属性 - 异常
			if (updateProNames == null || updateProNames.Length < 1)
			{
				throw new Exception("请至少指定一个要更改的属性!");
			}
			// 获取类型对象
			Type t = mdl.GetType();
			// 从数据库获取指定的数据
			List<T> listEntity = _dbContext.Set<T>().Where(predicate).ToList();
			if (listEntity.Count < 1)
			{
				throw new Exception("指定的要修改的实体在数据库中未找到!");
			}
			// 开始更改
			updateProNames.ToList().ForEach(p =>
			{
				PropertyInfo pro = t.GetProperty(p);
				if (pro == null)
				{
					throw new Exception("输入的更改列表中有拼写错误!");
				}
				object value = pro.GetValue(mdl, null);
				listEntity.ForEach(m => pro.SetValue(m, value, null));
			});
			return _dbContext.SaveChanges();
		}
		#endregion

		#region 查 - 获取一个实体

		/// <summary>
		/// 获取实体 - 根据主键
		/// </summary>
		/// <param name="primaryKey">主键</param>
		/// <returns>实体/null</returns>
		public T Find(object primaryKey)
		{
			return _dbContext.Set<T>().Find(primaryKey);
		}

		/// <summary>
		/// 获取实体 - 唯一的实体
		/// </summary>
		/// <param name="predicate">给定一个谓词</param>
		/// <returns>实体/异常(零个/多个)</returns>
		public T Single(Expression<Func<T, bool>> predicate)
		{
			return _dbContext.Set<T>().Single(predicate);
		}

		/// <summary>
		/// 获取实体 - 唯一或不存在的实体
		/// </summary>
		/// <returns>实体(一个)/null(零个)/异常(多个)</returns>
		public T SingleOrDefault(Expression<Func<T, bool>> predicate)
		{
			return _dbContext.Set<T>().SingleOrDefault(predicate);
		}

		/// <summary>
		/// 获取实体 - 第一个实体
		/// </summary>
		/// <param name="predicate">给定谓词</param>
		/// <returns>实体: 第一个; 异常: 零个</returns>
		public T First(Expression<Func<T, bool>> predicate)
		{
			return _dbContext.Set<T>().First(predicate);
		}

		/// <summary>
		/// 获取实体 - 排序 - 第一个实体
		/// </summary>
		/// <param name="predicate">给定谓词</param>
		/// <param name="order">排序</param>
		/// <returns>实体: 第一个; 异常: 零个</returns>
		public T First<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> order)
		{
			return _dbContext.Set<T>().OrderBy(order).First(predicate);
		}

		/// <summary>
		/// 获取实体 - 第一个或不存在的实体
		/// </summary>
		/// <returns>实体: 第一个; null: 零个</returns>
		public T FirstOrDefault(Expression<Func<T, bool>> predicate)
		{
			return _dbContext.Set<T>().FirstOrDefault(predicate);
		}

		/// <summary>
		/// 获取实体 - 按指定顺序排序 - 第一个或不存在的实体
		/// </summary>
		/// <returns>实体: 第一个; null: 零个</returns>
		public T FirstOrDefault<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> order)
		{
			return _dbContext.Set<T>().OrderBy(order).FirstOrDefault(predicate);
		}

		#endregion

		#region 查 - 获取实体集合
		/// <summary>
		/// 获取实体集合 - 根据条件
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public List<T> GetList(Expression<Func<T, bool>> predicate)
		{
			return _dbContext.Set<T>().Where(predicate).ToList();
			//不进行缓存的查询
			//return await _db.Set<T>().Where(predicate).AsNoTracking().ToListAsync();
		}

		/// <summary>
		/// 获取实体集合 - 排序查询
		/// </summary>
		/// <typeparam name="TKey">排序字段类型</typeparam>
		/// <param name="predicate">查询条件</param>
		/// <param name="orderLambda">排序条件</param>
		/// <returns></returns>
		public List<T> GetList<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderLambda)
		{
			return _dbContext.Set<T>()
				.Where(predicate)
				.OrderBy(orderLambda)
				.ToList();
		}

		/// <summary>
		/// 获取实体集合 - 分页查询
		/// </summary>
		/// <typeparam name="TKey">排序类型</typeparam>
		/// <param name="pageIndex">页码</param>
		/// <param name="pageSize">页容量</param>
		/// <param name="predicate">谓词</param>
		/// <param name="orderBy">排序lambda</param>
		/// <returns></returns>
		public List<T> GetList<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderBy)
		{
			return _dbContext.Set<T>()
				.Where(predicate)
				.OrderBy(orderBy)
				.Skip((pageIndex - 1) * pageSize)
				.Take(pageSize)
				.ToList();
		}

		///// <summary>
		///// 集合: 分页查询
		///// </summary>
		///// <param name="pageIndex">页码</param>
		///// <param name="pageSize">页容量</param>
		///// <param name="orderBy">排序lambda表达式</param>
		///// <param name="whereLambda">条件lambda表达式</param>
		///// <param name="total">返回数据的总条数</param>
		///// <returns></returns>
		//public List<T> GetPageList<TKey>(int pageIndex, int pageSize, Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> whereLambda, out int total)
		//{
		//	total = _db.Set<T>().Where(whereLambda).Count();
		//	return _db.Set<T>().Where(whereLambda).OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
		//} 
		#endregion

		#region 查 - 获取数据库时间

		/// <summary>
		/// 获取数据库时间
		/// </summary>
		/// <returns></returns>
		public DateTime GetTime()
		{
			// 加上.FirstOrDefault是为了针对延迟加载
			return _dbContext.Database.SqlQuery<DateTime>("SELECT GETDATE() AS [DbTime]").FirstOrDefault();
		}

		#endregion

		#region 异步版本
		#region 增 - 一个实体

		/// <summary>
		/// 添加实体 - 一个
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<T> AddAsync(T model)
		{
			_dbContext.Set<T>().Add(model);
			// 返回受影响的行数
			// 把Id赋值给model
			await _dbContext.SaveChangesAsync();
			return model;
		}

		#endregion

		#region 删 - 一个实体
		/// <summary>
		/// 删除 - 根据Id - 直接删除
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<int> DelAsync(T model)
		{
			_dbContext.Set<T>().Attach(model); // 将实体以Unchanged状态添加到上下文
			_dbContext.Set<T>().Remove(model);
			return await _dbContext.SaveChangesAsync();
		}

		/// <summary>
		/// 删除 - 根据Id - 先查后删
		/// </summary>
		/// <param name="primaryKey"></param>
		/// <returns>成功: 1 失败: 异常</returns>
		public async Task<int> DelAsync(object primaryKey)
		{
			T model = await _dbContext.Set<T>().FindAsync(primaryKey);
			_dbContext.Set<T>().Remove(model);
			return await _dbContext.SaveChangesAsync();
		}
		#endregion

		#region 删 - 一个集合
		/// <summary>
		/// 删除 - 批量删除
		/// 先查出来 然后一条一条的删除
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public async Task<int> DelAsync(Expression<Func<T, bool>> predicate)
		{
			var set = _dbContext.Set<T>().Where(predicate);
			_dbContext.Set<T>().RemoveRange(set);
			return await _dbContext.SaveChangesAsync();
			//await _db.Set<T>().Where(predicate).ForEachAsync(m => _db.Set<T>().Remove(m));
			//return await _db.SaveChangesAsync();
		}
		#endregion

		#region 改 - 一个实体

		/// <summary>
		/// 改 - 一个实体 - 先查后改
		/// </summary>
		/// <param name="mdl">不需要主键, 只写要修改的字段</param>
		/// <param name="key">主键</param>
		/// <param name="updateProNames">要修改的属性名</param>
		/// <returns></returns>
		public async Task<int> UpdateAsync(object key, T mdl, params string[] updateProNames)
		{
			// 如果没写属性 - 异常
			if (updateProNames == null || updateProNames.Length < 1)
			{
				throw new Exception("请至少指定一个要更改的属性!");
			}
			// 获取类型对象
			Type t = mdl.GetType();
			// 从数据库获取指定的数据
			T entity = await _dbContext.Set<T>().FindAsync(key);
			if (entity == null)
			{
				throw new Exception("指定的要修改的实体在数据库中未找到!");
			}
			// 开始更改
			updateProNames.ToList().ForEach(p =>
			{
				PropertyInfo pro = t.GetProperty(p);
				if (pro == null)
				{
					throw new Exception("输入的更改列表中有拼写错误!");
				}
				object value = pro.GetValue(mdl, null);
				pro.SetValue(entity, value, null);
			});
			return await _dbContext.SaveChangesAsync();
		}

		#region 该 - 一个实体 - 直接更改

		// <summary>
		// 更改一条数据, 需指定更改的字段
		// 这个函数在第二次执行同一个主键的数据时会报异常, ObjectStatusManager只能同时跟踪一个主键相同的对象
		// 改成Virtual, 让派生类通过Id查找出来再去改把
		// </summary>
		// <param name="model">要更改的数据</param>
		// <param name="modifiedProNames">更改的字段名</param>
		// <returns></returns>
		//public virtual int Modify(T model, params string[] modifiedProNames)
		//{
		//	DbEntityEntry<T> entry = _db.Entry(model);
		//	entry.State = EntityState.Unchanged;
		//	foreach (string item in modifiedProNames)
		//	{
		//		entry.Property(item).IsModified = true;
		//	}
		//	if (modifiedProNames.Length < 1)
		//	{
		//		throw new Exception(GetType() + " : 请指定要修改的属性的名字!");
		//	}

		//	return _db.SaveChanges();
		//}

		#endregion

		#endregion

		#region 改 - 一个集合
		/// <summary>
		/// 改 - 一个集合 - 先查后改
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="mdl"></param>
		/// <param name="updateProNames"></param>
		/// <returns></returns>
		public async Task<int> UpdateAsync(Expression<Func<T, bool>> predicate, T mdl, params string[] updateProNames)
		{
			// 如果没写属性 - 异常
			if (updateProNames == null || updateProNames.Length < 1)
			{
				throw new Exception("请至少指定一个要更改的属性!");
			}
			// 获取类型对象
			Type t = mdl.GetType();
			// 从数据库获取指定的数据
			List<T> listEntity = await _dbContext.Set<T>().Where(predicate).ToListAsync();
			if (listEntity.Count < 1)
			{
				throw new Exception("指定的要修改的实体在数据库中未找到!");
			}
			// 开始更改
			updateProNames.ToList().ForEach(p =>
			{
				PropertyInfo pro = t.GetProperty(p);
				if (pro == null)
				{
					throw new Exception("输入的更改列表中有拼写错误!");
				}
				object value = pro.GetValue(mdl, null);
				listEntity.ForEach(m => pro.SetValue(m, value, null));
			});
			return await _dbContext.SaveChangesAsync();
		}
		#endregion

		#region 查 - 获取一个实体

		/// <summary>
		/// 获取实体 - 根据主键
		/// </summary>
		/// <param name="primaryKey">主键</param>
		/// <returns>实体/null</returns>
		public async Task<T> FindAsync(object primaryKey)
		{
			return await _dbContext.Set<T>().FindAsync(primaryKey);
		}

		/// <summary>
		/// 获取实体 - 唯一的实体
		/// </summary>
		/// <param name="predicate">给定一个谓词</param>
		/// <returns>实体/异常(零个/多个)</returns>
		public async Task<T> SingleAsync(Expression<Func<T, bool>> predicate)
		{
			return await _dbContext.Set<T>().SingleAsync(predicate);
		}

		/// <summary>
		/// 获取实体 - 唯一或不存在的实体
		/// </summary>
		/// <returns>实体(一个)/null(零个)/异常(多个)</returns>
		public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
		{
			return await _dbContext.Set<T>().SingleOrDefaultAsync(predicate);
		}

		/// <summary>
		/// 获取实体 - 第一个实体
		/// </summary>
		/// <param name="predicate">给定谓词</param>
		/// <returns>实体: 第一个; 异常: 零个</returns>
		public async Task<T> FirstAsync(Expression<Func<T, bool>> predicate)
		{
			return await _dbContext.Set<T>().FirstAsync(predicate);
		}

		/// <summary>
		/// 获取实体 - 排序 - 第一个实体
		/// </summary>
		/// <param name="predicate">给定谓词</param>
		/// <param name="order">排序</param>
		/// <returns>实体: 第一个; 异常: 零个</returns>
		public async Task<T> FirstAsync<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> order)
		{
			return await _dbContext.Set<T>().OrderBy(order).FirstAsync(predicate);
		}

		/// <summary>
		/// 获取实体 - 第一个或不存在的实体
		/// </summary>
		/// <returns>实体: 第一个; null: 零个</returns>
		public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
		{
			return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
		}

		/// <summary>
		/// 获取实体 - 按指定顺序排序 - 第一个或不存在的实体
		/// </summary>
		/// <returns>实体: 第一个; null: 零个</returns>
		public async Task<T> FirstOrDefaultAsync<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> order)
		{
			return await _dbContext.Set<T>().OrderBy(order).FirstOrDefaultAsync(predicate);
		}

		#endregion

		#region 查 - 获取实体集合
		/// <summary>
		/// 获取实体集合 - 根据条件
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate)
		{
			return await _dbContext.Set<T>().Where(predicate).ToListAsync();
			//不进行缓存的查询
			//return await _db.Set<T>().Where(predicate).AsNoTracking().ToListAsync();
		}

		/// <summary>
		/// 获取实体集合 - 排序查询
		/// </summary>
		/// <typeparam name="TKey">排序字段类型</typeparam>
		/// <param name="predicate">查询条件</param>
		/// <param name="orderLambda">排序条件</param>
		/// <returns></returns>
		public async Task<List<T>> GetListAsync<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderLambda)
		{
			return await _dbContext.Set<T>()
				.Where(predicate)
				.OrderBy(orderLambda)
				.ToListAsync();
		}

		/// <summary>
		/// 获取实体集合 - 分页查询
		/// </summary>
		/// <typeparam name="TKey">排序类型</typeparam>
		/// <param name="pageIndex">页码</param>
		/// <param name="pageSize">页容量</param>
		/// <param name="predicate">谓词</param>
		/// <param name="orderBy">排序lambda</param>
		/// <returns></returns>
		public async Task<List<T>> GetListAsync<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderBy)
		{
			return await _dbContext.Set<T>()
				.Where(predicate)
				.OrderBy(orderBy)
				.Skip((pageIndex - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();
		}

		///// <summary>
		///// 集合: 分页查询
		///// </summary>
		///// <param name="pageIndex">页码</param>
		///// <param name="pageSize">页容量</param>
		///// <param name="orderBy">排序lambda表达式</param>
		///// <param name="whereLambda">条件lambda表达式</param>
		///// <param name="total">返回数据的总条数</param>
		///// <returns></returns>
		//public List<T> GetPageList<TKey>(int pageIndex, int pageSize, Expression<Func<T, TKey>> orderBy, Expression<Func<T, bool>> whereLambda, out int total)
		//{
		//	total = _db.Set<T>().Where(whereLambda).Count();
		//	return _db.Set<T>().Where(whereLambda).OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
		//} 
		#endregion

		#region 查 - 获取数据库时间

		/// <summary>
		/// 获取数据库时间
		/// </summary>
		/// <returns></returns>
		public async Task<DateTime> GetTimeAsync()
		{
			// 加上.FirstOrDefault是为了针对延迟加载
			return await Task.Run(() => _dbContext.Database.SqlQuery<DateTime>("SELECT GETDATE()").FirstOrDefault());
		}

		#endregion 
		#endregion
	}
}
