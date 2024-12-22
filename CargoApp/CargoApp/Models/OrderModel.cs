namespace CargoApp.Models;


public class OrderModel
{
    public int Id { get; set; }
    public string ClientName { get; set; }
    public string CourierName { get; set; }
    public string CargoDetails { get; set; }
    public string PickupAddress { get; set; }
    public string DeliveryAddress { get; set; }
    public string Status { get; set; } = "Новая";
    public string Comment { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.Now;
}