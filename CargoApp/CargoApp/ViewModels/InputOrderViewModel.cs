using CargoApp.Utilities.Enums;
using Catel.MVVM;

namespace CargoApp.ViewModels;

public sealed class InputOrderViewModel : ViewModelBase
{
    public string Id { get; set; }
    public string ClientName { get; set; }
    public string CourierName { get; set; }
    public string CargoDetails { get; set; }
    public string PickupAddress { get; set; }
    public string DeliveryAddress { get; set; }
    public OrderStatus Status { get; } = OrderStatus.New;
    public string Comment { get; set; }
    public DateTime CreationDate { get; } = DateTime.Now;
    
    public TaskCommand OKCommand { get; set; }
    public TaskCommand CancelCommand { get; set; }
    public TaskCommand SkipCommand { get; set; }

    public InputOrderViewModel()
    {
        Title = "Введите информацию о заявке";
        
        OKCommand = new TaskCommand(OkCommandAsync);
        CancelCommand = new TaskCommand(CancelCommandAsync);
        SkipCommand = new TaskCommand(SkipCommandAsync);
    }
    
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