using CargoApp.Models;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using AppContext = CargoApp.DB.AppContext;

namespace CargoApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IMessageService _messageService;
    private readonly IUIVisualizerService _uiVisualizerService;
    
    public TaskCommand CreateDataBase { get; private set; }
    public TaskCommand CreateOrder { get; private set; }

    public MainWindowViewModel()
    {
        _messageService = DependencyResolver.Resolve<IMessageService>();
        _uiVisualizerService = DependencyResolver.Resolve<UIVisualizerService>();
        
        CreateDataBase = new TaskCommand(CreateDataBaseAsync);
        CreateOrder = new TaskCommand(CreateOrderAsync);
    }

    private async Task CreateDataBaseAsync()
    {
        try
        {
            await using var context = new AppContext();
            await context.Database.EnsureCreatedAsync();
            await _messageService.ShowAsync("Таблица создана успешно в PostgreSQL!");
        }
        catch (Exception ex)
        {
            await _messageService.ShowAsync($"Ошибка при создании таблицы: {ex.Message}");
        }
    }

    private async Task CreateOrderAsync()
    {
        try
        {
            var inputVM = new InputOrderViewModel();
            var result = await _uiVisualizerService.ShowDialogAsync(inputVM);
            if (result.DialogResult != true)
            {
                await _messageService.ShowAsync("Order not created");
            }

            var order = new OrderModel();
            order.Id = 1;
            order.Comment = inputVM.Comment;
            order.ClientName = inputVM.ClientName;
            order.CourierName = inputVM.CourierName;
            
            /*await using var context = new AppContext();
            await context.Database.EnsureCreatedAsync();
            await _messageService.ShowAsync("Таблица создана успешно в PostgreSQL!");*/
        }
        catch (Exception ex)
        {
            //await _messageService.ShowAsync($"Ошибка при создании таблицы: {ex.Message}");
        }
    }
}