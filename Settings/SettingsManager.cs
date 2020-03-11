using Core;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Settings
{
    public interface ISettingsManager : IBaseRepository
    {
        string GetValue(string name);
        decimal GetDecimalValue(string name);
        int GetIntValue(string name);
        List<SystemSettings> GetAllSettings();
        SystemSettings SetValue(string key, string value);

        //int AddOrUpdate(SystemSettings setting);
        //SystemSettings Delete(int id);
    }

    public class SettingsManager : BaseEntityRepository, ISettingsManager
    {
        public List<SystemSettings> GetAllSettings()
        {
            return GetList<SystemSettings>(s => true);
        }

        public int GetIntValue(string name)
        {
            var result = Get<SystemSettings>(s => s.Key == name);
            if (result != null)
                return int.Parse(result.Value);
            return 0;
        }

        public string GetValue(string name)
        {
            var result = Get<SystemSettings>(s => s.Key == name);
            if (result != null)
                return result.Value;
            return string.Empty;
        }

        public decimal GetDecimalValue(string name)
        {
            var result = Get<SystemSettings>(s => s.Key == name);
            if (result != null)
                return decimal.Parse(result.Value);
            return 0;
        }
        public SystemSettings SetValue(string key, string value)
        {
            var ss = Get<SystemSettings>(s => s.Key == key);
            if (ss == null)
                throw new Exception($"systemSetting {key} not found");
            ss.Value = value;
            Add(ss);
            Context.SaveChanges();
            return ss;
        }

    }
}
