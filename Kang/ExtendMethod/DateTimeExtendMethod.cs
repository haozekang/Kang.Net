using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kang.ExtendMethod
{
    /// <summary>
    /// DateTime类型扩展方法
    /// </summary>
    public static class DateTimeExtendMethod
    {
        /// <summary>
        /// 1：获取“yyyy/MM/dd HH:mm:ss”格式的时间字串
        /// 2：获取“yyyy-MM-dd HH:mm:ss”格式的时间字串
        /// default:获取“yyyy/MM/dd HH:mm:ss”格式的时间字串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToyyyyMMdd_HHmmss(this DateTime dt,Int32 i)//自身必须是静态方法，第一个参数必须是this，后面紧跟要扩展的类的名称
        {
            switch (i)
            {
                case 1: return dt.ToString("yyyy/MM/dd HH:mm:ss");
                case 2: return dt.ToString("yyyy-MM-dd HH:mm:ss");
                default: return dt.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }
    }
}
