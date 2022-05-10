using System;
namespace EcaRules
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ECARules4AllAttribute : System.Attribute
    {
        public string Name { get; internal set; }
        public ECARules4AllAttribute(string name)
        {
            this.Name = name;
        }
    }
}
