using System;

namespace Kang.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class KangSQLTable : Attribute
    {
        private String table;

        private String key;

        private Boolean auto = false;

        public String Table
        {
            get
            {
                return table;
            }

            set
            {
                table = value;
            }
        }

        public String Key
        {
            get
            {
                return key;
            }

            set
            {
                key = value;
            }
        }

        public bool Auto
        {
            get
            {
                return auto;
            }

            set
            {
                auto = value;
            }
        }
    }
}
