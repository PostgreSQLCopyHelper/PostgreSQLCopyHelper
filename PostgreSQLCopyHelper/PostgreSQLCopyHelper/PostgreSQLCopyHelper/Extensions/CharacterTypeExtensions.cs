// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using NpgsqlTypes;

namespace PostgreSQLCopyHelper
{
    public static class CharacterTypeExtensions
    {
        public static PostgreSQLCopyHelper<TEntity> MapVarchar<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, String> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Varchar);
        }

        public static PostgreSQLCopyHelper<TEntity> MapCharacter<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, String> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Char);
        }

        public static PostgreSQLCopyHelper<TEntity> MapText<TEntity>(this PostgreSQLCopyHelper<TEntity> helper, string columnName, Func<TEntity, String> propertyGetter)
        {
            return helper.Map(columnName, propertyGetter, NpgsqlDbType.Text);
        }
    }
}
