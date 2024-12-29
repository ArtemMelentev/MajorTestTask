using CargoApp.Models;
using CargoApp.Utilities.Enums;

namespace CargoApp.Utilities.ExtensionClasses;

public static class OrderModelExtensionMethods
{
    public static bool IsCorrect(this OrderModel order)
    {
        bool isClientNameCorrect = !String.IsNullOrEmpty(order.ClientName);
        bool isPickupAddressCorrect = !String.IsNullOrEmpty(order.PickupAddress);

        bool isOrderCorrect = isClientNameCorrect && isPickupAddressCorrect;
        
        switch (order.Status)
        {
            case OrderStatus.InProcess:
                isOrderCorrect = isOrderCorrect && !String.IsNullOrEmpty(order.CourierName);
                break;
            case OrderStatus.Canceled:
                isOrderCorrect = isOrderCorrect && !String.IsNullOrEmpty(order.Comment);
                break;
        }

        return isOrderCorrect;
    }
}