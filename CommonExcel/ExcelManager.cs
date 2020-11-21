using Billing;
using CommonExcel.Model;
using IoC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CommonExcel
{
    public class ExcelManager : ExcelReader
    {
        public List<ExcelError> UploadProductTypes(Stream file, string filename)
        {
            List<ExcelError> errors;
            var excel = this.ParseRows<ProductsExcelModel>(filename, file, out errors);
            if (excel == null)
                throw new Exception("model not found");
            var manager = IocContainer.Get<IBillingManager>();
            
            foreach (var item in excel)
            {
                var productType = manager.GetExtProductType(item.ProductType);
                if(productType == null)
                {
                    productType = manager.CreateOrUpdateProductType(0, item.ProductType, item.ProductDiscountType);
                }
                var nomenklatura = manager.GetExtNomenklatura(item.NomenklaturaName);
                if(nomenklatura == null)
                {
                    nomenklatura = manager.CreateOrUpdateNomenklatura(0, item.NomenklaturaName, item.Code, productType.Id, item.LifeStyle, item.BasePrice, item.Description, string.Empty);
                }
                var sku = manager.GetExtSku(item.SkuName);
                if(sku == null)
                {
                    manager.CreateOrUpdateSku(0, nomenklatura.Id, item.Count, item.Corporation, item.SkuName, item.Enabled == 1 ? true : false);
                }
            }
            return errors;
        }


    }
}
