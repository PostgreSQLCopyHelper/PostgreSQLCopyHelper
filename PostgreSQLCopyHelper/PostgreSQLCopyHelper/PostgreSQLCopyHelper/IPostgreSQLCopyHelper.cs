// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Npgsql;

namespace PostgreSQLCopyHelper
{
    public interface IPostgreSQLCopyHelper<TEntity>
    {
        void SaveAll(NpgsqlConnection connection, IEnumerable<TEntity> entities);
    }
}
