using System.Collections.Generic;
using EcaRules;
using ECAScripts.Utils;
using UnityEngine;

public class StoreDemoRules : MonoBehaviour
{
    Rotation rotation = new Rotation();
    
    private void Start()
    {
        rotation.y = 45;
        /*
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), "interacts with", GameObject.Find("Radio")),
            new SimpleCondition(GameObject.Find("Radio"), "playing", "is", ECABoolean.NO),
            new List<Action>
            {
                new Action(GameObject.Find("Radio"), "plays")
            }));	
        
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), "interacts with", GameObject.Find("Radio")),
            new SimpleCondition(GameObject.Find("Radio"), "playing", "is", ECABoolean.YES),
            new List<Action>
            {
                new Action(GameObject.Find("Radio"), "stops")
            }));
        
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("SummerButton")),
            new List<Action>
            {
				
                new Action(GameObject.Find("Mannequin"), "wears", GameObject.Find("TryShirt")),
                new Action(GameObject.Find("Mannequin"), "wears", GameObject.Find("SummerHat")),
                new Action(GameObject.Find("Mannequin"), "wears", GameObject.Find("SummerPants")),
                new Action(GameObject.Find("SummerLight"), "turns", ECABoolean.ON),
                new Action(GameObject.Find("WinterLight"), "turns", ECABoolean.OFF)
                

            }));
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("WinterButton")),
            new List<Action>
            {
				
                new Action(GameObject.Find("Mannequin"), "wears", GameObject.Find("WinterShirt")),
                new Action(GameObject.Find("Mannequin"), "wears", GameObject.Find("WinterPants")),
                new Action(GameObject.Find("Mannequin"), "wears", GameObject.Find("TryHat")),
                new Action(GameObject.Find("SummerLight"), "turns", ECABoolean.OFF),
                new Action(GameObject.Find("WinterLight"), "turns", ECABoolean.ON)
				

            }));
        
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("SpinButton")),
            new List<Action>
            {
                new Action(GameObject.Find("RotatingStool"), "rotates around", rotation)
            })); 
        
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("BlackShoes")),
            new List<Action>
            {
                new Action(GameObject.Find("infoBlackShoes"), "shows")
            }));
        
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "stops-interacting with", GameObject.Find("BlackShoes")),
            new List<Action>
            {
                new Action(GameObject.Find("infoBlackShoes"), "hides")
            }));
        
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("VanxShoes")),
            new List<Action>
            {
                new Action(GameObject.Find("infoVanxShoes"), "shows")
            }));
        
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "stops-interacting with", GameObject.Find("VanxShoes")),
            new List<Action>
            {
                new Action(GameObject.Find("infoVanxShoes"), "hides")
            }));
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("VanxOld")),
            new List<Action>
            {
                new Action(GameObject.Find("infoVanxOld"), "shows")
            }));
        
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "stops-interacting with", GameObject.Find("VanxOld")),
            new List<Action>
            {
                new Action(GameObject.Find("infoVanxOld"), "hides")
            }));
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("PinkShoesCherry")),
            new List<Action>
            {
                new Action(GameObject.Find("infoPinkShoesCherry"), "shows")
            }));
        
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "stops-interacting with", GameObject.Find("PinkShoesCherry")),
            new List<Action>
            {
                new Action(GameObject.Find("infoPinkShoesCherry"), "hides")
            }));
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("BlackHeelsShoes")),
            new List<Action>
            {
                new Action(GameObject.Find("infoBlackHeelsShoes"), "shows")
            }));
        
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "stops-interacting with", GameObject.Find("BlackHeelsShoes")),
            new List<Action>
            {
                new Action(GameObject.Find("infoBlackHeelsShoes"), "hides")
            }));
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("BlackVanx")),
            new List<Action>
            {
                new Action(GameObject.Find("infoBlackVanx"), "shows")
            }));
        
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "stops-interacting with", GameObject.Find("BlackVanx")),
            new List<Action>
            {
                new Action(GameObject.Find("infoBlackVanx"), "hides")
            }));
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("VanxVintage")),
            new List<Action>
            {
                new Action(GameObject.Find("infoVanxVintage"), "shows")
            }));
        
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "stops-interacting with", GameObject.Find("VanxVintage")),
            new List<Action>
            {
                new Action(GameObject.Find("infoVanxVintage"), "hides")
            }));
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("PinkHeelsShoes")),
            new List<Action>
            {
                new Action(GameObject.Find("infoPinkHeelsShoes"), "shows")
            }));
        
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "stops-interacting with", GameObject.Find("PinkHeelsShoes")),
            new List<Action>
            {
                new Action(GameObject.Find("infoPinkHeelsShoes"), "hides")
            }));
        */
        
    }
}