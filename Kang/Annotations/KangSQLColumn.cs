using System;

namespace Kang.Annotations
{
    [AttributeUsage(AttributeTargets.Field)]
    public class KangSQLColumn : Attribute
    {
        private String name;

        private String type;

        public String Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public String Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }
    }
}
