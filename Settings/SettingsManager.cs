using Core;
using Core.Model;
using System;
using System.Linq;

namespace Settings
{
    public class SettingsManager
    {
        IUnitOfWork unitOfWork;
        public SettingsManager(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public int AddOrUpdate(SystemSettings setting)
        {
            if (string.IsNullOrEmpty(setting.Key))
                throw new ArgumentNullException("Key не может быть пустым");
            var dbSetting = unitOfWork.BillingContext.SystemSettings.FirstOrDefault(s => s.Id == setting.Id);
            var isExist = unitOfWork.BillingContext.SystemSettings.FirstOrDefault(s => s.Key == setting.Key);
            if (isExist != null && isExist.Id != setting.Id)
                throw new Exception("Setting с этим ключом уже существует");
            if (dbSetting != null)
            {
                dbSetting.Key = setting.Key;
                dbSetting.Value = setting.Value;
                unitOfWork.BillingContext.Update(dbSetting);
                setting = dbSetting;
            }
            else
            {
                unitOfWork.BillingContext.Add(setting);
            }
            unitOfWork.BillingContext.SaveChanges();
            return setting.Id;
        }

        public SystemSettings Delete(int id)
        {
            var toDelete = unitOfWork.BillingContext.SystemSettings.FirstOrDefault(s => s.Id == id);
            if (toDelete == null)
                return null;
            unitOfWork.BillingContext.Remove(toDelete);
            unitOfWork.BillingContext.SaveChanges();
            return toDelete;
        }

    }
}
