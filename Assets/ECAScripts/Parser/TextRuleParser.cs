using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using ECAScripts.Utils;
using UnityEngine;
using System.Globalization;

namespace EcaRules
{
    public class TextRuleParser : IEcaRuleParser, IECARulesParserListener, IAntlrErrorListener<IToken>
    {
        private string _objectName;
        private Type _objectType;
        private string _objectTypeName;
        private Position _position;
        private Path _path;
        private ECAColor _color;
        private float _floatLiteral;
        private ECABoolean _boolLiteral;
        private int? _intLiteral;
        private string _stringLiteral;
        private string _timeLiteral;
        private GameObject _reference;
        private EcaAction _event, _tmp;
        private CompositeEcaCondition _c;
        private SimpleEcaCondition simpleEca;
        private List<EcaAction> _actions;
        private string _op;
        private object _value;


        public List<Pair<Type, GameObject>> ObjectDeclarations { get; internal set; }
        public Dictionary<string, Position> PositionDeclarations { get; internal set; }
        public Dictionary<string, ECAColor> ColorDeclarations { get; internal set; } 
        public Dictionary<string, Path> PathDeclarations { get; internal set; }

        public TextRuleParser()
        {
            _color = new ECAColor();
            ObjectDeclarations = new ArrayList<Pair<Type, GameObject>>();
            PositionDeclarations = new Dictionary<string, Position>();
            PathDeclarations = new Dictionary<string, Path>();
            ColorDeclarations = new Dictionary<string, ECAColor>();
            _actions = new ArrayList<EcaAction>();
            _boolLiteral = new ECABoolean(ECABoolean.BoolType.FALSE);
        }

        public void ReadRuleFile(string path)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            using (StreamReader fileStream = new StreamReader(path))
            {
                AntlrInputStream inputStream = new AntlrInputStream(fileStream);

                ECARulesLexer lexer = new ECARulesLexer(inputStream);
                CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
                ECARulesParser parser = new ECARulesParser(commonTokenStream);

                parser.AddParseListener(this);
                parser.AddErrorListener(this);

                parser.BuildParseTree = true;
                IParseTree parseTree = parser.program();

            }
        }

        private void ReadPosition(ECARulesParser.PositionLiteralContext c)
        {
            _position = new Position();
            _position.x = float.Parse(c.floatLiteral()[0].GetText(), System.Globalization.CultureInfo.InvariantCulture);
            _position.y = float.Parse(c.floatLiteral()[1].GetText(), System.Globalization.CultureInfo.InvariantCulture);
            _position.z = float.Parse(c.floatLiteral()[2].GetText(), System.Globalization.CultureInfo.InvariantCulture);
        }

        private GameObject GetReference()
        {
            GameObject obj; 
            obj = GameObject.Find(_objectName);
            if (obj != null)
            {
                _objectType = null;
                foreach (Component c in obj.GetComponents(typeof(Component)))
                {
                    var attrs = c.GetType().GetCustomAttributes(typeof(ECARules4AllAttribute), false);
                    if (attrs != null && attrs.Length > 0)
                    {
                        ECARules4AllAttribute attr = (ECARules4AllAttribute)attrs[0];
                        if (attr.Name.Equals(_objectTypeName))
                        {
                            _objectType = c.GetType();
                        }
                    }


                }

                if (_objectType == null)
                {
                    throw new ArgumentException(
                    String.Format("The object {0} does not contain a {1} component",
                    _objectName,
                    _objectTypeName));
                }
            }
            else
            {
                throw new ArgumentException(
                    String.Format("Cannot find object with name {0}",
                    _objectName));
            }

            return obj;
        }

