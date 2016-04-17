// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace PostgreSQLCopyHelper.Model
{
    internal class TableDefinition
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
}
