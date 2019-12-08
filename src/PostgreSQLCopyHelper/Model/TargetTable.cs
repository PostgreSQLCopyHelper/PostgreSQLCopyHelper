using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostgreSQLCopyHelper.Utils;

namespace PostgreSQLCopyHelper.Model
{
    public class TargetTable
    {
        public string SchemaName { get; set; }

        public string TableName { get; set; }

        public bool UsePostgresQuoting { get; set; }

        public IList<TargetColumn> Columns { get; set; }

        public string GetFullQualifiedTableName()
        {
            return NpgsqlUtils.GetFullQualifiedTableName(SchemaName, TableName, UsePostgresQuoting);
        }
    }
}
