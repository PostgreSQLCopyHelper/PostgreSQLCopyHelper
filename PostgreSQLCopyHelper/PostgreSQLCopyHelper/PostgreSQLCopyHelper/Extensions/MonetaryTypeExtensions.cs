using NpgsqlTypes;
// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace PostgreSQLCopyHelper.Extensions
{
    public static class MonetaryTypeExtensions
    {
        public static PostgreSQLCopyHelper<TEntity> MapMoney<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, Decimal> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Money);
        }
    }
}
