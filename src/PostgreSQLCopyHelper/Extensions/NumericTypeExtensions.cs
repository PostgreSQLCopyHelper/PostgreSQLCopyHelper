// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NpgsqlTypes;

namespace PostgreSQLCopyHelper
{
    public static class NumericTypeExtensions
    {
        public static PostgreSQLCopyHelper<TEntity> MapSmallInt<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Int16> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Smallint);
        }

        public static PostgreSQLCopyHelper<TEntity> MapSmallInt<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Int16?> propertyGetter)
        {
            return helper.MapNullable(columnName, propertyGetter, NpgsqlDbType.Smallint);
        }

        public static PostgreSQLCopyHelper<TEntity> MapInteger<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Int32> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Integer);
        }

        public static PostgreSQLCopyHelper<TEntity> MapInteger<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Int32?> propertyGetter)
        {
            return helper.MapNullable(columnName, propertyGetter, NpgsqlDbType.Integer);
        }

        public static PostgreSQLCopyHelper<TEntity> MapBigInt<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Int64> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Bigint);
        }

        public static PostgreSQLCopyHelper<TEntity> MapBigInt<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Int64?> propertyGetter)
        {
            return helper.MapNullable(columnName, propertyGetter, NpgsqlDbType.Bigint);
        }

        public static PostgreSQLCopyHelper<TEntity> MapNumeric<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Decimal> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Numeric);
        }

        public static PostgreSQLCopyHelper<TEntity> MapNumeric<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Decimal?> propertyGetter)
        {
            return helper.MapNullable(columnName, propertyGetter, NpgsqlDbType.Numeric);
        }

        public static PostgreSQLCopyHelper<TEntity> MapReal<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Single> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Real);
        }

        public static PostgreSQLCopyHelper<TEntity> MapReal<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Single?> propertyGetter)
        {
            return helper.MapNullable(columnName, propertyGetter, NpgsqlDbType.Real);
        }

        public static PostgreSQLCopyHelper<TEntity> MapDouble<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Double> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Double);
        }

        public static PostgreSQLCopyHelper<TEntity> MapDouble<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Double?> propertyGetter)
        {
            return helper.MapNullable(columnName, propertyGetter, NpgsqlDbType.Double);
        }
    }
}
