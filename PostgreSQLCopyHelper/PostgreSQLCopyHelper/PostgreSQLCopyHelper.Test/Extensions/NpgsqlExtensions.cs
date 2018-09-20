using Npgsql;
using System.Collections.Generic;

namespace PostgreSQLCopyHelper.Test.Extensions
{
    public static class NpgsqlExtensions
    {
        public static IList<object[]> GetAll(this NpgsqlConnection connection, string schema, string table)
        {
            var sqlStatement = string.Format("SELECT * FROM {0}.{1}", schema, table);

            var sqlCommand = new NpgsqlCommand(sqlStatement, connection);

            var result = new List<object[]>();
            using (var dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    var values = new object[dataReader.FieldCount];
                    for (var i = 0; i < dataReader.FieldCount; i++)
                    {
                        values[i] = dataReader[i];
                    }
                    result.Add(values);
                }
            }

            return result;
        }
    }
}
