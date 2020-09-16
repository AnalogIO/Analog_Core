using System.Threading.Tasks;
using CoffeeCard.WebApi.Helpers;
using CoffeeCard.WebApi.Models.DataTransferObjects.MobilePay;
using CoffeeCard.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCard.WebApi.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiController]
    public class MobilePayController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;
        private readonly ClaimsUtilities _claimsUtilities;

        public MobilePayController(IPurchaseService purchaseService, ClaimsUtilities claimsUtilities)
        {
            _purchaseService = purchaseService;
            _claimsUtilities = claimsUtilities;
        }

        /// <summary>
        ///     Initiates a purchase from the given productId and returns an orderId
        /// </summary>
        [HttpPost("initiate")]
        public IActionResult InitiatePurchase(InitiatePurchaseDto initiatePurchaseDto)
        {
            var orderId = _purchaseService.InitiatePurchase(initiatePurchaseDto.ProductId, User.Claims);
            return Ok(new InitiatePurchaseResponseDto {OrderId = orderId});
        }

        /// <summary>
        ///     Validates the purchase against mobilepay backend and delivers the tickets if succeeded
        /// </summary>
        [HttpPost("complete")]
        public async Task<IActionResult> CompletePurchase(CompletePurchaseDto dto)
        {
            var user = await _claimsUtilities.ValidateAndReturnUserFromClaimAsync(User.Claims);
            await _purchaseService.CompletePurchase(dto, user);
            return Ok(new {Message = "The purchase was completed with success!"});
        }
    }
}