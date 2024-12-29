using System.Collections.ObjectModel;
using CargoApp.Models;
using CargoApp.Utilities.Enums;
using NpgsqlTypes;

namespace CargoApp.ViewModels;

public class OrderViewModel : InputViewModelBase
{
    private double _weight;
    private double _x;
    private double _y;
    private double _z;
    private string _clientName = String.Empty;
    private string _courierName = String.Empty;
    private string _pickupAddress = String.Empty;
    private string _deliveryAddress = String.Empty;
    private string _comment = String.Empty;
    private DateTime _creationDate = DateTime.Now;
    private OrderStatus _orderStatus = OrderStatus.New;
    private OrderWindowMode _orderWindowMode;
    
    public ObservableCollection<OrderStatus> OrderStatuses { get; }

    public double Weight
    {
        get => _weight;
        set
        {
            _weight = value;
            RaisePropertyChanged(nameof(Weight));
        }
    }
    
    public double X
    {
        get => _x;
        set
        {
            _x = value;
            RaisePropertyChanged(nameof(X));
        }
    }
    
    public double Y
    {
        get => _y;
        set
        {
            _y = value;
            RaisePropertyChanged(nameof(Y));
        }
    }
    
    public double Z
    {
        get => _z;
        set
        {
            _z = value;
            RaisePropertyChanged(nameof(Z));
        }
    }
    
    public string ClientName
    {
        get => _clientName;
        set
        {
            _clientName = value;
            RaisePropertyChanged(nameof(ClientName));
        }
    }

    public string CourierName
    {
        get => _courierName;
        set
        {
            _courierName = value;
            RaisePropertyChanged(nameof(CourierName));
        }
    }

    public string PickupAddress
    {
        get => _pickupAddress;
        set
        {
            _pickupAddress = value;
            RaisePropertyChanged(nameof(PickupAddress));
        }
    }

    public string DeliveryAddress
    {
        get => _deliveryAddress;
        set
        {
            _deliveryAddress = value;
            RaisePropertyChanged(nameof(DeliveryAddress));
        }
    }

    public string Comment
    {
        get => _comment;
        set
        {
            _comment = value;
            RaisePropertyChanged(nameof(Comment));
        }
    }

    public DateTime CreationDate
    {
        get => _creationDate.ToUniversalTime();
        set
        {
            _creationDate = value;
            RaisePropertyChanged(nameof(CreationDate));
        }
    }
    
    public OrderStatus SelectedStatus
    {
        get => _orderStatus;
        set
        {
            _orderStatus = value;
            RaisePropertyChanged(nameof(SelectedStatus));
        }
    }

    public bool IsShowClientName => SelectedStatus == OrderStatus.New || _orderWindowMode is OrderWindowMode.Input;

    public bool IsShowCourierName => (SelectedStatus is OrderStatus.New or OrderStatus.InProcess) ||
                                     _orderWindowMode is OrderWindowMode.Input;
    public bool IsShowCargoDetails =>  SelectedStatus == OrderStatus.New || _orderWindowMode is OrderWindowMode.Input;
    public bool IsShowPickupAddress => SelectedStatus == OrderStatus.New || _orderWindowMode is OrderWindowMode.Input;
    public bool IsShowDeliveryAddress => SelectedStatus == OrderStatus.New || _orderWindowMode is OrderWindowMode.Input;

    public bool IsShowComment => SelectedStatus is OrderStatus.New or OrderStatus.Canceled ||
                                 _orderWindowMode is OrderWindowMode.Input;
    public bool IsShowCreationDate => SelectedStatus == OrderStatus.New && _orderWindowMode != OrderWindowMode.Input;
    public bool IsShowStatus => _orderWindowMode != OrderWindowMode.Input;

    public OrderViewModel(string title, OrderModel orderModel, OrderWindowMode orderWindowMode, bool canOK = false,
        bool canCancel = false) : base(title, canOK, canCancel)
    {
        _orderWindowMode = orderWindowMode;

        Weight = orderModel.Weight;
        X = orderModel.X;
        Y = orderModel.Y;
        Z = orderModel.Z;
        ClientName = orderModel.ClientName;
        CourierName = orderModel.CourierName;
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
            Weight = Weight,
            X = X,
            Y = Y,
            Z = Z,
            ClientName = ClientName,
            CourierName = CourierName,
            PickupAddress = PickupAddress,
            DeliveryAddress = DeliveryAddress,
            Comment = Comment,
            Status = SelectedStatus,
            CreationDate = CreationDate
        };
    }
}