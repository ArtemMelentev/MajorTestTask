using System.Windows.Input;
using Catel.Windows;

namespace CargoApp.Views;

public partial class OrderTableWindow
{
    public OrderTableWindow(): base(DataWindowMode.Custom)
    {
        InitializeComponent();
        DataGrid.AddHandler(MouseDownEvent, new MouseButtonEventHandler(dataGrid_MouseDown), true);
    }
    
    private void dataGrid_MouseDown(object sender, MouseButtonEventArgs e)
    {

    }
}