using Billing.Dto;
using Core.Model;
using FileHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Billing.DTO
{
    public class SpecialisationDto : ProductTypeDto
    {
        public SpecialisationDto(Specialisation specialisation, bool main) : base(specialisation.ProductType, false)
        {
            if (specialisation == null)
                return;
            if (main)
            {
                Id = specialisation.Id;
                Name = specialisation.Name;
            }
            this.SpecialisationName = specialisation.Name;
            this.SpecialisationId = specialisation.Id;
            this.DiscountType = specialisation.ProductType.DiscountType;
        }
        [DisplayName("Название специализации")]
        public int SpecialisationId { get; set; }
        public string SpecialisationName { get; set; }

    }
}
