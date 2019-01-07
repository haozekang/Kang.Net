using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kang.KangException
{
    public class KangSQLException : ApplicationException
    {
        private string error;
        private Exception innerException;
        public KangSQLException(string msg) : base(msg)
        {
            this.error = msg;
        }
        public KangSQLException(string msg, Exception innerException) : base(msg)
        {
            this.innerException = innerException;
            this.error = msg;
        }

        public string GetError()
        {
            return error;
        }
    }
}
