using System;
using UnityEngine;
using ECARules4All.RuleEngine;
using ECAScripts.Utils;

/// <summary>
/// The <b>Vehicle</b> category represents, in an abstract way, all the vehicles that can be placed in the scene.
/// We distinguish them according to the element they support travelling on (air, land, sea, space).
/// </summary>
[ECARules4All("vehicle")]
[RequireComponent(typeof(ECAObject))]
[DisallowMultipleComponent]
    public class Vehicle : MonoBehaviour
    {
        /// <summary>
        /// <b>Speed</b> is the speed of the vehicle.
        /// </summary>
        [StateVariable("speed", ECARules4AllType.Float)] public float speed;
        /// <summary>
        /// <b>On</b> is the state of the vehicle.
        /// </summary>
        [StateVariable("on", ECARules4AllType.Boolean)] public ECABoolean on = new ECABoolean(ECABoolean.BoolType.OFF);
        private Vector3 localForward;
        private ECAObject reference;

        /// <summary>
        /// <b>Starts</b>: the vehicle starts.
        /// </summary>
        [Action(typeof(Vehicle), "starts")]
        public void Starts()
        {
            on.Assign(ECABoolean.BoolType.ON);
        }

        //TODO: verb not present in grammar
        /// <summary>
        /// <b>Steers</b>: the vehicle turns of a given angle.
        /// </summary>
        /// <param name="angle">The angle</param>
        [Action(typeof(Vehicle), "steers-at", typeof(float))]
        public void Steers(float angle)
        {
            Quaternion rotation = gameObject.transform.rotation;
            gameObject.transform.Rotate(rotation.x, rotation.y + angle, rotation.z);
        }
        
        /// <summary>
        /// <b>Accelerates</b>: the vehicle accelerates.
        /// </summary>
        /// <param name="f">The acceleration</param>
        [Action(typeof(Vehicle), "accelerates-by", typeof(float))]
        public void Accelerates(float f)
        {
            speed += f;
        }
        
        /// <summary>
        /// <b>SlowsDown</b>: the vehicle slows down.
        /// </summary>
        /// <param name="f">The deceleration</param>
        [Action(typeof(Vehicle), "slows-by", typeof(float))]
        public void SlowsDown(float f)
        {
            speed -= f;
        }
        
        /// <summary>
        /// <b>Stops</b>: the vehicle stops.
        /// </summary>
        [Action(typeof(Vehicle), "stops")]
        public void Stops()
        {
            on.Assign(ECABoolean.BoolType.OFF);
            speed = 0;
        }

        private void Start()
        {
            localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            reference = GetComponent<ECAObject>();
            reference.p.Assign(gameObject.transform.position);
            reference.r.Assign(gameObject.transform.rotation);
        }

        private void Update()
        {
            if (on)
            {
                transform.Translate( speed * Time.deltaTime * localForward);
                reference.p.Assign(gameObject.transform.position);
                reference.r.Assign(gameObject.transform.rotation);
            }
        }
    }
