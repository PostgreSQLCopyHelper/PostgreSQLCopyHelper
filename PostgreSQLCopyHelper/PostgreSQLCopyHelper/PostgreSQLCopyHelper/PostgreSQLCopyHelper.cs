// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Npgsql;
using System;
using System.Linq;
using System.Collections.Generic;
using NpgsqlTypes;
using PostgreSQLCopyHelper.Exceptions;
using PostgreSQLCopyHelper.Model;

namespace PostgreSQLCopyHelper
{
    public class PostgreSQLCopyHelper<TEntity> : IPostgreSQLCopyHelper<TEntity>
    {
        private TableDefinition Table { get; set; }

        private List<ColumnDefinition<TEntity>> Columns { get; set; }

        public PostgreSQLCopyHelper(string tableName)
            : this(string.Empty, tableName)
        {
        }

        public PostgreSQLCopyHelper(string schemaName, string tableName)
        {
            Table = new TableDefinition
            {
                Schema = schemaName,
                TableName = tableName
            };

            Columns = new List<ColumnDefinition<TEntity>>();
        }

        public void SaveAll(NpgsqlConnection connection, IEnumerable<TEntity> entities)
        {
            var copyCommand = GetCopyCommand();
            try
            {
                using (var binaryCopyWriter = connection.BeginBinaryImport(copyCommand))
                {
                    WriteToStream(binaryCopyWriter, entities);
                }
            }
            catch (PostgresException e)
            {
                throw new PostgresHelperSaveAllException($"Command \"{copyCommand}\" faild", e);
            }
        }

        public PostgreSQLCopyHelper<TEntity> Map<TProperty>(string columnName, Func<TEntity, TProperty> propertyGetter, NpgsqlDbType type)
        {
            return AddColumn(columnName, (writer, entity) =>
            {
                TProperty value = propertyGetter(entity);
                try
                {
                    writer.Write(value, type);
                }
                catch (NpgsqlException e)
                {
                    throw new PostgresHelperWriteRowException($"Exception occurred while writing \"{value}\" to \"{columnName}\" column", e);
                }
            });
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
            Columns.Add(new ColumnDefinition<TEntity>
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