using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
#if MRTK
using Microsoft.MixedReality.Toolkit.UI; 
#endif
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace XRSpotlightGUI
{
    public class Utils
    {
        //C:\Users\Username\AppData\Local\Temp\copiedRule.txt

        #if MRTK
        public static readonly Toolkits toolkit = Toolkits.MRTK;
        #elif STEAMVR
        public static readonly Toolkits toolkit = Toolkits.SteamVR;
        #endif
        
        public static string RemoveGameObjectPath(string path)
        {
            return path.Substring(path.LastIndexOf('/') + 1);
        }
        

        public static Collider[] findCollider(GameObject gameObject)
        {
            Collider [] colliders = gameObject.GetComponents<Collider>();
            return colliders;
        }
        
        public static Component[] findRigidbody(GameObject gameObject)
        {
            Component[] rigidbodies = gameObject.GetComponents<Rigidbody>();
            Component[] rigidbodies2D = gameObject.GetComponents<Rigidbody2D>();
            return rigidbodies.Concat(rigidbodies2D).ToArray();
        }
        
        public static string GetGameObjectPath(Object obj)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty("gameObject");
            return propertyInfo != null ? GetGameObjectPath(propertyInfo.GetValue(obj, null) as GameObject) : obj.name;
        }
        
        public static string GetGameObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }
        
        //We need those method for the popUp UI
        public static string ConvertSlashToUnicodeSlash(string text_)
        {
            return text_.Replace('/', '\u2215');
        }
 
        public static string ConvertUnicodeSlashToSlash(string text_)
        {
            return text_.Replace('\u2215', '/');
        }

        public static (string,string) CutModalityString(string text)
        {
            int index = 0, count =0;
            for (int i = 0; i < text.Length; ++i)
            {
                if (text[i] == '/')
                {
                    if (++count == 2)
                    {
                        index = i;
                        break;
                    }
                }
            }

            string s1 = "";
            string s2 = "";

            if (count > 0)
            {
                s1 = text.Substring(0, index);
                s2 = text.Substring(index + 2);
            }
            else s1 = text;
           
            return (s1,s2);
        }

        public static string GetInteractionString(InferredRule rule)
        {
            string result;
            result = $"When {rule.gameObject.name} is";

            int count = 0;
            for (int i = 0; i < Enum.GetValues(typeof(ModalitiesEnum)).Length; i++)
            {
                var junct = count == 0 ? "" : "/";
                if (rule.modalities.GetModality((ModalitiesEnum)i))
                {
                    result += $"{junct} {RuleEditor.Label4ModalityPhase((ModalitiesEnum)i, rule.trigger)} ";
                    count++;
                }
            }

            return result;
        }
        
        /*public static void CopyUnityEvents(object sourceObj, string source_UnityEvent, object dest, FieldInfo unityEvent) {
    //FieldInfo unityEvent = sourceObj.GetType().GetField(source_UnityEvent, E_Helpers.allBindings);
    
        SerializedObject so = new SerializedObject((Object)sourceObj);
        SerializedProperty persistentCalls = so.FindProperty(source_UnityEvent).FindPropertyRelative("m_PersistentCalls.m_Calls");
        for (int i = 0; i < persistentCalls.arraySize; ++i)
        {
            Object target = persistentCalls.GetArrayElementAtIndex(i).FindPropertyRelative("m_Target").objectReferenceValue;
            string methodName = persistentCalls.GetArrayElementAtIndex(i).FindPropertyRelative("m_MethodName").stringValue;
            MethodInfo method = null;
            try
            {
                method = target.GetType().GetMethod(methodName, E_Helpers.allBindings);
            }
            catch
            {
                foreach (MethodInfo info in target.GetType().GetMethods(E_Helpers.allBindings).Where(x => x.Name == methodName))
                {
                    ParameterInfo[] _params = info.GetParameters();
                    if (_params.Length < 2)
                    {
                        method = info;
                    }
                }
            }
            ParameterInfo[] parameters = method.GetParameters();
            switch(parameters[0].ParameterType.Name)
            {
                case nameof(System.Boolean):
                    bool bool_value = persistentCalls.GetArrayElementAtIndex(i).FindPropertyRelative("m_Arguments.m_BoolArgument").boolValue;
                    var bool_execute = System.Delegate.CreateDelegate(typeof(UnityAction<bool>), target, methodName) as UnityAction<bool>;
                    UnityEventTools.AddBoolPersistentListener(
                        dest as UnityEventBase,
                        bool_execute,
                        bool_value
                    );
                    break;
                case nameof(System.Int32):
                    int int_value = persistentCalls.GetArrayElementAtIndex(i).FindPropertyRelative("m_Arguments.m_IntArgument").intValue;
                    var int_execute = System.Delegate.CreateDelegate(typeof(UnityAction<int>), target, methodName) as UnityAction<int>;
                    UnityEventTools.AddIntPersistentListener(
                        dest as UnityEventBase,
                        int_execute,
                        int_value
                    );
                    break;
                case nameof(System.Single):
                    float float_value = persistentCalls.GetArrayElementAtIndex(i).FindPropertyRelative("m_Arguments.m_FloatArgument").floatValue;
                    var float_execute = System.Delegate.CreateDelegate(typeof(UnityAction<float>), target, methodName) as UnityAction<float>;
                    UnityEventTools.AddFloatPersistentListener(
                        dest as UnityEventBase,
                        float_execute,
                        float_value
                    );
                    break;
                case nameof(System.String):
                    string str_value = persistentCalls.GetArrayElementAtIndex(i).FindPropertyRelative("m_Arguments.m_StringArgument").stringValue;
                    var str_execute = System.Delegate.CreateDelegate(typeof(UnityAction<string>), target, methodName) as UnityAction<string>;
                    UnityEventTools.AddStringPersistentListener(
                        dest as UnityEventBase,
                        str_execute,
                        str_value
                    );
                    break;
                case nameof(System.Object):
                    Object obj_value = persistentCalls.GetArrayElementAtIndex(i).FindPropertyRelative("m_Arguments.m_ObjectArgument").objectReferenceValue;
                    var obj_execute = System.Delegate.CreateDelegate(typeof(UnityAction<Object>), target, methodName) as UnityAction<Object>;
                    UnityEventTools.AddObjectPersistentListener(
                        dest as UnityEventBase,
                        obj_execute,
                        obj_value
                    );
                    break;
                default:
                    var void_execute = System.Delegate.CreateDelegate(typeof(UnityAction), target, methodName) as UnityAction;
                    UnityEventTools.AddPersistentListener(
                        dest as UnityEvent,
                        void_execute
                    );
                    break;
            }
        }
    
}*/
    }
}