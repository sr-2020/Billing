using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("sin")]
    public class SIN : BaseEntity
    {
        [Column("character")]
        public int CharacterId { get; set; }
        public Character Character { get; set; }
        [ForeignKey("wallet")]
        [Column("wallet")]
        public int? WalletId { get; set; }
        public virtual Wallet Wallet { get; set; }
        [Column("scoring")]
        public int? ScoringId { get; set; }
        public Scoring Scoring { get; set; }
        [Column("work")]
        public int? Work { get; set; }
        [Column("blocked")]
        public bool Blocked { get; set; }
        [Column("eversion")]
        public string EVersion { get; set; }
        [Column("in_game")]
        public bool? InGame { get; set; }
        [Column("metatype_last")]
        [ForeignKey("metatype")]
        public int? OldMetaTypeId { get; set; }
        public Metatype OldMetaType { get; set; }
        [Column("passport")]
        [ForeignKey("passport")]
        public int? PassportId { get; set; }
        public Passport Passport { get; set; }
        [Column("insurance_last")]
        public int? OldInsurance { get; set; }
        [Column("debug_time")]
        public string DebugTime { get; set; }
    }
}
