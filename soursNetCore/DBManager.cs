using System;
using System.Data;
using System.Data.SqlClient;

namespace DeskAlerts
{
    internal class DBManager
    {
        private static string connectionString;
        private static IDbConnection connection;

        public static LoginsRepository login = new LoginsRepository();
        public static GroupRepository group = new GroupRepository();
        public static AlertRepository alert = new AlertRepository();
        public static Сondition state = new Сondition();
        public static PolicyRepository policy = new PolicyRepository();
        public DBManager() { }

        public DBManager(string dataSource,
            string userName, string userPassword,
            string init_ctalog) {
            var connect_str_builder =
                new SqlConnectionStringBuilder(connectionString) {
                    DataSource = dataSource,
                    UserID = userName,
                    Password = userPassword,
                    InitialCatalog = init_ctalog
                };
            connectionString = connect_str_builder.ConnectionString;
            Console.WriteLine($"Seccesful  {connect_str_builder}");
        }
        //public DBManager(string dataSource,
        //                           string init_ctalog)
        //{
        //    SqlConnectionStringBuilder connect_str_builder =
        //    new SqlConnectionStringBuilder(connectionString);
        //    connect_str_builder.DataSource = dataSource;
        //    connect_str_builder.InitialCatalog = init_ctalog;
        //    connect_str_builder.IntegratedSecurity = true;
        //    connectionString = connect_str_builder.ConnectionString;
        //    Console.WriteLine($"Seccesful  {connect_str_builder.ToString()}");

        //}
        public IDbConnection Connection {
            get {
                OpenConnection();
                return connection;
            }
        }

        public void OpenConnection() {
            if (connection == null)
                connection = new SqlConnection(connectionString);
        }


        public void StatusConnection() {
            Console.WriteLine($"State: {connection.State}");
        }

        public static string getLimitedQuery(string query, string columnName = null, int? offsetLine = null,
            int? limitLine = null) {
            if (columnName == null && limitLine == null && offsetLine == null)
                return query;
            var extendedQuery = string.Empty;
            if (offsetLine != null && limitLine != null && columnName != null)
                extendedQuery =
                    $"({query}) Order by {columnName} OFFSET ({offsetLine}) ROWS FETCH NEXT ({limitLine}) ROWS ONLY";
            else if (offsetLine != null && columnName != null)
                extendedQuery = $"({query}) Order by {columnName} Offset ({offsetLine}) Rows";
            else if (limitLine != null)
                extendedQuery = $"SELECT TOP {limitLine} * FROM ({query}) AS limit_table_on_{limitLine}";

            return extendedQuery;
        }
    }
}