// ReSharper disable InconsistentNaming
using System.Runtime.Remoting.Messaging;

namespace Bll
{
	// ReSharper disable once PartialTypeWithSinglePart
	public partial class BllSession
	{
		#region  01. TblClassBll
		TblClassBll _TblClassBll;
		public TblClassBll TblClassBll
		{ 
			get { return _TblClassBll ?? (_TblClassBll = new TblClassBll()); }
			set	{ _TblClassBll = value; }
		}
		#endregion

		#region  02. TblDormBll
		TblDormBll _TblDormBll;
		public TblDormBll TblDormBll
		{ 
			get { return _TblDormBll ?? (_TblDormBll = new TblDormBll()); }
			set	{ _TblDormBll = value; }
		}
		#endregion

		#region  03. TblStudentBll
		TblStudentBll _TblStudentBll;
		public TblStudentBll TblStudentBll
		{ 
			get { return _TblStudentBll ?? (_TblStudentBll = new TblStudentBll()); }
			set	{ _TblStudentBll = value; }
		}
		#endregion

		   

		#region	Current
		public static BllSession Current
		{
			get
			{
				var current = CallContext.GetData(typeof(BllSession).Name) as BllSession;
				if (current == null)
				{
					current = new BllSession();
					CallContext.SetData(typeof(BllSession).Name, current);
				}
				return current;
			}
		}
		#endregion

		#region	Private Metohd
		private BllSession(){}
		#endregion
	}
}