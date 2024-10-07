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
using ReactiveUI;
namespace Lab1.Views;

public partial class MainView : UserControl
{
    public DB DataBase { get; set; } = new DB();
    Dictionary<string, string> queries = new Dictionary<string, string>();
    public MainView()
    {
        InitializeComponent();
        CreateCostButton.Click += CreateCostButton_Click;
        ChangeCostButton.Click += ChangeCostButton_Click;
        DeleteCostButton.Click += DeleteCostButton_Click;
        QueryButton.Click += QueryButton_Click;
        CreateCost.Click += CreateCost_Click;
        ChangeCost.Click += ChangeCost_Click;
        DeleteCost.Click += DeleteCost_Click;
        Query.Click += Query_Click;
        Dialog.DialogClosing += Dialog_DialogClosing;
        UpdateTables();
    }    
    private void Dialog_DialogClosing(object? sender, DialogClosingEventArgs e)
    {
        ChangeCost.IsVisible = false;
        CreateCost.IsVisible = false;
        DeleteCost.IsVisible = false;
        Query.IsVisible = false;
        Cost_idBox.IsVisible = false;
        Type_idBox.IsHitTestVisible = true;
        AmountBox.IsHitTestVisible = true;
        Type_idBox.IsVisible = true;
        Department_idBox.IsVisible = true;
        AmountBox.IsVisible = true;
        Department_idBox.IsHitTestVisible = true;
        Type_idBox.Text = "";
        Department_idBox.Text = "";
        AmountBox.Text = "";
        Cost_idBox.SelectedItem = null;
        DialogContent.Width = 120;
        Cost_idBox.Width = 120;
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
        Cost_idBox.PlaceholderText = "Cost_id";
    }
    private void IfChangeBox()
    {
        Dialog.IsOpen = true;
        ChangeCost.IsVisible = true;
        Cost_idBox.IsVisible = true;
    }
    private void IfDeleteBox()
    {
        Dialog.IsOpen = true;
        Type_idBox.IsHitTestVisible = false;
        AmountBox.IsHitTestVisible = false;
        Department_idBox.IsHitTestVisible = false;
        Cost_idBox.IsVisible = true;
        DeleteCost.IsVisible = true;
    }
    private void IfCreateBox()
    {
        Dialog.IsOpen = true;
        CreateCost.IsVisible = true;
    }
    private void IfQueryBox()
    {
        Dialog.IsOpen = true;
        Type_idBox.IsVisible = false;
        Department_idBox.IsVisible = false;
        AmountBox.IsVisible = false;
        Query.IsVisible = true;
        Cost_idBox.IsVisible = true;
        Cost_idBox.PlaceholderText = "Query";
        DialogContent.Width = 250;
        Cost_idBox.Width = 250;
    }
    private void RestoreFields()
    {
        Cost_idBox.SelectedItem = null;
        Type_idBox.Text = "";
        Department_idBox.Text = "";
        AmountBox.Text = "";
    }
    private void CreateCostButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        IfCreateBox();
    }
    private void ChangeCostButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        IfChangeBox();
        SetIdBox();
    }
    private void DeleteCostButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SetIdBox();
        IfDeleteBox();
    

    }
    private void QueryButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        IfQueryBox();
        SetQueries();
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
    private void SetQueries()
    {
        queries.Clear();
        queries.Add("Type id = 2", "SELECT * FROM Costs WHERE type_id = 2");
        queries.Add("Employees < 20", "SELECT * FROM Departments WHERE number_of_employees < 20");
        queries.Add("Limit > 2000", "SELECT * FROM Costs_type WHERE limit_amount > 2000");
        queries.Add("EmployeesNum by DESC", "SELECT * FROM Departments ORDER BY number_of_employees DESC");
        queries.Add("Department id = 5", "SELECT * FROM Costs WHERE department_id = 5");
        queries.Add("Amount > 500", "SELECT * FROM Costs WHERE amount > 500");
        queries.Add("Costs type name by ASC", "SELECT * FROM Costs_type ORDER BY name DESC");
        queries.Add("Departments name by DESC", "SELECT * FROM Departments ORDER BY name ASC");
        queries.Add("Type id = 3 and Department id = 3", "SELECT * FROM Costs WHERE type_id = 3 AND department_id = 3");
        queries.Add("Amount < 1000", "SELECT * FROM Costs WHERE amount < 1000");
        Cost_idBox.ItemsSource = queries.Keys;

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
                RestoreFields();
                DataBase.InsertData(type_id, department_id, amount);
                UpdateTables();
            }
            
        }

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
                RestoreFields();
                DataBase.ChangeData(cost_id, type_id, department_id, amount);
                UpdateTables();
            }
        }
    }
    private async void DeleteCost_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(Cost_idBox.SelectedItem != null)
        {
            int cost_id = (int)Cost_idBox.SelectedItem;
            if (cost_id > 0)
            {
                RestoreFields();
                DataBase.DeleteCost(cost_id);
                SetIdBox();
                UpdateTables();
            }
        }
        else
        {
            var box = MessageBoxManager
          .GetMessageBoxStandard("Warning", "You haven't chosen any cost_id",
              ButtonEnum.Ok);
            var result = await box.ShowAsync();
        }
      
        
    }
    private async void Query_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(Cost_idBox.SelectedItem != null)
        {
            QueryDialog.IsOpen = true;
            DataTable table = DataBase.GetData(queries[Cost_idBox.SelectedItem.ToString()]);
            QueryGrid.Columns.Clear();
            QueryGrid.ItemsSource = table.DefaultView;
            foreach (DataColumn dataColumn in table.Columns)
            {
                QueryGrid.Columns.Add(new DataGridTextColumn { Header = dataColumn.ColumnName, Binding = new Binding($"Row.ItemArray[{dataColumn.Ordinal}]") });
            }
            Dialog.IsOpen = false;
        }
        else
        {
            var box = MessageBoxManager
          .GetMessageBoxStandard("Warning", "You haven't chosen any query",
              ButtonEnum.Ok);
            var result = await box.ShowAsync();
        }
        
        
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
        if (Cost_idBox.SelectedItem != null && Cost_idBox.PlaceholderText == "Cost_id")
        {
            var row = DataBase.GetData($"SELECT * from Costs WHERE cost_id = {Cost_idBox.SelectedItem}");
            Type_idBox.Text = row.Rows[0]["type_id"].ToString();
            Department_idBox.Text = row.Rows[0]["department_id"].ToString();
            AmountBox.Text = row.Rows[0]["amount"].ToString();
            
        }
        

    }
}
