using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using NUnit.Framework;
using PostgreSQLCopyHelper.Test.Extensions;

namespace PostgreSQLCopyHelper.Test.Issues
{
    [TestFixture]
    [Description("A Unit Test to see, if PostgreSQLCopyHelper works with Canceling Requests.")]
    public class Issue58_CancelRequest_Test : TransactionalTestBase
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
        public Task Test_CanceledBulkInsertThrowsWhenCanceledBeforeStarting()
        {
            subject = new PostgreSQLCopyHelper<User>("sample", "TestUsers")
                     .MapInteger("Id", x => x.Id)
                     .MapText("Name", x => x.Name);

            Assert.ThrowsAsync<TaskCanceledException>(async () =>
            {
                using var cancellationTokenSource = new CancellationTokenSource(1);
                await subject.SaveAllAsync(connection, FetchUserData(5), cancellationTokenSource.Token);
            }, $"Should Throw Exception of Type {nameof(TaskCanceledException)}!");

            return Task.CompletedTask;
        }

        [Test]
        public Task Test_CanceledBulkInsertThrowsWhenCanceled()
        {
            subject = new PostgreSQLCopyHelper<User>("sample", "TestUsers")
                     .MapInteger("Id", x => x.Id)
                     .MapText("Name", x => x.Name);

            Assert.ThrowsAsync<TaskCanceledException>(async () =>
            {
                using var cancellationTokenSource = new CancellationTokenSource(15);
                await subject.SaveAllAsync(connection, FetchUserData(10), cancellationTokenSource.Token);
            }, $"Should Throw Exception of Type {nameof(TaskCanceledException)}!");

            return Task.CompletedTask;
        }

        [Test]
        public async Task Test_CanceledBulkInsertDoesNotThrowWhenCancelledAfterCompletion()
        {
            subject = new PostgreSQLCopyHelper<User>("sample", "TestUsers")
                     .MapInteger("Id", x => x.Id)
                     .MapText("Name", x => x.Name);

            var users = new List<User>();

            await foreach (var user in FetchUserData(0))
            {
                users.Add(user);
            }

            using var cancellationTokenSource = new CancellationTokenSource(100);
            var recordsSaved = await subject.SaveAllAsync(connection, FetchUserData(1), cancellationTokenSource.Token);

            var result = connection.GetAll("sample", "TestUsers");

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordsSaved);

            Assert.IsNotNull(result[0][0]);
            Assert.IsNotNull(result[1][0]);

            Assert.AreEqual(users[0].Id, (int) result[0][0]);
            Assert.AreEqual(users[0].Name, (string) result[0][1]);

            Assert.AreEqual(users[1].Id, (int) result[1][0]);
            Assert.AreEqual(users[1].Name, (string) result[1][1]);

            Assert.AreEqual(2, recordsSaved);
        }

        private static async IAsyncEnumerable<User> FetchUserData(int delayMillis)
        {
            for (var i = 1; i <= 2; i++)
            {
                // Simulate waiting for data to come through.
                await Task.Delay(delayMillis);
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
