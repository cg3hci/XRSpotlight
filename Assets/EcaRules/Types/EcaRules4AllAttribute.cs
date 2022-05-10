using System;
namespace EcaRules
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class EcaRules4AllAttribute : System.Attribute
    {
        public string Name { get; internal set; }
        public EcaRules4AllAttribute(string name)
        {
            this.Name = name;
        }
    }
}
