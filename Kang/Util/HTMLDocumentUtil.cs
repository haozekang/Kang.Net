using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kang.Util
{
    /// <summary>
    /// HTML工具包
    /// </summary>
    public class HTMLDocumentUtil
    {
        /// <summary>
        /// 获取节点中的内容
        /// </summary>
        /// <param name="htmlString">html页面源代码</param>
        /// <param name="expression">表达式，例如：//a[@class='titlelnk']</param>
        /// <returns></returns>
        public static String[] GetAllNodesValueString(String htmlString, String expression)
        {
            HtmlDocument doc = new HtmlDocument();
            if (StringUtil.isNotBlank(htmlString))
            {
                doc.LoadHtml(htmlString);
                HtmlNodeCollection titleNodes = doc.DocumentNode.SelectNodes(expression);
                if (titleNodes != null)
                {
                    String[] strArr = new String[titleNodes.Count];
                    int i = 0;
                    foreach (var item in titleNodes)
                    {
                        strArr[i++] = item.InnerText;
                    }
                    return strArr;
                }
            }
            return null;
        }

        /// <summary>
        /// 筛选不想要的标签对
        /// </summary>
        /// <param name="htmlString">网页源码</param>
        /// <param name="tag">标签名</param>
        /// <returns></returns>
        public static String FilterTag(String htmlString,String tag)
        {
            if (StringUtil.isNotBlank(htmlString))
            {
                while (htmlString.IndexOf("<" + tag) >= 0)
                {
                    int zstart = htmlString.IndexOf("<" + tag);
                    int zend = htmlString.Substring(zstart).IndexOf(">") + zstart;
                    int ystart = htmlString.Substring(zend).IndexOf("</") + zend;
                    int yend = htmlString.Substring(ystart).IndexOf(tag + ">") + ystart;
                    String value = htmlString.Substring(zend + 1, ystart - zend - 1);
                    htmlString = htmlString.Substring(0, zstart) + value + htmlString.Substring(yend + tag.Length + 1);
                }
                return htmlString;
            }
            return null;
        }
    }
}