        private SimpleEcaCondition ReadBaseCondition(ECARulesParser.BaseConditionContext context)
        {
            SimpleEcaCondition ecaCondition = null; 
            _objectName = context.IDENTIFIER().GetText();
            _objectTypeName = context.type().IDENTIFIER().GetText();
            _reference = GetReference();

            string property = null;
            if (context.property().IDENTIFIER() != null)
                property = context.property().IDENTIFIER().GetText();
            if(context.property().COLOR() != null)
            {
                property = context.property().COLOR().GetText();
                
            }
            if (context.property().POSITION() != null)
            {
                property = context.property().POSITION().GetText();

            }
            if (context.property().PATH() != null)
            {
                property = context.property().PATH().GetText();

            }
            if (context.@object() != null)
            {
                _objectName = context.@object().IDENTIFIER().GetText();
                _objectTypeName = context.@object().type().IDENTIFIER().GetText();
                var _reference2 = GetReference();
                ecaCondition = new SimpleEcaCondition(_reference, property, _op, _reference2);
            }
            if (context.value() != null) ecaCondition = new SimpleEcaCondition(_reference, property, _op, _value);
            _op = null;
            return ecaCondition;
        }

        public void Reset()
        {
            _c = null;
            ObjectDeclarations.Clear();
            PositionDeclarations.Clear();
        }


        public void VisitTerminal(ITerminalNode node)
        {
            switch (node.Symbol.Type)
            {
                case ECARulesLexer.TIME_LITERAL:
                    _timeLiteral = node.GetText();
                    break;

                case ECARulesLexer.DECIMAL_LITERAL:
                    _intLiteral = int.Parse(node.GetText());
                    break;

                case ECARulesLexer.FLOAT_LITERAL:
                    _floatLiteral = float.Parse(node.GetText(), System.Globalization.CultureInfo.InvariantCulture);
                    break;

                case ECARulesLexer.STRING_LITERAL:
                    _stringLiteral = node.GetText().Substring(1, node.GetText().Length -2);
                    break;

                case ECARulesLexer.BOOL_LITERAL:
                    if (node.GetText().Equals("true"))
                        _boolLiteral.Assign(ECABoolean.BoolType.TRUE);
                    else
                        _boolLiteral.Assign(ECABoolean.BoolType.FALSE);
                    break;

                case ECARulesLexer.BOOL_YES_NO:
                    if (node.GetText().Equals("yes"))
                        _boolLiteral.Assign(ECABoolean.BoolType.YES);
                    else
                        _boolLiteral.Assign(ECABoolean.BoolType.NO);

                    break;

                case ECARulesLexer.COLOR_LITERAL:
                    Color tempColor = new Color();
                    UnityEngine.ColorUtility.TryParseHtmlString(node.GetText(), out tempColor);
                    _color.Color = tempColor;
                    break;

                case ECARulesLexer.ON:
                    _boolLiteral.Assign(ECABoolean.BoolType.ON);
                    break;

                case ECARulesLexer.OFF:
                    _boolLiteral.Assign(ECABoolean.BoolType.OFF);
                    break;

            }
        }

        public void VisitErrorNode(IErrorNode node)
        {
            throw new ArgumentException("Error in parsing rules");
        }

        public void EnterAction([NotNull] ECARulesParser.ActionContext context)
        {
            _tmp = new EcaAction();
        }

        public void EnterAlias([NotNull] ECARulesParser.AliasContext context)
        {
            
        }

        public void EnterAngle([NotNull] ECARulesParser.AngleContext context)
        {
           
        }

        public void EnterBaseCondition([NotNull] ECARulesParser.BaseConditionContext context)
        {
            
        }

        public void EnterBehaviourDeclaration([NotNull] ECARulesParser.BehaviourDeclarationContext context)
        {
            
        }

        public void EnterColor([NotNull] ECARulesParser.ColorContext context)
        {
            
        }

        public void EnterColorDeclaration([NotNull] ECARulesParser.ColorDeclarationContext context)
        {
            
        }

        public void EnterCondition([NotNull] ECARulesParser.ConditionContext context)
        {
            CompositeEcaCondition compositeEca = new CompositeEcaCondition();
            if(_c != null)
            {
                _c.AddChild(compositeEca);
                

            }
            _c = compositeEca;
        }

