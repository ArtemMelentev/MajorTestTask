using System.Collections.ObjectModel;
using CargoApp.Models;
using CargoApp.Utilities;
using CargoApp.Utilities.Enums;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using AppContext = CargoApp.DB.AppContext;

namespace CargoApp.ViewModels;

public class OrderTableViewModel : ViewModelBase
{
    private readonly IMessageService _messageService;
    private readonly IUIVisualizerService _uiVisualizerService;
    
    public Command SearchOrderCommand { get; set; }
    public TaskCommand SubmitInProcessCommand { get; set; }
    
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

    private string _searchQuery = String.Empty;
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            RaisePropertyChanged(nameof(SearchQuery));
        }
    }

    private int _selectedOrderIndex = -1;
    public int SelectedOrderIndex
    {
        get => _selectedOrderIndex;
        set
        {
            _selectedOrderIndex = value;
            RaisePropertyChanged(nameof(SelectedOrderIndex));
        }
    }

    public OrderTableViewModel()
    {
        _messageService = DependencyResolver.Resolve<IMessageService>();
        _uiVisualizerService = DependencyResolver.Resolve<IUIVisualizerService>();
        
        LoadOrders();
        SearchOrderCommand = new Command(SearchOrder);
        SubmitInProcessCommand = new TaskCommand(SubmitInProcessAsync);
    }

    private void LoadOrders()
    {
        using var context = new AppContext();
        Orders = new ObservableCollection<OrderModel>(context.Orders.ToList());
        ApplyFilter(SearchQuery);
    }

    private void SearchOrder()
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
            var booltest = FilteredOrders[0].CourierName?.ToLower().Contains(lowerCaseQuery);
            FilteredOrders = new ObservableCollection<OrderModel>(
                Orders.Where(order =>
                    (order.ClientName?.ToLower().Contains(lowerCaseQuery) ?? false) ||
                    (order.CourierName?.ToLower().Contains(lowerCaseQuery) ?? false) ||
                    (order.CargoDetails?.ToLower().Contains(lowerCaseQuery) ?? false) ||
                    (order.PickupAddress?.ToLower().Contains(lowerCaseQuery) ?? false) ||
                    (order.DeliveryAddress?.ToLower().Contains(lowerCaseQuery) ?? false) ||
                    (order.Status.ToString().ToLower().Contains(lowerCaseQuery)) ||
                    (order.Comment?.ToLower().Contains(lowerCaseQuery) ?? false) ||
                    order.CreationDate.ToString("g").ToLower().Contains(lowerCaseQuery))
            );
        }
    }

    private async Task SubmitInProcessAsync()
    {
        if (SelectedOrderIndex >= FilteredOrders.Count || SelectedOrderIndex < 0)
        {
            await _messageService.ShowAsync("Индекс вне диапазона массива заявок");
            return;
        }

        string inputName = "Введите данные курьера";
        var inputVm = new InputViewModel(inputName, true, true, new InputField(String.Empty, inputName));
        var result = await _uiVisualizerService.ShowDialogAsync(inputVm);
        bool isInputResultInvalid = String.IsNullOrEmpty(inputVm.Results[0]) || result.DialogResult == false;
        if (isInputResultInvalid)
        {
            await _messageService.ShowAsync("Введено некорректное значение");
            return;
        }
        var order = FilteredOrders[SelectedOrderIndex];
        order.CourierName = inputVm.Results[0]!;
        order.Status = OrderStatus.InProcess;
    }
}