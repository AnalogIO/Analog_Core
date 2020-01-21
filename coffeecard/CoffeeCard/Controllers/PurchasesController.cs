﻿using System.Collections.Generic;
using System.Linq;
using CoffeeCard.Helpers;
using CoffeeCard.Models.DataTransferObjects.Purchase;
using CoffeeCard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace CoffeeCard.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private IAccountService _accountService;
        private readonly IConfiguration _configuration;
        private readonly IMapperService _mapperService;
        private readonly IPurchaseService _purchaseService;

        public PurchasesController(IPurchaseService purchaseService, IMapperService mapper,
            IAccountService accountService, IConfiguration configuration)
        {
            _purchaseService = purchaseService;
            _mapperService = mapper;
            _accountService = accountService;
            _configuration = configuration;
        }

        /// <summary>
        ///     Returns a list of purchases for the given user via the supplied token in the header
        /// </summary>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiError), 401)]
        public ActionResult<IEnumerable<PurchaseDTO>> Get()
        {
            var purchases = _purchaseService.GetPurchases(User.Claims);
            return _mapperService.Map(purchases).ToList();
        }

        /// <summary>
        ///     Redeems the voucher supplied as parameter in the path
        /// </summary>
        [HttpPost("redeemvoucher")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiError), 401)]
        public ActionResult<PurchaseDTO> RedeemVoucher(string voucherCode)
        {
            var purchase = _purchaseService.RedeemVoucher(voucherCode, User.Claims);
            return _mapperService.Map(purchase);
        }

        /// <summary>
        ///     Issue purchase used by the ipad in the cafe
        /// </summary>
        /// <param name="issueProduct"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("issueproduct")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiError), 401)]
        public ActionResult<PurchaseDTO> IssueProduct(IssueProductDTO issueProduct)
        {
            var adminToken = Request.Headers.FirstOrDefault(x => x.Key == "admintoken").Value.FirstOrDefault();
            Log.Information(adminToken);
            if (adminToken != _configuration["AdminToken"]) throw new ApiException("AdminToken was invalid", 401);
            var purchase = _purchaseService.IssueProduct(issueProduct);
            return _mapperService.Map(purchase);
        }
    }
}