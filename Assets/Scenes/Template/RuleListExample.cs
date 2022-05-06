using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECARules4All.RuleEngine;

public class RuleListExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RuleEngine.GetInstance().Add(new Rule(
            new Action(GameObject.Find("Player"),"interacts with", GameObject.Find("Sphere")),
            new List<Action>
            {
                new Action(GameObject.Find("Player"), "starts animation", "WalkBack"),
            }
        ));
       /* RuleEngine.GetInstance().Add(new Rule(
            new Action(GameObject.Find("Player"),"interacts with", GameObject.Find("Visible")),
            new List<Action>
            {
                new Action(GameObject.Find("Sphere"), "changes", "visible", true),
            }
        ));
        
        RuleEngine.GetInstance().Add(new Rule(
            new Action(GameObject.Find("Player"),"interacts with", GameObject.Find("Invisible")),
            new List<Action>
            {
                new Action(GameObject.Find("Sphere"), "changes", "visible", false),

            }
        ));
        
        RuleEngine.GetInstance().Add(new Rule(
            new Action(GameObject.Find("Player"),"interacts with", GameObject.Find("Active")),
            new List<Action>
            {
                new Action(GameObject.Find("Sphere"), "changes", "active", true),
            }
        ));
        
        RuleEngine.GetInstance().Add(new Rule(
            new Action(GameObject.Find("Player"),"interacts with", GameObject.Find("Inactive")),
            new List<Action>
            {
                new Action(GameObject.Find("Sphere"), "changes", "active", false),

            }
        ));

        //This rule is for a Behaviour instead of a ECAObject, it works the same way
        RuleEngine.GetInstance().Add(new Rule(
            new Action(GameObject.Find("Player"),"interacts with", GameObject.Find("Sphere")),
            new Condition(GameObject.Find("Sphere"), "on", "is", false),
            new List<Action>
            {
                new Action(GameObject.Find("Sphere"), "turns", true),

            }
        ));   */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
