// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Npgsql;
using System;
using System.Linq;
using System.Collections.Generic;
using NpgsqlTypes;
using PostgreSQLCopyHelper.Model;

namespace PostgreSQLCopyHelper
{
    public class PostgreSQLCopyHelper<TEntity> : IPostgreSQLCopyHelper<TEntity>
    {
        private TableDefinition Table { get; }

        private List<ColumnDefinition<TEntity>> Columns { get; }

        private bool UseQuotedIdentifiers { get; }

        public PostgreSQLCopyHelper(string tableName, bool useQuotedIdentifiers = false)
            : this(string.Empty, tableName, useQuotedIdentifiers)
        {
        }

        public PostgreSQLCopyHelper(string schemaName, string tableName, bool useQuotedIdentifiers = false)
        {
            UseQuotedIdentifiers = useQuotedIdentifiers;
            Table = new TableDefinition
            {
                Schema = schemaName,
                TableName = tableName
            };

            Columns = new List<ColumnDefinition<TEntity>>();
        }

        public void SaveAll(NpgsqlConnection connection, IEnumerable<TEntity> entities)
        {
            using (var binaryCopyWriter = connection.BeginBinaryImport(GetCopyCommand()))
            {
                WriteToStream(binaryCopyWriter, entities);
                binaryCopyWriter.Complete();
            }
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
            var commaSeparatedColumns = string.Join(", ", Columns.Select(x => UseQuotedIdentifiers ? "\"" + x.ColumnName + "\"" : x.ColumnName));

            return $"COPY {Table.GetFullQualifiedTableName(UseQuotedIdentifiers)}({commaSeparatedColumns}) FROM STDIN BINARY;";
        }
    }
}
