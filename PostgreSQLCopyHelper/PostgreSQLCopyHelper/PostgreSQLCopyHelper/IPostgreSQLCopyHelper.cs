// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Npgsql;
using System.Collections.Generic;

namespace PostgreSQLCopyHelper
{
    public interface IPostgreSQLCopyHelper<TEntity>
    {
        void SaveAll(NpgsqlConnection connection, IEnumerable<TEntity> entities);
    }
}
