using Core;
using Core.Model;
using Core.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Settings
{
    public interface ISettingsManager : IBaseRepository
    {
        string GetValue(SystemSettingsEnum key);
        decimal GetDecimalValue(SystemSettingsEnum key);
        int GetIntValue(SystemSettingsEnum key);
        bool GetBoolValue(SystemSettingsEnum key);
        SystemSettings SetValue(SystemSettingsEnum key, string value);

        string GetValue(string key);
        decimal GetDecimalValue(string key);
        int GetIntValue(string key);
        bool GetBoolValue(string key);
        List<SystemSettings> GetAllSettings();
        SystemSettings SetValue(string key, string value);
    }

    public class SettingsManager : BaseEntityRepository, ISettingsManager
    {
        public List<SystemSettings> GetAllSettings()
        {
            return GetList<SystemSettings>(s => true);
        }

        public bool GetBoolValue(SystemSettingsEnum key)
        {
            return GetBoolValue(key.ToString());
        }

        public bool GetBoolValue(string key)
        {
            var result = GetAsNoTracking<SystemSettings>(s => s.Key == key);
            if (result != null)
                return bool.Parse(result.Value);
            throw new Exception($"SystemSetting {key} does not exists");
        }

        public int GetIntValue(SystemSettingsEnum key)
        {
            return GetIntValue(key.ToString());
        }

        public int GetIntValue(string key)
        {
            var result = GetAsNoTracking<SystemSettings>(s => s.Key == key);
            if (result != null)
                return int.Parse(result.Value);
            throw new Exception($"SystemSetting {key} does not exists");
        }
        public string GetValue(SystemSettingsEnum key)
        {
            return GetValue(key.ToString());
        }
        public string GetValue(string key)
        {
            var result = GetAsNoTracking<SystemSettings>(s => s.Key == key);
            if (result != null)
                return result.Value;
            throw new Exception($"SystemSetting {key} does not exists");
        }

        public decimal GetDecimalValue(SystemSettingsEnum key)
        {
            return GetDecimalValue(key.ToString());
        }
        public decimal GetDecimalValue(string key)
        {
            var result = GetAsNoTracking<SystemSettings>(s => s.Key == key);
            if (result != null)
                return decimal.Parse(result.Value);
            throw new Exception($"SystemSetting {key} does not exists");
        }
        public SystemSettings SetValue(SystemSettingsEnum key, string value)
        {
            return SetValue(key.ToString(), value);
        }

        public SystemSettings SetValue(string key, string value)
        {
            var ss = Get<SystemSettings>(s => s.Key == key);
            if (ss == null)
                throw new Exception($"systemSetting {key} not found");
            ss.Value = value;
            SaveContext();
            return ss;
        }

    }
}
