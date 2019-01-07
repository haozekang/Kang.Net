using System;
using MySql.Data.MySqlClient;
using System.Data;
using Kang.Util;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Kang.Annotations;
using Kang.KangException;

namespace Kang.SQLManager
{
    /// <summary>
    /// MySQL数据库操作类
    /// </summary>
    public class MySQLManager
    {
        MySqlConnection con = null;
        MySqlCommand cmd = null;
        MySqlDataReader reader = null;
        String url = "";

        /// <summary>
        /// 是否显示SQL语句
        /// </summary>
        public Boolean showStr = false;

        /// <summary>
        /// 是否打印数据库连接状态
        /// </summary>
        public Boolean showState = false;

        /// <summary>
        /// 是否显示SQL语句
        /// </summary>
        public Boolean rollBack = false;
        private Boolean rollBackError = false;
        private Boolean rollBackBegin = false;

        /// <summary>
        /// 实例化数据库操作对象
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="dbname"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public MySQLManager(String ip,String port,String dbname,String username,String password)
        {
            this.url = string.Concat(new string[]
            {
                "Server=",
                ip,
                ";Database=",
                dbname,
                "; User ID=",
                username,
                ";Password=",
                password,
                ";port=",
                port,
                ";CharSet=utf8;pooling=true;SslMode=none;"
            });
            this.con = new MySqlConnection(this.url);
            con = new MySqlConnection(url);
        }

        /// <summary>
        /// 关闭数据库
        /// </summary>
        public void Close()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }

        /// <summary>
        /// 开启事物
        /// </summary>
        public void OpenRoll()
        {
            if (this.showState)
            {
                Console.WriteLine("Before State:" + this.con.State);
            }
            if (!this.rollBack)
            {
                this.rollBack = true;
                this.con.Open();
                this.Query("begin");
                this.rollBackBegin = true;
                this.rollBackError = false;
            }
            if (this.showState)
            {
                Console.WriteLine("After State:" + this.con.State);
            }
        }

        /// <summary>
        /// 关闭事物
        /// </summary>
        public void CloseRoll()
        {
            if (showState)
            {
                Console.WriteLine("Before State:" + con.State);
            }
            if (rollBack)
            {
                if (rollBackError)
                {
                    Query("rollback");
                    rollBack = false;
                    rollBackError = false;
                    rollBackBegin = false;
                    if (IsOpen())
                    {
                        con.Close();
                    }
                    throw new KangSQLException("数据保存失败，已修改数据已回滚！");
                }
                else
                {
                    Query("commit");
                }
                rollBack = false;
                rollBackError = false;
                rollBackBegin = false;
                if (IsOpen())
                {
                    con.Close();
                }
            }
            if (showState)
            {
                Console.WriteLine("After State:" + con.State);
            }
        }

