using CargoApp.DB;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;

namespace CargoApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IMessageService _messageService;
    private readonly IUIVisualizerService _uiVisualizerService;

    public bool IsConnectedToDB { get; set; }
    
    public TaskCommand ConnectDataBaseCommand { get; private set; }
    public TaskCommand ShowOrderTableCommand { get; private set; }
    
    public MainWindowViewModel()
    {
        _messageService = DependencyResolver.Resolve<IMessageService>();
        _uiVisualizerService = DependencyResolver.Resolve<UIVisualizerService>();
        
        ConnectDataBaseCommand = new TaskCommand(ConnectDataBaseAsync);
        ShowOrderTableCommand = new TaskCommand(ShowOrderTableAsync);
    }

    private async Task ConnectDataBaseAsync()
    {
        try
        {
            var dbConnectionVM = new DBConnectionViewModel();
            var result = await _uiVisualizerService.ShowDialogAsync(dbConnectionVM);
            IsConnectedToDB = result.DialogResult ?? false;
        }
        catch (Exception ex)
        {
            await _messageService.ShowAsync(Strings.OrderTableCreationError + ex.Message);
        }
    }

    private async Task ShowOrderTableAsync()
    {
        var orderTableViewModel = DependencyResolver.Resolve<OrderTableViewModel>();
        if (orderTableViewModel is null)
        {
            await _messageService.ShowErrorAsync(Strings.OrderTableCreationError);
            return;
        }
        await _uiVisualizerService.ShowDialogAsync(orderTableViewModel);
    }
}