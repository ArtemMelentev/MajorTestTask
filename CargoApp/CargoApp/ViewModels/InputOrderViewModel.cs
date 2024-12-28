using System.Collections.ObjectModel;
using CargoApp.Utilities;
using CargoApp.Utilities.Enums;

namespace CargoApp.ViewModels;

public class InputOrderViewModel : InputViewModel
{
    private OrderStatus _orderStatus;
    
    public ObservableCollection<OrderStatus> OrderStatuses { get; }

    public OrderStatus SelectedStatus
    {
        get => _orderStatus;
        set
        {
            if (_orderStatus != value)
            {
                _orderStatus = value;
                RaisePropertyChanged(nameof(SelectedStatus));
            }
        }
    }
    
    public InputOrderViewModel(string title, bool canOK = false, bool canCancel = false, params InputField[] fields) :
        base(title, canOK, canCancel, fields)
    {
        OrderStatuses = new ObservableCollection<OrderStatus>(
            (OrderStatus[])Enum.GetValues(typeof(OrderStatus))
        );
    }
}