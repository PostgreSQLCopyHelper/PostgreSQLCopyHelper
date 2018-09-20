// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Npgsql;
using System;
using System.Linq;
using System.Collections.Generic;
using NpgsqlTypes;
using PostgreSQLCopyHelper.Model;
using PostgreSQLCopyHelper.Utils;

namespace PostgreSQLCopyHelper
{
    public class PostgreSQLCopyHelper<TEntity> : IPostgreSQLCopyHelper<TEntity>
    {
        private bool usePostgresQuoting;

        private readonly TableDefinition Table;

        private readonly List<ColumnDefinition<TEntity>> Columns;

        public PostgreSQLCopyHelper(string tableName)
            : this(string.Empty, tableName)
        {
        }

        public PostgreSQLCopyHelper(string schemaName, string tableName)
        {
            usePostgresQuoting = false;

            Table = new TableDefinition
            {
                Schema = schemaName,
                TableName = tableName
            };

            Columns = new List<ColumnDefinition<TEntity>>();
        }

        public ulong SaveAll(NpgsqlConnection connection, IEnumerable<TEntity> entities)
        {
            using (var binaryCopyWriter = connection.BeginBinaryImport(GetCopyCommand()))
            {
                WriteToStream(binaryCopyWriter, entities);
                return binaryCopyWriter.Complete();
            }
        }

        public PostgreSQLCopyHelper<TEntity> UsePostgresQuoting(bool enabled = true)
        {
            usePostgresQuoting = enabled;

            return this;
        }

        public PostgreSQLCopyHelper<TEntity> Map<TProperty>(string columnName, Func<TEntity, TProperty> propertyGetter, NpgsqlDbType type)
        {
            return AddColumn(columnName, (writer, entity) => writer.Write(propertyGetter(entity), type));
        }

        public PostgreSQLCopyHelper<TEntity> MapNullable<TProperty>(string columnName, Func<TEntity, TProperty?> propertyGetter, NpgsqlDbType type)
            where TProperty : struct
        {
            return AddColumn(columnName, (writer, entity) =>
            {
                var val = propertyGetter(entity);

                if (!val.HasValue)
                {
                    writer.WriteNull();
                }
                else
                {
                    writer.Write(val.Value, type);
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
            var commaSeparatedColumns = string.Join(", ", Columns.Select(x => x.ColumnName.GetIdentifier(usePostgresQuoting)));

            return string.Format("COPY {0}({1}) FROM STDIN BINARY;",
                Table.GetFullQualifiedTableName(usePostgresQuoting),
                commaSeparatedColumns);
        }
    }
}
