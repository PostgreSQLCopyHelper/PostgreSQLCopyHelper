// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Npgsql;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using PostgreSQLCopyHelper.Test.Extensions;

namespace PostgreSQLCopyHelper.Test
{
    [TestFixture]
    [Description("There is a Bug in Npgsql (https://github.com/npgsql/npgsql/issues/1965). It will most probably be fixed in one of the next version, and these tests will work.")]
    public class NullableBasicDataTypesTest : TransactionalTestBase
    {
        private class TestEntity
        {
            public Int16? SmallInt { get; set; }
            public Int32? Integer { get; set; }
            public Int64? BigInt { get; set; }
            public Decimal? Money { get; set; }
            public DateTime? Timestamp { get; set; }
            public Decimal? Numeric { get; set; }
            public Single? Real { get; set; }
            public Double? DoublePrecision { get; set; }
            public byte[] ByteArray { get; set; }
            public Guid? UUID { get; set; }
            public IPAddress IpAddress { get; set; }
            public PhysicalAddress MacAddress { get; set; }
            public DateTime? Date { get; set; }
            public TimeSpan? TimeSpan { get; set; }
            public string Json { get; set; }
            public string Jsonb { get; set; }
        }

        private PostgreSQLCopyHelper<TestEntity> subject;

        protected override void OnSetupInTransaction()
        {
            CreateTable();

            subject = new PostgreSQLCopyHelper<TestEntity>("sample", "unit_test")
                .MapSmallInt("col_smallint", x => x.SmallInt)
                .MapInteger("col_integer", x => x.Integer)
                .MapMoney("col_money", x => x.Money)
                .MapBigInt("col_bigint", x => x.BigInt)
                .MapTimeStamp("col_timestamp", x => x.Timestamp)
                .MapReal("col_real", x => x.Real)
                .MapDouble("col_double", x => x.DoublePrecision)
                .MapByteArray("col_bytea", x => x.ByteArray)
                .MapUUID("col_uuid", x => x.UUID)
                .MapInetAddress("col_inet", x => x.IpAddress)
                .MapMacAddress("col_macaddr", x => x.MacAddress)
                .MapDate("col_date", x => x.Date)
                .MapInterval("col_interval", x => x.TimeSpan)
                .MapNumeric("col_numeric", x => x.Numeric)
                .MapJson("col_json", x => x.Json)
                .MapJsonb("col_jsonb", x => x.Jsonb);
        }

        [Test]
        public void Test_NullableSmallInt()
        {
            var entity0 = new TestEntity()
            {
                SmallInt = Int16.MinValue
            };

            var entity1 = new TestEntity()
            {
                SmallInt = Int16.MaxValue
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][0]);
            Assert.IsNotNull(result[1][0]);

            Assert.AreEqual(entity0.SmallInt, (Int16)result[0][0]);
            Assert.AreEqual(entity1.SmallInt, (Int16)result[1][0]);
        }

