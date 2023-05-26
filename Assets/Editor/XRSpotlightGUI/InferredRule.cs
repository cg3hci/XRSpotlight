using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using XRSpotlightGUI.Configuration;
using Event = UnityEngine.Event;
using Object = UnityEngine.Object;

namespace XRSpotlightGUI
{
    public enum Phases
    {
        None = 0, 
        Entered = 1,
        Selected = 2, 
        Moved = 3, 
        Released = 4,
        Left = 5,
    }
    
    
    
    [Serializable()]
    public class InferredAction:ISerializable, ICloneable
    {
        //Fields we won't use for copy-pasting
        //gameobject containing the rule
        public Object obj;
        // the index of the action among the PersistentTargets
        public int index;
        // store the reference name for inspector highlighting
        public string inspector;
        // string "calls + methodName"
        public string action;
        //Serializable fields we need for copy-paste:
        public string componentAssemblyName;
        //the path of the gameoObject that contains the script
        public string objectPath; 
        //function called
        public string method; 
        //name ot the script that contains the method
        public string scriptName;
        public MemberReference[] eventPath;
        public int callState;
        public string argument;
        public string argumentType;
        public int callMode;
        
        // helper fields for rule similarity
        public double distance = Double.MaxValue;
        
        
        public InferredAction(Object obj, string action, string method, string componentAssemblyName)
        {
            this.obj = obj;
            this.action = action;
            this.method = method;
            this.componentAssemblyName = componentAssemblyName;
            if (obj != null)
            {
                objectPath = Utils.GetGameObjectPath(obj);
                scriptName = obj.GetType().AssemblyQualifiedName;
            }
            
        }

        public InferredAction(SerializationInfo info, StreamingContext context)
        {
            objectPath = (string)info.GetValue("objectPath", typeof(string));
            method = (string)info.GetValue("method", typeof(string));
            componentAssemblyName = (string)info.GetValue("componentAssemblyName", typeof(string));
            scriptName = (string)info.GetValue("scriptName", typeof(string));
            eventPath = (MemberReference[])info.GetValue("eventPath", typeof(MemberReference[]));
            inspector = (string) info.GetValue("inspector", typeof(string));
            callState = (int)info.GetValue("callState", typeof(int));
            argument = (string)info.GetValue("argument", typeof(string));
            argumentType = (string)info.GetValue("argumentType", typeof(string));
            callMode = (int)info.GetValue("callMode", typeof(int));
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("objectPath", objectPath);
            info.AddValue("method", method);
            info.AddValue("componentAssemblyName", componentAssemblyName);
            info.AddValue("scriptName", scriptName);
            info.AddValue("eventPath", eventPath);
            info.AddValue("inspector", inspector);
            info.AddValue("callState", callState);
            info.AddValue("argument", argument);
            info.AddValue("argumentType", argumentType);
            info.AddValue("callMode", callMode);
       
        }

        public object Clone()
        {
            InferredAction clone = new InferredAction(
                this.obj, this.action, this.method, this.componentAssemblyName)
            {
                argument =  this.argument,
                index =  this.index,
                inspector = this.inspector,
                argumentType = argumentType,
                callMode = callMode,
                callState = callState,
                scriptName = this.scriptName,
                objectPath = this.objectPath,
                distance = this.distance
            };

            return clone;
        }

        public bool Equals(object o)
        {
            if (o == null)
                return false;
            if (o is InferredAction a)
            {
                return a.argument != null && a.argument.Equals(this.argument) &&
                       a.argumentType != null && a.argumentType.Equals(this.argumentType) &&
                       a.callMode == this.callMode &&
                       a.callState == this.callState &&
                       a.scriptName != null && a.scriptName.Equals(this.scriptName) &&
                       a.objectPath != null && a.objectPath.Equals(this.objectPath);
            }

            return false;
        }

        public override string ToString()
        {
            return $"{action} {method} {componentAssemblyName} {scriptName} {objectPath} {inspector} {callState} {argument} {argumentType} {callMode}";
        }
    }
    
    

    [Serializable()]
    public class PersistentCallSerialization
    {
        public string target;
        public string methodName;
        
    }

    public class InferredRuleGroup
    {
        public string title;
        public List<InferredRule> rules = new List<InferredRule>();
        public bool pasteButton = false;
        public ModalitiesEnum pasteModality = ModalitiesEnum.gaze;
    }
    
    [Serializable()]
    public class InferredRule: ISerializable, IComparable<InferredRule>, IComparable
    {
        public GameObject gameObject;
        public Phases trigger;
        public string part;
        public List<InferredAction> actions;
        public Modalities modalities;
        public string interaction;
        public Toolkits toolkit;

        
        
        public static Phases PhaseFromString(string phase)
        {
            Phases p = Phases.None; 

            if (phase.Equals("leave")) p = Phases.Left;
            if (phase.Equals("enter")) p = Phases.Entered;
            if (phase.Equals("select")) p = Phases.Selected;
            if (phase.Equals("movement")) p = Phases.Moved;
            if (phase.Equals("release")) p = Phases.Released;

            return p;
        }
        
        public static Type ClassForName(string name)
        {
            Type componentType = null; 
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (ass.FullName.StartsWith("System."))
                    continue;
                componentType = ass.GetType(name);
                if (componentType != null)
                    break;
            }

            return componentType;
        }
        
        public InferredRule()
        {
            this.actions = new List<InferredAction>();
        }

        public InferredRule(SerializationInfo info, StreamingContext ctxt)
        {
            trigger = (Phases)info.GetValue("trigger", typeof(Phases));
            actions = (List<InferredAction>)info.GetValue("actions", typeof(List<InferredAction>));
            modalities = (Modalities)info.GetValue("modalities", typeof(Modalities));
            toolkit = (Toolkits)info.GetValue("toolkit", typeof(Toolkits));
        }

