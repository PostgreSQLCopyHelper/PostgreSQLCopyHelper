using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

            using (var cts = new CancellationTokenSource(1))
            {
                Assert.ThrowsAsync<TaskCanceledException>(async () =>
                    await subject.SaveAllAsync(connection, FetchUserData(5), cts.Token), $"Should Throw Exception of Type {nameof(TaskCanceledException)}!");
            }

            return Task.CompletedTask;
        }

        [Test]
        public Task Test_CanceledBulkInsertThrowsWhenCanceled()
        {
            subject = new PostgreSQLCopyHelper<User>("sample", "TestUsers")
                     .MapInteger("Id", x => x.Id)
                     .MapText("Name", x => x.Name);

            using (var cts = new CancellationTokenSource(15))
            {
                Assert.ThrowsAsync<TaskCanceledException>(async () =>
                    await subject.SaveAllAsync(connection, FetchUserData(10), cts.Token), $"Should Throw Exception of Type {nameof(TaskCanceledException)}!");
            }

            return Task.CompletedTask;
        }

        [Test]
        public async Task Test_CanceledBulkInsertDoesNotThrowWhenCancelledAfterCompletion()
        {
            subject = new PostgreSQLCopyHelper<User>("sample", "TestUsers")
                     .MapInteger("Id", x => x.Id)
                     .MapText("Name", x => x.Name);

            using (var cts = new CancellationTokenSource(50))
            {
                var recordsSaved = await subject.SaveAllAsync(connection, FetchUserData(10), cts.Token);
                var result = connection.GetAll("sample", "\"TestUsers\"")
                    .Cast<User>()
                    .OrderBy(x => x.Id)
                    .ToList();

                Assert.AreEqual(2, recordsSaved);
                Assert.AreEqual(1, result.First().Id);
                Assert.AreEqual(2, result.Last().Id);
            }
        }

        private static async IAsyncEnumerable<User> FetchUserData(int delayMillis, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            for (var i = 1; i <= 2; i++)
            {
                // Simulate waiting for data to come through.
                await Task.Delay(delayMillis, cancellationToken);
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