        [Test]
        public void Test_NullableSmallInt_NullFirst()
        {
            var entity0 = new TestEntity()
            {
                SmallInt = null
            };

            var entity1 = new TestEntity()
            {
                SmallInt = Int16.MaxValue
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 }); //Currently fails on this line

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][0]);
            Assert.IsNotNull(result[1][0]);

            Assert.AreEqual(DBNull.Value, result[0][0]);
            Assert.AreEqual(entity1.SmallInt, (Int16)result[1][0]);
        }

        [Test]
        public void Test_NullableInteger()
        {
            var entity0 = new TestEntity()
            {
                Integer = Int32.MinValue
            };

            var entity1 = new TestEntity()
            {
                Integer = Int32.MaxValue
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][1]);
            Assert.IsNotNull(result[1][1]);

            Assert.AreEqual(entity0.Integer, (Int32)result[0][1]);
            Assert.AreEqual(entity1.Integer, (Int32)result[1][1]);
        }

        [Test]
        public void Test_NullableInteger_NullFirst()
        {
            var entity0 = new TestEntity()
            {
                Integer = null
            };

            var entity1 = new TestEntity()
            {
                Integer = Int32.MaxValue
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][1]);
            Assert.IsNotNull(result[1][1]);

            Assert.AreEqual(DBNull.Value, result[0][1]);
            Assert.AreEqual(entity1.Integer, (Int32)result[1][1]);
        }

        [Test]
        public void Test_NullableMoney()
        {
            var entity0 = new TestEntity()
            {
                Money = -1234567890123.45M
            };

            var entity1 = new TestEntity()
            {
                Money = 92233720368547758.07M
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][2]);
            Assert.IsNotNull(result[1][2]);

            Assert.AreEqual(entity0.Money, (Decimal)result[0][2]);
            Assert.AreEqual(entity1.Money, (Decimal)result[1][2]);
        }

        [Test]
        public void Test_NullableMoney_NullFirst()
        {
            var entity0 = new TestEntity()
            {
                Money = null
            };

            var entity1 = new TestEntity()
            {
                Money = 92233720368547758.07M
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][2]);
            Assert.IsNotNull(result[1][2]);

            Assert.AreEqual(DBNull.Value, result[0][2]);
            Assert.AreEqual(entity1.Money, (Decimal)result[1][2]);
        }

        [Test]
        public void Test_NullableNumeric()
        {
            var entity0 = new TestEntity()
            {
                Numeric = Decimal.MinValue
            };

            var entity1 = new TestEntity()
            {
                Numeric = Decimal.MaxValue
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.AreEqual(entity0.Numeric, (Decimal)result[0][9]);
            Assert.AreEqual(entity1.Numeric, (Decimal)result[1][9]);
        }

        [Test]
        public void Test_NullableNumeric_NullFirst()
        {
            var entity0 = new TestEntity()
            {
                Numeric = null
            };

            var entity1 = new TestEntity()
            {
                Numeric = Decimal.MaxValue
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.AreEqual(DBNull.Value, result[0][9]);
            Assert.AreEqual(entity1.Numeric, (Decimal)result[1][9]);
        }

        [Test]
        public void Test_NullableBigint()
        {
            var entity0 = new TestEntity()
            {
                BigInt = Int64.MinValue
            };

            var entity1 = new TestEntity()
            {
                BigInt = Int64.MaxValue
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][3]);
            Assert.IsNotNull(result[1][3]);

            Assert.AreEqual(entity0.BigInt, (Int64)result[0][3]);
            Assert.AreEqual(entity1.BigInt, (Int64)result[1][3]);
        }

        [Test]
        public void Test_NullableBigint_NullFirst()
        {
            var entity0 = new TestEntity()
            {
                BigInt = null
            };

            var entity1 = new TestEntity()
            {
                BigInt = Int64.MaxValue
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][3]);
            Assert.IsNotNull(result[1][3]);

            Assert.AreEqual(DBNull.Value, result[0][3]);
            Assert.AreEqual(entity1.BigInt, (Int64)result[1][3]);
        }

        [Test]
        public void Test_NullableTimestamp()
        {
            var dateTimeToTest = new DateTime(2013, 1, 1, 13, 10, 1, DateTimeKind.Utc);

            var entity0 = new TestEntity()
            {
                Timestamp = dateTimeToTest
            };

            var entity1 = new TestEntity()
            {
                Timestamp = dateTimeToTest.AddDays(1)
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][4]);
            Assert.IsNotNull(result[1][4]);


            var result0 = (DateTime)result[0][4];

            Assert.AreEqual(entity0.Timestamp.Value.Year, result0.Year);
            Assert.AreEqual(entity0.Timestamp.Value.Month, result0.Month);
            Assert.AreEqual(entity0.Timestamp.Value.Day, result0.Day);
            Assert.AreEqual(entity0.Timestamp.Value.Hour, result0.Hour);
            Assert.AreEqual(entity0.Timestamp.Value.Minute, result0.Minute);
            Assert.AreEqual(entity0.Timestamp.Value.Second, result0.Second);

            var result1 = (DateTime)result[1][4];

            Assert.AreEqual(entity1.Timestamp.Value.Year, result1.Year);
            Assert.AreEqual(entity1.Timestamp.Value.Month, result1.Month);
            Assert.AreEqual(entity1.Timestamp.Value.Day, result1.Day);
            Assert.AreEqual(entity1.Timestamp.Value.Hour, result1.Hour);
            Assert.AreEqual(entity1.Timestamp.Value.Minute, result1.Minute);
            Assert.AreEqual(entity1.Timestamp.Value.Second, result1.Second);
        }

        [Test]
        public void Test_NullableTimestamp_NullFirst()
        {
            var dateTimeToTest = new DateTime(2013, 1, 1, 13, 10, 1, DateTimeKind.Utc);

            var entity0 = new TestEntity()
            {
                Timestamp = null
            };

            var entity1 = new TestEntity()
            {
                Timestamp = dateTimeToTest.AddDays(1)
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][4]);
            Assert.IsNotNull(result[1][4]);

            Assert.AreEqual(DBNull.Value, result[0][4]);

            var result1 = (DateTime)result[1][4];

            Assert.AreEqual(entity1.Timestamp.Value.Year, result1.Year);
            Assert.AreEqual(entity1.Timestamp.Value.Month, result1.Month);
            Assert.AreEqual(entity1.Timestamp.Value.Day, result1.Day);
            Assert.AreEqual(entity1.Timestamp.Value.Hour, result1.Hour);
            Assert.AreEqual(entity1.Timestamp.Value.Minute, result1.Minute);
            Assert.AreEqual(entity1.Timestamp.Value.Second, result1.Second);
        }

        [Test]
        public void Test_NullableReal()
        {
            var entity0 = new TestEntity()
            {
                Real = Single.MinValue
            };

            var entity1 = new TestEntity()
            {
                Real = Single.MaxValue
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][5]);
            Assert.IsNotNull(result[1][5]);

            Assert.AreEqual(entity0.Real, (Single)result[0][5]);
            Assert.AreEqual(entity1.Real, (Single)result[1][5]);
        }

        [Test]
        public void Test_NullableReal_NullFirst()
        {
            var entity0 = new TestEntity()
            {
                Real = null
            };

            var entity1 = new TestEntity()
            {
                Real = Single.MaxValue
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][5]);
            Assert.IsNotNull(result[1][5]);

            Assert.AreEqual(DBNull.Value, result[0][5]);
            Assert.AreEqual(entity1.Real, (Single)result[1][5]);
        }

        [Test]
        public void Test_NullableDouble()
        {
            var entity0 = new TestEntity()
            {
                DoublePrecision = Double.MinValue
            };

            var entity1 = new TestEntity()
            {
                DoublePrecision = Double.MaxValue
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.AreEqual(entity0.DoublePrecision, (Double)result[0][6]);
            Assert.AreEqual(entity1.DoublePrecision, (Double)result[1][6]);
        }

        [Test]
        public void Test_NullableDouble_NullFirst()
        {
            var entity0 = new TestEntity()
            {
                DoublePrecision = null
            };

            var entity1 = new TestEntity()
            {
                DoublePrecision = Double.MaxValue
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.AreEqual(DBNull.Value, result[0][6]);
            Assert.AreEqual(entity1.DoublePrecision, (Double)result[1][6]);
        }

        [Test]
        public void Test_ByteArray()
        {
            var entity0 = new TestEntity()
            {
                ByteArray = new byte[] { 1, 2, 3 }
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, recordSaved);

            Assert.AreEqual(entity0.ByteArray[0], ((byte[])result[0][7])[0]);
            Assert.AreEqual(entity0.ByteArray[1], ((byte[])result[0][7])[1]);
            Assert.AreEqual(entity0.ByteArray[2], ((byte[])result[0][7])[2]);
        }

        [Test]
        public void Test_ByteArray_NullArray()
        {
            var entity0 = new TestEntity()
            {
                ByteArray = null
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, recordSaved);
            Assert.AreEqual(DBNull.Value, result[0][7]);
        }

        [Test]
        public void Test_NullableUUID()
        {
            var entity0 = new TestEntity()
            {
                UUID = Guid.NewGuid()
            };

            var entity1 = new TestEntity()
            {
                UUID = Guid.NewGuid()
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.AreEqual(entity0.UUID, (Guid)result[0][8]);
            Assert.AreEqual(entity1.UUID, (Guid)result[1][8]);
        }


        [Test]
        public void Test_NullableUUID_NullFirst()
        {
            var entity0 = new TestEntity()
            {
                UUID = null
            };

            var entity1 = new TestEntity()
            {
                UUID = Guid.NewGuid()
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.AreEqual(DBNull.Value, result[0][8]);
            Assert.AreEqual(entity1.UUID, (Guid)result[1][8]);
        }

        [Test]
        public void Test_NullableDate()
        {
            var entity0 = new TestEntity()
            {
                Date = DateTime.UtcNow
            };

            var entity1 = new TestEntity()
            {
                Date = DateTime.UtcNow
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][12]);
            Assert.IsNotNull(result[1][12]);

            var result0 = (DateTime)result[0][12];

            Assert.AreEqual(entity0.Date.Value.Year, result0.Year);
            Assert.AreEqual(entity0.Date.Value.Month, result0.Month);
            Assert.AreEqual(entity0.Date.Value.Day, result0.Day);

            var result1 = (DateTime)result[1][12];

            Assert.AreEqual(entity1.Date.Value.Year, result1.Year);
            Assert.AreEqual(entity1.Date.Value.Month, result1.Month);
            Assert.AreEqual(entity1.Date.Value.Day, result1.Day);
        }

        [Test]
        public void Test_NullableDate_NullFirst()
        {
            var entity0 = new TestEntity()
            {
                Date = null
            };

            var entity1 = new TestEntity()
            {
                Date = DateTime.UtcNow
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][12]);
            Assert.IsNotNull(result[1][12]);

            Assert.AreEqual(DBNull.Value, result[0][12]);

            var result1 = (DateTime)result[1][12];

            Assert.AreEqual(entity1.Date.Value.Year, result1.Year);
            Assert.AreEqual(entity1.Date.Value.Month, result1.Month);
            Assert.AreEqual(entity1.Date.Value.Day, result1.Day);
        }


        [Test]
        public void Test_IpAddress()
        {
            var entity0 = new TestEntity()
            {
                IpAddress = IPAddress.Parse("1.1.1.1")
            };

            var entity1 = new TestEntity()
            {
                IpAddress = IPAddress.Parse("1.2.3.4")
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][10]);
            Assert.IsNotNull(result[1][10]);

            var result0 = (IPAddress)result[0][10];
            Assert.AreEqual(entity0.IpAddress, result0);

            var result1 = (IPAddress)result[1][10];
            Assert.AreEqual(entity1.IpAddress, result1);
        }

        [Test]
        public void Test_IpAddress_NullFirst()
        {
            var entity0 = new TestEntity()
            {
                IpAddress = null
            };

            var entity1 = new TestEntity()
            {
                IpAddress = IPAddress.Parse("1.2.3.4")
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][10]);
            Assert.IsNotNull(result[1][10]);

            Assert.AreEqual(DBNull.Value, result[0][10]);

            var result1 = (IPAddress)result[1][10];
            Assert.AreEqual(entity1.IpAddress, result1);
        }

        [Test, Explicit("This is a potential Bug in Npgsql")]
        public void Test_MacAddress()
        {
            var entity0 = new TestEntity()
            {
                MacAddress = PhysicalAddress.Parse("08-00-2B-01-02-03")
            };

            var entity1 = new TestEntity()
            {
                MacAddress = PhysicalAddress.Parse("01-02-2B-01-02-03")
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][11]);
            Assert.IsNotNull(result[1][11]);

            var result0 = (PhysicalAddress)result[0][11];
            Assert.AreEqual(entity0.MacAddress, result0);

            var result1 = (PhysicalAddress)result[1][11];
            Assert.AreEqual(entity1.MacAddress, result1);
        }

        [Test, Explicit("This is a potential Bug in Npgsql")]
        public void Test_MacAddress_NullFirst()
        {
            var entity0 = new TestEntity()
            {
                MacAddress = null
            };

            var entity1 = new TestEntity()
            {
                MacAddress = PhysicalAddress.Parse("01-02-2B-01-02-03")
            };

            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][11]);
            Assert.IsNotNull(result[1][11]);

            Assert.AreEqual(DBNull.Value, result[0][11]);

            var result1 = (PhysicalAddress)result[1][11];
            Assert.AreEqual(entity1.MacAddress, result1);
        }

        [Test]
        public void Test_NullableTimeSpan()
        {
            var entity0 = new TestEntity()
            {
                TimeSpan = TimeSpan.FromDays(1)
            };

            var entity1 = new TestEntity()
            {
                TimeSpan = TimeSpan.FromDays(2)
            };


            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.AreEqual(entity0.TimeSpan, (TimeSpan)result[0][13]);
            Assert.AreEqual(entity1.TimeSpan, (TimeSpan)result[1][13]);
        }

        [Test]
        public void Test_NullableTimeSpan_NullFirst()
        {
            var entity0 = new TestEntity()
            {
                TimeSpan = null
            };

            var entity1 = new TestEntity()
            {
                TimeSpan = TimeSpan.FromDays(2)
            };


            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.AreEqual(DBNull.Value, result[0][13]);
            Assert.AreEqual(entity1.TimeSpan, (TimeSpan)result[1][13]);
        }

        [Test]
        public void Test_Json()
        {
            var entity0 = new TestEntity()
            {
                Json = @"{
                          ""firstName"": ""John"",
                          ""lastName"": ""Smith"",
                          ""isAlive"": true
                        }"
            };

            var entity1 = new TestEntity()
            {
                Json = @"{
                          ""firstName"": ""Philipp"",
                          ""lastName"": ""Wagner""
                        }"
            };


            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][14]);
            Assert.IsNotNull(result[1][14]);

            var result0 = (string)result[0][14];
            Assert.AreEqual(entity0.Json, result0);

            var result1 = (string)result[1][14];
            Assert.AreEqual(entity1.Json, result1);
        }

        [Test]
        public void Test_Json_NullFirst()
        {
            var entity0 = new TestEntity()
            {
                Json = null
            };

            var entity1 = new TestEntity()
            {
                Json = @"{
                          ""firstName"": ""Philipp"",
                          ""lastName"": ""Wagner""
                        }"
            };


            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][14]);
            Assert.IsNotNull(result[1][14]);

            Assert.AreEqual(DBNull.Value, result[0][14]);

            var result1 = (string)result[1][14];
            Assert.AreEqual(entity1.Json, result1);
        }

        [Test]
        public void Test_Jsonb()
        {
            var entity0 = new TestEntity()
            {
                Jsonb = @"{
                          ""firstName"": ""John"",
                          ""lastName"": ""Smith"",
                          ""isAlive"": true
                        }"
            };

            var entity1 = new TestEntity()
            {
                Jsonb = @"{
                          ""firstName"": ""Philipp"",
                          ""lastName"": ""Wagner""
                        }"
            };


            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][15]);
            Assert.IsNotNull(result[1][15]);

            // TODO: More Useful Test for JSON equality here...
        }

        [Test]
        public void Test_Jsonb_NullFirst()
        {
            var entity0 = new TestEntity()
            {
                Jsonb = null
            };

            var entity1 = new TestEntity()
            {
                Jsonb = @"{
                          ""firstName"": ""Philipp"",
                          ""lastName"": ""Wagner""
                        }"
            };


            var recordSaved = subject.SaveAll(connection, new[] { entity0, entity1 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, recordSaved);

            Assert.IsNotNull(result[0][15]);
            Assert.IsNotNull(result[1][15]);

            // TODO: More Useful Test for JSON equality here...
        }

        private int CreateTable()
        {
            var sqlStatement = @"CREATE TABLE sample.unit_test
            (
                col_smallint smallint,
                col_integer integer,
                col_money money,
                col_bigint bigint,
                col_timestamp timestamp,
                col_real real,
                col_double double precision,
                col_bytea bytea,
                col_uuid uuid,
                col_numeric numeric,
                col_inet inet,
                col_macaddr macaddr,
                col_date date,
                col_interval interval,
                col_json json,
                col_jsonb jsonb
            );";

            var sqlCommand = new NpgsqlCommand(sqlStatement, connection);

            return sqlCommand.ExecuteNonQuery();
        }

        private IList<object[]> GetAll()
        {
            return connection.GetAll("sample", "unit_test");
        }
    }
}
