// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;

namespace PostgreSQLCopyHelper.Model
{
    internal class TableDefinition
    {
        public string Schema { get; set; }

        public string TableName { get; set; }

        public string GetFullQualifiedTableName(bool useQuotedIdentifiers)
        {
            var values = new[] { Schema, TableName }
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => useQuotedIdentifiers ? "\"" + x + "\"" : x);

            return string.Join(".", values);
        }

        public override string ToString()
        {
            return string.Format("TableDefinition (Schema = {0}, TableName = {1})", Schema, TableName);
        }
    }
}
