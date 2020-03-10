using System;
using System.Collections.Generic;
using System.Text;

namespace InternalServices.EreminModel
{
    public class CharacterModel
    {
        public BaseModel baseModel { get; set; }
        public WorkModel workModel { get; set; }
    }

    public class BaseModel
    {
        public string modelId { get; set; }
        public string discountWeaponsArmor { get; set; }
        public string discountDrones { get; set; }
        public string discountChemo { get; set; }
        public string discountImplants { get; set; }
        public string discountMagicStuff { get; set; }
    }

    public class WorkModel
    {
        public string modelId { get; set; }
        public string discountWeaponsArmor { get; set; }
        public string discountDrones { get; set; }
        public string discountChemo { get; set; }
        public string discountImplants { get; set; }
        public string discountMagicStuff { get; set; }
    }

}
