// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Npgsql;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using PostgreSQLCopyHelper.Test.Extensions;

namespace PostgreSQLCopyHelper.Test
{
    [TestFixture]
    public class ArrayTypesTest : TransactionalTestBase
    {
        private class Int16ArrayEntity
        {
            public Int16[] Array { get; set; }
        }

        private class Int32ArrayEntity
        {
            public Int32[] Array { get; set; }
        }

        private class Int64ArrayEntity
        {
            public Int64[] Array { get; set; }
        }

        private class DecimalArrayEntity
        {
            public Decimal[] Array { get; set; }
        }

        private class SingleArrayEntity
        {
            public Single[] Array { get; set; }
        }

        private class DoubleArrayEntity
        {
            public Double[] Array { get; set; }
        }

        private class StringArrayEntity
        {
            public String[] Array { get; set; }
        }

        
        [Test]
        public void Test_StringArray()
        {
            CreateTable("text");

            var subject = new PostgreSQLCopyHelper<StringArrayEntity>("sample", "unit_test")
                .MapArray("col_array", x => x.Array);


            var entity0 = new StringArrayEntity()
            {
                Array = new[] { "A", "B" }
            };

            subject.SaveAll(connection, new[] { entity0 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(1, result.Count);

            // Check if the Result is not null:
            Assert.IsNotNull(result[0][0]);

            // And now check the values:
            var resultArray = (String[])result[0][0];

            Assert.AreEqual("A", resultArray[0]);
            Assert.AreEqual("B", resultArray[1]);
        }

        [Test]
        public void Test_Int16Array()
        {
            CreateTable("smallint");

            var subject = new PostgreSQLCopyHelper<Int16ArrayEntity>("sample", "unit_test")
                .MapArray("col_array", x => x.Array);


            var entity0 = new Int16ArrayEntity()
            {
                Array = new Int16[] { 1, 2 }
            };

            subject.SaveAll(connection, new[] { entity0 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(1, result.Count);

            // Check if the Result is not null:
            Assert.IsNotNull(result[0][0]);

            // And now check the values:
            var resultArray = (Int16[])result[0][0];

            Assert.AreEqual(1, resultArray[0]);
            Assert.AreEqual(2, resultArray[1]);
        }

        [Test]
        public void Test_Int32Array()
        {
            CreateTable("integer");

            var subject = new PostgreSQLCopyHelper<Int32ArrayEntity>("sample", "unit_test")
                .MapArray("col_array", x => x.Array);


            var entity0 = new Int32ArrayEntity()
            {
                Array = new [] { 1, 2 }
            };

            subject.SaveAll(connection, new[] { entity0 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(1, result.Count);

            // Check if the Result is not null:
            Assert.IsNotNull(result[0][0]);

            // And now check the values:
            var resultArray = (Int32[])result[0][0];

            Assert.AreEqual(1, resultArray[0]);
            Assert.AreEqual(2, resultArray[1]);
        }


        [Test]
        public void Test_Int64Array()
        {
            CreateTable("bigint");

            var subject = new PostgreSQLCopyHelper<Int64ArrayEntity>("sample", "unit_test")
                .MapArray("col_array", x => x.Array);


            var entity0 = new Int64ArrayEntity()
            {
                Array = new Int64[] { 1, 2 }
            };

            subject.SaveAll(connection, new[] { entity0 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(1, result.Count);

            // Check if the Result is not null:
            Assert.IsNotNull(result[0][0]);

            // And now check the values:
            var resultArray = (Int64[])result[0][0];

            Assert.AreEqual(1, resultArray[0]);
            Assert.AreEqual(2, resultArray[1]);
        }

        [Test]
        public void Test_DecimalArray()
        {
            CreateTable("numeric");

            var subject = new PostgreSQLCopyHelper<DecimalArrayEntity>("sample", "unit_test")
                .MapArray("col_array", x => x.Array);


            var entity0 = new DecimalArrayEntity()
            {
                Array = new Decimal[] { 1, 2 }
            };

            subject.SaveAll(connection, new [] { entity0 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(1, result.Count);

            // Check if the Result is not null:
            Assert.IsNotNull(result[0][0]);

            // And now check the values:
            var resultArray = (Decimal[])result[0][0];

            Assert.AreEqual(1, resultArray[0]);
            Assert.AreEqual(2, resultArray[1]);
        }

        [Test]
        public void Test_SingleArray()
        {
            CreateTable("real");

            var subject = new PostgreSQLCopyHelper<SingleArrayEntity>("sample", "unit_test")
                .MapArray("col_array", x => x.Array);


            var entity0 = new SingleArrayEntity()
            {
                Array = new Single[] { 1.32f, 2.124f }
            };

            subject.SaveAll(connection, new[] { entity0 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(1, result.Count);

            // Check if the Result is not null:
            Assert.IsNotNull(result[0][0]);

            // And now check the values:
            var resultArray = (Single[])result[0][0];

            Assert.AreEqual(1.32f, resultArray[0], 1e-5);
            Assert.AreEqual(2.124f, resultArray[1], 1e-5);
        }

        [Test]
        public void Test_DoubleArray()
        {
            CreateTable("double precision");

            var subject = new PostgreSQLCopyHelper<DoubleArrayEntity>("sample", "unit_test")
                .MapArray("col_array", x => x.Array);


            var entity0 = new DoubleArrayEntity()
            {
                Array = new Double[] { 1.32, 2.124 }
            };

            subject.SaveAll(connection, new[] { entity0 });

            var result = GetAll();

            // Check if we have the amount of rows:
            Assert.AreEqual(1, result.Count);

            // Check if the Result is not null:
            Assert.IsNotNull(result[0][0]);

            // And now check the values:
            var resultArray = (Double[])result[0][0];

            Assert.AreEqual(1.32, resultArray[0], 1e-5);
            Assert.AreEqual(2.124, resultArray[1], 1e-5);
        }


        private int CreateTable(string arrayType)
        {
            var sqlStatement = string.Format("CREATE TABLE sample.unit_test(col_array {0}[]);", arrayType);

            var sqlCommand = new NpgsqlCommand(sqlStatement, connection);

            return sqlCommand.ExecuteNonQuery();
        }

        private List<object[]> GetAll()
        {
            return connection.GetAll("sample", "unit_test");
        }
    }
}
