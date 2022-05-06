using System;
using System.Globalization;
using System.IO;
using ECAScripts.Utils;
using UnityEngine;

namespace ECARules4All.RuleEngine
{
    public class TextRuleSerializer
    {
        public TextRuleSerializer()
        {

        }

        public void SaveRules(string path)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            using (StreamWriter writer = File.CreateText(path))
            {
                PrintDeclarations(writer);
                writer.WriteLine();
                PrintSimpleTypeDeclarations(writer);
                writer.WriteLine();
                PrintBehaviourDeclarations(writer);
                writer.WriteLine();

                //TODO add alias

                foreach (Rule r in RuleEngine.GetInstance().Rules())
                {
                    PrintRule(r, writer);
                }

                writer.Flush();
            }
        }

        public Type GetECAObjectType(GameObject gameObject)
        {
            bool isECAObject = false;
            bool changed;
            Type maxType = typeof(ECAObject);

            do
            {
                changed = false;
                foreach (Component c in gameObject.GetComponents(typeof(Component)))
                {
                    if (c.GetType() == typeof(ECAObject))
                    {
                        isECAObject = true;
                    }
                    else
                    {
                        foreach (RequireComponent require in c.GetType().GetCustomAttributes(
                            typeof(RequireComponent), true))
                        {
                            if (c.GetType() != typeof(Behaviour) && require.m_Type0 == maxType)
                            {
                                maxType = c.GetType();
                                changed = true;
                            }
                        }
                    }

                }
            } while (changed);
            

            if (isECAObject) return maxType;
            else return null;
        }

        private string GetTypeName(Type t)
        {
            var attrs = t.GetCustomAttributes(typeof(ECARules4AllAttribute), false);
            ECARules4AllAttribute attr = (ECARules4AllAttribute)attrs[0];
            return attr.Name;
        }

        private void PrintDeclarations(TextWriter writer)
        {
            var objects = GameObject.FindObjectsOfType<ECAObject>();
            foreach(ECAObject obj in objects)
            {
                GameObject gameObj = obj.gameObject;
                writer.WriteLine(String.Format("{0} {1} {2};",
                    "define",
                    GetTypeName(GetECAObjectType(gameObj)),
                    gameObj.name
                    )); ;
            }
        }

        private void PrintSimpleTypeDeclarations(TextWriter writer)
        {
            foreach(Rule r in RuleEngine.GetInstance().Rules())
            {
                PrintActionSimpleType(r.GetEvent(), writer);
                foreach(Action a in r.GetActions())
                {
                    PrintActionSimpleType(a, writer);
                }
            }
        }

        private void PrintActionSimpleType(Action a, TextWriter writer)
        {
            DeclareSimpleType(a.GetObject(), writer);
            DeclareSimpleType(a.GetModifierValue(), writer);
        }

        private void DeclareSimpleType(object o, TextWriter writer)
        {
            if (o == null) return;
            if(o is Position)
            {
                Position pos = o as Position;
                writer.Write("define position ");
                writer.Write(pos.Name);
                writer.Write(" = (");
                writer.Write(pos.ToString());
                writer.WriteLine(");");
            }
            if(o is Path)
            {
                Path path = o as Path;
                writer.Write("define path ");
                writer.Write(path.Name);
                writer.Write(" = {");
                for(int i = 0; i < path.Points.Count; i++)
                {
                    Position pos = path.Points[i];
                    writer.Write("(");
                    writer.Write(pos.ToString());
                    if(i < path.Points.Count - 1)
                        writer.Write("), ");
                    else
                        writer.Write(")");
                }
                writer.WriteLine("};");
            }
            if(o is ECAColor) 
            {
                ECAColor col = o as ECAColor;
                writer.Write("define color ");
                writer.Write(col.Name);
                writer.Write(" = ");
                if(col.Name!=null)
                    writer.Write(col.getHex());
                else writer.Write(ToHex(col.Color));
                writer.WriteLine(";");
            }
            /*if(o is Color)
            {
                string hex = ColorUtility.ToHtmlStringRGB((UnityEngine.Color) o);
                ECAColor ecaColor = new ECAColor(hex, false);
                writer.Write("define color ");
                writer.Write(ecaColor.Name);
                writer.Write(" = ");
                if(ecaColor.Name!=null)
                    writer.Write(ecaColor.getHex());
                else writer.Write(ToHex(ecaColor.Color));
                writer.WriteLine(";");
            }*/
        }

