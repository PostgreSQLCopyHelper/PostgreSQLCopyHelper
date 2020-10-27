// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using NodaTime;
using Npgsql;
using NUnit.Framework;
using PostgreSQLCopyHelper.Test;
using PostgreSQLCopyHelper.Test.Extensions;

namespace PostgreSQLCopyHelper.NodaTime.Test
{
    [TestFixture]
    public class NodaTimeExtensionsTest : TransactionalTestBase
    {
        public class AllNodaTypesEntity
        {
            public LocalTime LocalTime { get; set; }

            public LocalDateTime LocalDateTime { get; set; }

            public ZonedDateTime ZonedDateTime { get; set; }

            public OffsetTime OffsetTime { get; set; }

            public Instant Instant { get; set; }

            public OffsetDateTime OffsetDateTime { get; set; }

            public Period Period { get; set; }

            public LocalDate LocalDate { get; set; }
        }

        [Test]
        public void Test_Interval()
        {
            CreateTable("interval");

            var begin = new LocalDateTime(2020, 1, 23, 0, 12);
            var end = new LocalDateTime(2020, 12, 8, 12, 44);

            var subject = new PostgreSQLCopyHelper<AllNodaTypesEntity>("sample", "noda_time_test")
                .MapInterval("col_noda", x => x.Period);

            var entity = new AllNodaTypesEntity
            {
                Period = Period.Between(begin, end)
            };

            var entities = new[]
            {
                entity
            };

            subject.SaveAll(connection, entities);

            // Check what's written to DB:
            var rows = GetAll();

            var actual = (Period) rows[0][0];

            Assert.AreEqual(entity.Period, actual);
        }

        [Test]
        public void Test_LocalDate()
        {
            CreateTable("date");

            var subject = new PostgreSQLCopyHelper<AllNodaTypesEntity>("sample", "noda_time_test")
                .MapDate("col_noda", x => x.LocalDate);

            var entity = new AllNodaTypesEntity
            {
                LocalDate = new LocalDate(2011, 1, 2)
            };

            var entities = new[]
            {
                entity
            };

            subject.SaveAll(connection, entities);

            // Check what's written to DB:
            var rows = GetAll();

            var actual = (LocalDate) rows[0][0];

            Assert.AreEqual(entity.LocalDate, actual);
        }

        [Test]
        public void Test_LocalDateTime()
        {
            CreateTable("timestamp");

            var subject = new PostgreSQLCopyHelper<AllNodaTypesEntity>("sample", "noda_time_test")
                .MapTimeStamp("col_noda", x => x.LocalDateTime);

            var entity = new AllNodaTypesEntity
            {
                LocalDateTime = new LocalDateTime(2011, 1, 2, 21, 0, 0)
            };

            var entities = new[]
            {
                entity
            };

            subject.SaveAll(connection, entities);

            // Check what's written to DB:
            var rows = GetAll();

            var actual = (Instant) rows[0][0];

            var localTime = entity.LocalDateTime;
            var zonedTime = localTime.InZoneStrictly(DateTimeZone.Utc);
            var expected = zonedTime.ToInstant();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_Instant()
        {
            CreateTable("timestamp");

            var subject = new PostgreSQLCopyHelper<AllNodaTypesEntity>("sample", "noda_time_test")
                .MapTimeStamp("col_noda", x => x.Instant);

            var entity = new AllNodaTypesEntity
            {
                Instant = Instant.FromUtc(2011, 1, 2, 0, 0)
            };

            var entities = new[]
            {
                entity
            };

            subject.SaveAll(connection, entities);

            // Check what's written to DB:
            var rows = GetAll();

            var actual = (Instant) rows[0][0];

            Assert.AreEqual(entity.Instant, actual);
        }

        [Test]
        public void Test_OffsetTime()
        {
            CreateTable("timetz");

            var subject = new PostgreSQLCopyHelper<AllNodaTypesEntity>("sample", "noda_time_test")
                .MapTimeTz("col_noda", x => x.OffsetTime);

            var entity = new AllNodaTypesEntity
            {
                OffsetTime = new OffsetTime(new LocalTime(12, 41), Offset.FromHours(2))
            };

            var entities = new[]
            {
                entity
            };

            subject.SaveAll(connection, entities);

            // Check what's written to DB:
            var rows = GetAll();

            var actual = (OffsetTime) rows[0][0];

            Assert.AreEqual(entity.OffsetTime, actual);
        }

        [Test]
        public void Test_OffsetDateTime()
        {
            CreateTable("timestamptz");

            var subject = new PostgreSQLCopyHelper<AllNodaTypesEntity>("sample", "noda_time_test")
                .MapTimeStampTz("col_noda", x => x.OffsetDateTime);

            var entity = new AllNodaTypesEntity
            {
                OffsetDateTime = new OffsetDateTime(new LocalDateTime(2001, 11, 21, 0, 32), Offset.FromHours(2))
            };

            var entities = new[]
            {
                entity
            };

            subject.SaveAll(connection, entities);

            // Check what's written to DB:
            var rows = GetAll();

            var actual = (Instant) rows[0][0];

            Assert.AreEqual(entity.OffsetDateTime.ToInstant(), actual);
        }

        [Test]
        public void Test_LocalTime()
        {
            CreateTable("time");
            
            var subject = new PostgreSQLCopyHelper<AllNodaTypesEntity>("sample", "noda_time_test")
                .MapTime("col_noda", x => x.LocalTime);

            var entity = new AllNodaTypesEntity
            {
                LocalTime = new LocalTime(2011, 1, 2)
            };

            var entities = new[]
            {
                entity
            };

            subject.SaveAll(connection, entities);

            // Check what's written to DB:
            var rows = GetAll();

            var actual = (LocalTime) rows[0][0];

            Assert.AreEqual(entity.LocalTime, actual);            
        }

        [Test]
        public void Test_ZonedDateTime()
        {
            CreateTable("timestamptz");

            var subject = new PostgreSQLCopyHelper<AllNodaTypesEntity>("sample", "noda_time_test")
                .MapTimeStampTz("col_noda", x => x.ZonedDateTime);

            var timezone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Africa/Kigali");
            var instant = Instant.FromUtc(2011, 1, 5, 22, 50, 0) + Duration.FromMilliseconds(193);

            var entity = new AllNodaTypesEntity
            {
                ZonedDateTime = new ZonedDateTime(instant, timezone)
            };

            var entities = new[]
            {
                entity
            };

            subject.SaveAll(connection, entities);

            // Check what's written to DB:
            var rows = GetAll();

            // TODO: How does Postgres <-> NodaTime convert Timezones? There is a good test here, but 
            // I couldn't see through it yet:
            //
            //    https://github.com/npgsql/npgsql/blob/766658172f08abb0b87a6b7f01a7ea4b49952a29/test/Npgsql.PluginTests/NodaTimeTests.cs
            //
            var actual = (Instant) rows[0][0];

            Assert.AreEqual(instant, actual);
        }

        private int CreateTable(string postgresDbType)
        {
            var sqlStatement = $"CREATE TABLE sample.noda_time_test(col_noda {postgresDbType});";

            var sqlCommand = new NpgsqlCommand(sqlStatement, connection);

            return sqlCommand.ExecuteNonQuery();
        }

        private IList<object[]> GetAll()
        {
            return connection.GetAll("sample", "noda_time_test");
        }
    }
}
