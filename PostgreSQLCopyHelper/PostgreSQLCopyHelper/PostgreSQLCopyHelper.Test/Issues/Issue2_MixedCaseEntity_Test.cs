// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Npgsql;
using NUnit.Framework;
using System;
using PostgreSQLCopyHelper.Test.Extensions;

namespace PostgreSQLCopyHelper.Test.Issues
{
    [Description("A Unit Test to see, if PostgreSQLCopyHelper works with MixedCase Table Definitions.")]
    public class Issue2_MixedCaseEntity_Test : TransactionalTestBase
    {
        public class MixedCaseEntity
        {
            public int Property_One { get; set; }

            public string Property_Two { get; set; }
        }

        private PostgreSQLCopyHelper<MixedCaseEntity> subject;

        protected override void OnSetupInTransaction()
        {
            CreateTable();
        }

        [Test]
        public void Test_MixedCaseEntity_BulkInsert()
        {
            subject = new PostgreSQLCopyHelper<MixedCaseEntity>("sample", "\"MixedCaseEntity\"")
                     .MapInteger("\"Property_One\"", x => x.Property_One)
                     .MapText("\"Property_Two\"", x => x.Property_Two);
            
            // Try to work with the Bulk Inserter:
            var entity0 = new MixedCaseEntity
            {
                Property_One = 44,
                Property_Two = "hello everyone"
            };

            var entity1 = new MixedCaseEntity
            {
                Property_One = 89,
                Property_Two = "Isn't it nice to write in Camel Case!"
            };

            subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = connection.GetAll("sample", "\"MixedCaseEntity\"");

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);

            Assert.IsNotNull(result[0][0]);
            Assert.IsNotNull(result[1][0]);

            Assert.AreEqual(entity0.Property_One, (Int32)result[0][0]);
            Assert.AreEqual(entity0.Property_Two, (string)result[0][1]);

            Assert.AreEqual(entity1.Property_One, (Int32)result[1][0]);
            Assert.AreEqual(entity1.Property_Two, (string)result[1][1]);
        }

        private int CreateTable()
        {
            var sqlStatement = @"CREATE TABLE sample.""MixedCaseEntity""
                                (
                                    ""Property_One"" integer,
                                    ""Property_Two"" text                
                                 );";

            var sqlCommand = new NpgsqlCommand(sqlStatement, connection);

            return sqlCommand.ExecuteNonQuery();
        }

    }
}
