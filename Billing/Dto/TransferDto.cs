using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Billing.DTO
{
    public enum TransferType
    {
        Incoming,
        Outcoming
    }

    public class TransferSum
    {
        public List<TransferDto> Transfers { get; set; }
    }
    public class TransferDto
    {
        public int Id { get; set; }
        [Display(Name = "Овердрафт")]
        public bool Overdraft { get; set; }
        [Display(Name = "Рента(если есть)")]
        public int? RentaId { get; set; }
        public string ModelId { get; set; }
        public string TransferType { get; set; }
        public decimal NewBalance { get; set; }
        [Display(Name = "Комментарий")]
        public string Comment { get; set; }
        [Display(Name ="Сумма")]
        public decimal Amount { get; set; }
        [Display(Name = "Время создания")]
        public DateTime OperationTime { get; set; }
        [Display(Name = "От кого")]
        public string From { get; set; }
        [Display(Name ="Кому")]
        public string To { get; set; }
        [Display(Name = "Anonimous")]
        public bool Anonimous { get; set; }
    }
}
