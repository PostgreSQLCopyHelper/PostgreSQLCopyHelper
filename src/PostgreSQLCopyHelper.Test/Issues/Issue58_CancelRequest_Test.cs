using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using NUnit.Framework;

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
        public async Task Test_CanceledBulkInsertThrowsWhenCanceled()
        {
            subject = new PostgreSQLCopyHelper<User>("sample", "TestUsers")
                     .MapInteger("Id", x => x.Id)
                     .MapText("Name", x => x.Name);

            var cancellationTokenSource = new CancellationTokenSource();

            // Try to work with the Bulk Inserter:
            try
            {
                cancellationTokenSource.CancelAfter(15);
                await subject.SaveAllAsync(connection, FetchUserData(), cancellationTokenSource.Token);
                Assert.Fail("Should Never Reach Here!");
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception)
            {
                Assert.Fail("Should Throw Exception of Type TaskCanceledException!");
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
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
