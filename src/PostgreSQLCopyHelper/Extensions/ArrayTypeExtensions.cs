﻿// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NpgsqlTypes;

namespace PostgreSQLCopyHelper
{
    public static class ArrayTypeExtensions
    {
        public static PostgreSQLCopyHelper<TEntity> MapArray<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, byte[][]> propertyGetter)
        {
            return MapArray(helper, columnName, propertyGetter, NpgsqlDbType.Bytea);
        }
        public static PostgreSQLCopyHelper<TEntity> MapArray<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Int16[]> propertyGetter)
        {
            return MapArray(helper, columnName, propertyGetter, NpgsqlDbType.Smallint);
        }

        public static PostgreSQLCopyHelper<TEntity> MapArray<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Int32[]> propertyGetter)
        {
            return MapArray(helper, columnName, propertyGetter, NpgsqlDbType.Integer);
        }

        public static PostgreSQLCopyHelper<TEntity> MapArray<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Int64[]> propertyGetter)
        {
            return MapArray(helper, columnName, propertyGetter, NpgsqlDbType.Bigint);
        }

        public static PostgreSQLCopyHelper<TEntity> MapArray<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Decimal[]> propertyGetter)
        {
            return MapArray(helper, columnName, propertyGetter, NpgsqlDbType.Numeric);
        }

        public static PostgreSQLCopyHelper<TEntity> MapArray<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Single[]> propertyGetter)
        {
            return MapArray(helper, columnName, propertyGetter, NpgsqlDbType.Real);
        }

        public static PostgreSQLCopyHelper<TEntity> MapArray<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Double[]> propertyGetter)
        {
            return MapArray(helper, columnName, propertyGetter, NpgsqlDbType.Double);
        }

        public static PostgreSQLCopyHelper<TEntity> MapArray<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, String[]> propertyGetter)
        {
            return MapArray(helper, columnName, propertyGetter, NpgsqlDbType.Text);
        }

        public static PostgreSQLCopyHelper<TEntity> MapArray<TEntity, TProperty>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, TProperty> propertyGetter, NpgsqlDbType type)
        {
            return helper.Map(columnName, propertyGetter, (NpgsqlDbType.Array | type));
        }
    }
}
