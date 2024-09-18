using Avalonia.Controls;
using System.Data.SqlClient;
using System.Data;
using Lab1.Data;
using DialogHostAvalonia;

namespace Lab1.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        Button1.Click += Button1_Click;

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

    private void Button1_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Dialog.IsOpen = true;
    }
}
