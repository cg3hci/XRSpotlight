using System.Collections.Generic;
using ECARules4All.RuleEngine;
using UnityEngine;

public class RuleEngineLoader : MonoBehaviour
{
    private RuleEngine ruleEngine;
    private EventBus eventBus;

    private void Start()
    {
        ruleEngine = RuleEngine.GetInstance();
        eventBus = EventBus.GetInstance();
        //we're supposing that a GameObject called Player always exists in a scene, the check is for preventing errors
        // if (GameObject.Find("Player") != null)
        // {
        //     TextRuleParser ruleParser = new TextRuleParser();
        //     // ruleParser.ReadRuleFile(Application.dataPath + "\\storedRules.txt");
        //     //eventBus.Publish(new Action(GameObject.Find("Player"), "teleports to", GameObject.Find(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)));
        // }
        // foreach (var r in ruleEngine.Rules())
        // {
        //     Debug.Log(r);
        // }
    }
}