using Core.Model;
using FileHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Billing.DTO
{
    public class SpecialisationDto
    {
        public SpecialisationDto(Specialisation specialisation)
        {
            if (specialisation == null)
                return;
            this.SpecialisationName = specialisation.Name;
            this.SpecialisationId = specialisation.Id;
            if (specialisation?.ProductType == null)
                return;
            this.Id = specialisation.ProductType.Id;
            this.Name = specialisation.ProductType.Name;
            this.DiscountType = specialisation.ProductType.DiscountType;
        }
        public SpecialisationDto() { }
        public int Id { get; set; }
        [Column(0, false)]
        public string Name { get; set; }

        [DisplayName("тип скидки(1 или 2)")]
        [Column(1, false)]
        public int DiscountType { get; set; }
        [DisplayName("Название специализации")]
        [Column(2, false)]
        public int SpecialisationId { get; set; }
        public string SpecialisationName { get; set; }
    }
}
