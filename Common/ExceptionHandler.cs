using System;
using System.Collections.Generic;

namespace Common
{
    public class ExceptionHandler
    {
        /// <summary>
        /// 显示Exception中所有的消息(包括所有源Exception)
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetAllMessage(Exception ex)
        {
            string message = "Exception: (" + ex.GetType().Name + " in " + ex.TargetSite + " 方法)" + ex.Message;
            while (ex.InnerException != null)
            {
                message += ("    \nInnerException: ("+ ex.GetType().Name + " in " + ex.TargetSite +" 方法)" + ex.InnerException.Message);
                ex = ex.InnerException;
            }
            return message;
        }

        /// <summary>
        /// 获取所有的异常
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static List<Exception> GetAll(Exception ex)
        {
            List<Exception> list = new List<Exception> {ex};
            while (ex.InnerException != null)
            {
                list.Add(ex.InnerException);
                ex = ex.InnerException;
            }
            return list;
        }
    }
}
