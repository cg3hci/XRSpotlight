using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using XRSpotlightGUI.Configuration;
using Event = XRSpotlightGUI.Configuration.Event;
using Object = UnityEngine.Object;

namespace XRSpotlightGUI
{
    public class CopyPasteManager
    {
        private static readonly string RuleFilePath = Environment.ExpandEnvironmentVariables(Path.Combine(Path.GetTempPath(), "copiedRule.txt"));

        private  InferredRule clipboard;
        private static CopyPasteManager singleton; 
        private Mapping[] cache;

        public static CopyPasteManager GetInstance()
        {
            if (singleton == null)
            {
                singleton = new CopyPasteManager();
            }

            return singleton;
        }
        
        private CopyPasteManager()
        {
            cache = new Mapping[Enum.GetValues(typeof(Toolkits)).Length];
        }
        
        //Writes the rule to a file copiedRule.txt
        public  void Copy(InferredRule rule)
        {
            //clipboard = rule;
            //return;
            Stream stream = File.Open(RuleFilePath, FileMode.OpenOrCreate);
            //clearing the previous content of the file
            stream.SetLength(0);
            BinaryFormatter bformatter = new BinaryFormatter();
            
            bformatter.Serialize(stream, rule);
            stream.Close();
        }

        public  bool IsRuleFileEmpty()
        {
            return new FileInfo( RuleFilePath ).Length == 0;
        }
        
