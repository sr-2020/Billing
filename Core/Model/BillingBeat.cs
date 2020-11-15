using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("billing_beat")]
    public class BillingBeat : BaseEntity
    {
        [Column("start_time")]
        public DateTime StartTime { get; set; }
        [Column("finish_time")]
        public DateTime FinishTime { get; set; }
        [Column("period")]
        public int Period { get; set; }
        [Column("success_inflation")]
        public bool SuccessInflation { get; set; }
        [Column("success_rent")]
        public bool SuccessRent { get; set; }
        [Column("success_ikar")]
        public bool SuccessIkar { get; set; }
        [Column("success_work")]
        public bool SuccessWork { get; set; }
        [Column("success_scoring")]
        public bool SuccessScoring { get; set; }
    }
}
