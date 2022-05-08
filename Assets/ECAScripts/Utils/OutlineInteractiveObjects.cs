using System;
using System.Collections.Generic;
using EcaRules;
using UnityEngine;
using Action = System.Action;

namespace ECAScripts.Utils
{
    public class OutlineInteractiveObjects : MonoBehaviour
    {
        private List<EcaRule> interactiveRules = new List<EcaRule>();
        private List<GameObject> interactiveObjects = new List<GameObject>();
        
        void Start()
        {
            findInteractiveRules();
        }
        
        
        public void findInteractiveRules()
        {
            foreach (EcaRule r in EcaRuleEngine.GetInstance().Rules())
            {
                if (isInteractive(r))
                {
                    interactiveRules.Add(r);
                }
            }

            foreach (var gameObject in interactiveObjects)
            {
                outlineColor(gameObject, Color.red);
            }
            
        }

        bool isInteractive(EcaRule r)
        {
            String searchingSubject = "Player";
            String searchingVerb = "interacts with";
            EcaRules.EcaAction eventRule = r.GetEvent();
            if (eventRule.GetSubject().name.Equals(searchingSubject) && eventRule.GetActionMethod().Equals(searchingVerb))
            {
                interactiveObjects.Add((GameObject)eventRule.GetObject());
                return true;
            }

            return false;
        }
        
        public static void outlineColor(GameObject gameObject, Color color)
        {
            ECAOutline ecaOutline = gameObject.GetComponent<ECAOutline>();
            if (ecaOutline == null)
            {
                ecaOutline = gameObject.AddComponent<ECAOutline>();
                ecaOutline.OutlineColor = color;
                ecaOutline.OutlineWidth = 5f;
            }
            else
            {
                ecaOutline.OutlineColor = color;
                ecaOutline.OutlineWidth = 5f;
            }
        }
    }
}