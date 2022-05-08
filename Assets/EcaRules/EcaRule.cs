using System;
using System.Collections.Generic;


namespace EcaRules
{
    ///<summary>
    ///<c>Rule</c> describes a single rule, consisting of a "when" clause, a "then" list of actions and the optional "if" condition statements.
    ///<para/>
    ///See also: <seealso cref="EcaAction"/>, <seealso cref="EcaCondition"/>
    ///</summary>
    [Serializable]
    public class EcaRule
    {
        
        private EcaAction Event;
        private EcaCondition ecaConditions;
        private List<EcaAction> actions;

        ///<summary>
        ///Standard Declaration
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="when"/>: The "when" statement (Type: <seealso cref="EcaAction"/>)</description></item>
        ///<item><description><paramref name="ifStatement"/>: The "if" statement(s) (Type: List &lt;<seealso cref="EcaCondition"/>&gt;)</description></item>
        ///<item><description><paramref name="listOfActions"/>: The "then" list of actions(Type: List &lt;<seealso cref="EcaAction"/>&gt;)</description></item>
        ///</list>
        ///<para/>
        ///</summary>
        public EcaRule(EcaAction when, EcaCondition ifStatements, List<EcaAction> listOfActions)
        {
            Event = when;
            ecaConditions = ifStatements;
            actions = listOfActions;
        }


        ///<summary>
        ///Declaration without the if Statement 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="when"/>: The "when" statement (Type: <seealso cref="EcaAction"/>)</description></item>
        ///<item><description><paramref name="listOfActions"/>: The "then" list of actions(Type: List &lt;<seealso cref="EcaAction"/>&gt;)</description></item>
        ///</list>
        ///<para/>
        ///</summary>
        public EcaRule(EcaAction when, List<EcaAction> listOfActions)
        {
            Event = when;
            ecaConditions = null;
            actions = listOfActions;
        }

        ///<summary>
        ///<c>GetEvent</c> returns the action in the "when" clause. 
        ///<para/>
        ///<strong>Returns:</strong> The <seealso cref="EcaAction"/> associated with the rule
        ///</summary>
        public EcaAction GetEvent()
        {
            return Event;
        }

        ///<summary>
        ///<c>GetConditions</c> returns the condition(s) in the "if" clause. 
        ///<para/>
        ///<strong>Returns:</strong> The <seealso cref="EcaCondition"/>(s) associated with the rule
        ///</summary>
        public EcaCondition GetCondition()
        {
            return ecaConditions;
        }

        ///<summary>
        ///<c>GetActions</c> returns the list of actions in the "then" clause. 
        ///<para/>
        ///<strong>Returns:</strong> The list of <seealso cref="EcaAction"/> associated with the rule
        ///</summary>
        public List<EcaAction> GetActions()
        {
            return actions;
        }
    }
}