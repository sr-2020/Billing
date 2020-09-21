using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Model
{
    [Table("user_login")]
    public class UserLogin : BaseEntity
    {
        [Column("login")]
        public string Login { get; set; }

    }
}
