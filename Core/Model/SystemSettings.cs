using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
    public class SystemSettings: BaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
