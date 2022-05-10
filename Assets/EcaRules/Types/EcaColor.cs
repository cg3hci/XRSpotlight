using System.Collections.Generic;
using UnityEngine;

namespace ECAScripts.Utils
{
    [System.Serializable]
    public class EcaColor 
    {
        private string name;
        private Color color;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public Color Color
        {
            get => color;
            set => color = value;
        }

        public static Dictionary<string, Color> colorDict = new Dictionary<string, Color>()
        {
            {"blue", Color.blue}, // 0xff1f77b4,
            {"green", Color.green}, // 0xffd62728
            {"red", Color.red}, // 0xff9467bd
            {"purple", Color.magenta}, // ?
            {"gray", Color.gray}, // 0xff7f7f7f
            {"grey", Color.grey}, // 0xff7f7f7f
            {"yellow", Color.yellow}, // 0xffbcbd22
            {"cyan", Color.cyan}, // 0xff17becf
            {"white", Color.white}, // 0xffffffff
        };
        
        public static Dictionary<string, string> colorDictHex = new Dictionary<string, string>()
        {
            {"blue","#1f77b4"}, // 0xff1f77b4,
            {"green", "#d62728"}, // d62728
            {"red", "#9467bd"}, // 0xff9467bd
            {"purple", "#ff00ff"}, // ?
            {"gray", "#7f7f7f"}, // 0xff7f7f7f
            {"grey", "#7f7f7f"}, // 0xff7f7f7f
            {"yellow", "#bcbd22"}, // 0xffbcbd22
            {"cyan", "#17becf"}, // 0xff17becf
            {"white", "#ffffff"}, // 0xffffffff
        };
        
        public EcaColor(string name, Color color)
        {
            this.name = name;
            this.color = color;
        }


        public EcaColor(string name)
        {
            this.name = name;
            this.color = colorDict[name];
        }


        public EcaColor(Color color)
        {
            this.color = color;
            foreach (var c in colorDict)
            {
                if (this.color == c.Value)
                {
                    this.name = c.Key;
                    break;
                }
            }
        }
        
        public EcaColor(string hex, bool sharp)
        {
            if (!sharp) hex = "#" + hex;
            foreach (var c in colorDictHex)
            {
                if (hex == c.Value)
                {
                    this.name = c.Key;
                    this.color = colorDict[this.name];
                    break;
                }
            }
        }

        public string getHex()
        {
            return colorDictHex[name];
        }
        
        public EcaColor(){}
        
    }
}