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
    public class NullableNodaTimeExtensionsTest : TransactionalTestBase
    {
        public class NullableNodaTypesEntity
        {
            public LocalTime? LocalTime { get; set; }

            public LocalDateTime? LocalDateTime { get; set; }

            public ZonedDateTime? ZonedDateTime { get; set; }

            public OffsetTime? OffsetTime { get; set; }

            public Instant? Instant { get; set; }

            public OffsetDateTime? OffsetDateTime { get; set; }

            public LocalDate? LocalDate { get; set; }
        }

        [Test]
        public void Test_LocalDate()
        {
            CreateTable("date");

            var subject = new PostgreSQLCopyHelper<NullableNodaTypesEntity>("sample", "noda_time_test")
                .MapDate("col_noda", x => x.LocalDate);

            var entity = new NullableNodaTypesEntity
            {
                LocalDate = null
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

            var subject = new PostgreSQLCopyHelper<NullableNodaTypesEntity>("sample", "noda_time_test")
                .MapTimeStamp("col_noda", x => x.LocalDateTime);

            var entity = new NullableNodaTypesEntity
            {
                LocalDateTime = null
            };

            var entities = new[]
            {
                entity
            };

            subject.SaveAll(connection, entities);

            // Check what's written to DB:
            var rows = GetAll();

            var actual = (Instant) rows[0][0];

            var expected = entity.LocalDateTime;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_Instant()
        {
            CreateTable("timestamp");

            var subject = new PostgreSQLCopyHelper<NullableNodaTypesEntity>("sample", "noda_time_test")
                .MapTimeStamp("col_noda", x => x.Instant);

            var entity = new NullableNodaTypesEntity
            {
                Instant = null
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

            var subject = new PostgreSQLCopyHelper<NullableNodaTypesEntity>("sample", "noda_time_test")
                .MapTimeTz("col_noda", x => x.OffsetTime);

            var entity = new NullableNodaTypesEntity
            {
                OffsetTime = null
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

            var subject = new PostgreSQLCopyHelper<NullableNodaTypesEntity>("sample", "noda_time_test")
                .MapTimeStampTz("col_noda", x => x.OffsetDateTime);

            var entity = new NullableNodaTypesEntity
            {
                OffsetDateTime = null
            };

            var entities = new[]
            {
                entity
            };

            subject.SaveAll(connection, entities);

            // Check what's written to DB:
            var rows = GetAll();

            var actual = (Instant) rows[0][0];

            Assert.AreEqual(entity.OffsetDateTime, actual);
        }

        [Test]
        public void Test_LocalTime()
        {
            CreateTable("time");
            
            var subject = new PostgreSQLCopyHelper<NullableNodaTypesEntity>("sample", "noda_time_test")
                .MapTime("col_noda", x => x.LocalTime);

            var entity = new NullableNodaTypesEntity
            {
                LocalTime = null
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

            var subject = new PostgreSQLCopyHelper<NullableNodaTypesEntity>("sample", "noda_time_test")
                .MapTimeStampTz("col_noda", x => x.ZonedDateTime);

            var entity = new NullableNodaTypesEntity
            {
                ZonedDateTime = null
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

            Assert.AreEqual(entity.ZonedDateTime, actual);
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
