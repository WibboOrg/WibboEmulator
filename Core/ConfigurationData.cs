using MySqlX.XDevAPI.Common;

namespace WibboEmulator.Core
{
    public class ConfigurationData
    {
        private Dictionary<string, string> _data;

        public ConfigurationData(string filePath, bool maynotexist = false)
        {
            _data = new Dictionary<string, string>();

            if (!File.Exists(filePath))
            {
                if (!maynotexist)
                    throw new ArgumentException("Unable to locate configuration file at '" + filePath + "'.");
                return;
            }

            try
            {
                using var stream = new StreamReader(filePath);
                string line;

                while ((line = stream.ReadLine()) != null)
                {
                    if (line.Length < 1 || line.StartsWith("#"))
                    {
                        continue;
                    }

                    int delimiterIndex = line.IndexOf('=');

                    if (delimiterIndex != -1)
                    {
                        string key = line.Substring(0, delimiterIndex);
                        string val = line.Substring(delimiterIndex + 1);

                        _data.Add(key, val);
                    }
                }
            }

            catch (Exception e)
            {
                throw new ArgumentException("Could not process configuration file: " + e.Message);
            }
        }

        public bool GetDataBool(string key)
        {
            this._data.TryGetValue(key, out string value);

            return value == "true";
        }

        public string GetDataString(string key)
        {
            this._data.TryGetValue(key, out string value);

            return value;
        }

        public int GetDataNumber(string key)
        {
            this._data.TryGetValue(key, out string value);

            int.TryParse(value, out int result);

            return result;
        }
    }
}