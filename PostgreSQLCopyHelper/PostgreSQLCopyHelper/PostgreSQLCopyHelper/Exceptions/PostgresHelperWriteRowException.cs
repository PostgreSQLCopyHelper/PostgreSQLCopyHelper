using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgreSQLCopyHelper.Exceptions
{
    public class PostgresHelperWriteRowException : Exception
    {
        public PostgresHelperWriteRowException(string message, Exception ex = null) : base(message, ex)
        {
            
        }

        public PostgresHelperWriteRowException(string message) : base(message)
        {
            
        }
    }
}
