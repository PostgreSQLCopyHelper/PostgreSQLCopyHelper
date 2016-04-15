using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgreSQLCopyHelper.Test.Extensions
{
    public static class NpgsqlExtensions
    {
        public static List<object[]> GetAll(this NpgsqlConnection connection, string schema, string table)
        {
            var sqlStatement = string.Format("SELECT * FROM {0}.{1}", schema, table);

            var sqlCommand = new NpgsqlCommand(sqlStatement, connection);

            List<object[]> result = new List<object[]>();
            using (var dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    var values = new object[dataReader.FieldCount];
                    for (int i = 0; i < dataReader.FieldCount; i++)
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
