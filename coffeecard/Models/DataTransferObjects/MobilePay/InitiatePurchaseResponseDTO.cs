using System.ComponentModel.DataAnnotations;

namespace coffeecard.Models.DataTransferObjects.MobilePay
{
    public class InitiatePurchaseResponseDTO
    {
        public string OrderId { get; set; }
    }
}