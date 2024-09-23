using Avalonia.Controls;
using System.Data.SqlClient;
using System.Data;
using Lab1.Data;
using DialogHostAvalonia;
using Lab1.ViewModels;
using System;
using Avalonia.Data;
using MsBox;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using System.Collections.Generic;
using Avalonia.Media;
using Avalonia.Controls.Platform;
using System.Linq;
using System.Data.Common;
using System.ComponentModel.DataAnnotations;
namespace Lab1.Views;

public partial class MainView : UserControl
{
    public DB DataBase { get; set; } = new DB();
    public MainView()
    {
        InitializeComponent();
        CreateCostButton.Click += CreateCostButton_Click;
        ChangeCostButton.Click += ChangeCostButton_Click;
        CreateCost.Click += CreateCost_Click;
        ChangeCost.Click += ChangeCost_Click;
        ClearTable.Click += ClearTable_Click;
        Dialog.DialogClosing += Dialog_DialogClosing;
        UpdateTables();
    }

    private async void ChangeCost_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (CheckFields())
        {
            var box = MessageBoxManager
          .GetMessageBoxStandard("Warning", "Fields can't be empty",
              ButtonEnum.Ok);
            var result = await box.ShowAsync();
        }
        else
        {
            int cost_id = (int)Cost_idBox.SelectedItem;
            int type_id = int.Parse(Type_idBox.Text);
            int department_id = int.Parse(Department_idBox.Text);
            int amount = int.Parse(AmountBox.Text);
            bool[] conditions = ValidateData(type_id, department_id, amount);
            ShowMessages(conditions);
            if (conditions.All(x => x == true))
            {
                Cost_idBox.SelectedItem = null;
                Type_idBox.Text = "";
                Department_idBox.Text = "";
                AmountBox.Text = "";
                DataBase.ChangeData(cost_id, type_id, department_id, amount);
                UpdateTables();
            }            
        }
    }

    private void Dialog_DialogClosing(object? sender, DialogClosingEventArgs e)
    {
        ChangeCost.IsVisible = false;
        ChangeCost.IsEnabled = false;
        CreateCost.IsVisible = false;
        CreateCost.IsEnabled = false;
        Cost_idBox.IsVisible = false;
        Type_idBox.Text = "";
        Department_idBox.Text = "";
        AmountBox.Text = "";
        Cost_idBox.SelectedItem = null;

    }
    private void SetIdBox()
    {
        DataTable column = DataBase.GetData($"SELECT cost_id from Costs");
        List<int> ids = new List<int>();
        foreach (DataRow row in column.Rows)
        {
            ids.Add((int)row["cost_id"]);
        }
        Cost_idBox.ItemsSource = ids;
    }
    private void IfChangeBox()
    {
        Dialog.IsOpen = true;
        ChangeCost.IsVisible = true;
        ChangeCost.IsEnabled = true;
        Cost_idBox.IsVisible = true;
    }
    private void ChangeCostButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        IfChangeBox();
        SetIdBox();
    }

    private void ClearTable_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DataBase.ClearTable("Costs");
        UpdateTables();
    }
    private async void ShowMessages(bool[] conditions)
    {
        if (!conditions[0])
        {
            var box = MessageBoxManager
          .GetMessageBoxStandard("Warning", "There is no such id in Costs_type",
              ButtonEnum.Ok);
            var result = await box.ShowAsync();
            return;
        }
        if (!conditions[1])
        {
            var box = MessageBoxManager
          .GetMessageBoxStandard("Warning", "There is no such id in Departments",
              ButtonEnum.Ok);
            var result = await box.ShowAsync();
            return;
        }
        if (!conditions[2])
        {
            var box = MessageBoxManager
          .GetMessageBoxStandard("Warning", "Limit is exceeded",
              ButtonEnum.Ok);
            var result = await box.ShowAsync();
            return;
        }
    }
    private bool[] ValidateData(int type_id, int department_id, int amount)
    {
        bool ValidateId(string column, string table, int id)
        {
            DataTable column_ = DataBase.GetData($"SELECT {column} from {table}");
            column_.PrimaryKey = new DataColumn[] { column_.Columns[$"{column}"] };
            if (column_.Rows.Contains(id))
            {
                return true;
            }
            return false;
        }
        bool ValidateAmount(int id, int amount_)
        {
            var amount = DataBase.GetData($"SELECT limit_amount from Costs_type WHERE type_id = {id}");
            if (amount.Rows.Count > 0)
            {
                DataRow row = amount.Rows[0];
                if ((int)row["limit_amount"] > amount_)
                {
                    return true;
                }
            }
            return false;
        }
        bool[] conditions = new bool[3];
        conditions[0] = ValidateId("type_id", "Costs_type", type_id);
        conditions[1] = ValidateId("department_id", "Departments", department_id);
        conditions[2] = ValidateAmount(type_id, amount);
        return conditions;
    }
    private void UpdateTables()
    {
        Costs.Columns.Clear();
        Costs_type.Columns.Clear();
        Departments.Columns.Clear();
        DataTable costsTable = DataBase.GetData("SELECT * FROM Costs");
        DataTable costs_typeTable = DataBase.GetData("SELECT * FROM Costs_type");
        DataTable departmentsTable = DataBase.GetData("SELECT * FROM Departments");
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
    private bool CheckFields()
    {
        return Type_idBox.Text == null || Department_idBox.Text == null || AmountBox.Text == null || Type_idBox.Text == "" || Department_idBox.Text == "" || AmountBox.Text == "";
    }
    private async void CreateCost_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (CheckFields())
        {
            var box = MessageBoxManager
          .GetMessageBoxStandard("Warning", "Fields can't be empty",
              ButtonEnum.Ok);
            var result = await box.ShowAsync();
        }
        else
        {
            int type_id = int.Parse(Type_idBox.Text);
            int department_id = int.Parse(Department_idBox.Text);
            int amount = int.Parse(AmountBox.Text);           
            bool[] conditions = ValidateData(type_id, department_id, amount);
            ShowMessages(conditions);
            if(conditions.All(x => x == true))
            {
                Type_idBox.Text = "";
                Department_idBox.Text = "";
                AmountBox.Text = "";
                DataBase.InsertData(type_id, department_id, amount);
                UpdateTables();
            }
            
        }

    }

    private void CreateCostButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CreateCost.IsVisible = true;
        CreateCost.IsEnabled = true;
        Dialog.IsOpen = true;
    }

    private void DataGrid_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        if (Costs.SelectedItem is DataRowView selectedRow)
        {
            var cost_id = selectedRow["cost_id"];
            SetIdBox();
            IfChangeBox();
            Cost_idBox.SelectedItem = cost_id;
            Type_idBox.Text = selectedRow["type_id"].ToString();
            Department_idBox.Text = selectedRow["department_id"].ToString();
            AmountBox.Text = selectedRow["amount"].ToString();
        }
    }

    private void ComboBox_SelectionChanged_1(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        if (Cost_idBox.SelectedItem != null)
        {
            var row = DataBase.GetData($"SELECT * from Costs WHERE cost_id = {Cost_idBox.SelectedItem}");
            Type_idBox.Text = row.Rows[0]["type_id"].ToString();
            Department_idBox.Text = row.Rows[0]["department_id"].ToString();
            AmountBox.Text = row.Rows[0]["amount"].ToString();
        }

    }
}
