using Core;
using Core.Primitives;
using IoC;
using Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing
{
    public class BillingBlockAttribute : Attribute
    {
        public BillingBlockAttribute()
        {
            ISettingsManager _settings = IocContainer.Get<ISettingsManager>();
            var block = _settings.GetBoolValue(SystemSettingsEnum.block);
            if (block)
            {
                throw new BillingException("В данный момент ведется пересчет рентных платежей, попробуйте сделать перевод чуть позже");
            }
        }
    }
}
