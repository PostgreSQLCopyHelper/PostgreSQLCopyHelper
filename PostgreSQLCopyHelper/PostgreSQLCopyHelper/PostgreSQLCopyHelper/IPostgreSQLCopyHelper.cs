using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgreSQLCopyHelper
{
    public interface IPostgreSQLCopyHelper<TEntity>
    {
        void SaveAll(NpgsqlConnection connection, IEnumerable<TEntity> entities);
    }
}
