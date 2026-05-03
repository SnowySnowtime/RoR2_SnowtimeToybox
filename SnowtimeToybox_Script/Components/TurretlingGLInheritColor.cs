using IL.RoR2.Projectile;
using RoR2;
using UnityEngine;

namespace SnowtimeToybox.Components
{
    public class TurretlingGLInheritColor : MonoBehaviour
    {
        public Animator animator;
        public GameObject waow;
        public GameObject owner;

        public void Start()
        {
            animator = GetComponent<Animator>();
            //gameObject.GetComponent<ChildLocator>().FindChild("Grenade").gameObject.GetComponent<Animator>().SetFloat("hue", )
        }
        public void FixedUpdate()
        {
            //animator.SetFloat("hue", )
        }
    }
}