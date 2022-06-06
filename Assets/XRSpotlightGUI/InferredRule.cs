using System.Collections.Generic;
using UnityEngine;

namespace XRSpotlightGUI
{
    public enum Phases
    {
        None, Idle, Addressed, Selected, Moved, Released
    }

    public class InferredAction
    {
        public Object obj;
        public string action;
        public string method;

        public InferredAction(Object obj, string action, string method)
        {
            this.obj = obj;
            this.action = action;
            this.method = method;
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

        public void Add(InferredAction a)
        {
            this.actions.Add(a);
        }
    }

    public class Modalities
    {
        public bool gaze, touch, hand, remote;
    }
}