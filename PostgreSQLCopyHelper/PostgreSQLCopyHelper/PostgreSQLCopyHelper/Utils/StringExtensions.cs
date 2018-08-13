using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgreSQLCopyHelper.Utils
{
    public static class StringExtensions
    {
        public static string GetIdentifier(this string identifier, bool usePostgresQuotes)
        {
            if (usePostgresQuotes)
            {
                return NpgsqlUtils.QuoteIdentifier(identifier);
            }

            return identifier;
        }
    }
}
