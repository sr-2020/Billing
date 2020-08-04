using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Billing;
using Billing.Dto.Shop;
using BillingAPI.Filters;
using Core;
using IoC;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("shop")]
    public class ShopManagerController : Controller
    {
        //[Route("Details")]
        //[ShopAuthorization]
        //public IActionResult Details(int shop)
        //{
        //    var manager = IocContainer.Get<IShopManager>();
        //    var dto = manager.GetShop(shop);
        //    return View(dto);
        //}
        //[Route("Available")]
        //[ShopAuthorization]
        //public IActionResult Available(int shop)
        //{
        //    var manager = IocContainer.Get<IShopManager>();
        //    var list = manager.GetAvailableQR(shop);
        //    var name = manager.GetShopName(shop);
        //    var model = new ShopAvailableViewModel(list, shop, name);
        //    return View(model);
        //}

    }
}