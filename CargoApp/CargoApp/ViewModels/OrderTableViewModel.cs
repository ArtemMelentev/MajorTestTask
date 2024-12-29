using System.Collections.ObjectModel;
using CargoApp.DB;
using CargoApp.Models;
using CargoApp.Utilities;
using CargoApp.Utilities.Enums;
using CargoApp.Utilities.ExtensionClasses;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;

namespace CargoApp.ViewModels;

public class OrderTableViewModel : ViewModelBase
{
    private readonly IMessageService _messageService;
    private readonly IUIVisualizerService _uiVisualizerService;
    private readonly DBContext _dbContext;
    
    private ObservableCollection<OrderModel> _orders;
    private ObservableCollection<OrderModel> _filteredOrders;
    private string _searchQuery = String.Empty;
    private int _selectedOrderIndex = -1;
    
    public Command SearchOrderCommand { get; private set; }
    public TaskCommand SubmitInProcessCommand { get; private set; }
    public TaskCommand SaveToDatabaseCommand { get; private set; }
    public TaskCommand DeleteOrderCommand { get; private set; }
    public TaskCommand EditOrderCommand { get; private set; }
    public TaskCommand CreateOrderCommand { get; private set; }
    
    public ObservableCollection<OrderModel> Orders
    {
        get => _orders;
        set
        {
            _orders = value;
            RaisePropertyChanged(nameof(Orders));
        }
    }
    
    public ObservableCollection<OrderModel> FilteredOrders
    {
        get => _filteredOrders;
        set
        {
            _filteredOrders = value;
            RaisePropertyChanged(nameof(FilteredOrders));
        }
    }
    
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            RaisePropertyChanged(nameof(SearchQuery));
        }
    }
    
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
        bool isCorrect = await IsSelectedIndexCorrect();
        if (!isCorrect)
        {
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
        bool isCorrect = await IsSelectedIndexCorrect();
        if (!isCorrect)
        {
            return;
        }

        bool result = await ShowQuestionBeforeDeletionAsync();
        if (!result)
        {
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
        bool isCorrect = await IsSelectedIndexCorrect();
        if (!isCorrect)
        {
            return;
        }

        var originOrder = FilteredOrders[SelectedOrderIndex];
        string title = Strings.EditOrderVMName;
        var inputVM = new OrderViewModel(title, originOrder, OrderWindowMode.Edit,canOK: true, canCancel: true);
        var res = await _uiVisualizerService.ShowDialogAsync(inputVM);
        if (res.DialogResult != true)
        {
            await _messageService.ShowAsync(Strings.EditOrderCancelMessage);
            return;
        }

        var newOrder = inputVM.GetOrderModel();
        if (!newOrder.IsCorrect())
        {
            await _messageService.ShowAsync(Strings.OrderIncorrect);
            return;
        }

        originOrder.Weight = inputVM.Weight;
        originOrder.X = inputVM.X;
        originOrder.Y = inputVM.Y;
        originOrder.Z = inputVM.Z;
        originOrder.ClientName = inputVM.ClientName;
        originOrder.CourierName = inputVM.CourierName;
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
            var originOrder = new OrderModel();
            var inputVM = new OrderViewModel(title, originOrder, OrderWindowMode.Input, canOK: true, canCancel: true);
            var res = await _uiVisualizerService.ShowDialogAsync(inputVM);
            if (res.DialogResult != true)
            {
                await _messageService.ShowAsync(Strings.InputOrderCancelMessage);
                return;
            }

            var order = inputVM.GetOrderModel();
            if (!order.IsCorrect())
            {
                await _messageService.ShowAsync(Strings.OrderIncorrect);
                return;
            }
            FilteredOrders.Add(order);
            Orders.Add(order);
            
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
            await _messageService.ShowAsync(Strings.SaveChangesToDBSuccessful);
        }
        catch (Exception ex)
        {
            await _messageService.ShowAsync(Strings.CreateOrderError + ex.Message);
        }
    }
    
    private async Task<bool> IsSelectedIndexCorrect()
    {
        if (SelectedOrderIndex >= FilteredOrders.Count || SelectedOrderIndex < 0)
        {
            await _messageService.ShowAsync(Strings.OrderIndexError);
            return false;
        }

        return true;
    }
    
    private async Task<bool> ShowQuestionBeforeDeletionAsync()
    {
        var order = FilteredOrders[SelectedOrderIndex];
        
        var messageVM = new MessageViewModel(String.Empty, Strings.DeleteOrderQuestion + " " + order);
        var messageResult = await _uiVisualizerService.ShowDialogAsync(messageVM);

        return messageResult.DialogResult ?? false;
    }
}