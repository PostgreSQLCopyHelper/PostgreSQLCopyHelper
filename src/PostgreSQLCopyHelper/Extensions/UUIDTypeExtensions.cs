// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NpgsqlTypes;

namespace PostgreSQLCopyHelper
{
    public static class UUIDTypeExtensions
    {
        public static PostgreSQLCopyHelper<TEntity> MapUUID<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Guid> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Uuid);
        }

        public static PostgreSQLCopyHelper<TEntity> MapUUID<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Guid?> propertyGetter)
        {
            return helper.MapNullable(columnName, propertyGetter, NpgsqlDbType.Uuid);
        }
    }
}
