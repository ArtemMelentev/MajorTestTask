namespace CargoApp.ViewModels;

public class MessageViewModel : InputViewModelBase
{
    private string _message;

    public string Message
    {
        get => _message;
        set
        {
            _message = value;
            RaisePropertyChanged(nameof(Message));
        }
    }
    
    public MessageViewModel(string title, string message, bool canOK = true, bool canCancel = true) : base(title, canOK,
        canCancel)
    {
        _message = message;
    }
    
    
}