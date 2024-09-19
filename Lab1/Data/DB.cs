﻿using System;
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
        public void ClearTable(params string[] names)
        {
            foreach (string name in names)
            {
                string query = $"delete from {name}";
                string resetQuery = $"DBCC CHECKIDENT ('{name}', RESEED, 0);";
                OpenConnection();
                using (SqlCommand command = new SqlCommand(query, sqlConnection))
                {
                    command.ExecuteNonQuery();  
                }
                using (SqlCommand command = new SqlCommand(resetQuery, sqlConnection))
                {
                    command.ExecuteNonQuery();
                }
                
                CloseConnection();
            }
        }
        public void InsertData(int type_id, int department_id, int amount)
        {
            string query = "INSERT INTO Costs (account_number, type_id, department_id, amount, date_of_cost) " +
                                 "VALUES (@AccountNumber, @TypeId, @DepartmentId, @Amount, @DateOfCost)";
            OpenConnection();
            SqlCommand command = new SqlCommand(query, sqlConnection);
            Random random = new Random();
            command.Parameters.AddWithValue("@AccountNumber", random.Next(1000, 10000));
            command.Parameters.AddWithValue("@TypeId", type_id);
            command.Parameters.AddWithValue("@DepartmentId", department_id);
            command.Parameters.AddWithValue("@Amount", amount);
            command.Parameters.AddWithValue("@DateOfCost", DateTime.Now);
            command.ExecuteNonQuery();
            CloseConnection();
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
