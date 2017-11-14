using System;

namespace PostgreSQLCopyHelper.Exceptions
{
    class PostgresHelperSaveAllException : Exception
    {
        public PostgresHelperSaveAllException(string message, Exception ex = null) : base(message, ex)
        {

        }

        public PostgresHelperSaveAllException(string message) : base(message)
        {

        }
    }
}
