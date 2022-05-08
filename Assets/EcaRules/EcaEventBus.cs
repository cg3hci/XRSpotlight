using System.Collections.Generic;

namespace EcaRules
{
    ///<summary>
    ///Class that manages all the messages sent inside the system
    ///</summary>
    public class EcaEventBus
    {
        public static EcaEventBus singleton;
        private Dictionary<EcaAction, EcaActionListener> eventQueue = new Dictionary<EcaAction, EcaActionListener>();

        ///<summary>
        ///<c>GetInstance</c> returns an Instance of the EventBus. 
        ///<para/>
        ///<strong>Returns:</strong> The Singleton instance for the Event Bus
        ///</summary>
        public static EcaEventBus GetInstance()
        {
            if (singleton == null)
            {
                singleton = new EcaEventBus();
            }

            return singleton;
        }

        private EcaEventBus()
        {
        }

        ///<summary>
        ///<c>Publish</c> sends an <seealso cref="EcaAction"/> inside the EventBus, ready to be caught by any <seealso cref="EcaActionListener"/> who registered for catching it. 
        ///</summary>
        public void Publish(EcaAction a)
        {
            foreach (KeyValuePair<EcaAction, EcaActionListener> saved in eventQueue)
            {
                if (saved.Key == a)
                {
                    saved.Value.ActionPerformed(a);
                }
            }
        }

        ///<summary>
        ///<c>Subscribe</c> registers an <seealso cref="EcaActionListener"/> with an <seealso cref="EcaAction"/> so it can be notified in case the action gets executed. 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="a"/>: The action that the EventBus have to notify in case of execution</description></item>
        ///<item><description><paramref name="l"/>: The listener that will handle the notification</description></item>
        ///</list>
        ///</summary>
        public void Subscribe(EcaAction a, EcaActionListener l)
        {
            //This function checks whether if a key is already inside the EventQueue, if it's not it's added to the list
            //It's done like this because the equivalent library function doesn't work for some reason :)
            //The check is also in place because even if a Dictionary SHOULDN'T have more than one key with the same value
            //the system still adds it, duplicating it and creating unwanted errors
            var existenceCheck = true;
            foreach (var pair in eventQueue)
            {
                if (pair.Key == a)
                {
                    existenceCheck = false;
                }
            }

            if (existenceCheck)
                eventQueue.Add(a, l);
        }

        ///<summary>
        ///<c>Unsubscribe</c> unregisters an <seealso cref="EcaAction"/> from the system, if needed. 
        ///<para/>
        ///<strong>Parameters:</strong> 
        ///<list type="bullet">
        ///<item><description><paramref name="a"/>: The action to be removed</description></item>
        ///</list>
        ///</summary>
        public void Unsubscribe(EcaAction a)
        {
            eventQueue.Remove(a);
        }
    }
}