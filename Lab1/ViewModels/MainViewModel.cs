namespace Lab1.ViewModels;

using Lab1.Data;
using Lab1.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using static Lab1.ViewModels.MainViewModel;


public class MainViewModel : ViewModelBase
{
    public class Costs
    {
        public int account_number {  get; set; }
        public int cost_id { get; set; }
        public int type_id { get; set; }
        public int department_id { get; set; }
        public int amount { get; set; }
        public DateTime date_of_cost { get; set; }
    }
    public class Costs_type
    {
        public int type_id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public int limit_amount { get; set; }
    }
    public class Departments
    {
        public int department_id { get; set; }
        public string? name { get; set; }
        public int number_of_employees { get; set; }
    }
    public ObservableCollection<Costs> Costs_ { get; set; } = new ObservableCollection<Costs>();
    public ObservableCollection<Costs_type> Costs_type_ { get; set; } = new ObservableCollection<Costs_type>();
    public ObservableCollection<Departments> Departments_ { get; set; } = new ObservableCollection<Departments>();
    public void RefreshTables()
    {
        DB dataBase = new DB();
        Costs_.Clear();
        Costs_type_.Clear();
        Departments_.Clear();
        DataTable costsTable = dataBase.GetData("SELECT * FROM Costs");
        DataTable departmentsTable = dataBase.GetData("SELECT * FROM Departments");
        DataTable costs_typeTable = dataBase.GetData("SELECT * FROM Costs_type");
        foreach (DataRow row in costsTable.Rows)
        {
            var cost = new Costs();
            cost.account_number = int.Parse(row["account_number"].ToString()!);
            cost.cost_id = int.Parse(row["cost_id"].ToString()!);
            cost.type_id = int.Parse(row["type_id"].ToString()!);
            cost.department_id = int.Parse(row["department_id"].ToString()!);
            cost.amount = int.Parse(row["amount"].ToString()!);
            cost.date_of_cost = (DateTime)row["date_of_cost"];
            Costs_.Add(cost);
        }
        foreach (DataRow row in costs_typeTable.Rows)
        {
            var costs_type = new Costs_type();
            costs_type.type_id = int.Parse(row["type_id"].ToString()!);
            costs_type.name = row["name"].ToString();
            costs_type.description = row["description"].ToString();
            costs_type.limit_amount = int.Parse(row["limit_amount"].ToString()!);
            Costs_type_.Add(costs_type);
        }
        foreach (DataRow row in departmentsTable.Rows)
        {
            var department = new Departments();
            department.department_id = int.Parse(row["department_id"].ToString()!);
            department.name = row["name"].ToString();
            department.number_of_employees = int.Parse(row["number_of_employees"].ToString()!);
            Departments_.Add(department);
        }
    }
    public MainViewModel()
    {
        MainView.TableChanged += MainView_TableChanged;
        RefreshTables();

    }

    private void MainView_TableChanged(object? sender, EventArgs e)
    {
        RefreshTables();
    }
}
