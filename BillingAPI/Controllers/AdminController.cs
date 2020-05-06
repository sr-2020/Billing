using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Billing;
using Billing.DTO;
using Core;
using FileHelper;
using IoC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingAPI.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        #region product_type

        public IActionResult ProductList()
        {
            var manager = IocContainer.Get<IBillingManager>();
            var list = manager.GetProductTypes();
            return View(list);
        }

        public IActionResult Deleteallpt()
        {
            var manager = IocContainer.Get<IBillingManager>();
            var list = manager.GetProductTypes(); 
            var count = 0;
            var errors = 0;
            foreach (var item in list)
            {
                try
                {
                    manager.DeleteProductType(item.ProductTypeId);
                    count++;
                }
                catch (Exception e) 
                {
                    manager.RefreshContext();
                    errors++;
                }
            }
            var result = $"количество удаленных записей {count}, ошибок {errors}";
            return Content(result);
        }
        public IActionResult UploadPProductTypeList(IFormFile formFile)
        {
            if (formFile == null)
            {
                throw new Exception("file not found");
            }
            var parcer = new CsvParcer();
            var list = parcer.ParceCsv<ProductTypeDto>(formFile);
            var manager = IocContainer.Get<IBillingManager>();
            var count = 0;
            var errors = 0;
            foreach (var item in list)
            {
                try
                {
                    manager.CreateOrUpdateProductType(0, item.ProductTypeName, item.DiscountType);
                    count++;
                }
                catch (Exception e)
                {
                    errors++;
                }
            }
            var result = $"количество добавленных записей {count}, ошибок {errors}";
            return Content(result);
        }

        [System.Web.Http.HttpGet]
        public IActionResult Editpt(int id)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var dto = manager.GetProductTypes(id).FirstOrDefault();
            if (dto == null)
                dto = new ProductTypeDto();
            return View(dto);

        }
        [System.Web.Http.HttpPost]
        public IActionResult Editpt(ProductTypeDto dto)
        {
            var manager = IocContainer.Get<IBillingManager>();
            manager.CreateOrUpdateProductType(dto.ProductTypeId, dto.ProductTypeName, dto.DiscountType);
            return RedirectToAction("ProductList");
        }
        public IActionResult Deletept(int id)
        {
            var manager = IocContainer.Get<IBillingManager>();
            manager.DeleteProductType(id);
            return RedirectToAction("ProductList");
        }

        #endregion
    }
}