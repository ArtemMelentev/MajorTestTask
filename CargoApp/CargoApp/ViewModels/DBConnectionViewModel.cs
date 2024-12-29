using System.IO;
using System.Text.Json;
using CargoApp.DB;
using CargoApp.Utilities;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;

namespace CargoApp.ViewModels;

public class DBConnectionViewModel : ViewModelBase
{
    private readonly IMessageService _messageService;
    
    private string _host = "localhost";
    private string _port = "5432";
    private string _username = String.Empty;
    private string _password = String.Empty;
    
    public TaskCommand ConnectToDBCommand { get; set; }

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
        _messageService = DependencyResolver.Resolve<IMessageService>();
        ConnectToDBCommand = new TaskCommand(ConnectToDBAsync);

        if (!File.Exists(GlobalConstants.DBJsonFilePath))
        {
            return;
        }
        
        var json = File.ReadAllText(GlobalConstants.DBJsonFilePath);
        var config = JsonSerializer.Deserialize<DbConfig>(json);
        if (config == null)
        {
            return;
        }
        Host = config.Host;
        Port = config.Port;
        Username = config.Username;
    }

    private async Task ConnectToDBAsync()
    {
        var connectionString = BuildConnectionString(false);
        bool result = await TestConnectionAsync(connectionString);
        if (!result)
        {
            await _messageService.ShowErrorAsync(Strings.DatabaseConnectionInfoMessage);
            return;
        }

        if (!File.Exists(GlobalConstants.DBJsonFilePath))
        {
            SaveConfigToFile();
        }
        
        await CreateDatabaseAsync();
        
        await _messageService.ShowAsync(Strings.ConnectionSuccessful);
        await CloseViewModelAsync(true);
    }

    private string BuildConnectionString(bool includeDatabase)
    {
        if (includeDatabase && String.IsNullOrWhiteSpace(GlobalConstants.DataBaseName))
        {
            return String.Empty;
        }

        return $"Host={Host};Port={Port};Username={Username};Password={Password};" +
               (includeDatabase ? $"Database={GlobalConstants.DataBaseName};" : "");
    }

    private async Task<bool> TestConnectionAsync(string connectionString)
    {
        try
        {
            await using var context = new DBContext(connectionString);
            return await context.Database.CanConnectAsync();
        }
        catch (Exception ex)
        {
            await _messageService.ShowErrorAsync(Strings.ConnectionError + ex);
        }

        return false;
    }

    private async Task CreateDatabaseAsync()
    {
        try
        {
            await using var context = new DBContext();
            bool result = await context.Database.EnsureCreatedAsync();

            if (result)
            {
                await _messageService.ShowAsync(Strings.DBCreationSuccessful);
            }
        }
        catch (Exception ex)
        {
            await _messageService.ShowErrorAsync(Strings.DBCreationError + ex);
        }
    }

    private void SaveConfigToFile()
    {
        var config = new DbConfig
        {
            Host = Host,
            Port = Port,
            Username = Username,
            Password = Password,
            DatabaseName = GlobalConstants.DataBaseName,
        };
        
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(GlobalConstants.DBJsonFilePath, json);
    }
}