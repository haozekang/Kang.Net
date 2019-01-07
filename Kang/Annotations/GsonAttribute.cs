using System;

namespace Kang.Annotations
{
    [AttributeUsage(AttributeTargets.Field)]
    public class GsonAttribute : Attribute
    {
        private String attribute;

        public String Attribute
        {
            get
            {
                return attribute;
            }

            set
            {
                attribute = value;
            }
        }
    }
}
