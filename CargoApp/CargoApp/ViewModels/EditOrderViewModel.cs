using System.Collections.ObjectModel;
using CargoApp.Models;
using CargoApp.Utilities.Enums;

namespace CargoApp.ViewModels;

public class EditOrderViewModel : InputViewModelBase
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
        get => _creationDate.ToUniversalTime();
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

    public bool IsShowClientName => SelectedStatus == OrderStatus.New;
    public bool IsShowCourierName => SelectedStatus is OrderStatus.New or OrderStatus.InProcess;
    public bool IsShowCargoDetails =>  SelectedStatus == OrderStatus.New;
    public bool IsShowPickupAddress => SelectedStatus == OrderStatus.New;
    public bool IsShowDeliveryAddress => SelectedStatus == OrderStatus.New;
    public bool IsShowComment => SelectedStatus is OrderStatus.New or OrderStatus.Canceled;
    public bool IsShowCreationDate => SelectedStatus == OrderStatus.New;

    public EditOrderViewModel(string title, OrderModel orderModel,bool canOK = false, bool canCancel = false) :
        base(title, canOK, canCancel)
    {
        ClientName = orderModel.ClientName;
        CourierName = orderModel.CourierName;
        CargoDetails = orderModel.CargoDetails;
        PickupAddress = orderModel.PickupAddress;
        DeliveryAddress = orderModel.DeliveryAddress;
        Comment = orderModel.Comment;
        SelectedStatus = orderModel.Status;
        CreationDate = orderModel.CreationDate;

        OrderStatuses = new ObservableCollection<OrderStatus>(Enum.GetValues<OrderStatus>());
        if (orderModel.Status is not OrderStatus.New)
        {
            OrderStatuses.Remove(OrderStatus.New);
        }
    }

    public OrderModel GetOrderModel()
    {
        return new OrderModel
        {
            ClientName = ClientName,
            CourierName = CourierName,
            CargoDetails = CargoDetails,
            PickupAddress = PickupAddress,
            DeliveryAddress = DeliveryAddress,
            Comment = Comment,
            Status = SelectedStatus,
            CreationDate = CreationDate
        };
    }
}