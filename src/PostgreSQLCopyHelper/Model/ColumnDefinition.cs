﻿// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Npgsql;

namespace PostgreSQLCopyHelper.Model
{
    internal class ColumnDefinition<TEntity>
    {
        public string ColumnName { get; set; }

        public Func<NpgsqlBinaryImporter, TEntity, Task> Write { get; set; }

        public override string ToString()
        {
            return $"ColumnDefinition (ColumnName = {ColumnName}, Serialize = {Write})";
        }
    }
}
