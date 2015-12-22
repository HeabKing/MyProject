using System;
using System.IO;

namespace Common
{
	public class FileHelper
	{
		/// <summary>
		///     复制文件夹（及文件夹下所有子文件夹和文件）
		///		把源文件夹中的所有内容复制到目标文件夹中
		/// </summary>
		/// <param name="sourcePath">待复制的文件夹路径 如果路径不存在报异常DirectoryNotFound</param>
		/// <param name="destinationPath">目标路径 如果路径不存在则自动创建</param>
		public static void CopyDirectory(string sourcePath, string destinationPath)
		{
			var info = new DirectoryInfo(sourcePath);
			Directory.CreateDirectory(destinationPath);
			foreach (var fsi in info.GetFileSystemInfos())
			{
				var destName = Path.Combine(destinationPath, fsi.Name);

				if (fsi is FileInfo) //如果是文件，复制文件
				{
					try
					{
						File.Copy(fsi.FullName, destName);
					}
					catch (Exception ex)
					{
						if (ex.Message.Contains("已经存在"))
						{
						}
						else
						{
							throw;
						}
					}
				}
				else //如果是文件夹，新建文件夹，递归
				{
					Directory.CreateDirectory(destName);
					CopyDirectory(fsi.FullName, destName);
				}
			}
		}

		/// <summary>
		///     删除文件夹（及文件夹下所有子文件夹和文件）
		/// </summary>
		/// <param name="directoryPath"></param>
		public static void DeleteFolder(string directoryPath)
		{
			foreach (var d in Directory.GetFileSystemEntries(directoryPath))
			{
				if (File.Exists(d))
				{
					var fi = new FileInfo(d);
					if (fi.Attributes.ToString().IndexOf("ReadOnly", StringComparison.Ordinal) != -1)
						fi.Attributes = FileAttributes.Normal;
					File.Delete(d); //删除文件   
				}
				else
					DeleteFolder(d); //删除文件夹
			}
			Directory.Delete(directoryPath); //删除空文件夹
		}

		/// <summary>
		///     获取带文件路径的文件名（如“D:\AML\JHG\abc.txt”，获取abc.txt）
		/// </summary>
		/// <param name="filePath">文件名</param>
		/// <returns></returns>
		public static string GetFileName(string filePath)
		{
			//传进来的 filePath 应 filePath.TrimEnd('\\')
			//该方法也可以用split('\\')，但是建议用 LastIndexOf
			var dirNameIndex = filePath.LastIndexOf("\\", StringComparison.Ordinal);
			if (dirNameIndex != -1)
				return filePath.Substring(dirNameIndex + 1);
			return "error path";
		}

		/// <summary>
		///     以二进制流方式读取文件
		/// </summary>
		/// <param name="filePath">文件全路径（如：D:\AML\JHG\abc.txt）</param>
		/// <returns></returns>
		public static byte[] ReadFileByte(string filePath)
		{
			Stream fileStream = File.OpenRead(filePath);
			var arrBytes = new byte[fileStream.Length];
			var offset = 0;
			while (offset < arrBytes.LongLength)
			{
				offset += fileStream.Read(arrBytes, offset, arrBytes.Length - offset);
			}
			fileStream.Close();

			return arrBytes;
		}
	}
}