// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Npgsql;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
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
        private static readonly Type ByteType = typeof(byte);
        private static readonly Type CharType = typeof(char);
        private static readonly Type ShortType = typeof(short);
        private static readonly Type IntType = typeof(int);
        private static readonly Type LongType = typeof(long);
        private static readonly Type StringType = typeof(string);
        private static readonly Type CharArrayType = typeof(char[]);
        private static readonly Type DateTimeType = typeof(DateTime);
        private static readonly Type TimeSpanType = typeof(TimeSpan);
        private static readonly Type DateTimeOffsetType = typeof(DateTimeOffset);
        private static readonly Type IPAddresType = typeof(IPAddress);
        private static readonly Type NpgsqlInetType = typeof(NpgsqlInet);
        private static readonly Type NpgsqlDateType = typeof(NpgsqlDate);
        private static readonly Type FloatType = typeof(float);
        private static readonly Type DoubleType = typeof(double);
        private static readonly Type BoolType = typeof(bool);

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

        public void SaveAll(NpgsqlConnection connection, IEnumerable<TEntity> entities)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            string command = null;
            try
            {
                command = GetCopyCommand();
                using (var binaryCopyWriter = connection.BeginBinaryImport(command))
                {
                    WriteToStream(binaryCopyWriter, entities);
                    binaryCopyWriter.Complete();
                }
            }
            catch (Exception ex)
            {
                ex.Data["Command"] = command;

                ex.Data["CommandTimeout"] = connection.CommandTimeout;
                ex.Data["ConnectionTimeout"] = connection.ConnectionTimeout;

                ex.Data["FullState"] = connection.FullState;
                ex.Data["State"] = connection.State;

                ex.Data["Host"] = connection.Host;
                ex.Data["Port"] = connection.Port;
                ex.Data["UserName"] = connection.UserName;
                ex.Data["Database"] = connection.Database;

                ex.Data["PostgreSqlVersion"] = connection.PostgreSqlVersion;
                ex.Data["ServerVersion"] = connection.ServerVersion;
                ex.Data["ProcessID"] = connection.ProcessID;

                throw;
            }
        }

        public PostgreSQLCopyHelper<TEntity> UsePostgresQuoting(bool enabled = true)
        {
            usePostgresQuoting = enabled;

            return this;
        }

        public PostgreSQLCopyHelper<TEntity> Map<TProperty>(string columnName, Func<TEntity, TProperty> propertyGetter, NpgsqlDbType type)
        {
            Type returnType = propertyGetter.Method.ReturnType;
            if (Nullable.GetUnderlyingType(returnType) != null)
            {
                throw new ArgumentOutOfRangeException(nameof(propertyGetter), "Can´t register nulleable type with Map method.");
            }

            this.ValidateMapping(type, returnType);

            // Console.WriteLine("columnName: {0} type: {1} extractor: {2}", columnName, type, propertyGetter.Method.ReturnType.FullName);
            return AddColumn(columnName, (writer, entity) => writer.Write(propertyGetter(entity), type));
        }

        public PostgreSQLCopyHelper<TEntity> MapNullable<TProperty>(string columnName, Func<TEntity, TProperty?> propertyGetter, NpgsqlDbType type)
            where TProperty : struct
        {
            Type returnType = Nullable.GetUnderlyingType(propertyGetter.Method.ReturnType);
            this.ValidateMapping(type, returnType);
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

        private void ValidateMapping(NpgsqlDbType type, Type returnType)
        {
            bool isValid = true;
            switch (type)
            {
                case NpgsqlDbType.Bigint:
                    isValid = returnType == ByteType
                            || returnType == ShortType
                            || returnType == IntType
                            || returnType == LongType;
                    break;
                case NpgsqlDbType.Double:
                    isValid = returnType == FloatType
                           || returnType == DoubleType;
                    break;
                case NpgsqlDbType.Integer:
                    isValid = returnType == ByteType
                              || returnType == ShortType
                              || returnType == IntType;
                    break;

                case NpgsqlDbType.Real:
                    isValid = returnType == ByteType
                              || returnType == ShortType
                              || returnType == IntType
                              || returnType == LongType
                              || returnType == FloatType;
                    break;
                case NpgsqlDbType.Smallint:
                    isValid = returnType == ByteType
                              || returnType == ShortType;
                    break;
                case NpgsqlDbType.Numeric:
                case NpgsqlDbType.Money:
                    isValid = returnType == ByteType
                              || returnType == ShortType
                              || returnType == IntType
                              || returnType == LongType
                              || returnType == FloatType
                              || returnType == DoubleType
                              || returnType == typeof(decimal);
                    break;
                case NpgsqlDbType.Boolean:
                    isValid = returnType == BoolType;
                    break;
                case NpgsqlDbType.Box:
                    isValid = returnType == typeof(NpgsqlBox);
                    break;
                case NpgsqlDbType.Circle:
                    isValid = returnType == typeof(NpgsqlCircle);
                    break;
                case NpgsqlDbType.Line:
                    isValid = returnType == typeof(NpgsqlLine);
                    break;
                case NpgsqlDbType.LSeg:
                    isValid = returnType == typeof(NpgsqlLSeg);
                    break;
                case NpgsqlDbType.Path:
                    isValid = returnType == typeof(NpgsqlPath);
                    break;
                case NpgsqlDbType.Point:
                    isValid = returnType == typeof(NpgsqlPoint);
                    break;
                case NpgsqlDbType.Polygon:
                    isValid = returnType == typeof(NpgsqlPolygon);
                    break;
                case NpgsqlDbType.Char:
                    isValid = returnType == CharType
                           || returnType == ByteType
                           || returnType == ShortType
                           || returnType == IntType
                           || returnType == LongType;
                    break;
                case NpgsqlDbType.Text:
                case NpgsqlDbType.Varchar:
                case NpgsqlDbType.Name:
                    isValid = returnType == StringType
                           || returnType == CharType
                           || returnType == CharArrayType;
                    break;
                case NpgsqlDbType.Citext:
                    isValid = returnType == StringType
                              || returnType == CharArrayType;
                    break;
                case NpgsqlDbType.InternalChar:
                case NpgsqlDbType.Bytea:
                    isValid = returnType == typeof(byte[]);
                    break;
                case NpgsqlDbType.Date:
                    isValid = returnType == DateTimeType
                           || returnType == NpgsqlDateType;
                    break;
                case NpgsqlDbType.Time:
                    isValid = returnType == TimeSpanType;
                    break;
                case NpgsqlDbType.Timestamp:
                case NpgsqlDbType.TimestampTZ:
                    isValid = returnType == DateTimeType
                           || returnType == DateTimeOffsetType
                           || returnType == NpgsqlDateType;
                    break;
                case NpgsqlDbType.Interval:
                    isValid = returnType == TimeSpanType
                           || returnType == typeof(NpgsqlTimeSpan);
                    break;
                case NpgsqlDbType.TimeTZ:
                    isValid = returnType == DateTimeType
                           || returnType == DateTimeOffsetType
                           || returnType == TimeSpanType;
                    break;
                case NpgsqlDbType.Abstime:
                    break;
                case NpgsqlDbType.Inet:
                    isValid = returnType == IPAddresType
                           || returnType == NpgsqlInetType;
                    break;
                case NpgsqlDbType.Cidr:
                    isValid = returnType == typeof(ValueTuple<IPAddress, int>)
                           || returnType == IPAddresType
                           || returnType == NpgsqlInetType;
                    break;
                case NpgsqlDbType.MacAddr:
                    isValid = returnType == typeof(PhysicalAddress);
                    break;
                case NpgsqlDbType.MacAddr8:
                    break;
                case NpgsqlDbType.Bit:
                case NpgsqlDbType.Varbit:
                    isValid = returnType == BoolType
                              || returnType == StringType
                              || returnType == typeof(BitArray);
                    break;
                case NpgsqlDbType.TsVector:
                    isValid = returnType == typeof(NpgsqlTsVector);
                    break;
                case NpgsqlDbType.TsQuery:
                    isValid = returnType == typeof(NpgsqlTsQuery);
                    break;
                case NpgsqlDbType.Uuid:
                    isValid = returnType == typeof(Guid);
                    break;
                case NpgsqlDbType.Xml:
                case NpgsqlDbType.Json:
                case NpgsqlDbType.Jsonb:
                    isValid = returnType == StringType
                           || returnType == CharArrayType
                           || returnType == CharType;
                    break;
                case NpgsqlDbType.Hstore:
                    isValid = typeof(IDictionary<string, string>).IsAssignableFrom(returnType);
                    break;
                case NpgsqlDbType.Array:
                    isValid = returnType == typeof(Array)
                           || typeof(IList<>).IsAssignableFrom(returnType)
                           || typeof(IList).IsAssignableFrom(returnType);
                    break;
                case NpgsqlDbType.Range:
                    isValid = typeof(NpgsqlRange<>).IsAssignableFrom(returnType);
                    break;
                case NpgsqlDbType.Refcursor:
                    break;
                case NpgsqlDbType.Oidvector:
                    isValid = returnType == typeof(uint[]);
                    break;
                case NpgsqlDbType.Int2Vector:
                    break;
                case NpgsqlDbType.Oid:
                case NpgsqlDbType.Xid:
                case NpgsqlDbType.Cid:
                    isValid = returnType == typeof(uint);
                    break;
                case NpgsqlDbType.Regtype:
                    break;
                case NpgsqlDbType.Tid:
                    break;
                case NpgsqlDbType.Unknown:
                    break;
                case NpgsqlDbType.Geometry:
                    break;
                case NpgsqlDbType.Geography:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            if (!isValid)
            {
                InvalidOperationException exception = new InvalidOperationException($"Type: {type} is not assignable from {returnType.FullName}");
                exception.Data["DbType"] = type;
                exception.Data["ClrType"] = returnType;
                throw exception;
            }
        }

        private void WriteToStream(NpgsqlBinaryImporter writer, IEnumerable<TEntity> entities)
        {
            long rowNumber = 1;
            long colNumber = 1;
            string colName = null;

            IEnumerable<TEntity> enumerable = entities as TEntity[] ?? entities.ToArray();
            try
            {
                foreach (var entity in enumerable)
                {
                    writer.StartRow();
                    colNumber = 1;
                    foreach (var columnDefinition in Columns)
                    {
                        colName = columnDefinition.ColumnName;
                        columnDefinition.Write(writer, entity);
                        colNumber++;
                    }

                    rowNumber++;
                }
            }
            catch (Exception ex)
            {
                ex.Data["RowNumber"] = rowNumber;
                ex.Data["ColumnNumber"] = colNumber;
                ex.Data["ColumnName"] = colName;
                ex.Data["TotalRows"] = enumerable.Count();
                throw;
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
