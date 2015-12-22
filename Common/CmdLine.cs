using System;
using System.Diagnostics;
using System.Text;

namespace Common
{
	public class CmdLine
	{
		public static string Excute(string filename, string args = "")
		{
			var info = new ProcessStartInfo
			{
				FileName = filename,
				Arguments = args, // 空参数
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true,  // 防止黑框闪动
			};
			var p = Process.Start(info);
			if (p == null)
			{
				throw new Exception("Cmd命令执行出错了!");
			}
			return p.StandardOutput.ReadToEnd();
		}
	}

	#region CMD - 异步输出
	/// <summary>
	/// 执行命令行程序, 异步读取输出(StandardOutput/StandardError)
	/// MSDN: https://msdn.microsoft.com/en-us/library/system.diagnostics.process.beginerrorreadline.aspx
	/// StackOverfollow: http://stackoverflow.com/questions/1707516/c-sharp-and-ffmpeg-preferably-without-shell-commands
	/// </summary>
	public class CmdLineAsync
	{
		private readonly StringBuilder _standardOutput = new StringBuilder();
		private readonly StringBuilder _errorOutput = new StringBuilder();
		private Process _process;

		public string Output { get; set; }
		public string StandardOutput { get; set; }
		public string ErrorOutput { get; set; }
		public Process Process => _process;

		public string Excute(string fullname, string cmdstr, DataReceivedEventHandler outputDataReceived = null, DataReceivedEventHandler errorDataReceived = null)
		{
			_process = new Process
			{
				StartInfo =
				{
					FileName = fullname,
					Arguments = cmdstr,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
				}
			};
			// 标准输出的处理程序
			if (outputDataReceived == null)
			{
				_process.OutputDataReceived += process_OutputDataReceived;
			}
			else
			{
				_process.OutputDataReceived += outputDataReceived;
			}

			// 错误输出的处理程序
			if (errorDataReceived == null)
			{
				_process.ErrorDataReceived += process_ErrorDataReceived;
			}
			else
			{
				_process.ErrorDataReceived += errorDataReceived;
			}

			// 开始执行
			Console.WriteLine("启动命令行程序...");
			_process.Start();
			// 开始异步读取标准输出
			_process.BeginOutputReadLine();
			// 开始异步读取错误输出
			_process.BeginErrorReadLine();
			// 让命令行执行, 收集输出
			Console.WriteLine("正在收集输出...");
			_process.WaitForExit();
			Console.WriteLine("收集完毕...");
			// 关闭命令行程序
			Console.WriteLine("关闭程序...");
			_process.Close();
			Console.WriteLine("程序执行完成!");

			if (outputDataReceived == null && errorDataReceived == null)
			{
				return "StandardOutput: " + _standardOutput.ToString() +
					"ErrorOutput: " + _errorOutput.ToString();
			}
			if (errorDataReceived != null && outputDataReceived == null)
			{
				return "StandardOutput: " + _standardOutput.ToString();
			}
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			else if (outputDataReceived != null && errorDataReceived == null)
			{
				return "ErrorOutput: " + _errorOutput.ToString();
			}
			else
			{
				return "StandardOutput: " + _standardOutput.ToString() +
					"ErrorOutput: " + _errorOutput.ToString(); ;
			}
		}

		private void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Data))
			{
				_errorOutput.AppendLine(e.Data);
			}
		}

		private void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Data))
			{
				_standardOutput.AppendLine(e.Data);
			}
		}
	}
	#endregion
}
