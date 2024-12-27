using System.Collections.ObjectModel;
using CargoApp.Models;
using Catel.MVVM;
using AppContext = CargoApp.DB.AppContext;

namespace CargoApp.ViewModels;

public class OrderTableViewModel : ViewModelBase
{
    private ObservableCollection<OrderModel> _orders;
    public ObservableCollection<OrderModel> Orders
    {
        get => _orders;
        set
        {
            _orders = value;
            RaisePropertyChanged(nameof(Orders));
        }
    }

    public OrderTableViewModel() 
    {
        LoadOrders();
    }

    private void LoadOrders()
    {
        using var context = new AppContext();
        Orders = new ObservableCollection<OrderModel>(context.Orders.ToList());
    }
}