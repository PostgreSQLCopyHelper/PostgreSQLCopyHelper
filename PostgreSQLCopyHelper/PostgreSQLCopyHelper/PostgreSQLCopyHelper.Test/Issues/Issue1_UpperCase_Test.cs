using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;
using PostgreSQLCopyHelper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace PostgreSQLCopyHelper.Test.Issues
{
    [TestFixture]
    public class Issue1_UpperCase_Test : TransactionalTestBase
    {
        private class TestEntity
        {
            public Int16 SmallInt { get; set; }
        }

        private PostgreSQLCopyHelper<TestEntity> subject;

        protected override void OnSetupInTransaction()
        {
            CreateTable();
        }

        [Test]
        public void Test_UpperCase_BulkInsert()
        {
            // Use Upper Case Schema Name:
            subject = new PostgreSQLCopyHelper<TestEntity>("SAMPLE", "UNIT_TEST")
                .MapSmallInt("col_smallint", x => x.SmallInt);

            // Try to work with the Bulk Inserter:
            var entity0 = new TestEntity()
            {
                SmallInt = Int16.MinValue
            };

            var entity1 = new TestEntity()
            {
                SmallInt = Int16.MaxValue
            };

            subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);

            Assert.IsNotNull(result[0][0]);
            Assert.IsNotNull(result[1][0]);

            Assert.AreEqual(Int16.MinValue, (Int16)result[0][0]);
            Assert.AreEqual(Int16.MaxValue, (Int16)result[1][0]);
        }

        private int CreateTable()
        {
            var sqlStatement = @"CREATE TABLE sample.unit_test
            (
                col_smallint smallint
            );";

            var sqlCommand = new NpgsqlCommand(sqlStatement, connection);

            return sqlCommand.ExecuteNonQuery();
        }

        private List<object[]> GetAll()
        {
            var sqlStatement = @"SELECT * FROM sample.unit_test";
            var sqlCommand = new NpgsqlCommand(sqlStatement, connection);


            List<object[]> result = new List<object[]>();
            using (var dataReader = sqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    var values = new object[dataReader.FieldCount];
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        values[i] = dataReader[i];
                    }
                    result.Add(values);
                }
            }

            return result;
        }
    }
}
