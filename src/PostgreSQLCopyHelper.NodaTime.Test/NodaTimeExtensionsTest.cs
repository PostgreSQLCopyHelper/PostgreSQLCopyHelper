// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime;
using Npgsql;
using NUnit.Framework;
using PostgreSQLCopyHelper.NodaTime;
using PostgreSQLCopyHelper.NodaTime.Test.Extensions;

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

            public Interval Interval { get; set; }

            public LocalDate LocalDate { get; set; }
        }

        [Test]
        public void Test_LocalDate()
        {
            CreateTable("date");

            var subject = new PostgreSQLCopyHelper<AllNodaTypesEntity>("sample", "nody_time_test")
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
        public void Test_Instant()
        {
            CreateTable("timestamp");

            var subject = new PostgreSQLCopyHelper<AllNodaTypesEntity>("sample", "nody_time_test")
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
        public void Test_LocalTime()
        {
            CreateTable("time");

      
            var subject = new PostgreSQLCopyHelper<AllNodaTypesEntity>("sample", "nody_time_test")
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

            var subject = new PostgreSQLCopyHelper<AllNodaTypesEntity>("sample", "nody_time_test")
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
            var sqlStatement = $"CREATE TABLE sample.nody_time_test(col_noda {postgresDbType});";

            var sqlCommand = new NpgsqlCommand(sqlStatement, connection);

            return sqlCommand.ExecuteNonQuery();
        }

        private IList<object[]> GetAll()
        {
            return connection.GetAll("sample", "nody_time_test");
        }
    }
}
