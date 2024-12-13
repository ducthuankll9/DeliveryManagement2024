using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DeliveryManagement.Helper
{
    public class InitFileReader
    {
        private readonly Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();

        public InitFileReader(string filePath) {
            string currentSection = null;

            foreach (var line in File.ReadLines(filePath))
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = line.Substring(1, line.Length - 2);
                    data[currentSection] = new Dictionary<string, string>();
                }
                else if (!string.IsNullOrWhiteSpace(line) && currentSection != null)
                {
                    var parts = line.Split(new char[] { '=' }, 2);
                    if (parts.Length == 2)
                    {
                        data[currentSection][parts[0].Trim()] = parts[1].Trim();
                    }
                }
            }
        }

        public string GetStringValue(string section, string key)
        {
            if (data.ContainsKey(section) && data[section].ContainsKey(key))
            {
                return data[section][key];
            }
            return null;
        }

        public float GetFloatValue(string section, string key)
        {
            if (data.ContainsKey(section) && data[section].ContainsKey(key))
            {
                if (float.TryParse(data[section][key], out float result))
                {
                    return result;
                }
            }
            return 0.0f; // Default value if cannot parse
        }
    }
}