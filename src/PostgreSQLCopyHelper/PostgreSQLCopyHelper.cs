// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public TargetTable TargetTable
        {
            get
            {
                return new TargetTable
                {
                    SchemaName = _table.Schema,
                    TableName = _table.TableName,
                    UsePostgresQuoting = _usePostgresQuoting,
                    Columns = _columns
                        .Select(x => new TargetColumn { ColumnName = x.ColumnName, DbType = x.DbType, ClrType = x.ClrType, DataTypeName = x.DataTypeName })
                        .ToList()
                };
            }
        }

        public ulong SaveAll(NpgsqlConnection connection, IEnumerable<TEntity> entities)
        {
            using (NoSynchronizationContextScope.Enter())
            {
                return DoSaveAllAsync(connection, entities, CancellationToken.None).GetAwaiter().GetResult();
            }
        }

        public ValueTask<ulong> SaveAllAsync(NpgsqlConnection connection, IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new ValueTask<ulong>(Task.FromCanceled<ulong>(cancellationToken));
            }

            using (NoSynchronizationContextScope.Enter())
            {
                return DoSaveAllAsync(connection, entities, cancellationToken);
            }
        }

        public ValueTask<ulong> SaveAllAsync(NpgsqlConnection connection, IAsyncEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new ValueTask<ulong>(Task.FromCanceled<ulong>(cancellationToken));
            }

            using (NoSynchronizationContextScope.Enter())
            {
                return DoSaveAllAsync(connection, entities, cancellationToken);
            }
        }

        private async ValueTask<ulong> DoSaveAllAsync(NpgsqlConnection connection, IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await using var binaryCopyWriter = connection.BeginBinaryImport(GetCopyCommand());
            await WriteToStreamAsync(binaryCopyWriter, entities, cancellationToken);

            return await binaryCopyWriter.CompleteAsync(cancellationToken);
        }

        private async ValueTask<ulong> DoSaveAllAsync(NpgsqlConnection connection, IAsyncEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await using var binaryCopyWriter = connection.BeginBinaryImport(GetCopyCommand());
            await WriteToStreamAsync(binaryCopyWriter, entities, cancellationToken);

            return await binaryCopyWriter.CompleteAsync(cancellationToken);
        }

        public PostgreSQLCopyHelper<TEntity> UsePostgresQuoting(bool enabled = true)
        {
            _usePostgresQuoting = enabled;

            return this;
        }

        public PostgreSQLCopyHelper<TEntity> Map<TProperty>(string columnName, Func<TEntity, TProperty> propertyGetter)
        {
            return AddColumn(columnName, (writer, entity, cancellationToken) => writer.WriteAsync(propertyGetter(entity), cancellationToken), clrType: typeof(TProperty));
        }


        public PostgreSQLCopyHelper<TEntity> Map<TProperty>(string columnName, Func<TEntity, TProperty> propertyGetter, NpgsqlDbType dbType)
        {
            return AddColumn(columnName, (writer, entity, cancellationToken) => writer.WriteAsync(propertyGetter(entity), dbType, cancellationToken), dbType, typeof(TProperty));
        }

        public PostgreSQLCopyHelper<TEntity> Map<TProperty>(string columnName, Func<TEntity, TProperty> propertyGetter, string dataTypeName)
        {
            return AddColumn(columnName, (writer, entity, cancellationToken) => writer.WriteAsync(propertyGetter(entity), dataTypeName, cancellationToken), clrType: typeof(TProperty), dataTypeName: dataTypeName);
        }

        public PostgreSQLCopyHelper<TEntity> MapNullable<TProperty>(string columnName, Func<TEntity, TProperty?> propertyGetter, NpgsqlDbType dbType)
            where TProperty : struct
        {
            return AddColumn(columnName, async (writer, entity, cancellationToken) =>
            {
                var val = propertyGetter(entity);

                if (!val.HasValue)
                {
                    await writer.WriteNullAsync(cancellationToken);
                }
                else
                {
                    await writer.WriteAsync(val.Value, dbType, cancellationToken);
                }
            }, dbType, typeof(TProperty));
        }

        private async Task WriteToStreamAsync(NpgsqlBinaryImporter writer, IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            foreach (var entity in entities)
            {
                await WriteToStreamAsync(writer, entity, cancellationToken);
            }
        }

        private async Task WriteToStreamAsync(NpgsqlBinaryImporter writer, IAsyncEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await foreach (var entity in entities.WithCancellation(cancellationToken))
            {
                await WriteToStreamAsync(writer, entity, cancellationToken);
            }
        }

        private async Task WriteToStreamAsync(NpgsqlBinaryImporter writer, TEntity entity, CancellationToken cancellationToken)
        {
            await writer.StartRowAsync(cancellationToken);

            foreach (var columnDefinition in _columns)
            {
                await columnDefinition.WriteAsync(writer, entity, cancellationToken);
            }
        }

        private PostgreSQLCopyHelper<TEntity> AddColumn(string columnName, Func<NpgsqlBinaryImporter, TEntity, CancellationToken, Task> action, NpgsqlDbType? dbType = default, Type clrType = default, string dataTypeName = default)
        {
            _columns.Add(new ColumnDefinition<TEntity>
            {
                ColumnName = columnName,
                DbType = dbType,
                DataTypeName = dataTypeName,
                ClrType = clrType,
                WriteAsync = action
            });

            return this;
        }

        private string GetCopyCommand()
        {
            var commaSeparatedColumns = string.Join(", ", _columns.Select(x => x.ColumnName.GetIdentifier(_usePostgresQuoting)));

            return $"COPY {_table.GetFullyQualifiedTableName(_usePostgresQuoting)}({commaSeparatedColumns}) FROM STDIN BINARY;";
        }


    }
}
