using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using AppContext = CargoApp.DB.AppContext;

namespace CargoApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IMessageService _messageService;
    
    public TaskCommand CreateDataBase { get; private set; }
    public TaskCommand CreateOrder { get; private set; }

    public MainWindowViewModel()
    {
        _messageService = DependencyResolver.Resolve<IMessageService>();
        
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
            await using var context = new AppContext();
            await context.Database.EnsureCreatedAsync();
            await _messageService.ShowAsync("Таблица создана успешно в PostgreSQL!");
        }
        catch (Exception ex)
        {
            await _messageService.ShowAsync($"Ошибка при создании таблицы: {ex.Message}");
        }
    }
}