        public void EnterDeclaration([NotNull] ECARulesParser.DeclarationContext context)
        {
            
        }

        public void EnterEcarule([NotNull] ECARulesParser.EcaruleContext context)
        {
            _tmp = null;
            _c = new CompositeEcaCondition();
            //_conditionTree = new ConditionTree(;
            _actions = new List<EcaAction>();
        }

        public void EnterEveryRule(ParserRuleContext ctx)
        {
            
        }

        public void EnterFloatLiteral([NotNull] ECARulesParser.FloatLiteralContext context)
        {
            
        }

        public void EnterObject([NotNull] ECARulesParser.ObjectContext context)
        {
            
        }

        public void EnterObjectDeclaration([NotNull] ECARulesParser.ObjectDeclarationContext context)
        {
            
        }

        public void EnterOperator([NotNull] ECARulesParser.OperatorContext context)
        {
            
        }

        public void EnterPath([NotNull] ECARulesParser.PathContext context)
        {
            
        }

        public void EnterPathDeclaration([NotNull] ECARulesParser.PathDeclarationContext context)
        {
            _path = null;
        }

        public void EnterPosition([NotNull] ECARulesParser.PositionContext context)
        {
            
        }

        public void EnterPositionDeclaration([NotNull] ECARulesParser.PositionDeclarationContext context)
        {
            _position = null;
        }

        public void EnterPositionLiteral([NotNull] ECARulesParser.PositionLiteralContext context)
        {
            
        }

        public void EnterPreposition([NotNull] ECARulesParser.PrepositionContext context)
        {
            
        }

        public void EnterProgram([NotNull] ECARulesParser.ProgramContext context)
        {
            this.Reset();
        }

        public void EnterProperty([NotNull] ECARulesParser.PropertyContext context)
        {
            
        }

        public void EnterSubject([NotNull] ECARulesParser.SubjectContext context)
        {
            
        }

        public void EnterType([NotNull] ECARulesParser.TypeContext context)
        {
            

        }

        public void EnterValue([NotNull] ECARulesParser.ValueContext context)
        {
            
        }

        public void EnterVerb([NotNull] ECARulesParser.VerbContext context)
        {
            
        }

        public void ExitAction([NotNull] ECARulesParser.ActionContext context)
        {
            if(context.property() != null)
            {
                // switch the object value with the property
                _tmp.SetModifierValue(_value);
                _tmp.SetObject(context.property().GetText());
            }
            else
            {
                if (context.@object() != null) _tmp.SetObject(_reference);
                if (context.value() != null) _tmp.SetObject(_value);
            }

            if(context.preposition() != null)
            {
                _tmp.SetActionMethod(context.verb().GetText() + " " + context.preposition().GetText());
            }


            if(context.modifier() != null)
            {
                _tmp.SetModifier(context.modifier().preposition().GetText());
            }

            if (context.MEASURE_UNIT() != null)
            {
                _tmp.SetMeasureUnit(context.MEASURE_UNIT().GetText());
            }


            if(!_tmp.IsValid())
            {
                throw new ArgumentException(
                    String.Format("The action {0} is invalid, please check its definition",
                    _tmp.ToString()));
            }

            // distringuishes between events and actions
            ECARulesParser.EcaruleContext parent = (ECARulesParser.EcaruleContext)context.Parent;
            if (parent.THEN() == null)
            {
                // event
                _event = _tmp;
            }
            else
            {
                _actions.Add(_tmp);
                
            }
            _tmp = new EcaAction();

        }

        public void ExitAlias([NotNull] ECARulesParser.AliasContext context)
        {
            // TODO is there any alias implementation?
            throw new System.NotImplementedException();
        }

        public void ExitAngle([NotNull] ECARulesParser.AngleContext context)
        {
            // TODO how to manage the axes?
            _floatLiteral = float.Parse(context.floatLiteral().GetText(), System.Globalization.CultureInfo.InvariantCulture);
        }

