using System.Collections.Generic;
using Npgsql;

namespace PostgreSQLCopyHelper.Test.Extensions
{
    public static class NpgsqlExtensions
    {
        public static List<object[]> GetAll(this NpgsqlConnection connection, string schema, string table)
        {
            var sqlStatement = $"SELECT * FROM {schema}.{table}";

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
