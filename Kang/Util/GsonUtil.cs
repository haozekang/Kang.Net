using Newtonsoft.Json;
using System;

namespace Kang.Util
{
    /// <summary>
    /// JSON字串处理工具集
    /// </summary>
    public class GsonUtil
    {
        /// <summary>
        /// 将对象转换成JSON字串形式
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public static String GetObjectGsonString(Object ob)
        {
            return JsonConvert.SerializeObject(ob);
        }
    }
}
