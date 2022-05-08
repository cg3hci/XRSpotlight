using System;
using UnityEngine;
using EcaRules;
/// <summary>
/// <b>Firearm</b> is a class that represents a firearm, a firearm can expel bullets.
/// </summary>
[ECARules4All("firearm")]
[RequireComponent(typeof(Weapon))]
[DisallowMultipleComponent]
    public class Firearm : MonoBehaviour
    {
        /// <summary>
        /// <b>ParticleFire</b> is a reference to the particle system that is used to simulate the fire.
        /// </summary>
        public ParticleSystem particleFire;
        /// <summary>
        /// <b>ParticleRecharge</b> is a reference to the particle system that is used to simulate the recharging.
        /// </summary>
        public ParticleSystem particleRecharge;
        private GameObject particleRechargePrefab;
        private GameObject particleFirePrefab;
        /// <summary>
        /// <b>BulletPrefab</b> is a reference to the bullet prefab that is used to instantiate bullets.
        /// </summary>
        public GameObject bulletPrefab;
        
        /// <summary>
        /// <b>Charge</b> is the current charge of the firearm.
        /// </summary>
        [StateVariable("charge", ECARules4AllType.Integer)] public int charge;

        /// <summary>
        /// <b>Recharges</b>: The action of recharging the firearm. It plays the particle system and increases the charge.
        /// </summary>
        /// <param name="charge">The amount of charge</param>
        [Action(typeof(Firearm), "recharges", typeof(int))]
        public void Recharges(int charge)
        {
            if (charge > 0)
            {
                this.charge = charge;

                if (particleRecharge == null)
                {
                    particleRechargePrefab = Instantiate(Resources.Load("Particles/Particle_Recharges"), transform) as GameObject;
                    particleRecharge = particleRechargePrefab.GetComponent<ParticleSystem>();
                }
                else
                {
                    Instantiate(particleRecharge, transform);
                    particleRecharge = transform.Find(particleRecharge.name + "(Clone)").GetComponent<ParticleSystem>();
                }

                particleRecharge.Stop();
                particleRecharge.Play();
            }
        }
        
        /// <summary>
        /// <b>Fires</b>: The action of firing the firearm. It plays the particle system and decreases the charge.
        /// </summary>
        /// <param name="obj">The ECAObject that has been shot</param>
        [Action(typeof(Firearm), "fires", typeof(ECAObject))]
        public void Fires(ECAObject obj)
        {
            if (charge > 0)
            {
                charge -= 1;
                if (bulletPrefab != null)
                {
                    GameObject bulletInstance;
                    bulletInstance = Instantiate(bulletPrefab, transform.position, transform.rotation) as GameObject;
                    bulletInstance.GetComponent<Rigidbody>().AddForce(transform.forward * bulletPrefab.GetComponent<Bullet>().speed);
                }
                
                if (particleFire == null)
                {
                    particleFirePrefab = Instantiate(Resources.Load("Particles/Particle_Fire"), transform) as GameObject;
                    particleFire = particleFirePrefab.GetComponent<ParticleSystem>();
                }
                else
                {
                    Instantiate(particleFire, transform);
                    particleFire = transform.Find(particleFire.name + "(Clone)").GetComponent<ParticleSystem>();
                }
                particleFire.Stop();
                particleFire.Play();
                
                
                if (transform.Find(particleFire.name + "(Clone)"))
                {
                    transform.Find(particleFire.name + "(Clone)").GetComponent<ParticleSystem>().Stop();
                    transform.Find(particleFire.name + "(Clone)").GetComponent<ParticleSystem>().Play();
                }
                else
                {
                    Instantiate(particleFire, transform);
                    transform.Find(particleFire.name + "(Clone)").GetComponent<ParticleSystem>().Play();
                }
            }
        }
        
        /// <summary>
        /// <b>Aims</b>: The action of aiming the firearm.
        /// </summary>
        /// <param name="obj"></param>
        [Action(typeof(Firearm), "aims", typeof(ECAObject))]
        public void Aims(ECAObject obj)
        {
            transform.LookAt(obj.transform);
            this.GetComponent<ECAObject>().p.Assign(transform.position);
            this.GetComponent<ECAObject>().r.Assign(transform.rotation);
        }
        
    }
