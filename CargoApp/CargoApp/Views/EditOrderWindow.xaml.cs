using System.Globalization;
using Catel.Windows;

namespace CargoApp.Views;

public partial class EditOrderWindow
{
    public EditOrderWindow(): base(DataWindowMode.Custom)
    {
        InitializeComponent();
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
    }
}