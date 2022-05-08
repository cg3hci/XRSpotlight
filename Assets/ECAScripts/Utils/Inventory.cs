using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using EcaRules;


namespace ECAScripts.Utils
{
    public class Inventory : MonoBehaviour
    {
        public List<string> items = new List<string>();

        public void Start()
        {
            var info = new DirectoryInfo(Application.dataPath + "/Resources/Inventory");
            var fileInfo = info.GetFiles();
            foreach (var file in fileInfo)
            {
                //For each file in the directory we'd like to know only if prefabs are available (excluding everything else, including .meta files)
                var test = file.ToString().Split('\\');
                if (test[test.Length - 1].Split('.').Last().Contains("prefab"))
                {
                    items.Add(test[test.Length-1].Split('.')[0]);
                }
            }

            foreach (var element in items)
            {
                //TODO: Istanziare i bottoni da interfaccia e dargli un listener per l'onclick
            }
        }

        public void assignPrefabToPlaceholder(String element, GameObject target)
        {
            //TODO: This snippet of code is in beta phase, it'll be surely updated to a better implementation
            EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"), "activates"), 
                new List<EcaAction>()
                {
                    new EcaAction(target, "changes", "mesh", "to", element)
                }));
            EcaEventBus.GetInstance().Publish(new EcaAction(GameObject.Find("Player"), "activates"));
        }
    }
}