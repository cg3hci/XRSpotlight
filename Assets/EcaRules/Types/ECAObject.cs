using System;
using System.Collections;
using ECAScripts.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EcaRules
{
    /// <summary>
    /// <b>ECAObject</b> is the base class for all objects that can be used in the rule engine.
    /// All the other classes in this package inherit from this class or one of its subclasses.
    /// </summary>
    [DisallowMultipleComponent]
    [EcaRules4All("object")]
    public class ECAObject : MonoBehaviour
    {
        /// <summary>
        /// <b> GameCollider </b> is the collider of the object.
        /// </summary>
        private Collider[] gameCollider;
        /// <summary>
        /// <b>GameRender</b> is the renderer of the object.
        /// </summary>
        private Renderer[] gameRenderer;
        /// <summary>
        /// <b>isBusyMoving</b> is a boolean that indicates if the object is moving.
        /// </summary>
        private bool isBusyMoving = false;
        
        /// <summary>
        /// <b>p</b> is the position of the object.
        /// </summary>
        [EcaStateVariable("position", EcaRules4AllType.Position)]
        public EcaPosition p;
        /// <summary>
        /// <b>r</b> is the rotation of the object.
        /// </summary>
        [EcaStateVariable("rotation", EcaRules4AllType.Rotation)]
        public EcaRotation r;
        /// <summary>
        /// <b>isVisible</b> is a boolean that indicates if the object is visible.
        /// If the object is invisible, it will not be rendered but it will still collide with other objects.
        /// </summary>
        [EcaStateVariable("visible", EcaRules4AllType.Boolean)] 
        public ECABoolean isVisible = new ECABoolean(ECABoolean.BoolType.YES);
        /// <summary>
        /// <b>isActive</b> is a boolean that indicates if the object is active and visible.
        /// If the object is inactive, it will not be rendered and it will not collide with other objects.
        /// </summary>
        [EcaStateVariable("active", EcaRules4AllType.Boolean)]
        public ECABoolean isActive = new ECABoolean(ECABoolean.BoolType.YES);
        private Canvas canvas;
        

        private void Start()
        {
            TryGetComponent<Canvas>(out canvas);
            UpdateVisibility();
        }

        /// <summary>
        /// <b>Moves</b> (to) is a method that moves the object to a new position.
        /// </summary>
        /// <param name="newPos">The new position for the object </param>
        [EcaAction(typeof(ECAObject), "moves to", typeof(EcaPosition))]
        public void Moves(EcaPosition newPos)
        {
            float speed = 1.0F;
            Vector3 endMarker = new Vector3(newPos.x, newPos.y, newPos.z);
            StartCoroutine(MoveObject(speed, endMarker));
        }

        /// <summary>
        /// <b>Moves</b> (on) is a method that moves the object to a new position, following a path.
        /// </summary>
        /// <param name="path">The array of positions the object will follow, one after another</param>
        [EcaAction(typeof(ECAObject), "moves on", typeof(EcaPath))]
        public void Moves(EcaPath path)
        {
            StartCoroutine(WaitForOrderedMovement(path));
        }

        private IEnumerator WaitForOrderedMovement(EcaPath path)
        {
            foreach (EcaPosition pos in path.Points)
            {
                while (isBusyMoving)
                {
                    yield return null;
                }

                Moves(pos);
            }
        }

        /// <summary>
        /// <b>Rotates</b> sets the rotation of the object to a new value.
        /// </summary>
        /// <param name="newRot">The new rotation value fo the object. </param>
        [EcaAction(typeof(ECAObject), "rotates around", typeof(EcaRotation))]
        public void Rotates(EcaRotation newRot)
        {
            r.Assign(newRot);
            transform.Rotate(r.x, r.y, r.z);
        }
        
        /// <summary>
        /// <b>Looks</b> sets the rotation of the object in a way that it looks at a target.
        /// </summary>
        /// <param name="o">The target GameObject to look at.</param>
        [EcaAction(typeof(ECAObject), "looks at", typeof(GameObject))]
        public void Looks(GameObject o)
        {
            ECAObject looked = o.GetComponent<ECAObject>();
            Physics.Linecast (transform.position, o.transform.position, out var hit);
            if (o.name == hit.collider.gameObject.name && looked.isActive)
            {
                transform.LookAt(looked.gameObject.transform);
                r.Assign(transform.rotation);
            }
            
        }

        /// <summary>
        /// <b>Shows</b> makes the object visible. It makes it visible if it is not already.
        /// </summary>
        [EcaAction(typeof(ECAObject), "shows")]
        public void Shows()
        {
            isVisible.Assign(ECABoolean.BoolType.YES);
            UpdateVisibility();
        }
        
        /// <summary>
        /// <b>Hides</b> makes the object invisible. It makes it invisible if it is not already.
        /// </summary>
        [EcaAction(typeof(ECAObject), "hides")]
        public void Hides()
        {
            isVisible.Assign(ECABoolean.BoolType.NO);
            UpdateVisibility();
        }
        
        /// <summary>
        /// <b>Activates</b> makes the object active. It makes it active if it is not already. 
        /// </summary>
        [EcaAction(typeof(ECAObject), "activates")]
        public void Activates()
        {
            isActive.Assign(ECABoolean.BoolType.YES);
            UpdateVisibility();
        }
        /// <summary>
        /// <b>Deactivates</b> makes the object inactive. It makes it inactive if it is not already.
        /// </summary>
        [EcaAction(typeof(ECAObject), "deactivates")]
        public void Deactivates()
        {
            isActive.Assign(ECABoolean.BoolType.NO);
            UpdateVisibility();
        }
        
        /// <summary>
        /// <b>ShowsHides</b> sets the visibility of the object, defined by a parameter.
        /// </summary>
        /// <param name="yesNo">The new visibility state for the object. </param>
        [EcaAction(typeof(ECAObject), "changes", "visible", "to", typeof(YesNo))]
        public void ShowsHides(ECABoolean yesNo)
        {
            isVisible = yesNo;
            UpdateVisibility();
        }

        /// <summary>
        /// <b>ActivatesDeactivates</b> sets the active state of the object, defined by a parameter.
        /// </summary>
        /// <param name="yesNo"></param>
        [EcaAction(typeof(ECAObject), "changes", "active", "to", typeof(YesNo))]
        public void ActivatesDeactivates(ECABoolean yesNo)
        {
            isActive = yesNo;
            UpdateVisibility();
        }

        private void Awake()
        {
            gameCollider = this.gameObject.GetComponents<Collider>();
            gameRenderer = this.gameObject.GetComponents<Renderer>();

            if (gameRenderer.Length == 0)
                gameRenderer = this.gameObject.GetComponentsInChildren<Renderer>();
            
            if (gameCollider.Length == 0)
                gameCollider = this.gameObject.GetComponentsInChildren<Collider>();
            
            p = new EcaPosition();
            p.Assign(transform.position);
            r = new EcaRotation();
            r.Assign(transform.rotation);
        }
        
        //TODO: should be private
        public void UpdateVisibility()
        {

            foreach (Collider c in gameCollider)
            {
                c.enabled = isActive;
            }

            foreach (Renderer r in gameRenderer)
            {
                if (!isActive || !isVisible)
                {
                    r.enabled = false;
                }
                else r.enabled = true;
            }
            
            if (canvas != null)
            {
                if (!isActive || !isVisible)
                {
                    canvas.enabled = false;
                }
                else canvas.enabled = true;            
            }
        }
        
        private IEnumerator MoveObject(float speed, Vector3 endMarker)
        {
            isBusyMoving = true;
            Vector3 startMarker = gameObject.transform.position;
            float startTime = Time.time;
            float journeyLength = Vector3.Distance(startMarker, endMarker);
            while (gameObject.transform.position != endMarker)
            {
                float distCovered = (Time.time - startTime) * speed;

                // Fraction of journey completed equals current distance divided by total distance.
                float fractionOfJourney = distCovered / journeyLength;

                // Set our position as a fraction of the distance between the markers.

                gameObject.transform.position = Vector3.Lerp(startMarker, endMarker, fractionOfJourney);
                GetComponent<ECAObject>().p.Assign(gameObject.transform.position);
                yield return null;
            }
            GetComponent<ECAObject>().p.Assign(gameObject.transform.position);
            isBusyMoving = false;
        }

    }
}