// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace PostgreSQLCopyHelper
{
    public interface IPostgreSQLCopyHelper<TEntity>
    {
        ulong SaveAll(NpgsqlConnection connection, IEnumerable<TEntity> entities);
        ValueTask<ulong> SaveAllAsync(NpgsqlConnection connection, IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    }
}
