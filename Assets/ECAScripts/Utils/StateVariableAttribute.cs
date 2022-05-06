using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateVariableAttribute : System.Attribute
{
    public string Name { set; get; }
    public ECARules4AllType type { get; set; }
    public StateVariableAttribute (string name)
    {
        this.Name = name;
        this.type = ECARules4AllType.Identifier;
    }

    public StateVariableAttribute(string name, ECARules4AllType type)
    {
        this.Name = name;
        this.type = type;
    }
}
