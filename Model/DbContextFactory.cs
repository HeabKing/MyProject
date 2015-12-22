using System.Data.Entity;
using System.Runtime.Remoting.Messaging;

namespace Model
{
	/// <summary>
	/// 工厂 - 保证 DbContext 线程唯一
	/// </summary>
	public static class DbContextFactory
	{
		/// <summary>
		/// EF数据库访问上下文
		/// </summary>
		public static DbContext Context => GetDbContext();

		private static DbContext GetDbContext()
		{
			var dbContext = CallContext.GetData(typeof(DbContextFactory).Name + "dbContext") as
			DbContext;
			if (dbContext != null) return dbContext;

			dbContext = new DbEntities();   // 数据库实体
			//dbContext.Configuration.ValidateOnSaveEnabled = false;	// 实体验证 TODO
			CallContext.SetData(typeof(DbContextFactory).Name + "dbContext", dbContext);

			return dbContext;
		}
	}
}
