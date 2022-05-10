using System.Collections.Generic;

namespace EcaRules
{
    public class EcaRules4AllOperations
    {
        //FUTURE: sarebbe più elegante usare un'enumerazione, magari si può considerare una migrazione
        public static Dictionary<EcaRules4AllType, string[]> supportedOperations = new Dictionary<EcaRules4AllType, string[]>
        {
            {EcaRules4AllType.Boolean, new string[]{"changes"}},
            {EcaRules4AllType.Color, new string[]{"changes"}},
            {EcaRules4AllType.Float, new string[]{"changes", "increases", "decreases"}},
            {EcaRules4AllType.Identifier, new string[]{"changes"}},
            {EcaRules4AllType.Integer, new string[]{"changes", "increases", "decreases"}},
            {EcaRules4AllType.Path, new string[]{"changes"}},
            {EcaRules4AllType.Position, new string[]{"changes"}},
            {EcaRules4AllType.Rotation, new string[]{"changes"}},
            {EcaRules4AllType.Text, new string[]{"changes"}},
            {EcaRules4AllType.Time, new string[]{"changes", "increases", "decreases"}},
        };
        
        public static Dictionary<EcaRules4AllType, bool> supportsMathematicalConditionChecks = new Dictionary<EcaRules4AllType, bool>
        {
            {EcaRules4AllType.Boolean, false},
            {EcaRules4AllType.Color, false},
            {EcaRules4AllType.Float, true},
            {EcaRules4AllType.Identifier, false},
            {EcaRules4AllType.Integer, true},
            {EcaRules4AllType.Path, false},
            {EcaRules4AllType.Position, false},
            {EcaRules4AllType.Rotation, false},
            {EcaRules4AllType.Text, false},
            {EcaRules4AllType.Time, true}
        };
        
        
        public static Dictionary<string, string> operationAliases = new Dictionary<string, string>
        {
            {"changes", "changes"},
            {"increases", "increases"},
            {"decreases", "decreases"},
            //variabili di test, per provare verbi diversi per fare la stessa operazione
            {"swaps", "changes"},
            {"sets", "changes"},
            {"adds", "increases"},
            {"subtracts", "decreases"}
        };
    }
}