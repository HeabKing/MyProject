using System;
namespace Common
{
    public static class CmdCommand
    {
		/// <summary>
		/// 这个方法在执行ffmpeg -list_devices true -f dshow -i dummy时候, 返回""
		/// 但是对于 程序名 + 参数(一个)的支持还是很好的
		/// </summary>
		/// <param name="filename">程序名</param>
		/// <param name="args">参数</param>
		/// <returns></returns>
        public static string Excute(string filename, string args="")
        {
			var info = new System.Diagnostics.ProcessStartInfo
            {
                FileName = filename,
				Arguments = "/C " + args, // 空参数
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,  // 防止黑框闪动
            };
			var p = System.Diagnostics.Process.Start(info);
            if (p == null)
            {
                throw new Exception("Cmd命令执行出错了!");
            }
            return p.StandardOutput.ReadToEnd();
        }

	    public static string Excute2(string strCmd)
	    {
		    System.Diagnostics.Process p = new System.Diagnostics.Process
		    {
			    StartInfo =
			    {
					FileName = "cmd.exe",
					UseShellExecute = false,		// 是否使用操作系统shell启动
					RedirectStandardInput = true,	// 接受来自调用程序的输入信息
					RedirectStandardOutput = true,	// 由调用程序获取输出信息
					RedirectStandardError = true,	// 重定向标准错误输出
				    CreateNoWindow = true			// 是否显示黑框框
			    }
		    };
		    
		    p.Start();//启动程序

			//向cmd窗口发送输入信息
			p.StandardInput.WriteLine(strCmd);
			p.StandardInput.WriteLine("exit");//要得加上Exit要不然下一行程式 
			p.StandardInput.AutoFlush = true;
			//p.StandardInput.WriteLine("exit");
			//向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
			//同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令



			//获取cmd窗口的输出信息
			string output = p.StandardOutput.ReadToEnd();

			//StreamReader reader = p.StandardOutput;
			//string line=reader.ReadLine();
			//while (!reader.EndOfStream)
			//{
			//    str += line + "  ";
			//    line = reader.ReadLine();
			//}

			p.WaitForExit();//等待程序执行完退出进程
			p.Close();
			return output;
	    }
    }
}
