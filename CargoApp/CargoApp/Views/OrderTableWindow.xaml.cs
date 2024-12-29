using System.Windows.Controls;
using Catel.Windows;

namespace CargoApp.Views;

public partial class OrderTableWindow
{
    public OrderTableWindow(): base(DataWindowMode.Custom)
    {
        InitializeComponent();
    }
    
    private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (e.PropertyName == "Id")
        {
            e.Cancel = true;
        }
    }
}