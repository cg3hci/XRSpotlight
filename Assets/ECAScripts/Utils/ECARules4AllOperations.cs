using System.Collections.Generic;

namespace ECARules4All.RuleEngine
{
    public class ECARules4AllOperations
    {
        //FUTURE: sarebbe più elegante usare un'enumerazione, magari si può considerare una migrazione
        public static Dictionary<ECARules4AllType, string[]> supportedOperations = new Dictionary<ECARules4AllType, string[]>
        {
            {ECARules4AllType.Boolean, new string[]{"changes"}},
            {ECARules4AllType.Color, new string[]{"changes"}},
            {ECARules4AllType.Float, new string[]{"changes", "increases", "decreases"}},
            {ECARules4AllType.Identifier, new string[]{"changes"}},
            {ECARules4AllType.Integer, new string[]{"changes", "increases", "decreases"}},
            {ECARules4AllType.Path, new string[]{"changes"}},
            {ECARules4AllType.Position, new string[]{"changes"}},
            {ECARules4AllType.Rotation, new string[]{"changes"}},
            {ECARules4AllType.Text, new string[]{"changes"}},
            {ECARules4AllType.Time, new string[]{"changes", "increases", "decreases"}},
        };
        
        public static Dictionary<ECARules4AllType, bool> supportsMathematicalConditionChecks = new Dictionary<ECARules4AllType, bool>
        {
            {ECARules4AllType.Boolean, false},
            {ECARules4AllType.Color, false},
            {ECARules4AllType.Float, true},
            {ECARules4AllType.Identifier, false},
            {ECARules4AllType.Integer, true},
            {ECARules4AllType.Path, false},
            {ECARules4AllType.Position, false},
            {ECARules4AllType.Rotation, false},
            {ECARules4AllType.Text, false},
            {ECARules4AllType.Time, true}
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