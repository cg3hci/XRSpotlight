using System;
using System.Collections;
using System.Collections.Generic;
using EcaRules;
using ECAScripts.Utils;
using UnityEngine;


public class MuseumRules : MonoBehaviour
{
    
    public ECALight Room1Light;
    public ECALight Room2Light;
    public ECALight Room3Light;
    public ECALight Room4Light;
    public ECAObject canvasZeus;
    public ECAObject canvasDisco;
    public ECAObject canvasBoy;
    public GameObject set1;
    public GameObject set2;
    public bool whichSet = true;

    // Start is called before the first frame update
    //
    void Start()
    {
        // At the beginning the main lights are on
        Room1Light.on = ECABoolean.ON;
        Room2Light.on = ECABoolean.ON;
        Room3Light.on = ECABoolean.ON;
        Room4Light.on = ECABoolean.ON;
        // The plates with the info about the sculptures are not visible
        canvasZeus.isVisible = ECABoolean.NO;
        canvasDisco.isVisible = ECABoolean.NO;
        canvasBoy.isVisible = ECABoolean.NO;

        // List of all the lights in each room
        List<string> room1L = new List<string> {"Room1Light", "R1EcaLight1", "R1EcaLight2", "R1EcaLight3"};
        List<string> room2L = new List<string>{"Room2Light","R2EcaLight1","R2EcaLight2","R2EcaLight3","R2EcaLight4","R2EcaLight5","R2EcaLight6"};
        List<string> room3L = new List<string> {"Room3Light", "HawaiiLight"};
        List<string> room4L = new List<string> {"Room4Light", "R4EcaLight1", "R4EcaLight2", "R4EcaLight3"};

        // Creation of the rules that turn on a single light and off the others when the player interacts with a specific trigger in each room
        // room 1
        turnOneOn("R1LightSwitcher","Room1Light", room1L);
        turnOneOn("R1LightActivator1","R1EcaLight1", room1L);
        turnOneOn("R1LightActivator2","R1EcaLight2", room1L);
        turnOneOn("R1LightActivator3","R1EcaLight3", room1L);
        
        // room 2
        turnOneOn("R2LightSwitcher","Room2Light", room2L);
        turnOneOn("R2LightActivator1","R2EcaLight1", room2L);
        turnOneOn("R2LightActivator2","R2EcaLight2", room2L);
        turnOneOn("R2LightActivator3","R2EcaLight3", room2L);
        turnOneOn("R2LightActivator4","R2EcaLight4", room2L);
        turnOneOn("R2LightActivator5","R2EcaLight5", room2L);
        turnOneOn("R2LightActivator6","R2EcaLight6", room2L);
        
        // room 3
        turnOneOn("R3LightSwitcher","Room3Light", room3L);

        // room 4
        turnOneOn("R4LightSwitcher","Room4Light", room4L);
        turnOneOn("R4LightActivator1","R4EcaLight1", room4L);
        turnOneOn("R4LightActivator2","R4EcaLight2", room4L);
        turnOneOn("R4LightActivator3","R4EcaLight3", room4L);
        
        // Rules for the Canvases
        // Appears when the player approaches Zeus' sculpture (uses its light activator as trigger)
        EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("R1LightActivator2")),
            new List<EcaAction>
            {
                new EcaAction(GameObject.Find("CanvasZeus"), "changes", "visible", "to",ECABoolean.YES)
            }));
        
        // Same but with the Discobolus' sculpture
        EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("R1LightActivator1")),
            new List<EcaAction>
            {
                new EcaAction(GameObject.Find("CanvasDisco"), "changes", "visible", "to",ECABoolean.YES)
            }));
        
        // Same but with the Boy's sculpture
        EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("R1LightActivator3")),
            new List<EcaAction>
            {
                new EcaAction(GameObject.Find("CanvasBoy"), "changes", "visible", "to",ECABoolean.YES)
            }));
        
        // All canvases when the player walks away and turns the main light on (uses the room's main light switcher as trigger)
        EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"), "interacts with",
                GameObject.Find("R1LightSwitcher")),
            new List<EcaAction>
            {
                new EcaAction(GameObject.Find("CanvasZeus"), "changes", "visible", "to",ECABoolean.NO),
                new EcaAction(GameObject.Find("CanvasBoy"), "changes", "visible", "to",ECABoolean.NO),
                new EcaAction(GameObject.Find("CanvasDisco"), "changes", "visible", "to",ECABoolean.NO)
            }));
        
        
        // Same but with the Discobolus' sculpture
        EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("R2LightActivator1")),
            new List<EcaAction>
            {
                new EcaAction(GameObject.Find("CanvasPortrait1"), "changes", "visible", "to",ECABoolean.YES)
            }));
        
        
        // Same but with the Discobolus' sculpture
        EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("R2LightActivator2")),
            new List<EcaAction>
            {
                new EcaAction(GameObject.Find("CanvasPortrait2"), "changes", "visible", "to",ECABoolean.YES)
            }));
            
        // Same but with the Discobolus' sculpture
        EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("R2LightActivator3")),
            new List<EcaAction>
            {
                new EcaAction(GameObject.Find("CanvasPortrait3"), "changes", "visible", "to",ECABoolean.YES)
            }));

            // Same but with the Discobolus' sculpture
            EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"),
                    "interacts with", GameObject.Find("R2LightActivator4")),
                new List<EcaAction>
                {
                    new EcaAction(GameObject.Find("CanvasPortrait4"), "changes", "visible", "to",ECABoolean.YES)
                }));
            
            // Same but with the Discobolus' sculpture
            EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"), 
                    "interacts with", GameObject.Find("R2LightActivator5")),
                new List<EcaAction>
                {
                    new EcaAction(GameObject.Find("CanvasPortrait5"), "changes", "visible", "to",ECABoolean.YES)
                }));
            
            // Same but with the Discobolus' sculpture
            EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"), 
                    "interacts with", GameObject.Find("R2LightActivator6")),
                new List<EcaAction>
                {
                    new EcaAction(GameObject.Find("CanvasPortrait6"), "changes", "visible", "to",ECABoolean.YES)
                }));
            
            // All canvases when the player walks away and turns the main light on (uses the room's main light switcher as trigger)
            EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"), "interacts with",
                    GameObject.Find("R2LightSwitcher")),
                new List<EcaAction>
                {
                    new EcaAction(GameObject.Find("CanvasPortrait1"), "changes", "visible", "to",ECABoolean.NO),
                    new EcaAction(GameObject.Find("CanvasPortrait2"), "changes", "visible", "to",ECABoolean.NO),
                    new EcaAction(GameObject.Find("CanvasPortrait3"), "changes", "visible", "to",ECABoolean.NO),
                    new EcaAction(GameObject.Find("CanvasPortrait4"), "changes", "visible", "to",ECABoolean.NO),
                    new EcaAction(GameObject.Find("CanvasPortrait5"), "changes", "visible", "to",ECABoolean.NO),
                    new EcaAction(GameObject.Find("CanvasPortrait6"), "changes", "visible", "to",ECABoolean.NO)
                }));


            // Rules for the Canvases
            // Appears when the player approaches Zeus' sculpture (uses its light activator as trigger)
            EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"), 
                    "interacts with", GameObject.Find("R4LightActivator1")),
                new List<EcaAction>
                {
                    new EcaAction(GameObject.Find("CanvasAncient1"), "changes", "visible", "to",ECABoolean.YES)
                }));
        
            // Same but with the Discobolus' sculpture
            EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"), 
                    "interacts with", GameObject.Find("R4LightActivator2")),
                new List<EcaAction>
                {
                    new EcaAction(GameObject.Find("CanvasAncient2"), "changes", "visible", "to",ECABoolean.YES)
                }));
        
            // Same but with the Boy's sculpture
            EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"), 
                    "interacts with", GameObject.Find("R4LightActivator3")),
                new List<EcaAction>
                {
                    new EcaAction(GameObject.Find("CanvasAncient3"), "changes", "visible", "to",ECABoolean.YES)
                }));
        
            // All canvases when the player walks away and turns the main light on (uses the room's main light switcher as trigger)
            EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"), "interacts with",
                    GameObject.Find("R4LightSwitcher")),
                new List<EcaAction>
                {
                    new EcaAction(GameObject.Find("CanvasAncient1"), "changes", "visible", "to",ECABoolean.NO),
                    new EcaAction(GameObject.Find("CanvasAncient2"), "changes", "visible", "to",ECABoolean.NO),
                    new EcaAction(GameObject.Find("CanvasAncient3"), "changes", "visible", "to",ECABoolean.NO)
                }));


            if (whichSet)
            {
                set1.SetActive(true);
                set2.SetActive(false);
                
                EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"),
                        "interacts with", GameObject.Find("HawaiiTrigger1")),
                    new List<EcaAction>
                    {
                        new EcaAction(GameObject.Find("HawaiiVideo"), "changes", "visible", "to", ECABoolean.YES),
                        new EcaAction(GameObject.Find("HawaiiVideo"), "changes", "source", "to", "Assets/Prefabs/TemplateStuff/FBX/Museum/Video/Hawaii1.mp4"),
                        new EcaAction(GameObject.Find("HawaiiVideo"), "plays"),

                    }));
                EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"),
                        "interacts with", GameObject.Find("HawaiiTrigger2")),
                    new List<EcaAction>
                    {
                        new EcaAction(GameObject.Find("HawaiiVideo"), "changes", "visible", "to", ECABoolean.YES),
                        new EcaAction(GameObject.Find("HawaiiVideo"), "changes", "source", "to", "Assets/Prefabs/TemplateStuff/FBX/Museum/Video/Hawaii2.mp4"),
                        new EcaAction(GameObject.Find("HawaiiVideo"), "plays"),

                    }));
                
                EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"),
                        "interacts with", GameObject.Find("Fence")),
                    new SimpleEcaCondition(GameObject.Find("360Sphere"), "playing", "is", ECABoolean.NO),
                    new List<EcaAction>
                    {

                        new EcaAction(GameObject.Find("360Sphere"), "shows"),
                        new EcaAction(GameObject.Find("360Sphere"), "plays")


                    }));	        
                EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"),
                        "interacts with", GameObject.Find("Fence")),
                    new SimpleEcaCondition(GameObject.Find("360Sphere"), "playing", "is", ECABoolean.YES),
                    new List<EcaAction>
                    {

                        new EcaAction(GameObject.Find("360Sphere"), "hides"),
                        new EcaAction(GameObject.Find("360Sphere"), "stops")


                    }));
            }
            else
            {
                set2.SetActive(true);
                set1.SetActive(false);
                
                //Audio
            EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"),
                    "interacts with", GameObject.Find("HawaiiAudio")),
                new List<EcaAction>
                {

                    new EcaAction(GameObject.Find("Audio"), "changes", "source", "to", "Assets/Prefabs/TemplateStuff/FBX/Museum/Audio/HawaiiMusic.mp3"),
                    new EcaAction(GameObject.Find("Audio"), "plays"),


                }));
            
            EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"),
                    "interacts with", GameObject.Find("ChinaAudio")),
                new List<EcaAction>
                {

                    new EcaAction(GameObject.Find("Audio"), "changes", "source", "to", "Assets/Prefabs/TemplateStuff/FBX/Museum/Audio/ChinaMusic.mp3"),
                    new EcaAction(GameObject.Find("Audio"), "plays"),


                }));
            
            EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"),
                    "interacts with", GameObject.Find("EgyptAudio")),
                new List<EcaAction>
                {

                    new EcaAction(GameObject.Find("Audio"), "changes", "source", "to", "Assets/Prefabs/TemplateStuff/FBX/Museum/Audio/EgyptMusic.mp3"),
                    new EcaAction(GameObject.Find("Audio"), "plays"),


                }));
            
            EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"),
                    "interacts with", GameObject.Find("PlayGreek")),
                new List<EcaAction>
                {
                    
                    new EcaAction(GameObject.Find("GreekAudio"), "plays"),


                }));
            
            EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"),
                    "interacts with", GameObject.Find("StopGreek")),
                new List<EcaAction>
                {
                    
                    new EcaAction(GameObject.Find("GreekAudio"), "stops"),


                }));
            
            EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"),
                    "interacts with", GameObject.Find("PlayClassical")),
                new List<EcaAction>
                {
                    
                    new EcaAction(GameObject.Find("ClassicalAudio"), "plays"),


                }));
            
            EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"),
                    "interacts with", GameObject.Find("StopClassical")),
                new List<EcaAction>
                {
                    
                    new EcaAction(GameObject.Find("ClassicalAudio"), "stops"),


                }));
            
            EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"),
                    "interacts with", GameObject.Find("PlayPersian")),
                new List<EcaAction>
                {
                    
                    new EcaAction(GameObject.Find("PersianAudio"), "plays"),


                }));
            
            EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"),
                    "interacts with", GameObject.Find("StopPersian")),
                new List<EcaAction>
                {
                    
                    new EcaAction(GameObject.Find("PersianAudio"), "stops"),


                }));
            

            
            
            
            // Change painting
            /*
             RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
                    "interacts with", GameObject.Find("change1")),
                new List<Action>
                {
                    
                    new Action(GameObject.Find("Flower"), "changes", "mesh", "to", "prova"),


                }));
                
             RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
                     "interacts with", GameObject.Find("change2")),
                 new List<Action>
                 {
                    
                     new Action(GameObject.Find("AbstPig"), "changes", "mesh", "to", "prova"),


                 }));
             
             RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
                     "interacts with", GameObject.Find("change3")),
                 new List<Action>
                 {
                    
                     new Action(GameObject.Find("fbxPaint"), "changes", "mesh", "to", "prova"),


                 }));
             
             RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
                     "interacts with", GameObject.Find("change4")),
                 new List<Action>
                 {
                    
                     new Action(GameObject.Find("AbstSunset"), "changes", "mesh", "to", "prova"),


                 }));
             
             RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
                     "interacts with", GameObject.Find("change5")),
                 new List<Action>
                 {
                    
                     new Action(GameObject.Find("AbstPoint"), "changes", "mesh", "to", "prova"),


                 }));
                
             RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
                     "interacts with", GameObject.Find("change1")),
                 new List<Action>
                 {
                    
                     new Action(GameObject.Find("WomanSunset"), "changes", "mesh", "to", "prova"),


                 }));
            */
            }

            
    }

    // This method is used to create the rules that turn on a light and turn off all the other lights in a given room
    public void turnOneOn(string lightTrigger, string lightOn, List<string> lightsOff)
    {
        // Action list needed to add a new rule
        List<EcaAction> l = new List<EcaAction>();
        // Turns on the only light that was specified
        l.Add(new EcaAction(GameObject.Find(lightOn), "turns", ECABoolean.ON));
        
        // Turns off every other light in the room
        foreach(string s in lightsOff)
        {
            if(s != lightOn) 
                l.Add(new EcaAction(GameObject.Find(s), "turns", ECABoolean.OFF));
        }
        
        // Creates the rule
        EcaRuleEngine.GetInstance().Add(new EcaRule(new EcaAction(GameObject.Find("Player"),
                "interacts with", GameObject.Find(lightTrigger)), l ));
    }
    
    
}
