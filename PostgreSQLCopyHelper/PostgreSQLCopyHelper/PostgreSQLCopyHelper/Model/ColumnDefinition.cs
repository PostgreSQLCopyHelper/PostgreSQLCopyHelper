// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Npgsql;
using System;

namespace PostgreSQLCopyHelper.Model
{
    internal class ColumnDefinition<TEntity>
    {
        public string ColumnName { get; set; }

        public Action<NpgsqlBinaryImporter, TEntity> Write { get; set; }

        public override string ToString()
        {
            return string.Format("ColumnDefinition (ColumnName = {0}, Serialize = {1})", ColumnName, Write);
        }
    }
}
