using Npgsql;
using NUnit.Framework;
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
            OnSetupBeforeTransaction();

            connection = new NpgsqlConnection("Server=voicemanda;Port=5432;Database=sampledb;User Id=philipp;Password=test_pwd;");
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
