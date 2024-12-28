using CargoApp.DB;
using CargoApp.Models;
using CargoApp.Utilities;
using CargoApp.Utilities.Enums;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Microsoft.VisualBasic;

namespace CargoApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IMessageService _messageService;
    private readonly IUIVisualizerService _uiVisualizerService;
    
    public TaskCommand CreateDataBaseCommand { get; private set; }
    public TaskCommand CreateOrderCommand { get; private set; }
    public TaskCommand ShowOrderTableCommand { get; private set; }

    public MainWindowViewModel()
    {
        _messageService = DependencyResolver.Resolve<IMessageService>();
        _uiVisualizerService = DependencyResolver.Resolve<UIVisualizerService>();
        
        CreateDataBaseCommand = new TaskCommand(CreateDataBaseAsync);
        CreateOrderCommand = new TaskCommand(CreateOrderAsync);
        ShowOrderTableCommand = new TaskCommand(ShowOrderTableAsync);
    }

    private async Task CreateDataBaseAsync()
    {
        try
        {
            await using var context = new DBContext();
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
            string title = "Введите информацию о заявке";
            var inputVM = new InputViewModel(title,
                canOK: true, canCancel: true,
                new InputField(String.Empty, "Имя клиента"),
                new InputField(String.Empty, "Имя курьера"),
                new InputField(String.Empty, "Детали заказа"),
                new InputField(String.Empty, "Адрес забора"),
                new InputField(String.Empty, "Адрес доставки"),
                new InputField(String.Empty, "Комментарий"));
            var res = await _uiVisualizerService.ShowDialogAsync(inputVM);
            if (res.DialogResult != true)
            {
                await _messageService.ShowAsync("Заявка не была создана");
                return;
            }
           
            /*var inputVM = new InputOrderViewModel();
            var result = await _uiVisualizerService.ShowDialogAsync(inputVM);
            if (result.DialogResult != true)
            {
                await _messageService.ShowAsync("Заявка не была создана");
            }*/
         
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

            await using var context = new DBContext();
            context.Orders.Add(order);
            await context.SaveChangesAsync();
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