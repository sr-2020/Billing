using Core.Model;
using InternalServices.EreminModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing
{
    public class ImportDto
    {
        public SIN Sin { get; set; }
        public CharacterModel EreminModel { get; set; }
        public string ErrorText { get; set; }
    }
}
