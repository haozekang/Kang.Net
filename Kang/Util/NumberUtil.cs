using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kang.Util
{
    /// <summary>
    /// 数字处理工具集
    /// </summary>
    public class NumberUtil
    {
        static Random random = new Random();

        /// <summary>
        /// 获取随机整数
        /// </summary>
        /// <returns></returns>
        public static Int32 getRandomInt32Number()
        {
            return random.Next();
        }

        /// <summary>
        /// 获取随机整数，但是小于max
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Int32 getRandomInt32Number(Int32 max)
        {
            return random.Next(max);
        }

        /// <summary>
        /// 在[min,max)范围内获取一个随机整数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Int32 getRandomInt32Number(Int32 min,Int32 max)
        {
            return random.Next(min,max);
        }

        /// <summary>
        /// 字符串转整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Int32 stringToInt32(String str)
        {
            return Int32.Parse(str);
        }

        /// <summary>
        /// 字符串转浮点数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Double stringToDouble(String str)
        {
            return Double.Parse(str);
        }

    }
}
