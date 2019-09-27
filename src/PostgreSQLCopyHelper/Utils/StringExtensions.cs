// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PostgreSQLCopyHelper.Utils
{
    public static class StringExtensions
    {
        public static string GetIdentifier(this string identifier, bool usePostgresQuotes)
        {
            return usePostgresQuotes
                ? NpgsqlUtils.QuoteIdentifier(identifier)
                : identifier;
        }
    }
}
