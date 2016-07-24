// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Npgsql;
using NUnit.Framework;

namespace PostgreSQLCopyHelper.Test
{
    [TestFixture]
    public class TransactionalTestBase
    {
        protected NpgsqlConnection connection;
        private NpgsqlTransaction transaction;

        [SetUp]
        public void Setup()
        {
            OnSetupBeforeTransaction();

            connection = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=sampledb;User Id=philipp;Password=test_pwd;");
            connection.Open();

            transaction = connection.BeginTransaction();

            OnSetupInTransaction();
        }


        protected virtual void OnSetupBeforeTransaction()
        {
        }

        protected virtual void OnSetupInTransaction()
        {
        }

        [TearDown]
        protected void TearDown()
        {
            OnTeardownInTransaction();

            transaction.Rollback();
            transaction.Dispose();

            connection.Close();
            connection.Dispose();

            OnTeardownAfterTransaction();
        }

        protected virtual void OnTeardownInTransaction()
        {
        }

        protected virtual void OnTeardownAfterTransaction()
        {
        }

    }
}
