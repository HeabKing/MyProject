// ReSharper disable InconsistentNaming
using System.Runtime.Remoting.Messaging;


namespace Dal
{
	/// <summary>
	/// 1. 线程共享 2. 不用每次new表实体 3. 有条理
	/// </summary>
	public class DalSession
	{
		#region  01. TblClassDal
		TblClassDal _TblClassDal;
		public TblClassDal TblClassDal => _TblClassDal ?? (_TblClassDal = new TblClassDal()); 
	
		#endregion

		#region  02. TblDormDal
		TblDormDal _TblDormDal;
		public TblDormDal TblDormDal => _TblDormDal ?? (_TblDormDal = new TblDormDal()); 
	
		#endregion

		#region  03. TblStudentDal
		TblStudentDal _TblStudentDal;
		public TblStudentDal TblStudentDal => _TblStudentDal ?? (_TblStudentDal = new TblStudentDal()); 
	
		#endregion

		   

		#region	Current
		public static DalSession Current
		{
			get
			{
				var current = CallContext.GetData(typeof(DalSession).Name) as DalSession;
				if (current != null) return current;
				current = new DalSession();
				CallContext.SetData(typeof(DalSession).Name, current);
				return current;
			}
		}
		#endregion

		#region	Private Metohd
		private DalSession(){}
		#endregion
	}
}