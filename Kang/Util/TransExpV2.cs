﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kang.Util
{
    /// <summary>
    /// 对象复制，工具类，TIn:有数据的对象类型，TOut:无数据的对象类型
    /// </summary>
    /// <typeparam name="TIn">有数据的对象</typeparam>
    /// <typeparam name="TOut">空白对象</typeparam>
    public static class TransExpV2<TIn, TOut>
    {

        private static readonly Func<TIn, TOut> cache = GetFunc();
        private static Func<TIn, TOut> GetFunc()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();

            foreach (var item in typeof(TOut).GetProperties())
            {
                if (!item.CanWrite)
                    continue;

                MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
                MemberBinding memberBinding = Expression.Bind(item, property);
                memberBindingList.Add(memberBinding);
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

            return lambda.Compile();
        }

        /// <summary>
        /// 复制对象，tIn:被复制的对象实体
        /// </summary>
        /// <param name="tIn">被复制的对象实体</param>
        /// <returns></returns>
        public static TOut Trans(TIn tIn)
        {
            return cache(tIn);
        }

    }
}
