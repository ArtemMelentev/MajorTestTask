using CargoApp.Enums;
using Catel.MVVM;

namespace CargoApp.ViewModels;

public class InputOrderViewModel : ViewModelBase
{
    public string Id { get; set; }
    public string ClientName { get; set; }
    public string CourierName { get; set; }
    public string CargoDetails { get; set; }
    public string PickupAddress { get; set; }
    public string DeliveryAddress { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.New;
    public string Comment { get; set; }
    public DateTime CreationDate { get; } = DateTime.Now;
    
    public DialogResult Result { get; private set; }
    
    public TaskCommand OKCommand { get; set; }
    public TaskCommand CancelCommand { get; set; }
    public TaskCommand SkipCommand { get; set; }

    public InputOrderViewModel()
    {
        OKCommand = new TaskCommand(OkCommandAsync);
        CancelCommand = new TaskCommand(CancelCommandAsync);
        SkipCommand = new TaskCommand(SkipCommandAsync);
    }
    
    private async Task OkCommandAsync()
    {
        Result = DialogResult.Ok;
        await CloseViewModelAsync(true);
    }
    
    private async Task CancelCommandAsync()
    {
        Result = DialogResult.Cancel;
        await CloseViewModelAsync(false);
    }
    
    private async Task SkipCommandAsync()
    {
        Result = DialogResult.Skip;
        await CloseViewModelAsync(false);
    }
}