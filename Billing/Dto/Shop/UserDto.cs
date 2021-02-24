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
            Name = sin.PersonName;
            if (sin.Character != null)
                ModelId = sin.Character.Model.ToString();
            else
            {
                ModelId = sin.Sin;
            }
            Balance = sin.Wallet?.Balance ?? 0;
        }

        public string Name { get; set; }
        public string ModelId { get; set; }
        public decimal Balance { get; set; }
    }
}
