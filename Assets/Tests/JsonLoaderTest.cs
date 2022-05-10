using System.Collections;
using System.Net.Mime;
using NUnit.Framework;
using UnityEngine.TestTools;
using EcaRules;
using EcaRules.Json;
using UnityEngine;


public class JsonLoaderTest
{
    private static string path = System.IO.Path.Combine(new string[] {Application.dataPath, "StreamingAssets", "rules.json"});
    
    // A Test behaves as an ordinary method
    [Test]
    public void JsonLoaderTestSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    private JsonEcaRules CreateSampleRules()
    {
        JsonEcaRules rules = new JsonEcaRules
        {
            Rules = new JsonEcaRule[1]
        };
        JsonEcaRule rule = new JsonEcaRule();
        rules.Rules[0] = rule;
        rule.Event = new JsonEcaAction
        {
            Subj = "Player",
            Verb = "interacts with",
            DirObj = "SummerButton",
        };
        
        
        rule.Actions = new JsonEcaAction[5];

        rule.Actions[0] = new JsonEcaAction
        {
            Subj = "Mannequin",
            Verb = "wears",
            DirObj = "TryShirt"
        };
        
        rule.Actions[1] = new JsonEcaAction
        {
            Subj = "Mannequin",
            Verb = "wears",
            DirObj = "SummerHat"
        };
        
        rule.Actions[2] = new JsonEcaAction
        {
            Subj = "Mannequin",
            Verb = "wears",
            DirObj = "SummerPants"
        };
        
        rule.Actions[3] = new JsonEcaAction
        {
            Subj = "SummerLight",
            Verb = "turns",
            DirObj = "on"
        };
        
        rule.Actions[4] = new JsonEcaAction
        {
            Subj = "WinterLight",
            Verb = "turns",
            DirObj = "off"
        };

        return rules;
    }

    [Test]
    public void JsonLoaderCreateRuleFile()
    {
        var rules = CreateSampleRules();
        var serializer = new JsonRuleSerializer();
        serializer.Rules = rules;
        serializer.SaveRules(path);
    }

    [Test]
    public void JsonLoaderDeserializeFile()
    {
        var parser = new JsonRuleParser();
        parser.ReadRuleFile(path);
        Assert.IsTrue(parser.Rules.Equals(CreateSampleRules()));

    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator JsonLoaderTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
