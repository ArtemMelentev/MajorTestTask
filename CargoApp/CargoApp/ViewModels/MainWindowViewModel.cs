using CargoApp.DB;
using CargoApp.Models;
using CargoApp.Utilities;
using CargoApp.Utilities.Enums;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;

namespace CargoApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IMessageService _messageService;
    private readonly IUIVisualizerService _uiVisualizerService;
    private readonly DBContext _dbContext;

    public bool IsConnectedToDB { get; set; } = false;
    
    public TaskCommand ConnectDataBaseCommand { get; private set; }
    public TaskCommand ShowOrderTableCommand { get; private set; }
    
    public MainWindowViewModel()
    {
        _messageService = DependencyResolver.Resolve<IMessageService>();
        _uiVisualizerService = DependencyResolver.Resolve<UIVisualizerService>();
        _dbContext = ServiceLocator.Default.ResolveType<DBContext>();
        
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
            await _messageService.ShowAsync($"Ошибка при создании таблицы: {ex.Message}");
        }
    }

    private async Task ShowOrderTableAsync()
    {
        try
        {
            var orderTableViewModel = DependencyResolver.Resolve<OrderTableViewModel>();
            if (orderTableViewModel is null)
            {
                await _messageService.ShowErrorAsync("Ошибка создания таблицы заказов");
                return;
            }
            await _uiVisualizerService.ShowDialogAsync(orderTableViewModel);
        }
        catch (Exception ex)
        {
            await _messageService.ShowAsync($"Ошибка при создании таблицы: {ex.Message}");
        }
    }
}