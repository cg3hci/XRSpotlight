using UnityEngine;

namespace EcaRules
{
    /// <summary>
    /// <b>Environment</b> objects represent inanimated elements that configure the scene, like buildings, vegetation etc...
    /// Or elements that decorate or furnish a room.
    /// </summary>
    [EcaRules4All("environment")]
    [RequireComponent(typeof(ECAObject))]
    [DisallowMultipleComponent]
    public class EcaEnvironment : MonoBehaviour
    {
    }

}
