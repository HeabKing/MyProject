using System.IO;
using System.Web;
using System.Xml.Serialization;

namespace Common
{
	public class ConfigHelper<T> where T : class, new()
	{
		public static T GetConfig(string path = "~/Config/Config.ini")
		{
			path = HttpContext.Current.Server.MapPath(path);
			using (var sr = new StreamReader(path))
			{
				var serializer = new XmlSerializer(typeof(T));
				return (T)serializer.Deserialize(sr);
			}
		}

		public void SaveConfig(string path = "~/Config/Config.ini")
		{
			path = HttpContext.Current.Server.MapPath(path);
			using (var sw = new StreamWriter(path))
			{
				var serializer = new XmlSerializer(typeof(T));
				serializer.Serialize(sw, this);
			}
		}
	}
}
