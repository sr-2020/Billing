using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
    public class Tag : BaseEntity
    {
        public string Value { get; set; }
        public TagType Type { get; set; }
    }
}
