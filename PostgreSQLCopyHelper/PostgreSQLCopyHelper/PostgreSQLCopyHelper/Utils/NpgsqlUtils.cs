// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;

namespace PostgreSQLCopyHelper.Utils
{
    public static class NpgsqlUtils
    {
        public static readonly char QuoteChar = '"';

        public static string QuoteIdentifier(string identifier)
        {
            return RequiresQuoting(identifier)
                ? $"{QuoteChar}{identifier}{QuoteChar}"
                : identifier;
        }

        /// <summary>
        /// Determines, if an identifier required quoting according to the ANSI Standard.
        /// 
        /// This method has been written by the The Npgsql Development Team for the EF Core Integration:
        /// 
        ///     * https://github.com/npgsql/Npgsql.EntityFrameworkCore.PostgreSQL/blob/master/src/EFCore.PG/Storage/Internal/NpgsqlSqlGenerationHelper.cs
        /// 
        /// Please note, I am not checking for the reserved keywords.
        /// </summary>
        /// <param name="identifier">Identifier</param>
        /// <returns>true, iff quoting is required</returns>
        public static bool RequiresQuoting(string identifier)
        {
            var first = identifier.First();
            var last = identifier.Last();

            // This Identifier is already quoted:
            if (first == QuoteChar && last == QuoteChar)
            {
                return false;
            }

            if (!char.IsLower(first) && first != '_')
            {
                return true;
            }

            for (var i = 1; i < identifier.Length; i++)
            {
                var c = identifier[i];

                if (char.IsLower(c))
                {
                    continue;
                }

                switch (c)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '_':
                    case '$': // yes it's true
                        continue;
                }

                return true;
            }

            return false;
        }
    }
}
