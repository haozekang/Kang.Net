using System;

namespace Kang.Util
{
    /// <summary>
    /// 日期处理工具集
    /// </summary>
    public class DateUtil
    {
        /// <summary>
        /// 得到格式化的时间
        /// </summary>
        /// <param name="format"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static String getFormatDate(String format, DateTime date)
        {
            if (StringUtil.isBlank(format))
            {
                return null;
            }
            return String.Format("{0:"+ format + "}", date);
        }

        /// <summary>
        /// 将时间字符串转化为时间对象
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime stringToDate(String date)
        {
            if (StringUtil.isBlank(date))
            {
                return DateTime.Now;
            }
            return DateTime.Parse(date);
        }

        /// <summary>
        /// 获取多少分钟后的时间对象
        /// </summary>
        /// <param name="date"></param>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static DateTime addMinutes(DateTime date, Int32 minutes)
        {
            return date.AddMinutes(minutes);
        }

        /// <summary>
        /// 获取多少小时后的时间对象
        /// </summary>
        /// <param name="date"></param>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static DateTime addHours(DateTime date, Int32 hours)
        {
            return date.AddHours(hours);
        }

        /// <summary>
        /// 获取多少天后的时间对象
        /// </summary>
        /// <param name="date"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public static DateTime addDay(DateTime date, Int32 days)
        {
            return date.AddDays(days);
        }

        /// <summary>
        /// 获取多少月后的时间对象
        /// </summary>
        /// <param name="date"></param>
        /// <param name="months"></param>
        /// <returns></returns>
        public static DateTime addMonth(DateTime date, Int32 months)
        {
            return date.AddMonths(months);
        }

        /// <summary>
        /// 获取多少年后的时间对象
        /// </summary>
        /// <param name="date"></param>
        /// <param name="years"></param>
        /// <returns></returns>
        public static DateTime addYear(DateTime date, Int32 years)
        {
            return date.AddYears(years);
        }

        /// <summary>
        /// 获取两个时间对象之间相差多少秒（绝对值）
        /// </summary>
        /// <param name="DateTime1"></param>
        /// <param name="DateTime2"></param>
        /// <returns></returns>
        public static Double DateDiffSeconds(DateTime DateTime1, DateTime DateTime2)
        {
            Double dif = 0;
            try
            {
                TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                dif = ts.TotalSeconds;
            }
            catch
            {
            }
            return dif;
        }

        /// <summary>
        /// 获取两个时间对象之间相差多少天（绝对值）
        /// </summary>
        /// <param name="DateTime1"></param>
        /// <param name="DateTime2"></param>
        /// <returns></returns>
        public static Double DateDiffDays(DateTime DateTime1, DateTime DateTime2)
        {
            Double dif = 0;
            try
            {
                TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                dif = ts.TotalDays;
            }
            catch
            {
            }
            return dif;
        }
    }
}
