using Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Billing.DTO
{
    public class RentaDto
    {
        [Display(Name = "SIN")]
        public string ModelId { get; set; }
        [Display(Name = "Имя персонажа")]
        public string CharacterName { get; set; }
        public int RentaId { get; set; }
        [Display(Name = "Сумма по ренте")]
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
    }
}
