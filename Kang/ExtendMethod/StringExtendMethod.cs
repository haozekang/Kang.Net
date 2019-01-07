using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kang.ExtendMethod
{
    /// <summary>
    /// 字符串扩展方法
    /// </summary>
    public static class StringExtendMethod
    {
        /// <summary>
        /// 替换字符串中所有出现过的子字符串
        /// </summary>
        /// <param name="oldString"></param>
        /// <param name="newString"></param>
        /// <returns></returns>
        public static string ReplaceAll(this String str, List<String> oldString, List<String> newString)
        {
            if (oldString.Count > 0 && newString.Count > 0 && oldString.Count == newString.Count)
            {
                for (int i = 0,count = oldString.Count; i < count;i++)
                {
                    str = str.Replace(oldString[i],newString[i]);
                }
            }
            return str;
        }

        /// <summary>
        /// 替换字符串中所有出现过的子字符串
        /// </summary>
        /// <param name="oldString"></param>
        /// <param name="newString"></param>
        /// <returns></returns>
        public static string ReplaceAll(this String str, String[] oldString, String[] newString)
        {
            if (oldString.Length > 0 && newString.Length > 0 && oldString.Length == newString.Length)
            {
                for (int i = 0, count = oldString.Length; i < count; i++)
                {
                    str = str.Replace(oldString[i], newString[i]);
                }
            }
            return str;
        }
    }
}
