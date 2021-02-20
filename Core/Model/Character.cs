using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Core.Model
{
    [Table("character")]
    public class Character : BaseEntity
    {
        [Column("model_id")]
        public int Model { get; set; } //main!!! Its real Character
        [Column("game")]
        public int Game { get; set; }
        public virtual List<SIN> Sins { get; set; }

        public SIN GetActualSIN()
        {
            if (Sins == null)
                throw new BillingException("не найдена коллекция sins");
            return Sins.FirstOrDefault();
        }
    }
}
