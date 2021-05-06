using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("passport")]
    public class Passport : BaseEntity
    {
        [Column("sin_text")]
        public string Sin { get; set; }

        [Column("person_name")]
        public string PersonName { get; set; }
        [Column("metatype")]
        [ForeignKey("metatype")]
        public int? MetatypeId { get; set; }
        public Metatype Metatype { get; set; }
        [Column("citizenship")]
        public string Citizenship { get; set; }
        [Column("viza")]
        public string Viza { get; set; }
        [Column("mortgagee")]
        public string Mortgagee { get; set; }

    }
}
