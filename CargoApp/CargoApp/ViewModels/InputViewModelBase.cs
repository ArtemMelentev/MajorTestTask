using Catel.MVVM;

namespace CargoApp.ViewModels;

public class InputViewModelBase : ViewModelBase
{
    public bool CanOK { get; set; }
    public bool CanCancel { get; set; }
    
    public TaskCommand OKCommand { get; private set; }
    public TaskCommand CancelCommand { get; private set; }

    public InputViewModelBase(string title, 
        bool canOK = false,
        bool canCancel = false)
    {
        Title = title;
        
        CanOK = canOK;
        CanCancel = canCancel;
        
        OKCommand = new TaskCommand(OkCommandAsync);
        CancelCommand = new TaskCommand(CancelCommandAsync);
    }
    
    private async Task OkCommandAsync()
    {
        await CloseViewModelAsync(true);
    }
    
    private async Task CancelCommandAsync()
    {
        await CloseViewModelAsync(false);
    }
}