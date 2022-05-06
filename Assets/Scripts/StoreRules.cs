using System.Collections;
using System.Collections.Generic;
using ECARules4All;
using ECARules4All.RuleEngine;
using ECAScripts.Utils;
using UnityEngine;

public class StoreRules : MonoBehaviour
{
	public GameObject set1;
	public GameObject set2;
	// true = set1, false = set2
	public bool whichSet = true;
	
    public ECAObject canvas1;
    public ECAObject dummyTorso;

    public ECALight br1;
    public ECALight br2;
    public ECALight pink1;
    public ECALight pink2;

   // public Sound sound;
    // Start is called before the first frame update
    void Start()
    {
	    
        canvas1.isVisible = ECABoolean.NO;
        dummyTorso.isVisible = ECABoolean.NO;
        br1.@on = ECABoolean.OFF;
        br2.@on = ECABoolean.OFF;
        pink1.@on = ECABoolean.OFF;
        pink2.@on = ECABoolean.OFF;
		
        // Rotation
        Rotation rotation = new Rotation();
        rotation.x = 0;
        rotation.y = 45;
        rotation.z = 0;

        if (whichSet)
        {
	        set2.SetActive(false);
	        set1.SetActive(true);
	        
	        // Canvases
        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("CanvasTrigger1")),
            new List<Action>
            {
                new Action(GameObject.Find("Canvas1"),"changes", "active", "to",ECABoolean.TRUE),
                new Action(GameObject.Find("Canvas1"), "changes", "visible", "to",ECABoolean.YES),
                new Action(GameObject.Find("Canvas2"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas3"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas4"), "changes", "visible", "to",ECABoolean.NO),
				new Action(GameObject.Find("Canvas5"), "changes", "visible", "to",ECABoolean.NO),
				new Action(GameObject.Find("Canvas6"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas7"), "changes", "visible", "to",ECABoolean.NO)


            }));

		RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("CanvasTrigger2")),
            new List<Action>
            {
                new Action(GameObject.Find("Canvas2"),"changes", "active", "to",ECABoolean.TRUE),
                new Action(GameObject.Find("Canvas2"), "changes", "visible", "to",ECABoolean.YES),
                new Action(GameObject.Find("Canvas1"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas3"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas4"), "changes", "visible", "to",ECABoolean.NO),
				new Action(GameObject.Find("Canvas5"), "changes", "visible", "to",ECABoolean.NO),
				new Action(GameObject.Find("Canvas6"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas7"), "changes", "visible", "to",ECABoolean.NO)


            }));

		RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("CanvasTrigger3")),
            new List<Action>
            {
                new Action(GameObject.Find("Canvas3"),"changes", "active", "to",ECABoolean.TRUE),
                new Action(GameObject.Find("Canvas3"), "changes", "visible", "to",ECABoolean.YES),
                new Action(GameObject.Find("Canvas2"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas1"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas4"), "changes", "visible", "to",ECABoolean.NO),
				new Action(GameObject.Find("Canvas5"), "changes", "visible", "to",ECABoolean.NO),
				new Action(GameObject.Find("Canvas6"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas7"), "changes", "visible", "to",ECABoolean.NO)


            }));

		RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("CanvasTrigger4")),
            new List<Action>
            {
                new Action(GameObject.Find("Canvas4"),"changes", "active", "to",ECABoolean.TRUE),
                new Action(GameObject.Find("Canvas4"), "changes", "visible", "to",ECABoolean.YES),
                new Action(GameObject.Find("Canvas2"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas3"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas1"), "changes", "visible", "to",ECABoolean.NO),
				new Action(GameObject.Find("Canvas5"), "changes", "visible", "to",ECABoolean.NO),
				new Action(GameObject.Find("Canvas6"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas7"), "changes", "visible", "to",ECABoolean.NO)


            }));

		RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("CanvasTrigger5")),
            new List<Action>
            {
                new Action(GameObject.Find("Canvas5"),"changes", "active", "to",ECABoolean.TRUE),
                new Action(GameObject.Find("Canvas4"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas2"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas3"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas1"), "changes", "visible", "to",ECABoolean.NO),
				new Action(GameObject.Find("Canvas5"), "changes", "visible", "to",ECABoolean.YES),
				new Action(GameObject.Find("Canvas6"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas7"), "changes", "visible", "to",ECABoolean.NO)


				
            }));

		RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("CanvasTrigger6")),
            new List<Action>
            {
                new Action(GameObject.Find("Canvas6"),"changes", "active", "to",ECABoolean.TRUE),
                new Action(GameObject.Find("Canvas4"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas2"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas3"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas1"), "changes", "visible", "to",ECABoolean.NO),
				new Action(GameObject.Find("Canvas5"), "changes", "visible", "to",ECABoolean.NO),
				new Action(GameObject.Find("Canvas6"), "changes", "visible", "to",ECABoolean.YES),
                new Action(GameObject.Find("Canvas7"), "changes", "visible", "to",ECABoolean.NO)


				
            }));

		
		RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("CanvasTrigger7")),
            new List<Action>
            {
                new Action(GameObject.Find("Canvas7"),"changes", "active", "to",ECABoolean.TRUE),
                new Action(GameObject.Find("Canvas4"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas2"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas3"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas1"), "changes", "visible", "to",ECABoolean.NO),
				new Action(GameObject.Find("Canvas5"), "changes", "visible", "to",ECABoolean.NO),
				new Action(GameObject.Find("Canvas6"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas7"), "changes", "visible", "to",ECABoolean.YES)


				
            }));

		RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
                "interacts with", GameObject.Find("CanvasTrigger8")),
            new List<Action>
            {
                
                new Action(GameObject.Find("Canvas4"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas2"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas3"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas1"), "changes", "visible", "to",ECABoolean.NO),
				new Action(GameObject.Find("Canvas5"), "changes", "visible", "to",ECABoolean.NO),
				new Action(GameObject.Find("Canvas6"), "changes", "visible", "to",ECABoolean.NO),
                new Action(GameObject.Find("Canvas7"), "changes", "visible", "to",ECABoolean.NO)


				
            }));
		
		// Lights change
		RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
				"interacts with", GameObject.Find("ButtonRed")),
			new List<Action>
			{
                
				new Action(GameObject.Find("Light"), "changes", "color", "to","#C53224"),

				
			}));
		RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
				"interacts with", GameObject.Find("ButtonYellow")),
			new List<Action>
			{
                
				new Action(GameObject.Find("Light"), "changes", "color", "to","#FFECB1"),

				
			}));
		
		
		
		RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
				"interacts with", GameObject.Find("ButtonRotate")),
			new List<Action>
			{
                
				new Action(GameObject.Find("RotatingStool"), "rotates around", rotation)

				
			}));


		// COLLEZIONI
		RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
				"interacts with", GameObject.Find("ButtonSummer")),
			new List<Action>
			{
				
				new Action(GameObject.Find("TryDummy"), "wears", GameObject.Find("TryShirt")),
				new Action(GameObject.Find("TryDummy"), "wears", GameObject.Find("SummerHat")),
				new Action(GameObject.Find("TryDummy"), "wears", GameObject.Find("SummerPants"))

			}));
		RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
				"interacts with", GameObject.Find("ButtonWinter")),
			new List<Action>
			{
				
				new Action(GameObject.Find("TryDummy"), "wears", GameObject.Find("WinterShirt")),
				new Action(GameObject.Find("TryDummy"), "wears", GameObject.Find("WinterPants")),
				new Action(GameObject.Find("TryDummy"), "wears", GameObject.Find("TryHat"))
				

			}));
		
		
		//Big tv
		RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"), 
				"interacts with", GameObject.Find("Philips1")),
			new List<Action>
			{
				new Action(GameObject.Find("PlaneVideo"), "plays")


			}));
        }
        else
        {


	        set1.SetActive(false);
	        set2.SetActive(true);


	        // Small Tv's and spotlights
	        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
			        "interacts with", GameObject.Find("TvBrown1")),
		        new List<Action>
		        {
			        new Action(GameObject.Find("Br1Video"), "plays"),
			        new Action(GameObject.Find("Br2Video"), "stops"),
			        new Action(GameObject.Find("Pink1Video"), "stops"),
			        new Action(GameObject.Find("Pink2Video"), "stops"),
			        new Action(GameObject.Find("Shoes1Video"), "stops"),
			        new Action(GameObject.Find("Shoes2Video"), "stops"),
			        new Action(GameObject.Find("Vans1Video"), "stops"),
			        new Action(GameObject.Find("Vans2Video"), "stops"),

			        new Action(GameObject.Find("Br1Light"), "turns", ECABoolean.ON),
			        new Action(GameObject.Find("Br2Light"), "turns", ECABoolean.OFF),
			        new Action(GameObject.Find("Pink1Light"), "turns", ECABoolean.OFF),
			        new Action(GameObject.Find("Pink2Light"), "turns", ECABoolean.OFF),
			        new Action(GameObject.Find("Light"), "turns", ECABoolean.OFF)

		        }));

	        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
			        "interacts with", GameObject.Find("TvBrown2")),
		        new List<Action>
		        {
			        new Action(GameObject.Find("Br2Video"), "plays"),
			        new Action(GameObject.Find("Br1Video"), "stops"),
			        new Action(GameObject.Find("Pink1Video"), "stops"),
			        new Action(GameObject.Find("Pink2Video"), "stops"),
			        new Action(GameObject.Find("Shoes1Video"), "stops"),
			        new Action(GameObject.Find("Shoes2Video"), "stops"),
			        new Action(GameObject.Find("Vans1Video"), "stops"),
			        new Action(GameObject.Find("Vans2Video"), "stops"),

			        new Action(GameObject.Find("Br2Light"), "turns", ECABoolean.ON),
			        new Action(GameObject.Find("Br1Light"), "turns", ECABoolean.OFF),
			        new Action(GameObject.Find("Pink1Light"), "turns", ECABoolean.OFF),
			        new Action(GameObject.Find("Pink2Light"), "turns", ECABoolean.OFF),
			        new Action(GameObject.Find("Light"), "turns", ECABoolean.OFF)


		        }));

	        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
			        "interacts with", GameObject.Find("TvPink1")),
		        new List<Action>
		        {
			        new Action(GameObject.Find("Pink1Video"), "plays"),
			        new Action(GameObject.Find("Br2Video"), "stops"),
			        new Action(GameObject.Find("Br1Video"), "stops"),
			        new Action(GameObject.Find("Pink2Video"), "stops"),
			        new Action(GameObject.Find("Shoes1Video"), "stops"),
			        new Action(GameObject.Find("Shoes2Video"), "stops"),
			        new Action(GameObject.Find("Vans1Video"), "stops"),
			        new Action(GameObject.Find("Vans2Video"), "stops"),

			        new Action(GameObject.Find("Pink1Light"), "turns", ECABoolean.ON),
			        new Action(GameObject.Find("Br2Light"), "turns", ECABoolean.OFF),
			        new Action(GameObject.Find("Br1Light"), "turns", ECABoolean.OFF),
			        new Action(GameObject.Find("Pink2Light"), "turns", ECABoolean.OFF),
			        new Action(GameObject.Find("Light"), "turns", ECABoolean.OFF)


		        }));


	        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
			        "interacts with", GameObject.Find("TvPink2")),
		        new List<Action>
		        {
			        new Action(GameObject.Find("Pink2Video"), "plays"),
			        new Action(GameObject.Find("Br2Video"), "stops"),
			        new Action(GameObject.Find("Pink1Video"), "stops"),
			        new Action(GameObject.Find("Br1Video"), "stops"),
			        new Action(GameObject.Find("Shoes1Video"), "stops"),
			        new Action(GameObject.Find("Shoes2Video"), "stops"),
			        new Action(GameObject.Find("Vans1Video"), "stops"),
			        new Action(GameObject.Find("Vans2Video"), "stops"),

			        new Action(GameObject.Find("Pink2Light"), "turns", ECABoolean.ON),
			        new Action(GameObject.Find("Br2Light"), "turns", ECABoolean.OFF),
			        new Action(GameObject.Find("Pink1Light"), "turns", ECABoolean.OFF),
			        new Action(GameObject.Find("Br1Light"), "turns", ECABoolean.OFF),
			        new Action(GameObject.Find("Light"), "turns", ECABoolean.OFF)


		        }));

	        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
			        "interacts with", GameObject.Find("TvShoes1")),
		        new List<Action>
		        {
			        new Action(GameObject.Find("Shoes1Video"), "plays"),
			        new Action(GameObject.Find("Br2Video"), "stops"),
			        new Action(GameObject.Find("Pink1Video"), "stops"),
			        new Action(GameObject.Find("Pink2Video"), "stops"),
			        new Action(GameObject.Find("Br1Video"), "stops"),
			        new Action(GameObject.Find("Shoes2Video"), "stops"),
			        new Action(GameObject.Find("Vans1Video"), "stops"),
			        new Action(GameObject.Find("Vans2Video"), "stops")


		        }));

	        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
			        "interacts with", GameObject.Find("TvShoes2")),
		        new List<Action>
		        {
			        new Action(GameObject.Find("Shoes2Video"), "plays"),
			        new Action(GameObject.Find("Br2Video"), "stops"),
			        new Action(GameObject.Find("Pink1Video"), "stops"),
			        new Action(GameObject.Find("Pink2Video"), "stops"),
			        new Action(GameObject.Find("Shoes1Video"), "stops"),
			        new Action(GameObject.Find("Br1Video"), "stops"),
			        new Action(GameObject.Find("Vans1Video"), "stops"),
			        new Action(GameObject.Find("Vans2Video"), "stops")


		        }));

	        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
			        "interacts with", GameObject.Find("TvVans1")),
		        new List<Action>
		        {
			        new Action(GameObject.Find("Vans1Video"), "plays"),
			        new Action(GameObject.Find("Br2Video"), "stops"),
			        new Action(GameObject.Find("Pink1Video"), "stops"),
			        new Action(GameObject.Find("Pink2Video"), "stops"),
			        new Action(GameObject.Find("Shoes1Video"), "stops"),
			        new Action(GameObject.Find("Shoes2Video"), "stops"),
			        new Action(GameObject.Find("Br1Video"), "stops"),
			        new Action(GameObject.Find("Vans2Video"), "stops")


		        }));

	        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
			        "interacts with", GameObject.Find("TvVans2")),
		        new List<Action>
		        {
			        new Action(GameObject.Find("Vans2Video"), "plays"),
			        new Action(GameObject.Find("Br2Video"), "stops"),
			        new Action(GameObject.Find("Pink1Video"), "stops"),
			        new Action(GameObject.Find("Pink2Video"), "stops"),
			        new Action(GameObject.Find("Shoes1Video"), "stops"),
			        new Action(GameObject.Find("Shoes2Video"), "stops"),
			        new Action(GameObject.Find("Br1Video"), "stops"),
			        new Action(GameObject.Find("Vans1Video"), "stops")


		        }));

	        // Turn main light on
	        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
			        "interacts with", GameObject.Find("ButtonLight")),
		        new List<Action>
		        {
			        new Action(GameObject.Find("Pink2Video"), "stops"),
			        new Action(GameObject.Find("Br2Video"), "stops"),
			        new Action(GameObject.Find("Pink1Video"), "stops"),
			        new Action(GameObject.Find("Br1Video"), "stops"),
			        new Action(GameObject.Find("Shoes1Video"), "stops"),
			        new Action(GameObject.Find("Shoes2Video"), "stops"),
			        new Action(GameObject.Find("Vans1Video"), "stops"),
			        new Action(GameObject.Find("Vans2Video"), "stops"),

			        new Action(GameObject.Find("Pink2Light"), "turns", ECABoolean.OFF),
			        new Action(GameObject.Find("Br2Light"), "turns", ECABoolean.OFF),
			        new Action(GameObject.Find("Pink1Light"), "turns", ECABoolean.OFF),
			        new Action(GameObject.Find("Br1Light"), "turns", ECABoolean.OFF),
			        new Action(GameObject.Find("Light"), "turns", ECABoolean.ON)


		        }));

	        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
			        "interacts with", GameObject.Find("ButtonRotateDummy")),
		        new List<Action>
		        {

			        new Action(GameObject.Find("DummyRotate"), "rotates around", rotation)


		        }));

	        //Audio
	        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
			        "interacts with", GameObject.Find("Radio")),
		        new SimpleCondition(GameObject.Find("Audio"), "playing", "is", ECABoolean.NO),
		        new List<Action>
		        {

			        new Action(GameObject.Find("Audio"), "plays")


		        }));	        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
			        "interacts with", GameObject.Find("Radio")),
		        new SimpleCondition(GameObject.Find("Audio"), "playing", "is", ECABoolean.YES),
		        new List<Action>
		        {

			        new Action(GameObject.Find("Audio"), "stops")


		        }));
	        
	        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
			        "interacts with", GameObject.Find("VansOldSkool1 (1)")),
		        new List<Action>
		        {
			        new Action(GameObject.Find("infoPanel2"), "shows")
		        }));	        
	        
	        RuleEngine.GetInstance().Add(new Rule(new Action(GameObject.Find("Player"),
			        "stops-interacting with", GameObject.Find("VansOldSkool1 (1)")),
		        new List<Action>
		        {
			        new Action(GameObject.Find("infoPanel2"), "hides")
		        }));


        }
    }
}
