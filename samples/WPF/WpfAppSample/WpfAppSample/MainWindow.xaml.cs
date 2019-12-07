using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;
using PostgreSQLCopyHelper;

namespace WpfAppSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class TestEntity
        {
            public string A { get; set; }

            public string B { get; set; }

            public string C { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SynchronousImportButton_Click(object sender, RoutedEventArgs e)
        {
            var helper = CreateCopyHelper();

            using (var connection = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=sampledb;User Id=philipp;Password=test_pwd;"))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    CreateTable(connection);

                    var entities = CreateSampleData(1000000);

                    helper.SaveAll(connection, entities);

                    transaction.Rollback();
                }
            }
        }

        private async void AsynchronousImportButton_Click(object sender, RoutedEventArgs e)
        {
            var helper = CreateCopyHelper();

            using (var connection = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=sampledb;User Id=philipp;Password=test_pwd;"))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    CreateTable(connection);

                    var entities = CreateSampleData(1000000);

                    await helper.SaveAllAsync(connection, entities);

                    await transaction.RollbackAsync();
                }
            }
        }

        private static PostgreSQLCopyHelper<TestEntity> CreateCopyHelper()
        {
            return new PostgreSQLCopyHelper<TestEntity>("sample.issue59")
                .MapText("a", x => x.A)
                .MapText("b", x => x.B);
        }

        private static void CreateTable(NpgsqlConnection connection)
        {
            var sqlStatement = @"CREATE TABLE sample.issue59
            (
                a text,
                b text,
                c text
            );";

            var sqlCommand = new NpgsqlCommand(sqlStatement, connection);

            sqlCommand.ExecuteNonQuery();
        }

        private static IEnumerable<TestEntity> CreateSampleData(int count)
        {
            return Enumerable
                .Range(0, count)
                .Select(x => new TestEntity
                {
                    A = $"A {x}",
                    B = $"B {x}",
                });
        }
    }
}
