// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NpgsqlTypes;

namespace PostgreSQLCopyHelper.Model
{
    public class TargetColumn
    {
        public string ColumnName { get; internal set; }

        public Type ClrType { get; internal set; } 

        public NpgsqlDbType? DbType { get; internal set; }

        public string DataTypeName { get; internal set; }
    }
}
