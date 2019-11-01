using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using NUnit.Framework;
using PostgreSQLCopyHelper.Test.Extensions;

namespace PostgreSQLCopyHelper.Test.Issues
{
    [TestFixture]
    [Description("A Unit Test to see, if PostgreSQLCopyHelper works with AsyncEnumerable.")]
    public class Issue56_AsyncEnumerable_Test : TransactionalTestBase
    {
        private class User
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        private PostgreSQLCopyHelper<User> subject;

        protected override void OnSetupInTransaction()
        {
            CreateTable();
        }

        [Test]
        public async Task Test_AsyncEnumerable_BulkInsert()
        {
            subject = new PostgreSQLCopyHelper<User>("sample", "TestUsers")
                     .MapInteger("Id", x => x.Id)
                     .MapText("Name", x => x.Name);

            var recordsSaved = new List<User>();

            await foreach (var user in FetchUserData())
            {
                recordsSaved.Add(user);
            }

            // Try to work with the Bulk Inserter:
            await subject.SaveAllAsync(connection, FetchUserData());

            var result = connection.GetAll("sample", "TestUsers");

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordsSaved.Count);

            Assert.IsNotNull(result[0][0]);
            Assert.IsNotNull(result[1][0]);

            Assert.AreEqual(recordsSaved[0].Id, (int) result[0][0]);
            Assert.AreEqual(recordsSaved[0].Name, (string) result[0][1]);

            Assert.AreEqual(recordsSaved[1].Id, (int) result[1][0]);
            Assert.AreEqual(recordsSaved[1].Name, (string) result[1][1]);
        }

        private static async IAsyncEnumerable<User> FetchUserData()
        {
            for (var i = 1; i <= 2; i++)
            {
                // Simulate waiting for data to come through.
                await Task.Delay(10);
                yield return new User
                {
                    Id = i,
                    Name = $"Username {i}"
                };
            }
        }

        private int CreateTable()
        {
            var sqlStatement = @"CREATE TABLE sample.TestUsers
                                (
                                    Id integer,
                                    Name text                
                                 );";

            var sqlCommand = new NpgsqlCommand(sqlStatement, connection);

            return sqlCommand.ExecuteNonQuery();
        }
    }
}
