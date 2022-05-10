using Antlr4.Runtime.Atn;
using UnityEngine;

namespace ECAScripts.Utils
{
    [System.Serializable]
    public class ECABoolean
    {
        public static readonly ECABoolean YES = new ECABoolean(BoolType.YES);
        public static readonly ECABoolean ON = new ECABoolean(BoolType.ON);
        public static readonly ECABoolean TRUE = new ECABoolean(BoolType.TRUE);
        public static readonly ECABoolean NO = new ECABoolean(BoolType.NO);
        public static readonly ECABoolean OFF = new ECABoolean(BoolType.OFF);
        public static readonly ECABoolean FALSE = new ECABoolean(BoolType.FALSE);


        public enum BoolType
        {
            YES,
            ON,
            TRUE,
            NO,
            OFF,
            FALSE
        }

        public BoolType choice;

        public ECABoolean(BoolType type)
        {
            choice = type;
        }

        public ECABoolean(bool type)
        {
            choice = type ? BoolType.TRUE : BoolType.FALSE;
        }

        public BoolType GetBoolType()
        {
            return choice;
        }

        public void Assign(ECABoolean boolean)
        {
            choice = boolean.choice;
        }

        public void Assign(BoolType boolean)
        {
            choice = boolean;
        }

        public static bool operator ==(ECABoolean one, ECABoolean two)
        {
            return (one.choice <= BoolType.TRUE) == (two.choice <= BoolType.TRUE);
        }

        public static bool operator !=(ECABoolean one, ECABoolean two)
        {
            return (one.choice <= BoolType.TRUE) != (two.choice <= BoolType.TRUE);
        }

        public static bool operator ==(ECABoolean one, bool two)
        {
            return (one.choice <= BoolType.TRUE) == two;
        }

        public static bool operator !=(ECABoolean one, bool two)
        {
            return (one.choice <= BoolType.TRUE) != two;
        }

        public static bool operator ==(bool one, ECABoolean two)
        {
            return (two.choice <= BoolType.TRUE) == one;
        }

        public static bool operator !=(bool one, ECABoolean two)
        {
            return (two.choice <= BoolType.TRUE) != one;
        }

        //This line of code lets us check if the value is true or not just like a classic boolean 
        public static implicit operator bool(ECABoolean one) => one.choice <= BoolType.TRUE;

        public override string ToString()
        {
            return choice.ToString().ToLower();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ECABoolean)) return false;
            return this.choice == ((ECABoolean) obj).choice;
        }
    }

    [System.Serializable]
    public class YesNo : ECABoolean
    {
        public YesNo(BoolType type) : base(type)
        {
        }

        public YesNo(bool type) : base(type)
        {
        }
    }

    [System.Serializable]
    public class OnOff : ECABoolean
    {
        public OnOff(BoolType type) : base(type)
        {
        }

        public OnOff(bool type) : base(type)
        {
        }
    }

    [System.Serializable]
    public class TrueFalse : ECABoolean
    {
        public TrueFalse(BoolType type) : base(type)
        {
        }

        public TrueFalse(bool type) : base(type)
        {
        }
    }
}