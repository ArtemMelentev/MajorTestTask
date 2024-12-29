using System.Globalization;
using Catel.Windows;

namespace CargoApp.Views;

public partial class OrderWindow
{
    public OrderWindow(): base(DataWindowMode.Custom)
    {
        InitializeComponent();
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
    }
}