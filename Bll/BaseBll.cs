using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Bll
{
	/// <summary>
	/// 业务层父类
	/// </summary>
	public abstract class BaseBll<T> where T : class, new()
	{
		#region 控制反转相关代码 => Dal.BaseDal<T> XxDal
		protected BaseBll()
		{
			// ReSharper disable once DoNotCallOverridableMethodsInConstructor
			SetDal();
		}

		/// <summary>
		/// 数据层接口对象 - 等待被实例化 - 这里直接new也可以, 但是这样就不是一个线程共享一个事例了
		/// 所以要等待子类进行实例化, 实现控制反转
		/// </summary>
		protected Dal.BaseDal<T> XxDal;

		/// <summary>
		/// 由子类实现, 为 业务父类 的 数据接口对象 设置值
		/// </summary>
		protected abstract void SetDal();
		#endregion

		#region 增 - 一个实体

		/// <summary>
		/// 添加实体 - 一个
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public T Add(T model)
		{
			return XxDal.Add(model);
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
			return XxDal.Del(model);
		}

		/// <summary>
		/// 删除 - 根据Id - 先查后删
		/// </summary>
		/// <param name="primaryKey"></param>
		/// <returns>成功: 1 失败: 异常</returns>
		public int Del(object primaryKey)
		{
			return XxDal.Del(primaryKey);
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
			return XxDal.Del(predicate);
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
			return XxDal.Update(key, mdl, updateProNames);
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
			return XxDal.Update(predicate, mdl, updateProNames);
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
			return XxDal.Find(primaryKey);
		}

		/// <summary>
		/// 获取实体 - 唯一的实体
		/// </summary>
		/// <param name="predicate">给定一个谓词</param>
		/// <returns>实体/异常(零个/多个)</returns>
		public T Single(Expression<Func<T, bool>> predicate)
		{
			return XxDal.Single(predicate);
		}

		/// <summary>
		/// 获取实体 - 唯一或不存在的实体
		/// </summary>
		/// <returns>实体(一个)/null(零个)/异常(多个)</returns>
		public T SingleOrDefault(Expression<Func<T, bool>> predicate)
		{
			return XxDal.SingleOrDefault(predicate);
		}

		/// <summary>
		/// 获取实体 - 第一个实体
		/// </summary>
		/// <param name="predicate">给定谓词</param>
		/// <returns>实体: 第一个; 异常: 零个</returns>
		public T First(Expression<Func<T, bool>> predicate)
		{
			return XxDal.First(predicate);
		}

		/// <summary>
		/// 获取实体 - 排序 - 第一个实体
		/// </summary>
		/// <param name="predicate">给定谓词</param>
		/// <param name="order">排序</param>
		/// <returns>实体: 第一个; 异常: 零个</returns>
		public T First<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> order)
		{
			return XxDal.First(predicate, order);
		}

		/// <summary>
		/// 获取实体 - 第一个或不存在的实体
		/// </summary>
		/// <returns>实体: 第一个; null: 零个</returns>
		public T FirstOrDefault(Expression<Func<T, bool>> predicate)
		{
			return XxDal.FirstOrDefault(predicate);
		}

		/// <summary>
		/// 获取实体 - 按指定顺序排序 - 第一个或不存在的实体
		/// </summary>
		/// <returns>实体: 第一个; null: 零个</returns>
		public T FirstOrDefault<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> order)
		{
			return XxDal.FirstOrDefault(predicate, order);
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
			return XxDal.GetList(predicate);
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
			return XxDal.GetList(predicate, orderLambda);
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
			return XxDal.GetList(pageIndex, pageSize, predicate, orderBy);
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
			return XxDal.GetTime();
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
			return await XxDal.AddAsync(model);
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
			return await XxDal.DelAsync(model);
		}

		/// <summary>
		/// 删除 - 根据Id - 先查后删
		/// </summary>
		/// <param name="primaryKey"></param>
		/// <returns>成功: 1 失败: 异常</returns>
		public async Task<int> DelAsync(object primaryKey)
		{
			return await XxDal.DelAsync(primaryKey);
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
			return await XxDal.DelAsync(predicate);
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
			return await XxDal.UpdateAsync(key, mdl, updateProNames);
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
			return await XxDal.UpdateAsync(predicate, mdl, updateProNames);
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
			return await XxDal.FindAsync(primaryKey);
		}

		/// <summary>
		/// 获取实体 - 唯一的实体
		/// </summary>
		/// <param name="predicate">给定一个谓词</param>
		/// <returns>实体/异常(零个/多个)</returns>
		public async Task<T> SingleAsync(Expression<Func<T, bool>> predicate)
		{
			return await XxDal.SingleAsync(predicate);
		}

		/// <summary>
		/// 获取实体 - 唯一或不存在的实体
		/// </summary>
		/// <returns>实体(一个)/null(零个)/异常(多个)</returns>
		public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
		{
			return await XxDal.SingleOrDefaultAsync(predicate);
		}

		/// <summary>
		/// 获取实体 - 第一个实体
		/// </summary>
		/// <param name="predicate">给定谓词</param>
		/// <returns>实体: 第一个; 异常: 零个</returns>
		public async Task<T> FirstAsync(Expression<Func<T, bool>> predicate)
		{
			return await XxDal.FirstAsync(predicate);
		}

		/// <summary>
		/// 获取实体 - 排序 - 第一个实体
		/// </summary>
		/// <param name="predicate">给定谓词</param>
		/// <param name="order">排序</param>
		/// <returns>实体: 第一个; 异常: 零个</returns>
		public async Task<T> FirstAsync<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> order)
		{
			return await XxDal.FirstAsync(predicate, order);
		}

		/// <summary>
		/// 获取实体 - 第一个或不存在的实体
		/// </summary>
		/// <returns>实体: 第一个; null: 零个</returns>
		public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
		{
			return await XxDal.FirstOrDefaultAsync(predicate);
		}

		/// <summary>
		/// 获取实体 - 按指定顺序排序 - 第一个或不存在的实体
		/// </summary>
		/// <returns>实体: 第一个; null: 零个</returns>
		public async Task<T> FirstOrDefaultAsync<TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> order)
		{
			return await XxDal.FirstOrDefaultAsync(predicate, order);
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
			return await XxDal.GetListAsync(predicate);
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
			return await XxDal.GetListAsync(predicate, orderLambda);
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
			return await XxDal.GetListAsync(pageIndex, pageSize, predicate, orderBy);
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
			return await XxDal.GetTimeAsync();
		}

		#endregion 

		#endregion
	}
}
