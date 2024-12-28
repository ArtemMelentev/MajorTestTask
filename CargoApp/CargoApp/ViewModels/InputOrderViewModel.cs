using System.Collections.ObjectModel;
using CargoApp.Utilities;
using CargoApp.Utilities.Enums;

namespace CargoApp.ViewModels;

public class InputOrderViewModel : InputViewModel
{
    private string _clientName = String.Empty;
    private string _courierName = String.Empty;
    private string _cargoDetails = String.Empty;
    private string _pickupAddress = String.Empty;
    private string _deliveryAddress = String.Empty;
    private string _comment = String.Empty;
    private DateTime _creationDate = DateTime.Now;
    private OrderStatus _orderStatus = OrderStatus.New;
    
    public ObservableCollection<OrderStatus> OrderStatuses { get; }

    public string ClientName
    {
        get => _clientName;
        set
        {
            if (value == _clientName)
            {
                return;
            }
            _clientName = value;
            RaisePropertyChanged(nameof(ClientName));
        }
    }

    public string CourierName
    {
        get => _courierName;
        set
        {
            if (value == _courierName)
            {
                return;
            }
            _courierName = value;
            RaisePropertyChanged(nameof(CourierName));
        }
    }

    public string CargoDetails
    {
        get => _cargoDetails;
        set
        {
            if (value == _cargoDetails)
            {
                return;
            }
            _cargoDetails = value;
            RaisePropertyChanged(nameof(CargoDetails));
        }
    }

    public string PickupAddress
    {
        get => _pickupAddress;
        set
        {
            if (value == _pickupAddress)
            {
                return;
            }
            _pickupAddress = value;
            RaisePropertyChanged(nameof(PickupAddress));
        }
    }

    public string DeliveryAddress
    {
        get => _deliveryAddress;
        set
        {
            if (value == _deliveryAddress)
            {
                return;
            }
            _deliveryAddress = value;
            RaisePropertyChanged(nameof(DeliveryAddress));
        }
    }

    public string Comment
    {
        get => _comment;
        set
        {
            if (value == _comment)
            {
                return;
            }
            _comment = value;
            RaisePropertyChanged(nameof(Comment));
        }
    }

    public DateTime CreationDate
    {
        get => _creationDate;
        set
        {
            if (value.Equals(_creationDate))
            {
                return;
            }
            _creationDate = value;
            RaisePropertyChanged(nameof(CreationDate));
        }
    }
    
    public OrderStatus SelectedStatus
    {
        get => _orderStatus;
        set
        {
            if (_orderStatus == value)
            {
                return;
            }
            _orderStatus = value;
            RaisePropertyChanged(nameof(SelectedStatus));
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