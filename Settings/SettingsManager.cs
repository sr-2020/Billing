using Core;
using Core.Model;
using System;
using System.Linq;

namespace Settings
{
    public interface ISettingsManager: IBaseRepository 
    {
        string GetValue(string name);
        //int AddOrUpdate(SystemSettings setting);
        //SystemSettings Delete(int id);
    }

    public class SettingsManager : BaseEntityRepository, ISettingsManager
    {
        public string GetValue(string name)
        {
            var result = Get<SystemSettings>(s => s.Key == name);
            if(result != null)
                return result.Value;
            return string.Empty;
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
