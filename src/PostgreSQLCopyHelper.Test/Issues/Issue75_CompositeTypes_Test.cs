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
        private class SampleEntity
        {
            public string TextColumn { get; set; }

            public PersonType CompositeTypeColumn { get; set; }
        }

        private class PersonType
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public DateTime BirthDate { get; set; }
        }

        private PostgreSQLCopyHelper<SampleEntity> subject;

        protected override void OnSetupInTransaction()
        {
            DropTableAndType();
            CreateTableAndType();
        }

        protected override void OnTeardownInTransaction()
        {
            DropTableAndType();
        }

        [Test]
        public void Test_CompositeBulkInsert()
        {
            connection.ReloadTypes();

            connection.TypeMapper.MapComposite<PersonType>("sample.person_type");

            // ... alternatively you can set it globally at any place in your application using the NpgsqlConnection.GlobalTypeMapper:
            //
            // NpgsqlConnection.GlobalTypeMapper.MapComposite<PersonType>("sample.person_type");

            subject = new PostgreSQLCopyHelper<SampleEntity>("sample", "CompositeTest")
                     .MapText("col_text", x => x.TextColumn)
                     .Map("col_person", x => x.CompositeTypeColumn);

            var entities = new List<SampleEntity>();

            entities.Add(new SampleEntity
            {
                TextColumn = "0",
                CompositeTypeColumn = new PersonType { FirstName = "Fake", LastName = "Fakerton", BirthDate = new DateTime(1987, 1, 11) }
            });
    
            entities.Add(new SampleEntity
            {
                TextColumn = "1",
                CompositeTypeColumn = new PersonType { FirstName = "Philipp", LastName = "Wagner", BirthDate = new DateTime(1912, 1, 11) }
            });

            subject.SaveAll(connection, entities);

            var result = connection.GetAll("SELECT col_text, col_person from sample.CompositeTest")
                .Select(x => new SampleEntity {
                    TextColumn = (string) x[0],
                    CompositeTypeColumn = (PersonType) x[1]
                })
                .OrderBy(x => x.TextColumn)
                .ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual("0", result[0].TextColumn);
            Assert.AreEqual("Fake", result[0].CompositeTypeColumn.FirstName);
            Assert.AreEqual("Fakerton", result[0].CompositeTypeColumn.LastName);
            Assert.AreEqual(new DateTime(1987, 1, 11), result[0].CompositeTypeColumn.BirthDate);

            Assert.AreEqual("1", result[1].TextColumn);
            Assert.AreEqual("Philipp", result[1].CompositeTypeColumn.FirstName);
            Assert.AreEqual("Wagner", result[1].CompositeTypeColumn.LastName);
            Assert.AreEqual(new DateTime(1912, 1, 11), result[1].CompositeTypeColumn.BirthDate);
        }

        private void CreateTableAndType()
        {
            {
                var sqlStatement = @"create type sample.person_type as
                                (
                                    first_name text,
                                    last_name text,
                                    birth_date date
                                );";

                var sqlCommand = new NpgsqlCommand(sqlStatement, connection);

                sqlCommand.ExecuteNonQuery();
            }
            {
                var sqlStatement = @"CREATE TABLE sample.CompositeTest
                                (
                                    col_text text,
                                    col_person sample.person_type                
                                 );";

                var sqlCommand = new NpgsqlCommand(sqlStatement, connection);

                sqlCommand.ExecuteNonQuery();
            }
        }

        private void DropTableAndType()
        {
            {
                var sqlStatement = @"DROP TABLE IF EXISTS sample.CompositeTest";
                var sqlCommand = new NpgsqlCommand(sqlStatement, connection);

                sqlCommand.ExecuteNonQuery();
            }
            {
                var sqlStatement = @"DROP TYPE IF EXISTS sample.person_type";
                var sqlCommand = new NpgsqlCommand(sqlStatement, connection);

                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}
