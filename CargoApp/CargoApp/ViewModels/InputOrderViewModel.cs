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
}