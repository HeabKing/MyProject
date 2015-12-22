using System;
using System.Web.Security;  // System.Web;

namespace Common
{
    public class SecurityHelper
    {
        #region 1. static string Md5(string str) - 返回Md5字符串
        /// <summary>
        /// 返回 MD5 加密字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Md5(string str)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
        } 
        #endregion

        #region 2. 加密解密
        #region 1. static string EncryptUserInfo(string userInfo)
        /// <summary>
        /// 使用票据 对象 将用户数据加密成字符串 加密是跟设备相关的, 在别的设备上无法解密
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static string EncryptUserInfo(string userInfo)
        {
            // 1 将用户数据存入票据对象
            var ticket = new FormsAuthenticationTicket(1, "哈哈", DateTime.Now, DateTime.Now, true, userInfo);
            // 2. 将票据对象 加密成字符串(可逆)
            string strData = FormsAuthentication.Encrypt(ticket);
            return strData;
        }
        #endregion
        #region 2. 解密 加密的字符串
        /// <summary>
        /// 加密字符串 解密
        /// </summary>
        /// <param name="cryptograph">加密字符串</param>
        /// <returns></returns>
        public static string DecryptUserInfo(string cryptograph)
        {
            // 1. 将加密字符串 解压成 票据对象
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cryptograph);
            // 2. 将 票据对象里面的 用户数据 返回
            if (ticket != null)
                return ticket.UserData;
            return null;
        }
        #endregion 
        #endregion
    }
}
