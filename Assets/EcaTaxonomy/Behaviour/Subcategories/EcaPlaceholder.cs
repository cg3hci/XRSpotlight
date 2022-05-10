using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using EcaRules;
using ECAScripts.Utils;
using UnityEditor;

/// <summary>
/// <b>Placeholder</b> is a <see cref="EcaBehaviour"/> that is used to represent a placeholder in the scene. It will be
/// used by the End User Developers in order to import and use custom mesh models.
/// </summary>
[EcaRules4All("placeholder")]
[RequireComponent(typeof(EcaBehaviour), typeof(MeshFilter), typeof(MeshCollider))]
[DisallowMultipleComponent]
public class EcaPlaceholder : MonoBehaviour
{
    /// <summary>
    /// <b>newMesh</b> is the mesh model that the object will use.
    /// </summary>
    [EcaStateVariable("mesh", EcaRules4AllType.Identifier)]
    public Mesh newMesh;

    /// <summary>
    /// <b>Changes</b> sets the new mesh model that the object will use.
    /// </summary>
    /// <param name="meshName">The path of the mesh in the user-accessible mesh folder</param>
    [EcaAction(typeof(EcaPlaceholder), "changes", "mesh", "to", typeof(string))]
    public void Changes(string meshName)
    {
        newMesh = new Mesh();
        meshName = Regex.Replace(meshName, "/[^a-zA-Z ]/", "");
        newMesh = Resources.Load<Mesh>("Inventory\\Meshes\\" + meshName);
        transform.localScale = Vector3.one;
        gameObject.GetComponent<MeshFilter>().mesh = newMesh;
        gameObject.GetComponent<MeshCollider>().sharedMesh = newMesh;
    }
}