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
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Companion;
using QuestPDF.Previewer;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using Avalonia.Data.Converters;
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
        QueryWindow.DialogClosing += QueryWindow_DialogClosing;
        UpdateTables();
    }
    private async void ShowMessage(string message)
    {
        var box = MessageBoxManager
         .GetMessageBoxStandard("Warning", message,
             ButtonEnum.Ok);
        var result = await box.ShowAsync();
    }
    private void QueryWindow_DialogClosing(object? sender, DialogClosingEventArgs e)
    {
        QueryBox.SelectedItem = null;
        ParameterBox.SelectedItem = null;
        StartDate.SelectedDate = null;
        EndDate.SelectedDate = null;
        ParameterBox.IsVisible = false;
        ParameterWrite.IsVisible = false;
        ParameterWrite.Text = "";
    }

    private void Dialog_DialogClosing(object? sender, DialogClosingEventArgs e)
    {
        ChangeCost.IsVisible = false;
        CreateCost.IsVisible = false;
        DeleteCost.IsVisible = false;
        Cost_idBox.IsVisible = false;
        Date.IsVisible = false;
        Date.SelectedDate = null;
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
        Date.IsVisible = true;
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
        Date.IsVisible = false;
    }
    private void IfCreateBox()
    {
        Dialog.IsOpen = true;
        Cost_idBox.IsVisible = false;
        CreateCost.IsVisible = true;
        Date.IsVisible = true;
    }
    private void IfQueryBox()
    {
        Dialog.IsOpen = true;
        Type_idBox.IsVisible = false;
        Department_idBox.IsVisible = false;
        AmountBox.IsVisible = false;
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
        QueryWindow.IsOpen = true;
        SetComplexQueries();
        //SetLinkedQueries();
    }
    private void ShowMessages(bool[] conditions)
    {
        if (!conditions[0])
        {
            ShowMessage("There is no such id in Costs_type");
            return;
        }
        if (!conditions[1])
        {
            ShowMessage("There is no such id in Departments");
            return;
        }
    }
    private bool[] ValidateData(int type_id, int department_id)
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

        bool[] conditions = new bool[2];
        conditions[0] = ValidateId("type_id", "Costs_type", type_id);
        conditions[1] = ValidateId("department_id", "Departments", department_id);
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
            var textColumn = new DataGridTextColumn { Header = dataColumn.ColumnName };
            if (dataColumn.DataType == typeof(DateTime))
            {
                textColumn.Binding = new Binding($"Row.ItemArray[{dataColumn.Ordinal}]")
                {
                    Converter = (IValueConverter)App.Current.Resources["DateConverter"]
                };
            }
            else
            {
                textColumn.Binding = new Binding($"Row.ItemArray[{dataColumn.Ordinal}]");
            }
            Costs.Columns.Add(textColumn);
        }
        foreach (DataColumn dataColumn in costs_typeTable.Columns)
        {
            Costs_type.Columns.Add(new DataGridTextColumn { Header = dataColumn.ColumnName, Binding = new Binding($"Row.ItemArray[{dataColumn.Ordinal}]") });
        }
        foreach (DataColumn dataColumn in departmentsTable.Columns)
        {
            Departments.Columns.Add(new DataGridTextColumn { Header = dataColumn.ColumnName, Binding = new Binding($"Row.ItemArray[{dataColumn.Ordinal}]") });
        }
        QuestPDF.Settings.License = LicenseType.Community;
        var document = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                    });
                    for (int i = 0; i < queries.Count * 2; i++)
                    {
                        if (i % 2 == 0)
                        {
                            table.Cell().Row((uint)i + 1).Column(1).AlignCenter().Text(queries.Keys.ElementAt(i / 2));
                        }
                        else
                        {
                            table.Cell().Row((uint)i + 1).Column(1).AlignCenter().Padding(5)
                                                    .Container().Table(table =>
                                                    {
                                                        DataTable query = DataBase.GetData(queries.Values.ElementAt(i / 2));
                                                        table.ColumnsDefinition(columns =>
                                                        {
                                                            foreach (var column in query.Columns)
                                                            {
                                                                columns.RelativeColumn();
                                                            }
                                                            for (int i = 0; i < query.Columns.Count; i++)
                                                            {
                                                                table.Cell().Row(1).Column((uint)i + 1)
                                            .Border(1)
                                            .Text(query.Columns[i].ColumnName)
                                            .Bold()
                                            .AlignCenter();
                                                            }
                                                            for (int row = 0; row < query.Rows.Count; row++)
                                                            {
                                                                for (int col = 0; col < query.Columns.Count; col++)
                                                                {
                                                                    table.Cell().Row((uint)row + 2).Column((uint)col + 1).Border(1).AlignCenter().Text(query.Rows[row][col].ToString());
                                                                }

                                                            }
                                                        });
                                                    });
                        }

                    }
                });
            });
        });
        //document.GeneratePdfAndShow();
        //document.ShowInCompanionAsync();
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
        QueryBox.ItemsSource = queries.Keys;
    }
    private void SetLinkedQueries()
    {
        queries.Clear();
        queries.Add("Total amount by type", "SELECT ct.name AS CostType, SUM(c.amount) AS TotalAmount FROM Costs c JOIN Costs_type ct ON c.type_id = ct.type_id GROUP BY ct.name");
        queries.Add("Number of costs for department", "SELECT d.name AS Department, COUNT(c.cost_id) AS NumberOfCosts FROM Costs c JOIN Departments d ON c.department_id = d.department_id GROUP BY d.name");
        queries.Add("Average cost for department", "SELECT d.name AS Department, AVG(c.amount) AS AverageCost FROM Costs c JOIN Departments d ON c.department_id = d.department_id GROUP BY d.name");
        queries.Add("Max amount by type", "SELECT ct.name AS CostType, MAX(c.amount) AS MaxAmount FROM Costs c JOIN Costs_type ct ON c.type_id = ct.type_id GROUP BY ct.name");
        queries.Add("Min cost for department", "SELECT d.name AS Department, MIN(c.amount) AS MinCost FROM Costs c JOIN Departments d ON c.department_id = d.department_id GROUP BY d.name");
        queries.Add("Count of departments for type", "SELECT ct.name AS CostType, COUNT(DISTINCT c.department_id) AS CountOfDepartments FROM Costs c JOIN Costs_type ct ON c.type_id = ct.type_id JOIN Departments d ON c.department_id = d.department_id GROUP BY ct.name");
        queries.Add("Count of types for department", "SELECT d.name AS Department, COUNT(DISTINCT c.type_id) AS CountOfTypes FROM Costs c JOIN Departments d ON c.department_id = d.department_id GROUP BY d.name");
        QueryBox.ItemsSource = queries.Keys;
    }
    private void SetComplexQueries()
    {
        queries.Clear();
        queries.Add("Exceeded avarage amount", "SELECT d.name AS Department, SUM(c.amount) AS TotalAmount FROM Costs c JOIN Departments d ON c.department_id = d.department_id GROUP BY d.name HAVING SUM(c.amount) > (SELECT AVG(avg_amount) FROM (SELECT SUM(amount) AS avg_amount FROM Costs GROUP BY department_id) AS subquery)");
        queries.Add("Costs for type by parameter", "SELECT c.account_number, c.amount, c.date_of_cost, ct.name AS cost_type, d.name AS department FROM Costs c JOIN Costs_type ct ON c.type_id = ct.type_id JOIN Departments d ON c.department_id = d.department_id WHERE ct.name = @CostType");
        queries.Add("Departments' exceeded selected limit", "SELECT d.name AS Department, SUM(c.amount) AS TotalAmount FROM Costs c JOIN Departments d ON c.department_id = d.department_id GROUP BY d.name HAVING SUM(c.amount) > @LimitAmount");
        queries.Add("Departments with no cost", "SELECT d.name AS Department FROM Departments d WHERE NOT EXISTS (SELECT 1 FROM Costs c WHERE c.department_id = d.department_id)");
        queries.Add("Costs for department by parameter", "SELECT c.account_number, c.amount, c.date_of_cost, ct.name AS cost_type, d.name AS department FROM Costs c JOIN Costs_type ct ON c.type_id = ct.type_id JOIN Departments d ON c.department_id = d.department_id WHERE d.name = @DepartmentName");
        queries.Add("Departments' count of costs if exceeded avarage count", "SELECT d.name AS Department, COUNT(c.cost_id) AS NumberOfCosts FROM Costs c JOIN Departments d ON c.department_id = d.department_id GROUP BY d.name HAVING COUNT(cost_id) > (SELECT AVG(avg_costAmount) FROM (SELECT COUNT(cost_id) AS avg_costAmount FROM Costs GROUP BY department_id) AS subquery)");
        queries.Add("Costs for department in date range", "SELECT d.name AS Department, SUM(c.amount) AS TotalAmount FROM Costs c JOIN Departments d ON c.department_id = d.department_id WHERE c.date_of_cost BETWEEN @StartDate AND @EndDate GROUP BY d.name");
        QueryBox.ItemsSource = queries.Keys;
    }
    private void CreateCost_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (CheckFields())
        {
            ShowMessage("Fields can't be empty");
            return;
        }
        int type_id = int.Parse(Type_idBox.Text);
        int department_id = int.Parse(Department_idBox.Text);
        int amount = int.Parse(AmountBox.Text);
        DateTime date = Date.SelectedDate == null ? DateTime.Now : (DateTime)Date.SelectedDate;
        bool[] conditions = ValidateData(type_id, department_id);
        ShowMessages(conditions);
        if (conditions.All(x => x == true))
        {
            RestoreFields();
            DataBase.InsertData(type_id, department_id, amount, date);
            UpdateTables();
        }

    }
    private void ChangeCost_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (CheckFields())
        {
            ShowMessage("Fields can't be empty");
            return;
        }
        int cost_id = (int)Cost_idBox.SelectedItem;
        int type_id = int.Parse(Type_idBox.Text);
        int department_id = int.Parse(Department_idBox.Text);
        int amount = int.Parse(AmountBox.Text);
        DateTime date = Date.SelectedDate == null ? DateTime.Now : (DateTime)Date.SelectedDate;
        bool[] conditions = ValidateData(type_id, department_id);
        ShowMessages(conditions);
        if (conditions.All(x => x == true))
        {
            RestoreFields();
            DataBase.ChangeData(cost_id, type_id, department_id, amount, date);
            UpdateTables();
        }
    }
    private void DeleteCost_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(Cost_idBox.SelectedItem == null)
        {
            ShowMessage("You haven't chosen any cost_id");
            return;
        }
        int cost_id = (int)Cost_idBox.SelectedItem;
        if (cost_id > 0)
        {
            RestoreFields();
            DataBase.DeleteCost(cost_id);
            SetIdBox();
            UpdateTables();
        }


    }
    private void Query_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(QueryBox.SelectedItem == null)
        {
            ShowMessage("You haven't chosen any query");
            return;
        }
        string parameter = "";
        if(ParameterBox.IsVisible == true)
        {
            if(ParameterBox.SelectedItem == null)
            {
                ShowMessage("You haven't chosen any parameter");
                return;
            }
            parameter = ParameterBox.SelectedItem.ToString();
        }      
        string query = QueryBox.SelectedItem.ToString();
        int parameterInt = 0;
        if(ParameterWrite.IsVisible == true)
        {
            if(!int.TryParse(ParameterWrite.Text, out parameterInt))
            {
                ShowMessage("Wrong value. Write int");
                return;
            }
        }
        DateTime start = default, end = default;
        if(StartDate.IsVisible == true)
        {
            start = StartDate.SelectedDate == null ? DateTime.Now : (DateTime)StartDate.SelectedDate;
            end = EndDate.SelectedDate == null ? DateTime.Now : (DateTime)EndDate.SelectedDate;
        }       
        QueryDialog.IsOpen = true;
        DataTable table = DataBase.GetData(queries[query], parameter, parameterInt, start, end);
        QueryGrid.Columns.Clear();
        QueryGrid.ItemsSource = table.DefaultView;
        foreach (DataColumn dataColumn in table.Columns)
        {
            QueryGrid.Columns.Add(new DataGridTextColumn { Header = dataColumn.ColumnName, Binding = new Binding($"Row.ItemArray[{dataColumn.Ordinal}]") });
        }
        QueryWindow.IsOpen = false;


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
    private void QueryBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        if(QueryBox.SelectedItem == null)
        {
            return;
        }
        string query = QueryBox.SelectedItem.ToString();

        if (queries[query].Contains("@"))
        {
            ParameterBox.IsVisible = true;
            List<string> names = new List<string>();
            DataTable table = new DataTable();
            if (queries[query].Contains("CostType"))
            {
                table = DataBase.GetData("SELECT name FROM Costs_type");
                ParameterBox.PlaceholderText = "CostType";
            }
            if (queries[query].Contains("DepartmentName"))
            {
                table = DataBase.GetData("SELECT name FROM Departments");
                ParameterBox.PlaceholderText = "Department";
            }
            if (queries[query].Contains("LimitAmount"))
            {
                ParameterBox.IsVisible = false;
                ParameterWrite.IsVisible = true;
                ParameterWrite.Watermark = "Amount";
            }
            if (queries[query].Contains("Date"))
            {
                ParameterBox.IsVisible = false;
                StartDate.IsVisible = true;
                EndDate.IsVisible = true;
            }
            else { ParameterWrite.IsVisible = false; }
            foreach (DataRow row in table.Rows)
            {
                names.Add(row["name"].ToString());
            }
            ParameterBox.Width = 200;
            ParameterBox.ItemsSource = names;
        }
        else
        {
            ParameterWrite.IsVisible = false;
            ParameterBox.IsVisible = false;
            StartDate.IsVisible = false;
            EndDate.IsVisible = false;
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
            Date.SelectedDate = (DateTime)row.Rows[0]["date_of_cost"];
        }


    }
}
