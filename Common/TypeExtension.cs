using System;
using System.Reflection;

namespace Common
{
	/// <summary>
	/// 通过字符串属性名 获取/设置 对象的属性
	/// </summary>
	public static class ExtensinonType
	{
		#region Get

		/// <summary>
		/// 获取属性值 - 指定属性名称 - 字符串形式
		/// </summary>
		/// <param name="p">实体</param>
		/// <param name="proName">实体的属性名</param>
		/// <returns>实体对应属性名的值</returns>
		public static object GetPropertyValue<T>(this T p, string proName)
		{
			Type t = p.GetType();
			PropertyInfo proinfo = t.GetProperty(proName);
			if (proinfo == null)
			{
				throw new Exception("指定的属性在类中不存在!");
			}
			return proinfo.GetValue(p, null);

			#region 1.0版本

			//// 获取实体类 类型对象
			//Type t = p.GetType();   // typeof(Person);
			//						// 获取 实体类 所有 公有属性
			//List<PropertyInfo> proInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
			//// 获取 指定属性 对象
			//PropertyInfo rightPro = proInfos.FirstOrDefault(pro => pro.Name == proName);
			//if (rightPro == null)
			//{
			//	throw new Exception("指定的属性名" + proName + "在实体中不存在!");
			//}
			//// 返回指定属性对应的值
			//return rightPro.GetValue(p, null);

			#endregion		}
		}

		#endregion

		#region Set

		/// <summary>
		/// 设置属性值 - 指定属性名 - 字符串形式
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="p">实体</param>
		/// <param name="newValue">赋值给实体指定属性的值</param>
		/// <param name="proName">实体属性的名字</param>
		public static void SetPropertyValue<T>(this T p, string proName, object newValue)
		{
			Type t = p.GetType();
			PropertyInfo proinfo = t.GetProperty(proName);
			if (proinfo == null)
			{
				throw new Exception("指定的属性名不正确!");
			}
			proinfo.SetValue(p, newValue, null);

			#region 1.0版本

			//// 获取实体类 类型对象
			//Type t = p.GetType();   // typeof(Person);
			//						// 获取 实体类 所有 公有属性
			//List<PropertyInfo> proInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
			//// 获取 指定属性 对象
			//PropertyInfo rightPro = proInfos.FirstOrDefault(pro => pro.Name == proName);
			//if (rightPro == null)
			//{
			//	throw new Exception("指定的属性名" + proName + "在实体中不存在!");
			//}
			//rightPro.SetValue(p, newValue, null);   // null 不加上有的时候会出现多个候选方法, 从而导致出错 

			#endregion
		}

		#endregion
	}
}
