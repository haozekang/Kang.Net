using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace Kang.Util
{
    /// <summary>
    /// 字符串处理工具集
    /// </summary>
    public class StringUtil
    {
        /// <summary>
        /// 判断字符串是否为空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Boolean isBlank(String str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return true;
            }
            if (str.Length == 0)
            {
                return true;
            }
            if (str == "")
            {
                return true;
            }
            if (string.Empty == str)
            {
                return true;
            }
            if (string.IsNullOrEmpty(str))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断字符串是否不为空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Boolean isNotBlank(String str)
        {
            return !isBlank(str);
        }

        /// <summary>
        /// 从字符串后面截取一定长度的字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static String getSubStringFromEnd(String str,Int32 len)
        {
            if (isBlank(str))
                return null;
            if (len <= 0)
                return null;
            if (len >= str.Length)
                return str;
            else
                return str.Substring(str.Length - len);
        }

        /// <summary>
        /// 将数组转换为字符串
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static String changeArrayToString(String[] arr)
        {
            return String.Join("", arr);
        }

        /// <summary>
        /// 将数组转换为字符串，以String间隔
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="join"></param>
        /// <returns></returns>
        public static String changeArrayToString(String[] arr, String join)
        {
            return String.Join(join, arr);
        }

        /// <summary>
        /// 将字符串数组转换为List
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static List<String> changeArrayToList(String[] arr)
        {
            return arr.ToList();
        }

        /// <summary>
        /// 将字符串List转换为数组
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static String[] changeListToArray(List<String> arr)
        {
            return arr.ToArray();
        }

        /// <summary>
        /// 判断字符串数组中是否存在特定的字符串
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Boolean stringArrayHaveString(String[] arr,String str)
        {
            if (str == null)
            {
                return arr.Where(x => x == null).Select(x => x).ToArray().Count() > 0 ? true : false;
            }else
            {
                return arr.Where(x => str.Equals(x)).Select(x => x).ToArray().Count() > 0 ? true : false;
            }
        }

        /// <summary>
        /// Unicode转中文（符合js规则的）
        /// </summary>
        /// <returns></returns>
        public static string EncodingUnicode(string str)
        {
            string outStr = "";
            Regex reg = new Regex(@"(?i)\\u([0-9a-fA-Z]{4})");
            outStr = reg.Replace(str, delegate (Match m1)
            {
                return ((char)Convert.ToInt32(m1.Groups[1].Value, 16)).ToString();
            });
            return outStr;
        }

        /// <summary>
        /// 替换字符串中所有出现过的子字符串
        /// </summary>
        /// <param name="oldString"></param>
        /// <param name="newString"></param>
        /// <returns></returns>
        public static string ReplaceAll(String str, List<String>oldString,List<String>newString)
        {
            if (oldString.Count > 0 && newString.Count > 0 && oldString.Count == newString.Count)
            {
                for (int i = 0, count = oldString.Count; i < count; i++)
                {
                    str = str.Replace(oldString[i], newString[i]);
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
        public static string ReplaceAll(String str, String[] oldString, String[] newString)
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