        private string ToHex(Color color)
        {
            return String.Format("#{0}{1}{2}{3}"
                , color.a.ToString("X").Length == 1 ? String.Format("0{0}", color.a.ToString("X")) : color.a.ToString("X")
                , color.r.ToString("X").Length == 1 ? String.Format("0{0}", color.r.ToString("X")) : color.r.ToString("X")
                , color.g.ToString("X").Length == 1 ? String.Format("0{0}", color.g.ToString("X")) : color.g.ToString("X")
                , color.b.ToString("X").Length == 1 ? String.Format("0{0}", color.b.ToString("X")) : color.b.ToString("X"));
        }
        
        private void PrintBehaviourDeclarations(TextWriter writer)
        {
            var objects = GameObject.FindObjectsOfType<ECAObject>();
            foreach (ECAObject obj in objects)
            {
                GameObject gameObj = obj.gameObject;
                string objType = GetTypeName(GetECAObjectType(gameObj));

                foreach (Component c in gameObj.GetComponents(typeof(Component)))
                {
                    foreach (RequireComponent require in c.GetType().GetCustomAttributes(
                            typeof(RequireComponent), true))
                    {
                        if(require.m_Type0 == typeof(Behaviour))
                        {
                            var attrs = c.GetType().GetCustomAttributes(typeof(ECARules4AllAttribute), false);
                            if (attrs != null && attrs.Length > 0)
                            {
                                ECARules4AllAttribute attr = (ECARules4AllAttribute)attrs[0];

                                writer.Write("the ");
                                writer.Write(objType);
                                writer.Write(" ");
                                writer.Write(gameObj.name);
                                writer.Write(" has a ");
                                writer.Write(attr.Name);
                                writer.WriteLine(";");
                                writer.Flush();

                            }
                        }
                   
                        
                    }
                }
                    
            }
        }

        public void PrintRule(Rule r, TextWriter writer)
        {
            // write event
            writer.Write("when ");
            // TODO: Check for color in Event
            PrintAction(r.GetEvent(), writer);
            writer.WriteLine();

            if(r.GetCondition() != null)
            {
                writer.Write("if ");
                PrintCondition(r.GetCondition(), writer);
                writer.WriteLine();
            }
            

            // write actions
            writer.Write("then ");

            foreach(Action a in r.GetActions())
            {
                PrintAction(a, writer);
                writer.WriteLine(";");
            }
                writer.Write("\n");
            
            // writer.Write("     ");
            writer.Flush();
            
            
            // writer.WriteLine();
            // writer.WriteLine();
        }

        private void PrintAction(Action a, TextWriter writer)
        {
            // TODO: check if a contains "color" and then swap it [e.g.: ColoredLight (Unity....) changes color to #d62....]

            writer.Write("the ");
            writer.Write(GetTypeName(GetECAObjectType(a.GetSubject())));
            writer.Write(" ");
            writer.Write(a.GetSubject().name);
            writer.Write(" ");
            writer.Write(a.GetActionMethod());
            writer.Write(" ");
            //if(a.GetPreposition() != null)
            //{
            //    writer.Write(a.GetPreposition());
            //    writer.Write(" ");
            //}


            // write object
            if (a.GetObject() != null)
            {
                if (a.GetObject() is GameObject)
                {
                    GameObject gameObject = (GameObject)a.GetObject();
                    writer.Write("the ");
                    writer.Write(GetTypeName(GetECAObjectType(gameObject)));
                    writer.Write(" ");
                    writer.Write(gameObject.name);
                    writer.Write(" ");
                }
                else
                {
                    object val = a.GetObject();
                    PrintValue(val, writer, a.GetModifierValue() != null);
                }
            }

            if (a.GetModifier() != null)
            {
                writer.Write(a.GetModifier());
                writer.Write(" ");
            }

            // Color should be here
            if (a.GetModifierValue() != null)
            {
                if (a.GetModifierValueType() == typeof(ECAColor))
                {
                    ECAColor ecaColor = (ECAColor)a.GetModifierValue();
                    writer.Write("the color ");
                    writer.Write(ecaColor.Name);
                    writer.Write(" ");
                }
                else PrintValue(a.GetModifierValue(), writer, false);   
            }

            if(a.GetMeasureUnit() != null)
            {
                writer.Write(a.GetMeasureUnit());
                writer.Write(" ");
            }
        }

