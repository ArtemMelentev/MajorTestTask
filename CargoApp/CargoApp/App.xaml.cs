using System.Windows;
using CargoApp.DB;
using Catel.IoC;

namespace CargoApp;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var serviceLocator = ServiceLocator.Default;
        serviceLocator.RegisterType<DBContext>(registrationType: RegistrationType.Transient);
    }
}