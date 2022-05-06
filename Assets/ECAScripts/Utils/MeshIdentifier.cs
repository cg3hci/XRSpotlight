using UnityEngine;

namespace ECAScripts.Utils
{    [System.Serializable]
    public class MeshIdentifier
    {
        public string name;
        public Mesh mesh;

        public MeshIdentifier(string name, Mesh mesh)
        {
            this.name = name;
            this.mesh = mesh;
        }
    }
}