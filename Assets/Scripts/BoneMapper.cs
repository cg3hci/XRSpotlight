using System.Collections.Generic;
using UnityEngine;

public class BoneMapper : MonoBehaviour
{
    public SkinnedMeshRenderer characterRenderer;
    public bool allowNonMatchingSkeletons = true;
    private SkinnedMeshRenderer[] clothMeshRenderers;
    private Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();

    // Start is called before the first frame update
    void Start()
    {
        clothMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        if(clothMeshRenderers.Length > 0)
        {
            MapBones();
        }
        
    }

    private void MapBones()
    {
        //Create Map
        char[] splitChars = {'.',',',':',';'};
        foreach (Transform bone in characterRenderer.bones)
        {
            //Remove eventual prefixes
            string[] split = bone.gameObject.name.Split(splitChars);
            boneMap[split[split.Length-1]] = bone;
        }

        //Do the actual mapping if it is possibile
        foreach (SkinnedMeshRenderer clothMeshRenderer in clothMeshRenderers)
        {
            bool mappingIsRight = true;
         
            Transform[] newBones = new Transform[clothMeshRenderer.bones.Length];
            for (int i = 0; i < clothMeshRenderer.bones.Length; ++i)
            {
                GameObject bone = clothMeshRenderer.bones[i].gameObject;
                string[] split = bone.gameObject.name.Split(splitChars);
                
                if (!boneMap.TryGetValue(split[split.Length - 1], out newBones[i]))
                {
                    Debug.Log("The bone "+bone.name+" doesn't exist in the target skeleton.");
                    mappingIsRight = false;
                }
            }
            if (mappingIsRight || (!mappingIsRight && allowNonMatchingSkeletons))
            {
                clothMeshRenderer.bones = newBones;
                clothMeshRenderer.rootBone = characterRenderer.rootBone;
            }

        }
        
    }
}
