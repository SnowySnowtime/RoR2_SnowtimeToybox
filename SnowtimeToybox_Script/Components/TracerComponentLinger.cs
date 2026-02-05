using RoR2;
using UnityEngine;

namespace SnowtimeToybox.Components
{
    public class TracerComponentLinger : MonoBehaviour
    {
        public Tracer tracer;

        public void OnEnable()
        {
            tracer = GetComponent<Tracer>();
        }
        public void LateUpdate()
        {
            tracer.distanceTraveled = Mathf.Min(tracer.distanceTraveled, tracer.totalDistance - 1f);
        }
    }
}