        public void Add(InferredAction a)
        {
            this.actions.Add(a);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("trigger", trigger);
            info.AddValue("actions", actions);
            info.AddValue("modalities", modalities);
            info.AddValue("toolkit", toolkit);
        }

        public int CompareTo(InferredRule other)
        {
            if (string.IsNullOrEmpty(this.part) && ! string.IsNullOrEmpty(other.part))
                return -1;
            if (!string.IsNullOrEmpty(this.part)  && string.IsNullOrEmpty(other.part))
                return 1;
            if((this.part == null && other.part == null) || (this.part != null && this.part.Equals(other.part)))
                return this.trigger - other.trigger;
            return string.Compare(this.part, other.part, StringComparison.InvariantCulture);
        }

        public int CompareTo(object obj)
        {
            if (obj is InferredRule rule)
            {
                return CompareTo(rule);
            }

            return 0;
        }

        public bool IncludesModalities(InferredRule r)
        {
            return r.modalities.IncludesModalities(this.modalities);

        }

        public override string ToString()
        {
            return $"Trigger: {trigger}, Modalities:  {modalities}, Actions:  {actions}";
        }
    }

    [Serializable()]
    public class InferredBehaviour : ISerializable
    {
        //the path of the gameoObject that contains the script
        public string objectPath; 
        // name of the component, we need it to disable/enable with the toggle button
        public string componentName;
        public string[] definitions;

        public InferredBehaviour(GameObject go, string componentName, string []definitions)
        {
            objectPath = Utils.GetGameObjectPath(go);
            this.componentName = componentName;
            this.definitions = definitions;
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("objectPath", objectPath);
            info.AddValue("componentName", componentName);
            info.AddValue("definitions", definitions);
        }
        
        public InferredBehaviour(SerializationInfo info, StreamingContext ctxt)
        {
            objectPath = info.GetString("objectPath");
            componentName = info.GetString("componentName");
            definitions = (string[])info.GetValue("definitions", typeof(string[]));
        }

        public override string ToString()
        {
            return "component name: " + componentName + " object path: " + objectPath + " definitions: " + string.Join(",", definitions);
        }
    }
    
    [Serializable()]
    public class Modalities: ISerializable, ICloneable
    {
        public bool[] modality;

        public Modalities()
        {
            modality = new bool[Enum.GetValues(typeof(ModalitiesEnum)).Length];
        }
        public Modalities(SerializationInfo info, StreamingContext ctxt)
        {
            modality = new bool[Enum.GetValues(typeof(ModalitiesEnum)).Length];
            modality[(int) ModalitiesEnum.gaze] = (bool)info.GetValue("gaze", typeof(bool));
            modality[(int) ModalitiesEnum.touch] = (bool)info.GetValue("touch", typeof(bool));
            modality[(int) ModalitiesEnum.grab] = (bool)info.GetValue("grab", typeof(bool));
            modality[(int) ModalitiesEnum.hand] = (bool)info.GetValue("hand", typeof(bool));
            modality[(int) ModalitiesEnum.remote] = (bool)info.GetValue("remote", typeof(bool));     
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("gaze",  modality[(int) ModalitiesEnum.gaze]);
            info.AddValue("touch", modality[(int) ModalitiesEnum.touch]);
            info.AddValue("grab", modality[(int) ModalitiesEnum.grab]);
            info.AddValue("hand", modality[(int) ModalitiesEnum.hand]);
            info.AddValue("remote", modality[(int) ModalitiesEnum.remote]);
        }

        public void SetModality(ModalitiesEnum m, bool val)
        {
            modality[(int)m] = val;
        }

        public bool GetModality(ModalitiesEnum m)
        {
            return modality[(int)m];
        }

        public override bool Equals(object o)
        {
            if (o is Modalities modalities)
            {
                for (int i = 0; i < modality.Length; i++)
                {
                    if (this.modality[i] != modalities.modality[i])
                        return false;
                }

                return true;
            }

            return false;
        }

        public object Clone()
        {
            Modalities toReturn = new Modalities();
            for (int i = 0; i < modality.Length; i++)
            {
                toReturn.modality[i] = this.modality[i];
            }

            return toReturn;
        }

        public  bool IncludesModalities(Modalities b)
        {
            for (int i = 0; i < this.modality.Length; i++)
            {
                if (b.modality[i] && !this.modality[i]) return false;
                
            }

            return true;
        }

        public int CountModalities()
        {
            int count = 0;
            foreach (var m in this.modality)
            {
                if (m) count++;
            }

            return count;

        }

        public int CountCommonModalities(Modalities b)
        {
            int count = 0;
            for (int i = 0; i < this.modality.Length; i++)
            {
                if (this.modality[i] && b.modality[i]) count++;
            }

            return count;
        }

        public override string ToString()
        {
            /*return string.Join(",", modality);*/
            string toReturn = "";
            for (int i = 0; i < modality.Length; i++)
            {
                if (modality[i])
                {
                    toReturn += ((ModalitiesEnum)i) + " ";
                }
            }
            return toReturn;
        }
    }

    public enum ModalitiesEnum
    {
        gaze, touch, grab, hand, remote
    }
    
    [Serializable()]
    public class ParametersActions
    {
        public string name;
        public string type;
        
        public ParametersActions(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
        
        public ParametersActions(SerializationInfo info, StreamingContext ctxt)
        {
            name = (string)info.GetValue("name", typeof(string));
            type = (string)info.GetValue("type", typeof(string));
        }
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", name);
            info.AddValue("type", type);
        }
    }
}