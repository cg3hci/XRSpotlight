using System;
using System.IO;
using UnityEngine;

namespace XRSpotlightGUI.Configuration
{
    [Serializable]
    public class Mapping
    {
        public string configuration;
        public Element[] elements;
        public static readonly string MRTK = "Assets/Editor/XRSpotlightGUI/Configuration/ConfigurationScripts/MRTKconfig.json";
        public static readonly string STEAMVR = "Assets/Editor/XRSpotlightGUI/Configuration/ConfigurationScripts/SteamVRconfig.json";
        
        public static Mapping FromFile(string path)
        {
            string json =
                File.ReadAllText(path);
            var mapping =   JsonUtility.FromJson<Mapping>(json);
            mapping.configuration = path;
            return mapping;
        }

        public override bool Equals(object o)
        {
            if (o is Mapping m)
            {
                return m.configuration.Equals(this.configuration);
            }

            return false;
        }
    }

    [Serializable]
    public class Element
    {
        public string className;
        public bool isComponent;
        public string[] definitionNoEvents;
        public Event[] events;
        public EventReference[] eventReferences;
    }

    [Serializable]
    public class EventReference
    {
        public MemberReference[] reference;
    }

    [Serializable]
    public class Event
    {
        public MemberReference[] reference;
        public MemberReference[] subpart;
        public string definition;
        public string[] modality;
        public string defaultAction;
    }

    [Serializable]
    public class MemberReference : ICloneable
    {
        public string member;
        public string name;
        public string type;
        public bool nullByDefault = false;
        public string inspectorProperty;

        public object Clone()
        {
            MemberReference clone = new MemberReference()
            {
                member = this.member,
                name = this.name,
                type = this.type,
                nullByDefault = this.nullByDefault,
                inspectorProperty = this.inspectorProperty
            };
            return clone;
        }
    }
}