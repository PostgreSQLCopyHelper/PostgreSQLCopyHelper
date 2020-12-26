using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;
using PostgreSQLCopyHelper.Test.Extensions;

namespace PostgreSQLCopyHelper.Test.Issues
{
    [TestFixture]
    public class Issue75_CompositeTypes_Test : TransactionalTestBase
    {
        private class PersonType
        {
            [PgName("first_name")]
            public string FirstName { get; set; }

            [PgName("last_name")]
            public string LastName { get; set; }

            [PgName("birth_date")]
            public DateTime BirthDate { get; set; }
        }

        private PostgreSQLCopyHelper<PersonType> subject;

        protected override void OnSetupInTransaction()
        {
            DropTableAndType(connection, transaction);
            CreateTableAndType(connection, transaction);
        }

        protected override void OnTeardownInTransaction()
        {
            DropTableAndType(connection, transaction);
        }

        [Test]
        public void Test_CompositeBulkInsert()
        {
            connection.ReloadTypes();

            connection.TypeMapper.MapComposite<PersonType>("sample.person_type");

            subject = new PostgreSQLCopyHelper<PersonType>("sample", "CompositeTest")
                     .Map("col_person", x => x);

            var entities = new List<PersonType>();

            entities.Add(new PersonType { FirstName = "Philipp", LastName = "Wagner", BirthDate = new DateTime(1912, 1, 11) });
            entities.Add(new PersonType { FirstName = "Fake", LastName = "Fakerton", BirthDate = new DateTime(1987, 1, 11) });

            // Try to work with the Bulk Inserter:
            subject.SaveAll(connection, entities);

            var result = connection.GetAll("sample", "CompositeTest")
                .Select(x => (PersonType) x[0])
                .OrderBy(x => x.FirstName)
                .ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual("Fake", result[0].FirstName);
            Assert.AreEqual("Fakerton", result[0].LastName);
            Assert.AreEqual(new DateTime(1987, 1, 11), result[0].BirthDate);

            Assert.AreEqual("Philipp", result[1].FirstName);
            Assert.AreEqual("Wagner", result[1].LastName);
            Assert.AreEqual(new DateTime(1912, 1, 11), result[1].BirthDate);
        }

        private void CreateTableAndType(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            {
                var sqlStatement = @"create type sample.person_type as
                                (
                                    first_name text,
                                    last_name text,
                                    birth_date date
                                );";

                var sqlCommand = new NpgsqlCommand(sqlStatement, connection, transaction);

                sqlCommand.ExecuteNonQuery();
            }
            {
                var sqlStatement = @"CREATE TABLE sample.CompositeTest
                                (
                                    col_person sample.person_type                
                                 );";

                var sqlCommand = new NpgsqlCommand(sqlStatement, connection, transaction);

                sqlCommand.ExecuteNonQuery();
            }
        }

        private void DropTableAndType(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            {
                var sqlStatement = @"DROP TABLE IF EXISTS sample.CompositeTest";
                var sqlCommand = new NpgsqlCommand(sqlStatement, connection, transaction);

                sqlCommand.ExecuteNonQuery();
            }
            {
                var sqlStatement = @"DROP TYPE IF EXISTS sample.person_type";
                var sqlCommand = new NpgsqlCommand(sqlStatement, connection, transaction);

                sqlCommand.ExecuteNonQuery();
            }

        }
    }
}
