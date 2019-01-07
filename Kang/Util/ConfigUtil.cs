using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Kang.Util
{
    public class ConfigUtil
    {
        private String m_FileName;

        public String FileName
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }

        [DllImport("kernel32.dll")]
        private static extern Int32 GetPrivateProfileInt(
            String lpAppName,
            String lpKeyName,
            Int32 nDefault,
            String lpFileName
            );

        [DllImport("kernel32.dll")]
        private static extern Int32 GetPrivateProfileString(
            String lpAppName,
            String lpKeyName,
            String lpDefault,
            StringBuilder lpReturnedString,
            Int32 nSize,
            String lpFileName
            );

        [DllImport("kernel32.dll")]
        private static extern Int32 GetPrivateProfileSectionNames(
            byte[] lpReturnedString,
            Int32 nSize,
            String lpFileName
            );

        [DllImport("kernel32.dll")]
        private static extern Int32 WritePrivateProfileString(
            String lpAppName,
            String lpKeyName,
            String lpString,
            String lpFileName
            );

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="aFileName">Ini文件路径</param>
        public ConfigUtil(String aFileName)
        {
            this.m_FileName = aFileName;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigUtil()
        { }

        /// <summary>
        /// [扩展]读Int数值
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public Int32 ReadInt(String section, String name, Int32 def)
        {
            return GetPrivateProfileInt(section, name, def, this.m_FileName);
        }

        /// <summary>
        /// [扩展]读取string字符串
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public String ReadString(String section, String name, String def)
        {
            StringBuilder vRetSb = new StringBuilder(2048);
            GetPrivateProfileString(section, name, def, vRetSb, 2048, this.m_FileName);
            return vRetSb.ToString();
        }

        /// <summary>
        /// [扩展]写入Int数值，如果不存在 节-键，则会自动创建
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="Ival">写入值</param>
        public void WriteInt(String section, String name, Int32 Ival)
        {

            WritePrivateProfileString(section, name, Ival.ToString(), this.m_FileName);
        }

        /// <summary>
        /// [扩展]写入String字符串，如果不存在 节-键，则会自动创建
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="strVal">写入值</param>
        public void WriteString(String section, String name, String strVal)
        {
            WritePrivateProfileString(section, name, strVal, this.m_FileName);
        }

        /// <summary>
        /// 删除指定的 节
        /// </summary>
        /// <param name="section"></param>
        public void DeleteSection(String section)
        {
            WritePrivateProfileString(section, null, null, this.m_FileName);
        }

        /// <summary>
        /// 删除全部 节
        /// </summary>
        public void DeleteAllSection()
        {
            WritePrivateProfileString(null, null, null, this.m_FileName);
        }

        /// <summary>
        /// 读取指定 节-键 的值
        /// </summary>
        /// <param name="section"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public String IniReadValue(String section, String name)
        {
            StringBuilder strSb = new StringBuilder(256);
            GetPrivateProfileString(section, name, "", strSb, 256, this.m_FileName);
            return strSb.ToString();
        }

        /// <summary>
        /// 写入指定值，如果不存在 节-键，则会自动创建
        /// </summary>
        /// <param name="section"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void IniWriteValue(String section, String name, String value)
        {
            WritePrivateProfileString(section, name, value, this.m_FileName);
        }

        /// <summary>
        /// 获取所有的节（Section）名称，返回字符串数组，没有数据时返回null
        /// </summary>
        /// <returns></returns>
        public String[] IniReadAllSection()
        {
            byte[] bytes = new byte[2048];
            int count = GetPrivateProfileSectionNames(bytes, 2048, this.m_FileName);
            List<String> list = new List<String>();
            for (int i = 0,index = 0;i<count;i++)
            {
                if (bytes[i] == 0)
                {
                    list.Add(Encoding.UTF8.GetString(bytes, index, i - index));
                    index = i + 1;
                }
            }
            if (list.Count <= 0)
            {
                return null;
            }
            return list.ToArray();
        }
    }
}
