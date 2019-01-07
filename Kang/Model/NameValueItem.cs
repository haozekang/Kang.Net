using System;

namespace Kang.Model
{
    public class NameValueItem
    {
        private String name;
        private Object value;

        public NameValueItem(String name, Object value)
        {
            this.name = name;
            this.value = value;
        }

        public string Name
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

        public object Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }
    }
}
