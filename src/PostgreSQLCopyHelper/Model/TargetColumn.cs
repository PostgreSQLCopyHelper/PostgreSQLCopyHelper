// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using NpgsqlTypes;

namespace PostgreSQLCopyHelper.Model
{
    public class TargetColumn
    {
        public string ColumnName { get; internal set; }

        public NpgsqlDbType DbType { get; internal set; }
    }
}
