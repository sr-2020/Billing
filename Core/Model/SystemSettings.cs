using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Model
{
    public class SystemSettings : BaseEntity
    {
        [Display(Name ="ключ")]
        public string Key { get; set; }
        [Display(Name = "значение")]
        public string Value { get; set; }
    }
}
