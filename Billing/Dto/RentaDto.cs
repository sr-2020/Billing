using Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Billing.DTO
{
    public class RentaDto
    {
        public RentaDto(Renta renta)
        {
            HasQRWrite = renta.HasQRWrite;
            PriceId = renta.PriceId;
            RentaId = renta.Id;
            FinalPrice = BillingHelper.GetFinalPrice(renta);
            BasePrice = renta.BasePrice;
            Discount = renta.Discount;
            Scoring = renta.CurrentScoring;
            CharacterName = renta.Sin?.Passport?.PersonName ?? "Unknown";
            ProductType = renta.Sku?.Nomenklatura?.Specialisation?.ProductType?.Name;
            Shop = renta.Shop?.Name;
            NomenklaturaName = renta.Sku?.Nomenklatura?.Name;
            SkuName = renta.Sku?.Name;
            Corporation = renta.Sku?.Corporation?.Name;
            QRRecorded = renta.QRRecorded;
            DateCreated = renta.DateCreated;
            Specialisation = renta.Sku?.Nomenklatura?.Specialisation?.Name;
            Stealable = renta.Stealable;
            Count = renta.Count;
        }
        [Display(Name = "SIN")]
        public string ModelId { get; set; }
        [Display(Name = "Имя персонажа")]
        public string CharacterName { get; set; }
        public int RentaId { get; set; }
        [Display(Name = "Сумма по ренте")]
        public int Count { get; set; }
        public decimal FinalPrice { get; set; }
        public string ProductType { get; set; }
        public string Specialisation { get; set; }
        public string NomenklaturaName { get; set; }
        [Display(Name = "Название Ску")]
        public string SkuName { get; set; }
        public string Corporation { get; set; }
        public string Shop { get; set; }
        public bool HasQRWrite { get; set; }
        [Display(Name = "Записанный QR")]
        public string QRRecorded { get; set; }
        public int PriceId { get; set; }
        [Display(Name = "Дата создания")]
        public DateTime DateCreated { get; set; }
        public decimal BasePrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Scoring { get; set; }
        public bool Stealable { get; set; }
    }

    public class RentaSumDto
    {
        public List<RentaDto> Rentas { get; set; }
        public decimal Sum { get; set; }
    }
}
