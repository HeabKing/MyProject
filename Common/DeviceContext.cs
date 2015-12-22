using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Common
{
	/// <summary>
	/// 计算机环境类
	/// </summary>
	public class DeviceContext
	{
		#region 判断 - 连网
		[DllImport("wininet")]
		private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

		public static bool IsOnLine()
		{
			int i;
			return InternetGetConnectedState(out i, 0);
		}
		#endregion

		#region 获取 - 鼠标位置

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern bool GetCursorPos(out Point point);

		public static Point MousePoint
		{
			get
			{
				Point point;
				GetCursorPos(out point);
				return point;
			}
		}

		#endregion

		/// <summary>
		/// 获取 - 鼠标多长时间没动了 注意: 多次开启不会影响结果
		/// </summary>
		public class MouseHoldTime
		{
			private readonly System.Timers.Timer _timer = new System.Timers.Timer();
			private Point _oldPoint;            // 鼠标原始位置
			private DateTime _oldPointSetTime;  // 鼠标原始位置设置时间

			/// <summary>
			/// 开启/关闭
			/// </summary>
			public bool Enable
			{
				get
				{
					return _timer.Enabled;
				}
				set
				{
					// 使得多次开启不会重置
					if (_timer.Enabled && value)
					{
						return;
					}
					if (value)
					{
						_timer.Interval = 1000;	// 默认是1秒
						_timer.Elapsed += _timer_Elapsed;
						_timer.Enabled = true;  // 启动 跟 _timer.Start() 一个效果
						_oldPoint = MousePoint;
						_oldPointSetTime = DateTime.Now;
					}
					else
					{
						_timer.Enabled = false;
					}
				}
			}

			private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
			{
				Point nowPoint = MousePoint;
				if (nowPoint == _oldPoint) return;
				_oldPoint = nowPoint;
				_oldPointSetTime = DateTime.Now;
			}

			/// <summary>
			/// 获取 - 鼠标停止的时间
			/// </summary>
			public TimeSpan Time
			{
				get
				{
					if (!_timer.Enabled)
					{
						return new TimeSpan(0);
					}
					else
					{
						return DateTime.Now - _oldPointSetTime;
					}
				}
			}

			/// <summary>
			/// 获取/设置 - 检测鼠标状态的时间间隔
			/// </summary>
			public double Interval {
				get
				{
					return _timer.Interval;
				}
				set
				{
					_timer.Interval = value;
				}
			}

			#region Current 单例模式
			private MouseHoldTime() { }

			private static MouseHoldTime _current;

			public static MouseHoldTime Current
			{
				get
				{
					if (_current != null)
					{
						return _current;
					}
					_current = new MouseHoldTime();
					return _current;
				}
			} 
			#endregion
		}

		#region 获取 - 设备所有网卡的Mac地址
		/// <summary>
		/// 获取 - 设备所有网卡的Mac地址
		/// </summary>
		/// <returns></returns>
		public static string[] GetGpuMacs()
		{
			string result = CmdLine.Excute("ipconfig", "/all");
			Regex reg = new Regex(" ([A-Za-z0-9]{2}-){5}[A-Za-z0-9]{2}\r\n");
			var set = reg.Matches(result);

			return (from Match item in set select item.Value.Substring(1, 17)).ToArray();
		} 
		#endregion
	}
}
