using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

namespace EcaRules
{
    public class EcaPosition
    {
        public string Name { get; set; }

        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        
        public EcaPosition()
        {
            x = 0.0f;
            y = 0.0f;
            z = 0.0f;
        }
        public EcaPosition(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        public void Assign(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        public void Assign(EcaPosition p)
        {
            this.x = p.x;
            this.y = p.y;
            this.z = p.z;
        }
        
        public void Assign(Vector3 p)
        {
            this.x = p.x;
            this.y = p.y;
            this.z = p.z;
        }

        public Vector3 GetPosition()
        {
            return new Vector3(x, y, z);
        }

        public override string ToString()
        {
            return x + ", " + y + ", " + z;
        }

        public override bool Equals(object obj)
        {
            if(obj is EcaPosition)
            {
                EcaPosition p = obj as EcaPosition;
                return this.x == p.x && this.y == p.y && this.z == p.z;
            }
            else
            {
                return false;
            }
        }
    }

    public class EcaRotation
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        
        public void Assign(EcaRotation r)
        {
            this.x = r.x;
            this.y = r.y;
            this.z = r.z;
        }
        
        public void Assign(Vector3 r)
        {
            this.x = r.x;
            this.y = r.y;
            this.z = r.z;
        }
        
        public void Assign(Quaternion r)
        {
            this.x = r.eulerAngles.x;
            this.y = r.eulerAngles.y;
            this.z = r.eulerAngles.z;
        }
    }

    public class EcaPath
    {
        public string Name { get; set; }

        public List<EcaPosition> Points { get; set; }

        public EcaPath(List<EcaPosition> list)
        {
            Name = "Path";
            Points = list;
        }
        
        public EcaPath(string name, List<EcaPosition> list)
        {
            Name = name;
            Points = list;
        }

        public override bool Equals(object obj)
        {
            if(obj is EcaPath)
            {
                EcaPath path = obj as EcaPath;

                if(this.Points.Count != path.Points.Count)
                {
                    return false;
                }

                for(int i = 0; i < this.Points.Count; i++)
                {
                    if (!this.Points[i].Equals(path.Points[i]))
                        return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
