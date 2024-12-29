using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using CargoApp.DB;
using CargoApp.Utilities;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CargoApp.ViewModels;

public class DBConnectionViewModel : ViewModelBase
{
    private const string _dataBaseName = "ordersdb";
    private readonly DBContext _dbContext;
    private readonly IMessageService _messageService;
    
    private string _host = "localhost";
    private string _port = "5432";
    private string _username = String.Empty;
    private string _password = String.Empty;
    
    public TaskCommand ConnectToDBCommand { get; set; }
    
    public bool IsConnected { get; private set; } = false;

    public string Host
    {
        get => _host;
        set
        {
            _host = value;
            RaisePropertyChanged(nameof(Host));
        }
    }

    public string Port
    {
        get => _port;
        set
        {
            _port = value;
            RaisePropertyChanged(nameof(Port));
        }
    }
    
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            RaisePropertyChanged(nameof(Username));
        }
    }
    
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            RaisePropertyChanged(nameof(Password));
        }
    }
    
    public DBConnectionViewModel()
    {
        _dbContext = ServiceLocator.Default.ResolveType<DBContext>();
        _messageService = DependencyResolver.Resolve<IMessageService>();
        ConnectToDBCommand = new TaskCommand(ConnectToDBAsync);
    }

    private async Task ConnectToDBAsync()
    {
        try
        {
            // Проверить подключение
            var connectionString = BuildConnectionString(false); // Без указания имени базы
            await TestConnectionAsync(connectionString);
            
            // Сохранить конфигурацию
            SaveConfigToFile();
            
            // Создать базу данных
            await CreateDatabaseAsync();

            await _messageService.ShowAsync("Настройки успешно сохранены!");
        }
        catch (Exception ex)
        {
            await _messageService.ShowErrorAsync($"Ошибка: {ex.Message}");
        }
    }

    private async Task SaveDatabaseConfigAsync()
    {
        try
        {
            // Проверить подключение
            var connectionString = BuildConnectionString(false); // Без указания имени базы
            await TestConnectionAsync(connectionString);

            // Сохранить конфигурацию
            SaveConfigToFile();
            // Создать базу данных
            await CreateDatabaseAsync();
            
            await _messageService.ShowAsync("Настройки успешно сохранены!");
        }
        catch (Exception ex)
        {
            await _messageService.ShowErrorAsync($"Ошибка: {ex.Message}");
        }
    }

    private string BuildConnectionString(bool includeDatabase)
    {
        if (includeDatabase && string.IsNullOrWhiteSpace(_dataBaseName))
            throw new Exception("Имя базы данных не указано.");

        return $"Host={Host};Port={Port};Username={Username};Password={Password};" +
               (includeDatabase ? $"Database={_dataBaseName};" : "");
    }

    private async Task TestConnectionAsync(string connectionString)
    {
        try
        {
            var optionsBuilder = new DbContextOptionsBuilder<DBContext>();
            optionsBuilder.UseNpgsql(connectionString);
            
            bool isCanConnect = await _dbContext.Database.CanConnectAsync();
            await _messageService.ShowAsync("Подключение успешно.");
        }
        catch (Exception ex)
        {
            await _messageService.ShowErrorAsync("Не удалось подключиться к серверу базы данных. Проверьте введённые данные." + ex);
        }
    }

    private async Task CreateDatabaseAsync()
    {
        try
        {
            var connectionString = BuildConnectionString(false); // Без указания имени базы
            var optionsBuilder = new DbContextOptionsBuilder<DBContext>();
            optionsBuilder.UseNpgsql(connectionString);


            await _dbContext.Database.EnsureCreatedAsync();

            await _messageService.ShowAsync($"База данных {_dataBaseName} успешно создана.");
        }
        catch (Exception ex)
        {
            await _messageService.ShowErrorAsync($"Ошибка при создании базы данных {_dataBaseName}." + ex);
        }
    }

    private void SaveConfigToFile()
    {
        var config = new
        {
            Host,
            Port,
            Username,
            Password,
            _dataBaseName
        };

        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(GlobalConstants.DBJsonFilePath, json);
        Console.WriteLine("Конфигурация сохранена в файл dbconfig.json.");
    }
}