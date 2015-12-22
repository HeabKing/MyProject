using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.ModelBinding;

// ReSharper disable once CheckNamespace
namespace System.Web.Http.ModelBinding // 这里把命名空间弄得跟要拓展的类的命名空间一样的话就不用引用这个命名空间了
{
	public static class ModeStateExtension
	{
		// ModelState的拓展方法, 用来直接获取客户端传给Mvc的数据的错误
		public static string GetErrors(this ModelStateDictionary modelstate)
		{
			StringBuilder sb = new StringBuilder();
			if (!modelstate.IsValid)
			{
				List<string> keys = modelstate.Keys.ToList();
				foreach (var key in keys)
				{
					var errors = modelstate[key].Errors.ToList();
					int i = 0;
					foreach (var error in errors)
					{
						sb.Append($"[Error {i++} : {key} - {error.ErrorMessage}] ");
					}
				}
			}
			else
			{
				sb.Append("ok - 没有错误!");
			}
			return sb.ToString();
		}
	}
}