        /// <summary>
        /// 执行数据库查询语句，可查询单个字段单条信息
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <returns></returns>
        public String ExecuteSelect(String sqlStr)
        {
            if (showState)
            {
                Console.WriteLine("Before State:" + con.State);
            }
            if (!rollBack && IsClosed())
            {
                con.Open();
            }
            string result;
            if (this.rollBack && this.rollBackError)
            {
                result = null;
            }
            else
            {
                try
                {
                    if (this.showStr)
                    {
                        Console.WriteLine("SQL:" + sqlStr);
                    }
                    if (this.showState)
                    {
                        Console.WriteLine("Doing State:" + this.con.State);
                    }
                    this.cmd = new MySqlCommand(sqlStr, this.con);
                    this.reader = this.cmd.ExecuteReader();
                    if (this.reader.Read())
                    {
                        string str = "";
                        if (this.reader[0] != DBNull.Value)
                        {
                            str = this.reader.GetString(0);
                        }
                        if (!this.reader.IsClosed)
                        {
                            this.reader.Close();
                        }
                        if (!this.rollBack && this.IsOpen())
                        {
                            this.con.Close();
                        }
                        return str;
                    }
                }
                catch (Exception e)
                {
                    if (!this.rollBack && this.IsOpen())
                    {
                        this.con.Close();
                    }
                    this.rollBackError = true;
                    throw new KangSQLException(e.ToString());
                }
                finally
                {
                    if (!this.reader.IsClosed)
                    {
                        this.reader.Close();
                    }
                }
                if (!this.rollBack && this.IsOpen())
                {
                    this.con.Close();
                }
                if (this.showState)
                {
                    Console.WriteLine("After State:" + this.con.State);
                }
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Boolean Save(Object data)
        {
            bool result;
            if (this.rollBack && this.rollBackError)
            {
                result = false;
            }
            else
            {
                List<FieldInfo> fields = data.GetType().GetRuntimeFields().ToList<FieldInfo>();
                KangSQLTable kangSQLTable = (KangSQLTable)Attribute.GetCustomAttribute(data.GetType(), typeof(KangSQLTable));
                if (kangSQLTable == null)
                {
                    throw new KangSQLException("===>kangSQLTable is null!");
                }
                string tableName = kangSQLTable.Table;
                string PRIMARY_KEY = kangSQLTable.Key;
                if (StringUtil.isBlank(PRIMARY_KEY))
                {
                    throw new KangSQLException("===>PRIMARY_KEY is null!");
                }
                bool auto = kangSQLTable.Auto;
                string SQLstr = "INSERT INTO " + tableName + " (";
                string SQLEndstr = ") VALUES (";
                int i = 0;
                foreach (FieldInfo fieldval in fields)
                {
                    KangSQLColumn kangSQLColumn = (KangSQLColumn)Attribute.GetCustomAttribute(fieldval, typeof(KangSQLColumn));
                    if (!(PRIMARY_KEY.Equals(kangSQLColumn.Name) && auto))
                    {
                        if (i == 0)
                        {
                            SQLstr += kangSQLColumn.Name;
                        }
                        else
                        {
                            SQLstr = SQLstr + "," + kangSQLColumn.Name;
                        }
                        string fieldvalue;
                        if (fieldval.GetValue(data) == null)
                        {
                            fieldvalue = null;
                        }
                        else
                        {
                            fieldvalue = fieldval.GetValue(data).ToString();
                        }
                        if (i == 0)
                        {
                            if (StringUtil.isBlank(fieldvalue))
                            {
                                SQLEndstr += "NULL";
                            }
                            else
                            {
                                SQLEndstr = SQLEndstr + "'" + fieldvalue + "'";
                            }
                        }
                        else
                        {
                            if (StringUtil.isBlank(fieldvalue))
                            {
                                SQLEndstr += ",NULL";
                            }
                            else
                            {
                                SQLEndstr = SQLEndstr + ",'" + fieldvalue + "'";
                            }
                        }
                        i++;
                    }
                }
                SQLstr = SQLstr + SQLEndstr + ")";
                if (this.showState)
                {
                    Console.WriteLine("Before State:" + this.con.State);
                }
                if (!this.rollBack && this.IsClosed())
                {
                    this.con.Open();
                }
                if (this.showStr)
                {
                    Console.WriteLine("SQL:" + SQLstr);
                }
                if (this.showState)
                {
                    Console.WriteLine("Doing State:" + this.con.State);
                }
                i = 0;
                try
                {
                    this.cmd = new MySqlCommand(SQLstr, this.con);
                    i = this.cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    if (!this.rollBack && this.IsOpen())
                    {
                        this.con.Close();
                    }
                    this.rollBackError = true;
                    throw new KangSQLException(e.ToString());
                }
                if (!this.rollBack && this.IsOpen())
                {
                    this.con.Close();
                }
                if (this.showState)
                {
                    Console.WriteLine("After State:" + this.con.State);
                }
                bool flag = i > 0;
                result = flag;
            }
            return result;
        }

        /// <summary>
        /// 可替换参数的Save方法
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sql_fields"></param>
        /// <param name="sql_values"></param>
        /// <returns></returns>
        public Boolean Save(Object data,String[] sql_fields, String[] sql_values)
        {
            bool result;
            if (this.rollBack && this.rollBackError)
            {
                result = false;
            }
            else
            {
                List<FieldInfo> fields = data.GetType().GetRuntimeFields().ToList<FieldInfo>();
                KangSQLTable kangSQLTable = (KangSQLTable)Attribute.GetCustomAttribute(data.GetType(), typeof(KangSQLTable));
                if (kangSQLTable == null)
                {
                    throw new KangSQLException("===>kangSQLTable is null!");
                }
                string tableName = kangSQLTable.Table;
                string PRIMARY_KEY = kangSQLTable.Key;
                if (StringUtil.isBlank(PRIMARY_KEY))
                {
                    throw new KangSQLException("===>PRIMARY_KEY is null!");
                }
                bool auto = kangSQLTable.Auto;
                string SQLstr = "INSERT INTO " + tableName + " (";
                string SQLEndstr = ") VALUES (";
                int i = 0;
                foreach (FieldInfo fieldval in fields)
                {
                    KangSQLColumn kangSQLColumn = (KangSQLColumn)Attribute.GetCustomAttribute(fieldval, typeof(KangSQLColumn));
                    if (!(PRIMARY_KEY.Equals(kangSQLColumn.Name) && auto))
                    {
                        if (i == 0)
                        {
                            SQLstr += kangSQLColumn.Name;
                        }
                        else
                        {
                            SQLstr = SQLstr + "," + kangSQLColumn.Name;
                        }

                        string fieldvalue = null;
                        bool replace_flag = false;
                        if (sql_fields != null)
                        {
                            int k = 0,f_count = sql_fields.Length;
                            for (k = 0;k < f_count;k++)
                            {
                                if (kangSQLColumn.Name.Equals(sql_fields[k]))
                                {
                                    replace_flag = true;
                                    fieldvalue = sql_values[k];
                                }
                            }
                        }

                        if (!replace_flag)
                        {
                            if (fieldval.GetValue(data) == null)
                            {
                                fieldvalue = null;
                            }
                            else
                            {
                                fieldvalue = fieldval.GetValue(data).ToString();
                            }
                        }

                        if (i == 0)
                        {
                            if (StringUtil.isBlank(fieldvalue))
                            {
                                SQLEndstr += "NULL";
                            }
                            else
                            {
                                if (replace_flag)
                                {
                                    SQLEndstr = SQLEndstr + fieldvalue;
                                }
                                else
                                {
                                    SQLEndstr = SQLEndstr + "'" + fieldvalue + "'";
                                }
                            }
                        }
                        else
                        {
                            if (StringUtil.isBlank(fieldvalue))
                            {
                                SQLEndstr += ",NULL";
                            }
                            else
                            {
                                if (replace_flag)
                                {
                                    SQLEndstr = SQLEndstr + "," + fieldvalue;
                                }
                                else
                                {
                                    SQLEndstr = SQLEndstr + ",'" + fieldvalue + "'";
                                }
                            }
                        }
                        i++;
                    }
                }
                SQLstr = SQLstr + SQLEndstr + ")";
                if (this.showState)
                {
                    Console.WriteLine("Before State:" + this.con.State);
                }
                if (!this.rollBack && this.IsClosed())
                {
                    this.con.Open();
                }
                if (this.showStr)
                {
                    Console.WriteLine("SQL:" + SQLstr);
                }
                if (this.showState)
                {
                    Console.WriteLine("Doing State:" + this.con.State);
                }
                i = 0;
                try
                {
                    this.cmd = new MySqlCommand(SQLstr, this.con);
                    i = this.cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    if (!this.rollBack && this.IsOpen())
                    {
                        this.con.Close();
                    }
                    this.rollBackError = true;
                    throw new KangSQLException(e.ToString());
                }
                if (!this.rollBack && this.IsOpen())
                {
                    this.con.Close();
                }
                if (this.showState)
                {
                    Console.WriteLine("After State:" + this.con.State);
                }
                bool flag = i > 0;
                result = flag;
            }
            return result;
        }

        /// <summary>
        /// 根据主键删除数据
        /// </summary>
        /// <param name="rowguid"></param>
        /// <returns></returns>
        public Boolean Delete<T>(String rowguid)
        {
            T value = Activator.CreateInstance<T>();
            bool result;
            if (this.rollBack && this.rollBackError)
            {
                result = false;
            }
            else
            {
                List<FieldInfo> fields = value.GetType().GetRuntimeFields().ToList<FieldInfo>();
                KangSQLColumn kangSQLColumn = null;
                KangSQLTable kangSQLTable = (KangSQLTable)Attribute.GetCustomAttribute(value.GetType(), typeof(KangSQLTable));
                if (kangSQLTable == null)
                {
                    throw new KangSQLException("===>kangSQLTable is null!");
                }
                string tableName = kangSQLTable.Table;
                string PRIMARY_KEY = kangSQLTable.Key;
                if (StringUtil.isBlank(PRIMARY_KEY))
                {
                    throw new KangSQLException("===>PRIMARY_KEY is null!");
                }
                string fieldvalue = "";
                string SQLstr = "";
                int i = 0;
                foreach (FieldInfo fieldval in fields)
                {
                    kangSQLColumn = (KangSQLColumn)Attribute.GetCustomAttribute(fieldval, typeof(KangSQLColumn));
                    if (PRIMARY_KEY.Equals(kangSQLColumn.Name))
                    {
                        if (StringUtil.isBlank(rowguid))
                        {
                            fieldvalue = null;
                            throw new KangSQLException("主键不能为空");
                        }
                        fieldvalue = rowguid;
                    }
                }
                if (StringUtil.isNotBlank(fieldvalue))
                {
                    SQLstr = string.Concat(new string[]
                    {
                        "DELETE FROM ",
                        tableName,
                        " WHERE ",
                        PRIMARY_KEY,
                        " = '",
                        fieldvalue,
                        "'"
                    });
                }
                else
                {
                    throw new KangSQLException("===>PRIMARY_KEY value is null!");
                }
                value = default(T);
                if (this.showState)
                {
                    Console.WriteLine("Before State:" + this.con.State);
                }
                if (!this.rollBack && this.IsClosed())
                {
                    this.con.Open();
                }
                if (this.showStr)
                {
                    Console.WriteLine("SQL:" + SQLstr);
                }
                if (this.showState)
                {
                    Console.WriteLine("Doing State:" + this.con.State);
                }
                try
                {
                    this.cmd = new MySqlCommand(SQLstr, this.con);
                    i = this.cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    if (!this.rollBack && this.IsOpen())
                    {
                        this.con.Close();
                    }
                    this.rollBackError = true;
                    throw new KangSQLException(e.ToString());
                }
                if (!this.rollBack && this.IsOpen())
                {
                    this.con.Close();
                }
                if (this.showState)
                {
                    Console.WriteLine("After State:" + this.con.State);
                }
                bool flag = i > 0;
                result = flag;
            }
            return result;
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Boolean Update(Object data)
        {
            bool result;
            if (this.rollBack && this.rollBackError)
            {
                result = false;
            }
            else
            {
                List<FieldInfo> fields = data.GetType().GetRuntimeFields().ToList<FieldInfo>();
                KangSQLTable kangSQLTable = (KangSQLTable)Attribute.GetCustomAttribute(data.GetType(), typeof(KangSQLTable));
                if (kangSQLTable == null)
                {
                    throw new KangSQLException("===>kangSQLTable is null!");
                }
                string tableName = kangSQLTable.Table;
                string PRIMARY_KEY = kangSQLTable.Key;
                if (StringUtil.isBlank(PRIMARY_KEY))
                {
                    throw new KangSQLException("===>PRIMARY_KEY is null!");
                }
                string PRIMARY_KEY_VALUE = "";
                bool auto = kangSQLTable.Auto;
                string SQLstr = "UPDATE " + tableName + " SET ";
                int i = 0;
                foreach (FieldInfo fieldval in fields)
                {
                    KangSQLColumn kangSQLColumn = (KangSQLColumn)Attribute.GetCustomAttribute(fieldval, typeof(KangSQLColumn));
                    string fieldvalue;
                    if (fieldval.GetValue(data) == null)
                    {
                        fieldvalue = null;
                    }
                    else
                    {
                        fieldvalue = fieldval.GetValue(data).ToString();
                    }
                    if (kangSQLColumn.Name.Equals(PRIMARY_KEY))
                    {
                        PRIMARY_KEY_VALUE = fieldval.GetValue(data).ToString();
                    }
                    else
                    {
                        if (i == 0)
                        {
                            if (StringUtil.isBlank(fieldvalue))
                            {
                                SQLstr = SQLstr + kangSQLColumn.Name + "=NULL";
                            }
                            else
                            {
                                SQLstr = string.Concat(new string[]
                                {
                                    SQLstr,
                                    kangSQLColumn.Name,
                                    "='",
                                    fieldvalue,
                                    "'"
                                });
                            }
                        }
                        else
                        {
                            if (StringUtil.isBlank(fieldvalue))
                            {
                                SQLstr = SQLstr + "," + kangSQLColumn.Name + "=NULL";
                            }
                            else
                            {
                                SQLstr = string.Concat(new string[]
                                {
                                    SQLstr,
                                    ",",
                                    kangSQLColumn.Name,
                                    "='",
                                    fieldvalue,
                                    "'"
                                });
                            }
                        }
                        i++;
                    }
                }
                SQLstr = string.Concat(new string[]
                {
                    SQLstr,
                    " WHERE ",
                    PRIMARY_KEY,
                    "='",
                    PRIMARY_KEY_VALUE,
                    "'"
                });
                if (this.showState)
                {
                    Console.WriteLine("Before State:" + this.con.State);
                }
                if (!this.rollBack && this.IsClosed())
                {
                    this.con.Open();
                }
                if (this.showStr)
                {
                    Console.WriteLine("SQL:" + SQLstr);
                }
                if (this.showState)
                {
                    Console.WriteLine("Doing State:" + this.con.State);
                }
                i = 0;
                try
                {
                    this.cmd = new MySqlCommand(SQLstr, this.con);
                    i = this.cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    if (!this.rollBack && this.IsOpen())
                    {
                        this.con.Close();
                    }
                    this.rollBackError = true;
                    throw new KangSQLException(e.ToString());
                }
                if (!this.rollBack && this.IsOpen())
                {
                    this.con.Close();
                }
                if (this.showState)
                {
                    Console.WriteLine("After State:" + this.con.State);
                }
                if (i > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 更新表部分字段信息
        /// </summary>
        /// <param name="guid">主键值</param>
        /// <param name="sql_fields"></param>
        /// <param name="sql_values"></param>
        /// <returns></returns>
        public Boolean Update<T>(String guid,String[] sql_fields, String[] sql_values)
        {
            if (StringUtil.isBlank(guid))
            {
                throw new KangSQLException("===>guid is null!");
            }
            if (sql_fields.Length != sql_values.Length)
            {
                throw new KangSQLException("===>字段和数据长度不对应!");
            }
            T value = Activator.CreateInstance<T>();
            bool result;
            if (this.rollBack && this.rollBackError)
            {
                result = false;
            }
            else
            {
                List<FieldInfo> fields = value.GetType().GetRuntimeFields().ToList<FieldInfo>();
                KangSQLTable kangSQLTable = (KangSQLTable)Attribute.GetCustomAttribute(value.GetType(), typeof(KangSQLTable));
                if (kangSQLTable == null)
                {
                    throw new KangSQLException("===>kangSQLTable is null!");
                }
                string tableName = kangSQLTable.Table;
                string PRIMARY_KEY = kangSQLTable.Key;
                if (StringUtil.isBlank(PRIMARY_KEY))
                {
                    throw new KangSQLException("===>PRIMARY_KEY is null!");
                }

                string SQLstr = "UPDATE " + tableName + " SET ";

                int i = 0, f_count = sql_fields.Length, v_count = sql_values.Length;
                for (i = 0;i < f_count && i < v_count;i++)
                {
                    if (i == 0)
                    {
                        if (StringUtil.isBlank(sql_values[i]))
                        {
                            SQLstr = SQLstr + sql_fields[i] + "=NULL";
                        }else
                        {
                            SQLstr = SQLstr + sql_fields[i] + "='" + sql_values[i] + "'";
                        }
                    }else
                    {
                        if (StringUtil.isBlank(sql_values[i]))
                        {
                            SQLstr = SQLstr + "," + sql_fields[i] + "=NULL";
                        }
                        else
                        {
                            SQLstr = SQLstr + "," + sql_fields[i] + "='" + sql_values[i] + "'";
                        }
                    }
                }
                SQLstr = SQLstr + " WHERE 1=1 AND " + PRIMARY_KEY + "='" + guid + "'";

                if (this.showState)
                {
                    Console.WriteLine("Before State:" + this.con.State);
                }
                if (!this.rollBack && this.IsClosed())
                {
                    this.con.Open();
                }
                if (this.showStr)
                {
                    Console.WriteLine("SQL:" + SQLstr);
                }
                if (this.showState)
                {
                    Console.WriteLine("Doing State:" + this.con.State);
                }
                i = 0;
                try
                {
                    this.cmd = new MySqlCommand(SQLstr, this.con);
                    i = this.cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    if (!this.rollBack && this.IsOpen())
                    {
                        this.con.Close();
                    }
                    this.rollBackError = true;
                    throw new KangSQLException(e.ToString());
                }
                if (!this.rollBack && this.IsOpen())
                {
                    this.con.Close();
                }
                if (this.showState)
                {
                    Console.WriteLine("After State:" + this.con.State);
                }
                if (i > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 更新数据（不更新NULL值）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Boolean UpdateNoNull(Object data)
        {
            bool result;
            if (this.rollBack && this.rollBackError)
            {
                result = false;
            }
            else
            {
                List<FieldInfo> fields = data.GetType().GetRuntimeFields().ToList<FieldInfo>();
                KangSQLTable kangSQLTable = (KangSQLTable)Attribute.GetCustomAttribute(data.GetType(), typeof(KangSQLTable));
                if (kangSQLTable == null)
                {
                    throw new KangSQLException("===>kangSQLTable is null!");
                }
                string tableName = kangSQLTable.Table;
                string PRIMARY_KEY = kangSQLTable.Key;
                if (StringUtil.isBlank(PRIMARY_KEY))
                {
                    throw new KangSQLException("===>PRIMARY_KEY is null!");
                }
                string PRIMARY_KEY_VALUE = "";
                bool auto = kangSQLTable.Auto;
                string SQLstr = "UPDATE " + tableName + " SET ";
                int i = 0;
                foreach (FieldInfo fieldval in fields)
                {
                    KangSQLColumn kangSQLColumn = (KangSQLColumn)Attribute.GetCustomAttribute(fieldval, typeof(KangSQLColumn));
                    string fieldvalue;
                    if (fieldval.GetValue(data) == null)
                    {
                        fieldvalue = null;
                    }
                    else
                    {
                        fieldvalue = fieldval.GetValue(data).ToString();
                    }
                    if (kangSQLColumn.Name.Equals(PRIMARY_KEY))
                    {
                        PRIMARY_KEY_VALUE = fieldval.GetValue(data).ToString();
                    }
                    else
                    {
                        if (i == 0)
                        {
                            if (StringUtil.isBlank(fieldvalue))
                            {
                                //SQLstr = SQLstr + kangSQLColumn.Name + "=NULL";
                            }
                            else
                            {
                                SQLstr = string.Concat(new string[]
                                {
                                    SQLstr,
                                    kangSQLColumn.Name,
                                    "='",
                                    fieldvalue,
                                    "'"
                                });
                            }
                        }
                        else
                        {
                            if (StringUtil.isBlank(fieldvalue))
                            {
                                //SQLstr = SQLstr + "," + kangSQLColumn.Name + "=NULL";
                            }
                            else
                            {
                                SQLstr = string.Concat(new string[]
                                {
                                    SQLstr,
                                    ",",
                                    kangSQLColumn.Name,
                                    "='",
                                    fieldvalue,
                                    "'"
                                });
                            }
                        }
                        i++;
                    }
                }
                SQLstr = string.Concat(new string[]
                {
                    SQLstr,
                    " WHERE ",
                    PRIMARY_KEY,
                    "='",
                    PRIMARY_KEY_VALUE,
                    "'"
                });
                if (this.showState)
                {
                    Console.WriteLine("Before State:" + this.con.State);
                }
                if (!this.rollBack && this.IsClosed())
                {
                    this.con.Open();
                }
                if (this.showStr)
                {
                    Console.WriteLine("SQL:" + SQLstr);
                }
                if (this.showState)
                {
                    Console.WriteLine("Doing State:" + this.con.State);
                }
                i = 0;
                try
                {
                    this.cmd = new MySqlCommand(SQLstr, this.con);
                    i = this.cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    if (!this.rollBack && this.IsOpen())
                    {
                        this.con.Close();
                    }
                    this.rollBackError = true;
                    throw new KangSQLException(e.ToString());
                }
                if (!this.rollBack && this.IsOpen())
                {
                    this.con.Close();
                }
                if (this.showState)
                {
                    Console.WriteLine("After State:" + this.con.State);
                }
                if (i > 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 查询单个实体
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public T Query <T> (String str)
        {
            T value = Activator.CreateInstance<T>();
            Type type = value.GetType();
            T result;
            if (this.rollBack && this.rollBackError)
            {
                result = default(T);
            }
            else
            {
                List<FieldInfo> fields = type.GetRuntimeFields().ToList<FieldInfo>();
                KangSQLTable kangSQLTable = (KangSQLTable)Attribute.GetCustomAttribute(type, typeof(KangSQLTable));
                if (kangSQLTable == null)
                {
                    throw new KangSQLException("===>kangSQLTable is null!");
                }
                string tableName = kangSQLTable.Table;
                string PRIMARY_KEY = kangSQLTable.Key;
                if (StringUtil.isBlank(PRIMARY_KEY))
                {
                    throw new KangSQLException("===>PRIMARY_KEY is null!");
                }
                bool auto = kangSQLTable.Auto;
                if (this.showState)
                {
                    Console.WriteLine("Before State:" + this.con.State);
                }
                if (!this.rollBack && this.IsClosed())
                {
                    this.con.Open();
                }
                if (this.showStr)
                {
                    Console.WriteLine("SQL:" + str);
                }
                try
                {
                    this.cmd = new MySqlCommand(str, this.con);
                    this.reader = this.cmd.ExecuteReader();
                    int colCount = this.reader.FieldCount;
                    string[] nameArr = new string[colCount];
                    for (int i = 0; i < colCount; i++)
                    {
                        nameArr[i] = this.reader.GetName(i);
                    }
                    if (this.reader.Read())
                    {
                        foreach (FieldInfo fieldval in fields)
                        {
                            KangSQLColumn kangSQLColumn = (KangSQLColumn)Attribute.GetCustomAttribute(fieldval, typeof(KangSQLColumn));
                            if (StringUtil.stringArrayHaveString(nameArr, kangSQLColumn.Name))
                            {
                                if (this.reader[kangSQLColumn.Name] != DBNull.Value)
                                {
                                    fieldval.SetValue(value, this.reader.GetString(kangSQLColumn.Name));
                                }
                            }
                        }
                        if (!this.reader.IsClosed)
                        {
                            this.reader.Close();
                        }
                        if (!this.rollBack && this.IsOpen())
                        {
                            this.con.Close();
                        }
                        if (this.showState)
                        {
                            Console.WriteLine("Doing State:" + this.con.State);
                        }
                        return value;
                    }
                }
                catch (Exception e)
                {
                    if (!this.rollBack && this.IsOpen())
                    {
                        this.con.Close();
                    }
                    this.rollBackError = true;
                    throw new KangSQLException(e.ToString());
                }
                finally
                {
                    if (!this.reader.IsClosed)
                    {
                        this.reader.Close();
                    }
                }
                if (!this.rollBack && this.IsOpen())
                {
                    this.con.Close();
                }
                if (this.showState)
                {
                    Console.WriteLine("After State:" + this.con.State);
                }
                result = default(T);
            }
            return result;
        }

        /// <summary>
        /// 根据主键值查询实体
        /// </summary>
        /// <param name="rowguid"></param>
        /// <returns></returns>
        public T QueryByGuid <T> (String rowguid)
        {
            T value = Activator.CreateInstance<T>();
            Type type = value.GetType();
            T result;
            if (this.rollBack && this.rollBackError)
            {
                result = default(T);
            }
            else
            {
                List<FieldInfo> fields = type.GetRuntimeFields().ToList<FieldInfo>();
                KangSQLTable kangSQLTable = (KangSQLTable)Attribute.GetCustomAttribute(type, typeof(KangSQLTable));
                if (kangSQLTable == null)
                {
                    throw new KangSQLException("===>kangSQLTable is null!");
                }
                string tableName = kangSQLTable.Table;
                string PRIMARY_KEY = kangSQLTable.Key;
                if (StringUtil.isBlank(PRIMARY_KEY))
                {
                    throw new KangSQLException("===>PRIMARY_KEY is null!");
                }
                bool auto = kangSQLTable.Auto;
                string SQLstr = string.Concat(new string[]
                {
                    "SELECT * FROM ",
                    tableName,
                    " WHERE ",
                    PRIMARY_KEY,
                    " = '",
                    rowguid,
                    "'"
                });
                if (this.showState)
                {
                    Console.WriteLine("Before State:" + this.con.State);
                }
                if (!this.rollBack && this.IsClosed())
                {
                    this.con.Open();
                }
                if (this.showStr)
                {
                    Console.WriteLine("SQL:" + SQLstr);
                }
                try
                {
                    this.cmd = new MySqlCommand(SQLstr, this.con);
                    this.reader = this.cmd.ExecuteReader();
                    int colCount = this.reader.FieldCount;
                    string[] nameArr = new string[colCount];
                    for (int i = 0; i < colCount; i++)
                    {
                        nameArr[i] = this.reader.GetName(i);
                    }
                    if (this.reader.Read())
                    {
                        foreach (FieldInfo fieldval in fields)
                        {
                            KangSQLColumn kangSQLColumn = (KangSQLColumn)Attribute.GetCustomAttribute(fieldval, typeof(KangSQLColumn));
                            if (StringUtil.stringArrayHaveString(nameArr, kangSQLColumn.Name))
                            {
                                if (this.reader[kangSQLColumn.Name] != DBNull.Value)
                                {
                                    fieldval.SetValue(value, this.reader.GetString(kangSQLColumn.Name));
                                }
                            }
                        }
                        if (this.showState)
                        {
                            Console.WriteLine("Doing State:" + this.con.State);
                        }
                        if (!this.reader.IsClosed)
                        {
                            this.reader.Close();
                        }
                        if (!this.rollBack && this.IsOpen())
                        {
                            this.con.Close();
                        }
                        return value;
                    }
                }
                catch (Exception e)
                {
                    if (!this.rollBack && this.IsOpen())
                    {
                        this.con.Close();
                    }
                    this.rollBackError = true;
                    throw new KangSQLException(e.ToString());
                }
                finally
                {
                    if (!this.reader.IsClosed)
                    {
                        this.reader.Close();
                    }
                }
                if (!this.rollBack && this.IsOpen())
                {
                    this.con.Close();
                }
                if (this.showState)
                {
                    Console.WriteLine("After State:" + this.con.State);
                }
                result = default(T);
            }
            return result;
        }

        /// <summary>
        /// 执行查询语句
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public List<T> QueryList <T> (String str)
        {
            if (StringUtil.isBlank(str))
            {
                throw new KangSQLException("===>SQL cmd string is null!");
            }
            T value = Activator.CreateInstance<T>();
            Type type = value.GetType();
            List<T> result;
            if (this.rollBack && this.rollBackError)
            {
                result = null;
            }
            else
            {
                List<FieldInfo> fields = type.GetRuntimeFields().ToList<FieldInfo>();
                if (this.showState)
                {
                    Console.WriteLine("Before State:" + this.con.State);
                }
                if (!this.rollBack && this.IsClosed())
                {
                    this.con.Open();
                }
                if (this.showStr)
                {
                    Console.WriteLine("SQL:" + str);
                }
                if (this.showState)
                {
                    Console.WriteLine("Doing State:" + this.con.State);
                }
                List<T> list = new List<T>();
                try
                {
                    this.cmd = new MySqlCommand(str, this.con);
                    this.reader = this.cmd.ExecuteReader();
                    int colCount = this.reader.FieldCount;
                    string[] nameArr = new string[colCount];
                    for (int i = 0; i < colCount; i++)
                    {
                        nameArr[i] = this.reader.GetName(i);
                    }
                    while (this.reader.Read())
                    {
                        value = Activator.CreateInstance<T>();
                        foreach (FieldInfo fieldval in fields)
                        {
                            KangSQLColumn kangSQLColumn = (KangSQLColumn)Attribute.GetCustomAttribute(fieldval, typeof(KangSQLColumn));
                            if (StringUtil.stringArrayHaveString(nameArr, kangSQLColumn.Name))
                            {
                                if (this.reader[kangSQLColumn.Name] != DBNull.Value)
                                {
                                    fieldval.SetValue(value, this.reader.GetString(kangSQLColumn.Name));
                                }
                            }
                        }
                        list.Add(value);
                    }
                }
                catch (Exception e)
                {
                    if (!this.rollBack && this.IsOpen())
                    {
                        this.con.Close();
                    }
                    this.rollBackError = true;
                    throw new KangSQLException(e.ToString());
                }
                finally
                {
                    if (!this.reader.IsClosed)
                    {
                        this.reader.Close();
                    }
                }
                if (!this.rollBack && this.IsOpen())
                {
                    this.con.Close();
                }
                if (this.showState)
                {
                    Console.WriteLine("After State:" + this.con.State);
                }
                result = list;
            }
            return result;
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public Boolean Query(String str)
        {
            if (StringUtil.isBlank(str))
            {
                throw new KangSQLException("===>SQL cmd string is null!");
            }
            bool result;
            if (this.rollBack && this.rollBackError)
            {
                result = false;
            }
            else
            {
                int i = 0;
                if (this.showState)
                {
                    Console.WriteLine("Before State:" + this.con.State);
                }
                if (!this.rollBack && this.IsClosed())
                {
                    this.con.Open();
                }
                if (this.showStr)
                {
                    Console.WriteLine("SQL:" + str);
                }
                if (this.showState)
                {
                    Console.WriteLine("Doing State:" + this.con.State);
                }
                try
                {
                    this.cmd = new MySqlCommand(str, this.con);
                    i = this.cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    if (!this.rollBack && this.IsOpen())
                    {
                        this.con.Close();
                    }
                    this.rollBackError = true;
                    throw new KangSQLException(e.ToString());
                }
                if (!this.rollBack && this.IsOpen())
                {
                    this.con.Close();
                }
                if (this.showState)
                {
                    Console.WriteLine("After State:" + this.con.State);
                }
                bool flag = i > 0;
                result = flag;
            }
            return result;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="bytes"></param>
        /// <returns>GUID</returns>
        public String UploadFile(String filename, Byte[] bytes)
        {
            string result;
            if (this.rollBack && this.rollBackError)
            {
                result = null;
            }
            else
            {
                string guid = "";
                string type = filename.Substring(filename.LastIndexOf("."), filename.Length - filename.LastIndexOf("."));
                filename = filename.Substring(0, filename.LastIndexOf("."));
                string SQLstr = "INSERT INTO file (rowguid,filename,type,filedata) VALUES (?rowguid,?filename,?type,?filedata)";
                if (this.showState)
                {
                    Console.WriteLine("Before State:" + this.con.State);
                }
                if (!this.rollBack && this.IsClosed())
                {
                    this.con.Open();
                }
                guid = Guid.NewGuid().ToString();
                this.cmd = new MySqlCommand(SQLstr, this.con);
                this.cmd.Parameters.Add("@rowguid", MySqlDbType.VarChar);
                this.cmd.Parameters.Add("@filename", MySqlDbType.VarChar);
                this.cmd.Parameters.Add("@type", MySqlDbType.VarChar);
                this.cmd.Parameters.Add("@filedata", MySqlDbType.Blob);
                this.cmd.Parameters[0].Value = guid;
                this.cmd.Parameters[1].Value = filename;
                this.cmd.Parameters[2].Value = type;
                this.cmd.Parameters[3].Value = bytes;
                if (this.showState)
                {
                    Console.WriteLine("Doing State:" + this.con.State);
                }
                try
                {
                    this.cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    if (!this.rollBack && this.IsOpen())
                    {
                        this.con.Close();
                    }
                    this.rollBackError = true;
                    throw new KangSQLException(e.ToString());
                }
                if (!this.rollBack && this.IsOpen())
                {
                    this.con.Close();
                }
                if (this.showState)
                {
                    Console.WriteLine("After State:" + this.con.State);
                }
                result = guid;
            }
            return result;
        }

        /// <summary>
        /// 判断当前连接是否有效
        /// </summary>
        /// <returns></returns>
        public Boolean IsConnecting()
        {
            if (con != null)
            {
                if (con.State == ConnectionState.Open)
                {
                    if (con.State == ConnectionState.Connecting)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 判断当前数据库是否打开
        /// </summary>
        /// <returns></returns>
        public Boolean IsOpen()
        {
            if (con != null)
            {
                if (con.State == ConnectionState.Open)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断当前数据库是否关闭
        /// </summary>
        /// <returns></returns>
        public Boolean IsClosed()
        {
            if (con != null)
            {
                if (con.State == ConnectionState.Closed)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
