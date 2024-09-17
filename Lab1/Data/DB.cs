using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
namespace Lab1.Data
{
    internal class DB
    {
        SqlConnection sqlConnection = new SqlConnection(@"Data Source=Romaro-PC;Initial Catalog=Lab1;Integrated Security=True;TrustServerCertificate=True");
        public DataTable GetData(string query)
        {
            DataTable dataTable = new DataTable();
            OpenConnection();

            SqlCommand command = new SqlCommand(query, sqlConnection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataTable);


            CloseConnection();
            return dataTable;
        }
        public void OpenConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
        }
        public void CloseConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }
        public SqlConnection GetConnection()
        {
            return sqlConnection;
        }
    }
    
}
