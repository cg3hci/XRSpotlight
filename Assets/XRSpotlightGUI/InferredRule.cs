using System.Collections.Generic;
using UnityEngine;

namespace XRSpotlightGUI
{
    public enum Phases
    {
        None, Addressed, Selected, Moved, Released
    }

    public class InferredAction
    {
        public GameObject obj;
        public string action;

        public InferredAction(GameObject obj, string action)
        {
            this.obj = obj;
            this.action = action; 
        }
    }

    public class InferredRule
    {
        public Phases trigger;
        public List<InferredAction> actions;
        public Modalities modalities;

        public InferredRule()
        {
            this.actions = new List<InferredAction>();
        }
    }

    public class Modalities
    {
        public bool gaze, touch, hand, remote;
    }
}