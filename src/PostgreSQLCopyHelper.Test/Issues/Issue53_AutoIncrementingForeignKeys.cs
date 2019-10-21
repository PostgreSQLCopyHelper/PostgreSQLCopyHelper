// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Npgsql;
using NUnit.Framework;
using PostgreSQLCopyHelper.Test.Extensions;

namespace PostgreSQLCopyHelper.Test.Issues
{
    [TestFixture]
    [Description("A Unit Test to see, if PostgreSQLCopyHelper works with Auto Incrementing Foreign Keys. https://github.com/PostgreSQLCopyHelper/PostgreSQLCopyHelper/issues/53")]
    public class Issue53_AutoIncrementingForeignKeys : TransactionalTestBase
    {
        private class Parent
        {
            public int Id;
            public string Name;
        }

        private class Child
        {
            public int Id;
            public int ParentId;
            public string Name;
        }

        protected override void OnSetupInTransaction()
        {
            CreateTables();
        }

        [Test]
        public void Test_Issue53()
        {
            var subject2 = new PostgreSQLCopyHelper<Parent>("sample", "Issue53Parent")
                .MapInteger("Id", x => x.Id)
                .MapText("Name", x => x.Name);

            var subject = new PostgreSQLCopyHelper<Child>("sample", "Issue53Child")
                .MapInteger("Id", x => x.Id)
                .MapInteger("ParentId", x => x.ParentId)
                .MapText("Name", x => x.Name);

            // Try to work with the Bulk Inserter:
            var parentRecordsSaved = subject2.SaveAll(connection, CreateParentData());
            var childRecordsSaved = subject.SaveAll(connection, CreateChildData());

            var parentResults = connection.GetAll("sample", "Issue53Parent");
            var childResults = connection.GetAll("sample", "Issue53Child");

            Assert.AreEqual(2, parentResults.Count);
            Assert.AreEqual(2, parentRecordsSaved);

            Assert.AreEqual(3, childResults.Count);
            Assert.AreEqual(3, childRecordsSaved);
        }

        [Test]
        public void Test_Issue53_Wrong_Order()
        {
            var subject = new PostgreSQLCopyHelper<Child>("sample", "Issue53Child")
                .MapInteger("Id", x => x.Id)
                .MapInteger("ParentId", x => x.ParentId)
                .MapText("Name", x => x.Name);

            const string errorText = "insert or update on table \"issue53child\" violates foreign key constraint \"issue53child_parentid_fkey\"";
            var ex = Assert.Throws<PostgresException>(() => subject.SaveAll(connection, CreateChildData()));
            Assert.That(ex.Message, Is.SupersetOf(errorText));
        }

        [Test]
        public void Test_Issue53_Auto_Increment()
        {
            var subject = new PostgreSQLCopyHelper<Parent>("sample", "Issue53Parent")
                .MapText("Name", x => x.Name);

            // Try to work with the Bulk Inserter:
            var parentData = CreateParentData().ToArray();
            var parentRecordsSaved = subject.SaveAll(connection, parentData);

            var parentResults = connection.GetAll("sample", "Issue53Parent");

            Assert.AreEqual(2, parentResults.Count);
            Assert.AreEqual(2, parentRecordsSaved);

            Assert.AreEqual(parentData[0].Id, (int) parentResults[0][0]);
            Assert.AreEqual(parentData[0].Name, (string) parentResults[0][1]);

            Assert.AreEqual(parentData[1].Id, (int) parentResults[1][0]);
            Assert.AreEqual(parentData[1].Name, (string) parentResults[1][1]);
        }

        private static IEnumerable<Parent> CreateParentData()
        {
            return new List<Parent>
            {
                new Parent
                {
                    Id = 1,
                    Name = "Parent 1",
                },
                new Parent
                {
                    Id = 2,
                    Name = "Parent 2",
                },
            };
        }

        private static IEnumerable<Child> CreateChildData()
        {
            return new List<Child>
            {
                new Child
                {
                    Id = 1,
                    ParentId = 1,
                    Name = "Child 1 of Parent 1",
                },
                new Child
                {
                    Id = 2,
                    ParentId = 1,
                    Name = "Child 2 of Parent 1",
                },
                new Child
                {
                    Id = 1,
                    ParentId = 2,
                    Name = "Child 1 of Parent 2",
                },
            };
        }

        private int CreateTables()
        {
            const string createSequenceStatement = "CREATE SEQUENCE Issue53Parent_Id_Seq;";
            var createSequenceCommand = new NpgsqlCommand(createSequenceStatement, connection);
            var didCreateSequence = createSequenceCommand.ExecuteNonQuery();

            const string createParentTableStatement = @"CREATE TABLE sample.Issue53Parent
                                  (
                                     Id          integer NOT NULL DEFAULT nextval('Issue53Parent_Id_Seq'::regclass),
                                     Name        text NOT NULL,
                                     CONSTRAINT  PK_Issue53Parent PRIMARY KEY(id)
                                   );";
            var createParentTableCommand = new NpgsqlCommand(createParentTableStatement, connection);
            var didCreateParentTable = createParentTableCommand.ExecuteNonQuery();

            const string createChildTableStatement = @"CREATE TABLE sample.Issue53Child
                                (
                                    Id          integer NOT NULL,
                                    ParentId    integer NOT NULL,
                                    Name        text NOT NULL,
                                    FOREIGN KEY (ParentId)
                                            REFERENCES sample.Issue53Parent (Id) MATCH SIMPLE
                                            ON UPDATE NO ACTION
                                            ON DELETE CASCADE
                                 );";
            var createChildTableCommand = new NpgsqlCommand(createChildTableStatement, connection);
            var didCreateChildTable = createChildTableCommand.ExecuteNonQuery();

            return didCreateSequence + didCreateChildTable + didCreateParentTable;
        }
    }
}
