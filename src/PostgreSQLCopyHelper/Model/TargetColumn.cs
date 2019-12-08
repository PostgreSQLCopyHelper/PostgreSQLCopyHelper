using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpgsqlTypes;

namespace PostgreSQLCopyHelper.Model
{
    public class TargetColumn
    {
        public string ColumnName { get; set; }

        public NpgsqlDbType DbType { get; set; }
    }
}
