// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace PostgreSQLCopyHelper.Model
{
    internal class ColumnDefinition<TEntity>
    {
        public string ColumnName { get; set; }

        public Type ClrType { get; set; }

        public NpgsqlDbType? DbType { get; set; }

        public string DataTypeName { get; set; }

        public Func<NpgsqlBinaryImporter, TEntity, CancellationToken, Task> WriteAsync { get; set; }

        public override string ToString()
        {
            return $"ColumnDefinition (ColumnName = {ColumnName}, Serialize = {WriteAsync})";
        }
    }
}
