using System;
using System.IO;
using Microsoft.Win32;

namespace Common
{
    /// <summary>
    /// 开机自启动
    /// </summary>
    public class AutoStart
    {
        public static void SetAutoStart(string fullName, bool isStart = true)
        {

            if (!File.Exists(fullName))
            {
                throw new Exception("指定的文件 " + fullName + " 不存在!");
            }
            string fileName = Path.GetFileNameWithoutExtension(fullName);
            //打开注册表子项
            //如果该项不存在的话，则创建该子项
            string temp = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            var key = Registry.LocalMachine.OpenSubKey(temp, true) ??
                              Registry.LocalMachine.CreateSubKey(temp);

            if (isStart)
            {
                try
                {
                    if (key != null)
                    {
                        key.SetValue(fileName, fullName);   //设置为开机启动
                        key.Close();
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception("写入开机自启动注册表失败!", ex);
                }
            }
            else
            {
                try
                {
                    if (key != null)
                    {
                        key.DeleteValue(fileName);  //取消开机启动
                        key.Close();
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception("删除开机自启动注册表信息失败!", ex);
                }
            }
        }

        public static void SetThisAutoStart()
        {
            var temp = AppDomain.CurrentDomain.SetupInformation;
            string filename = temp.ApplicationBase + temp.ApplicationName;
            SetAutoStart(filename);
        }
    }
}
