using System.Collections;
using System.Collections.Generic;
using EcaRules;
using UnityEngine;
using ECAScripts.Utils;

public class RuleList : MonoBehaviour
{
    // Start is called before the first frame update 
    void Start()
    { 
        /*
        RuleEngine.GetInstance().Add(new Rule(
            new Action(GameObject.Find("Player"),"interacts with", GameObject.Find("Visible")),
            //Esempio di Condizioni composte
            new CompositeCondition(CompositeCondition.ConditionType.OR, new List<Condition>
            {
                new SimpleCondition(GameObject.Find("Sphere"), "visible", "is", true),
                new SimpleCondition(GameObject.Find("Sphere"), "active", "is", false),
                new CompositeCondition(CompositeCondition.ConditionType.OR, new List<Condition>
                {
                    new SimpleCondition(GameObject.Find("Sphere"), "visible", "is", true),
                    new SimpleCondition(GameObject.Find("Sphere"), "active", "is", false)
                })
            }),
            new List<Action>
            {
                //It's called Uman because it's a poetic license, not a grammar error :)
                //Esempio (commentato) di spostamento verso posizione singola
                //new Action(GameObject.Find("Uman"), "jumps to", new Position(-3.5f, 1.62f, -2.42f)),
                //Esempio di spostamento tramite Path (collezione di Position)
                new Action(GameObject.Find("Uman"), "jumps on", new Path(new List<Position>()
                {
                    new Position(-3.5f, 1.62f, -2.42f),  new Position(-3.6f, 1.62f, 2.42f), new Position(3.42f, 1.62f, 2.42f), new Position(3.42f, 1.62f, -2.42f)  
                })),
                new Action(GameObject.Find("Sword"), "stabs", GameObject.Find("Uman")),
                new Action(GameObject.Find("Sword"), "fires", GameObject.Find("Uman")),
                new Action(GameObject.Find("pandolfo"), "wears", GameObject.Find("armani")),
                new Action(GameObject.Find("People"), "wears", GameObject.Find("hatto")),
                new Action(GameObject.Find("People"), "wears", GameObject.Find("maglietta")),
                new Action(GameObject.Find("People"), "wears", GameObject.Find("panta")),
                new Action(GameObject.Find("People"), "wears", GameObject.Find("scarpe")),
            }
        ));
        
        //Esempio di regola senza condizioni
        RuleEngine.GetInstance().Add(new Rule(
            new Action(GameObject.Find("Player"),"interacts with", GameObject.Find("Visible")),
            new List<Action>
            {
                new Action(GameObject.Find("Sphere"), "changes", "visible", "to", ECABoolean.YES),
                new Action(GameObject.Find("sput"), "turns", ECABoolean.ON),
                new Action(GameObject.Find("Cylinder"), "looks at", GameObject.Find("Sphere")),
                new Action(GameObject.Find("Cannas"), "changes", "visible", "to", ECABoolean.YES),

            }
        ));
        
        RuleEngine.GetInstance().Add(new Rule(
            new Action(GameObject.Find("Player"),"interacts with", GameObject.Find("Active")),
            //Esempio di condizione singola
            new SimpleCondition(GameObject.Find("Sphere"), "active", "is", false),
        new List<Action>
            {
                new Action(GameObject.Find("Sphere"), "changes", "active", "to", ECABoolean.YES),
            }
        ));
        
        RuleEngine.GetInstance().Add(new Rule(
            new Action(GameObject.Find("Player"),"interacts with", GameObject.Find("Invisible")),
            new CompositeCondition(CompositeCondition.ConditionType.OR, new List<Condition>
            {
                new SimpleCondition(GameObject.Find("Sphere"), "visible", "is", ECABoolean.YES)
            }),
            new List<Action>
            {
                new Action(GameObject.Find("Cannas"), "changes", "visible", "to", ECABoolean.NO),

                new Action(GameObject.Find("Sphere"), "changes" ,  "visible", "to", ECABoolean.NO),
            }
        ));
        
        RuleEngine.GetInstance().Add(new Rule(
            new Action(GameObject.Find("Player"),"changes", "color", "to", new Color()),
            new CompositeCondition(CompositeCondition.ConditionType.OR, new List<Condition>
            {
                new SimpleCondition(GameObject.Find("Sphere"), "visible", "is", ECABoolean.YES)
            }),
            new List<Action>
            {
                new Action(GameObject.Find("Sphere"), "changes" ,  "visible", "to", ECABoolean.NO),
            }
        ));

        RuleEngine.GetInstance().Add(new Rule(
            new Action(GameObject.Find("Player"), "interacts with", GameObject.Find("Inactive")),
            new List<Action>
            {
                //Esempio (commentato) di possibile utilizzo di verbo diverso per il cambio variabile
                //Riferitevi a ECAScripts/Utils/ECARules4AllOperations.operationAliases per una lista di verbi, e cosa significano per il RuleEngine
                //new Action(GameObject.Find("Sphere"), "swaps", "active", false),
                new Action(GameObject.Find("Sphere"), "changes", "active", "to", ECABoolean.NO),

            }
        ));*/
        
        /* Questa Publish serve per avviare eventuali cambi di Placeholder, nel caso ce ne dovessero essere*/
        //EventBus.GetInstance().Publish(new Action(GameObject.Find("Player"), "activates"));
        
        /*when    the character visitor presses the button bigButton1
         then    the furniture stool rotates by 30 degrees around Y 

        when    the character visitor interacts with the furniture table1 
        then    the light mainLight turns off 
        the light tableLight1 turns on 
        the video tableAdvertisement1 plays 

        when    the visitor interacts with the audio radio 
        if      the audio radio playing is false 
        then    the audio radio plays */

        /*Rotation rot = new Rotation();
        rot.y = 30.0f;
        RuleEngine.GetInstance().Add(new Rule(
            new Action(GameObject.Find("visitor"),"pushes", GameObject.Find("bigButton1")),
            new List<Action>
            {
                new Action(GameObject.Find("bigMannequin"), "shows")
            }
        ));*/
        
        /*RuleEngine.GetInstance().Add(new Rule(
            new Action(GameObject.Find("visitor"),"interacts with", GameObject.Find("table1")),
            new List<Action>
            {
                new Action(GameObject.Find("mainLight"), "turns", ECABoolean.OFF),
                new Action(GameObject.Find("tableLight1"), "turns", ECABoolean.ON),
                new Action(GameObject.Find("tableAdvertisement1"), "plays"),
                
            }
        ));
        
        RuleEngine.GetInstance().Add(new Rule(
            new Action(GameObject.Find("visitor"),"interacts with", GameObject.Find("radio")),
            new SimpleCondition(GameObject.Find("radio"), "playing", "is", false),
            new List<Action>
            {
                new Action(GameObject.Find("radio"), "plays")
            }
        ));*/
        
        
        /*
        //Save the rules into the txt file 
        TextRuleSerializer serializer = new TextRuleSerializer();
        serializer.SaveRules(Application.dataPath + "/storedRules.txt");*/
    }
}
