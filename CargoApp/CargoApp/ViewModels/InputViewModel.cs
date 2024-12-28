using CargoApp.Utilities;
using CargoApp.Utilities.Enums;
using Catel.MVVM;

namespace CargoApp.ViewModels;

public class InputViewModel : ViewModelBase
{
    public InputField[] Fields { get; }
    public List<string?> Results => Fields.Select(v => v.DefaultValue).ToList();
    public bool CanOK { get; set; }
    public bool CanCancel { get; set; }

    public InputViewModel(string title, 
        bool canOK = false,
        bool canCancel = false,
        params InputField[] fields)
    {
        Title = title;
        Fields = fields;
        
        CanOK = canOK;
        CanCancel = canCancel;
        
        OKCommand = new TaskCommand(OkCommandAsync);
        CancelCommand = new TaskCommand(CancelCommandAsync);
    }
    
    public TaskCommand OKCommand { get; set; }
    public TaskCommand CancelCommand { get; set; }
    private async Task OkCommandAsync()
    {
        await CloseViewModelAsync(true);
    }
    
    private async Task CancelCommandAsync()
    {
        await CloseViewModelAsync(false);
    }
}