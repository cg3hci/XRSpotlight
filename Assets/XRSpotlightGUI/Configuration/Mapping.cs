using System;

namespace XRSpotlightGUI.Configuration
{
    [Serializable]
    public class Mapping
    {
        public string configuration;
        public Element[] elements;
    }

    [Serializable]
    public class Element
    {
        public string className;
        public bool isComponent;
        public string definitionNoEvents;
        public Event[] events;
        public MemberReference[] eventReferences;
    }

    [Serializable]
    public class Event
    {
        public MemberReference[] reference;
        public string definition;
        public string[] modality;
    }

    [Serializable]
    public class MemberReference
    {
        public string member;
        public string name;
        public string type;
    }
}