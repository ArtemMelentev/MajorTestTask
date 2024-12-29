using System.ComponentModel;
using System.Runtime.CompilerServices;
using CargoApp.Utilities;
using CargoApp.Utilities.Enums;

namespace CargoApp.Models;


public class OrderModel : INotifyPropertyChanged
{
    private string _clientName = String.Empty;
    private string _courierName = String.Empty;
    private double _weight;
    private double _x;
    private double _y; 
    private double _z; 
    private string _pickupAddress = String.Empty;
    private string _deliveryAddress = String.Empty;
    private OrderStatus _status = OrderStatus.New;
    private string _comment = String.Empty;
    private DateTime _creationDate = DateTime.Now;

    public double Weight
    {
        get => _weight;
        set
        {
            _weight = value;
            OnPropertyChanged();
        }
    }
    
    public double X
    {
        get => _x;
        set
        {
            _x = value;
            OnPropertyChanged();
        }
    }
    
    public double Y
    {
        get => _y;
        set
        {
            _y = value;
            OnPropertyChanged();
        }
    }
    
    public double Z
    {
        get => _z;
        set
        {
            _z = value;
            OnPropertyChanged();
        }
    }
    
    public string ClientName
    {
        get => _clientName;
        set
        {
            _clientName = value;
            OnPropertyChanged();
        }
    }

    public string CourierName
    {
        get => _courierName;
        set
        {
            _courierName = value;
            OnPropertyChanged();
        }
    }

    public string PickupAddress
    {
        get => _pickupAddress;
        set
        {
            _pickupAddress = value;
            OnPropertyChanged();
        }
    }

    public string DeliveryAddress
    {
        get => _deliveryAddress;
        set
        {
            _deliveryAddress = value;
            OnPropertyChanged();
        }
    }

    public OrderStatus Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged();
        }
    }

    public string Comment
    {
        get => _comment;
        set
        {
            _comment = value;
            OnPropertyChanged();
        }
    }

    public DateTime CreationDate
    {
        get => _creationDate;
        set
        {
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
        return "Order : " + ClientName + " " + CourierName + " " + PickupAddress + " " +
               Weight + " " + X + " " + Y + " " + Z + " " + DeliveryAddress + " " + CreationDate + " " + Comment;
    }
}