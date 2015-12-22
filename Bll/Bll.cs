namespace Bll
{
	public partial class TblClassBll : BaseBll<Model.TblClass>
	{
		protected override void SetDal()
		{
			XxDal = Dal.DalSession.Current.TblClassDal;
		}
		
		//public System.DateTime GetDbDateTime()
		//{
			//return XxDal.GetDbDateTime();
		//}
	}
		
	public partial class TblDormBll : BaseBll<Model.TblDorm>
	{
		protected override void SetDal()
		{
			XxDal = Dal.DalSession.Current.TblDormDal;
		}
		
		//public System.DateTime GetDbDateTime()
		//{
			//return XxDal.GetDbDateTime();
		//}
	}
		
	public partial class TblStudentBll : BaseBll<Model.TblStudent>
	{
		protected override void SetDal()
		{
			XxDal = Dal.DalSession.Current.TblStudentDal;
		}
		
		//public System.DateTime GetDbDateTime()
		//{
			//return XxDal.GetDbDateTime();
		//}
	}
		
}