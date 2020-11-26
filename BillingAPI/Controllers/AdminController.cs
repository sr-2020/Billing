using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Billing;
using Billing.Dto;
using Billing.DTO;
using CommonExcel;
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

        [Microsoft.AspNetCore.Mvc.HttpGet]
        public FileResult LoadExcel()
        {
            var manager = new ExcelManager();
            var memStream = manager.LoadMainExcel();

            byte[] fileBytes = memStream.ToArray();
            string fileName = "shreconomics.xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

        }


        [Microsoft.AspNetCore.Mvc.HttpGet]
        public FileResult LoadTransfers()
        {
            var manager = new ExcelManager();
            var memStream = manager.LoadTransfers();

            byte[] fileBytes = memStream.ToArray();
            string fileName = "transfers.xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        public FileResult LoadRents()
        {
            var manager = new ExcelManager();
            var memStream = manager.LoadRents();

            byte[] fileBytes = memStream.ToArray();
            string fileName = "rents.xlsx";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

        }

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
                    manager.DeleteProductType(item.ProductTypeId, true);
                    count++;
                }
                catch (Exception e)
                {
                    errors++;
                }
            }
            var result = $"количество удаленных записей {count}, ошибок {errors}";
            return Content(result);
        }

        public IActionResult UploadProductsList(Microsoft.AspNetCore.Http.IFormFile formFile)
        {
            if (formFile == null)
            {
                throw new Exception("file not found");
            }
            var manager = new ExcelManager();
            var errors = manager.UploadProductTypes(formFile.OpenReadStream(), formFile.FileName);
            return new JsonResult(errors);
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
            manager.DeleteProductType(id, false);
            return RedirectToAction("ProductList");
        }

        #endregion

        #region nomenklatura
        public IActionResult NomenklaturaList(int productid)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var producttype = manager.Get<ProductType>(p => p.Id == productid);
            if (producttype == null)
                throw new BillingException("producttype not found");
            var list = manager.GetNomenklaturas(productid, 0);
            var model = new NomenklaturaPage();
            model.ProductTypeId = productid;
            model.ProductName = producttype.Name;
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
            manager.DeleteNomenklatura(id, false);
            return RedirectToAction("NomenklaturaList", new { productid });
        }

        #endregion
        #region sku
        public IActionResult SkuList(int nomenklaturaid, int productid)
        {
            var manager = IocContainer.Get<IBillingManager>();
            var producttype = manager.Get<ProductType>(p => p.Id == productid);
            var nomenklatura = manager.Get<Nomenklatura>(n => n.Id == nomenklaturaid);
            if (producttype == null || nomenklatura == null)
                throw new Exception("Номенклатура или продут не найдены");
            var list = manager.GetSkus(0, nomenklaturaid, null);
            var model = new SkuPage();
            model.NomenklaturaId = nomenklaturaid;
            model.NomenklaturaName = nomenklatura.Name;
            model.ProductTypeId = productid;
            model.ProductTypeName = producttype.Name;
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
            return RedirectToAction("SkuList", new { productid = dto.ProductTypeId, nomenklaturaid = dto.NomenklaturaId });
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