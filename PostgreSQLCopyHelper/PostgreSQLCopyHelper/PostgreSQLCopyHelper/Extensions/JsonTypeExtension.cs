// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NpgsqlTypes;

namespace PostgreSQLCopyHelper
{
    public static class JsonTypeExtensions
    {
        public static PostgreSQLCopyHelper<TEntity> MapJson<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, string> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Json);
        }

        public static PostgreSQLCopyHelper<TEntity> MapJsonb<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, string> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Jsonb);
        }
    }
}
