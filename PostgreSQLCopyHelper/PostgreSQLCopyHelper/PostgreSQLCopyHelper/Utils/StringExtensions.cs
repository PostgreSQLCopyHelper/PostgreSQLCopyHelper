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
