using Npgsql;
using NpgsqlTypes;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PostgreSQLCopyHelper
{
    public class PostgreSQLCopyHelper<TEntity>
    {
        private class TableDefinition
        {
            public string Schema { get; set; }

            public string TableName { get; set; }

            public string GetFullQualifiedTableName()
            {
                if (string.IsNullOrWhiteSpace(Schema))
                {
                    return TableName;
                }
                return string.Format("{0}.{1}", Schema, TableName);
            }

            public override string ToString()
            {
                return string.Format("TableDefinition (Schema = {0}, TableName = {1})", Schema, TableName);
            }
        }

        private class ColumnDefinition
        {
            public string ColumnName { get; set; }

            public Action<NpgsqlBinaryImporter, TEntity> Write { get; set; }

            public override string ToString()
            {
                return string.Format("ColumnDefinition (ColumnName = {0}, Serialize = {1})", ColumnName, Write);
            }
        }

        private TableDefinition Table { get; set; }

        private List<ColumnDefinition> Columns { get; set; }

        public PostgreSQLCopyHelper()
        {
            Columns = new List<ColumnDefinition>();
        }

        public PostgreSQLCopyHelper<TEntity> WithTableName(string schemaName, string tableName)
        {
            Table = new TableDefinition
            {
                Schema = schemaName,
                TableName = tableName
            };
            return this;
        }

        public PostgreSQLCopyHelper<TEntity> AddColumn(string columnName, Func<TEntity, string> propertyGetter)
        {
            AddColumn(columnName, (writer, entity) => writer.Write(propertyGetter(entity), NpgsqlDbType.Text));

            return this;
        }

        public PostgreSQLCopyHelper<TEntity> AddColumn(string columnName, Func<TEntity, System.DateTime> propertyGetter)
        {
            AddColumn(columnName, (writer, entity) => writer.Write(propertyGetter(entity), NpgsqlDbType.Date));

            return this;
        }

        public void SaveAll(NpgsqlConnection connection, IEnumerable<TEntity> entities)
        {
            using (var binaryCopyWriter = connection.BeginBinaryImport(GetCopyCommand()))
            {
                try
                {
                    WriteToStream(binaryCopyWriter, entities);
                }
                catch (Exception)
                {
                    binaryCopyWriter.Cancel();

                    throw;
                }
            }
        }

        private void WriteToStream(NpgsqlBinaryImporter writer, IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                writer.StartRow();

                foreach (var columnDefinition in Columns)
                {
                    columnDefinition.Write(writer, entity);
                }
            }
        }

        private PostgreSQLCopyHelper<TEntity> AddColumn(string columnName, Action<NpgsqlBinaryImporter, TEntity> action)
        {
            Columns.Add(new ColumnDefinition
            {
                ColumnName = columnName,
                Write = action
            });

            return this;
        }

        private string GetCopyCommand()
        {
            var commaSeparatedColumns = string.Join(", ", Columns.Select(x => x.ColumnName));

            return string.Format("COPY {0}({1}) FROM STDIN BINARY;",
                Table.GetFullQualifiedTableName(),
                commaSeparatedColumns);
        }
    }
}