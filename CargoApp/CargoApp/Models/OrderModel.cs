using System.ComponentModel;
using System.Runtime.CompilerServices;
using CargoApp.Utilities.Enums;

namespace CargoApp.Models;


public class OrderModel : INotifyPropertyChanged
{
    private string _clientName = String.Empty;
    private string _courierName = String.Empty;
    private string _cargoDetails = String.Empty;
    private string _pickupAddress = String.Empty;
    private string _deliveryAddress = String.Empty;
    private OrderStatus _status = OrderStatus.New;
    private string _comment = String.Empty;
    private DateTime _creationDate = DateTime.Now;

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
            OnPropertyChanged();
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
            OnPropertyChanged();
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
            OnPropertyChanged();
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
            OnPropertyChanged();
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
            OnPropertyChanged();
        }
    }

    public OrderStatus Status
    {
        get => _status;
        set
        {
            if (value == _status)
            {
                return;
            }
            _status = value;
            OnPropertyChanged();
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
            OnPropertyChanged();
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
            OnPropertyChanged();
        }
    }

    public int Id { get; set; }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return "Order : " + ClientName + " " + CourierName + " " + CargoDetails + " " + PickupAddress + " " +
               DeliveryAddress + " " + CreationDate + " " + Comment;
    }
}