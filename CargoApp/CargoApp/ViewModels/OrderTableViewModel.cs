﻿using System.Collections.ObjectModel;
using CargoApp.DB;
using CargoApp.Models;
using CargoApp.Utilities;
using CargoApp.Utilities.Enums;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;

namespace CargoApp.ViewModels;

public class OrderTableViewModel : ViewModelBase
{
    private readonly IMessageService _messageService;
    private readonly IUIVisualizerService _uiVisualizerService;
    private readonly DBContext _dbContext;
    
    public Command SearchOrderCommand { get; set; }
    public TaskCommand SubmitInProcessCommand { get; private set; }
    public TaskCommand SaveToDatabaseCommand { get; private set; }
    public TaskCommand DeleteOrderCommand { get; private set; }
    public TaskCommand EditOrderCommand { get; private set; }
    public TaskCommand CreateOrderCommand { get; private set; }
    
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
        _dbContext = ServiceLocator.Default.ResolveType<DBContext>();
        
        LoadOrders();
        SearchOrderCommand = new Command(SearchOrder);
        SubmitInProcessCommand = new TaskCommand(SubmitInProcessAsync);
        SaveToDatabaseCommand = new TaskCommand(SaveToDatabaseAsync);
        DeleteOrderCommand = new TaskCommand(DeleteOrderAsync);
        EditOrderCommand = new TaskCommand(EditOrderAsync);
        CreateOrderCommand = new TaskCommand(CreateOrderAsync);
    }

    private void LoadOrders()
    {
        Orders = new ObservableCollection<OrderModel>(_dbContext.Orders.ToList());
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
        if (!IsSelectedIndexCorrect())
        {
            await _messageService.ShowAsync(Strings.OrderIndexError);
            return;
        }
        var order = FilteredOrders[SelectedOrderIndex];
        if (order.Status != OrderStatus.New)
        {
            return;
        }
        
        string inputName = Strings.CourierInfoIput;
        var inputVm = new InputViewModel(inputName, true, true, new InputField(inputName));
        var result = await _uiVisualizerService.ShowDialogAsync(inputVm);
        bool isInputResultInvalid = String.IsNullOrEmpty(inputVm.Results[0]) || result.DialogResult == false;
        if (isInputResultInvalid)
        {
            await _messageService.ShowAsync(Strings.InputError);
            return;
        }
        
        order.CourierName = inputVm.Results[0]!;
        order.Status = OrderStatus.InProcess;
    }

    private async Task SaveToDatabaseAsync()
    {
        try
        {
            foreach (var order in FilteredOrders)
            {
                _dbContext.Orders.Update(order);
            }
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await _messageService.ShowAsync(Strings.SaveChangesToDBError + ex.Message);
        }
    }

    private async Task DeleteOrderAsync()
    {
        if (!IsSelectedIndexCorrect())
        {
            await _messageService.ShowAsync("Индекс вне диапазона массива заявок");
            return;
        }
        try
        {
            var order = FilteredOrders[SelectedOrderIndex];
            Orders.Remove(order);
            FilteredOrders.RemoveAt(SelectedOrderIndex);
            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await _messageService.ShowAsync(Strings.DeleteOrderError + ex.Message);
        }
    }

    private async Task EditOrderAsync()
    {
        if (!IsSelectedIndexCorrect())
        {
            await _messageService.ShowAsync("Индекс вне диапазона массива заявок");
            return;
        }

        var originOrder = FilteredOrders[SelectedOrderIndex];
        string title = Strings.EditOrderVMName;
        var inputVM = new EditOrderViewModel(title, originOrder, canOK: true, canCancel: true);
        var res = await _uiVisualizerService.ShowDialogAsync(inputVM);
        if (res.DialogResult != true)
        {
            await _messageService.ShowAsync(Strings.EditOrderCancelMessage);
            return;
        }
        
        originOrder.ClientName = inputVM.ClientName;
        originOrder.CourierName = inputVM.CourierName;
        originOrder.CargoDetails = inputVM.CargoDetails;
        originOrder.PickupAddress = inputVM.PickupAddress;
        originOrder.DeliveryAddress = inputVM.DeliveryAddress;
        originOrder.Comment = inputVM.Comment;
        originOrder.Status = inputVM.SelectedStatus;
        originOrder.CreationDate = inputVM.CreationDate;
    }
    
    private async Task CreateOrderAsync()
    {
        try
        {
            string title = Strings.InputOrderInfoVMName;
            var inputVM = new InputViewModel(title,
                canOK: true, canCancel: true,
                new InputField(Strings.ClientName),
                new InputField(Strings.CourierName),
                new InputField(Strings.CargoDetails),
                new InputField(Strings.PickupAddress),
                new InputField(Strings.DeliveryAddress),
                new InputField(Strings.Comment));
            var res = await _uiVisualizerService.ShowDialogAsync(inputVM);
            if (res.DialogResult != true)
            {
                await _messageService.ShowAsync(Strings.InputOrderCancelMessage);
                return;
            }
         
            var order = new OrderModel
            {
                ClientName = inputVM.Results[0],
                CourierName = inputVM.Results[1],
                CargoDetails = inputVM.Results[2],
                PickupAddress = inputVM.Results[3],
                DeliveryAddress = inputVM.Results[4],
                Comment = inputVM.Results[5],
                Status = OrderStatus.New
            };

            FilteredOrders.Add(order);
            
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await _messageService.ShowAsync(Strings.CreateOrderError + ex.Message);
        }
    }

    private bool IsSelectedIndexCorrect()
    {
        return SelectedOrderIndex < FilteredOrders.Count && SelectedOrderIndex >= 0;
    }
}