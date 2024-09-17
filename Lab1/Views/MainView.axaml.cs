using Avalonia.Controls;
using System.Data.SqlClient;
using System.Data;
using Lab1.Data;

namespace Lab1.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        
        //Data.DB dataBase = new Data.DB();
        //SqlDataAdapter adapter = new SqlDataAdapter();
        //DataTable table = new DataTable();

        //string querystring = $"SELECT * FROM Costs";
        //dataBase.OpenConnection();
        //SqlCommand command = new SqlCommand(querystring, dataBase.GetConnection());
        //var result = command.ExecuteScalar();
        //adapter.SelectCommand = command;
        //adapter.Fill(table);
    }
}
