using System.Globalization;
using System.IO;
using UnityEngine;

namespace EcaRules.Json
{
    public class JsonRuleSerializer: IEcaRuleSerializer
    {
        public JsonEcaRules Rules { get; set; }

        public void SaveRules(string path)
        {
            if (Rules == null) return;
            var json = JsonUtility.ToJson(Rules, true);
            
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            using var writer = File.CreateText(path);
            writer.Write(json);
            writer.Flush();
        }
    }
}