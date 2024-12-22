using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using AppContext = CargoApp.DB.AppContext;

namespace CargoApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IMessageService _messageService;
    
    public TaskCommand CreateDataBase { get; private set; }

    public MainWindowViewModel()
    {
        _messageService = DependencyResolver.Resolve<IMessageService>();
        
        CreateDataBase = new TaskCommand(CreateDataBaseAsync);
    }

    private async Task CreateDataBaseAsync()
    {
        try
        {
            await using var context = new AppContext();
            await context.Database.EnsureCreatedAsync(); // Создает базу и таблицу, если их нет
            await _messageService.ShowAsync("Таблица создана успешно в PostgreSQL!");
        }
        catch (Exception ex)
        {
            await _messageService.ShowAsync($"Ошибка при создании таблицы: {ex.Message}");
        }
    }
}