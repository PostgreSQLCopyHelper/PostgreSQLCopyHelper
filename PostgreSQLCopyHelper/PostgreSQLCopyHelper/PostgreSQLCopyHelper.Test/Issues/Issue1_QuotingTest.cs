// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Npgsql;
using NUnit.Framework;
using PostgreSQLCopyHelper.Test.Extensions;

namespace PostgreSQLCopyHelper.Test.Issues
{
    [TestFixture]
    [Description("A Unit Test to see, if PostgreSQLCopyHelper works with Quoted identifiers.")]
    public class Issue1_QuotingTest : TransactionalTestBase
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
        public void Test_Issue1_QuotingColumnsAndTableNameTest()
        {
            subject = new PostgreSQLCopyHelper<MixedCaseEntity>("sample", "MixedCaseEntity")
                     .UsePostgresQuoting()
                     .MapInteger("Property_One", x => x.Property_One)
                     .MapText("Property_Two", x => x.Property_Two);

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

            var recordsSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = connection.GetAll("sample", "\"MixedCaseEntity\"");

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordsSaved);

            Assert.IsNotNull(result[0][0]);
            Assert.IsNotNull(result[1][0]);

            Assert.AreEqual(entity0.Property_One, (Int32) result[0][0]);
            Assert.AreEqual(entity0.Property_Two, (string) result[0][1]);

            Assert.AreEqual(entity1.Property_One, (Int32) result[1][0]);
            Assert.AreEqual(entity1.Property_Two, (string) result[1][1]);
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
