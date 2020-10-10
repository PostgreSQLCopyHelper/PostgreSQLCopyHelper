// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using NodaTime;
using Npgsql;
using NUnit.Framework;
using PostgreSQLCopyHelper.NodaTime;
using PostgreSQLCopyHelper.NodaTime.Test.Extensions;

namespace PostgreSQLCopyHelper.NodaTime.Test
{
    [TestFixture]
    public class NodaTimeExtensionsTest : TransactionalTestBase
    {

        public class NodaTimeClass
        {
            public LocalTime LocalTimeValue { get; set; }
        }

        [Test]
        public void Test_LocalTime()
        {
            CreateTable("text");

            var subject = new PostgreSQLCopyHelper<NodaTimeClass>("sample", "nody_time_test")
                .MapTime("col_array", x => x.LocalTimeValue);
        }


        private int CreateTable(string arrayType)
        {
            var sqlStatement = $"CREATE TABLE sample.unit_test(col_array {arrayType}[]);";

            var sqlCommand = new NpgsqlCommand(sqlStatement, connection);

            return sqlCommand.ExecuteNonQuery();
        }

        private IList<object[]> GetAll()
        {
            return connection.GetAll("sample", "unit_test");
        }
    }
}