        public void ExitBaseCondition([NotNull] ECARulesParser.BaseConditionContext context)
        {
            simpleEca = ReadBaseCondition(context);
            _c.AddChild(simpleEca);
        }

        public void ExitBehaviourDeclaration([NotNull] ECARulesParser.BehaviourDeclarationContext context)
        {
            _objectName = context.IDENTIFIER().GetText();
            _objectTypeName = context.type(0).IDENTIFIER().GetText();
            _reference = GetReference();
            string behaviour = context.type(1).IDENTIFIER().GetText();

            bool found = false;
            foreach(Component c in _reference.GetComponents(typeof(Component)))
            {
                if(Attribute.IsDefined(c.GetType(), typeof(ECARules4AllAttribute)))
                {
                    var attrs = c.GetType().GetCustomAttributes(typeof(ECARules4AllAttribute), false);
                    if (attrs != null && attrs.Length > 0)
                    {
                        ECARules4AllAttribute attr = (ECARules4AllAttribute)attrs[0];
                        if (attr.Name.Equals(behaviour)) 
                        {
                            foreach (RequireComponent require in c.GetType().GetCustomAttributes(
                            typeof(RequireComponent), true))
                            {
                                if(require.m_Type0 == typeof(Behaviour))
                                {
                                    found = true;
                                    break;
                                }
                                
                            }

                            if (!found)
                            {
                                throw new ArgumentException(
                                    String.Format("The {0} type must be a Behaviour"),
                                    behaviour);
                            }
                            break;   
                        }
                    }
                }
            }

            if (!found)
            {
                throw new ArgumentException(
                    String.Format("The object {0} does not contain a {1} component",
                    _objectName,
                     behaviour));
            }
        }

        public void ExitColor([NotNull] ECARulesParser.ColorContext context)
        {
            if (ColorDeclarations.ContainsKey(context.IDENTIFIER().GetText()))
            {
                _color = ColorDeclarations[context.IDENTIFIER().GetText()];
            }
            else
            {
                throw new ArgumentException(String.Format(
                    "The color {0} is not defined", context.IDENTIFIER().GetText()));
            }
        }

        public void ExitColorDeclaration([NotNull] ECARulesParser.ColorDeclarationContext context)
        {
            Color tempColor = new Color();
            UnityEngine.ColorUtility.TryParseHtmlString(context.COLOR_LITERAL().GetText(), out tempColor);
            _color.Color = tempColor;
            ColorDeclarations.Add(context.IDENTIFIER().GetText(), _color);
        }

        public void ExitCondition([NotNull] ECARulesParser.ConditionContext context)
        {
            if (context.NOT() != null)
                _c.Op = CompositeEcaCondition.ConditionType.NOT;
            
            if (_c.parent != null)
            {
                _c = (CompositeEcaCondition)_c.parent;

            }

            if (context.AND().Length > 0)
                _c.Op = CompositeEcaCondition.ConditionType.AND;
            if (context.OR().Length > 0)
                _c.Op = CompositeEcaCondition.ConditionType.OR;


        }

        public void ExitDeclaration([NotNull] ECARulesParser.DeclarationContext context)
        {
            
        }

        public void ExitEcarule([NotNull] ECARulesParser.EcaruleContext context)
        {
            // remove the composite condition if not needed
            EcaCondition c = SimplifyCondition(_c);
            
            EcaRule ecaRule = new EcaRule(_event, c, _actions);
            EcaRuleEngine.GetInstance().Add(ecaRule);
        }

        private EcaCondition SimplifyCondition(CompositeEcaCondition c)
        {
            if (c == null)
                return null;

            CompactCompositeCondition(c);

            CompositeEcaCondition cursor = c;
            while (cursor != null && c.ChildrenCount() == 1 && cursor.Op != CompositeEcaCondition.ConditionType.NOT)
            {
                EcaCondition child = cursor.GetChild(0);
                child.parent = null;
                if (child is CompositeEcaCondition) cursor = child as CompositeEcaCondition;
                else return child;
            }

            if(cursor.ChildrenCount() == 0)
            {
                return null;
            }

            return cursor;
        }

