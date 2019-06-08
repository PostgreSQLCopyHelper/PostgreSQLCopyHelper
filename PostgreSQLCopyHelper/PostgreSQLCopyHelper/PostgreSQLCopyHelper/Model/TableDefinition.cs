// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using PostgreSQLCopyHelper.Utils;

namespace PostgreSQLCopyHelper.Model
{
    internal class TableDefinition
    {
        public string Schema { get; set; }

        public string TableName { get; set; }

        public string GetFullQualifiedTableName(bool usePostgresQuoting)
        {
            return string.IsNullOrWhiteSpace(Schema)
                ? TableName.GetIdentifier(usePostgresQuoting)
                : $"{Schema.GetIdentifier(usePostgresQuoting)}.{TableName.GetIdentifier(usePostgresQuoting)}";
        }

        public override string ToString()
        {
            return $"TableDefinition (Schema = {Schema}, TableName = {TableName})";
        }
    }
}
