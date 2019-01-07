using Kang.Util;
using System;

namespace Kang.Model
{
    public class KangSQLTableModel
    {
        public KangSQLTableModel()
        {
        }

        /// <summary>
        /// 返回类型
        /// </summary>
        /// <returns></returns>
        public static Type GetType()
        {
            return typeof(KangSQLTableModel);
        }

        /// <summary>
        /// 返回实体的Json字串
        /// </summary>
        /// <returns></returns>
        public new String ToString()
        {
            return GsonUtil.GetObjectGsonString(this);
        }
    }
}
