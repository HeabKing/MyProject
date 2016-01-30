using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	public static class DynamicFactory
	{
		// ConcurrentDictionary 表示可由多个线程同时访问的键值对的线程安全集合。
		private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, Type> SDynamicTypes = new System.Collections.Concurrent.ConcurrentDictionary<Type, Type>();    // Concurrent 并行, 同时发生

		private static readonly Func<Type, Type> SDynamicTypeCreator = CreateDynamicType;

		/// <summary>
		/// 将任意类型转换成动态类型 (任意类型/匿名类型 -> 确定类型 -> dynamic)
		/// 此动态类型会根据输入对象中的属性信息, 生成对应的公有字段, 然后使用反射进行赋值
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public static dynamic ToDynamic(this object entity)
		{
			var entityType = entity.GetType();  // 获取任意类型的类型, 如果是匿名类型(如: <>f__AnonymousType0`1), 他的作用域是internal
			var dynamicType = SDynamicTypes.GetOrAdd(entityType, SDynamicTypeCreator);  // 如果指定的键尚不存在，则将键/值对添加到 ConcurrentDictionary<TKey, TValue> 中。

			var dynamicObject = Activator.CreateInstance(dynamicType);
			foreach (var entityProperty in entityType.GetProperties())
			{
				var value = entityProperty.GetValue(entity, null);
				dynamicType.GetField(entityProperty.Name).SetValue(dynamicObject, value);
			}

			return dynamicObject;
		}

		private static Type CreateDynamicType(Type entityType)
		{
			var asmName = new System.Reflection.AssemblyName("DynamicAssembly_" + Guid.NewGuid());
			var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, System.Reflection.Emit.AssemblyBuilderAccess.Run);
			var moduleBuilder = asmBuilder.DefineDynamicModule("DynamicModule_" + Guid.NewGuid());

			var typeBuilder = moduleBuilder.DefineType(
				entityType + "$DynamicType",
				System.Reflection.TypeAttributes.Public);

			typeBuilder.DefineDefaultConstructor(System.Reflection.MethodAttributes.Public);

			foreach (var entityProperty in entityType.GetProperties())
			{
				typeBuilder.DefineField(entityProperty.Name, entityProperty.PropertyType, System.Reflection.FieldAttributes.Public);
			}

			return typeBuilder.CreateType();
		}
	}
}
