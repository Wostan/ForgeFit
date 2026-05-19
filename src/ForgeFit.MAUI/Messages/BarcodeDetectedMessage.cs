using ForgeFit.MAUI.Models.DTOs.Food;

namespace ForgeFit.MAUI.Messages;

public sealed class BarcodeDetectedMessage(string barcode, FoodProductResponse product)
{
    public string Barcode { get; } = barcode;
    public FoodProductResponse Product { get; } = product;
}
