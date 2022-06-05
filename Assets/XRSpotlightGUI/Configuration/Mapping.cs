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
        public string component;
        public string definitionNoEvents;
        
    }

    [Serializable]
    public class Event
    {
        public string name;
        public string definition; 
    }
}