using System.Collections.Generic;
using EcaRules;
using UnityEngine;
using Path = System.IO.Path;

public class EcaRuleEngineLoader : MonoBehaviour
{
    private EcaRuleEngine ecaRuleEngine;
    private EcaEventBus ecaEventBus;

    private void Awake()
    {
        ecaRuleEngine = EcaRuleEngine.GetInstance();
        ecaEventBus = EcaEventBus.GetInstance();
        //we're supposing that a GameObject called Player always exists in a scene, the check is for preventing errors
       
        TextRuleParser ruleParser = new TextRuleParser();
        string path = Path.Combine(Application.streamingAssetsPath, "storedRules.txt");
        ruleParser.ReadRuleFile(path);
        foreach (var rule in ecaRuleEngine.Rules())
        {
            Debug.Log(rule);
        }
    }
}