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
    
    public TaskCommand ConnectDataBaseCommand { get; private set; }
    public TaskCommand CreateOrderCommand { get; private set; }
    public TaskCommand ShowOrderTableCommand { get; private set; }

    public MainWindowViewModel()
    {
        _messageService = DependencyResolver.Resolve<IMessageService>();
        _uiVisualizerService = DependencyResolver.Resolve<UIVisualizerService>();
        _dbContext = ServiceLocator.Default.ResolveType<DBContext>();
        
        ConnectDataBaseCommand = new TaskCommand(ConnectDataBaseAsync);
        CreateOrderCommand = new TaskCommand(CreateOrderAsync);
        ShowOrderTableCommand = new TaskCommand(ShowOrderTableAsync);
    }

    private async Task ConnectDataBaseAsync()
    {
        try
        {
            await _dbContext.Database.EnsureCreatedAsync();
            await _messageService.ShowAsync("Таблица создана успешно в PostgreSQL!");
            await _dbContext.Database.CanConnectAsync();
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
            string title = "Введите информацию о заявке";
            var inputVM = new InputViewModel(title,
                canOK: true, canCancel: true,
                new InputField("Имя клиента"),
                new InputField("Имя курьера"),
                new InputField("Детали заказа"),
                new InputField("Адрес забора"),
                new InputField("Адрес доставки"),
                new InputField("Комментарий"));
            var res = await _uiVisualizerService.ShowDialogAsync(inputVM);
            if (res.DialogResult != true)
            {
                await _messageService.ShowAsync("Заявка не была создана");
                return;
            }
         
            var order = new OrderModel
            {
                ClientName = inputVM.Results[0],
                CourierName = inputVM.Results[1],
                CargoDetails = inputVM.Results[2],
                PickupAddress = inputVM.Results[3],
                DeliveryAddress = inputVM.Results[4],
                Comment = inputVM.Results[5],
                Status = OrderStatus.New
            };
            
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
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