        private void CompactCompositeCondition(CompositeEcaCondition c)
        {
            if (c == null) return;

            if (c.ChildrenCount() == 2 && c.GetChild(1) is CompositeEcaCondition)
            {
                CompositeEcaCondition right = c.GetChild(1) as CompositeEcaCondition;
                if (c.Op == right.Op)
                {
                    RemoveCompositeConditionLevel(c, right, 1);
                }
            }

            for (int i = 0; i < c.ChildrenCount(); i++)
            {
                if(c.GetChild(i) is CompositeEcaCondition)
                {
                    CompositeEcaCondition child = c.GetChild(i) as CompositeEcaCondition;
                    if(child.ChildrenCount() == 1 && child.Op != CompositeEcaCondition.ConditionType.NOT)
                    {
                        RemoveCompositeConditionLevel(c, child, i);
                        i--;
                    }
                }
                
            }
            
            

            for(int i = 0; i< c.ChildrenCount(); i++)
            {
                if(c.GetChild(i) is CompositeEcaCondition)
                {
                    CompactCompositeCondition(c.GetChild(i) as CompositeEcaCondition);
                }
            }
        }

        private void RemoveCompositeConditionLevel(CompositeEcaCondition c, CompositeEcaCondition child, int index)
        {
            c.RemoveChild(child);
            for (int i = child.ChildrenCount() -1; i >=0; i--)
            {
                c.InsertChild(index, child.GetChild(i));
            }
        }

        public void ExitEveryRule(ParserRuleContext ctx)
        {
            
        }

        public void ExitFloatLiteral([NotNull] ECARulesParser.FloatLiteralContext context)
        {
            if (context.DECIMAL_LITERAL() != null)
                _intLiteral = int.Parse(context.GetText(), CultureInfo.InvariantCulture);
            if(context.FLOAT_LITERAL() != null)
                _floatLiteral = float.Parse(context.GetText(), CultureInfo.InvariantCulture);
        }

        public void ExitObject([NotNull] ECARulesParser.ObjectContext context)
        {
            _objectName = context.IDENTIFIER().GetText();
            _reference = GetReference();
        }

        public void ExitObjectDeclaration([NotNull] ECARulesParser.ObjectDeclarationContext context)
        {
            _objectName = context.IDENTIFIER().GetText();
            _reference = GetReference();
            this.ObjectDeclarations.Add(new Pair<Type, GameObject>(_objectType, _reference));
        }

        public void ExitOperator([NotNull] ECARulesParser.OperatorContext context)
        {
            if(context.GT() != null) _op = ">";
            if (context.LT() != null) _op = "<";
            if (context.EQUAL() != null) _op = "=";
            if (context.LE() != null) _op = "<=";
            if (context.GE() != null) _op = ">=";
            if (context.NOTEQUAL() != null) _op = "!=";
            if (context.IS() != null) _op = "is";
        }

        public void ExitPath([NotNull] ECARulesParser.PathContext context)
        {
            if (PathDeclarations.ContainsKey(context.IDENTIFIER().GetText()))
            {
                _path = PathDeclarations[context.IDENTIFIER().GetText()];
            }
            else
            {
                throw new ArgumentException(String.Format(
                    "The path {0} is not defined", context.IDENTIFIER().GetText()));
            }
        }

        public void ExitPathDeclaration([NotNull] ECARulesParser.PathDeclarationContext context)
        {
            _path = new Path(new List<Position>());
            _path.Name = context.IDENTIFIER().GetText();
            foreach (ECARulesParser.PositionLiteralContext c in context.positionLiteral())
            {
                ReadPosition(c);
                _path.Points.Add(_position);

            }

            PathDeclarations.Add(context.IDENTIFIER().GetText(), _path);
        }

