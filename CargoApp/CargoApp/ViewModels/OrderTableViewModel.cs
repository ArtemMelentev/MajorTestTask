using System.Collections.ObjectModel;
using CargoApp.Models;
using Catel.MVVM;
using AppContext = CargoApp.DB.AppContext;

namespace CargoApp.ViewModels;

public class OrderTableViewModel : ViewModelBase
{
    public TaskCommand SearchOrderCommand { get; set; }
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
    
    private ObservableCollection<OrderModel> _filteredOrders;
    public ObservableCollection<OrderModel> FilteredOrders
    {
        get => _filteredOrders;
        set
        {
            _filteredOrders = value;
            RaisePropertyChanged(nameof(FilteredOrders));
        }
    }

    private string _searchQuery;
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            RaisePropertyChanged(nameof(SearchQuery));
        }
    }

    public OrderTableViewModel() 
    {
        LoadOrders();
        SearchOrderCommand = new TaskCommand(SearchOrderAsync);
    }

    private void LoadOrders()
    {
        using var context = new AppContext();
        Orders = new ObservableCollection<OrderModel>(context.Orders.ToList());
        ApplyFilter(SearchQuery);
    }

    private async Task SearchOrderAsync()
    {
        ApplyFilter(SearchQuery);
    }
    
    private void ApplyFilter(string searchQuery)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            FilteredOrders = new ObservableCollection<OrderModel>(Orders);
        }
        else
        {
            var lowerCaseQuery = searchQuery.ToLower();
            FilteredOrders = new ObservableCollection<OrderModel>(
                Orders.Where(order =>
                    (order.ClientName?.ToLower().Contains(lowerCaseQuery) ?? false) ||
                    (order.CargoDetails?.ToLower().Contains(lowerCaseQuery) ?? false) ||
                    (order.PickupAddress?.ToLower().Contains(lowerCaseQuery) ?? false) ||
                    (order.DeliveryAddress?.ToLower().Contains(lowerCaseQuery) ?? false) ||
                    (order.Status.ToString().Contains(lowerCaseQuery)) ||
                    (order.Comment?.ToLower().Contains(lowerCaseQuery) ?? false) ||
                    order.CreationDate.ToString("g").ToLower().Contains(lowerCaseQuery))
            );
        }
    }
}