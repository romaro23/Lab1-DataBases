using Avalonia.Controls;
using System.Data.SqlClient;
using System.Data;
using Lab1.Data;
using DialogHostAvalonia;
using Lab1.ViewModels;
using System;
using Avalonia.Data;

namespace Lab1.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        Button1.Click += Button1_Click;
        CreateCost.Click += CreateCost_Click;
        ClearTable.Click += ClearTable_Click;
        UpdateTables();
    }

    private void ClearTable_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DB db = new DB();
        db.ClearTable("Costs");
        UpdateTables();
    }
    private void UpdateTables()
    {
        Costs.Columns.Clear();
        Costs_type.Columns.Clear();
        Departments.Columns.Clear();
        DB dataBase = new DB();
        DataTable costsTable = dataBase.GetData("SELECT * FROM Costs");
        DataTable costs_typeTable = dataBase.GetData("SELECT * FROM Costs_type");
        DataTable departmentsTable = dataBase.GetData("SELECT * FROM Departments");       
        Costs.ItemsSource = costsTable.DefaultView;
        Costs_type.ItemsSource = costs_typeTable.DefaultView;
        Departments.ItemsSource = departmentsTable.DefaultView;
        foreach (DataColumn dataColumn in costsTable.Columns)
        {
            Costs.Columns.Add(new DataGridTextColumn { Header = dataColumn.ColumnName, Binding = new Binding($"Row.ItemArray[{dataColumn.Ordinal}]") });
        }
        foreach (DataColumn dataColumn in costs_typeTable.Columns)
        {
            Costs_type.Columns.Add(new DataGridTextColumn { Header = dataColumn.ColumnName, Binding = new Binding($"Row.ItemArray[{dataColumn.Ordinal}]") });
        }
        foreach (DataColumn dataColumn in departmentsTable.Columns)
        {
            Departments.Columns.Add(new DataGridTextColumn { Header = dataColumn.ColumnName, Binding = new Binding($"Row.ItemArray[{dataColumn.Ordinal}]") });
        }
    }
    private void CreateCost_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        
        int type_id = int.Parse(Type_idBox.Text);
        int department_id = int.Parse(Department_idBox.Text);
        int amount = int.Parse(AmountBox.Text);
        Type_idBox.Text = "";
        Department_idBox.Text = "";
        AmountBox.Text = "";
        Dialog.IsOpen = false;
        DB dataBase = new DB();
        dataBase.InsertData(type_id, department_id, amount);      
        UpdateTables();
     }

    private void Button1_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Dialog.IsOpen = true;
    }
}