        public InferredRule GetRuleFromFile()
        {
            //return clipboard;
            InferredRule inferredRule = null;
            Stream stream = File.Open(RuleFilePath, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            inferredRule = (InferredRule)bf.Deserialize(stream);
            stream.Close();
            Debug.Log("rule to paste:  "+ inferredRule);
            return inferredRule;
        }

        public void Paste(GameObject destination)
        {
            //Read rule from file
            InferredRule ruleToPaste = GetRuleFromFile();
            
            this.Paste(ruleToPaste, destination, ruleToPaste.trigger, null);

        }
        
        public void Paste(GameObject destination, Phases p, bool bestFitMode = false)
        {
            //Read rule from file
            InferredRule ruleToPaste = GetRuleFromFile();

            this.Paste(ruleToPaste, destination, p, null, bestFitMode);
        }

        public void Paste(GameObject destination, Modalities modalities, bool bestFitMode = false)
        {
            //Read rule from file
            InferredRule ruleToPaste = GetRuleFromFile();

            this.Paste(ruleToPaste, destination, ruleToPaste.trigger, modalities, bestFitMode);
        }
        
        private void Paste(InferredRule ruleToPaste, GameObject destination, Phases p, Modalities modalities, bool bestFitMode = false)
        {
            
            //check if the file is empty
            if (IsRuleFileEmpty())
            {
                Debug.Log("No rules to copy");
                return;
            }

            Dictionary<string, List<SimilarGameObject>> candidates = new Dictionary<string, List<SimilarGameObject>>();
            
            foreach (var action in ruleToPaste.actions)
            {
                if (!candidates.ContainsKey(action.objectPath))
                {
                    candidates.Add(action.objectPath, 
                        FindReplacementCandidates(action.objectPath, Type.GetType(action.scriptName)));
                }
            }

            /*// simulating the selection process
            Random r = new Random();
            foreach (var key in candidates.Keys)
            {
                selection.Add(key, r.Next(0, candidates[key].Count-1));
            }*/
            
            foreach (var action in ruleToPaste.actions)
            {
                List<SimilarGameObject> candidatesList = candidates[action.objectPath];
                switch (candidatesList.Count)
                {
                    case 0:
                        PasteMapping mapping = GetDestinationComponent(
                        destination, 
                        ruleToPaste.trigger,
                        p, 
                        action, 
                        GetMapping4Toolkit(ruleToPaste.toolkit), 
                        GetMapping4Toolkit(Utils.toolkit), 
                        ruleToPaste.modalities,
                        modalities ?? ruleToPaste.modalities
                        );
                        
                        if (mapping == null)
                        {
                            EditorUtility.DisplayDialog("No mapping found!",
                                $"Cannot find mapping for action ${action.objectPath} ${action.method}", "Got it");
                        }
                        else
                        {
                            bool receiver = false;
                            string pathName;
                            #if MRTK                
                                if (AddMrtkReceiver(mapping.action, mapping.component, out pathName))
                                {
                                    receiver = true;
                                }
                                else
                                {
                                    Debug.Log("No mrtk receiver found");
                                }
                            #endif

                            var goForIt = EditorUtility.DisplayDialog("No candidates found!",
                                "We cannot find any game object having the " + action.scriptName.Split(',')[0] +
                                " script. Please add the effect of the interaction inserting an handler for the highlighted event.", "Got it");

                            if (goForIt)
                            {
                                /*
                                [valentino] the inspector is not updated when the script is added to the gameObject until the whole function is finished;
                                indeed, if you try to highlight the script, it doesn't work. So, with the following line we wait for the inspector to be updated and then
                                highlight the script, using an asynchronous task. It is done like this because we can't instantiate Coroutines 
                                on non-monoBehaviour objects (and using threads won't work since they are on a different thread than the main one and throw exception 
                                on the execution of "Highlighter.Highlight").
                                
                                The Exception thrown is:
                                UnityEngine.UnityException: get_searchMode can only be called from the main thread.
                                Constructors and field initializers will be executed from the loading thread when loading a scene.
                                Don't use this function in the constructor or field initializers, instead move initialization code to the Awake or Start function.
                                */
                                if (ruleToPaste.toolkit == Utils.toolkit)
                                    Highlight(action.inspector);
                                else
                                {
                                    /*
                                     //This won't work because it needs to have the full path, and doing it once it's not enough.
                                     //I'm keeping it here in case we find it's a good idea to use it.
                                     Dictionary<string, Element> elementIndex = InferenceEngine.GetInstance().elementIndex;
                                    var element = elementIndex[mapping.action.componentAssemblyName.Split(',')[0]];
                                    var evt = element.events[0];
                                    ResolvedReference componentReference = new ResolvedReference
                                    {
                                        resolved = mapping.component,
                                        propertyPath = ""
                                    };
                                    ResolvedReference[] paths = InferenceEngine.GetInstance().FollowReferencePaths(evt.reference, componentReference);
                                    var path = paths[0];
                                    if (path.resolved is UnityEventBase unityEvent)
                                    {
                                        for (var i = 0; i < unityEvent.GetPersistentEventCount(); i++)
                                        {
                                            action.inspector = path.propertyPath;
                                        }
                                    }*/
                                    
                                    //As of now it seems to cover all the cases. If not we should find a better way to do it.
                                    //Other strings found that wouldn't work: Events.Array.data[0].Settings.Array.data[0].EventValue
                                    //Highlight("Events.Array.data[0].Settings.Array.data[1].EventValue");
                                    #if MRTK
                                    Highlight(receiver ? resolvePathForInspector(pathName) : mapping.action.eventPath.Last().name);
                                    #elif STEAMVR
                                    Highlight(mapping.action.eventPath.Last().name);
                                    #endif
                                }
                            }
                        }
                        
                        return;
                    case 1:
                        var replacement = candidates.First().Value.First().obj;
                        PasteActionInCandidate(ruleToPaste, p, action, destination, modalities, replacement);
                        break;
                    default:
                        if (bestFitMode)
                        {
                            var best = candidates.First().Value.First().obj;
                            PasteActionInCandidate(ruleToPaste, p, action, destination, modalities, best);
                        }
                        else
                        {
                            List<string> keys = candidatesList.Select(key => Utils.GetGameObjectPath(key.obj)).ToList();
                            RuleEditor wnd = EditorWindow.GetWindow<RuleEditor>();
                            wnd.ShowCandidatesInPaste(keys, ruleToPaste, p, action, destination, modalities);
                        }
                        break;
                }
            }
            
        }

        #if MRTK
        private string resolvePathForInspector(string pathName)
        {
            /*
            This is a custom solution specifically designed for the implementation of the receivers in MRTK.
            For some reason the inspector won't highlight the proper field, even if the path is correct, if the list of events contains
            more than one element; in that case the highlight will work only if the second array index is exactly 1
            example: "Events.Array.data[0].Settings.Array.data[1].EventValue" will properly work only if the second number is 0 if there's only one element
            and 1 if there's more than one element.
            */
            
            //This type of string (Events.Array.data[*].Event) is always good so we send it back right away...
            if (pathName.Split('.').Last() == "Event")
                return pathName;

            //...Otherwise we get the second index of the array and we manipulate it properly, as explained before.
            var secondIndex = pathName.Split('[')[2].Split(']')[0];
                int secondIndexInt;
                if (int.TryParse(secondIndex, out secondIndexInt))
                {
                    return secondIndexInt switch
                    {
                        0 => pathName,
                        1 => pathName,
                        _ => pathName.Replace(secondIndex, "1")
                    };
                }
                return pathName;
        }
        #endif
        private async void Highlight(string action)
        {
            //This delay is here to make sure the inspector is updated before highlighting the script. It could be lower but it should be fine as is.
            await Task.Delay(250);
            Highlighter.Highlight("Inspector", action + ".m_PersistentCalls.m_Calls", HighlightSearchMode.Identifier);
        }

        public void PasteActionInCandidate(
            InferredRule ruleToPaste,
            Phases p,
            InferredAction action, 
            GameObject destination, 
            Modalities modalities,
            GameObject replacement)
        {
            PasteMapping mapping = GetDestinationComponent(
                    destination, 
                    ruleToPaste.trigger,
                    p, 
                    action, 
                    GetMapping4Toolkit(ruleToPaste.toolkit), 
                    GetMapping4Toolkit(Utils.toolkit), 
                    ruleToPaste.modalities,
                    modalities ?? ruleToPaste.modalities
                );

                if (mapping == null)
                {
                    Debug.Log($"Cannot find mapping for action ${action.objectPath} ${action.method}");
                    //continue;
                }
                
                // replace the game object with the selected candidates
                mapping.action.obj = replacement;
                mapping.action.objectPath = Utils.GetGameObjectPath(replacement);
                
                PasteAction(destination, mapping.action, mapping.component);
        }

        public List<SimilarGameObject> FindReplacementCandidates(string path, Type c)
        {
            List<SimilarGameObject> replacements = new List<SimilarGameObject>();

            
            // distance 0: we search for an exact copy of the GameObject in the scene (probably the same of the src)
            var exact = GameObject.Find(path);
            if (exact != null && exact.GetComponent(c) != null)
            {
                SimilarGameObject similar = new SimilarGameObject()
                {
                    obj = exact,
                    distance = 0
                };
                replacements.Add(similar);
            }
            
            // distance 1: we cannot find the exact copy. We look for other GameObjects having the same script
            var componentObjects = (Component[]) Object.FindObjectsOfType(c);
            foreach (var co in componentObjects)
            {
                SimilarGameObject similar = new SimilarGameObject()
                {
                    obj = co.gameObject,
                    distance = 1
                };
                if (exact != similar.obj)
                {
                    replacements.Add(similar);
                }
               
            }
            
            
            // TODO distance 2: we should find some criteria for finding other candidates.
            return replacements;
        }

        public Mapping GetMapping4Toolkit(Toolkits t)
        {

            string path = null;
            switch (t)
            {
                case Toolkits.MRTK:
                    path = Mapping.MRTK;
                    break;
                
                
                case Toolkits.SteamVR:
                    path = Mapping.STEAMVR;
                    break;
            }

            if (path == null)
                return null;

            var i = (int)t;
            if (cache[i] == null)
            {
                cache[i] = Mapping.FromFile(path);
            }

            return cache[i];
            
        }

        private PasteMapping GetDestinationComponent(
            GameObject destination, 
            Phases srcPhase,
            Phases destPhase,
            InferredAction action, 
            Mapping srcToolkit,
            Mapping destToolkit,
            Modalities srcModalities,
            Modalities destModalities)
        {
            PasteMapping mapping = new PasteMapping();
            if (srcToolkit.Equals(destToolkit) && srcModalities.Equals(destModalities) && srcPhase == destPhase)
            {

                // both modalities and toolkits are the same. We can use the same component
                var destComponent = destination.GetComponent(Type.GetType(action.componentAssemblyName));
                if (destComponent == null)
                {
                    // the destination object does not contain the required component. We add it here
                    destComponent = destination.AddComponent(Type.GetType(action.componentAssemblyName));
                }

                // in both cases, we have our destination component, having the same type of the source
                mapping.component = destComponent;
                mapping.action = action;
                return mapping;
            }
            else
            {
                // either different toolkits or modalities, we search for a compatible component supporting the 
                //  same modality and the same abstract interaction phase
                var compatibleList = FindInteractionInToolkit(action, destToolkit, destModalities, destPhase);
                var compatible = compatibleList.FirstOrDefault();
                if (compatible == null) return null;

                var destComponent = destination.GetComponent(Type.GetType(compatible.componentAssemblyName));
                if (destComponent == null)
                {
                    // the destination object does not contain the required component. We add it here
                    destComponent = destination.AddComponent(Type.GetType(compatible.componentAssemblyName));
                }
                mapping.component = destComponent;
                mapping.action = compatible;
                return mapping;
            }
            
        }

        class InteractionCandidate
        {
            public Element element;
            public Event evt;
            public Modalities mod;
            public float similarityScore = 0;
            
            public InteractionCandidate(Element element, Event evt, Modalities mod)
            {
                this.element = element;
                this.evt = evt;
                this.mod = mod;
            }
        }
        public List<InferredAction> FindInteractionInToolkit(InferredAction source, Mapping toolkit, Modalities modalities, Phases phase)
        {
            List<Element> roots = new List<Element>();
            List<InferredAction> result = new List<InferredAction>();
            List <InteractionCandidate> candidates = new List<InteractionCandidate>();
            Element evtBase = null;
            
            foreach (var element in toolkit.elements)
            {
                foreach (var evt in element.events)
                {
                    // we do not map subparts...
                    if(evt.subpart!= null && evt.subpart.Length > 0) continue;
                    
                    var evtModalities = new Modalities();
                    evtModalities.SetModality(ModalitiesEnum.gaze, HasModality(evt, "gaze"));
                    evtModalities.SetModality(ModalitiesEnum.hand, HasModality(evt, "hand"));
                    evtModalities.SetModality(ModalitiesEnum.grab, HasModality(evt, "grab"));
                    evtModalities.SetModality(ModalitiesEnum.remote, HasModality(evt, "remote"));
                    evtModalities.SetModality(ModalitiesEnum.touch, HasModality(evt, "touch"));
                    
                    
                    if (//evtModalities.IncludesModalities(modalities) &&
                        InferredRule.PhaseFromString(evt.definition) == phase)
                    {
                        if (candidates.Count == 0) {
                            // if we did not find any candidate yet, we accept it straightaway
                            InteractionCandidate candidate = new InteractionCandidate(element, evt, evtModalities);
                            candidates.Add(candidate);
                            candidate.similarityScore = candidate.mod.CountCommonModalities(modalities);
                        }
                        else
                        {
                            int i = 0;
                            InteractionCandidate candidate = new InteractionCandidate(element, evt, evtModalities);
                            candidate.similarityScore = candidate.mod.CountCommonModalities(modalities);
                            /*for(i=0; i<candidates.Count; i++){
                                // if we already found a candidate, we keep the order one having less additional
                                // modalities beyond the requested
                                var evtDelta = //evtModalities.CountModalities() -
                                               candidate.mod.CountCommonModalities(modalities);
                                var foundDelta = //candidates[i].mod.CountModalities() -
                                                 candidates[i].mod.CountCommonModalities(modalities);
                                candidate.similarityScore = foundDelta;
                                if (evtDelta > foundDelta)
                                {
                                    candidates.Insert(i, candidate);
                                    
                                    break;
                                }
                            }*/
                            //if(i==candidates.Count){
                                // we did not find a candidate with less additional modalities. We add it at the end
                                candidates.Add(candidate);
                            //}
                        }
                        
                    }
                }

                if (element.isComponent)
                {
                    roots.Add(element);

                    if (element.eventReferences != null)
                    {
                        evtBase = element;
                    }
                }

            }
            
            
            candidates.Sort((x, y) =>
                (int)(y.similarityScore - x.similarityScore)
            );
            foreach (var cand in candidates)
            {
                if (roots.Contains(cand.element))
                {
                    Type componentType = InferredRule.ClassForName(cand.element.className);
                    if(componentType == null) continue;
                    InferredAction destination = source.Clone() as InferredAction;
                    destination.componentAssemblyName = componentType.AssemblyQualifiedName;
                    destination.eventPath = cand.evt.reference;
                    destination.distance = Math.Abs(modalities.CountModalities() - cand.similarityScore);
                    result.Add(destination);
                }
                else
                {
                    //  [davide] I assume that if a component has event references
                    //  it contains any other event not defined in a component, and that this element is only one
                    //  This should be revised if we find toolkits breaking this assumption. 
                    Type componentType = InferredRule.ClassForName(evtBase.className);
                    InferredAction destination = source.Clone() as InferredAction;
                    destination.componentAssemblyName = componentType.AssemblyQualifiedName;
                    List<MemberReference> completeReference = new List<MemberReference>();
                    foreach (var r in evtBase.eventReferences[0].reference)
                    {
                        completeReference.Add(r.Clone() as MemberReference);;
                    }
                   
                    completeReference.AddRange(cand.evt.reference); 
                    Type eventType = InferredRule.ClassForName(cand.element.className);
                    completeReference[evtBase.eventReferences[0].reference.Length - 1].type = eventType.AssemblyQualifiedName;
                    destination.eventPath = completeReference.ToArray(); 
                    destination.distance = Math.Abs(modalities.CountModalities() - cand.similarityScore);
                    result.Add(destination);
                }
                
            }

            return result;
        }
        
        private bool HasModality(Event evt, string modality)
        {
            return evt.modality != null && ((IList)evt.modality).Contains(modality);
        }

        
        private InferredRule GetRuleByPhase(Phases phase, InferredRule[] rules)
        {
            foreach (var rule in rules)
            {
                if (rule.trigger == phase)
                {
                    return rule;
                }
            }
            return null;
        }

        private void PasteAction(GameObject destination, InferredAction action, Component component)
        {
            var evtSrc = destination.GetComponent(component.GetType().Name);

            if (!AddMrtkReceiver(action, evtSrc))
            {
                if (!AddUnityEventHandler(action, evtSrc))
                {
                    Debug.Log($"Cannot copy the action {action.method}");
                }
            }

        }

        private bool AddUnityEventHandler(InferredAction a, Component c)
        {
            SerializedObject so = new SerializedObject(c);
            var inspector = a.eventPath[a.eventPath.Length - 1].inspectorProperty;
            if (inspector == null)
                inspector = a.eventPath[a.eventPath.Length - 1].name;
            SerializedProperty eventItem = so.FindProperty(inspector);
            if (eventItem == null) return false;
            CopyPersistentCall(a, eventItem);
            so.ApplyModifiedProperties();
            return true;

        } 

        private bool AddMrtkReceiver(InferredAction a, Component c)
        {
            return AddMrtkReceiver(a, c, out _);
        }
        
        private bool AddMrtkReceiver(InferredAction a, Component c , out string pathName)
        {
            // We assume that all events having paths longer than 1 contain event references in the configuration file. 
            if (a.eventPath.Length > 1)
            {
                SerializedObject so = new SerializedObject(c);
                SerializedProperty current = null;

                for (var i = 0; i < a.eventPath.Length; i++)
                {
                    var step = a.eventPath[i];
                    if (i == 0)
                    {
                        current = so.FindProperty(step.inspectorProperty);
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(step.inspectorProperty))
                        {
                            var eventItem = FindReceveiverByType(current, step.type);
                            SerializedProperty uEvent = eventItem.FindPropertyRelative("Event");
                            SerializedProperty eventName = eventItem.FindPropertyRelative("Name");
                            SerializedProperty className = eventItem.FindPropertyRelative("ClassName");

                            if (className != null)
                            {
                                SerializedProperty assemblyQualifiedName =
                                    eventItem.FindPropertyRelative("AssemblyQualifiedName");
                                Type receiverType = Type.GetType(step.type);
                                assemblyQualifiedName.stringValue = receiverType?.AssemblyQualifiedName;
                                className.stringValue = receiverType.Name;
                            }


                            var next = a.eventPath[i + 1];
                            if (!next.inspectorProperty.Equals("Event"))
                            {
                                // the event properties are not in the ReceiverBase Event field
                                var subpath = next.inspectorProperty.Split('.');
                                foreach (var s in subpath)
                                {
                                    if (s.StartsWith("data") || s.StartsWith("EventValue"))
                                    {
                                        continue;
                                    }

                                    if (s.StartsWith("Array"))
                                    {
                                        var index = eventItem.arraySize;
                                        eventItem.InsertArrayElementAtIndex(index);
                                        eventItem = eventItem.GetArrayElementAtIndex(index);
                                        continue;
                                    }

                                    eventItem = eventItem.FindPropertyRelative(s);
                                }

                                eventName = eventItem.FindPropertyRelative("Name");
                                eventName.stringValue = next.name;

                                var eventType = eventItem.FindPropertyRelative("Type");
                                eventType.enumValueIndex = 18;

                                var eventLabel = eventItem.FindPropertyRelative("Label");
                                eventLabel.stringValue = GetReceiverLabel(next.name);
                                eventItem = eventItem.FindPropertyRelative("EventValue");

                            }
                            else
                            {
                                // the event overloads the property of ReceiverBase
                                eventItem = uEvent;
                            }



                            CopyPersistentCall(a, eventItem);
                            so.ApplyModifiedProperties();
                            pathName = eventItem.propertyPath;
                            return true;
                        }

                        current = current.FindPropertyRelative(step.inspectorProperty);
                    }
                }
                pathName = "";
                return false;
            }
            else
            {
                pathName = "";
                return false;
            }



        }

