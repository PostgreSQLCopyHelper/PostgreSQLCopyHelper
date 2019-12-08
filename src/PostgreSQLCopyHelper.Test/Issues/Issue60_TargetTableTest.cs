using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;
using PostgreSQLCopyHelper.Test.Extensions;

namespace PostgreSQLCopyHelper.Test.Issues
{
    [TestFixture]
    public class Issue60_TargetTableTest
    {
        private class User
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        [Test]
        public void Test_TargetTable_ColumnsEmpty()
        {
            var subject = new PostgreSQLCopyHelper<User>("sample", "TestUsers");

            var targetTable = subject.TargetTable;

            Assert.AreEqual("sample", targetTable.SchemaName);
            Assert.AreEqual("TestUsers", targetTable.TableName);

            Assert.AreEqual(0, targetTable.Columns.Count);
        }

        [Test]
        public void Test_TargetTable_SchemaEmpty_ColumnsEmpty()
        {
            var subject = new PostgreSQLCopyHelper<User>("TestUsers");

            var targetTable = subject.TargetTable;

            Assert.AreEqual(string.Empty, targetTable.SchemaName);
            Assert.AreEqual("TestUsers", targetTable.TableName);

            Assert.AreEqual(0, targetTable.Columns.Count);
        }

        [Test]
        public void Test_TargetTable_NotEmpty()
        {
            var subject = new PostgreSQLCopyHelper<User>("sample", "TestUsers")
                     .MapInteger("Id", x => x.Id)
                     .MapText("Name", x => x.Name);

            var targetTable = subject.TargetTable;

            Assert.AreEqual("sample", targetTable.SchemaName);
            Assert.AreEqual("TestUsers", targetTable.TableName);

            Assert.AreEqual(2, targetTable.Columns.Count);

            Assert.AreEqual("Id", targetTable.Columns[0].ColumnName);
            Assert.AreEqual(NpgsqlDbType.Integer, targetTable.Columns[0].DbType);

            Assert.AreEqual("Name", targetTable.Columns[1].ColumnName);
            Assert.AreEqual(NpgsqlDbType.Text, targetTable.Columns[1].DbType);
        }

        [Test]
        public void Test_TargetTable_FullQualifiedTableName_NoPostgresQuoting()
        {
            var subject = new PostgreSQLCopyHelper<User>("sample", "TestUsers")
                .UsePostgresQuoting(false);

            var targetTable = subject.TargetTable;

            Assert.AreEqual("sample.TestUsers", targetTable.GetFullQualifiedTableName());
        }

        [Test]
        public void Test_TargetTable_FullQualifiedTableName_WithPostgresQuoting()
        {
            var subject = new PostgreSQLCopyHelper<User>("Sample", "TestUsers")
                .UsePostgresQuoting();

            var targetTable = subject.TargetTable;

            Assert.AreEqual("\"Sample\".\"TestUsers\"", targetTable.GetFullQualifiedTableName());
        }
    }
}
