using Avalonia.Controls;
using System.Data.SqlClient;
using System.Data;
using Lab1.Data;
using DialogHostAvalonia;
using Lab1.ViewModels;
using System;

namespace Lab1.Views;

public partial class MainView : UserControl
{
    public static event EventHandler TableChanged;
    public MainView()
    {
        InitializeComponent();
        Button1.Click += Button1_Click;
        CreateCost.Click += CreateCost_Click;
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
        TableChanged?.Invoke(this, EventArgs.Empty);
     }

    private void Button1_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Dialog.IsOpen = true;
    }
}