        private static SerializedProperty FindReceveiverByType(SerializedProperty current, string assemblyQualifiedName)
        {
            for (var i = 0; i < current.arraySize; i++)
            {
                var cursor = current.GetArrayElementAtIndex(i);
                var cursorAssembly = cursor.FindPropertyRelative("AssemblyQualifiedName").stringValue;
                if (cursorAssembly != null && cursorAssembly.Equals(assemblyQualifiedName))
                {
                    return cursor;
                }
            }


            var e = current.arraySize;
            current.InsertArrayElementAtIndex(e);
            SerializedProperty eventItem = current.GetArrayElementAtIndex(e);
            return eventItem;
        }

        private static void CopyPersistentCall(InferredAction a, SerializedProperty eventItem)
        {
            bool temporaryGameObject = false;
            SerializedProperty persistentCalls =
                eventItem.FindPropertyRelative("m_PersistentCalls.m_Calls");

            var index = persistentCalls.arraySize;
            persistentCalls.InsertArrayElementAtIndex(index);
            var call = persistentCalls.GetArrayElementAtIndex(index);

            Type scriptType = Type.GetType(a.scriptName);
            var gameObject = GameObject.Find(a.objectPath);
            //[valentino] this is a workaround for the zero candidates issue. We create a dummy gameObject to hold the script.
            temporaryGameObject = gameObject == null;
            if (temporaryGameObject)
            {
                var substitute = new GameObject();
                substitute.AddComponent(scriptType);
                gameObject = substitute;
            }
            var target = gameObject.GetComponent(scriptType);
            //[valentino] after we used it for the workaround, we destroy it. Leaving it in the scene will
            //assign it to the destination gameObject, which is not what we want.
            if (temporaryGameObject)
                Object.DestroyImmediate(gameObject);
            call.FindPropertyRelative("m_Target").objectReferenceValue = target;
            call.FindPropertyRelative("m_Mode").enumValueIndex = 1;

            call.FindPropertyRelative("m_TargetAssemblyTypeName").stringValue =
                scriptType?.FullName + ", " + scriptType?.Namespace;
            //scriptType?.Name + ", " + scriptType?.Assembly.GetName().Name;

            call.FindPropertyRelative("m_MethodName").stringValue = a.method;
            call.FindPropertyRelative("m_Mode").enumValueIndex = a.callMode;
            call.FindPropertyRelative("m_CallState").enumValueIndex = a.callState;
            var argument = call.FindPropertyRelative("m_Arguments");

            switch (a.callMode)
            {
                case 2:
                    // object parameter
                    if (a.argumentType != null)
                    {
                        UnityEngine.Object[] objs = Resources.FindObjectsOfTypeAll(Type.GetType(a.argumentType));
                        foreach (var obj in objs)
                        {
                            if (obj.name.Equals(a.argument))
                            {

                                argument.FindPropertyRelative("m_ObjectArgument").objectReferenceValue = obj;
                                var argumentType = Type.GetType(a.argumentType);
                                argument.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue =
                                    argumentType?.FullName + ", " + argumentType?.Namespace;
                                break;
                            }
                        }

                    }

                    break;

                case 3:
                    // int parameter
                    var intval = int.Parse(a.argument);
                    argument.FindPropertyRelative("m_IntArgument").intValue = intval;
                    break;

                case 4:
                    // float value
                    var floatval = float.Parse(a.argument);
                    argument.FindPropertyRelative("m_FloatArgument").floatValue = floatval;
                    break;

                case 5:
                    // string value
                    argument.FindPropertyRelative("m_StringArgument").stringValue = a.argument;
                    break;

                case 6:
                    // bool value
                    var boolval = bool.Parse(a.argument);
                    argument.FindPropertyRelative("m_BoolArgument").boolValue = boolval;
                    break;
            }


        }

        private string GetReceiverLabel(string methodName)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var c in methodName.ToCharArray())
            {
                if (Char.IsUpper(c))
                {
                    builder.Append(" ");
                }

                builder.Append(c);
            }


            return builder.ToString();
        }
    }

    internal class PasteMapping
    {
        public Component component;
        public InferredAction action;
    }

    public class SimilarGameObject
    {
        public GameObject obj;
        public int distance;
    }
}