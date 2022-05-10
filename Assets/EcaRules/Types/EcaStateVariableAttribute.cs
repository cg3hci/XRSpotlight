using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EcaStateVariableAttribute : System.Attribute
{
    public string Name { set; get; }
    public EcaRules4AllType type { get; set; }
    public EcaStateVariableAttribute (string name)
    {
        this.Name = name;
        this.type = EcaRules4AllType.Identifier;
    }

    public EcaStateVariableAttribute(string name, EcaRules4AllType type)
    {
        this.Name = name;
        this.type = type;
    }
}