        private void PrintCondition(Condition c, TextWriter writer)
        {
            if(c is CompositeCondition){
                PrintCompositeCondition(c as CompositeCondition, writer);
                
            }

            if(c is SimpleCondition)
            {
                PrintSimpleCondition(c as SimpleCondition, writer);
                
            }
        }

        private void PrintCompositeCondition(CompositeCondition c, TextWriter writer)
        {
            if (c.Op == CompositeCondition.ConditionType.NOT)
            {
                writer.Write("not ");
            }

            if (c.GetParent() != null &&
                ((c.ChildrenCount() > 1 && c.Op != (c.GetParent() as CompositeCondition).Op )||
                 c.GetChild(0) is CompositeCondition ))
            {
                writer.Write("(");
            }



            int count = c.ChildrenCount() - 1;
            foreach(Condition child in c.Children())
            {
                PrintCondition(child, writer);

                if(count > 0)
                {
                   PrintOp(c.Op, writer);
                }
                
                count--;
            }

            if (c.GetParent() != null &&
                ((c.ChildrenCount() > 1 && c.Op != (c.GetParent() as CompositeCondition).Op) ||
                 c.GetChild(0) is CompositeCondition))
            {
                writer.Write(")");
            }
            
        }

        private void PrintValue(object val, TextWriter writer, bool hasModifierValue)
        {
            if (val is Path)
            {
                Path path = val as Path;
                writer.Write("the path ");
                writer.Write(path.Name);
                writer.Write(" ");
            }
            else if (val is Position)
            {
                Position position = val as Position;
                writer.Write("the position ");
                writer.Write(position.Name);
                writer.Write(" ");
            }
            else if (val is ECAColor) 
            {
                writer.Write("#");
                ECAColor tempColor = (ECAColor) val;
                writer.Write(UnityEngine.ColorUtility.ToHtmlStringRGB((tempColor.Color)).ToLowerInvariant());
            }
            else if (val is UnityEngine.Color) 
            {
                writer.Write("#");
                writer.Write(UnityEngine.ColorUtility.ToHtmlStringRGB((UnityEngine.Color)val).ToLowerInvariant());
            }
            else if (val is ECACamera.POV)
            {
                ECACamera.POV pov = (ECACamera.POV) val;
                if (pov == ECACamera.POV.First) writer.Write("1st person ");
                if (pov == ECACamera.POV.Third) writer.Write("3rd person");

            }
            else if (val is float)
            {
                writer.Write(((float)val).ToString("F", CultureInfo.InvariantCulture));
            }
            else if (val is string && !hasModifierValue)
            {
                writer.Write("\"");
                writer.Write((string)val);
                writer.Write("\" ");
            }
            else
            {
                writer.Write(val);
                writer.Write(" ");
            }
        }

        private void PrintOp(CompositeCondition.ConditionType op, TextWriter writer)
        {
            switch (op)
            {
               
                case CompositeCondition.ConditionType.AND:
                    writer.Write("and ");
                    break;

                case CompositeCondition.ConditionType.OR:
                    writer.Write("or ");
                    break;

                default:
                    break;

            }
        }

        private void PrintSimpleCondition(SimpleCondition c, TextWriter writer)
        {
            writer.Write("the ");
            writer.Write(GetTypeName(GetECAObjectType(c.GetSubject())));
            writer.Write(" ");
            writer.Write(c.GetSubject().name);
            writer.Write(" ");
            if(c.GetProperty() != null)
            {
                writer.Write(c.GetProperty());
                writer.Write(" ");
            }
            if(c.GetSymbol() != null)
            {
                writer.Write(c.GetSymbol());
                writer.Write(" ");
            }
            
            if(c.GetValueToCompare() is GameObject)
            {
                GameObject gameObject = (GameObject)c.GetValueToCompare();
                writer.Write("the ");
                writer.Write(GetTypeName(GetECAObjectType(gameObject)));
                writer.Write(" ");
                writer.Write(gameObject.name);
                writer.Write(" ");
            }
            /*if(c.GetValueToCompare() is Color)
            {
                ECAColor ecaColor = new ECAColor((Color)c.GetValueToCompare());
                writer.Write("the color ");
                writer.Write(ecaColor.Name);
                writer.Write(" ");
            }*/
            else
            {
                PrintValue(c.GetValueToCompare(), writer, false);
            }
            writer.Write(" ");
        }
    }
}