        public void ExitPosition([NotNull] ECARulesParser.PositionContext context)
        {
            if (PositionDeclarations.ContainsKey(context.IDENTIFIER().GetText()))
            {
                _position = PositionDeclarations[context.IDENTIFIER().GetText()];
            }
            else
            {
                throw new ArgumentException(String.Format(
                    "The position {0} is not defined", context.IDENTIFIER().GetText()));
            }

        }

        public void ExitPositionDeclaration([NotNull] ECARulesParser.PositionDeclarationContext context)
        {
            ReadPosition(context.positionLiteral());
            _position.Name = context.IDENTIFIER().GetText();
            this.PositionDeclarations.Add(context.IDENTIFIER().GetText(), _position);
        }

        public void ExitPositionLiteral([NotNull] ECARulesParser.PositionLiteralContext context)
        {
            if (context.floatLiteral().Length == 3)
            {
                ReadPosition(context);
            }
        }

        public void ExitPreposition([NotNull] ECARulesParser.PrepositionContext context)
        {
            
        }

        public void ExitProgram([NotNull] ECARulesParser.ProgramContext context)
        {
            
        }

        public void ExitProperty([NotNull] ECARulesParser.PropertyContext context)
        {
            if(context.IDENTIFIER() != null)
                _tmp.SetModifier(context.IDENTIFIER().GetText());
            if(context.COLOR() != null)
                _tmp.SetModifier("color");
            if (context.PATH() != null)
                _tmp.SetModifier("path");
            if (context.POSITION() != null)
                _tmp.SetModifier("position");
        }

        public void ExitSubject([NotNull] ECARulesParser.SubjectContext context)
        {
            _objectName = context.IDENTIFIER().GetText();
            _reference = GetReference();
            _tmp.SetSubject(_reference);
        }

        public void ExitType([NotNull] ECARulesParser.TypeContext context)
        {
            _objectTypeName = context.IDENTIFIER().GetText();
        }

        public void ExitValue([NotNull] ECARulesParser.ValueContext context)
        {
            if (context.floatLiteral() != null)
            {
                if (context.floatLiteral().FLOAT_LITERAL() != null) _value = _floatLiteral;
                if (context.floatLiteral().DECIMAL_LITERAL() != null) _value = _intLiteral;
            }
            if (context.position() != null) _value = _position;
            if (context.path() != null)  _value =  _path;
            if (context.color() != null) _value = _color;
            if (context.POV_LITERAL() != null)
            {
                if (context.POV_LITERAL().GetText().Contains("1st"))
                {
                    _value = ECACamera.POV.First;
                }
                if (context.POV_LITERAL().GetText().Contains("3rd"))
                {
                    _value = ECACamera.POV.Third;
                }

            }


            if (context.BOOL_LITERAL() != null) _value = new ECABoolean(_boolLiteral.choice);
            if (context.BOOL_YES_NO() != null) _value = new ECABoolean(_boolLiteral.choice);
            if (context.ON() != null) _value = new ECABoolean(ECABoolean.BoolType.ON);
            if (context.OFF() != null) _value = new ECABoolean(ECABoolean.BoolType.OFF);
            if (context.COLOR_LITERAL() != null) _value = _color;
            if (context.TIME_LITERAL() != null) _value = _timeLiteral;
            if (context.STRING_LITERAL() != null) _value = _stringLiteral;
            // TODO add angle management 
            if (context.angle() != null) _value = null;
        }

        public void ExitVerb([NotNull] ECARulesParser.VerbContext context)
        {
            _tmp.SetActionMethod(context.GetText());
        }

        public void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new ArgumentException(
                String.Format("The input file contains a syntax error. Please read the following message\r\n {0}",
                    e.Message));
        }

        public void EnterModifier([NotNull] ECARulesParser.ModifierContext context)
        {
            
        }

        public void ExitModifier([NotNull] ECARulesParser.ModifierContext context)
        {
            
        }
    }


    
}