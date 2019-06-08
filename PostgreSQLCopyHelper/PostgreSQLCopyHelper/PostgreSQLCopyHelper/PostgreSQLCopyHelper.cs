// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using NpgsqlTypes;
using PostgreSQLCopyHelper.Model;
using PostgreSQLCopyHelper.Utils;

namespace PostgreSQLCopyHelper
{
    public class PostgreSQLCopyHelper<TEntity> : IPostgreSQLCopyHelper<TEntity>
    {
        private bool _usePostgresQuoting;

        private readonly TableDefinition _table;

        private readonly List<ColumnDefinition<TEntity>> _columns;

        public PostgreSQLCopyHelper(string tableName)
            : this(string.Empty, tableName)
        {
        }

        public PostgreSQLCopyHelper(string schemaName, string tableName)
        {
            _usePostgresQuoting = false;

            _table = new TableDefinition
            {
                Schema = schemaName,
                TableName = tableName
            };

            _columns = new List<ColumnDefinition<TEntity>>();
        }

        public void SaveAll(NpgsqlConnection connection, IEnumerable<TEntity> entities)
        {
            using (var binaryCopyWriter = connection.BeginBinaryImport(GetCopyCommand()))
            {
                WriteToStream(binaryCopyWriter, entities);
                binaryCopyWriter.Complete();
            }
        }

        public PostgreSQLCopyHelper<TEntity> UsePostgresQuoting(bool enabled = true)
        {
            _usePostgresQuoting = enabled;

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

                foreach (var columnDefinition in _columns)
                {
                    columnDefinition.Write(writer, entity);
                }
            }
        }

        private PostgreSQLCopyHelper<TEntity> AddColumn(string columnName, Action<NpgsqlBinaryImporter, TEntity> action)
        {
            _columns.Add(new ColumnDefinition<TEntity>
            {
                ColumnName = columnName,
                Write = action
            });

            return this;
        }

        private string GetCopyCommand()
        {
            var commaSeparatedColumns = string.Join(", ", _columns.Select(x => x.ColumnName.GetIdentifier(_usePostgresQuoting)));

            return $"COPY {_table.GetFullQualifiedTableName(_usePostgresQuoting)}({commaSeparatedColumns}) FROM STDIN BINARY;";
        }
    }
}
