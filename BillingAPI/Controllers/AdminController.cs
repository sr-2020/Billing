using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Billing;
using Billing.Dto;
using Billing.DTO;
using Core;
using Core.Model;
using Core.Primitives;
using FileHelper;
using IoC;
using Microsoft.AspNetCore.Authorization;
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
                    manager.Delete<ProductType>(item.ProductTypeId);
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
        public IActionResult UploadPProductTypeList(Microsoft.AspNetCore.Http.IFormFile formFile)
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

        [Microsoft.AspNetCore.Mvc.HttpGet]
        public IActionResult Editpt(int id)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var dto = manager.GetProductTypes(id).FirstOrDefault();
            if (dto == null)
                dto = new ProductTypeDto();
            return View(dto);

        }
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IActionResult Editpt(ProductTypeDto dto)
        {
            var manager = IocContainer.Get<IBillingManager>();
            manager.CreateOrUpdateProductType(dto.ProductTypeId, dto.ProductTypeName, dto.DiscountType);
            return RedirectToAction("ProductList");
        }
        public IActionResult Deletept(int id)
        {
            var manager = IocContainer.Get<IBillingManager>();
            manager.Delete<ProductType>(id);
            return RedirectToAction("ProductList");
        }

        #endregion

        #region nomenklatura
        public IActionResult NomenklaturaList(int productid)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var list = manager.GetNomenklaturas(productid, 0);
            var model = new NomenklaturaPage();
            model.ProductTypeId = productid;
            model.Items = list;

            return View(model);
        }
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public IActionResult Editnm(int id, int productId)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var dto = manager.GetNomenklaturas(0, 0, id).FirstOrDefault();
            if (dto == null)
            {
                dto = new NomenklaturaDto();
                dto.ProductTypeId = productId;
            }
            return View(dto);
        }
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IActionResult Editnm(NomenklaturaDto dto)
        {
            var manager = IocContainer.Get<IBillingManager>();

            manager.CreateOrUpdateNomenklatura(dto.NomenklaturaId, dto.NomenklaturaName, dto.Code, dto.ProductTypeId, dto.LifeStyleId, dto.BasePrice, dto.Description, dto.UrlPicture);
            return RedirectToAction("NomenklaturaList", new { productid = dto.ProductTypeId });
        }

        public IActionResult Deletenm(int id, int productid)
        {
            var manager = IocContainer.Get<IBillingManager>();
            manager.Delete<Nomenklatura>(id);
            return RedirectToAction("NomenklaturaList", new { productid });
        }

        #endregion
        #region sku
        public IActionResult SkuList(int nomenklaturaid, int productid)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var list = manager.GetSkus(0, nomenklaturaid, null);
            var model = new SkuPage();
            model.NomenklaturaId = nomenklaturaid;
            model.ProductTypeId = productid;
            model.Items = list;
            return View(model);
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        public IActionResult Editsku(int id, int nomenklaturaid, int productid)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var dto = manager.GetSkus(0, 0, null, id).FirstOrDefault();
            if (dto == null)
            {
                dto = new SkuDto();
                dto.ProductTypeId = productid;
                dto.NomenklaturaId = nomenklaturaid;
            }
            return View(dto);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public IActionResult Editsku(SkuDto dto)
        {
            var manager = IocContainer.Get<IBillingManager>();
            manager.CreateOrUpdateSku(dto.SkuId, dto.NomenklaturaId, dto.Count, dto.CorporationId, dto.SkuName, dto.Enabled);
            return RedirectToAction("SkuList", new { productid = dto.ProductTypeId, nomenklaturaid = dto.NomenklaturaId});
        }
        public IActionResult Deletesku(int id, int nomenklaturaid, int productid)
        {
            var manager = IocContainer.Get<IBillingManager>();
            manager.Delete<Sku>(id);
            return RedirectToAction("SkuList", new { productid, nomenklaturaid = nomenklaturaid });
        }

        #endregion
    }
}