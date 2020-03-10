using Core;
using Core.Model;
using System;
using System.Linq;

namespace Settings
{
    public interface ISettingsManager: IBaseRepository 
    {
        string GetValue(string name);
        decimal GetDecimalValue(string name);
        int GetIntValue(string name);
        //int AddOrUpdate(SystemSettings setting);
        //SystemSettings Delete(int id);
    }

    public class SettingsManager : BaseEntityRepository, ISettingsManager
    {
        public int GetIntValue(string name)
        {
            var result = Get<SystemSettings>(s => s.Key == name);
            if (result != null)
                return int.Parse(result.Value);
            throw new Exception($"systemSetting {name} not found");
        }

        public string GetValue(string name)
        {
            var result = Get<SystemSettings>(s => s.Key == name);
            if(result != null)
                return result.Value;
            return string.Empty;
        }

        public decimal GetDecimalValue(string name)
        {
            var result = Get<SystemSettings>(s => s.Key == name);
            if (result != null)
                return decimal.Parse(result.Value);
            throw new Exception($"systemSetting {name} not found");
        }

        //public int AddOrUpdate(SystemSettings setting)
        //{
        //    if (string.IsNullOrEmpty(setting.Key))
        //        throw new ArgumentNullException("Key не может быть пустым");
        //    var dbSetting = Context.SystemSettings.FirstOrDefault(s => s.Id == setting.Id);
        //    var isExist = Context.SystemSettings.FirstOrDefault(s => s.Key == setting.Key);
        //    if (isExist != null && isExist.Id != setting.Id)
        //        throw new Exception("Setting с этим ключом уже существует");
        //    if (dbSetting != null)
        //    {
        //        dbSetting.Key = setting.Key;
        //        dbSetting.Value = setting.Value;
        //        Context.Update(dbSetting);
        //        setting = dbSetting;
        //    }
        //    else
        //    {
        //        Context.Add(setting);
        //    }
        //    Context.SaveChanges();
        //    return setting.Id;
        //}

        //public SystemSettings Delete(int id)
        //{
        //    var toDelete = Context.SystemSettings.FirstOrDefault(s => s.Id == id);
        //    if (toDelete == null)
        //        return null;
        //    Context.Remove(toDelete);
        //    Context.SaveChanges();
        //    return toDelete;
        //}

    }
}
