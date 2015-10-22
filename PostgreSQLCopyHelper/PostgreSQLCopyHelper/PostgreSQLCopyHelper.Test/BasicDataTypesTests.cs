using Npgsql;
using NUnit.Framework;
using PostgreSQLCopyHelper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgreSQLCopyHelper.Test
{
    [TestFixture]
    public abstract class TransactionalTestBase
    {
        protected NpgsqlConnection connection;
        private NpgsqlTransaction transaction;

        [SetUp]
        protected void Setup()
        {
            connection = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=sampledb;User Id=philipp;Password=test_pwd;");
            connection.Open();

            transaction = connection.BeginTransaction();
        }

        [TearDown]
        protected void TearDown()
        {
            transaction.Rollback();
            transaction.Dispose();

            connection.Close();
            connection.Dispose();
        }
    }

    [TestFixture]
    public class BulkCopyTest : TransactionalTestBase
    {
        private class LocalWeatherData
        {
            public string WBAN { get; set; }

            public DateTime Date { get; set; }

            public string SkyCondition { get; set; }
        }

        [Test(Description = "Inserts Bulk Data into the Database")]
        public void CopyHelperTest()
        {
            var copyHelper = CreateCopyHelper();
            var entities = CreateDataSet(100);

            // Write to a Fresh DB:
            copyHelper.SaveAll(connection, entities);

            // Check if we have the amount of rows:
            Assert.AreEqual(100, GetCount());
        }

        private PostgreSQLCopyHelper<LocalWeatherData> CreateCopyHelper()
        {
            return new PostgreSQLCopyHelper<LocalWeatherData>()
                .WithTableName("sample", "local_weather")
                .MapText("wban", x => x.WBAN)
                .MapText("sky_condition", x => x.SkyCondition)
                .MapDate("date", x => x.Date);
        }

        private IList<LocalWeatherData> CreateDataSet(int numberOfValues)
        {
            DateTime startDate = DateTime.UtcNow;

            return Enumerable.Range(0, numberOfValues)
                .Select(x => new LocalWeatherData()
                {
                    WBAN = string.Format("WBAN {0}", x),
                    SkyCondition = "CLR",
                    Date = startDate.AddMinutes(x)
                }).ToList();
        }

        private long GetCount()
        {
            NpgsqlCommand countQuery = new NpgsqlCommand("SELECT count(*) FROM sample.local_weather", connection);

            return (long)countQuery.ExecuteScalar();
        }
    }
}
