using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCard.Common.Configuration;
using CoffeeCard.MobilePay.Service;
using CoffeeCard.WebApi.Models;
using CoffeeCard.WebApi.Services;
using Moq;
using Xunit;

namespace CoffeeCard.Tests.Unit.Services
{
    public class PurchaseServiceTest
    {
        [Fact(DisplayName = "CheckIncompletePurchases given a user with no purchases does nothing")]
        public async Task CheckIncompletePurchasesGivenAUserWithNoPurchasesDoesNothing()
        {
            // Arrange
            var mockMobilePayService = new Mock<IMobilePayService>();
            var mockEmailService = new Mock<IEmailService>();
            var mockMapperService = new Mock<IMapperService>();
            var mobilePaySettings = new MobilePaySettings();

            var purchaseService = new PurchaseService(null, mockMobilePayService.Object, mobilePaySettings, mockEmailService.Object, mockMapperService.Object);

            var user = new User();

            await purchaseService.CheckIncompletePurchases(user);

            mockMobilePayService.Verify(m => m.GetPaymentStatus(It.IsAny<string>()), Times.Never());
        }

        [Fact(DisplayName = "CheckIncompletePurchases given a user with incomplete purchases older than one day does nothing")]
        public async Task CheckIncompletePurchasesGivenAUserWithIncompletePurchasesOlderThanOneDayDoesNothing()
        {
            // Arrange
            var mockMobilePayService = new Mock<IMobilePayService>();
            var mockEmailService = new Mock<IEmailService>();
            var mockMapperService = new Mock<IMapperService>();
            var mobilePaySettings = new MobilePaySettings();

            var purchaseService = new PurchaseService(null, mockMobilePayService.Object, mobilePaySettings, mockEmailService.Object, mockMapperService.Object);

            var user = new User()
            {
                Purchases = new List<Purchase>()
                { 
                    new Purchase()
                    {
                        Completed = false,
                        DateCreated = DateTime.UtcNow.AddDays(-2)
                    },
                    new Purchase()
                    {
                        Completed = false,
                        DateCreated = DateTime.UtcNow.AddDays(-3)
                    }
                }
            };

            await purchaseService.CheckIncompletePurchases(user);

            mockMobilePayService.Verify(m => m.GetPaymentStatus(It.IsAny<string>()), Times.Never());
        }
    }
}
