using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Shop
{
    public class UserDto
    {
        public UserDto(SIN sin)
        {
            if (sin == null)
                return;
            Name = sin.Passport?.PersonName;
            if (sin.Character != null)
            {
                Id = sin.Character.Id;
                ModelId = sin.Character.Model.ToString();
            }
            else
            {
                ModelId = sin.Passport?.Sin;
            }
            Balance = sin.Wallet?.Balance ?? 0;
        }
        public int? Id { get; set; }
        public string Name { get; set; }
        public string ModelId { get; set; }
        public decimal Balance { get; set; }
        public string Rights { get; set; }
    }
}
