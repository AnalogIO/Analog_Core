using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CoffeeCard.WebApi.Models;
using CoffeeCard.WebApi.Models.DataTransferObjects.MobilePay;
using CoffeeCard.WebApi.Models.DataTransferObjects.Purchase;

namespace CoffeeCard.WebApi.Services
{
    public interface IPurchaseService : IDisposable
    {
        IEnumerable<Purchase> GetPurchases(IEnumerable<Claim> claims);
        Purchase RedeemVoucher(string voucherCode, IEnumerable<Claim> claims);
        string InitiatePurchase(int productId, IEnumerable<Claim> claims);
        Task<Purchase> CompletePurchase(CompletePurchaseDto dto, User user);
        Task CheckIncompletePurchases(User user);
        Purchase IssueProduct(IssueProductDto issueProduct);
    }
}