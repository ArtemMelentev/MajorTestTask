using CargoApp.Utilities;
using CargoApp.Utilities.Enums;
using Catel.MVVM;

namespace CargoApp.ViewModels;

public class InputViewModel : ViewModelBase
{
    public InputField[] Fields { get; }
    public List<string?> Results => Fields.Select(v => v.Value).ToList();
    public bool CanOK { get; set; }
    public bool CanCancel { get; set; }
    public bool CanSkip { get; set; }

    public InputViewModel(string title, 
        bool canOK = false,
        bool canCancel = false,
        bool canSkip = false,
        params InputField[] fields)
    {
        Title = title;
        Fields = fields;
        
        CanOK = canOK;
        CanCancel = canCancel;
        CanSkip = canSkip;
        
        OKCommand = new TaskCommand(OkCommandAsync);
        CancelCommand = new TaskCommand(CancelCommandAsync);
        SkipCommand = new TaskCommand(SkipCommandAsync);
    }
    
    public TaskCommand OKCommand { get; set; }
    public TaskCommand CancelCommand { get; set; }
    public TaskCommand SkipCommand { get; set; }
    private async Task OkCommandAsync()
    {
        await CloseViewModelAsync(true);
    }
    
    private async Task CancelCommandAsync()
    {
        await CloseViewModelAsync(false);
    }
    
    private async Task SkipCommandAsync()
    {
        await CloseViewModelAsync(false);
    }
}